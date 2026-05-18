using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 8 Business cards.
    /// </summary>
    public static class BusinessCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[8];

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

            return cards;
        }
    }
}
