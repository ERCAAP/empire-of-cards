using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Definitions for the 10 Combos.
    /// </summary>
    public static class ComboDefs
    {
        public static ComboData[] Create(CardData[] allCards)
        {
            var combos = new ComboData[10];

            // COMBO 01 - Latte Art (Easy)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_01_LatteArt";
                c.comboId = "COMBO_01_LatteArt";
                c.comboName = "Latte Art";
                c.displayText = "LATTE ART!";
                c.tier = ComboTier.Easy;
                c.description = "Coffee Shop + Barista = customers x2, income +50%.";
                c.requiredCardIds = new[] { "B02_Kahveci", "C03_Barista" };
                c.requiredTags = new[] { CardTag.Coffee };
                c.requiresSpecificPlacement = true;
                c.employeeCardId = "C03_Barista";
                c.businessCardId = "B02_Kahveci";
                c.customerMultiplier = 2f;
                c.incomeMultiplier = 1.5f;
                c.glowColor = new Color(0.6f, 0.4f, 0.2f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.3f;
                c.screenShakeDuration = 0.3f;
                combos[0] = c;
            }

            // COMBO 02 - Organic Synergy (Easy)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_02_OrganicSynergy";
                c.comboId = "COMBO_02_OrganicSynergy";
                c.comboName = "Organic Synergy";
                c.displayText = "ORGANIC SYNERGY!";
                c.tier = ComboTier.Easy;
                c.description = "Burger Chain + Chef = income +30, customers +50%.";
                c.requiredCardIds = new[] { "B03_BurgerZinciri", "C04_Sef" };
                c.requiredTags = new[] { CardTag.Food };
                c.requiresSpecificPlacement = true;
                c.employeeCardId = "C04_Sef";
                c.businessCardId = "B03_BurgerZinciri";
                c.bonusIncome = 30;
                c.customerMultiplier = 1.5f;
                c.glowColor = new Color(1f, 0.5f, 0f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.3f;
                c.screenShakeDuration = 0.3f;
                combos[1] = c;
            }

            // COMBO 03 - Viral Storm (Easy)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_03_ViralStorm";
                c.comboId = "COMBO_03_ViralStorm";
                c.comboName = "Viral Storm";
                c.displayText = "VIRAL STORM!";
                c.tier = ComboTier.Easy;
                c.description = "Tech Startup + Marketing Guru = income x2.";
                c.requiredCardIds = new[] { "B04_TechStartup", "C05_MarketingGurusu" };
                c.requiredTags = new[] { CardTag.Tech, CardTag.Marketing };
                c.requiresSpecificPlacement = true;
                c.employeeCardId = "C05_MarketingGurusu";
                c.businessCardId = "B04_TechStartup";
                c.incomeMultiplier = 2f;
                c.glowColor = new Color(0f, 0.8f, 1f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.3f;
                c.screenShakeDuration = 0.3f;
                combos[2] = c;
            }

            // COMBO 04 - Fast Food Empire (Medium)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_04_FastFoodEmpire";
                c.comboId = "COMBO_04_FastFoodEmpire";
                c.comboName = "Fast Food Empire";
                c.displayText = "FAST FOOD EMPIRE!";
                c.tier = ComboTier.Medium;
                c.description = "Nightclub + Influencer + Viral Trend event = customers x3.";
                c.requiredCardIds = new[] { "B05_GeceKulubu", "C06_Influencer" };
                c.requiredTags = new[] { CardTag.Trendy, CardTag.Entertainment };
                c.requiresSpecificPlacement = true;
                c.employeeCardId = "C06_Influencer";
                c.businessCardId = "B05_GeceKulubu";
                c.requiresActiveEvent = true;
                c.requiredEventId = "E03_ViralTrend";
                c.customerMultiplier = 3f;
                c.glowColor = new Color(0.8f, 0f, 1f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.5f;
                c.screenShakeDuration = 0.4f;
                combos[3] = c;
            }

            // COMBO 05 - Underground Empire (Medium)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_05_UndergroundEmpire";
                c.comboId = "COMBO_05_UndergroundEmpire";
                c.comboName = "Underground Empire";
                c.displayText = "UNDERGROUND EMPIRE!";
                c.tier = ComboTier.Medium;
                c.description = "Hacker + Fraudster = +200 income/turn but FBI +8% extra.";
                c.requiredCardIds = new[] { "C07_Hacker", "C09_Dolandirici" };
                c.requiredTags = new[] { CardTag.Illegal };
                c.requiresSpecificPlacement = false;
                c.bonusIncome = 200;
                c.extraFBIRisk = 8;
                c.glowColor = new Color(0.2f, 0.2f, 0.2f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.4f;
                c.screenShakeDuration = 0.3f;
                combos[4] = c;
            }

            // COMBO 06 - Safe Crime (Medium)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_06_SafeCrime";
                c.comboId = "COMBO_06_SafeCrime";
                c.comboName = "Safe Crime";
                c.displayText = "SAFE CRIME!";
                c.tier = ComboTier.Medium;
                c.description = "Accountant + Fraudster = illegal income tax-free.";
                c.requiredCardIds = new[] { "C08_Muhasebeci", "C09_Dolandirici" };
                c.requiredTags = new[] { CardTag.Finance };
                c.requiresSpecificPlacement = false;
                c.bonusIncome = 0;
                c.incomeMultiplier = 1f;
                c.glowColor = new Color(0f, 0.8f, 0f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.3f;
                c.screenShakeDuration = 0.3f;
                combos[5] = c;
            }

            // COMBO 07 - AI Revolution (Hard)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_07_AIRevolution";
                c.comboId = "COMBO_07_AIRevolution";
                c.comboName = "AI Revolution";
                c.displayText = "AI REVOLUTION!";
                c.tier = ComboTier.Hard;
                c.description = "Tech Startup + Automation + AI Assistant = +1 action, income x2.";
                c.requiredCardIds = new[] { "B04_TechStartup", "U02_Otomasyon", "U06_YapayZekaAsistani" };
                c.requiredTags = new[] { CardTag.Tech, CardTag.AI };
                c.requiresSpecificPlacement = false;
                c.extraActions = 1;
                c.incomeMultiplier = 2f;
                c.glowColor = new Color(0f, 1f, 1f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.6f;
                c.screenShakeDuration = 0.5f;
                combos[6] = c;
            }

            // COMBO 08 - Ad Blitz (Medium)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_08_AdBlitz";
                c.comboId = "COMBO_08_AdBlitz";
                c.comboName = "Ad Blitz";
                c.displayText = "AD BLITZ!";
                c.tier = ComboTier.Medium;
                c.description = "Organic Farm + Burger Chain + Chef = all Food businesses +50 income.";
                c.requiredCardIds = new[] { "B06_OrganikCiftlik", "B03_BurgerZinciri", "C04_Sef" };
                c.requiredTags = new[] { CardTag.Food, CardTag.Organic };
                c.requiresSpecificPlacement = false;
                c.bonusIncome = 50;
                c.glowColor = new Color(0.2f, 0.8f, 0.2f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.4f;
                c.screenShakeDuration = 0.4f;
                combos[7] = c;
            }

            // COMBO 09 - Crisis Hunter (Hard)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_09_CrisisHunter";
                c.comboId = "COMBO_09_CrisisHunter";
                c.comboName = "Crisis Hunter";
                c.displayText = "CRISIS HUNTER!";
                c.tier = ComboTier.Hard;
                c.description = "During Economic Crisis with 1000+ money: shop 50% off, steal 1 rival employee.";
                c.requiredCardIds = new string[0];
                c.requiredTags = new[] { CardTag.Finance };
                c.requiresActiveEvent = true;
                c.requiredEventId = "E02_EkonomikKriz";
                c.requiresMinMoney = true;
                c.minMoneyRequired = 1000;
                c.shopDiscount = 0.5f;
                c.transferRivalEmployee = true;
                c.glowColor = new Color(1f, 0.84f, 0f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.5f;
                c.screenShakeDuration = 0.4f;
                combos[8] = c;
            }

            // COMBO 10 - Monopoly (Automatic)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_10_Monopoly";
                c.comboId = "COMBO_10_Monopoly";
                c.comboName = "Monopoly";
                c.displayText = "MONOPOLY!";
                c.tier = ComboTier.Automatic;
                c.description = "4+ businesses, 55%+ market share = rival -3 customers/turn, income +20%.";
                c.requiredCardIds = new string[0];
                c.requiredTags = new CardTag[0];
                c.requiresSpecificPlacement = false;
                c.minActiveBusinesses = 4;
                c.minMarketShare = 0.55f;
                c.rivalCustomerPenalty = 3;
                c.incomeMultiplier = 1.2f;
                c.glowColor = new Color(1f, 0f, 0f, 1f);
                c.comboSoundId = "combo_trigger";
                c.screenShakeIntensity = 0.7f;
                c.screenShakeDuration = 0.6f;
                combos[9] = c;
            }

            return combos;
        }
    }
}
