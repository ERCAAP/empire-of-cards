using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public struct RivalSectorContext
    {
        public VentureType ventureType;
        public float playerShare;
        public int currentTurn;
        public float rivalRating;
        public float rivalQuality;
        public float rivalPressure;
        public int businessCount;
        public int employeeCount;
        public BoardPressureType playerPressure;
        public VentureBoardSnapshot playerSnapshot;
        public RivalRuntimeState rivalState;
        public string runCategoryLabel;
    }

    public interface IRivalSectorController
    {
        RivalMove DecidePrimaryMove(RivalSectorContext context);
        RivalMove DecideSecondaryMove(RivalMove primaryMove, RivalSectorContext context);
        string GetLaneLabel(RivalMove move);
        string GetMoveDescription(RivalMove move, RivalSectorContext context);
        string GetMoveCardName(RivalMove move, RivalSectorContext context);
        string GetMoveMood(RivalMove move);
        float GetPressureDelta(RivalMove move, RivalSectorContext context);
    }

    internal abstract class BaseRivalSectorController : IRivalSectorController
    {
        public virtual RivalMove DecidePrimaryMove(RivalSectorContext context)
        {
            if (context.currentTurn <= 2)
                return RivalMove.QualityImprove;
            if (context.playerShare >= 56f)
                return RivalMove.MarketingBlitz;
            if (context.currentTurn >= 14)
                return RivalMove.OpenBranch;
            if (context.playerPressure == BoardPressureType.LowRating || context.playerPressure == BoardPressureType.WeakQuality)
                return RivalMove.PriceWar;
            return RivalMove.QualityImprove;
        }

        public virtual RivalMove DecideSecondaryMove(RivalMove primaryMove, RivalSectorContext context)
        {
            if (primaryMove == RivalMove.MarketingBlitz)
                return RivalMove.PriceWar;
            if (primaryMove == RivalMove.PriceWar)
                return RivalMove.StaffPoach;
            if (context.currentTurn >= 10 && context.rivalState.escalationLevel >= 2)
                return RivalMove.OpenBranch;
            return RivalMove.QualityImprove;
        }

        public virtual string GetLaneLabel(RivalMove move)
        {
            return move switch
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
        }

        public abstract string GetMoveDescription(RivalMove move, RivalSectorContext context);
        public abstract string GetMoveCardName(RivalMove move, RivalSectorContext context);

        public virtual string GetMoveMood(RivalMove move)
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

        public virtual float GetPressureDelta(RivalMove move, RivalSectorContext context)
        {
            float baseDelta = move switch
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

            return baseDelta + context.rivalState.escalationLevel * 0.35f;
        }
    }

    internal sealed class FastFoodRivalSectorController : BaseRivalSectorController
    {
        public override RivalMove DecidePrimaryMove(RivalSectorContext context)
        {
            if (context.currentTurn <= 2)
                return RivalMove.QualityImprove;
            if (context.currentTurn <= 4)
                return RivalMove.MarketingBlitz;
            if (context.playerShare >= 52f)
                return RivalMove.PriceWar;
            if (context.playerPressure == BoardPressureType.CapacityShortfall)
                return RivalMove.MarketingBlitz;
            return RivalMove.QualityImprove;
        }

        public override string GetMoveDescription(RivalMove move, RivalSectorContext context)
        {
            return move switch
            {
                RivalMove.PriceWar => "Rival is undercutting combo prices and stealing rush-hour baskets.",
                RivalMove.MarketingBlitz => "Rival is pushing delivery apps, flyers, and Google review momentum.",
                RivalMove.QualityImprove => "Rival is tightening kitchen quality and speed.",
                RivalMove.StaffPoach => "Rival is grabbing cleaner crew and counter staff first.",
                RivalMove.OpenBranch => "Rival is opening another traffic-capturing counter nearby.",
                RivalMove.Sabotage => "Rival is jamming your rush lane with queue disruption.",
                _ => "Rival kitchen is adapting."
            };
        }

        public override string GetMoveCardName(RivalMove move, RivalSectorContext context)
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
    }

    internal sealed class CafeRivalSectorController : BaseRivalSectorController
    {
        public override RivalMove DecidePrimaryMove(RivalSectorContext context)
        {
            if (context.currentTurn <= 2)
                return RivalMove.QualityImprove;
            if (context.currentTurn <= 4)
                return RivalMove.MarketingBlitz;
            if (context.playerPressure == BoardPressureType.LowRating)
                return RivalMove.MarketingBlitz;
            if (context.playerShare >= 50f)
                return RivalMove.StaffPoach;
            return RivalMove.QualityImprove;
        }

        public override string GetMoveDescription(RivalMove move, RivalSectorContext context)
        {
            return move switch
            {
                RivalMove.PriceWar => "Rival is discounting drinks to break your morning routine hold.",
                RivalMove.MarketingBlitz => "Rival is leaning on Maps, Reels, and neighborhood buzz.",
                RivalMove.QualityImprove => "Rival is improving bean quality and drink consistency.",
                RivalMove.StaffPoach => "Rival is fishing for baristas and floor talent.",
                RivalMove.OpenBranch => "Rival is widening its footprint in the local coffee circuit.",
                RivalMove.Sabotage => "Rival is weaponizing rush-hour slowness against your reviews.",
                _ => "Rival cafe is adapting."
            };
        }

        public override string GetMoveCardName(RivalMove move, RivalSectorContext context)
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
    }

    internal sealed class TechAppRivalSectorController : BaseRivalSectorController
    {
        public override RivalMove DecidePrimaryMove(RivalSectorContext context)
        {
            if (context.currentTurn <= 2)
                return RivalMove.QualityImprove;
            if (context.currentTurn <= 4)
                return RivalMove.SeekInvestment;
            if (context.playerPressure == BoardPressureType.LowRating || context.rivalRating < 3.5f)
                return RivalMove.QualityImprove;
            if (context.playerShare >= 48f)
                return RivalMove.MarketingBlitz;
            return RivalMove.MarketingBlitz;
        }

        public override RivalMove DecideSecondaryMove(RivalMove primaryMove, RivalSectorContext context)
        {
            if (primaryMove == RivalMove.MarketingBlitz)
                return RivalMove.QualityImprove;
            if (context.currentTurn >= 12)
                return RivalMove.OpenBranch;
            return base.DecideSecondaryMove(primaryMove, context);
        }

        public override string GetMoveDescription(RivalMove move, RivalSectorContext context)
        {
            string category = string.IsNullOrWhiteSpace(context.runCategoryLabel) ? "app" : context.runCategoryLabel;
            return move switch
            {
                RivalMove.PriceWar => $"{category} rival is undercutting acquisition efficiency and install conversion.",
                RivalMove.MarketingBlitz => $"{category} rival is buying visibility, creators, and review momentum.",
                RivalMove.QualityImprove => $"{category} rival is tightening product quality and trust.",
                RivalMove.StaffPoach => $"{category} rival is competing for scarce product talent.",
                RivalMove.SeekInvestment => $"{category} rival secured capital to keep burning aggressively.",
                RivalMove.OpenBranch => $"{category} rival is widening footprint across a new channel.",
                RivalMove.Sabotage => $"{category} rival is pressuring your weakest operational layer.",
                _ => "Rival app adapts."
            };
        }

        public override string GetMoveCardName(RivalMove move, RivalSectorContext context)
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
    }

    internal sealed class ClothingRivalSectorController : BaseRivalSectorController
    {
        public override RivalMove DecidePrimaryMove(RivalSectorContext context)
        {
            if (context.currentTurn <= 2)
                return RivalMove.QualityImprove;
            if (context.currentTurn <= 4)
                return RivalMove.MarketingBlitz;
            if (context.playerPressure == BoardPressureType.LowCash)
                return RivalMove.PriceWar;
            if (context.playerShare >= 50f)
                return RivalMove.PriceWar;
            return RivalMove.QualityImprove;
        }

        public override string GetMoveDescription(RivalMove move, RivalSectorContext context)
        {
            return move switch
            {
                RivalMove.PriceWar => "Rival is discounting hard to clear stock and drag your margin down.",
                RivalMove.MarketingBlitz => "Rival is pushing storefront visuals and social fashion traffic.",
                RivalMove.QualityImprove => "Rival is investing in fit, tailoring, and fabric trust.",
                RivalMove.StaffPoach => "Rival is targeting stylists and tailoring talent.",
                RivalMove.OpenBranch => "Rival is expanding its fashion footprint into your lane.",
                RivalMove.Sabotage => "Rival is turning return pressure into a public trust problem.",
                _ => "Rival boutique is adapting."
            };
        }

        public override string GetMoveCardName(RivalMove move, RivalSectorContext context)
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
    }

    internal sealed class GroceryRivalSectorController : BaseRivalSectorController
    {
        public override RivalMove DecidePrimaryMove(RivalSectorContext context)
        {
            if (context.currentTurn <= 2)
                return RivalMove.QualityImprove;
            if (context.currentTurn <= 4)
                return RivalMove.PriceWar;
            if (context.playerPressure == BoardPressureType.CapacityShortfall)
                return RivalMove.MarketingBlitz;
            if (context.playerShare >= 50f)
                return RivalMove.PriceWar;
            return RivalMove.QualityImprove;
        }

        public override string GetMoveDescription(RivalMove move, RivalSectorContext context)
        {
            return move switch
            {
                RivalMove.PriceWar => "Rival is cutting staple prices to win neighborhood baskets.",
                RivalMove.MarketingBlitz => "Rival is pushing convenience, WhatsApp orders, and late-night pull.",
                RivalMove.QualityImprove => "Rival is stabilizing freshness and checkout trust.",
                RivalMove.StaffPoach => "Rival is pulling reliable cashiers and stockers off the block.",
                RivalMove.OpenBranch => "Rival is stretching into another local convenience pocket.",
                RivalMove.Sabotage => "Rival is creating shelf panic around your weak freshness lane.",
                _ => "Rival market is adapting."
            };
        }

        public override string GetMoveCardName(RivalMove move, RivalSectorContext context)
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
    }

    public static class RivalSectorControllerFactory
    {
        public static IRivalSectorController Create(VentureType ventureType)
        {
            return ventureType switch
            {
                VentureType.FastFood => new FastFoodRivalSectorController(),
                VentureType.Cafe => new CafeRivalSectorController(),
                VentureType.TechApp => new TechAppRivalSectorController(),
                VentureType.ClothingStore => new ClothingRivalSectorController(),
                VentureType.GroceryStore => new GroceryRivalSectorController(),
                _ => new FastFoodRivalSectorController()
            };
        }
    }
}
