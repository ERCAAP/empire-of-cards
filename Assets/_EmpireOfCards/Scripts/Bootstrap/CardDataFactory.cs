using UnityEngine;
using System.Collections.Generic;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Creates ALL 40 CardData ScriptableObjects, combos, balance data,
    /// starting deck, rival data, and shop pool -- entirely in memory.
    /// </summary>
    public static class CardDataFactory
    {
        private static Dictionary<string, CardData> _cardLookup;

        /// <summary>
        /// Single entry point. Creates every piece of game data and returns it bundled.
        /// </summary>
        public static GameDataBundle CreateAllData()
        {
            _cardLookup = new Dictionary<string, CardData>();

            var bundle = new GameDataBundle();

            // 1. All 40 cards
            bundle.allCards = CreateAllCards();

            // 2. Game balance
            bundle.balanceData = CreateGameBalance();

            // 3. Starting deck (14 cards)
            bundle.startingDeck = CreateStartingDeck();

            // 4. MegaCorp rival
            bundle.rivalData = CreateMegaCorpRival();

            // 5. 10 Combos
            bundle.combos = CreateAllCombos();

            // 6. Shop pool (all non-starter cards)
            bundle.shopPool = CreateShopPool(bundle.allCards);

            // 7. Expose the lookup so wiring can use it if needed
            bundle.cardLookup = _cardLookup;

            Debug.Log($"[CardDataFactory] Data created: {bundle.allCards.Length} cards, " +
                      $"{bundle.combos.Length} combos, shop pool: {bundle.shopPool.Length} cards.");

            return bundle;
        }

        // ================================================================
        // CARD HELPER METHODS
        // ================================================================

        private static CardData CreateCard(string id, string name, CardType type, Rarity rarity,
            int cost, string desc, CardTag[] tags)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardId = id;
            card.cardName = name;
            card.cardType = type;
            card.rarity = rarity;
            card.buyCost = cost;
            card.description = desc;
            card.tags = tags;
            card.name = id; // For debug display
            _cardLookup[id] = card;
            return card;
        }

        private static CardData CreateBusiness(string id, string name, Rarity rarity,
            string desc, int cost, int income, int customers, int slots, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Business, rarity, cost, desc, tags);
            c.incomePerTurn = income;
            c.customersPerTurn = customers;
            c.employeeSlots = slots;
            c.hasTrendBonus = false;
            c.trendIncomeMultiplier = 1f;
            c.activationDelay = 0;
            c.requiresTrendToOperate = false;
            c.hasRandomIncome = false;
            c.randomIncomeMin = 0;
            c.randomIncomeMax = 0;
            c.foodBonusTag = "";
            c.foodBonusAmount = 0;
            c.globalCustomerBonus = 0;
            c.canEvolve = false;
            return c;
        }

        private static CardData CreateEmployee(string id, string name, Rarity rarity,
            string desc, int salary, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Employee, rarity, 0, desc, tags);
            c.salaryPerTurn = salary;
            c.customerBonus = 0;
            c.synergyCustomerBonus = 0;
            c.synergyTag = CardTag.Food;
            c.incomeMultiplier = 0f;
            c.incomeFlatBonus = 0f;
            c.incomeBonusTag = CardTag.Food;
            c.fbiRiskPerTurn = 0;
            c.illegalIncomePerTurn = 0;
            c.preventsTransfer = false;
            c.taxReduction = 0f;
            c.activeAbilityType = ActiveAbilityType.None;
            c.activeAbilityName = "";
            c.activeAbilityDesc = "";
            c.abilityValue1 = 0f;
            c.abilityValue2 = 0;
            return c;
        }

        private static CardData CreateAction(string id, string name, Rarity rarity,
            string desc, int cost, ActionEffectType effect, int value, float multiplier,
            int fbiRisk, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Action, rarity, cost, desc, tags);
            c.actionEffectType = effect;
            c.actionValue = value;
            c.actionMultiplier = multiplier;
            c.actionFBIRisk = fbiRisk;
            c.actionDebtDuration = 0;
            c.actionDebtPercent = 0f;
            c.actionIncomeSacrifice = 0f;
            return c;
        }

        private static CardData CreateUpgrade(string id, string name, Rarity rarity,
            string desc, int cost, UpgradeEffectType effect, float value, bool isGlobal,
            int closedSlots, int extraActs, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Upgrade, rarity, cost, desc, tags);
            c.upgradeEffectType = effect;
            c.upgradeValue = value;
            c.isGlobalUpgrade = isGlobal;
            c.closedEmployeeSlots = closedSlots;
            c.extraActions = extraActs;
            return c;
        }

        private static CardData CreateEvent(string id, string name, Rarity rarity,
            string desc, int duration, EventEffectType effect, float multiplier,
            CardTag[] affected, int customerPenalty, float fbiThreshold)
        {
            var c = CreateCard(id, name, CardType.Event, rarity, 0, desc, new CardTag[0]);
            c.eventEffectType = effect;
            c.eventDuration = duration;
            c.eventMultiplier = multiplier;
            c.affectedTags = affected;
            c.eventCustomerPenalty = customerPenalty;
            c.eventFBIThreshold = fbiThreshold;
            return c;
        }

        private static CardData FindCard(string id)
        {
            if (_cardLookup.TryGetValue(id, out var card)) return card;
            Debug.LogError($"[CardDataFactory] Card not found: {id}");
            return null;
        }

        // ================================================================
        // ALL 40 CARDS
        // ================================================================

        private static CardData[] CreateAllCards()
        {
            var all = new List<CardData>(40);
            all.AddRange(CreateBusinessCards());
            all.AddRange(CreateEmployeeCards());
            all.AddRange(CreateActionCards());
            all.AddRange(CreateUpgradeCards());
            all.AddRange(CreateEventCards());
            return all.ToArray();
        }

        // ----------------------------------------------------------------
        // 8 BUSINESS CARDS
        // ----------------------------------------------------------------

        private static CardData[] CreateBusinessCards()
        {
            var cards = new CardData[8];

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

        // ----------------------------------------------------------------
        // 10 EMPLOYEE CARDS
        // ----------------------------------------------------------------

        private static CardData[] CreateEmployeeCards()
        {
            var cards = new CardData[10];

            // C01 - Intern
            cards[0] = CreateEmployee("C01_Stajyer", "Intern", Rarity.Common,
                "Cheap but weak. Starter card.", 15,
                tags: new[] { CardTag.Basic });
            cards[0].customerBonus = 1;
            cards[0].activeAbilityType = ActiveAbilityType.AddCustomersThisTurn;
            cards[0].activeAbilityName = "Hustle";
            cards[0].activeAbilityDesc = "Adds +3 customers to this business this turn.";
            cards[0].abilityValue2 = 3;

            // C02 - Junior Marketer
            cards[1] = CreateEmployee("C02_CaylakPazarlamaci", "Junior Marketer", Rarity.Common,
                "Small but consistent bonus.", 20,
                tags: new[] { CardTag.Marketing, CardTag.Basic });
            cards[1].incomeMultiplier = 0.10f;
            cards[1].activeAbilityType = ActiveAbilityType.AddCustomersThisTurn;
            cards[1].activeAbilityName = "Campaign";
            cards[1].activeAbilityDesc = "Adds +5 customers to this business this turn.";
            cards[1].abilityValue2 = 5;

            // C03 - Barista
            cards[2] = CreateEmployee("C03_Barista", "Barista", Rarity.Uncommon,
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
            cards[3] = CreateEmployee("C04_Sef", "Chef", Rarity.Uncommon,
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
            cards[4] = CreateEmployee("C05_MarketingGurusu", "Marketing Guru", Rarity.Rare,
                "Expensive but powerful. Combo piece.", 45,
                tags: new[] { CardTag.Marketing, CardTag.Guru });
            cards[4].incomeMultiplier = 0.25f;
            cards[4].activeAbilityType = ActiveAbilityType.AddCustomersToAll;
            cards[4].activeAbilityName = "Viral Campaign";
            cards[4].activeAbilityDesc = "+3 customers to all businesses.";
            cards[4].abilityValue2 = 3;

            // C06 - Influencer
            cards[5] = CreateEmployee("C06_Influencer", "Influencer", Rarity.Rare,
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
            cards[6] = CreateEmployee("C07_Hacker", "Hacker", Rarity.Rare,
                "Powerful but dangerous. FBI risk every turn.", 60,
                tags: new[] { CardTag.Tech, CardTag.Illegal });
            cards[6].customerBonus = -4;
            cards[6].fbiRiskPerTurn = 10;
            cards[6].activeAbilityType = ActiveAbilityType.None;
            cards[6].activeAbilityName = "Passive Infiltration";
            cards[6].activeAbilityDesc = "Steals 4 customers from rival each turn (passive).";

            // C08 - Accountant
            cards[7] = CreateEmployee("C08_Muhasebeci", "Accountant", Rarity.Uncommon,
                "Boring but saves every penny.", 30,
                tags: new[] { CardTag.Finance });
            cards[7].taxReduction = 0.5f;
            cards[7].activeAbilityType = ActiveAbilityType.NullifyTaxThisTurn;
            cards[7].activeAbilityName = "Tax Plan";
            cards[7].activeAbilityDesc = "Tax is nullified this turn.";

            // C09 - Fraudster
            cards[8] = CreateEmployee("C09_Dolandirici", "Fraudster", Rarity.Rare,
                "Fast money. But FBI is knocking.", 40,
                tags: new[] { CardTag.Illegal, CardTag.Finance });
            cards[8].illegalIncomePerTurn = 120;
            cards[8].fbiRiskPerTurn = 12;
            cards[8].activeAbilityType = ActiveAbilityType.BonusIncomeWithPenalty;
            cards[8].activeAbilityName = "Ponzi";
            cards[8].activeAbilityDesc = "+300 instant but -150 next turn.";
            cards[8].abilityValue2 = 300;

            // C10 - Loyal Manager
            cards[9] = CreateEmployee("C10_SadikMudur", "Loyal Manager", Rarity.Uncommon,
                "Rival can't steal employees. Defensive.", 45,
                tags: new[] { CardTag.Management });
            cards[9].customerBonus = 0;
            cards[9].incomeFlatBonus = 20f;
            cards[9].preventsTransfer = true;
            cards[9].activeAbilityType = ActiveAbilityType.MotivateAllEmployees;
            cards[9].activeAbilityName = "Motivation";
            cards[9].activeAbilityDesc = "All employees gain +1 customer this turn.";

            return cards;
        }

        // ----------------------------------------------------------------
        // 10 ACTION CARDS
        // ----------------------------------------------------------------

        private static CardData[] CreateActionCards()
        {
            var cards = new CardData[10];

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
            cards[7].actionDebtPercent = 0.15f;

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

        // ----------------------------------------------------------------
        // 6 UPGRADE CARDS
        // ----------------------------------------------------------------

        private static CardData[] CreateUpgradeCards()
        {
            var cards = new CardData[6];

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

        // ----------------------------------------------------------------
        // 6 EVENT CARDS
        // ----------------------------------------------------------------

        private static CardData[] CreateEventCards()
        {
            var cards = new CardData[6];

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

        // ================================================================
        // GAME BALANCE
        // ================================================================

        private static GameBalanceData CreateGameBalance()
        {
            var gb = ScriptableObject.CreateInstance<GameBalanceData>();
            gb.name = "GameBalance_Runtime";

            // General
            gb.startingMoney = 500;
            gb.maxTurns = 20;
            gb.startingActions = 3;
            gb.maxActions = 5;
            gb.startingBusinessSlots = 3;
            gb.maxBusinessSlots = 5;
            gb.handSize = 5;
            gb.redrawsPerTurn = 1;
            gb.shopCardsPerTurn = 3;
            gb.eventInterval = 3;

            // Economy
            gb.taxRate = 0.15f;
            gb.reducedTaxRate = 0.075f;
            gb.minTaxRate = 0.03f;
            gb.sellRate = 0.4f;

            // FBI
            gb.fbiRaidPenalty = 300;
            gb.fbiStartingRisk = 0f;

            // Territory
            gb.totalTerritories = 10;
            gb.winTerritories = 6;
            gb.loseTerritories = 7;

            // Market Pool
            gb.baseMarketCustomers = 60;
            gb.earlyGrowthPerTurn = 5;
            gb.midGrowthPerTurn = 6;
            gb.lateGrowthPerTurn = 8;
            gb.endGrowthPerTurn = 10;

            // Score
            gb.territoryScoreMultiplier = 500;
            gb.moneyScoreMultiplier = 1;
            gb.comboScoreMultiplier = 200;
            gb.businessScoreMultiplier = 100;
            gb.earlyFinishBonusPerTurn = 300;
            gb.fbiEvasionBonus = 50;
            gb.winBonus = 1000;

            // Business Evolution
            gb.evolutionCustomerThreshold = 40;
            gb.evolutionTurnRequirement = 15;

            // Employee
            gb.employeeLeaveTurnThreshold = 8;

            return gb;
        }

        // ================================================================
        // STARTING DECK (14 cards)
        // ================================================================

        private static DeckPresetData CreateStartingDeck()
        {
            var deck = ScriptableObject.CreateInstance<DeckPresetData>();
            deck.name = "StarterDeck_Runtime";

            deck.presetName = "Starter Deck";
            deck.startingMoney = 500;

            CardData b01 = FindCard("B01_Bufe");
            CardData c01 = FindCard("C01_Stajyer");
            CardData c02 = FindCard("C02_CaylakPazarlamaci");
            CardData a01 = FindCard("A01_ElIlani");
            CardData a02 = FindCard("A02_KucukYatirim");
            CardData u01 = FindCard("U01_OfisMalzemeleri");

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

            return deck;
        }

        // ================================================================
        // MEGACORP RIVAL
        // ================================================================

        private static RivalData CreateMegaCorpRival()
        {
            var rival = ScriptableObject.CreateInstance<RivalData>();
            rival.name = "MegaCorp_Runtime";

            rival.rivalId = "RIVAL_MegaCorp";
            rival.rivalName = "MegaCorp";
            rival.personality = RivalPersonality.Balanced;
            rival.tagline = "This industry isn't big enough for both of us.";

            // Starting Stats
            rival.startingMoney = 400;
            rival.startingIncome = 80;
            rival.startingCustomers = 5;
            rival.startingBusinessName = "MegaCorp HQ";

            // Behavior
            rival.actionsPerTurn = 2;
            rival.aggressionThreshold = 0.5f;
            rival.maxBusinesses = 4;
            rival.maxEmployeesPerBusiness = 3;

            // Growth Parameters
            rival.businessCostThreshold = 200;
            rival.hireCostThreshold = 80;
            rival.baseBusinessIncome = 80;
            rival.baseBusinessCustomers = 5;
            rival.employeeIncomeBoost = 30;
            rival.employeeCustomerBoost = 3;
            rival.aggressiveCustomerBoost = 8;
            rival.aggressiveIncomeBoost = 50;
            rival.passiveCustomerGrowth = 2;
            rival.passiveIncomeGrowth = 10;

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

            return rival;
        }

        // ================================================================
        // 10 COMBOS
        // ================================================================

        private static ComboData[] CreateAllCombos()
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

        // ================================================================
        // SHOP POOL (all non-starter cards)
        // ================================================================

        private static CardData[] CreateShopPool(CardData[] allCards)
        {
            var starterIds = new HashSet<string>
            {
                "B01_Bufe",
                "C01_Stajyer",
                "C02_CaylakPazarlamaci",
                "A01_ElIlani",
                "A02_KucukYatirim",
                "U01_OfisMalzemeleri"
            };

            var pool = new List<CardData>();
            for (int i = 0; i < allCards.Length; i++)
            {
                var card = allCards[i];
                if (card.cardType == CardType.Event) continue;
                if (starterIds.Contains(card.cardId)) continue;
                pool.Add(card);
            }

            return pool.ToArray();
        }
    }

    /// <summary>
    /// Holds all game data created by CardDataFactory.
    /// </summary>
    public class GameDataBundle
    {
        public CardData[] allCards;
        public CardData[] shopPool;
        public GameBalanceData balanceData;
        public DeckPresetData startingDeck;
        public RivalData rivalData;
        public ComboData[] combos;
        public Dictionary<string, CardData> cardLookup;
    }
}
