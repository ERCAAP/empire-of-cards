using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Economy;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Thin coordinator for the economy pipeline (GDD Section 9).
    /// Delegates to: IncomeCalculator, TaxCalculator, MarketPool, DebtTracker.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        // --- Data Reference ---
        [Header("Balance Data")]
        [SerializeField] private GameBalanceData balanceData;

        // --- Manager References ---
        [Header("References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ComboSystem comboSystem;
        private SlotManager slotManager;

        // --- Runtime State ---
        [Header("Turn Summary (Read Only)")]
        [SerializeField] private int grossIncome;
        [SerializeField] private int totalSalaries;
        [SerializeField] private int taxAmount;
        [SerializeField] private int netIncome;
        [SerializeField] private int totalCustomersThisTurn;

        // --- Platform Rating (GDD v3.0 Section 8) ---
        [Header("Platform Rating")]
        [SerializeField] private float platformRating = Constants.PLATFORM_RATING_DEFAULT;

        // --- Cash Flow (GDD v3.0) ---
        [Header("Cash Flow")]
        [SerializeField] private int cashBalance;
        private int cashCrisisCounter;
        private bool inCashCrisis;

        // --- Ability System Reference ---
        private AbilitySystem abilitySystem;

        // --- Sub-systems ---
        private IncomeCalculator incomeCalculator;
        private TaxCalculator taxCalculator;
        private MarketPool marketPool;
        private DebtTracker debtTracker;
        private SalarySystem salarySystem;
        private InsuranceSystem insuranceSystem;
        private CreditSystem creditSystem;
        private InflationSystem inflationSystem;
        private StockSystem stockSystem;
        private TaxPeriodSystem taxPeriodSystem;

        // --- Properties ---
        public int GrossIncome => grossIncome;
        public int TotalSalaries => totalSalaries;
        public int TaxAmount => taxAmount;
        public int NetIncome => netIncome;
        public int TotalCustomersThisTurn => totalCustomersThisTurn;
        public float PlatformRating => platformRating;
        public int CashBalance => cashBalance;
        public bool IsCashCrisis => inCashCrisis;

        // --- New Economy Sub-system Properties ---
        public SalarySystem SalarySystem => salarySystem;
        public InsuranceSystem InsuranceSystem => insuranceSystem;
        public CreditSystem CreditSystem => creditSystem;
        public InflationSystem InflationSystem => inflationSystem;
        public StockSystem StockSystem => stockSystem;
        public TaxPeriodSystem TaxPeriodSystem => taxPeriodSystem;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(GameBalanceData balance, BoardManager board, ComboSystem combo,
                         AbilitySystem ability = null, SlotManager slots = null)
        {
            this.balanceData = balance;
            this.boardManager = board;
            this.comboSystem = combo;
            this.abilitySystem = ability;
            this.slotManager = slots;

            incomeCalculator = new IncomeCalculator(comboSystem);
            taxCalculator = new TaxCalculator(balanceData);
            marketPool = new MarketPool(balanceData);
            debtTracker = new DebtTracker();
            salarySystem = new SalarySystem();
            insuranceSystem = new InsuranceSystem();
            creditSystem = new CreditSystem();
            inflationSystem = new InflationSystem();
            stockSystem = new StockSystem();
            taxPeriodSystem = new TaxPeriodSystem();
        }

        // ----------------------------------------------------------------
        // End of Turn Pipeline
        // ----------------------------------------------------------------

        /// <summary>
        /// Runs the full end-of-turn economy pipeline:
        /// 1. Calculate income   2. Calculate salaries   3. Count accountants
        /// 4. Calculate tax      5. Net = income - salaries - tax
        /// 6. Apply to GameManager   7. Tick investor debt   8. Track customers
        /// Also builds an IncomeBreakdown and fires the cascade event.
        /// </summary>
        public void ProcessEndOfTurn()
        {
            if (boardManager == null)
            {
                Debug.LogError("[EconomyManager] BoardManager reference is null.");
                return;
            }

            GameManager gm = GameManager.Instance;
            if (gm == null) return;

            IReadOnlyList<ActiveBusiness> businesses = boardManager.PlayerBusinesses;
            CardData activeEvent = boardManager.ActiveEvent;

            // --- Build breakdown for cascade animation ---
            var breakdown = new IncomeBreakdown();

            // Step 1: Income (detailed -- populates per-business steps)
            grossIncome = incomeCalculator.CalculateTurnIncomeDetailed(
                businesses, activeEvent, debtTracker.DebtPercent, debtTracker.TurnsRemaining,
                breakdown.steps);

            // Apply AbilitySystem income multiplier (e.g., Chef "Special Menu", Consultant scaling)
            if (abilitySystem != null && abilitySystem.IncomeMultiplier != 1f)
            {
                int beforeAbility = grossIncome;
                grossIncome = Mathf.RoundToInt(grossIncome * abilitySystem.IncomeMultiplier);
                Debug.Log($"[EconomyManager] AbilitySystem income multiplier applied: x{abilitySystem.IncomeMultiplier} -> gross={grossIncome}");

                int abilityDelta = grossIncome - beforeAbility;
                if (abilityDelta != 0)
                {
                    breakdown.steps.Add(new IncomeStep(
                        $"Ability x{abilitySystem.IncomeMultiplier:F1}",
                        abilityDelta,
                        isMultiplier: true));
                }
            }

            EventBus.IncomeReceived(grossIncome);

            // Step 2: Salaries
            totalSalaries = CalculateSalaries(businesses);
            if (totalSalaries > 0)
            {
                breakdown.steps.Add(new IncomeStep("Salaries", -totalSalaries, isNegative: true));
            }

            // Step 3-4: Tax (skip if AbilitySystem grants tax-free this turn)
            int accountantCount = taxCalculator.CountAccountants(businesses);
            if (abilitySystem != null && abilitySystem.TaxFree)
            {
                taxAmount = 0;
                Debug.Log("[EconomyManager] AbilitySystem: tax-free this turn");
                // Show tax-free as a positive cascade step
                breakdown.steps.Add(new IncomeStep("Tax Free!", 0, isMultiplier: true));
            }
            else
            {
                taxAmount = taxCalculator.CalculateTax(grossIncome, accountantCount);
                if (taxAmount > 0)
                {
                    breakdown.steps.Add(new IncomeStep("Tax", -taxAmount, isNegative: true));
                }
            }

            // Step 5: Net
            netIncome = grossIncome - totalSalaries - taxAmount;

            // Soft cap penalty: -5% income per turn after turn 20 (Constants.SOFT_CAP_TURN)
            int currentTurn = gm != null ? gm.CurrentTurn : 0;
            if (currentTurn > Constants.SOFT_CAP_TURN)
            {
                int penaltyTurns = currentTurn - Constants.SOFT_CAP_TURN;
                float penaltyRate = penaltyTurns * Constants.SOFT_CAP_PENALTY;
                penaltyRate = Mathf.Min(penaltyRate, 0.5f); // Cap at 50% max penalty
                int beforePenalty = netIncome;
                netIncome = Mathf.RoundToInt(netIncome * (1f - penaltyRate));

                int penaltyDelta = netIncome - beforePenalty;
                if (penaltyDelta != 0)
                {
                    breakdown.steps.Add(new IncomeStep(
                        $"Late Penalty -{Mathf.RoundToInt(penaltyRate * 100f)}%",
                        penaltyDelta,
                        isNegative: true));
                }
            }

            // Finalize breakdown and fire cascade event
            breakdown.netIncome = netIncome;
            EventBus.IncomeBreakdownReported(breakdown);

            // Step 6: Apply
            if (netIncome > 0)
                gm.GainMoney(netIncome);
            else if (netIncome < 0)
                gm.SpendMoney(Mathf.Abs(netIncome));

            // Step 7: Tick investor debt
            debtTracker.Tick();

            // Step 7b: Tick credit interest (GDD Section 5.7)
            int creditInterest = creditSystem.TickCredits();
            if (creditInterest > 0)
            {
                gm.SpendMoney(creditInterest);
                breakdown.steps.Add(new IncomeStep("Credit Interest", -creditInterest, isNegative: true));
                Debug.Log($"[EconomyManager] Credit interest paid: {creditInterest}");
            }

            // Step 7c: Inflation check (GDD Section 5.9)
            if (inflationSystem.ShouldTriggerInflation(currentTurn))
            {
                inflationSystem.TriggerInflation(currentTurn);
            }

            // Step 7d: Stock spoilage (GDD Section 9.1-9.4)
            if (boardManager != null && boardManager.PlayerBusinesses.Count > 0)
            {
                int totalSpoilage = 0;
                foreach (var biz in boardManager.PlayerBusinesses)
                {
                    if (biz.isClosed || biz.businessCard == null) continue;
                    int spoilage = stockSystem.ApplySpoilage(
                        biz.businessCard.incomePerTurn,
                        biz.businessCard.ventureType,
                        currentTurn);
                    totalSpoilage += spoilage;
                }
                if (totalSpoilage > 0)
                {
                    gm.SpendMoney(totalSpoilage);
                    breakdown.steps.Add(new IncomeStep("Stock Spoilage", -totalSpoilage, isNegative: true));
                }
            }

            // Step 7e: Tax period check (GDD Section 5.8)
            taxPeriodSystem.TrackProfit(netIncome);
            if (taxPeriodSystem.IsTaxPeriod(currentTurn))
            {
                var taxResult = taxPeriodSystem.ProcessTaxPeriod(currentTurn, gm.PlayerMoney);
                if (taxResult.amountPaid > 0)
                {
                    gm.SpendMoney(taxResult.amountPaid);
                    breakdown.steps.Add(new IncomeStep("Tax Period", -taxResult.amountPaid, isNegative: true));
                }
            }
            else
            {
                // Tick any existing tax debt interest
                int taxDebtInterest = taxPeriodSystem.TickTaxDebt();
                if (taxDebtInterest > 0)
                {
                    gm.SpendMoney(taxDebtInterest);
                    breakdown.steps.Add(new IncomeStep("Tax Debt Interest", -taxDebtInterest, isNegative: true));
                }
            }

            // Step 8: Track customers attracted per business for evolution
            totalCustomersThisTurn = 0;
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed) continue;
                int bizCustomers = CalculateBusinessCustomers(businesses[i]);
                boardManager.AddCustomersAttracted(i, bizCustomers);
                totalCustomersThisTurn += bizCustomers;
            }

            // Apply AbilitySystem customer modifiers (multiplier + extra)
            if (abilitySystem != null)
            {
                if (abilitySystem.CustomerMultiplier != 1f)
                {
                    totalCustomersThisTurn = Mathf.RoundToInt(totalCustomersThisTurn * abilitySystem.CustomerMultiplier);
                    Debug.Log($"[EconomyManager] AbilitySystem customer multiplier applied: x{abilitySystem.CustomerMultiplier}");
                }
                if (abilitySystem.ExtraCustomers > 0)
                {
                    totalCustomersThisTurn += abilitySystem.ExtraCustomers;
                    Debug.Log($"[EconomyManager] AbilitySystem extra customers applied: +{abilitySystem.ExtraCustomers}");
                }
            }
        }

        // ----------------------------------------------------------------
        // Delegated Public API (preserves existing call sites)
        // ----------------------------------------------------------------

        public int CalculateTurnIncome(IReadOnlyList<ActiveBusiness> businesses, CardData activeEvent)
        {
            grossIncome = incomeCalculator.CalculateTurnIncome(
                businesses, activeEvent, debtTracker.DebtPercent, debtTracker.TurnsRemaining);
            EventBus.IncomeReceived(grossIncome);
            return grossIncome;
        }

        public int CalculateTax(int income, int accountantCount)
        {
            taxAmount = taxCalculator.CalculateTax(income, accountantCount);
            return taxAmount;
        }

        public int GetMarketPool(int currentTurn)
        {
            return marketPool.GetMarketPool(currentTurn);
        }

        public void StartInvestorDebt(int duration, float percent)
        {
            debtTracker.StartDebt(duration, percent);
        }

        // ----------------------------------------------------------------
        // Salary System (GDD Section 5.5)
        // ----------------------------------------------------------------

        public void RequestSalaryChoice()
        {
            IReadOnlyList<ActiveBusiness> businesses = boardManager.PlayerBusinesses;
            int salaries = CalculateSalaries(businesses);
            if (salaries > 0)
            {
                EventBus.SalaryChoiceRequired(salaries);
            }
        }

        public SalaryResult ProcessSalaryChoice(SalaryChoice choice)
        {
            IReadOnlyList<ActiveBusiness> businesses = boardManager.PlayerBusinesses;
            int salaries = CalculateSalaries(businesses);
            GameManager gm = GameManager.Instance;
            int money = gm != null ? gm.PlayerMoney : 0;
            return salarySystem.ProcessSalary(choice, salaries, money);
        }

        // ----------------------------------------------------------------
        // Credit System (GDD Section 5.7)
        // ----------------------------------------------------------------

        public bool CanTakeCredit(CreditType type)
        {
            GameManager gm = GameManager.Instance;
            CompanyTier tier = CompanyTier.Trader;
            if (gm != null && gm.CompanyTierSystem != null)
                tier = gm.CompanyTierSystem.CurrentTier;
            return creditSystem.CanTakeCredit(type, tier);
        }

        public void TakeCredit(CreditType type)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null) return;

            CompanyTier tier = CompanyTier.Trader;
            if (gm.CompanyTierSystem != null)
                tier = gm.CompanyTierSystem.CurrentTier;

            if (!creditSystem.CanTakeCredit(type, tier))
            {
                Debug.LogWarning($"[EconomyManager] Cannot take {type} credit at tier {tier}");
                return;
            }

            creditSystem.TakeCredit(type);
            int amount = creditSystem.GetCreditAmount(type);
            gm.GainMoney(amount);
        }

        public int GetTotalCreditDebt()
        {
            return creditSystem.TotalDebt;
        }

        // ----------------------------------------------------------------
        // Sell Price
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the sell price of a card: buyCost x 40% (GDD).
        /// </summary>
        public int GetSellPrice(CardData card)
        {
            if (card == null) return 0;
            float rate = balanceData != null ? balanceData.sellRate : Constants.SELL_RATE;
            return Mathf.Max(1, Mathf.RoundToInt(card.buyCost * rate));
        }

        // ----------------------------------------------------------------
        // Helpers kept here (small, used only by ProcessEndOfTurn)
        // ----------------------------------------------------------------

        private int CalculateSalaries(IReadOnlyList<ActiveBusiness> businesses)
        {
            int total = 0;
            foreach (var business in businesses)
            {
                if (business.isClosed) continue;
                foreach (var employee in business.employees)
                {
                    if (employee != null)
                        total += employee.salaryPerTurn;
                }
            }
            return total;
        }

        private int CalculateBusinessCustomers(ActiveBusiness business)
        {
            if (business.businessCard == null) return 0;
            CardData card = business.businessCard;

            if (card.activationDelay > 0 && business.turnsActive < card.activationDelay)
                return 0;

            int customers = card.customersPerTurn;
            foreach (var emp in business.employees)
            {
                if (emp == null) continue;
                bool hasSynergy = CheckSynergy(emp, card);
                customers += hasSynergy ? emp.synergyCustomerBonus : emp.customerBonus;
            }
            return customers;
        }

        private bool CheckSynergy(CardData employee, CardData business)
        {
            if (business.tags == null) return false;
            foreach (var tag in business.tags)
            {
                if (tag == employee.synergyTag)
                    return true;
            }
            return false;
        }

        // ----------------------------------------------------------------
        // Slot-based Market Share (GDD v3.0 Section 7)
        // ----------------------------------------------------------------

        /// <summary>
        /// Aggregates qualityScore, priceScore, and serviceSpeedScore from all
        /// occupied Operation slots via SlotManager. Falls back to BoardManager
        /// businesses if SlotManager is not wired.
        /// Returns (totalQuality, totalPrice, totalSpeed, occupiedCount).
        /// </summary>
        public (float quality, float price, float speed, int count) GetOperationSlotScores()
        {
            float totalQuality = 0f;
            float totalPrice = 0f;
            float totalSpeed = 0f;
            int count = 0;

            if (slotManager != null)
            {
                var ops = slotManager.OperationSlots;
                for (int i = 0; i < ops.Count; i++)
                {
                    CardData card = ops[i];
                    if (card == null) continue;
                    totalQuality += card.qualityScore;
                    totalPrice += card.priceScore;
                    totalSpeed += card.serviceSpeedScore;
                    count++;
                }
            }
            else if (boardManager != null)
            {
                var businesses = boardManager.PlayerBusinesses;
                for (int i = 0; i < businesses.Count; i++)
                {
                    if (businesses[i].isClosed || businesses[i].businessCard == null) continue;
                    CardData card = businesses[i].businessCard;
                    totalQuality += card.qualityScore;
                    totalPrice += card.priceScore;
                    totalSpeed += card.serviceSpeedScore;
                    count++;
                }
            }

            return (totalQuality, totalPrice, totalSpeed, count);
        }

        /// <summary>
        /// Calculates the player's fractional market share (0.0-1.0) using
        /// SlotManager v2 scores and the MarketPool weight formula.
        /// </summary>
        public float CalculatePlayerMarketShare(float marketingScore, float loyaltyScore)
        {
            var (quality, price, speed, count) = GetOperationSlotScores();
            if (count == 0) return 0f;

            float avgQuality = quality / count;
            float avgPrice = price / count;
            float avgSpeed = speed / count;

            return marketPool.CalculatePlayerMarketShare(
                avgQuality, avgPrice, platformRating, marketingScore, avgSpeed, loyaltyScore);
        }

        // ----------------------------------------------------------------
        // Platform Rating (GDD v3.0 Section 8)
        // ----------------------------------------------------------------

        /// <summary>
        /// Adjusts platform rating by delta, clamped to [1.0, 5.0].
        /// Fires EventBus.PlatformRatingChanged.
        /// </summary>
        public void ModifyPlatformRating(float delta)
        {
            platformRating = UnityEngine.Mathf.Clamp(
                platformRating + delta,
                Constants.PLATFORM_RATING_MIN,
                Constants.PLATFORM_RATING_MAX);
            EventBus.PlatformRatingChanged(platformRating);
        }

        /// <summary>
        /// Returns a customer multiplier based on platform rating bands.
        /// 4.5+ = +25%, 4.0+ = +15%, 3.5+ = +5%, 3.0+ = 0%, below 3.0 = -10%.
        /// </summary>
        public float GetRatingCustomerMultiplier()
        {
            if (platformRating >= 4.5f) return 1.25f;
            if (platformRating >= 4.0f) return 1.15f;
            if (platformRating >= 3.5f) return 1.05f;
            if (platformRating >= 3.0f) return 1.00f;
            return 0.90f; // Below 3.0 — poor reputation
        }

        /// <summary>
        /// Decay platform rating by Constants.PLATFORM_RATING_DECAY_PER_TURN each turn
        /// if no marketing slot is active. Called from ResolvePhase.
        /// </summary>
        public void DecayPlatformRating()
        {
            ModifyPlatformRating(-Constants.PLATFORM_RATING_DECAY_PER_TURN);
        }

        // ----------------------------------------------------------------
        // Cash Flow
        // ----------------------------------------------------------------

        /// <summary>
        /// Updates the tracked cash balance and fires an event.
        /// Called by GameManager after GainMoney/SpendMoney operations.
        /// </summary>
        public void UpdateCashBalance(int newBalance)
        {
            cashBalance = newBalance;
            EventBus.CashBalanceChanged(cashBalance);

            if (cashBalance < 0)
            {
                cashCrisisCounter++;
                if (!inCashCrisis && cashCrisisCounter >= Constants.CASH_CRISIS_THRESHOLD)
                {
                    inCashCrisis = true;
                    EventBus.CashCrisisStarted();
                }
            }
            else if (inCashCrisis)
            {
                inCashCrisis = false;
                cashCrisisCounter = 0;
                EventBus.CashCrisisResolved();
            }
            else
            {
                cashCrisisCounter = 0;
            }
        }

        /// <summary>
        /// Resets economy state for a new run.
        /// </summary>
        public void Reset()
        {
            grossIncome = 0;
            totalSalaries = 0;
            taxAmount = 0;
            netIncome = 0;
            totalCustomersThisTurn = 0;
            platformRating = Constants.PLATFORM_RATING_DEFAULT;
            cashBalance = 0;
            cashCrisisCounter = 0;
            inCashCrisis = false;
            debtTracker.Reset();
            creditSystem?.Reset();
            inflationSystem?.Reset();
            taxPeriodSystem?.Reset();
        }
    }
}
