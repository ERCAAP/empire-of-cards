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
        [SerializeField] private TurnBriefData currentBrief = new TurnBriefData();
        [SerializeField] private TurnReportData lastReport = new TurnReportData();

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
        private float _rivalPressureImpact;
        private string _rivalPressureStyle = "balanced";

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
        public TurnBriefData CurrentBrief => currentBrief;
        public TurnReportData LastReport => lastReport;
        public float RivalPressureImpact => _rivalPressureImpact;
        public string RivalPressureStyle => _rivalPressureStyle;

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
            UpdatePressure(0f, 0, 0);
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
            float previousCash = snapshot.cash;
            float previousRating = snapshot.rating;
            float previousMarketShare = snapshot.marketShare;

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

            totalCustomersThisTurn = Mathf.RoundToInt(servedDemand * 6f);

            snapshot.marketShare = Mathf.Clamp(
                snapshot.marketShare +
                ((snapshot.rating - 3f) * 2f) +
                ((snapshot.quality - 5f) * 1.5f) +
                (servedDemand - overload) * 0.5f -
                (_rivalPressureImpact * 1.15f),
                0f, 100f);

            UpdateDerivedMetrics(servedDemand, overload, suppliers.Count, marketing.Count, temp);
            UpdatePressure(overload, marketing.Count, staff.Count);

            gm.SetPlayerCustomers(Mathf.RoundToInt(snapshot.marketShare));
            if (netIncome > 0)
                gm.GainMoney(netIncome);
            else if (netIncome < 0)
                gm.AdjustMoney(netIncome);

            snapshot.cash = gm.PlayerMoney;
            snapshot.activeCrisisTags = CollectActiveCrisisTags(temp);
            UpdateCashCrisis();

            EventBus.PlatformRatingChanged(snapshot.rating);
            EventBus.LegalRiskUpdated(Mathf.RoundToInt(snapshot.legalRisk));
            EventBus.CashBalanceChanged(Mathf.RoundToInt(snapshot.cash));
            EventBus.MarketShareUpdated(Mathf.RoundToInt(snapshot.marketShare), gm.RivalCustomers);
            EventBus.OrganicCustomersGained(Mathf.RoundToInt(organicDemand * 4f));
            EventBus.IncomeBreakdownReported(BuildIncomeBreakdown(grossIncome, totalSalaries, upkeepCosts, taxAmount, netIncome));
            lastReport = BuildTurnReport(previousCash, previousRating, previousMarketShare, overload, grossIncome, totalSalaries, upkeepCosts, taxAmount);
            EventBus.TurnReportGenerated(lastReport);
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

        public void SyncCashFromResources(int newBalance)
        {
            if (snapshot == null)
                snapshot = new VentureBoardSnapshot();

            snapshot.cash = newBalance;
            EventBus.CashBalanceChanged(newBalance);
        }

        public void RegisterRivalPressure(float pressureImpact, string pressureStyle)
        {
            _rivalPressureImpact = Mathf.Max(0f, pressureImpact);
            if (!string.IsNullOrWhiteSpace(pressureStyle))
                _rivalPressureStyle = pressureStyle;
        }

        public TurnBriefData GenerateTurnBrief(int currentTurn)
        {
            currentBrief = new TurnBriefData
            {
                currentTurn = currentTurn,
                pressure = currentPressure,
                headline = GetBriefHeadline(currentPressure),
                detail = GetBriefDetail(currentPressure)
            };

            EventBus.TurnBriefGenerated(currentBrief);
            return currentBrief;
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
            _rivalPressureImpact = 0f;
            _rivalPressureStyle = "balanced";
            if (_activeProfile != null)
                SetActiveProfile(_activeProfile);
        }

        public void RestoreSnapshot(VentureBoardSnapshot restored)
        {
            if (restored == null)
                return;

            snapshot = restored;
            EventBus.PlatformRatingChanged(snapshot.rating);
            EventBus.LegalRiskUpdated(Mathf.RoundToInt(snapshot.legalRisk));
            EventBus.CashBalanceChanged(Mathf.RoundToInt(snapshot.cash));
            UpdatePressure(Mathf.Max(0f, snapshot.demand - snapshot.capacity), 0, 0);
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

        private IncomeBreakdown BuildIncomeBreakdown(int gross, int salaries, int upkeep, int tax, int net)
        {
            var breakdown = new IncomeBreakdown();
            breakdown.steps.Add(new IncomeStep("Revenue", gross));
            if (salaries > 0)
                breakdown.steps.Add(new IncomeStep("Salaries", salaries, false, true));
            if (upkeep > 0)
                breakdown.steps.Add(new IncomeStep("Upkeep", upkeep, false, true));
            if (tax > 0)
                breakdown.steps.Add(new IncomeStep("Tax", tax, false, true));
            breakdown.netIncome = net;
            return breakdown;
        }

        private TurnReportData BuildTurnReport(
            float previousCash,
            float previousRating,
            float previousMarketShare,
            float overload,
            int gross,
            int salaries,
            int upkeep,
            int tax)
        {
            var report = new TurnReportData
            {
                headline = BuildReportHeadline(overload),
                summary = $"Net {(netIncome >= 0 ? "+" : "")}{netIncome} | Rating {(snapshot.rating - previousRating >= 0f ? "+" : "")}{(snapshot.rating - previousRating):0.0} | Share {(snapshot.marketShare - previousMarketShare >= 0f ? "+" : "")}{(snapshot.marketShare - previousMarketShare):0.0}",
                netIncome = netIncome,
                ratingDelta = snapshot.rating - previousRating,
                marketShareDelta = snapshot.marketShare - previousMarketShare
            };

            report.reasons.Add($"Revenue {gross}, salaries {salaries}, upkeep {upkeep}, tax {tax}.");
            if (overload > 0.1f)
                report.reasons.Add($"Demand exceeded capacity by {overload:0.0}; trust took a hit.");
            else
                report.reasons.Add($"Capacity covered demand; service flow stayed stable.");

            if (snapshot.rating > previousRating)
                report.reasons.Add($"Rating improved to {snapshot.rating:0.0}, boosting organic demand.");
            else if (snapshot.rating < previousRating)
                report.reasons.Add($"Rating slipped to {snapshot.rating:0.0}; recovery cards matter next.");

            if (_rivalPressureImpact > 0.1f)
                report.reasons.Add($"Rival pressure: {_rivalPressureStyle} pushed back on your market share.");

            if (snapshot.cash < previousCash)
                report.reasons.Add($"Cash fell to {snapshot.cash:0}; margin pressure is active.");

            return report;
        }

        private string GetBriefHeadline(BoardPressureType pressure)
        {
            return pressure switch
            {
                BoardPressureType.CapacityShortfall => "Rush pressure is building.",
                BoardPressureType.LowCash => "Cash runway is tightening.",
                BoardPressureType.LowRating => "Trust is slipping.",
                BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                BoardPressureType.WeakQuality => "Quality is underperforming.",
                BoardPressureType.StaffInstability => "Team stability is fragile.",
                BoardPressureType.LowDemand => "Demand needs a push.",
                _ => "Board is stable, push your edge."
            };
        }

        private string GetBriefDetail(BoardPressureType pressure)
        {
            return pressure switch
            {
                BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} > capacity {snapshot.capacity:0.0}. Add staff or throughput.",
                BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Margin discipline matters this turn.",
                BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Recovery and quality upgrades now pay off.",
                BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Defensive reactions are valuable.",
                BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Supplier and staff choices are lagging.",
                BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Burnout will cascade if ignored.",
                BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Marketing or discovery should lead.",
                _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
            };
        }

        private string BuildReportHeadline(float overload)
        {
            if (netIncome < 0)
                return "This turn burned cash.";
            if (overload > 0.25f)
                return "Growth outpaced the board.";
            if (snapshot.marketShare > 55f)
                return "You tightened your grip on the market.";
            return "Board held together this turn.";
        }

        private static string[] CollectActiveCrisisTags(IReadOnlyList<CardData> temp)
        {
            var tags = new HashSet<string>();
            for (int i = 0; i < temp.Count; i++)
            {
                var card = temp[i];
                if (card == null || card.crisisTags == null) continue;
                for (int t = 0; t < card.crisisTags.Length; t++)
                {
                    if (!string.IsNullOrWhiteSpace(card.crisisTags[t]))
                        tags.Add(card.crisisTags[t]);
                }
            }

            var result = new string[tags.Count];
            tags.CopyTo(result);
            return result;
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
