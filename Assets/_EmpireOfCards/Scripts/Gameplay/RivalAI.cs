using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Rival;

namespace EmpireOfCards.Gameplay
{
    [Serializable]
    public class RivalBusiness
    {
        public string name;
        public int income;
        public int customers;
        public int employeeCount;
        public int maxEmployees = 3;
        public VentureType ventureType;
        public float qualityScore = 5f;
        public float platformRating = 3f;
        public int legalRisk;
        public float priceScore = 5f;
    }

    public class RivalAI : MonoBehaviour
    {
        [SerializeField] private RivalData data;
        [SerializeField] private int rivalMoney;
        [SerializeField] private int rivalIncome;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private int totalRivalEmployees;
        [SerializeField] private List<RivalBusiness> rivalBusinesses = new List<RivalBusiness>();
        [SerializeField] private VentureType activeVenture = VentureType.FastFood;
        [SerializeField] private float rivalRating = 3f;
        [SerializeField] private float rivalQuality = 5f;
        [SerializeField] private float rivalPressure = 0f;
        [SerializeField] private float rivalMomentumCustomers = 0f;
        [SerializeField] private string lastPlayedCardName;
        [SerializeField] private string lastLaneLabel;
        [SerializeField] private string lastPressureStyle;

        private readonly List<RivalQueuedAction> _queuedActions = new List<RivalQueuedAction>();
        private IRivalSectorController _sectorController;
        private RivalRuntimeState _runtimeState = new RivalRuntimeState();
        private RivalSectorContext? _cachedContext;

        public int RivalMoney => rivalMoney;
        public int RivalCustomers => rivalCustomers;
        public int RivalIncome => rivalIncome;
        public float RivalRating => rivalRating;
        public float RivalQuality => rivalQuality;
        public float RivalPressure => rivalPressure;
        public IReadOnlyList<RivalBusiness> RivalBusinesses => rivalBusinesses;
        public string RivalDisplayName => rivalBusinesses.Count > 0 ? rivalBusinesses[0].name : "Rival";
        public string CurrentMood => "focused";
        public string MoodIcon => "!";
        public RivalData Data => data;
        public IReadOnlyList<RivalQueuedAction> QueuedActions => _queuedActions;
        public string LastPlayedCardName => lastPlayedCardName;
        public string LastLaneLabel => lastLaneLabel;
        public string LastPressureStyle => lastPressureStyle;

        public void Init(RivalData rivalData)
        {
            data = rivalData;
        }

        public void Initialize()
        {
            Initialize(VentureType.FastFood);
        }

        public void Initialize(VentureType playerVenture)
        {
            activeVenture = playerVenture;
            _sectorController = RivalSectorControllerFactory.Create(playerVenture);
            _runtimeState = new RivalRuntimeState
            {
                escalationLevel = 0,
                campaignsLaunched = 0,
                pressureBank = 0f,
                activePlan = "opening",
                lastResolvedTurn = 0
            };
            rivalMoney = data != null ? data.startingMoney : 450;
            rivalIncome = 60;
            rivalCustomers = 32;
            totalRivalEmployees = 2;
            rivalRating = 3.2f;
            rivalQuality = 5f;
            rivalPressure = 0f;
            rivalMomentumCustomers = 0f;
            lastPlayedCardName = string.Empty;
            lastLaneLabel = string.Empty;
            lastPressureStyle = "balanced";
            _queuedActions.Clear();
            _cachedContext = null;

            rivalBusinesses.Clear();
            rivalBusinesses.Add(new RivalBusiness
            {
                name = GetRivalName(playerVenture),
                income = rivalIncome,
                customers = rivalCustomers,
                employeeCount = totalRivalEmployees,
                ventureType = playerVenture,
                qualityScore = rivalQuality,
                platformRating = rivalRating,
                priceScore = playerVenture == VentureType.GroceryStore ? 7f : 5f
            });
        }

        public void Initialize(RivalData rivalData)
        {
            data = rivalData;
            Initialize();
        }

        public void TakeTurn(int playerBlocks, int rivalBlocks, int currentTurn)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            float playerShare = gm.EconomyManager != null && gm.EconomyManager.Snapshot != null
                ? gm.EconomyManager.Snapshot.marketShare
                : gm.PlayerCustomers;

            BuildQueuedActions(playerShare, currentTurn);
            foreach (var action in _queuedActions)
                ResolveQueuedAction(action, playerShare);
            FinalizeQueuedTurn(playerShare);
        }

