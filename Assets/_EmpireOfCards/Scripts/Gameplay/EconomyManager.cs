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

            var board = GatherBoardCards();
            var techCategory = gm.ActiveTechCategoryProfile;
            float previousCash = snapshot.cash;
            float previousRating = snapshot.rating;
            float previousMarketShare = snapshot.marketShare;

            float organicDemand = CalculateCoreMetrics(board, techCategory);
            ApplySubsystemEffects(board);

            float servedDemand = Mathf.Min(snapshot.demand, snapshot.capacity);
            float overload = Mathf.Max(0f, snapshot.demand - snapshot.capacity);
            ApplyRatingDelta(board, techCategory, overload);

            int upkeepCosts = CalculateIncome(board, gm, servedDemand);
            int subsystemExpenses = ApplyExpenseSubsystems(gm);

            netIncome = grossIncome - totalSalaries - upkeepCosts - taxAmount - subsystemExpenses;
            if (abilitySystem != null && abilitySystem.IncomeMultiplier != 1f)
                netIncome = Mathf.RoundToInt(netIncome * abilitySystem.IncomeMultiplier);

            totalCustomersThisTurn = Mathf.RoundToInt(servedDemand * 6f);

            UpdateMarketAndDerived(board, gm, servedDemand, overload);
            PublishResults(gm, board, previousCash, previousRating, previousMarketShare, overload, organicDemand, upkeepCosts, subsystemExpenses);
        }

        private BoardCards GatherBoardCards()
        {
            return new BoardCards
            {
                operations = boardManager.GetCardsInSlotType(SlotType.Operation),
                staff = boardManager.GetCardsInSlotType(SlotType.Staff),
                marketing = boardManager.GetCardsInSlotType(SlotType.Marketing),
                suppliers = boardManager.GetCardsInSlotType(SlotType.Supplier),
                temp = boardManager.GetCardsInSlotType(SlotType.TempEffect)
            };
        }

        private float CalculateCoreMetrics(BoardCards board, TechCategoryProfile techCategory)
        {
            float opDemand = Sum(board.operations, c => c.demandDelta + c.customersPerTurn * 0.2f);
            float marketingDemand = Sum(board.marketing, c => c.demandDelta);
            float organicDemand = Mathf.Max(0f, (snapshot.rating - 3f) * _activeProfile.ratingToOrganicDemandWeight);
            float tempDemand = Sum(board.temp, c => c.demandDelta);

            snapshot.demand = Mathf.Max(0f, _activeProfile.baseDemand + opDemand + marketingDemand + organicDemand + tempDemand + (techCategory != null ? techCategory.demandModifier : 0f));
            snapshot.capacity = Mathf.Max(1f,
                _activeProfile.startingCapacity +
                (techCategory != null ? techCategory.capacityModifier : 0f) +
                Sum(board.operations, c => c.capacityDelta) +
                Sum(board.staff, c => c.capacityDelta) +
                Sum(board.suppliers, c => c.capacityDelta) +
                Sum(board.temp, c => c.capacityDelta));

            snapshot.quality = Mathf.Clamp(
                _activeProfile.startingQuality +
                (techCategory != null ? techCategory.qualityModifier : 0f) +
                Sum(board.operations, c => c.qualityDelta) +
                Sum(board.staff, c => c.qualityDelta) +
                Sum(board.suppliers, c => c.qualityDelta) +
                Sum(board.temp, c => c.qualityDelta),
                0f, 10f);

            snapshot.staffStability = Mathf.Clamp(
                _activeProfile.startingStaffStability +
                Sum(board.staff, c => c.staffStabilityDelta) +
                Sum(board.temp, c => c.staffStabilityDelta) -
                Mathf.Max(0f, board.marketing.Count - board.staff.Count) * 0.4f,
                0f, 10f);

            snapshot.legalRisk = Mathf.Clamp(
                snapshot.legalRisk - _activeProfile.legalRiskDecayPerTurn +
                (techCategory != null ? techCategory.legalRiskModifier : 0f) +
                Sum(board.suppliers, c => c.legalRiskDeltaPerTurn) +
                Sum(board.marketing, c => c.legalRiskDeltaPerTurn) +
                Sum(board.temp, c => c.legalRiskDeltaPerTurn),
                0f, Constants.LEGAL_RISK_MAX);

            return organicDemand;
        }

        private void ApplySubsystemEffects(BoardCards board)
        {
            float ssStability = snapshot.staffStability;
            float ssLegalRisk = snapshot.legalRisk;
            float ssQuality = snapshot.quality;
            ApplySalaryEffects(ref ssStability);
            ApplyInsuranceEffects(ref ssLegalRisk, board.staff.Count);
            ApplyStockEffects(ref ssQuality, board.suppliers.Count > 0);
            snapshot.staffStability = ssStability;
            snapshot.legalRisk = ssLegalRisk;
            snapshot.quality = ssQuality;
        }

        private void ApplyRatingDelta(BoardCards board, TechCategoryProfile techCategory, float overload)
        {
            float ratingDelta = ((snapshot.quality - 5f) * _activeProfile.qualityToRatingWeight)
                - (overload * _activeProfile.capacityPenaltyMultiplier)
                - Mathf.Max(0f, 4f - snapshot.staffStability) * _activeProfile.staffInstabilityPenalty
                + (techCategory != null ? techCategory.ratingModifier : 0f)
                + Sum(board.marketing, c => c.ratingDeltaPerTurn)
                + Sum(board.temp, c => c.ratingDeltaPerTurn);

            snapshot.rating = Mathf.Clamp(snapshot.rating + ratingDelta, _activeProfile.minRating, _activeProfile.maxRating);
        }

        private int CalculateIncome(BoardCards board, GameManager gm, float servedDemand)
        {
            grossIncome = Mathf.RoundToInt(
                servedDemand * _activeProfile.baseRevenuePerDemand +
                Sum(board.operations, c => c.cashDeltaPerTurn) +
                Sum(board.marketing, c => c.cashDeltaPerTurn) +
                Sum(board.suppliers, c => c.cashDeltaPerTurn) +
                Sum(board.temp, c => c.cashDeltaPerTurn));

            if (_investorDebtTurns > 0)
            {
                grossIncome = Mathf.RoundToInt(grossIncome * (1f - _investorDebtPercent));
                _investorDebtTurns--;
            }

            float seasonMultiplier = GetCurrentSeasonMultiplier(gm);
            grossIncome = Mathf.RoundToInt(grossIncome * seasonMultiplier);

            totalSalaries = Mathf.RoundToInt(Sum(board.staff, c => Mathf.Max(0f, c.upkeepCostPerTurn > 0f ? c.upkeepCostPerTurn : c.salaryPerTurn)));
            int upkeepCosts = Mathf.RoundToInt(
                Sum(board.marketing, c => c.upkeepCostPerTurn) +
                Sum(board.suppliers, c => c.upkeepCostPerTurn) +
                Sum(board.temp, c => c.upkeepCostPerTurn));
            taxAmount = grossIncome > 0 ? Mathf.RoundToInt(grossIncome * (_activeProfile.ventureType == VentureType.TechApp ? 0.08f : 0.10f)) : 0;

            return upkeepCosts;
        }

        private int ApplyExpenseSubsystems(GameManager gm)
        {
            int subsystemExpenses = 0;
            float taxLegalRisk = snapshot.legalRisk;
            ApplyCreditEffects(ref subsystemExpenses);
            ApplyTaxEffects(ref taxLegalRisk, ref subsystemExpenses, gm.CurrentTurn);
            snapshot.legalRisk = taxLegalRisk;
            return subsystemExpenses;
        }

        private void UpdateMarketAndDerived(BoardCards board, GameManager gm, float servedDemand, float overload)
        {
            snapshot.marketShare = Mathf.Clamp(
                snapshot.marketShare +
                ((snapshot.rating - 3f) * 2f) +
                ((snapshot.quality - 5f) * 1.5f) +
                (servedDemand - overload) * 0.5f -
                (_rivalPressureImpact * 1.15f),
                0f, 100f);

            UpdateDerivedMetrics(servedDemand, overload, board.suppliers.Count, board.marketing.Count, board.temp);
            UpdatePressure(overload, board.marketing.Count, board.staff.Count);

            gm.SetPlayerCustomers(Mathf.RoundToInt(snapshot.marketShare));
            if (netIncome > 0)
                gm.GainMoney(netIncome);
            else if (netIncome < 0)
                gm.AdjustMoney(netIncome);

            snapshot.cash = gm.PlayerMoney;
            snapshot.activeCrisisTags = CollectActiveCrisisTags(board.temp);
            UpdateCashCrisis();
        }

        private void PublishResults(
            GameManager gm,
            BoardCards board,
            float previousCash,
            float previousRating,
            float previousMarketShare,
            float overload,
            float organicDemand,
            int upkeepCosts,
            int subsystemExpenses)
        {
            EventBus.PlatformRatingChanged(snapshot.rating);
            EventBus.LegalRiskUpdated(Mathf.RoundToInt(snapshot.legalRisk));
            EventBus.CashBalanceChanged(Mathf.RoundToInt(snapshot.cash));
            EventBus.MarketShareUpdated(Mathf.RoundToInt(snapshot.marketShare), gm.RivalCustomers);
            EventBus.OrganicCustomersGained(Mathf.RoundToInt(organicDemand * 4f));
            EventBus.IncomeBreakdownReported(BuildIncomeBreakdown(grossIncome, totalSalaries, upkeepCosts, taxAmount, subsystemExpenses, netIncome));
            lastReport = BuildTurnReport(previousCash, previousRating, previousMarketShare, overload, grossIncome, totalSalaries, upkeepCosts, taxAmount);
            EventBus.TurnReportGenerated(lastReport);
        }

        private struct BoardCards
        {
            public IReadOnlyList<CardData> operations;
            public IReadOnlyList<CardData> staff;
            public IReadOnlyList<CardData> marketing;
            public IReadOnlyList<CardData> suppliers;
            public IReadOnlyList<CardData> temp;
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
            var gm = GameManager.Instance;
            var runtime = gm != null ? gm.ActiveVentureRuntime : null;
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
                    buildIdentity = GameClarityFormatter.GetBuildIdentity(gm)
                };
                EventBus.TurnBriefGenerated(currentBrief);
                return currentBrief;
            }

            var venture = gm != null && gm.SelectedVenture != null
                ? gm.SelectedVenture.ventureType
                : VentureType.FastFood;
            var category = gm != null ? gm.ActiveTechCategoryProfile : null;
            string buildIdentity = GameClarityFormatter.GetBuildIdentity(gm);

            if (boardManager != null && currentTurn <= 3)
            {
                int opCount = boardManager.GetCardsInSlotType(SlotType.Operation).Count;
                int stCount = boardManager.GetCardsInSlotType(SlotType.Staff).Count;
                int suCount = boardManager.GetCardsInSlotType(SlotType.Supplier).Count;
                int mkCount = boardManager.GetCardsInSlotType(SlotType.Marketing).Count;

                if (TurnNarrativeService.TryBuildOpeningBrief(
                        currentTurn, venture, opCount, stCount, suCount, mkCount,
                        currentPressure, buildIdentity, out var openingBrief))
                {
                    currentBrief = openingBrief;
                    EventBus.TurnBriefGenerated(currentBrief);
                    return currentBrief;
                }
            }

            currentBrief = new TurnBriefData
            {
                currentTurn = currentTurn,
                pressure = currentPressure,
                headline = TurnNarrativeService.GetBriefHeadline(currentPressure, venture, category),
                detail = TurnNarrativeService.GetBriefDetail(currentPressure, venture, category, snapshot),
                recommendedMove = TurnNarrativeService.GetRecommendedMove(currentPressure, buildIdentity),
                buildIdentity = buildIdentity
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

        private IncomeBreakdown BuildIncomeBreakdown(int gross, int salaries, int upkeep, int tax, int subsystem, int net)
        {
            var breakdown = new IncomeBreakdown();
            breakdown.steps.Add(new IncomeStep("Revenue", gross));
            if (salaries > 0)
                breakdown.steps.Add(new IncomeStep("Salaries", salaries, false, true));
            if (upkeep > 0)
                breakdown.steps.Add(new IncomeStep("Upkeep", upkeep, false, true));
            if (tax > 0)
                breakdown.steps.Add(new IncomeStep("Tax", tax, false, true));
            if (subsystem > 0)
                breakdown.steps.Add(new IncomeStep("Credit & Tax Debt", subsystem, false, true));
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
            var gm = GameManager.Instance;
            var venture = gm != null && gm.SelectedVenture != null
                ? gm.SelectedVenture.ventureType
                : VentureType.FastFood;
            var category = gm != null ? gm.ActiveTechCategoryProfile : null;

            var report = new TurnReportData
            {
                headline = TurnNarrativeService.BuildReportHeadline(venture, category, netIncome, overload, snapshot.marketShare),
                summary = $"Net {(netIncome >= 0 ? "+" : "")}{netIncome} | Rating {(snapshot.rating - previousRating >= 0f ? "+" : "")}{(snapshot.rating - previousRating):0.0} | Share {(snapshot.marketShare - previousMarketShare >= 0f ? "+" : "")}{(snapshot.marketShare - previousMarketShare):0.0}",
                primaryReason = TurnNarrativeService.BuildPrimaryReason(overload, gross, salaries, upkeep, tax, netIncome, snapshot.rating, snapshot.cash, _rivalPressureImpact, _rivalPressureStyle),
                buildIdentity = GameClarityFormatter.GetBuildIdentity(gm),
                netIncome = netIncome,
                ratingDelta = snapshot.rating - previousRating,
                marketShareDelta = snapshot.marketShare - previousMarketShare
            };

            report.reasons.Add($"Revenue {gross}, salaries {salaries}, upkeep {upkeep}, tax {tax}.");
            TurnNarrativeService.AppendCategoryReason(report.reasons, category);
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
