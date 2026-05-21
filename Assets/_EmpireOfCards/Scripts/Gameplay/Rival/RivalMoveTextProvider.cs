using System.Collections.Generic;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay.Rival
{
    public static class RivalMoveTextProvider
    {
        private static readonly Dictionary<(VentureType, RivalMove), string> Descriptions = new()
        {
            // FastFood
            { (VentureType.FastFood, RivalMove.PriceWar), "Rival is undercutting combo prices and stealing rush-hour baskets." },
            { (VentureType.FastFood, RivalMove.MarketingBlitz), "Rival is pushing delivery apps, flyers, and Google review momentum." },
            { (VentureType.FastFood, RivalMove.QualityImprove), "Rival is tightening kitchen quality and speed." },
            { (VentureType.FastFood, RivalMove.StaffPoach), "Rival is grabbing the cleaner crew and counter staff first." },
            { (VentureType.FastFood, RivalMove.OpenBranch), "Rival is opening another traffic-capturing counter nearby." },
            { (VentureType.FastFood, RivalMove.SeekInvestment), "Rival fast food chain secured expansion capital." },
            { (VentureType.FastFood, RivalMove.Sabotage), "Rival is jamming your rush lane with queue disruption." },

            // Cafe
            { (VentureType.Cafe, RivalMove.PriceWar), "Rival is discounting drinks to break your morning routine hold." },
            { (VentureType.Cafe, RivalMove.MarketingBlitz), "Rival is leaning on Maps, Reels, and neighborhood buzz." },
            { (VentureType.Cafe, RivalMove.QualityImprove), "Rival is improving bean quality and drink consistency." },
            { (VentureType.Cafe, RivalMove.StaffPoach), "Rival is fishing for baristas and floor talent." },
            { (VentureType.Cafe, RivalMove.OpenBranch), "Rival is widening its footprint in the local coffee circuit." },
            { (VentureType.Cafe, RivalMove.SeekInvestment), "Rival cafe secured roastery credit to stay aggressive." },
            { (VentureType.Cafe, RivalMove.Sabotage), "Rival is weaponizing rush-hour slowness against your reviews." },

            // ClothingStore
            { (VentureType.ClothingStore, RivalMove.PriceWar), "Rival is discounting hard to clear stock and drag your margin down." },
            { (VentureType.ClothingStore, RivalMove.MarketingBlitz), "Rival is pushing storefront visuals and social fashion traffic." },
            { (VentureType.ClothingStore, RivalMove.QualityImprove), "Rival is investing in fit, tailoring, and fabric trust." },
            { (VentureType.ClothingStore, RivalMove.StaffPoach), "Rival is targeting stylists and tailoring talent." },
            { (VentureType.ClothingStore, RivalMove.OpenBranch), "Rival is expanding its fashion footprint into your lane." },
            { (VentureType.ClothingStore, RivalMove.SeekInvestment), "Rival boutique secured seasonal credit." },
            { (VentureType.ClothingStore, RivalMove.Sabotage), "Rival is turning return pressure into a public trust problem." },

            // GroceryStore
            { (VentureType.GroceryStore, RivalMove.PriceWar), "Rival is cutting staple prices to win neighborhood baskets." },
            { (VentureType.GroceryStore, RivalMove.MarketingBlitz), "Rival is pushing convenience, WhatsApp orders, and late-night pull." },
            { (VentureType.GroceryStore, RivalMove.QualityImprove), "Rival is stabilizing freshness and checkout trust." },
            { (VentureType.GroceryStore, RivalMove.StaffPoach), "Rival is pulling reliable cashiers and stockers off the block." },
            { (VentureType.GroceryStore, RivalMove.OpenBranch), "Rival is stretching into another local convenience pocket." },
            { (VentureType.GroceryStore, RivalMove.SeekInvestment), "Rival market secured distributor credit." },
            { (VentureType.GroceryStore, RivalMove.Sabotage), "Rival is creating shelf panic around your weak freshness lane." },
        };

        private static readonly Dictionary<(VentureType, RivalMove), string> CardNames = new()
        {
            // FastFood
            { (VentureType.FastFood, RivalMove.PriceWar), "Combo Price Slash" },
            { (VentureType.FastFood, RivalMove.MarketingBlitz), "Delivery Blitz" },
            { (VentureType.FastFood, RivalMove.QualityImprove), "Kitchen Tune-Up" },
            { (VentureType.FastFood, RivalMove.StaffPoach), "Counter Staff Poach" },
            { (VentureType.FastFood, RivalMove.SeekInvestment), "Expansion Cash" },
            { (VentureType.FastFood, RivalMove.OpenBranch), "Street Corner Lease" },
            { (VentureType.FastFood, RivalMove.Sabotage), "Queue Disruption" },

            // Cafe
            { (VentureType.Cafe, RivalMove.PriceWar), "Morning Discount Push" },
            { (VentureType.Cafe, RivalMove.MarketingBlitz), "Neighborhood Buzz" },
            { (VentureType.Cafe, RivalMove.QualityImprove), "Bean Quality Pass" },
            { (VentureType.Cafe, RivalMove.StaffPoach), "Barista Poach" },
            { (VentureType.Cafe, RivalMove.SeekInvestment), "Roastery Credit" },
            { (VentureType.Cafe, RivalMove.OpenBranch), "Second Corner Lease" },
            { (VentureType.Cafe, RivalMove.Sabotage), "Rush-Hour Disruption" },

            // ClothingStore
            { (VentureType.ClothingStore, RivalMove.PriceWar), "Clearance Drop" },
            { (VentureType.ClothingStore, RivalMove.MarketingBlitz), "Lookbook Surge" },
            { (VentureType.ClothingStore, RivalMove.QualityImprove), "Fit & Fabric Pass" },
            { (VentureType.ClothingStore, RivalMove.StaffPoach), "Stylist Poach" },
            { (VentureType.ClothingStore, RivalMove.SeekInvestment), "Seasonal Credit" },
            { (VentureType.ClothingStore, RivalMove.OpenBranch), "Mall Pop-Up" },
            { (VentureType.ClothingStore, RivalMove.Sabotage), "Return Pressure Spike" },

            // GroceryStore
            { (VentureType.GroceryStore, RivalMove.PriceWar), "Staple Basket Cut" },
            { (VentureType.GroceryStore, RivalMove.MarketingBlitz), "Convenience Push" },
            { (VentureType.GroceryStore, RivalMove.QualityImprove), "Freshness Pass" },
            { (VentureType.GroceryStore, RivalMove.StaffPoach), "Cashier Poach" },
            { (VentureType.GroceryStore, RivalMove.SeekInvestment), "Distributor Credit" },
            { (VentureType.GroceryStore, RivalMove.OpenBranch), "Neighborhood Annex" },
            { (VentureType.GroceryStore, RivalMove.Sabotage), "Shelf Panic" },
        };

        private static readonly Dictionary<RivalMove, string> FallbackDescriptions = new()
        {
            { RivalMove.PriceWar, "Rival cuts price to drag demand away." },
            { RivalMove.MarketingBlitz, "Rival buys visibility and review momentum." },
            { RivalMove.QualityImprove, "Rival invests in quality and trust." },
            { RivalMove.StaffPoach, "Rival pressures your staffing edge." },
            { RivalMove.SeekInvestment, "Rival secures new capital to stay aggressive." },
            { RivalMove.OpenBranch, "Rival expands footprint into your district." },
            { RivalMove.Sabotage, "Rival pushes operational disruption." },
        };

        private static readonly Dictionary<RivalMove, string> FallbackCardNames = new()
        {
            { RivalMove.PriceWar, "Price Drop Campaign" },
            { RivalMove.MarketingBlitz, "Visibility Blitz" },
            { RivalMove.QualityImprove, "Quality Sprint" },
            { RivalMove.StaffPoach, "Staff Poach" },
            { RivalMove.SeekInvestment, "Funding Round" },
            { RivalMove.OpenBranch, "Expansion Lease" },
            { RivalMove.Sabotage, "Ops Disruption" },
        };

        public static string GetDescription(RivalMove move, VentureType venture)
        {
            if (Descriptions.TryGetValue((venture, move), out var desc))
                return desc;
            if (FallbackDescriptions.TryGetValue(move, out var fallback))
                return fallback;
            return "Rival adapts.";
        }

        public static string GetCardName(RivalMove move, VentureType venture)
        {
            if (CardNames.TryGetValue((venture, move), out var name))
                return name;
            if (FallbackCardNames.TryGetValue(move, out var fallback))
                return fallback;
            return "Pressure Shift";
        }
    }
}
