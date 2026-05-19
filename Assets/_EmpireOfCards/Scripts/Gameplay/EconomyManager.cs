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

        // --- Runtime State ---
        [Header("Turn Summary (Read Only)")]
        [SerializeField] private int grossIncome;
        [SerializeField] private int totalSalaries;
        [SerializeField] private int taxAmount;
        [SerializeField] private int netIncome;
        [SerializeField] private int totalCustomersThisTurn;

        // --- Sub-systems ---
        private IncomeCalculator incomeCalculator;
        private TaxCalculator taxCalculator;
        private MarketPool marketPool;
        private DebtTracker debtTracker;

        // --- Properties ---
        public int GrossIncome => grossIncome;
        public int TotalSalaries => totalSalaries;
        public int TaxAmount => taxAmount;
        public int NetIncome => netIncome;
        public int TotalCustomersThisTurn => totalCustomersThisTurn;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(GameBalanceData balance, BoardManager board, ComboSystem combo)
        {
            this.balanceData = balance;
            this.boardManager = board;
            this.comboSystem = combo;

            incomeCalculator = new IncomeCalculator(comboSystem);
            taxCalculator = new TaxCalculator(balanceData);
            marketPool = new MarketPool(balanceData);
            debtTracker = new DebtTracker();
        }

        // ----------------------------------------------------------------
        // End of Turn Pipeline
        // ----------------------------------------------------------------

        /// <summary>
        /// Runs the full end-of-turn economy pipeline:
        /// 1. Calculate income   2. Calculate salaries   3. Count accountants
        /// 4. Calculate tax      5. Net = income - salaries - tax
        /// 6. Apply to GameManager   7. Tick investor debt   8. Track customers
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

            // Step 1: Income
            grossIncome = incomeCalculator.CalculateTurnIncome(
                businesses, activeEvent, debtTracker.DebtPercent, debtTracker.TurnsRemaining);
            EventBus.IncomeReceived(grossIncome);

            // Step 2: Salaries
            totalSalaries = CalculateSalaries(businesses);

            // Step 3-4: Tax
            int accountantCount = taxCalculator.CountAccountants(businesses);
            taxAmount = taxCalculator.CalculateTax(grossIncome, accountantCount);

            // Step 5: Net
            netIncome = grossIncome - totalSalaries - taxAmount;

            // Soft cap penalty: -5% income per turn after turn 25 (GDD Section 1.7)
            int currentTurn = gm != null ? gm.CurrentTurn : 0;
            if (currentTurn > Constants.SOFT_CAP_TURN)
            {
                int penaltyTurns = currentTurn - Constants.SOFT_CAP_TURN;
                float penaltyRate = penaltyTurns * Constants.SOFT_CAP_PENALTY;
                penaltyRate = Mathf.Min(penaltyRate, 0.5f); // Cap at 50% max penalty
                netIncome = Mathf.RoundToInt(netIncome * (1f - penaltyRate));
            }

            // Step 6: Apply
            if (netIncome > 0)
                gm.GainMoney(netIncome);
            else if (netIncome < 0)
                gm.SpendMoney(Mathf.Abs(netIncome));

            // Step 7: Tick investor debt
            debtTracker.Tick();

            // Step 8: Track customers attracted per business for evolution
            totalCustomersThisTurn = 0;
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed) continue;
                int bizCustomers = CalculateBusinessCustomers(businesses[i]);
                boardManager.AddCustomersAttracted(i, bizCustomers);
                totalCustomersThisTurn += bizCustomers;
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
            debtTracker.Reset();
        }
    }
}
