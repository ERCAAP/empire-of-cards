using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 12 Business cards.
    /// </summary>
    public static class BusinessCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[12];

            // B01 - Diner
            cards[0] = CardHelper.CreateBusiness("B01_Bufe", "Diner", Rarity.Common,
                "Humble beginnings. Everyone's first business.", 0, 50, 3, 1,
                new[] { CardTag.Food, CardTag.Basic });
            cards[0].canEvolve = true;

            // B02 - Coffee Shop
            cards[1] = CardHelper.CreateBusiness("B02_Kahveci", "Coffee Shop", Rarity.Common,
                "Trend-sensitive. Very profitable at the right time.", 150, 80, 5, 2,
                new[] { CardTag.Food, CardTag.Coffee, CardTag.Trendy });
            cards[1].hasTrendBonus = true;
            cards[1].trendIncomeMultiplier = 1.5f;

            // B03 - Burger Chain
            cards[2] = CardHelper.CreateBusiness("B03_BurgerZinciri", "Burger Chain", Rarity.Uncommon,
                "Many employees = many synergies. But salaries add up.", 250, 100, 6, 3,
                new[] { CardTag.Food, CardTag.Chain });

            // B04 - Tech Startup
            cards[3] = CardHelper.CreateBusiness("B04_TechStartup", "Tech Startup", Rarity.Uncommon,
                "Patience required. But when it hits, it hits big.", 200, 150, 4, 2,
                new[] { CardTag.Tech, CardTag.Startup });
            cards[3].activationDelay = 3;

            // B05 - Nightclub
            cards[4] = CardHelper.CreateBusiness("B05_GeceKulubu", "Nightclub", Rarity.Rare,
                "High reward, high risk. Dead when trends die.", 350, 180, 10, 2,
                new[] { CardTag.Entertainment, CardTag.Nightlife, CardTag.Trendy });
            cards[4].requiresTrendToOperate = true;

            // B06 - Organic Farm
            cards[5] = CardHelper.CreateBusiness("B06_OrganikCiftlik", "Organic Farm", Rarity.Common,
                "Weak alone. But powers up all food businesses.", 120, 40, 2, 1,
                new[] { CardTag.Food, CardTag.Organic, CardTag.Support });
            cards[5].foodBonusTag = "Food";
            cards[5].foodBonusAmount = 20;

            // B07 - Crypto Exchange
            cards[6] = CardHelper.CreateBusiness("B07_KriptoBorsasi", "Crypto Exchange", Rarity.Rare,
                "Gambling. Sometimes zero, sometimes jackpot.", 300, 0, 2, 1,
                new[] { CardTag.Tech, CardTag.Crypto, CardTag.Risky });
            cards[6].hasRandomIncome = true;
            cards[6].randomIncomeMin = 0;
            cards[6].randomIncomeMax = 250;

            // B08 - Ad Agency
            cards[7] = CardHelper.CreateBusiness("B08_ReklamAjansi", "Ad Agency", Rarity.Uncommon,
                "Low income but boosts all your businesses.", 200, 60, 3, 2,
                new[] { CardTag.Marketing, CardTag.Support });
            cards[7].globalCustomerBonus = 2;

            // B09 - Franchise Hub
            // DECISION: High income potential but ONLY if you already have 2+ businesses.
            // Income scales with your board. Worthless early, dominant late.
            cards[8] = CardHelper.CreateBusiness("B09_FranchiseHub", "Franchise Hub", Rarity.Uncommon,
                "Empty alone. But each business you own feeds it +40 income.", 280, 20, 2, 1,
                new[] { CardTag.Chain, CardTag.Franchise, CardTag.Scaling });
            cards[8].foodBonusTag = "";  // uses custom scaling logic, not food bonus
            // Scaling mechanic: +40 income per active business on board (handled in IncomeCalculator)

            // B10 - Luxury Boutique
            // DECISION: Extremely high income, but only 1 employee slot and requires trend.
            // Less risky than Nightclub (partial income without trend) but fewer synergy options.
            cards[9] = CardHelper.CreateBusiness("B10_LuksButik", "Luxury Boutique", Rarity.Rare,
                "Premium goods for premium customers. Trends double the traffic.", 400, 200, 8, 1,
                new[] { CardTag.Luxury, CardTag.Trendy });
            cards[9].hasTrendBonus = true;
            cards[9].trendIncomeMultiplier = 2.0f;

            // B11 - Consulting Firm
            // DECISION: Zero customers (cannot claim territory) but massive income multiplier
            // for ONE other business. Pure economy card — you trade territory pressure for cash.
            cards[10] = CardHelper.CreateBusiness("B11_DanismanlikFirmasi", "Consulting Firm", Rarity.Uncommon,
                "No customers. But your best business earns 40% more.", 220, 70, 0, 2,
                new[] { CardTag.Consulting, CardTag.Support, CardTag.Finance });

            // B12 - Pop-Up Shop
            // DECISION: Costs nothing but self-destructs after 4 turns. Fast tempo play.
            // Do you burn a slot on a temporary business, or save it for something permanent?
            cards[11] = CardHelper.CreateBusiness("B12_PopUpShop", "Pop-Up Shop", Rarity.Common,
                "Free and fast. But closes after 4 turns. Make it count.", 0, 90, 5, 1,
                new[] { CardTag.Basic, CardTag.Trendy });
            cards[11].activationDelay = 0;
            // Self-destruct after 4 turns (handled in BoardManager lifecycle)

            return cards;
        }
    }
}