        public void FinalizeQueuedTurn(float playerShare)
        {
            var gm = GameManager.Instance;
            if (gm == null)
                return;

            rivalCustomers = Mathf.Clamp(Mathf.RoundToInt(100f - playerShare + rivalPressure + rivalMomentumCustomers), 0, 100);
            rivalIncome = Mathf.RoundToInt(45f + rivalQuality * 6f + rivalRating * 8f);
            rivalMoney += Mathf.Max(10, rivalIncome / 4);
            rivalMomentumCustomers = 0f;

            if (rivalBusinesses.Count > 0)
            {
                var lead = rivalBusinesses[0];
                lead.customers = rivalCustomers;
                lead.income = rivalIncome;
                lead.platformRating = rivalRating;
                lead.qualityScore = rivalQuality;
            }

            if (_queuedActions.Count > 0)
            {
                lastPlayedCardName = _queuedActions[_queuedActions.Count - 1].displayName;
                lastLaneLabel = _queuedActions[_queuedActions.Count - 1].laneLabel;
                lastPressureStyle = _queuedActions[_queuedActions.Count - 1].shortDescription;
            }

            _runtimeState.lastResolvedTurn = gm.CurrentTurn;
            _runtimeState.pressureBank = rivalPressure;
            if (!string.IsNullOrWhiteSpace(lastLaneLabel))
                _runtimeState.activePlan = lastLaneLabel;

            _cachedContext = null;

            gm.EconomyManager?.RegisterRivalPressure(rivalPressure, lastPressureStyle);
            EventBus.RivalActed(_queuedActions.Count > 0 ? DescribeAction(_queuedActions[_queuedActions.Count - 1]) : "Rival adapts.");
            EventBus.RivalMoodChanged(_queuedActions.Count > 0 ? _queuedActions[_queuedActions.Count - 1].moodIcon : "!");
        }

        public void RespondToPoach_Accept() { }
        public void RespondToPoach_CounterOffer() { }
        public void RespondToPoach_RejectWithBonus() { }

        public void CalculateRivalCustomers()
        {
            rivalCustomers = Mathf.Clamp(rivalCustomers, 0, 100);
        }

        public void CalculateRivalIncome()
        {
            rivalIncome = Mathf.RoundToInt(45f + rivalQuality * 6f + rivalRating * 8f);
        }

        public void ApplyCustomerPenalty(int penalty)
        {
            rivalCustomers = Mathf.Max(0, rivalCustomers - penalty);
        }

        public void ApplyNextPurchaseCostIncrease(float multiplier)
        {
            rivalPressure += multiplier * 2f;
        }

        public void CloseWeakestBusiness(int turns)
        {
            rivalCustomers = Mathf.Max(0, rivalCustomers - 8);
            rivalPressure -= 2f;
        }

        public int DisableProductionOneTurn()
        {
            var gm = GameManager.Instance;
            if (gm != null && gm.BoardManager != null)
                gm.BoardManager.SetProductionDisabledNextTurn(true);
            return 1;
        }

        public string GetTaunt(int playerBlocks, int rivalBlocks)
        {
            var move = DecideMove(playerBlocks * 10f, GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1);
            return GetMoveDescription(move);
        }

        public void OnPlayerComboTriggered()
        {
            rivalPressure += 1.5f;
        }

        public void Reset()
        {
            rivalBusinesses.Clear();
            rivalMoney = 0;
            rivalIncome = 0;
            rivalCustomers = 0;
            totalRivalEmployees = 0;
            rivalPressure = 0f;
            rivalMomentumCustomers = 0f;
            _queuedActions.Clear();
            _runtimeState = new RivalRuntimeState();
            _cachedContext = null;
        }

        public RivalRuntimeState CaptureState()
        {
            return new RivalRuntimeState
            {
                escalationLevel = _runtimeState.escalationLevel,
                campaignsLaunched = _runtimeState.campaignsLaunched,
                pressureBank = rivalPressure,
                activePlan = _runtimeState.activePlan,
                lastResolvedTurn = _runtimeState.lastResolvedTurn
            };
        }

        public void RestoreState(RivalRuntimeState state)
        {
            _runtimeState = state ?? new RivalRuntimeState();
            rivalPressure = _runtimeState.pressureBank;
        }

