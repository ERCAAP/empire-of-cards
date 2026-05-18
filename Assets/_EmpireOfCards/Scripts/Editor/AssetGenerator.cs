#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Editor
{
    /// <summary>
    /// Generates ALL ScriptableObject assets from the GDD data.
    /// Menu: EmpireOfCards > Generate All Assets
    /// Creates: GameBalance, 40 Cards, Starting Deck, MegaCorp Rival, 10 Combos, MetaProgression
    /// </summary>
    public static class AssetGenerator
    {
        // -------------------------------------------------------------------
        // Root paths
        // -------------------------------------------------------------------
        private const string DataRoot        = "Assets/_EmpireOfCards/Data";
        private const string CardsRoot       = "Assets/_EmpireOfCards/Data/Cards";
        private const string BusinessCards   = "Assets/_EmpireOfCards/Data/Cards/Business";
        private const string EmployeeCards   = "Assets/_EmpireOfCards/Data/Cards/Employee";
        private const string ActionCards     = "Assets/_EmpireOfCards/Data/Cards/Action";
        private const string UpgradeCards    = "Assets/_EmpireOfCards/Data/Cards/Upgrade";
        private const string EventCards      = "Assets/_EmpireOfCards/Data/Cards/Event";
        private const string DecksRoot       = "Assets/_EmpireOfCards/Data/Decks";
        private const string RivalsRoot      = "Assets/_EmpireOfCards/Data/Rivals";
        private const string CombosRoot      = "Assets/_EmpireOfCards/Data/Combos";

        // -------------------------------------------------------------------
        // MENU ITEM
        // -------------------------------------------------------------------
        [MenuItem("EmpireOfCards/Generate All Assets")]
        public static void GenerateAll()
        {
            EnsureFolders();

            GenerateGameBalance();
            var cards = GenerateAllCards();
            GenerateStartingDeck(cards);
            GenerateMegaCorpRival();
            GenerateCombos(cards);
            GenerateMetaProgression();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[AssetGenerator] All assets generated successfully!");
        }

        // -------------------------------------------------------------------
        // FOLDER CREATION
        // -------------------------------------------------------------------
        private static void EnsureFolders()
        {
            EnsureFolder("Assets/_EmpireOfCards",        "Data");
            EnsureFolder(DataRoot,                       "Cards");
            EnsureFolder(CardsRoot,                      "Business");
            EnsureFolder(CardsRoot,                      "Employee");
            EnsureFolder(CardsRoot,                      "Action");
            EnsureFolder(CardsRoot,                      "Upgrade");
            EnsureFolder(CardsRoot,                      "Event");
            EnsureFolder(DataRoot,                       "Decks");
            EnsureFolder(DataRoot,                       "Rivals");
            EnsureFolder(DataRoot,                       "Combos");
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            string full = parent + "/" + folderName;
            if (!AssetDatabase.IsValidFolder(full))
            {
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        // -------------------------------------------------------------------
        // HELPER: create or overwrite an asset
        // -------------------------------------------------------------------
        private static T CreateOrReplace<T>(string path) where T : ScriptableObject
        {
            T existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                // Reuse existing to preserve references
                return existing;
            }
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void MarkDirty(Object obj)
        {
            EditorUtility.SetDirty(obj);
        }

        // ===================================================================
        // 1. GAME BALANCE
        // ===================================================================
        private static void GenerateGameBalance()
        {
            string path = DataRoot + "/GameBalance.asset";
            var gb = CreateOrReplace<GameBalanceData>(path);

            // General
            gb.startingMoney          = 500;
            gb.maxTurns               = 20;
            gb.startingActions        = 3;
            gb.maxActions             = 5;
            gb.startingBusinessSlots  = 3;
            gb.maxBusinessSlots       = 5;
            gb.handSize               = 5;
            gb.redrawsPerTurn         = 1;
            gb.shopCardsPerTurn       = 3;
            gb.eventInterval          = 3;

            // Economy
            gb.taxRate                = 0.15f;
            gb.reducedTaxRate         = 0.075f;
            gb.minTaxRate             = 0.03f;
            gb.sellRate               = 0.4f;

            // FBI
            gb.fbiRaidPenalty         = 300;
            gb.fbiStartingRisk        = 0f;

            // Territory
            gb.totalTerritories       = 10;
            gb.winTerritories         = 6;
            gb.loseTerritories        = 7;

            // Market Pool
            gb.baseMarketCustomers    = 60;
            gb.earlyGrowthPerTurn     = 5;
            gb.midGrowthPerTurn       = 6;
            gb.lateGrowthPerTurn      = 8;
            gb.endGrowthPerTurn       = 10;

            // Score
            gb.territoryScoreMultiplier   = 500;
            gb.moneyScoreMultiplier       = 1;
            gb.comboScoreMultiplier       = 200;
            gb.businessScoreMultiplier    = 100;
            gb.earlyFinishBonusPerTurn    = 300;
            gb.fbiEvasionBonus            = 50;
            gb.winBonus                   = 1000;

            // Business Evolution
            gb.evolutionCustomerThreshold = 40;
            gb.evolutionTurnRequirement   = 15;

            // Employee
            gb.employeeLeaveTurnThreshold = 8;

            MarkDirty(gb);
            Debug.Log("[AssetGenerator] GameBalance.asset created.");
        }

        // ===================================================================
        // 2. ALL 40 CARDS
        // ===================================================================
        private static CardData[] GenerateAllCards()
        {
            CardData[] all = new CardData[40];
            int idx = 0;

            // --- 8 Business Cards ---
            CardData[] businesses = GenerateBusinessCards();
            for (int i = 0; i < businesses.Length; i++) all[idx++] = businesses[i];

            // --- 10 Employee Cards ---
            CardData[] employees = GenerateEmployeeCards();
            for (int i = 0; i < employees.Length; i++) all[idx++] = employees[i];

            // --- 10 Action Cards ---
            CardData[] actions = GenerateActionCards();
            for (int i = 0; i < actions.Length; i++) all[idx++] = actions[i];

            // --- 6 Upgrade Cards ---
            CardData[] upgrades = GenerateUpgradeCards();
            for (int i = 0; i < upgrades.Length; i++) all[idx++] = upgrades[i];

            // --- 6 Event Cards ---
            CardData[] events = GenerateEventCards();
            for (int i = 0; i < events.Length; i++) all[idx++] = events[i];

            Debug.Log($"[AssetGenerator] {idx} card assets created.");
            return all;
        }

        // -------------------------------------------------------------------
        // 2a. BUSINESS CARDS
        // -------------------------------------------------------------------
        private static CardData[] GenerateBusinessCards()
        {
            CardData[] cards = new CardData[8];

            // B01 - Diner
            cards[0] = CreateBusiness("B01_Bufe", "Diner", Rarity.Common,
                "Humble beginnings. Everyone's first business.", 0, 50, 3, 1,
                new[] { CardTag.Food, CardTag.Basic });
            cards[0].canEvolve = true;

            // B02 - Coffee Shop
            cards[1] = CreateBusiness("B02_Kahveci", "Coffee Shop", Rarity.Common,
                "Trend-sensitive. Very profitable at the right time.", 150, 80, 5, 2,
                new[] { CardTag.Food, CardTag.Coffee, CardTag.Trendy });
            cards[1].hasTrendBonus = true;
            cards[1].trendIncomeMultiplier = 1.5f;

            // B03 - Burger Chain
            cards[2] = CreateBusiness("B03_BurgerZinciri", "Burger Chain", Rarity.Uncommon,
                "Many employees = many synergies. But salaries add up.", 250, 100, 6, 3,
                new[] { CardTag.Food, CardTag.Chain });

            // B04 - Tech Startup
            cards[3] = CreateBusiness("B04_TechStartup", "Tech Startup", Rarity.Uncommon,
                "Patience required. But when it hits, it hits big.", 200, 150, 4, 2,
                new[] { CardTag.Tech, CardTag.Startup });
            cards[3].activationDelay = 3;

            // B05 - Nightclub
            cards[4] = CreateBusiness("B05_GeceKulubu", "Nightclub", Rarity.Rare,
                "High reward, high risk. Dead when trends die.", 350, 180, 10, 2,
                new[] { CardTag.Entertainment, CardTag.Nightlife, CardTag.Trendy });
            cards[4].requiresTrendToOperate = true;

            // B06 - Organic Farm
            cards[5] = CreateBusiness("B06_OrganikCiftlik", "Organic Farm", Rarity.Common,
                "Weak alone. But powers up all food businesses.", 120, 40, 2, 1,
                new[] { CardTag.Food, CardTag.Organic, CardTag.Support });
            cards[5].foodBonusTag = "Food";
            cards[5].foodBonusAmount = 20;

            // B07 - Crypto Exchange
            cards[6] = CreateBusiness("B07_KriptoBorsasi", "Crypto Exchange", Rarity.Rare,
                "Gambling. Sometimes zero, sometimes jackpot.", 300, 0, 2, 1,
                new[] { CardTag.Tech, CardTag.Crypto, CardTag.Risky });
            cards[6].hasRandomIncome = true;
            cards[6].randomIncomeMin = 0;
            cards[6].randomIncomeMax = 250;

            // B08 - Ad Agency
            cards[7] = CreateBusiness("B08_ReklamAjansi", "Ad Agency", Rarity.Uncommon,
                "Low income but boosts all your businesses.", 200, 60, 3, 2,
                new[] { CardTag.Marketing, CardTag.Support });
            cards[7].globalCustomerBonus = 2;

            return cards;
        }

        private static CardData CreateBusiness(string id, string name, Rarity rarity,
            string desc, int cost, int income, int customers, int slots, CardTag[] tags)
        {
            string path = BusinessCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);
            c.cardId              = id;
            c.cardName            = name;
            c.cardType            = CardType.Business;
            c.rarity              = rarity;
            c.description         = desc;
            c.buyCost             = cost;
            c.incomePerTurn       = income;
            c.customersPerTurn    = customers;
            c.employeeSlots       = slots;
            c.tags                = tags;
            // Reset defaults that may linger from previous generation
            c.hasTrendBonus              = false;
            c.trendIncomeMultiplier      = 1f;
            c.activationDelay            = 0;
            c.requiresTrendToOperate     = false;
            c.hasRandomIncome            = false;
            c.randomIncomeMin            = 0;
            c.randomIncomeMax            = 0;
            c.foodBonusTag               = "";
            c.foodBonusAmount            = 0;
            c.globalCustomerBonus        = 0;
            c.canEvolve                  = false;
            MarkDirty(c);
            return c;
        }

        // -------------------------------------------------------------------
        // 2b. EMPLOYEE CARDS
        // -------------------------------------------------------------------
        private static CardData[] GenerateEmployeeCards()
        {
            CardData[] cards = new CardData[10];

            // C01 - Intern
            cards[0] = CreateEmployee("C01_Stajyer", "Intern", Rarity.Common,
                "Cheap but weak. Starter card.", 15, tags: new[] { CardTag.Basic });
            cards[0].customerBonus       = 1;
            cards[0].activeAbilityType   = ActiveAbilityType.AddCustomersThisTurn;
            cards[0].activeAbilityName   = "Hustle";
            cards[0].activeAbilityDesc   = "Adds +3 customers to this business this turn.";
            cards[0].abilityValue2       = 3;

            // C02 - Junior Marketer
            cards[1] = CreateEmployee("C02_CaylakPazarlamaci", "Junior Marketer", Rarity.Common,
                "Small but consistent bonus.", 20, tags: new[] { CardTag.Marketing, CardTag.Basic });
            cards[1].incomeMultiplier    = 0.10f;
            cards[1].activeAbilityType   = ActiveAbilityType.AddCustomersThisTurn;
            cards[1].activeAbilityName   = "Campaign";
            cards[1].activeAbilityDesc   = "Adds +5 customers to this business this turn.";
            cards[1].abilityValue2       = 5;

            // C03 - Barista
            cards[2] = CreateEmployee("C03_Barista", "Barista", Rarity.Uncommon,
                "Doubles in coffee shops.", 25,
                tags: new[] { CardTag.Food, CardTag.Coffee });
            cards[2].customerBonus         = 3;
            cards[2].synergyCustomerBonus  = 6;
            cards[2].synergyTag            = CardTag.Coffee;
            cards[2].activeAbilityType     = ActiveAbilityType.MultiplyCustomersThisTurn;
            cards[2].activeAbilityName     = "Latte Festival";
            cards[2].activeAbilityDesc     = "Customers x2 this turn.";
            cards[2].abilityValue1         = 2f;

            // C04 - Chef
            cards[3] = CreateEmployee("C04_Sef", "Chef", Rarity.Uncommon,
                "Strong in food sector.", 30,
                tags: new[] { CardTag.Food });
            cards[3].customerBonus       = 3;
            cards[3].incomeFlatBonus     = 30f;
            cards[3].incomeBonusTag      = CardTag.Food;
            cards[3].activeAbilityType   = ActiveAbilityType.MultiplyIncomeThisTurn;
            cards[3].activeAbilityName   = "Special Menu";
            cards[3].activeAbilityDesc   = "Income x1.5 this turn.";
            cards[3].abilityValue1       = 1.5f;

            // C05 - Marketing Guru
            cards[4] = CreateEmployee("C05_MarketingGurusu", "Marketing Guru", Rarity.Rare,
                "Expensive but powerful. Combo piece.", 45,
                tags: new[] { CardTag.Marketing, CardTag.Guru });
            cards[4].incomeMultiplier    = 0.25f;
            cards[4].activeAbilityType   = ActiveAbilityType.AddCustomersToAll;
            cards[4].activeAbilityName   = "Viral Campaign";
            cards[4].activeAbilityDesc   = "+3 customers to all businesses.";
            cards[4].abilityValue2       = 3;

            // C06 - Influencer
            cards[5] = CreateEmployee("C06_Influencer", "Influencer", Rarity.Rare,
                "Explodes during trends. Average otherwise.", 50,
                tags: new[] { CardTag.Marketing, CardTag.Influencer, CardTag.Trendy });
            cards[5].customerBonus         = 5;
            cards[5].synergyCustomerBonus  = 12;
            cards[5].synergyTag            = CardTag.Trendy;
            cards[5].activeAbilityType     = ActiveAbilityType.StealCustomersFromRival;
            cards[5].activeAbilityName     = "Post Story";
            cards[5].activeAbilityDesc     = "Steal 5 customers from rival.";
            cards[5].abilityValue2         = 5;

            // C07 - Hacker
            cards[6] = CreateEmployee("C07_Hacker", "Hacker", Rarity.Rare,
                "Powerful but dangerous. FBI risk every turn.", 60,
                tags: new[] { CardTag.Tech, CardTag.Illegal });
            cards[6].customerBonus       = -4;
            cards[6].fbiRiskPerTurn      = 10;
            cards[6].activeAbilityType   = ActiveAbilityType.None;
            cards[6].activeAbilityName   = "Passive Infiltration";
            cards[6].activeAbilityDesc   = "Steals 4 customers from rival each turn (passive).";

            // C08 - Accountant
            cards[7] = CreateEmployee("C08_Muhasebeci", "Accountant", Rarity.Uncommon,
                "Boring but saves every penny.", 30,
                tags: new[] { CardTag.Finance });
            cards[7].taxReduction        = 0.5f;
            cards[7].activeAbilityType   = ActiveAbilityType.NullifyTaxThisTurn;
            cards[7].activeAbilityName   = "Tax Plan";
            cards[7].activeAbilityDesc   = "Tax is nullified this turn.";

            // C09 - Fraudster
            cards[8] = CreateEmployee("C09_Dolandirici", "Fraudster", Rarity.Rare,
                "Fast money. But FBI is knocking.", 40,
                tags: new[] { CardTag.Illegal, CardTag.Finance });
            cards[8].illegalIncomePerTurn = 120;
            cards[8].fbiRiskPerTurn       = 12;
            cards[8].activeAbilityType    = ActiveAbilityType.BonusIncomeWithPenalty;
            cards[8].activeAbilityName    = "Ponzi";
            cards[8].activeAbilityDesc    = "+300 instant but -150 next turn.";
            cards[8].abilityValue2        = 300;

            // C10 - Loyal Manager
            cards[9] = CreateEmployee("C10_SadikMudur", "Loyal Manager", Rarity.Uncommon,
                "Rival can't steal employees. Defensive.", 45,
                tags: new[] { CardTag.Management });
            cards[9].customerBonus       = 0;
            cards[9].incomeFlatBonus     = 20f;
            cards[9].preventsTransfer    = true;
            cards[9].activeAbilityType   = ActiveAbilityType.MotivateAllEmployees;
            cards[9].activeAbilityName   = "Motivation";
            cards[9].activeAbilityDesc   = "All employees gain +1 customer this turn.";

            return cards;
        }

        private static CardData CreateEmployee(string id, string name, Rarity rarity,
            string desc, int salary, CardTag[] tags)
        {
            string path = EmployeeCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);
            c.cardId              = id;
            c.cardName            = name;
            c.cardType            = CardType.Employee;
            c.rarity              = rarity;
            c.description         = desc;
            c.buyCost             = 0;
            c.salaryPerTurn       = salary;
            c.tags                = tags;
            // Reset employee defaults
            c.customerBonus          = 0;
            c.synergyCustomerBonus   = 0;
            c.synergyTag             = CardTag.Food;
            c.incomeMultiplier       = 0f;
            c.incomeFlatBonus        = 0f;
            c.incomeBonusTag         = CardTag.Food;
            c.fbiRiskPerTurn         = 0;
            c.illegalIncomePerTurn   = 0;
            c.preventsTransfer       = false;
            c.taxReduction           = 0f;
            c.activeAbilityType      = ActiveAbilityType.None;
            c.activeAbilityName      = "";
            c.activeAbilityDesc      = "";
            c.abilityValue1          = 0f;
            c.abilityValue2          = 0;
            MarkDirty(c);
            return c;
        }

        // -------------------------------------------------------------------
        // 2c. ACTION CARDS
        // -------------------------------------------------------------------
        private static CardData[] GenerateActionCards()
        {
            CardData[] cards = new CardData[10];

            // A01 - Flyer
            cards[0] = CreateAction("A01_ElIlani", "Flyer", Rarity.Common,
                "Free but weak. Starter card.", 0,
                ActionEffectType.AddCustomersToRandom, 3, 0f, 0,
                new[] { CardTag.Marketing, CardTag.Basic });

            // A02 - Small Investment
            cards[1] = CreateAction("A02_KucukYatirim", "Small Investment", Rarity.Common,
                "Quick cash. Starter card.", 0,
                ActionEffectType.AddMoneyInstant, 150, 0f, 0,
                new[] { CardTag.Finance, CardTag.Basic });

            // A03 - Viral Marketing
            cards[2] = CreateAction("A03_ViralPazarlama", "Viral Marketing", Rarity.Uncommon,
                "Explodes when played at the right turn.", 150,
                ActionEffectType.MultiplyAllCustomers, 0, 2f, 0,
                new[] { CardTag.Marketing, CardTag.Viral });

            // A04 - Hostile Takeover
            cards[3] = CreateAction("A04_DusmancaDevralma", "Hostile Takeover", Rarity.Rare,
                "Expensive but directly weakens rival.", 400,
                ActionEffectType.CloseRivalWeakestBusiness, 0, 0f, 0,
                new[] { CardTag.Aggressive });

            // A05 - Fake Reviews
            cards[4] = CreateAction("A05_SahteYorumlar", "Fake Reviews", Rarity.Uncommon,
                "Cheap customers. But risky.", 80,
                ActionEffectType.AddCustomersWithFBI, 8, 0f, 12,
                new[] { CardTag.Marketing, CardTag.Illegal });

            // A06 - Price Slashing
            cards[5] = CreateAction("A06_FiyatKirma", "Price Slashing", Rarity.Uncommon,
                "Sacrifice income, steal customers.", 0,
                ActionEffectType.StealCustomersHalfIncome, 8, 0f, 0,
                new[] { CardTag.Aggressive, CardTag.Pricing });
            cards[5].actionIncomeSacrifice = 0.5f;

            // A07 - Sabotage
            cards[6] = CreateAction("A07_Sabotaj", "Sabotage", Rarity.Rare,
                "Powerful but very risky.", 250,
                ActionEffectType.DisableRivalOneTurn, 0, 0f, 15,
                new[] { CardTag.Aggressive, CardTag.Illegal });

            // A08 - Investor Pitch
            cards[7] = CreateAction("A08_YatirimciSunumu", "Investor Pitch", Rarity.Uncommon,
                "Big money now. Pay later.", 0,
                ActionEffectType.MoneyNowPayLater, 600, 0f, 0,
                new[] { CardTag.Finance, CardTag.Investor });
            cards[7].actionDebtDuration = 3;
            cards[7].actionDebtPercent  = 0.15f;

            // A09 - Emergency Hire
            cards[8] = CreateAction("A09_AcilIseAlim", "Emergency Hire", Rarity.Uncommon,
                "Quick employee. But random.", 100,
                ActionEffectType.DrawAndPlayEmployee, 0, 0f, 0,
                new[] { CardTag.Hiring });

            // A10 - Liquidation Sale
            cards[9] = CreateAction("A10_TasfiyeSatisi", "Liquidation Sale", Rarity.Common,
                "Last resort. Or strategic move.", 0,
                ActionEffectType.SacrificeBusiness, 0, 0f, 0,
                new[] { CardTag.Finance, CardTag.Desperate });

            return cards;
        }

        private static CardData CreateAction(string id, string name, Rarity rarity,
            string desc, int cost, ActionEffectType effect, int value, float multiplier,
            int fbiRisk, CardTag[] tags)
        {
            string path = ActionCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);
            c.cardId              = id;
            c.cardName            = name;
            c.cardType            = CardType.Action;
            c.rarity              = rarity;
            c.description         = desc;
            c.buyCost             = cost;
            c.tags                = tags;
            c.actionEffectType    = effect;
            c.actionValue         = value;
            c.actionMultiplier    = multiplier;
            c.actionFBIRisk       = fbiRisk;
            // Reset action-specific defaults
            c.actionDebtDuration    = 0;
            c.actionDebtPercent     = 0f;
            c.actionIncomeSacrifice = 0f;
            MarkDirty(c);
            return c;
        }

        // -------------------------------------------------------------------
        // 2d. UPGRADE CARDS
        // -------------------------------------------------------------------
        private static CardData[] GenerateUpgradeCards()
        {
            CardData[] cards = new CardData[6];

            // U01 - Office Supplies
            cards[0] = CreateUpgrade("U01_OfisMalzemeleri", "Office Supplies", Rarity.Common,
                "Small but free. Starter card.", 0,
                UpgradeEffectType.IncomePercentSingle, 10f, false, 0, 0,
                new[] { CardTag.Basic, CardTag.Office });

            // U02 - Automation
            cards[1] = CreateUpgrade("U02_Otomasyon", "Automation", Rarity.Uncommon,
                "Strong income boost. But loses an employee slot.", 300,
                UpgradeEffectType.IncomePercentWithSlotLoss, 30f, false, 1, 0,
                new[] { CardTag.Tech, CardTag.Automation });

            // U03 - Delivery Network
            cards[2] = CreateUpgrade("U03_TeslimatAgi", "Delivery Network", Rarity.Uncommon,
                "Very valuable with multiple businesses.", 250,
                UpgradeEffectType.GlobalCustomerPerTurn, 2f, true, 0, 0,
                new[] { CardTag.Logistics });

            // U04 - Billboard
            cards[3] = CreateUpgrade("U04_ReklamPanosu", "Billboard", Rarity.Common,
                "Cheap, simple, effective.", 120,
                UpgradeEffectType.GlobalCustomerFlat, 3f, true, 0, 0,
                new[] { CardTag.Marketing });

            // U05 - Security System
            cards[4] = CreateUpgrade("U05_GuvenlikSistemi", "Security System", Rarity.Uncommon,
                "Essential for illegal strategies.", 280,
                UpgradeEffectType.ReduceFBIRisk, 25f, true, 0, 0,
                new[] { CardTag.Security });

            // U06 - AI Assistant
            cards[5] = CreateUpgrade("U06_YapayZekaAsistani", "AI Assistant", Rarity.Rare,
                "The game's strongest upgrade. Extra action per turn.", 400,
                UpgradeEffectType.ExtraAction, 0f, true, 0, 1,
                new[] { CardTag.Tech, CardTag.AI });

            return cards;
        }

        private static CardData CreateUpgrade(string id, string name, Rarity rarity,
            string desc, int cost, UpgradeEffectType effect, float value, bool isGlobal,
            int closedSlots, int extraActs, CardTag[] tags)
        {
            string path = UpgradeCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);
            c.cardId                = id;
            c.cardName              = name;
            c.cardType              = CardType.Upgrade;
            c.rarity                = rarity;
            c.description           = desc;
            c.buyCost               = cost;
            c.tags                  = tags;
            c.upgradeEffectType     = effect;
            c.upgradeValue          = value;
            c.isGlobalUpgrade       = isGlobal;
            c.closedEmployeeSlots   = closedSlots;
            c.extraActions          = extraActs;
            MarkDirty(c);
            return c;
        }

        // -------------------------------------------------------------------
        // 2e. EVENT CARDS
        // -------------------------------------------------------------------
        private static CardData[] GenerateEventCards()
        {
            CardData[] cards = new CardData[6];

            // E01 - Coffee Craze
            cards[0] = CreateEvent("E01_KahveCilginligi", "Coffee Craze", Rarity.Common,
                "Food sector booming.", 2,
                EventEffectType.TagCustomerBoost, 0.5f,
                new[] { CardTag.Food, CardTag.Coffee }, 0, 0f);

            // E02 - Economic Crisis
            cards[1] = CreateEvent("E02_EkonomikKriz", "Economic Crisis", Rarity.Common,
                "Everyone suffers. But the prepared find opportunity.", 2,
                EventEffectType.AllIncomeReduction, -0.3f,
                new CardTag[0], 0, 0f);

            // E03 - Viral Trend
            cards[2] = CreateEvent("E03_ViralTrend", "Viral Trend", Rarity.Uncommon,
                "Marketing-heavy strategy shines here.", 1,
                EventEffectType.TagDoubleEffect, 1.0f,
                new[] { CardTag.Marketing }, 0, 0f);

            // E04 - Data Breach
            cards[3] = CreateEvent("E04_VeriSizintisi", "Data Breach", Rarity.Uncommon,
                "Tech-focused beware. Security investment matters.", 1,
                EventEffectType.TagCustomerPenalty, 0f,
                new[] { CardTag.Tech }, -5, 0f);

            // E05 - Investor Season
            cards[4] = CreateEvent("E05_YatirimciSezonu", "Investor Season", Rarity.Uncommon,
                "Play investment cards this turn = jackpot.", 1,
                EventEffectType.TagDoubleEffectFinance, 1.0f,
                new[] { CardTag.Finance }, 0, 0f);

            // E06 - Cancel Culture
            cards[5] = CreateEvent("E06_IptalKulturu", "Cancel Culture", Rarity.Rare,
                "Disaster for dirty players. Opportunity for clean ones.", 1,
                EventEffectType.HighFBICustomerPenalty, -0.4f,
                new CardTag[0], 0, 0.3f);

            return cards;
        }

        private static CardData CreateEvent(string id, string name, Rarity rarity,
            string desc, int duration, EventEffectType effect, float multiplier,
            CardTag[] affected, int customerPenalty, float fbiThreshold)
        {
            string path = EventCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);
            c.cardId                = id;
            c.cardName              = name;
            c.cardType              = CardType.Event;
            c.rarity                = rarity;
            c.description           = desc;
            c.buyCost               = 0;
            c.tags                  = new CardTag[0];
            c.eventEffectType       = effect;
            c.eventDuration         = duration;
            c.eventMultiplier       = multiplier;
            c.affectedTags          = affected;
            c.eventCustomerPenalty  = customerPenalty;
            c.eventFBIThreshold     = fbiThreshold;
            MarkDirty(c);
            return c;
        }

        // ===================================================================
        // 3. STARTING DECK PRESET
        // ===================================================================
        private static void GenerateStartingDeck(CardData[] allCards)
        {
            string path = DecksRoot + "/StarterDeck.asset";
            var deck = CreateOrReplace<DeckPresetData>(path);

            deck.presetName    = "Starter Deck";
            deck.startingMoney = 500;

            // Find the cards we need by ID
            CardData b01 = FindCard(allCards, "B01_Bufe");
            CardData c01 = FindCard(allCards, "C01_Stajyer");
            CardData c02 = FindCard(allCards, "C02_CaylakPazarlamaci");
            CardData a01 = FindCard(allCards, "A01_ElIlani");
            CardData a02 = FindCard(allCards, "A02_KucukYatirim");
            CardData u01 = FindCard(allCards, "U01_OfisMalzemeleri");

            // 14 cards: 2xB01, 3xC01, 2xC02, 3xA01, 2xA02, 2xU01
            deck.cards = new DeckEntry[]
            {
                new DeckEntry { card = b01, count = 2 },
                new DeckEntry { card = c01, count = 3 },
                new DeckEntry { card = c02, count = 2 },
                new DeckEntry { card = a01, count = 3 },
                new DeckEntry { card = a02, count = 2 },
                new DeckEntry { card = u01, count = 2 },
            };

            MarkDirty(deck);
            Debug.Log("[AssetGenerator] StarterDeck.asset created (14 cards).");
        }

        private static CardData FindCard(CardData[] cards, string id)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null && cards[i].cardId == id) return cards[i];
            }
            Debug.LogError($"[AssetGenerator] Card not found: {id}");
            return null;
        }

        // ===================================================================
        // 4. MEGACORP RIVAL
        // ===================================================================
        private static void GenerateMegaCorpRival()
        {
            string path = RivalsRoot + "/MegaCorp.asset";
            var rival = CreateOrReplace<RivalData>(path);

            rival.rivalId                  = "RIVAL_MegaCorp";
            rival.rivalName                = "MegaCorp";
            rival.personality              = RivalPersonality.Balanced;
            rival.tagline                  = "This industry isn't big enough for both of us.";

            // Starting Stats
            rival.startingMoney            = 400;
            rival.startingIncome           = 80;
            rival.startingCustomers        = 5;
            rival.startingBusinessName     = "MegaCorp HQ";

            // Behavior
            rival.actionsPerTurn           = 2;
            rival.aggressionThreshold      = 0.5f;
            rival.maxBusinesses            = 4;
            rival.maxEmployeesPerBusiness  = 3;

            // Growth Parameters
            rival.businessCostThreshold    = 200;
            rival.hireCostThreshold        = 80;
            rival.baseBusinessIncome       = 80;
            rival.baseBusinessCustomers    = 5;
            rival.employeeIncomeBoost      = 30;
            rival.employeeCustomerBoost    = 3;
            rival.aggressiveCustomerBoost  = 8;
            rival.aggressiveIncomeBoost    = 50;
            rival.passiveCustomerGrowth    = 2;
            rival.passiveIncomeGrowth      = 10;

            rival.possibleBusinessNames = new[]
            {
                "Tech Store",
                "Supermarket",
                "Cafe Chain",
                "Fitness Center"
            };

            // Growth Schedule (GDD Section 8.3)
            rival.growthMilestones = new RivalMilestone[]
            {
                new RivalMilestone
                {
                    turn = 5,
                    targetBusinesses = 2,
                    targetEmployees = 2,
                    targetTerritories = 3,
                    enableAggression = false
                },
                new RivalMilestone
                {
                    turn = 8,
                    targetBusinesses = 3,
                    targetEmployees = 4,
                    targetTerritories = 4,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 12,
                    targetBusinesses = 3,
                    targetEmployees = 6,
                    targetTerritories = 5,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 15,
                    targetBusinesses = 4,
                    targetEmployees = 8,
                    targetTerritories = 6,
                    enableAggression = true
                },
            };

            // Dialogue
            rival.growingTaunts = new[]
            {
                "Our market share is growing.",
                "This is just the beginning."
            };
            rival.playerGrowingTaunts = new[]
            {
                "Interesting move...",
                "Didn't see that coming."
            };
            rival.aggressiveTaunts = new[]
            {
                "This industry isn't big enough for both of us.",
                "Competition is heating up."
            };
            rival.losingTaunts = new[]
            {
                "This isn't over.",
                "We'll be back."
            };
            rival.winningTaunts = new[]
            {
                "It was inevitable.",
                "The market is ours."
            };

            MarkDirty(rival);
            Debug.Log("[AssetGenerator] MegaCorp.asset created.");
        }

        // ===================================================================
        // 5. COMBOS (10 combos from GDD Combo Matrix)
        // ===================================================================
        private static void GenerateCombos(CardData[] allCards)
        {
            // COMBO 01 - Latte Art (Easy)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_01_LatteArt.asset");
                combo.comboId       = "COMBO_01_LatteArt";
                combo.comboName     = "Latte Art";
                combo.displayText   = "LATTE ART!";
                combo.tier          = ComboTier.Easy;
                combo.description   = "Coffee Shop + Barista = customers x2, income +50%.";
                combo.requiredCardIds = new[] { "B02_Kahveci", "C03_Barista" };
                combo.requiredTags    = new[] { CardTag.Coffee };
                combo.requiresSpecificPlacement = true;
                combo.employeeCardId  = "C03_Barista";
                combo.businessCardId  = "B02_Kahveci";
                combo.customerMultiplier = 2f;
                combo.incomeMultiplier   = 1.5f;
                combo.glowColor          = new Color(0.6f, 0.4f, 0.2f, 1f); // coffee color
                combo.comboSoundId       = "combo_trigger";
                combo.screenShakeIntensity = 0.3f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 02 - Organic Synergy (Easy)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_02_OrganicSynergy.asset");
                combo.comboId       = "COMBO_02_OrganicSynergy";
                combo.comboName     = "Organic Synergy";
                combo.displayText   = "ORGANIC SYNERGY!";
                combo.tier          = ComboTier.Easy;
                combo.description   = "Burger Chain + Chef = income +30, customers +50%.";
                combo.requiredCardIds = new[] { "B03_BurgerZinciri", "C04_Sef" };
                combo.requiredTags    = new[] { CardTag.Food };
                combo.requiresSpecificPlacement = true;
                combo.employeeCardId  = "C04_Sef";
                combo.businessCardId  = "B03_BurgerZinciri";
                combo.bonusIncome     = 30;
                combo.customerMultiplier = 1.5f;
                combo.glowColor          = new Color(1f, 0.5f, 0f, 1f); // orange
                combo.comboSoundId       = "combo_trigger";
                combo.screenShakeIntensity = 0.3f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 03 - Viral Storm (Easy)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_03_ViralStorm.asset");
                combo.comboId       = "COMBO_03_ViralStorm";
                combo.comboName     = "Viral Storm";
                combo.displayText   = "VIRAL STORM!";
                combo.tier          = ComboTier.Easy;
                combo.description   = "Tech Startup + Marketing Guru = income x2.";
                combo.requiredCardIds = new[] { "B04_TechStartup", "C05_MarketingGurusu" };
                combo.requiredTags    = new[] { CardTag.Tech, CardTag.Marketing };
                combo.requiresSpecificPlacement = true;
                combo.employeeCardId  = "C05_MarketingGurusu";
                combo.businessCardId  = "B04_TechStartup";
                combo.incomeMultiplier = 2f;
                combo.glowColor        = new Color(0f, 0.8f, 1f, 1f); // blue
                combo.comboSoundId     = "combo_trigger";
                combo.screenShakeIntensity = 0.3f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 04 - Fast Food Empire (Medium)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_04_FastFoodEmpire.asset");
                combo.comboId       = "COMBO_04_FastFoodEmpire";
                combo.comboName     = "Fast Food Empire";
                combo.displayText   = "FAST FOOD EMPIRE!";
                combo.tier          = ComboTier.Medium;
                combo.description   = "Nightclub + Influencer + Viral Trend event = customers x3.";
                combo.requiredCardIds = new[] { "B05_GeceKulubu", "C06_Influencer" };
                combo.requiredTags    = new[] { CardTag.Trendy, CardTag.Entertainment };
                combo.requiresSpecificPlacement = true;
                combo.employeeCardId  = "C06_Influencer";
                combo.businessCardId  = "B05_GeceKulubu";
                combo.requiresActiveEvent = true;
                combo.requiredEventId     = "E03_ViralTrend";
                combo.customerMultiplier  = 3f;
                combo.glowColor           = new Color(0.8f, 0f, 1f, 1f); // purple
                combo.comboSoundId        = "combo_trigger";
                combo.screenShakeIntensity = 0.5f;
                combo.screenShakeDuration  = 0.4f;
                MarkDirty(combo);
            }

            // COMBO 05 - Underground Empire (Medium)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_05_UndergroundEmpire.asset");
                combo.comboId       = "COMBO_05_UndergroundEmpire";
                combo.comboName     = "Underground Empire";
                combo.displayText   = "UNDERGROUND EMPIRE!";
                combo.tier          = ComboTier.Medium;
                combo.description   = "Hacker + Fraudster = +200 income/turn but FBI +8% extra.";
                combo.requiredCardIds = new[] { "C07_Hacker", "C09_Dolandirici" };
                combo.requiredTags    = new[] { CardTag.Illegal };
                combo.requiresSpecificPlacement = false;
                combo.bonusIncome     = 200;
                combo.extraFBIRisk    = 8;
                combo.glowColor       = new Color(0.2f, 0.2f, 0.2f, 1f); // dark
                combo.comboSoundId    = "combo_trigger";
                combo.screenShakeIntensity = 0.4f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 06 - Safe Crime (Medium)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_06_SafeCrime.asset");
                combo.comboId       = "COMBO_06_SafeCrime";
                combo.comboName     = "Safe Crime";
                combo.displayText   = "SAFE CRIME!";
                combo.tier          = ComboTier.Medium;
                combo.description   = "Accountant + Fraudster = illegal income tax-free.";
                combo.requiredCardIds = new[] { "C08_Muhasebeci", "C09_Dolandirici" };
                combo.requiredTags    = new[] { CardTag.Finance };
                combo.requiresSpecificPlacement = false;
                combo.bonusIncome       = 0;
                combo.incomeMultiplier  = 1f;
                combo.glowColor         = new Color(0f, 0.8f, 0f, 1f); // green
                combo.comboSoundId      = "combo_trigger";
                combo.screenShakeIntensity = 0.3f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 07 - AI Revolution (Hard)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_07_AIRevolution.asset");
                combo.comboId       = "COMBO_07_AIRevolution";
                combo.comboName     = "AI Revolution";
                combo.displayText   = "AI REVOLUTION!";
                combo.tier          = ComboTier.Hard;
                combo.description   = "Tech Startup + Automation + AI Assistant = +1 action, income x2.";
                combo.requiredCardIds = new[] { "B04_TechStartup", "U02_Otomasyon", "U06_YapayZekaAsistani" };
                combo.requiredTags    = new[] { CardTag.Tech, CardTag.AI };
                combo.requiresSpecificPlacement = false;
                combo.extraActions     = 1;
                combo.incomeMultiplier = 2f;
                combo.glowColor        = new Color(0f, 1f, 1f, 1f); // cyan
                combo.comboSoundId     = "combo_trigger";
                combo.screenShakeIntensity = 0.6f;
                combo.screenShakeDuration  = 0.5f;
                MarkDirty(combo);
            }

            // COMBO 08 - Ad Blitz (Medium)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_08_AdBlitz.asset");
                combo.comboId       = "COMBO_08_AdBlitz";
                combo.comboName     = "Ad Blitz";
                combo.displayText   = "AD BLITZ!";
                combo.tier          = ComboTier.Medium;
                combo.description   = "Organic Farm + Burger Chain + Chef = all Food businesses +50 income.";
                combo.requiredCardIds = new[] { "B06_OrganikCiftlik", "B03_BurgerZinciri", "C04_Sef" };
                combo.requiredTags    = new[] { CardTag.Food, CardTag.Organic };
                combo.requiresSpecificPlacement = false;
                combo.bonusIncome     = 50;
                combo.glowColor       = new Color(0.2f, 0.8f, 0.2f, 1f); // green
                combo.comboSoundId    = "combo_trigger";
                combo.screenShakeIntensity = 0.4f;
                combo.screenShakeDuration  = 0.4f;
                MarkDirty(combo);
            }

            // COMBO 09 - Crisis Hunter (Hard)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_09_CrisisHunter.asset");
                combo.comboId       = "COMBO_09_CrisisHunter";
                combo.comboName     = "Crisis Hunter";
                combo.displayText   = "CRISIS HUNTER!";
                combo.tier          = ComboTier.Hard;
                combo.description   = "During Economic Crisis with 1000+ money: shop 50% off, steal 1 rival employee.";
                combo.requiredCardIds = new string[0];
                combo.requiredTags    = new[] { CardTag.Finance };
                combo.requiresActiveEvent  = true;
                combo.requiredEventId      = "E02_EkonomikKriz";
                combo.requiresMinMoney     = true;
                combo.minMoneyRequired     = 1000;
                combo.shopDiscount         = 0.5f;
                combo.transferRivalEmployee = true;
                combo.glowColor            = new Color(1f, 0.84f, 0f, 1f); // gold
                combo.comboSoundId         = "combo_trigger";
                combo.screenShakeIntensity = 0.5f;
                combo.screenShakeDuration  = 0.4f;
                MarkDirty(combo);
            }

            // COMBO 10 - Monopoly (Automatic)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_10_Monopoly.asset");
                combo.comboId       = "COMBO_10_Monopoly";
                combo.comboName     = "Monopoly";
                combo.displayText   = "MONOPOLY!";
                combo.tier          = ComboTier.Automatic;
                combo.description   = "4+ businesses, 55%+ market share = rival -3 customers/turn, income +20%.";
                combo.requiredCardIds  = new string[0];
                combo.requiredTags     = new CardTag[0];
                combo.requiresSpecificPlacement = false;
                combo.minActiveBusinesses = 4;
                combo.minMarketShare      = 0.55f;
                combo.rivalCustomerPenalty = 3;
                combo.incomeMultiplier     = 1.2f;
                combo.glowColor            = new Color(1f, 0f, 0f, 1f); // red
                combo.comboSoundId         = "combo_trigger";
                combo.screenShakeIntensity = 0.7f;
                combo.screenShakeDuration  = 0.6f;
                MarkDirty(combo);
            }

            Debug.Log("[AssetGenerator] 10 combo assets created.");
        }

        // ===================================================================
        // 6. META PROGRESSION
        // ===================================================================
        private static void GenerateMetaProgression()
        {
            string path = DataRoot + "/MetaProgression.asset";
            var meta = CreateOrReplace<MetaProgressionData>(path);

            // Unlock Tiers (GDD Section 9)
            meta.unlockTiers = new UnlockTier[]
            {
                new UnlockTier
                {
                    xpRequired = 50,
                    unlockDescription = "Shop pool unlocked. Common cards available.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
                new UnlockTier
                {
                    xpRequired = 200,
                    unlockDescription = "Uncommon cards enter the shop pool.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
                new UnlockTier
                {
                    xpRequired = 500,
                    unlockDescription = "Rare cards unlocked. Shadow Inc. rival available.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null // Shadow Inc. post-MVP
                },
                new UnlockTier
                {
                    xpRequired = 1000,
                    unlockDescription = "All cards unlocked. Ascension 1 mode active.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
                new UnlockTier
                {
                    xpRequired = 2000,
                    unlockDescription = "The Cartel rival unlocked. Ascension 2 mode active.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null // The Cartel post-MVP
                },
                new UnlockTier
                {
                    xpRequired = 5000,
                    unlockDescription = "Ascension 3 mode. All content unlocked.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
            };

            // Ascension Levels
            meta.ascensionLevels = new AscensionLevel[]
            {
                new AscensionLevel
                {
                    level = 1,
                    description = "Rival more aggressive, events more frequent.",
                    rivalAggressionMultiplier = 1.2f,
                    startingMoneyModifier = 0,
                    crisisFrequencyMultiplier = 1.2f
                },
                new AscensionLevel
                {
                    level = 2,
                    description = "Starting money -100, rival very aggressive.",
                    rivalAggressionMultiplier = 1.5f,
                    startingMoneyModifier = -100,
                    crisisFrequencyMultiplier = 1.3f
                },
                new AscensionLevel
                {
                    level = 3,
                    description = "Hardest mode. Crisis frequency x1.5, rival merciless.",
                    rivalAggressionMultiplier = 2.0f,
                    startingMoneyModifier = -200,
                    crisisFrequencyMultiplier = 1.5f
                },
            };

            MarkDirty(meta);
            Debug.Log("[AssetGenerator] MetaProgression.asset created.");
        }
    }
}
#endif
