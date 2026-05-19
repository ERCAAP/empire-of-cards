using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 8 Event cards.
    /// </summary>
    public static class EventCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[8];

            // E01 - Coffee Craze
            cards[0] = CardHelper.CreateEvent("E01_CoffeeCraze", "Coffee Craze", Rarity.Common,
                "Food sector booming.", 2,
                EventEffectType.TagCustomerBoost, 0.5f,
                new[] { CardTag.Food, CardTag.Coffee }, 0, 0f);

            // E02 - Economic Crisis
            cards[1] = CardHelper.CreateEvent("E02_EconomicCrisis", "Economic Crisis", Rarity.Common,
                "Everyone suffers. But the prepared find opportunity.", 2,
                EventEffectType.AllIncomeReduction, -0.3f,
                new CardTag[0], 0, 0f);

            // E03 - Viral Trend
            cards[2] = CardHelper.CreateEvent("E03_ViralTrend", "Viral Trend", Rarity.Uncommon,
                "Marketing-heavy strategy shines here.", 1,
                EventEffectType.TagDoubleEffect, 1.0f,
                new[] { CardTag.Marketing }, 0, 0f);

            // E04 - Data Breach
            cards[3] = CardHelper.CreateEvent("E04_DataBreach", "Data Breach", Rarity.Uncommon,
                "Tech-focused beware. Security investment matters.", 1,
                EventEffectType.TagCustomerPenalty, 0f,
                new[] { CardTag.Tech }, -5, 0f);

            // E05 - Investor Season
            cards[4] = CardHelper.CreateEvent("E05_InvestorSeason", "Investor Season", Rarity.Uncommon,
                "Play investment cards this turn = jackpot.", 1,
                EventEffectType.TagDoubleEffectFinance, 1.0f,
                new[] { CardTag.Finance }, 0, 0f);

            // E06 - Cancel Culture
            cards[5] = CardHelper.CreateEvent("E06_CancelCulture", "Cancel Culture", Rarity.Rare,
                "Disaster for dirty players. Opportunity for clean ones.", 1,
                EventEffectType.HighFBICustomerPenalty, -0.4f,
                new CardTag[0], 0, 0.3f);

            // E07 - Gold Rush
            // DECISION: Every unclaimed territory generates +20 bonus income for BOTH players.
            // Creates a race to claim territories before the opponent profits.
            // Rewards aggressive expansion. Punishes sitting on cash.
            cards[6] = CardHelper.CreateEvent("E07_GoldRush", "Gold Rush", Rarity.Uncommon,
                "Unclaimed land is worth gold. Expand or let your rival profit.", 2,
                EventEffectType.TerritoryScramble, 0.0f,
                new CardTag[0], 0, 0f);

            // E08 - Black Friday
            // DECISION: Shop floods with 5 rare+ cards at 30% off.
            // Both players get access. Creates a spending frenzy.
            // Do you blow your savings on discounted power cards, or let the rival grab them?
            cards[7] = CardHelper.CreateEvent("E08_BlackFriday", "Black Friday", Rarity.Rare,
                "The shop is overflowing with rare deals. Buy fast or lose out.", 1,
                EventEffectType.ShopFloodRare, 0.3f,
                new CardTag[0], 0, 0f);

            return cards;
        }
    }
}
