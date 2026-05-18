using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 6 Upgrade cards.
    /// </summary>
    public static class UpgradeCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[6];

            // U01 - Office Supplies
            cards[0] = CardHelper.CreateUpgrade("U01_OfisMalzemeleri", "Office Supplies", Rarity.Common,
                "Small but free. Starter card.", 0,
                UpgradeEffectType.IncomePercentSingle, 10f, false, 0, 0,
                new[] { CardTag.Basic, CardTag.Office });

            // U02 - Automation
            cards[1] = CardHelper.CreateUpgrade("U02_Otomasyon", "Automation", Rarity.Uncommon,
                "Strong income boost. But loses an employee slot.", 300,
                UpgradeEffectType.IncomePercentWithSlotLoss, 30f, false, 1, 0,
                new[] { CardTag.Tech, CardTag.Automation });

            // U03 - Delivery Network
            cards[2] = CardHelper.CreateUpgrade("U03_TeslimatAgi", "Delivery Network", Rarity.Uncommon,
                "Very valuable with multiple businesses.", 250,
                UpgradeEffectType.GlobalCustomerPerTurn, 2f, true, 0, 0,
                new[] { CardTag.Logistics });

            // U04 - Billboard
            cards[3] = CardHelper.CreateUpgrade("U04_ReklamPanosu", "Billboard", Rarity.Common,
                "Cheap, simple, effective.", 120,
                UpgradeEffectType.GlobalCustomerFlat, 3f, true, 0, 0,
                new[] { CardTag.Marketing });

            // U05 - Security System
            cards[4] = CardHelper.CreateUpgrade("U05_GuvenlikSistemi", "Security System", Rarity.Uncommon,
                "Essential for illegal strategies.", 280,
                UpgradeEffectType.ReduceFBIRisk, 25f, true, 0, 0,
                new[] { CardTag.Security });

            // U06 - AI Assistant
            cards[5] = CardHelper.CreateUpgrade("U06_YapayZekaAsistani", "AI Assistant", Rarity.Rare,
                "The game's strongest upgrade. Extra action per turn.", 400,
                UpgradeEffectType.ExtraAction, 0f, true, 0, 1,
                new[] { CardTag.Tech, CardTag.AI });

            return cards;
        }
    }
}
