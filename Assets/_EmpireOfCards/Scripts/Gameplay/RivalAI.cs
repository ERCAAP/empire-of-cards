using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

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

        public int RivalMoney => rivalMoney;
        public int RivalCustomers => rivalCustomers;
        public int RivalIncome => rivalIncome;
        public IReadOnlyList<RivalBusiness> RivalBusinesses => rivalBusinesses;
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

            gm.EconomyManager?.RegisterRivalPressure(rivalPressure, lastPressureStyle);
            EventBus.RivalActed(_queuedActions.Count > 0 ? DescribeAction(_queuedActions[_queuedActions.Count - 1]) : "Rival adapts.");
            EventBus.RivalMoodChanged(lastPressureStyle);
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
            return GetMoveDescription(DecideMove(playerBlocks * 10f, GameManager.Instance != null ? GameManager.Instance.CurrentTurn : 1));
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
        }

        public List<RivalQueuedAction> BuildQueuedActions(float playerShare, int currentTurn)
        {
            _queuedActions.Clear();
            rivalPressure = Mathf.Max(0f, rivalPressure * 0.65f);

            RivalMove primaryMove = DecideMove(playerShare, currentTurn);
            _queuedActions.Add(BuildAction(primaryMove));

            if (currentTurn >= 8 || playerShare >= 46f)
            {
                RivalMove secondaryMove = ChooseSecondaryMove(primaryMove, currentTurn);
                _queuedActions.Add(BuildAction(secondaryMove));
            }

            return _queuedActions;
        }

        private RivalMove DecideMove(float playerShare, int currentTurn)
        {
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
            string lane = move switch
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

            if (action.displayName.Contains("Poach") && playerShare > 45f)
                rivalMomentumCustomers += 3f;
            else
                rivalMomentumCustomers += action.demandSteal;

            switch (action.displayName)
            {
                case "Price Drop Campaign":
                    rivalMomentumCustomers += 2f;
                    break;
                case "Funding Round":
                    rivalMoney += 120;
                    break;
                case "Expansion Lease":
                    rivalIncome += 15;
                    break;
                case "Staff Poach":
                    totalRivalEmployees++;
                    break;
                case "Ops Disruption":
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
            var techCategory = GameManager.Instance != null ? GameManager.Instance.ActiveTechCategoryProfile : null;
            if (activeVenture == VentureType.TechApp && techCategory != null)
            {
                return move switch
                {
                    RivalMove.PriceWar => $"{techCategory.displayName} rival is undercutting acquisition efficiency and install conversion.",
                    RivalMove.MarketingBlitz => $"{techCategory.displayName} rival is buying visibility, creators, and review momentum.",
                    RivalMove.QualityImprove => $"{techCategory.displayName} rival is tightening product quality and trust.",
                    RivalMove.StaffPoach => $"{techCategory.displayName} rival is competing for scarce product talent.",
                    RivalMove.SeekInvestment => $"{techCategory.displayName} rival secured capital to keep burning aggressively.",
                    RivalMove.OpenBranch => $"{techCategory.displayName} rival is widening footprint across a new channel.",
                    RivalMove.Sabotage => $"{techCategory.displayName} rival is pressuring your weakest operational layer.",
                    _ => "Rival app adapts."
                };
            }

            if (activeVenture == VentureType.FastFood)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Rival is undercutting combo prices and stealing rush-hour baskets.",
                    RivalMove.MarketingBlitz => "Rival is pushing delivery apps, flyers, and Google review momentum.",
                    RivalMove.QualityImprove => "Rival is tightening kitchen quality and speed.",
                    RivalMove.StaffPoach => "Rival is grabbing the cleaner crew and counter staff first.",
                    RivalMove.OpenBranch => "Rival is opening another traffic-capturing counter nearby.",
                    _ => "Rival kitchen is adapting."
                };
            }

            if (activeVenture == VentureType.Cafe)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Rival is discounting drinks to break your morning routine hold.",
                    RivalMove.MarketingBlitz => "Rival is leaning on Maps, Reels, and neighborhood buzz.",
                    RivalMove.QualityImprove => "Rival is improving bean quality and drink consistency.",
                    RivalMove.StaffPoach => "Rival is fishing for baristas and floor talent.",
                    RivalMove.OpenBranch => "Rival is widening its footprint in the local coffee circuit.",
                    _ => "Rival cafe is adapting."
                };
            }

            if (activeVenture == VentureType.ClothingStore)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Rival is discounting hard to clear stock and drag your margin down.",
                    RivalMove.MarketingBlitz => "Rival is pushing storefront visuals and social fashion traffic.",
                    RivalMove.QualityImprove => "Rival is investing in fit, tailoring, and fabric trust.",
                    RivalMove.StaffPoach => "Rival is targeting stylists and tailoring talent.",
                    RivalMove.OpenBranch => "Rival is expanding its fashion footprint into your lane.",
                    _ => "Rival boutique is adapting."
                };
            }

            if (activeVenture == VentureType.GroceryStore)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Rival is cutting staple prices to win neighborhood baskets.",
                    RivalMove.MarketingBlitz => "Rival is pushing convenience, WhatsApp orders, and late-night pull.",
                    RivalMove.QualityImprove => "Rival is stabilizing freshness and checkout trust.",
                    RivalMove.StaffPoach => "Rival is pulling reliable cashiers and stockers off the block.",
                    RivalMove.OpenBranch => "Rival is stretching into another local convenience pocket.",
                    _ => "Rival market is adapting."
                };
            }

            return move switch
            {
                RivalMove.PriceWar => "Rival cuts price to drag demand away.",
                RivalMove.MarketingBlitz => "Rival buys visibility and review momentum.",
                RivalMove.QualityImprove => "Rival invests in quality and trust.",
                RivalMove.StaffPoach => "Rival pressures your staffing edge.",
                RivalMove.SeekInvestment => "Rival secures new capital to stay aggressive.",
                RivalMove.OpenBranch => "Rival expands footprint into your district.",
                RivalMove.Sabotage => "Rival pushes operational disruption.",
                _ => "Rival adapts."
            };
        }

        private string DescribeAction(RivalQueuedAction action)
        {
            return $"{action.displayName} -> {action.laneLabel}";
        }

        private string GetMoveCardName(RivalMove move)
        {
            var techCategory = GameManager.Instance != null ? GameManager.Instance.ActiveTechCategoryProfile : null;
            if (activeVenture == VentureType.TechApp && techCategory != null)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Acquisition Undercut",
                    RivalMove.MarketingBlitz => "Channel Surge",
                    RivalMove.QualityImprove => "Product Reliability Pass",
                    RivalMove.StaffPoach => "Talent Poach",
                    RivalMove.SeekInvestment => "Growth Round",
                    RivalMove.OpenBranch => "Platform Expansion",
                    RivalMove.Sabotage => "Trust Disruption",
                    _ => "Pressure Shift"
                };
            }

            if (activeVenture == VentureType.FastFood)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Combo Price Slash",
                    RivalMove.MarketingBlitz => "Delivery Blitz",
                    RivalMove.QualityImprove => "Kitchen Tune-Up",
                    RivalMove.StaffPoach => "Counter Staff Poach",
                    RivalMove.SeekInvestment => "Expansion Cash",
                    RivalMove.OpenBranch => "Street Corner Lease",
                    RivalMove.Sabotage => "Queue Disruption",
                    _ => "Pressure Shift"
                };
            }

            if (activeVenture == VentureType.Cafe)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Morning Discount Push",
                    RivalMove.MarketingBlitz => "Neighborhood Buzz",
                    RivalMove.QualityImprove => "Bean Quality Pass",
                    RivalMove.StaffPoach => "Barista Poach",
                    RivalMove.SeekInvestment => "Roastery Credit",
                    RivalMove.OpenBranch => "Second Corner Lease",
                    RivalMove.Sabotage => "Rush-Hour Disruption",
                    _ => "Pressure Shift"
                };
            }

            if (activeVenture == VentureType.ClothingStore)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Clearance Drop",
                    RivalMove.MarketingBlitz => "Lookbook Surge",
                    RivalMove.QualityImprove => "Fit & Fabric Pass",
                    RivalMove.StaffPoach => "Stylist Poach",
                    RivalMove.SeekInvestment => "Seasonal Credit",
                    RivalMove.OpenBranch => "Mall Pop-Up",
                    RivalMove.Sabotage => "Return Pressure Spike",
                    _ => "Pressure Shift"
                };
            }

            if (activeVenture == VentureType.GroceryStore)
            {
                return move switch
                {
                    RivalMove.PriceWar => "Staple Basket Cut",
                    RivalMove.MarketingBlitz => "Convenience Push",
                    RivalMove.QualityImprove => "Freshness Pass",
                    RivalMove.StaffPoach => "Cashier Poach",
                    RivalMove.SeekInvestment => "Distributor Credit",
                    RivalMove.OpenBranch => "Neighborhood Annex",
                    RivalMove.Sabotage => "Shelf Panic",
                    _ => "Pressure Shift"
                };
            }

            return move switch
            {
                RivalMove.PriceWar => "Price Drop Campaign",
                RivalMove.MarketingBlitz => "Visibility Blitz",
                RivalMove.QualityImprove => "Quality Sprint",
                RivalMove.StaffPoach => "Staff Poach",
                RivalMove.SeekInvestment => "Funding Round",
                RivalMove.OpenBranch => "Expansion Lease",
                RivalMove.Sabotage => "Ops Disruption",
                _ => "Pressure Shift"
            };
        }

        private string GetMoveMood(RivalMove move)
        {
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
    }
}
