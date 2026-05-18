using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 6 Event cards.
    /// </summary>
    public static class EventCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[6];

            // E01 - Coffee Craze
            cards[0] = CardHelper.CreateEvent("E01_KahveCilginligi", "Coffee Craze", Rarity.Common,
                "Food sector booming.", 2,
                EventEffectType.TagCustomerBoost, 0.5f,
                new[] { CardTag.Food, CardTag.Coffee }, 0, 0f);

            // E02 - Economic Crisis
            cards[1] = CardHelper.CreateEvent("E02_EkonomikKriz", "Economic Crisis", Rarity.Common,
                "Everyone suffers. But the prepared find opportunity.", 2,
                EventEffectType.AllIncomeReduction, -0.3f,
                new CardTag[0], 0, 0f);

            // E03 - Viral Trend
            cards[2] = CardHelper.CreateEvent("E03_ViralTrend", "Viral Trend", Rarity.Uncommon,
                "Marketing-heavy strategy shines here.", 1,
                EventEffectType.TagDoubleEffect, 1.0f,
                new[] { CardTag.Marketing }, 0, 0f);

            // E04 - Data Breach
            cards[3] = CardHelper.CreateEvent("E04_VeriSizintisi", "Data Breach", Rarity.Uncommon,
                "Tech-focused beware. Security investment matters.", 1,
                EventEffectType.TagCustomerPenalty, 0f,
                new[] { CardTag.Tech }, -5, 0f);

            // E05 - Investor Season
            cards[4] = CardHelper.CreateEvent("E05_YatirimciSezonu", "Investor Season", Rarity.Uncommon,
                "Play investment cards this turn = jackpot.", 1,
                EventEffectType.TagDoubleEffectFinance, 1.0f,
                new[] { CardTag.Finance }, 0, 0f);

            // E06 - Cancel Culture
            cards[5] = CardHelper.CreateEvent("E06_IptalKulturu", "Cancel Culture", Rarity.Rare,
                "Disaster for dirty players. Opportunity for clean ones.", 1,
                EventEffectType.HighFBICustomerPenalty, -0.4f,
                new CardTag[0], 0, 0.3f);

            return cards;
        }
    }
}