        public List<RivalQueuedAction> BuildQueuedActions(float playerShare, int currentTurn)
        {
            _queuedActions.Clear();
            rivalPressure = Mathf.Max(0f, rivalPressure * 0.65f);

            _cachedContext = BuildSectorContext(playerShare, currentTurn);

            RivalMove primaryMove = DecideMove(playerShare, currentTurn);
            _queuedActions.Add(BuildAction(primaryMove));
            _runtimeState.campaignsLaunched++;

            if (currentTurn >= 6 || playerShare >= 46f || _runtimeState.escalationLevel >= 2)
            {
                RivalMove secondaryMove = ChooseSecondaryMove(primaryMove, currentTurn);
                _queuedActions.Add(BuildAction(secondaryMove));
            }

            if (currentTurn >= 4 || playerShare >= 50f)
                _runtimeState.escalationLevel = Mathf.Clamp(_runtimeState.escalationLevel + 1, 0, 5);

            return _queuedActions;
        }

        private RivalMove DecideMove(float playerShare, int currentTurn)
        {
            if (_sectorController != null)
                return _sectorController.DecidePrimaryMove(GetOrBuildContext(playerShare, currentTurn));

            if (playerShare >= 58f)
                return RivalMove.MarketingBlitz;
            if (playerShare >= 50f)
                return activeVenture == VentureType.FastFood || activeVenture == VentureType.GroceryStore
                    ? RivalMove.PriceWar
                    : RivalMove.MarketingBlitz;
            if (currentTurn >= 15)
                return RivalMove.OpenBranch;
            if (activeVenture == VentureType.TechApp && rivalRating < 3.6f)
                return RivalMove.QualityImprove;
            if (activeVenture == VentureType.Cafe && currentTurn % 4 == 0)
                return RivalMove.StaffPoach;
            return RivalMove.QualityImprove;
        }

        private RivalMove ChooseSecondaryMove(RivalMove primaryMove, int currentTurn)
        {
            if (_sectorController != null)
            {
                var gm = GameManager.Instance;
                float share = gm != null && gm.EconomyManager != null && gm.EconomyManager.Snapshot != null
                    ? gm.EconomyManager.Snapshot.marketShare
                    : 0f;
                return _sectorController.DecideSecondaryMove(primaryMove, GetOrBuildContext(share, currentTurn));
            }

            if (primaryMove == RivalMove.MarketingBlitz)
                return activeVenture == VentureType.TechApp ? RivalMove.QualityImprove : RivalMove.PriceWar;
            if (primaryMove == RivalMove.PriceWar)
                return RivalMove.StaffPoach;
            if (primaryMove == RivalMove.OpenBranch)
                return RivalMove.MarketingBlitz;
            if (activeVenture == VentureType.TechApp && currentTurn >= 12)
                return RivalMove.SeekInvestment;
            return RivalMove.QualityImprove;
        }

        private RivalQueuedAction BuildAction(RivalMove move)
        {
            string lane = _sectorController != null ? _sectorController.GetLaneLabel(move) : move switch
            {
                RivalMove.MarketingBlitz => "Growth Lane",
                RivalMove.PriceWar => "Price Lane",
                RivalMove.QualityImprove => "Quality Lane",
                RivalMove.StaffPoach => "Staff Lane",
                RivalMove.SeekInvestment => "Capital Lane",
                RivalMove.OpenBranch => "Expansion Lane",
                RivalMove.Sabotage => "Risk Lane",
                _ => "Pressure Lane"
            };

            return new RivalQueuedAction
            {
                moveType = move,
                cardId = $"{activeVenture}_{move}",
                displayName = GetMoveCardName(move),
                laneLabel = lane,
                shortDescription = GetMoveDescription(move),
                moodIcon = GetMoveMood(move),
                pressureDelta = GetPressureDelta(move),
                ratingDelta = move == RivalMove.MarketingBlitz ? 0.35f : move == RivalMove.QualityImprove ? 0.15f : 0f,
                qualityDelta = move == RivalMove.QualityImprove ? 0.45f : 0f,
                demandSteal = move == RivalMove.PriceWar ? 5f : move == RivalMove.MarketingBlitz ? 4f : move == RivalMove.OpenBranch ? 6f : 2f
            };
        }

