using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Core.TurnPhases;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Economy;
using EmpireOfCards.UI.Clarity;

namespace EmpireOfCards.Gameplay
{
    public class EconomyManager : MonoBehaviour
    {
        [Header("Balance Data")]
        [SerializeField] private GameBalanceData balanceData;

        [Header("References")]
        [SerializeField] private BoardManager boardManager;
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
        private StockSystem stockSystem;
        private TaxPeriodSystem taxPeriodSystem;

        private InsuranceType _currentInsuranceType = InsuranceType.Uninsured;

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
        public StockSystem StockSystem => stockSystem;
        public TaxPeriodSystem TaxPeriodSystem => taxPeriodSystem;
        public VentureBoardSnapshot Snapshot => snapshot;
        public BoardPressureType CurrentPressure => currentPressure;
        public TurnBriefData CurrentBrief => currentBrief;
        public TurnReportData LastReport => lastReport;
        public float RivalPressureImpact => _rivalPressureImpact;
        public string RivalPressureStyle => _rivalPressureStyle;

        public void Init(GameBalanceData balance, BoardManager board,
            AbilitySystem ability = null, SlotManager slots = null)
        {
            balanceData = balance;
            boardManager = board;
            abilitySystem = ability;
            slotManager = slots;

            salarySystem = new SalarySystem();
            insuranceSystem = new InsuranceSystem();
            creditSystem = new CreditSystem();
            stockSystem = new StockSystem();
            taxPeriodSystem = new TaxPeriodSystem();
        }

