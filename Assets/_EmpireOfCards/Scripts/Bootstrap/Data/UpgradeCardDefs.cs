using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 8 Upgrade cards.
    /// </summary>
    public static class UpgradeCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[8];

            // U01 - Office Supplies
            cards[0] = CardHelper.CreateUpgrade("U01_OfficeSupplies", "Office Supplies", Rarity.Common,
                "Small but free. Starter card.", 0,
                UpgradeEffectType.IncomePercentSingle, 10f, false, 0, 0,
                new[] { CardTag.Basic, CardTag.Office });

            // U02 - Automation
            cards[1] = CardHelper.CreateUpgrade("U02_Automation", "Automation", Rarity.Uncommon,
                "Strong income boost. But loses an employee slot.", 300,
                UpgradeEffectType.IncomePercentWithSlotLoss, 30f, false, 1, 0,
                new[] { CardTag.Tech, CardTag.Automation });

            // U03 - Delivery Network
            cards[2] = CardHelper.CreateUpgrade("U03_DeliveryNetwork", "Delivery Network", Rarity.Uncommon,
                "Very valuable with multiple businesses.", 250,
                UpgradeEffectType.GlobalCustomerPerTurn, 2f, true, 0, 0,
                new[] { CardTag.Logistics });

            // U04 - Billboard
            cards[3] = CardHelper.CreateUpgrade("U04_Billboard", "Billboard", Rarity.Common,
                "Cheap, simple, effective.", 120,
                UpgradeEffectType.GlobalCustomerFlat, 3f, true, 0, 0,
                new[] { CardTag.Marketing });

            // U05 - Security System
            cards[4] = CardHelper.CreateUpgrade("U05_SecuritySystem", "Security System", Rarity.Uncommon,
                "Essential for illegal strategies.", 280,
                UpgradeEffectType.ReduceFBIRisk, 25f, true, 0, 0,
                new[] { CardTag.Security });

            // U06 - AI Assistant
            cards[5] = CardHelper.CreateUpgrade("U06_AIAssistant", "AI Assistant", Rarity.Rare,
                "The game's strongest upgrade. Extra action per turn.", 400,
                UpgradeEffectType.ExtraAction, 0f, true, 0, 1,
                new[] { CardTag.Tech, CardTag.AI });

            // U07 - Break Room
            // DECISION: +15 income per employee in the business. Worthless on a 1-slot business.
            // Amazing on Burger Chain (3 slots = +45). Competes with Automation for the slot.
            // Automation gives +30% but kills a slot; Break Room rewards FILLING slots.
            cards[6] = CardHelper.CreateUpgrade("U07_BreakRoom", "Break Room", Rarity.Uncommon,
                "Happy workers, happy income. +15 per employee in this business.", 180,
                UpgradeEffectType.IncomePerEmployeeSingle, 15f, false, 0, 0,
                new[] { CardTag.Office, CardTag.Management });

            // U08 - Patent Wall
            // DECISION: Global upgrade that makes rival's businesses cost 25% more.
            // Defensive and disruptive. Does nothing for your income.
            // Costs 320 that could buy a business. Worth it only if rival is expanding.
            cards[7] = CardHelper.CreateUpgrade("U08_PatentWall", "Patent Wall", Rarity.Rare,
                "Bureaucracy as a weapon. Rival businesses cost 25% more.", 320,
                UpgradeEffectType.RivalCostIncrease, 25f, true, 0, 0,
                new[] { CardTag.Tech, CardTag.Defensive });

            return cards;
        }
    }
}