        public void ResolveQueuedAction(RivalQueuedAction action, float playerShare)
        {
            if (action == null)
                return;

            lastPlayedCardName = action.displayName;
            lastLaneLabel = action.laneLabel;
            lastPressureStyle = action.shortDescription;

            rivalPressure += action.pressureDelta;
            rivalRating = Mathf.Clamp(rivalRating + action.ratingDelta, 1f, 5f);
            rivalQuality = Mathf.Clamp(rivalQuality + action.qualityDelta, 0f, 10f);

            if (action.moveType == RivalMove.StaffPoach && playerShare > 45f)
                rivalMomentumCustomers += 3f;
            else
                rivalMomentumCustomers += action.demandSteal;

            switch (action.moveType)
            {
                case RivalMove.PriceWar:
                    rivalMomentumCustomers += 2f;
                    break;
                case RivalMove.SeekInvestment:
                    rivalMoney += 120;
                    break;
                case RivalMove.OpenBranch:
                    rivalIncome += 15;
                    break;
                case RivalMove.StaffPoach:
                    totalRivalEmployees++;
                    break;
                case RivalMove.Sabotage:
                    var gm = GameManager.Instance;
                    if (gm != null && gm.BoardManager != null)
                        gm.BoardManager.SetProductionDisabledNextTurn(true);
                    break;
            }
        }

        private string GetRivalName(VentureType venture)
        {
            var techCategory = GameManager.Instance != null ? GameManager.Instance.ActiveTechCategoryProfile : null;
            return venture switch
            {
                VentureType.FastFood => "Rival Kitchen",
                VentureType.Cafe => "Rival Cafe",
                VentureType.TechApp => techCategory != null && !string.IsNullOrWhiteSpace(techCategory.rivalName) ? techCategory.rivalName : "Rival App",
                VentureType.ClothingStore => "Rival Boutique",
                VentureType.GroceryStore => "Rival Market",
                _ => "Rival Co."
            };
        }

        private string GetMoveDescription(RivalMove move)
        {
            if (_sectorController != null && _cachedContext.HasValue)
                return _sectorController.GetMoveDescription(move, _cachedContext.Value);

            return RivalMoveTextProvider.GetDescription(move, activeVenture);
        }

        private string DescribeAction(RivalQueuedAction action)
        {
            return $"{action.displayName} -> {action.laneLabel}";
        }

        private string GetMoveCardName(RivalMove move)
        {
            if (_sectorController != null && _cachedContext.HasValue)
                return _sectorController.GetMoveCardName(move, _cachedContext.Value);

            return RivalMoveTextProvider.GetCardName(move, activeVenture);
        }

        private string GetMoveMood(RivalMove move)
        {
            if (_sectorController != null)
                return _sectorController.GetMoveMood(move);

            return move switch
            {
                RivalMove.PriceWar => "$",
                RivalMove.MarketingBlitz => "AD",
                RivalMove.QualityImprove => "Q",
                RivalMove.StaffPoach => "HR",
                RivalMove.SeekInvestment => "VC",
                RivalMove.OpenBranch => "++",
                RivalMove.Sabotage => "!",
                _ => "..."
            };
        }

        private float GetPressureDelta(RivalMove move)
        {
            if (_sectorController != null && _cachedContext.HasValue)
                return _sectorController.GetPressureDelta(move, _cachedContext.Value);

            return move switch
            {
                RivalMove.PriceWar => 4.5f,
                RivalMove.MarketingBlitz => 5.2f,
                RivalMove.QualityImprove => 2.4f,
                RivalMove.StaffPoach => 2.8f,
                RivalMove.SeekInvestment => 1.4f,
                RivalMove.OpenBranch => 3.4f,
                RivalMove.Sabotage => 2.1f,
                _ => 1f
            };
        }

        private RivalSectorContext GetOrBuildContext(float playerShare, int currentTurn)
        {
            if (_cachedContext.HasValue)
                return _cachedContext.Value;
            return BuildSectorContext(playerShare, currentTurn);
        }

        private RivalSectorContext BuildSectorContext(float playerShare, int currentTurn)
        {
            var gm = GameManager.Instance;
            return new RivalSectorContext
            {
                ventureType = activeVenture,
                playerShare = playerShare,
                currentTurn = currentTurn,
                rivalRating = rivalRating,
                rivalQuality = rivalQuality,
                rivalPressure = rivalPressure,
                businessCount = gm != null && gm.BoardManager != null ? gm.BoardManager.GetActiveBusinessCount() : 0,
                employeeCount = totalRivalEmployees,
                playerPressure = gm != null && gm.EconomyManager != null ? gm.EconomyManager.CurrentPressure : BoardPressureType.None,
                playerSnapshot = gm != null && gm.EconomyManager != null ? gm.EconomyManager.Snapshot : null,
                rivalState = _runtimeState,
                runCategoryLabel = gm != null ? gm.RunCategoryLabel : null
            };
        }
    }
}