        public void SetActiveProfile(VentureEconomyProfile profile)
        {
            var techCategory = GameManager.Instance != null ? GameManager.Instance.ActiveTechCategoryProfile : null;
            _activeProfile = profile;
            snapshot = new VentureBoardSnapshot
            {
                ventureType = profile != null ? profile.ventureType : VentureType.FastFood,
                cash = profile != null ? profile.startingCash : Constants.STARTING_MONEY,
                demand = (profile != null ? profile.startingDemand : 2f) + (techCategory != null ? techCategory.demandModifier : 0f),
                capacity = (profile != null ? profile.startingCapacity : 3f) + (techCategory != null ? techCategory.capacityModifier : 0f),
                quality = (profile != null ? profile.startingQuality : 3f) + (techCategory != null ? techCategory.qualityModifier : 0f),
                rating = (profile != null ? profile.startingRating : Constants.PLATFORM_RATING_DEFAULT) + (techCategory != null ? techCategory.ratingModifier : 0f),
                staffStability = profile != null ? profile.startingStaffStability : 6f,
                legalRisk = (profile != null ? profile.startingLegalRisk : 0f) + (techCategory != null ? techCategory.legalRiskModifier : 0f),
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
            var techCategory = gm.ActiveTechCategoryProfile;
            float previousCash = snapshot.cash;
            float previousRating = snapshot.rating;
            float previousMarketShare = snapshot.marketShare;

            float opDemand = Sum(operations, c => c.demandDelta + c.customersPerTurn * 0.2f);
            float marketingDemand = Sum(marketing, c => c.demandDelta);
            float organicDemand = Mathf.Max(0f, (snapshot.rating - 3f) * _activeProfile.ratingToOrganicDemandWeight);
            float tempDemand = Sum(temp, c => c.demandDelta);

            snapshot.demand = Mathf.Max(0f, _activeProfile.baseDemand + opDemand + marketingDemand + organicDemand + tempDemand + (techCategory != null ? techCategory.demandModifier : 0f));
            snapshot.capacity = Mathf.Max(1f,
                _activeProfile.startingCapacity +
                (techCategory != null ? techCategory.capacityModifier : 0f) +
                Sum(operations, c => c.capacityDelta) +
                Sum(staff, c => c.capacityDelta) +
                Sum(suppliers, c => c.capacityDelta) +
                Sum(temp, c => c.capacityDelta));

            snapshot.quality = Mathf.Clamp(
                _activeProfile.startingQuality +
                (techCategory != null ? techCategory.qualityModifier : 0f) +
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
                (techCategory != null ? techCategory.legalRiskModifier : 0f) +
                Sum(suppliers, c => c.legalRiskDeltaPerTurn) +
                Sum(marketing, c => c.legalRiskDeltaPerTurn) +
                Sum(temp, c => c.legalRiskDeltaPerTurn),
                0f, Constants.LEGAL_RISK_MAX);

            // --- Economy subsystem effects (modify snapshot before rating/income) ---
            float ssStability = snapshot.staffStability;
            float ssLegalRisk = snapshot.legalRisk;
            float ssQuality = snapshot.quality;
            ApplySalaryEffects(ref ssStability);
            ApplyInsuranceEffects(ref ssLegalRisk, staff.Count);
            ApplyStockEffects(ref ssQuality, suppliers.Count > 0);
            snapshot.staffStability = ssStability;
            snapshot.legalRisk = ssLegalRisk;
            snapshot.quality = ssQuality;

            float servedDemand = Mathf.Min(snapshot.demand, snapshot.capacity);
            float overload = Mathf.Max(0f, snapshot.demand - snapshot.capacity);
            float ratingDelta = ((snapshot.quality - 5f) * _activeProfile.qualityToRatingWeight)
                - (overload * _activeProfile.capacityPenaltyMultiplier)
                - Mathf.Max(0f, 4f - snapshot.staffStability) * _activeProfile.staffInstabilityPenalty
                + (techCategory != null ? techCategory.ratingModifier : 0f)
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

            // Apply season multiplier to gross income (GDD Section 14)
            float seasonMultiplier = GetCurrentSeasonMultiplier(gm);
            grossIncome = Mathf.RoundToInt(grossIncome * seasonMultiplier);

            totalSalaries = Mathf.RoundToInt(Sum(staff, c => Mathf.Max(0f, c.upkeepCostPerTurn > 0f ? c.upkeepCostPerTurn : c.salaryPerTurn)));
            int upkeepCosts = Mathf.RoundToInt(
                Sum(marketing, c => c.upkeepCostPerTurn) +
                Sum(suppliers, c => c.upkeepCostPerTurn) +
                Sum(temp, c => c.upkeepCostPerTurn));
            taxAmount = grossIncome > 0 ? Mathf.RoundToInt(grossIncome * (_activeProfile.ventureType == VentureType.TechApp ? 0.08f : 0.10f)) : 0;

            // --- Credit + Tax subsystem effects (modify expenses) ---
            int subsystemExpenses = 0;
            float taxLegalRisk = snapshot.legalRisk;
            ApplyCreditEffects(ref subsystemExpenses);
            ApplyTaxEffects(ref taxLegalRisk, ref subsystemExpenses, gm.CurrentTurn);
            snapshot.legalRisk = taxLegalRisk;

            netIncome = grossIncome - totalSalaries - upkeepCosts - taxAmount - subsystemExpenses;

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

        public bool CanTakeCredit(CreditType type) => creditSystem != null && creditSystem.CanTakeCredit(type);

        public void TakeCredit(CreditType type)
        {
            if (creditSystem == null)
                creditSystem = new CreditSystem();

            int amount = creditSystem.GetCreditAmount(type);
            creditSystem.TakeCredit(type);
            snapshot.cash += amount;
            EventBus.CashBalanceChanged(Mathf.RoundToInt(snapshot.cash));
        }

        public int GetTotalCreditDebt() => creditSystem != null ? creditSystem.TotalDebt : 0;

        public void SetInsuranceType(InsuranceType type)
        {
            _currentInsuranceType = type;
        }

        public InsuranceType CurrentInsuranceType => _currentInsuranceType;

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
            var runtime = GameManager.Instance != null ? GameManager.Instance.ActiveVentureRuntime : null;
            TurnScriptBeat beat = runtime != null ? runtime.GetBeatForTurn(currentTurn) : null;
            if (beat != null)
            {
                currentBrief = new TurnBriefData
                {
                    currentTurn = currentTurn,
                    pressure = currentPressure,
                    headline = beat.headline,
                    detail = beat.detail,
                    recommendedMove = beat.recommendedMove,
                    buildIdentity = GameClarityFormatter.GetBuildIdentity(GameManager.Instance)
                };
                EventBus.TurnBriefGenerated(currentBrief);
                return currentBrief;
            }

            if (TryBuildOpeningBrief(currentTurn, out var openingBrief))
            {
                currentBrief = openingBrief;
                EventBus.TurnBriefGenerated(currentBrief);
                return currentBrief;
            }

            currentBrief = new TurnBriefData
            {
                currentTurn = currentTurn,
                pressure = currentPressure,
                headline = GetBriefHeadline(currentPressure),
                detail = GetBriefDetail(currentPressure),
                recommendedMove = GetRecommendedMove(currentPressure),
                buildIdentity = GameClarityFormatter.GetBuildIdentity(GameManager.Instance)
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
            _currentInsuranceType = InsuranceType.Uninsured;
            salarySystem?.Reset();
            creditSystem?.Reset();
            taxPeriodSystem?.Reset();
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

        private float GetCurrentSeasonMultiplier(GameManager gm)
        {
            int currentTurn = gm.CurrentTurn;
            int seasonIndex = Mathf.Clamp((currentTurn - 1) / Constants.TURNS_PER_SEASON, 0, 4);
            SeasonType season = (SeasonType)seasonIndex;
            VentureType venture = _activeProfile != null ? _activeProfile.ventureType : VentureType.FastFood;
            return ResolvePhase.GetVentureSeasonMultiplier(venture, season);
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
                primaryReason = BuildPrimaryReason(overload, gross, salaries, upkeep, tax),
                buildIdentity = GameClarityFormatter.GetBuildIdentity(GameManager.Instance),
                netIncome = netIncome,
                ratingDelta = snapshot.rating - previousRating,
                marketShareDelta = snapshot.marketShare - previousMarketShare
            };

            report.reasons.Add($"Revenue {gross}, salaries {salaries}, upkeep {upkeep}, tax {tax}.");
            AppendCategoryReason(report.reasons);
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
            var venture = GameManager.Instance != null && GameManager.Instance.SelectedVenture != null
                ? GameManager.Instance.SelectedVenture.ventureType
                : VentureType.FastFood;
            var category = GameManager.Instance != null ? GameManager.Instance.ActiveTechCategoryProfile : null;
            if (category != null)
            {
                return pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"{category.displayName} growth is outrunning the stack.",
                    BoardPressureType.LowRating => $"{category.displayName} trust is getting fragile.",
                    BoardPressureType.HighLegalRisk => $"{category.displayName} risk is becoming visible.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.WeakQuality => "Quality is underperforming.",
                        BoardPressureType.StaffInstability => "Team stability is fragile.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                };
            }

            return venture switch
            {
                VentureType.FastFood => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "Kitchen pressure is spiking.",
                    BoardPressureType.LowRating => "Local trust is wobbling.",
                    BoardPressureType.WeakQuality => "Ingredient quality is falling behind.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                        BoardPressureType.StaffInstability => "Team stability is fragile.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                },
                VentureType.Cafe => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "The bar is backing up.",
                    BoardPressureType.LowRating => "Neighborhood trust is fading.",
                    BoardPressureType.StaffInstability => "The shift is starting to crack.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                        BoardPressureType.WeakQuality => "Quality is underperforming.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                },
                VentureType.ClothingStore => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "The floor cannot absorb demand.",
                    BoardPressureType.LowRating => "Brand trust is slipping.",
                    BoardPressureType.WeakQuality => "Fabric and fit are under pressure.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                        BoardPressureType.StaffInstability => "Team stability is fragile.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                },
                VentureType.GroceryStore => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "Shelf pressure is building.",
                    BoardPressureType.LowRating => "Mahalle trust is slipping.",
                    BoardPressureType.WeakQuality => "Freshness discipline is slipping.",
                    _ => pressure switch
                    {
                        BoardPressureType.LowCash => "Cash runway is tightening.",
                        BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                        BoardPressureType.StaffInstability => "Team stability is fragile.",
                        BoardPressureType.LowDemand => "Demand needs a push.",
                        _ => "Board is stable, push your edge."
                    }
                },
                _ => pressure switch
                {
                    BoardPressureType.CapacityShortfall => "Rush pressure is building.",
                    BoardPressureType.LowCash => "Cash runway is tightening.",
                    BoardPressureType.LowRating => "Trust is slipping.",
                    BoardPressureType.HighLegalRisk => "Legal exposure is rising.",
                    BoardPressureType.WeakQuality => "Quality is underperforming.",
                    BoardPressureType.StaffInstability => "Team stability is fragile.",
                    BoardPressureType.LowDemand => "Demand needs a push.",
                    _ => "Board is stable, push your edge."
                }
            };
        }

        private string GetBriefDetail(BoardPressureType pressure)
        {
            var venture = GameManager.Instance != null && GameManager.Instance.SelectedVenture != null
                ? GameManager.Instance.SelectedVenture.ventureType
                : VentureType.FastFood;
            var category = GameManager.Instance != null ? GameManager.Instance.ActiveTechCategoryProfile : null;
            if (category != null)
            {
                return pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"{category.displayName} traffic is beating delivery capacity. Backend and support must catch up.",
                    BoardPressureType.LowCash => $"{category.displayName} spend is too hot. Paid growth and infra discipline matter now.",
                    BoardPressureType.LowRating => $"{category.displayName} reviews are fragile. Fix trust before pushing more installs.",
                    BoardPressureType.HighLegalRisk => $"{category.displayName} is carrying visible risk. Privacy, dark patterns, or unstable launches may cascade.",
                    BoardPressureType.WeakQuality => $"{category.displayName} quality is lagging. Product reliability and core experience need help.",
                    BoardPressureType.StaffInstability => $"{category.displayName} team flow is shaky. Burnout or rushed releases can snowball.",
                    BoardPressureType.LowDemand => $"{category.displayName} discovery is too weak. ASO, creators, or targeted acquisition should lead.",
                    _ => $"{category.displayName}: rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                };
            }

            return venture switch
            {
                VentureType.FastFood => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} is outrunning kitchen and counter throughput.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Delivery spend and wages are squeezing margin.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Reviews and quality fixes should lead.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Fake reviews or hygiene shortcuts can cascade.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Ingredient discipline and cleanup matter now.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Rush fatigue will slow service.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Local buzz and Google presence need help.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                },
                VentureType.Cafe => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} is overrunning the bar and floor flow.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Premium beans and staffing are biting margin.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Slow service or weak drinks are visible.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Shortcut ambience or complaints may escalate.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Beans, milk, and consistency are trailing.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Burnout can spill into reviews.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Regulars and visual discovery need a push.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                },
                VentureType.ClothingStore => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} is outpacing floor conversion and stock handling.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Discounting and return pressure are squeezing margin.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Fit, returns, and brand trust need recovery.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Cheap fabric or shady claims can rebound.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Fabric, fit, and atelier quality are behind.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. The sales floor is getting brittle.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Vitrine storytelling and trend pull need help.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                },
                VentureType.GroceryStore => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} is beating shelf and checkout throughput.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Thin margin and waste are active this turn.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Freshness trust is too fragile right now.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. SKT shortcuts or trust gaps can snowball.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Freshness and shelf discipline are slipping.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Empty shelves and slow lines can follow.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Convenience and neighborhood loyalty need help.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                },
                _ => pressure switch
                {
                    BoardPressureType.CapacityShortfall => $"Demand {snapshot.demand:0.0} > capacity {snapshot.capacity:0.0}. Add staff or throughput.",
                    BoardPressureType.LowCash => $"Cash is {snapshot.cash:0}. Margin discipline matters this turn.",
                    BoardPressureType.LowRating => $"Rating is {snapshot.rating:0.0}. Recovery and quality upgrades now pay off.",
                    BoardPressureType.HighLegalRisk => $"Legal risk is {snapshot.legalRisk:0}. Defensive reactions are valuable.",
                    BoardPressureType.WeakQuality => $"Quality is {snapshot.quality:0.0}. Supplier and staff choices are lagging.",
                    BoardPressureType.StaffInstability => $"Staff stability is {snapshot.staffStability:0.0}. Burnout will cascade if ignored.",
                    BoardPressureType.LowDemand => $"Demand is only {snapshot.demand:0.0}. Marketing or discovery should lead.",
                    _ => $"Rating {snapshot.rating:0.0}, quality {snapshot.quality:0.0}, share {snapshot.marketShare:0.0}."
                }
            };
        }

        private string BuildReportHeadline(float overload)
        {
            var venture = GameManager.Instance != null && GameManager.Instance.SelectedVenture != null
                ? GameManager.Instance.SelectedVenture.ventureType
                : VentureType.FastFood;
            var category = GameManager.Instance != null ? GameManager.Instance.ActiveTechCategoryProfile : null;
            if (category != null)
            {
                if (netIncome < 0)
                    return $"{category.displayName} burned runway this turn.";
                if (overload > 0.25f)
                    return $"{category.displayName} growth outran stability.";
            }

            if (venture == VentureType.FastFood && overload > 0.25f)
                return "The rush outran your kitchen.";
            if (venture == VentureType.Cafe && overload > 0.25f)
                return "The shift lost pace under pressure.";
            if (venture == VentureType.ClothingStore && netIncome < 0)
                return "Discounting and returns ate the turn.";
            if (venture == VentureType.GroceryStore && netIncome < 0)
                return "Margin got squeezed by waste and convenience.";
            if (netIncome < 0)
                return "This turn burned cash.";
            if (overload > 0.25f)
                return "Growth outpaced the board.";
            if (snapshot.marketShare > 55f)
                return "You tightened your grip on the market.";
            return "Board held together this turn.";
        }

        private bool TryBuildOpeningBrief(int currentTurn, out TurnBriefData brief)
        {
            brief = null;

            var gm = GameManager.Instance;
            var venture = gm != null ? gm.SelectedVenture : null;
            if (venture == null || boardManager == null || currentTurn > 3)
                return false;

            int operationCount = boardManager.GetCardsInSlotType(SlotType.Operation).Count;
            int staffCount = boardManager.GetCardsInSlotType(SlotType.Staff).Count;
            int supplierCount = boardManager.GetCardsInSlotType(SlotType.Supplier).Count;
            int marketingCount = boardManager.GetCardsInSlotType(SlotType.Marketing).Count;

            string headline;
            string detail;
            string move;

            switch (venture.ventureType)
            {
                case VentureType.FastFood:
                    if (operationCount <= 1)
                    {
                        headline = "Open the floor before you chase volume.";
                        detail = "The counter is live, but the rush still needs seating and service flow around the grill.";
                        move = "Play Extra Tables first. Add a Line Cook or counter helper before heavy flyer demand.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "Put people on the line.";
                        detail = "Your rush cannot feel real until the grill and front counter have actual labor behind them.";
                        move = "Hire Line Cook or Front Counter Server now. Save big demand pushes until the queue feels stable.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Protect food quality before the reviews hit.";
                        detail = "Ingredient trust and hygiene are what make a busy fast food board survive the first surge.";
                        move = "Take Premium Butcher or Night Cleaner before stacking more marketing pressure.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now the neighborhood should notice you.";
                        detail = "Your floor is credible enough to start converting local attention into repeat foot traffic.";
                        move = "Push Google Business or Flyer Team only after the board can absorb the next rush.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case VentureType.Cafe:
                    if (operationCount <= 1)
                    {
                        headline = "Make the cafe feel open, not just named.";
                        detail = "The espresso bar exists, but guests still need a room, a seat, and visible flow around the counter.";
                        move = "Play Window Seating first. Then line up Senior Barista before you spend on buzz.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "The room needs a real shift behind it.";
                        detail = "A cafe does not feel alive until the bar and floor have an actual operator holding consistency.";
                        move = "Hire Senior Barista now. Floor Runner is the next stabilizer if service starts backing up.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Lock the taste before you chase the crowd.";
                        detail = "Beans, milk, and drink consistency are what turn one-time curiosity into neighborhood loyalty.";
                        move = "Play Specialty Beans or Milk Contract before you lean into Instagram or repeat traffic.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now you can build the regular loop.";
                        detail = "Service is credible enough to turn trust into Maps discovery, reels, and repeat morning visits.";
                        move = "Use Maps Reviews first for trust, then Instagram Reels or Stamp Card once the floor still feels smooth.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case VentureType.TechApp:
                    if (operationCount <= 1)
                    {
                        headline = "Ship the core before you buy growth.";
                        detail = "The product is live, but the stack still needs more reliability before users arrive at scale.";
                        move = "Play Backend Upgrade or a second product card before paid acquisition starts pulling installs.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "Put real builders behind the MVP.";
                        detail = "The app still feels fragile until a developer or support layer turns the launch into a system.";
                        move = "Hire Core Developer first. Support or PM follow once the product loop is real.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Stability economics come before growth economics.";
                        detail = "Cloud, analytics, and payment infrastructure decide whether growth becomes retention or chaos.";
                        move = "Take Cloud Credits or Export Pipeline before the biggest acquisition cards.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now turn reliability into user growth.";
                        detail = "You finally have enough product trust to scale discovery without instantly burning reviews.";
                        move = "Lead with ASO Push. Add paid acquisition only if backend and rating still look safe.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case VentureType.ClothingStore:
                    if (operationCount <= 1)
                    {
                        headline = "Dress the floor before you advertise the brand.";
                        detail = "The storefront exists, but the customer still needs depth, fit confidence, and visible merchandise logic.";
                        move = "Play Inventory Rail first. Then add fit support before forcing trend demand.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "Style needs staff, not only display.";
                        detail = "A clothing board feels empty until a stylist or tailor can convert browsing into trust.";
                        move = "Hire Floor Stylist first. Tailor becomes the next key stabilizer.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Protect fit and fabric before trend traffic.";
                        detail = "Returns and weak materials are what make a promising clothing run collapse early.";
                        move = "Play Reliable Atelier or Premium Fabric Mill before aggressive demand plays.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now the collection is ready to be seen.";
                        detail = "The board has enough fit confidence to turn display into higher-value browsing and conversion.";
                        move = "Use Instagram Lookbook or Window Story Display once return pressure feels covered.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case VentureType.GroceryStore:
                    if (operationCount <= 1)
                    {
                        headline = "Make the store function before you extend convenience.";
                        detail = "Fresh shelves are live, but the basket still needs smoother checkout and a clearer trip flow.";
                        move = "Play Checkout Upgrade first. Then add the first staff layer before growing convenience demand.";
                    }
                    else if (staffCount == 0)
                    {
                        headline = "Neighborhood trust starts with the people inside.";
                        detail = "A grocery board does not feel dependable until the register and shelf rhythm have actual staff support.";
                        move = "Hire Trusted Cashier first. Follow with stock or fresh-keeping support if lines stay messy.";
                    }
                    else if (supplierCount == 0)
                    {
                        headline = "Protect freshness before you widen reach.";
                        detail = "Morning supply quality and spoilage discipline are what make repeat traffic stick in grocery.";
                        move = "Take Morning Hal Route before you invest harder in convenience or late-night traffic.";
                    }
                    else if (marketingCount == 0)
                    {
                        headline = "Now convenience can become loyalty.";
                        detail = "The store is credible enough to convert service reliability into repeat neighborhood demand.";
                        move = "Use WhatsApp Orders or Late Night Sign once checkout and freshness still feel under control.";
                    }
                    else
                    {
                        return false;
                    }
                    break;

                default:
                    return false;
            }

            brief = new TurnBriefData
            {
                currentTurn = currentTurn,
                pressure = currentPressure,
                headline = headline,
                detail = detail,
                recommendedMove = move,
                buildIdentity = GameClarityFormatter.GetBuildIdentity(gm)
            };

            return true;
        }

        private string GetRecommendedMove(BoardPressureType pressure)
        {
            return pressure switch
            {
                BoardPressureType.CapacityShortfall => "Play a Fix Capacity or staff card before adding more demand.",
                BoardPressureType.LowDemand => "Play a Create Demand card only if your board can absorb the traffic.",
                BoardPressureType.LowRating => "Play a Recover Rating or quality card before pushing harder.",
                BoardPressureType.HighLegalRisk => "Play a Reduce Risk reaction and avoid another shortcut.",
                BoardPressureType.LowCash => "Play an Improve Margin card or cut expensive pressure lanes.",
                BoardPressureType.WeakQuality => "Play a supplier or quality-focused staff card.",
                BoardPressureType.StaffInstability => "Stabilize the team before the next rush turn.",
                _ => $"Lean into your build: {GameClarityFormatter.GetBuildIdentity(GameManager.Instance)}."
            };
        }

        private string BuildPrimaryReason(float overload, int gross, int salaries, int upkeep, int tax)
        {
            if (overload > 0.25f)
                return $"Demand beat capacity by {overload:0.0}, so trust took a hit.";

            if (netIncome < 0 && salaries + upkeep > gross)
                return $"Salaries and upkeep ({salaries + upkeep}) outpaced revenue {gross}.";

            if (snapshot.rating < 3.4f)
                return $"Low trust kept rating at {snapshot.rating:0.0}; recovery now has higher payoff.";

            if (snapshot.cash < 120f)
                return $"Cash fell to {snapshot.cash:0}; margin pressure is now your main fight.";

            if (_rivalPressureImpact > 0.1f)
                return $"Rival pressure ({_rivalPressureStyle}) slowed your share gain this turn.";

            return $"Revenue {gross} covered salaries {salaries}, upkeep {upkeep}, and tax {tax}.";
        }

        private static void AppendCategoryReason(List<string> reasons)
        {
            var category = GameManager.Instance != null ? GameManager.Instance.ActiveTechCategoryProfile : null;
            if (category == null || reasons == null)
                return;

            reasons.Add(category.scenarioNote);
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

        // ---------------------------------------------------------------
        // Economy Subsystem Effects (called during ProcessEndOfTurn)
        // ---------------------------------------------------------------

        private void ApplySalaryEffects(ref float staffStability)
        {
            if (salarySystem == null) return;

            var choice = salarySystem.LastChoice;
            if (choice == SalaryChoice.Delay)
            {
                // -0.5 staff stability per consecutive delayed turn
                float penalty = salarySystem.ConsecutiveDelayTurns * 0.5f;
                staffStability -= penalty;
            }
            else if (choice == SalaryChoice.PartialPay)
            {
                // -0.2 staff stability for partial pay
                staffStability -= 0.2f;
            }

            staffStability = Mathf.Clamp(staffStability, 0f, 10f);
        }

        private void ApplyInsuranceEffects(ref float legalRisk, int staffCount)
        {
            if (insuranceSystem == null || staffCount <= 0) return;

            int riskPerStaff = insuranceSystem.GetLegalRiskPerTurn(_currentInsuranceType);
            legalRisk += riskPerStaff * staffCount;
            legalRisk = Mathf.Clamp(legalRisk, 0f, Constants.LEGAL_RISK_MAX);
        }

        private void ApplyCreditEffects(ref int expenses)
        {
            if (creditSystem == null || creditSystem.ActiveCreditCount <= 0) return;

            int interestPayment = creditSystem.TickCredits();
            expenses += interestPayment;
        }

        private void ApplyStockEffects(ref float quality, bool hasSupplier)
        {
            if (stockSystem == null || _activeProfile == null) return;

            var venture = _activeProfile.ventureType;
            if (venture != VentureType.FastFood && venture != VentureType.Cafe && venture != VentureType.GroceryStore)
                return;

            // Low quality + no supplier = spoilage penalty
            if (quality < 3.0f && !hasSupplier)
            {
                quality -= 0.3f;
                quality = Mathf.Max(0f, quality);
            }
        }

        private void ApplyTaxEffects(ref float legalRisk, ref int expenses, int currentTurn)
        {
            if (taxPeriodSystem == null) return;

            // Track this turn's profit for period accumulation
            taxPeriodSystem.TrackProfit(netIncome);

            // Tick existing tax debt (interest + audit check)
            if (taxPeriodSystem.HasTaxDebt)
            {
                int debtInterest = taxPeriodSystem.TickTaxDebt();
                expenses += debtInterest;

                // +5 legal risk per turn with unpaid tax debt
                legalRisk += 5f;
                legalRisk = Mathf.Clamp(legalRisk, 0f, Constants.LEGAL_RISK_MAX);
            }
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
