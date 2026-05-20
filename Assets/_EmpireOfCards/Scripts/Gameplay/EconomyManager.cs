using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Economy;

namespace EmpireOfCards.Gameplay
{
    public class EconomyManager : MonoBehaviour
    {
        [Header("Balance Data")]
        [SerializeField] private GameBalanceData balanceData;

        [Header("References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ComboSystem comboSystem;
        private SlotManager slotManager;

        [Header("Turn Summary")]
        [SerializeField] private int grossIncome;
        [SerializeField] private int totalSalaries;
        [SerializeField] private int taxAmount;
        [SerializeField] private int netIncome;
        [SerializeField] private int totalCustomersThisTurn;

        [Header("Venture Snapshot")]
        [SerializeField] private VentureBoardSnapshot snapshot = new VentureBoardSnapshot();
        [SerializeField] private BoardPressureType currentPressure;

        private VentureEconomyProfile _activeProfile;
        private AbilitySystem abilitySystem;

        private SalarySystem salarySystem;
        private InsuranceSystem insuranceSystem;
        private CreditSystem creditSystem;
        private InflationSystem inflationSystem;
        private StockSystem stockSystem;
        private TaxPeriodSystem taxPeriodSystem;

        private float _investorDebtPercent;
        private int _investorDebtTurns;
        private bool _cashCrisis;

        public int GrossIncome => grossIncome;
        public int TotalSalaries => totalSalaries;
        public int TaxAmount => taxAmount;
        public int NetIncome => netIncome;
        public int TotalCustomersThisTurn => totalCustomersThisTurn;
        public float PlatformRating => snapshot != null ? snapshot.rating : Constants.PLATFORM_RATING_DEFAULT;
        public int CashBalance => snapshot != null ? Mathf.RoundToInt(snapshot.cash) : 0;
        public bool IsCashCrisis => _cashCrisis;
        public SalarySystem SalarySystem => salarySystem;
        public InsuranceSystem InsuranceSystem => insuranceSystem;
        public CreditSystem CreditSystem => creditSystem;
        public InflationSystem InflationSystem => inflationSystem;
        public StockSystem StockSystem => stockSystem;
        public TaxPeriodSystem TaxPeriodSystem => taxPeriodSystem;
        public VentureBoardSnapshot Snapshot => snapshot;
        public BoardPressureType CurrentPressure => currentPressure;

        public void Init(GameBalanceData balance, BoardManager board, ComboSystem combo,
            AbilitySystem ability = null, SlotManager slots = null)
        {
            balanceData = balance;
            boardManager = board;
            comboSystem = combo;
            abilitySystem = ability;
            slotManager = slots;

            salarySystem = new SalarySystem();
            insuranceSystem = new InsuranceSystem();
            creditSystem = new CreditSystem();
            inflationSystem = new InflationSystem();
            stockSystem = new StockSystem();
            taxPeriodSystem = new TaxPeriodSystem();
        }

        public void SetActiveProfile(VentureEconomyProfile profile)
        {
            _activeProfile = profile;
            snapshot = new VentureBoardSnapshot
            {
                ventureType = profile != null ? profile.ventureType : VentureType.FastFood,
                cash = profile != null ? profile.startingCash : Constants.STARTING_MONEY,
                demand = profile != null ? profile.startingDemand : 2f,
                capacity = profile != null ? profile.startingCapacity : 3f,
                quality = profile != null ? profile.startingQuality : 3f,
                rating = profile != null ? profile.startingRating : Constants.PLATFORM_RATING_DEFAULT,
                staffStability = profile != null ? profile.startingStaffStability : 6f,
                legalRisk = profile != null ? profile.startingLegalRisk : 0f,
                marketShare = profile != null ? profile.startingMarketShare : 10f,
                derivedMetrics = BuildDerivedMetrics(profile),
                activeCrisisTags = System.Array.Empty<string>()
            };

            EventBus.PlatformRatingChanged(snapshot.rating);
            EventBus.LegalRiskUpdated(Mathf.RoundToInt(snapshot.legalRisk));
            EventBus.CashBalanceChanged(Mathf.RoundToInt(snapshot.cash));
        }

        public void ProcessEndOfTurn()
        {
            var gm = GameManager.Instance;
            if (gm == null || boardManager == null || slotManager == null || _activeProfile == null)
                return;

            var operations = boardManager.GetCardsInSlotType(SlotType.Operation);
            var staff = boardManager.GetCardsInSlotType(SlotType.Staff);
            var marketing = boardManager.GetCardsInSlotType(SlotType.Marketing);
            var suppliers = boardManager.GetCardsInSlotType(SlotType.Supplier);
            var temp = boardManager.GetCardsInSlotType(SlotType.TempEffect);

            float opDemand = Sum(operations, c => c.demandDelta + c.customersPerTurn * 0.2f);
            float marketingDemand = Sum(marketing, c => c.demandDelta);
            float organicDemand = Mathf.Max(0f, (snapshot.rating - 3f) * _activeProfile.ratingToOrganicDemandWeight);
            float tempDemand = Sum(temp, c => c.demandDelta);

            snapshot.demand = Mathf.Max(0f, _activeProfile.baseDemand + opDemand + marketingDemand + organicDemand + tempDemand);
            snapshot.capacity = Mathf.Max(1f,
                _activeProfile.startingCapacity +
                Sum(operations, c => c.capacityDelta) +
                Sum(staff, c => c.capacityDelta) +
                Sum(suppliers, c => c.capacityDelta) +
                Sum(temp, c => c.capacityDelta));

            snapshot.quality = Mathf.Clamp(
                _activeProfile.startingQuality +
                Sum(operations, c => c.qualityDelta) +
                Sum(staff, c => c.qualityDelta) +
                Sum(suppliers, c => c.qualityDelta) +
                Sum(temp, c => c.qualityDelta),
                0f, 10f);

            snapshot.staffStability = Mathf.Clamp(
                _activeProfile.startingStaffStability +
                Sum(staff, c => c.staffStabilityDelta) +
                Sum(temp, c => c.staffStabilityDelta) -
                Mathf.Max(0f, marketing.Count - staff.Count) * 0.4f,
                0f, 10f);

            snapshot.legalRisk = Mathf.Clamp(
                snapshot.legalRisk - _activeProfile.legalRiskDecayPerTurn +
                Sum(suppliers, c => c.legalRiskDeltaPerTurn) +
                Sum(marketing, c => c.legalRiskDeltaPerTurn) +
                Sum(temp, c => c.legalRiskDeltaPerTurn),
                0f, Constants.LEGAL_RISK_MAX);

            float servedDemand = Mathf.Min(snapshot.demand, snapshot.capacity);
            float overload = Mathf.Max(0f, snapshot.demand - snapshot.capacity);
            float ratingDelta = ((snapshot.quality - 5f) * _activeProfile.qualityToRatingWeight)
                - (overload * _activeProfile.capacityPenaltyMultiplier)
                - Mathf.Max(0f, 4f - snapshot.staffStability) * _activeProfile.staffInstabilityPenalty
                + Sum(marketing, c => c.ratingDeltaPerTurn)
                + Sum(temp, c => c.ratingDeltaPerTurn);

            snapshot.rating = Mathf.Clamp(snapshot.rating + ratingDelta, _activeProfile.minRating, _activeProfile.maxRating);

            grossIncome = Mathf.RoundToInt(
                servedDemand * _activeProfile.baseRevenuePerDemand +
                Sum(operations, c => c.cashDeltaPerTurn) +
                Sum(marketing, c => c.cashDeltaPerTurn) +
                Sum(suppliers, c => c.cashDeltaPerTurn) +
                Sum(temp, c => c.cashDeltaPerTurn));

            if (_investorDebtTurns > 0)
            {
                grossIncome = Mathf.RoundToInt(grossIncome * (1f - _investorDebtPercent));
                _investorDebtTurns--;
            }

            totalSalaries = Mathf.RoundToInt(Sum(staff, c => Mathf.Max(0f, c.upkeepCostPerTurn > 0f ? c.upkeepCostPerTurn : c.salaryPerTurn)));
            int upkeepCosts = Mathf.RoundToInt(
                Sum(marketing, c => c.upkeepCostPerTurn) +
                Sum(suppliers, c => c.upkeepCostPerTurn) +
                Sum(temp, c => c.upkeepCostPerTurn));
            taxAmount = grossIncome > 0 ? Mathf.RoundToInt(grossIncome * (_activeProfile.ventureType == VentureType.TechApp ? 0.08f : 0.10f)) : 0;
            netIncome = grossIncome - totalSalaries - upkeepCosts - taxAmount;

            if (abilitySystem != null && abilitySystem.IncomeMultiplier != 1f)
                netIncome = Mathf.RoundToInt(netIncome * abilitySystem.IncomeMultiplier);

            snapshot.cash += netIncome;
            totalCustomersThisTurn = Mathf.RoundToInt(servedDemand * 6f);

            snapshot.marketShare = Mathf.Clamp(
                snapshot.marketShare +
                ((snapshot.rating - 3f) * 2f) +
                ((snapshot.quality - 5f) * 1.5f) +
                (servedDemand - overload) * 0.5f,
                0f, 100f);

            UpdateDerivedMetrics(servedDemand, overload, suppliers.Count, marketing.Count, temp);
            UpdatePressure(overload, marketing.Count, staff.Count);
            UpdateCashCrisis();

            gm.SetPlayerCustomers(Mathf.RoundToInt(snapshot.marketShare));
            EventBus.PlatformRatingChanged(snapshot.rating);
            EventBus.LegalRiskUpdated(Mathf.RoundToInt(snapshot.legalRisk));
            EventBus.CashBalanceChanged(Mathf.RoundToInt(snapshot.cash));
            EventBus.MarketShareUpdated(Mathf.RoundToInt(snapshot.marketShare), gm.RivalCustomers);

            if (netIncome > 0)
                gm.GainMoney(netIncome);
            else if (netIncome < 0)
                gm.AdjustMoney(netIncome);
        }

        public int CalculateTurnIncome(IReadOnlyList<ActiveBusiness> businesses, CardData activeEvent)
        {
            return grossIncome;
        }

        public int CalculateTax(int income, int accountantCount)
        {
            return income > 0 ? Mathf.RoundToInt(income * 0.10f) : 0;
        }

        public int GetMarketPool(int currentTurn)
        {
            return balanceData != null ? balanceData.GetMarketPool(currentTurn) : Constants.BASE_MARKET_CUSTOMERS;
        }

        public void StartInvestorDebt(int duration, float percent)
        {
            _investorDebtTurns = Mathf.Max(0, duration);
            _investorDebtPercent = Mathf.Clamp01(percent);
        }

        public void RequestSalaryChoice()
        {
            EventBus.SalaryChoiceRequired(totalSalaries);
        }

        public SalaryResult ProcessSalaryChoice(SalaryChoice choice)
        {
            if (salarySystem == null)
                salarySystem = new SalarySystem();
            return salarySystem.ProcessSalary(choice, totalSalaries, CashBalance);
        }

        public bool CanTakeCredit(CreditType type) => true;

        public void TakeCredit(CreditType type)
        {
            int amount = type switch
            {
                CreditType.SmallBusiness => 100,
                CreditType.Medium => 180,
                CreditType.LargeInvestment => 280,
                CreditType.Emergency => 140,
                _ => 100
            };
            snapshot.cash += amount;
            EventBus.CreditTaken(type, amount);
            EventBus.CashBalanceChanged(Mathf.RoundToInt(snapshot.cash));
        }

        public int GetTotalCreditDebt() => 0;

        public int GetSellPrice(CardData card)
        {
            if (card == null) return 0;
            return Mathf.RoundToInt(card.buyCost * Constants.SELL_RATE);
        }

        public (float quality, float price, float speed, int count) GetOperationSlotScores()
        {
            var ops = boardManager != null ? boardManager.GetCardsInSlotType(SlotType.Operation) : System.Array.Empty<CardData>();
            return (
                Average(ops, c => c.qualityScore, 5f),
                Average(ops, c => c.priceScore, 5f),
                Average(ops, c => c.serviceSpeedScore, 5f),
                ops.Count
            );
        }

        public float CalculatePlayerMarketShare(float marketingScore, float loyaltyScore)
        {
            return Mathf.Clamp01(snapshot.marketShare / 100f);
        }

        public void ModifyPlatformRating(float delta)
        {
            if (_activeProfile == null) return;
            snapshot.rating = Mathf.Clamp(snapshot.rating + delta, _activeProfile.minRating, _activeProfile.maxRating);
            EventBus.PlatformRatingChanged(snapshot.rating);
        }

        public float GetRatingCustomerMultiplier()
        {
            return 1f + Mathf.Max(0f, snapshot.rating - 3f) * 0.1f;
        }

        public void DecayPlatformRating()
        {
            ModifyPlatformRating(-Constants.PLATFORM_RATING_DECAY_PER_TURN);
        }

        public void UpdateCashBalance(int newBalance)
        {
            snapshot.cash = newBalance;
            EventBus.CashBalanceChanged(newBalance);
        }

        public void Reset()
        {
            grossIncome = 0;
            totalSalaries = 0;
            taxAmount = 0;
            netIncome = 0;
            totalCustomersThisTurn = 0;
            _investorDebtPercent = 0f;
            _investorDebtTurns = 0;
            _cashCrisis = false;
            currentPressure = BoardPressureType.None;
            if (_activeProfile != null)
                SetActiveProfile(_activeProfile);
        }

        private void UpdateDerivedMetrics(float servedDemand, float overload, int supplierCount, int marketingCount, IReadOnlyList<CardData> temp)
        {
            if (snapshot.derivedMetrics == null || snapshot.derivedMetrics.Length == 0)
                return;

            foreach (var metric in snapshot.derivedMetrics)
            {
                switch (metric.id)
                {
                    case "ingredient_quality":
                    case "bean_quality":
                    case "stock_health":
                        metric.value = Mathf.Clamp(snapshot.quality + supplierCount * 0.4f, 0f, 10f);
                        break;
                    case "service_speed":
                    case "consistency":
                    case "stability":
                        metric.value = Mathf.Clamp(snapshot.capacity - overload * 0.5f, 0f, 10f);
                        break;
                    case "hygiene":
                    case "ambience":
                    case "local_loyalty":
                        metric.value = Mathf.Clamp(snapshot.staffStability + snapshot.rating * 0.5f, 0f, 10f);
                        break;
                    case "churn":
                    case "return_pressure":
                    case "spoilage":
                        metric.value = Mathf.Clamp(10f - snapshot.rating - snapshot.quality * 0.4f, 0f, 10f);
                        break;
                    case "infra_cost":
                    case "credit_ledger":
                        metric.value = Mathf.Clamp(marketingCount + temp.Count, 0f, 10f);
                        break;
                }
            }
        }

        private void UpdatePressure(float overload, int marketingCount, int staffCount)
        {
            if (snapshot.legalRisk >= Constants.LEGAL_RISK_DANGER_THRESHOLD)
            {
                currentPressure = BoardPressureType.HighLegalRisk;
                return;
            }

            if (snapshot.rating <= 3f)
            {
                currentPressure = BoardPressureType.LowRating;
                return;
            }

            if (snapshot.cash <= 80f)
            {
                currentPressure = BoardPressureType.LowCash;
                return;
            }

            if (overload > 1.5f || snapshot.capacity < snapshot.demand)
            {
                currentPressure = BoardPressureType.CapacityShortfall;
                return;
            }

            if (snapshot.quality < 4f)
            {
                currentPressure = BoardPressureType.WeakQuality;
                return;
            }

            if (marketingCount > staffCount + 1 || snapshot.demand < 3f)
            {
                currentPressure = BoardPressureType.LowDemand;
                return;
            }

            if (snapshot.staffStability < 4f)
            {
                currentPressure = BoardPressureType.StaffInstability;
                return;
            }

            currentPressure = BoardPressureType.None;
        }

        private void UpdateCashCrisis()
        {
            bool crisisNow = snapshot.cash < 0f;
            if (crisisNow && !_cashCrisis)
            {
                _cashCrisis = true;
                EventBus.CashCrisisStarted();
            }
            else if (!crisisNow && _cashCrisis)
            {
                _cashCrisis = false;
                EventBus.CashCrisisResolved();
            }
        }

        private static float Sum(IReadOnlyList<CardData> cards, System.Func<CardData, float> selector)
        {
            float total = 0f;
            for (int i = 0; i < cards.Count; i++)
                total += selector(cards[i]);
            return total;
        }

        private static float Average(IReadOnlyList<CardData> cards, System.Func<CardData, float> selector, float fallback)
        {
            if (cards == null || cards.Count == 0)
                return fallback;

            return Sum(cards, selector) / cards.Count;
        }

        private static DerivedMetricValue[] BuildDerivedMetrics(VentureEconomyProfile profile)
        {
            if (profile == null || profile.derivedMetrics == null)
                return System.Array.Empty<DerivedMetricValue>();

            var values = new DerivedMetricValue[profile.derivedMetrics.Length];
            for (int i = 0; i < profile.derivedMetrics.Length; i++)
            {
                values[i] = new DerivedMetricValue
                {
                    id = profile.derivedMetrics[i].id,
                    value = profile.derivedMetrics[i].startingValue
                };
            }
            return values;
        }
    }
}
