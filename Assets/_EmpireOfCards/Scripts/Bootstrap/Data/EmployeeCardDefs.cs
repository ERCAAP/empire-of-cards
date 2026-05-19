using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 14 Employee cards.
    /// </summary>
    public static class EmployeeCardDefs
    {
        public static CardData[] Create()
        {
            var cards = new CardData[14];

            // C01 - Intern
            cards[0] = CardHelper.CreateEmployee("C01_Stajyer", "Intern", Rarity.Common,
                "Cheap but weak. Starter card.", 15,
                tags: new[] { CardTag.Basic });
            cards[0].customerBonus = 1;
            cards[0].activeAbilityType = ActiveAbilityType.AddCustomersThisTurn;
            cards[0].activeAbilityName = "Hustle";
            cards[0].activeAbilityDesc = "Adds +3 customers to this business this turn.";
            cards[0].abilityValue2 = 3;

            // C02 - Junior Marketer
            cards[1] = CardHelper.CreateEmployee("C02_CaylakPazarlamaci", "Junior Marketer", Rarity.Common,
                "Small but consistent bonus.", 20,
                tags: new[] { CardTag.Marketing, CardTag.Basic });
            cards[1].incomeMultiplier = 0.10f;
            cards[1].activeAbilityType = ActiveAbilityType.AddCustomersThisTurn;
            cards[1].activeAbilityName = "Campaign";
            cards[1].activeAbilityDesc = "Adds +5 customers to this business this turn.";
            cards[1].abilityValue2 = 5;

            // C03 - Barista
            cards[2] = CardHelper.CreateEmployee("C03_Barista", "Barista", Rarity.Uncommon,
                "Doubles in coffee shops.", 25,
                tags: new[] { CardTag.Food, CardTag.Coffee });
            cards[2].customerBonus = 3;
            cards[2].synergyCustomerBonus = 6;
            cards[2].synergyTag = CardTag.Coffee;
            cards[2].activeAbilityType = ActiveAbilityType.MultiplyCustomersThisTurn;
            cards[2].activeAbilityName = "Latte Festival";
            cards[2].activeAbilityDesc = "Customers x2 this turn.";
            cards[2].abilityValue1 = 2f;

            // C04 - Chef
            cards[3] = CardHelper.CreateEmployee("C04_Sef", "Chef", Rarity.Uncommon,
                "Strong in food sector.", 30,
                tags: new[] { CardTag.Food });
            cards[3].customerBonus = 3;
            cards[3].incomeFlatBonus = 30f;
            cards[3].incomeBonusTag = CardTag.Food;
            cards[3].activeAbilityType = ActiveAbilityType.MultiplyIncomeThisTurn;
            cards[3].activeAbilityName = "Special Menu";
            cards[3].activeAbilityDesc = "Income x1.5 this turn.";
            cards[3].abilityValue1 = 1.5f;

            // C05 - Marketing Guru
            cards[4] = CardHelper.CreateEmployee("C05_MarketingGurusu", "Marketing Guru", Rarity.Rare,
                "Expensive but powerful. Combo piece.", 45,
                tags: new[] { CardTag.Marketing, CardTag.Guru });
            cards[4].incomeMultiplier = 0.25f;
            cards[4].activeAbilityType = ActiveAbilityType.AddCustomersToAll;
            cards[4].activeAbilityName = "Viral Campaign";
            cards[4].activeAbilityDesc = "+3 customers to all businesses.";
            cards[4].abilityValue2 = 3;

            // C06 - Influencer
            cards[5] = CardHelper.CreateEmployee("C06_Influencer", "Influencer", Rarity.Rare,
                "Explodes during trends. Average otherwise.", 50,
                tags: new[] { CardTag.Marketing, CardTag.Influencer, CardTag.Trendy });
            cards[5].customerBonus = 5;
            cards[5].synergyCustomerBonus = 12;
            cards[5].synergyTag = CardTag.Trendy;
            cards[5].activeAbilityType = ActiveAbilityType.StealCustomersFromRival;
            cards[5].activeAbilityName = "Post Story";
            cards[5].activeAbilityDesc = "Steal 5 customers from rival.";
            cards[5].abilityValue2 = 5;

            // C07 - Hacker
            cards[6] = CardHelper.CreateEmployee("C07_Hacker", "Hacker", Rarity.Rare,
                "Powerful but dangerous. FBI risk every turn.", 60,
                tags: new[] { CardTag.Tech, CardTag.Illegal });
            cards[6].customerBonus = 0;
            cards[6].fbiRiskPerTurn = 10;
            cards[6].activeAbilityType = ActiveAbilityType.None;
            cards[6].activeAbilityName = "Passive Infiltration";
            cards[6].activeAbilityDesc = "Steals 4 customers from rival each turn (passive).";

            // C08 - Accountant
            cards[7] = CardHelper.CreateEmployee("C08_Muhasebeci", "Accountant", Rarity.Uncommon,
                "Boring but saves every penny.", 30,
                tags: new[] { CardTag.Finance });
            cards[7].taxReduction = 0.5f;
            cards[7].activeAbilityType = ActiveAbilityType.NullifyTaxThisTurn;
            cards[7].activeAbilityName = "Tax Plan";
            cards[7].activeAbilityDesc = "Tax is nullified this turn.";

            // C09 - Fraudster
            cards[8] = CardHelper.CreateEmployee("C09_Dolandirici", "Fraudster", Rarity.Rare,
                "Fast money. But FBI is knocking.", 40,
                tags: new[] { CardTag.Illegal, CardTag.Finance });
            cards[8].illegalIncomePerTurn = 120;
            cards[8].fbiRiskPerTurn = 12;
            cards[8].activeAbilityType = ActiveAbilityType.BonusIncomeWithPenalty;
            cards[8].activeAbilityName = "Ponzi";
            cards[8].activeAbilityDesc = "+300 instant but -150 next turn.";
            cards[8].abilityValue2 = 300;

            // C10 - Loyal Manager
            cards[9] = CardHelper.CreateEmployee("C10_SadikMudur", "Loyal Manager", Rarity.Uncommon,
                "Rival can't steal employees. Defensive.", 45,
                tags: new[] { CardTag.Management });
            cards[9].customerBonus = 0;
            cards[9].incomeFlatBonus = 20f;
            cards[9].preventsTransfer = true;
            cards[9].activeAbilityType = ActiveAbilityType.MotivateAllEmployees;
            cards[9].activeAbilityName = "Motivation";
            cards[9].activeAbilityDesc = "All employees gain +1 customer this turn.";

            // C11 - Consultant
            // DECISION: Low immediate value but income grows +5 every turn the consultant stays.
            // Patience vs. replacing with a stronger immediate employee.
            // After 6 turns: +30 income (overtakes most employees). Risk: employee leaving mechanic.
            cards[10] = CardHelper.CreateEmployee("C11_Danismani", "Consultant", Rarity.Uncommon,
                "Weak at first. But grows stronger every turn. Patience pays.", 35,
                tags: new[] { CardTag.Consulting, CardTag.Scaling });
            cards[10].customerBonus = 1;
            cards[10].incomeMultiplier = 0.05f;  // starts at +5%, grows via ScaleIncomePerTurn
            cards[10].activeAbilityType = ActiveAbilityType.ScaleIncomePerTurn;
            cards[10].activeAbilityName = "Experience";
            cards[10].activeAbilityDesc = "Income bonus grows +5 each turn this employee stays.";
            cards[10].abilityValue2 = 5;

            // C12 - Bouncer
            // DECISION: Negative income impact (-10 income) but steals 3 customers from rival
            // every turn. Aggressive territory play. Pairs with Nightclub for flavor.
            cards[11] = CardHelper.CreateEmployee("C12_Fedai", "Bouncer", Rarity.Uncommon,
                "Costs you income but intimidates the competition. -3 rival customers/turn.", 25,
                tags: new[] { CardTag.Nightlife, CardTag.Aggressive, CardTag.Defensive });
            cards[11].customerBonus = 2;
            cards[11].incomeFlatBonus = -10f;  // costs income
            cards[11].synergyCustomerBonus = 5;
            cards[11].synergyTag = CardTag.Entertainment;
            cards[11].activeAbilityType = ActiveAbilityType.ReduceRivalCustomers;
            cards[11].activeAbilityName = "Intimidate";
            cards[11].activeAbilityDesc = "Rival loses 3 customers from their strongest business.";
            cards[11].abilityValue2 = 3;

            // C13 - Headhunter
            // DECISION: No stats of their own. Copies a random ally employee's active ability.
            // Wild card — the value depends entirely on who else you have hired.
            // Amazing with Fraudster's Ponzi or Chef's Special Menu, mediocre with Intern's Hustle.
            cards[12] = CardHelper.CreateEmployee("C13_HeadHunter", "Headhunter", Rarity.Rare,
                "No skills. But copies one of your other employees' abilities each turn.", 55,
                tags: new[] { CardTag.Management, CardTag.Hiring });
            cards[12].customerBonus = 0;
            cards[12].activeAbilityType = ActiveAbilityType.CopyRandomEmployeeAbility;
            cards[12].activeAbilityName = "Poach Talent";
            cards[12].activeAbilityDesc = "Activates a random ally employee's ability.";

            // C14 - Lobbyist
            // DECISION: Expensive salary, no direct income. But rival's next business costs 25% more.
            // Pure disruption card. Slows rival expansion but doesn't help you earn.
            cards[13] = CardHelper.CreateEmployee("C14_Lobici", "Lobbyist", Rarity.Rare,
                "Plays dirty politics. Rival businesses cost 25% more.", 60,
                tags: new[] { CardTag.Finance, CardTag.Aggressive });
            cards[13].customerBonus = 0;
            cards[13].activeAbilityType = ActiveAbilityType.SabotageCostIncrease;
            cards[13].activeAbilityName = "Red Tape";
            cards[13].activeAbilityDesc = "Rival's next business purchase costs 25% more.";
            cards[13].abilityValue1 = 0.25f;

            return cards;
        }
    }
}
