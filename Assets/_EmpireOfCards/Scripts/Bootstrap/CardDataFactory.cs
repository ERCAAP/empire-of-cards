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

            // B01 - Bufe
            cards[0] = CreateBusiness("B01_Bufe", "Bufe", Rarity.Common,
                "Kosebasindan bir bufe. Mutevazi ama sadik.", 0, 50, 3, 1,
                new[] { CardTag.Food, CardTag.Basic });
            cards[0].canEvolve = true;

            // B02 - Kahveci
            cards[1] = CreateBusiness("B02_Kahveci", "Kahveci", Rarity.Common,
                "Trendy bir kahve dukkani. Trend aktifken cok kazandirir.", 150, 80, 5, 2,
                new[] { CardTag.Food, CardTag.Coffee, CardTag.Trendy });
            cards[1].hasTrendBonus = true;
            cards[1].trendIncomeMultiplier = 1.5f;

            // B03 - Burger Zinciri
            cards[2] = CreateBusiness("B03_BurgerZinciri", "Burger Zinciri", Rarity.Uncommon,
                "Fast-food zinciri. Guclu ve istikrarli.", 250, 100, 6, 3,
                new[] { CardTag.Food, CardTag.Chain });

            // B04 - Tech Startup
            cards[3] = CreateBusiness("B04_TechStartup", "Tech Startup", Rarity.Uncommon,
                "3 tur sonra aktif olur ama yuksek gelir.", 200, 150, 4, 2,
                new[] { CardTag.Tech, CardTag.Startup });
            cards[3].activationDelay = 3;

            // B05 - Gece Kulubu
            cards[4] = CreateBusiness("B05_GeceKulubu", "Gece Kulubu", Rarity.Rare,
                "Trend aktifken muhtesem, yoksa kapali.", 350, 180, 10, 2,
                new[] { CardTag.Entertainment, CardTag.Nightlife, CardTag.Trendy });
            cards[4].requiresTrendToOperate = true;

            // B06 - Organik Ciftlik
            cards[5] = CreateBusiness("B06_OrganikCiftlik", "Organik Ciftlik", Rarity.Common,
                "Tum Food isletmelere +20 gelir verir.", 120, 40, 2, 1,
                new[] { CardTag.Food, CardTag.Organic, CardTag.Support });
            cards[5].foodBonusTag = "Food";
            cards[5].foodBonusAmount = 20;

            // B07 - Kripto Borsasi
            cards[6] = CreateBusiness("B07_KriptoBorsasi", "Kripto Borsasi", Rarity.Rare,
                "Geliri rastgele: 0 ile 250 arasi. Kumar.", 300, 0, 2, 1,
                new[] { CardTag.Tech, CardTag.Crypto, CardTag.Risky });
            cards[6].hasRandomIncome = true;
            cards[6].randomIncomeMin = 0;
            cards[6].randomIncomeMax = 250;

            // B08 - Reklam Ajansi
            cards[7] = CreateBusiness("B08_ReklamAjansi", "Reklam Ajansi", Rarity.Uncommon,
                "Tum isletmelere +2 musteri/tur.", 200, 60, 3, 2,
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

            // C01 - Stajyer
            cards[0] = CreateEmployee("C01_Stajyer", "Stajyer", Rarity.Common,
                "Ucuz ve basit. +1 musteri. Aktif: +3 musteri bu tur.", 15,
                tags: new[] { CardTag.Basic });
            cards[0].customerBonus = 1;
            cards[0].activeAbilityType = ActiveAbilityType.AddCustomersThisTurn;
            cards[0].activeAbilityName = "Ekstra Caba";
            cards[0].activeAbilityDesc = "Bu tur isletmeye +3 musteri ekler.";
            cards[0].abilityValue2 = 3;

            // C02 - Caylak Pazarlamaci
            cards[1] = CreateEmployee("C02_CaylakPazarlamaci", "Caylak Pazarlamaci", Rarity.Common,
                "+%10 gelir. Aktif: +5 musteri bu tur.", 20,
                tags: new[] { CardTag.Marketing, CardTag.Basic });
            cards[1].incomeMultiplier = 0.10f;
            cards[1].activeAbilityType = ActiveAbilityType.AddCustomersThisTurn;
            cards[1].activeAbilityName = "Brosur Dagitimi";
            cards[1].activeAbilityDesc = "Bu tur isletmeye +5 musteri ekler.";
            cards[1].abilityValue2 = 5;

            // C03 - Barista
            cards[2] = CreateEmployee("C03_Barista", "Barista", Rarity.Uncommon,
                "+3 musteri, Coffee isletmede +6. Aktif: musteri x2 bu tur.", 25,
                tags: new[] { CardTag.Food, CardTag.Coffee });
            cards[2].customerBonus = 3;
            cards[2].synergyCustomerBonus = 6;
            cards[2].synergyTag = CardTag.Coffee;
            cards[2].activeAbilityType = ActiveAbilityType.MultiplyCustomersThisTurn;
            cards[2].activeAbilityName = "Latte Festivali";
            cards[2].activeAbilityDesc = "Bu tur musteriler x2.";
            cards[2].abilityValue1 = 2f;

            // C04 - Sef
            cards[3] = CreateEmployee("C04_Sef", "Sef", Rarity.Uncommon,
                "+3 musteri, Food isletmede +30 gelir. Aktif: gelir x1.5 bu tur.", 30,
                tags: new[] { CardTag.Food });
            cards[3].customerBonus = 3;
            cards[3].incomeFlatBonus = 30f;
            cards[3].incomeBonusTag = CardTag.Food;
            cards[3].activeAbilityType = ActiveAbilityType.MultiplyIncomeThisTurn;
            cards[3].activeAbilityName = "Ozel Menu";
            cards[3].activeAbilityDesc = "Bu tur gelir x1.5.";
            cards[3].abilityValue1 = 1.5f;

            // C05 - Marketing Gurusu
            cards[4] = CreateEmployee("C05_MarketingGurusu", "Marketing Gurusu", Rarity.Rare,
                "+%25 gelir. Aktif: tum isletmelere +3 musteri.", 45,
                tags: new[] { CardTag.Marketing, CardTag.Guru });
            cards[4].incomeMultiplier = 0.25f;
            cards[4].activeAbilityType = ActiveAbilityType.AddCustomersToAll;
            cards[4].activeAbilityName = "Kampanya Patlamasi";
            cards[4].activeAbilityDesc = "Tum isletmelere +3 musteri.";
            cards[4].abilityValue2 = 3;

            // C06 - Influencer
            cards[5] = CreateEmployee("C06_Influencer", "Influencer", Rarity.Rare,
                "+5 musteri, Trendy'de +12. Aktif: rakipten 5 musteri cal.", 50,
                tags: new[] { CardTag.Marketing, CardTag.Influencer, CardTag.Trendy });
            cards[5].customerBonus = 5;
            cards[5].synergyCustomerBonus = 12;
            cards[5].synergyTag = CardTag.Trendy;
            cards[5].activeAbilityType = ActiveAbilityType.StealCustomersFromRival;
            cards[5].activeAbilityName = "Viral Icerik";
            cards[5].activeAbilityDesc = "Rakipten 5 musteri cal.";
            cards[5].abilityValue2 = 5;

            // C07 - Hacker
            cards[6] = CreateEmployee("C07_Hacker", "Hacker", Rarity.Rare,
                "Rakipten -4 musteri calar ama FBI riski +%10/tur.", 60,
                tags: new[] { CardTag.Tech, CardTag.Illegal });
            cards[6].customerBonus = -4;
            cards[6].fbiRiskPerTurn = 10;
            cards[6].activeAbilityType = ActiveAbilityType.None;
            cards[6].activeAbilityName = "Pasif Sizma";
            cards[6].activeAbilityDesc = "Her tur rakipten 4 musteri calar (pasif).";

            // C08 - Muhasebeci
            cards[7] = CreateEmployee("C08_Muhasebeci", "Muhasebeci", Rarity.Uncommon,
                "Vergiyi %50 azaltir. Aktif: bu tur vergi %0.", 30,
                tags: new[] { CardTag.Finance });
            cards[7].taxReduction = 0.5f;
            cards[7].activeAbilityType = ActiveAbilityType.NullifyTaxThisTurn;
            cards[7].activeAbilityName = "Yaratici Muhasebe";
            cards[7].activeAbilityDesc = "Bu tur vergi sifirlanir.";

            // C09 - Dolandirici
            cards[8] = CreateEmployee("C09_Dolandirici", "Dolandirici", Rarity.Rare,
                "+120 yasadisi gelir/tur. FBI +%12/tur. Aktif: +300 ama ceza.", 40,
                tags: new[] { CardTag.Illegal, CardTag.Finance });
            cards[8].illegalIncomePerTurn = 120;
            cards[8].fbiRiskPerTurn = 12;
            cards[8].activeAbilityType = ActiveAbilityType.BonusIncomeWithPenalty;
            cards[8].activeAbilityName = "Buyuk Vurgun";
            cards[8].activeAbilityDesc = "+300 aninda ama sonraki tur -150.";
            cards[8].abilityValue2 = 300;

            // C10 - Sadik Mudur
            cards[9] = CreateEmployee("C10_SadikMudur", "Sadik Mudur", Rarity.Uncommon,
                "Transfer korumasi. +20 gelir. Aktif: tum calisanlar motive.", 45,
                tags: new[] { CardTag.Management });
            cards[9].customerBonus = 0;
            cards[9].incomeFlatBonus = 20f;
            cards[9].preventsTransfer = true;
            cards[9].activeAbilityType = ActiveAbilityType.MotivateAllEmployees;
            cards[9].activeAbilityName = "Motivasyon Konusmasi";
            cards[9].activeAbilityDesc = "Tum calisanlar bu tur +1 musteri.";

            return cards;
        }

        // ----------------------------------------------------------------
        // 10 ACTION CARDS
        // ----------------------------------------------------------------

        private static CardData[] CreateActionCards()
        {
            var cards = new CardData[10];

            // A01 - El Ilani
            cards[0] = CreateAction("A01_ElIlani", "El Ilani", Rarity.Common,
                "Rastgele isletmeye +3 musteri.", 0,
                ActionEffectType.AddCustomersToRandom, 3, 0f, 0,
                new[] { CardTag.Marketing, CardTag.Basic });

            // A02 - Kucuk Yatirim
            cards[1] = CreateAction("A02_KucukYatirim", "Kucuk Yatirim", Rarity.Common,
                "Aninda +150 para.", 0,
                ActionEffectType.AddMoneyInstant, 150, 0f, 0,
                new[] { CardTag.Finance, CardTag.Basic });

            // A03 - Viral Pazarlama
            cards[2] = CreateAction("A03_ViralPazarlama", "Viral Pazarlama", Rarity.Uncommon,
                "Tum musteriler x2 bu tur.", 150,
                ActionEffectType.MultiplyAllCustomers, 0, 2f, 0,
                new[] { CardTag.Marketing, CardTag.Viral });

            // A04 - Dusmanca Devralma
            cards[3] = CreateAction("A04_DusmancaDevralma", "Dusmanca Devralma", Rarity.Rare,
                "Rakibin en zayif isletmesini kapat.", 400,
                ActionEffectType.CloseRivalWeakestBusiness, 0, 0f, 0,
                new[] { CardTag.Aggressive });

            // A05 - Sahte Yorumlar
            cards[4] = CreateAction("A05_SahteYorumlar", "Sahte Yorumlar", Rarity.Uncommon,
                "+8 musteri ama FBI +%12.", 80,
                ActionEffectType.AddCustomersWithFBI, 8, 0f, 12,
                new[] { CardTag.Marketing, CardTag.Illegal });

            // A06 - Fiyat Kirma
            cards[5] = CreateAction("A06_FiyatKirma", "Fiyat Kirma", Rarity.Uncommon,
                "+8 musteri cal, gelir %50 feda.", 0,
                ActionEffectType.StealCustomersHalfIncome, 8, 0f, 0,
                new[] { CardTag.Aggressive, CardTag.Pricing });
            cards[5].actionIncomeSacrifice = 0.5f;

            // A07 - Sabotaj
            cards[6] = CreateAction("A07_Sabotaj", "Sabotaj", Rarity.Rare,
                "Rakip 1 tur uretim yapamaz. FBI +%15.", 250,
                ActionEffectType.DisableRivalOneTurn, 0, 0f, 15,
                new[] { CardTag.Aggressive, CardTag.Illegal });

            // A08 - Yatirimci Sunumu
            cards[7] = CreateAction("A08_YatirimciSunumu", "Yatirimci Sunumu", Rarity.Uncommon,
                "+600 aninda, 3 tur %15 gelir yatirimciya.", 0,
                ActionEffectType.MoneyNowPayLater, 600, 0f, 0,
                new[] { CardTag.Finance, CardTag.Investor });
            cards[7].actionDebtDuration = 3;
            cards[7].actionDebtPercent = 0.15f;

            // A09 - Acil Ise Alim
            cards[8] = CreateAction("A09_AcilIseAlim", "Acil Ise Alim", Rarity.Uncommon,
                "Rastgele calisan cek ve hemen oyna.", 100,
                ActionEffectType.DrawAndPlayEmployee, 0, 0f, 0,
                new[] { CardTag.Hiring });

            // A10 - Tasfiye Satisi
            cards[9] = CreateAction("A10_TasfiyeSatisi", "Tasfiye Satisi", Rarity.Common,
                "Isletmeyi sat, 2x fiyatini geri al.", 0,
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

            // U01 - Ofis Malzemeleri
            cards[0] = CreateUpgrade("U01_OfisMalzemeleri", "Ofis Malzemeleri", Rarity.Common,
                "Bir isletmenin gelirine +%10.", 0,
                UpgradeEffectType.IncomePercentSingle, 10f, false, 0, 0,
                new[] { CardTag.Basic, CardTag.Office });

            // U02 - Otomasyon
            cards[1] = CreateUpgrade("U02_Otomasyon", "Otomasyon", Rarity.Uncommon,
                "+%30 gelir ama 1 calisan slotu kapanir.", 300,
                UpgradeEffectType.IncomePercentWithSlotLoss, 30f, false, 1, 0,
                new[] { CardTag.Tech, CardTag.Automation });

            // U03 - Teslimat Agi
            cards[2] = CreateUpgrade("U03_TeslimatAgi", "Teslimat Agi", Rarity.Uncommon,
                "Tum isletmelere +2 musteri/tur.", 250,
                UpgradeEffectType.GlobalCustomerPerTurn, 2f, true, 0, 0,
                new[] { CardTag.Logistics });

            // U04 - Reklam Panosu
            cards[3] = CreateUpgrade("U04_ReklamPanosu", "Reklam Panosu", Rarity.Common,
                "+3 musteri/tur genel.", 120,
                UpgradeEffectType.GlobalCustomerFlat, 3f, true, 0, 0,
                new[] { CardTag.Marketing });

            // U05 - Guvenlik Sistemi
            cards[4] = CreateUpgrade("U05_GuvenlikSistemi", "Guvenlik Sistemi", Rarity.Uncommon,
                "FBI riski -%25.", 280,
                UpgradeEffectType.ReduceFBIRisk, 25f, true, 0, 0,
                new[] { CardTag.Security });

            // U06 - Yapay Zeka Asistani
            cards[5] = CreateUpgrade("U06_YapayZekaAsistani", "Yapay Zeka Asistani", Rarity.Rare,
                "+1 ekstra aksiyon hakki.", 400,
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

            // E01 - Kahve Cilginligi
            cards[0] = CreateEvent("E01_KahveCilginligi", "Kahve Cilginligi", Rarity.Common,
                "2 tur: Food/Coffee isletmelere +%50 musteri.", 2,
                EventEffectType.TagCustomerBoost, 0.5f,
                new[] { CardTag.Food, CardTag.Coffee }, 0, 0f);

            // E02 - Ekonomik Kriz
            cards[1] = CreateEvent("E02_EkonomikKriz", "Ekonomik Kriz", Rarity.Common,
                "2 tur: Tum gelirler -%30.", 2,
                EventEffectType.AllIncomeReduction, -0.3f,
                new CardTag[0], 0, 0f);

            // E03 - Viral Trend
            cards[2] = CreateEvent("E03_ViralTrend", "Viral Trend", Rarity.Uncommon,
                "1 tur: Marketing kartlari 2x etki.", 1,
                EventEffectType.TagDoubleEffect, 1.0f,
                new[] { CardTag.Marketing }, 0, 0f);

            // E04 - Veri Sizintisi
            cards[3] = CreateEvent("E04_VeriSizintisi", "Veri Sizintisi", Rarity.Uncommon,
                "1 tur: Tech isletmeler -5 musteri.", 1,
                EventEffectType.TagCustomerPenalty, 0f,
                new[] { CardTag.Tech }, -5, 0f);

            // E05 - Yatirimci Sezonu
            cards[4] = CreateEvent("E05_YatirimciSezonu", "Yatirimci Sezonu", Rarity.Uncommon,
                "1 tur: Finance kartlar 2x etki.", 1,
                EventEffectType.TagDoubleEffectFinance, 1.0f,
                new[] { CardTag.Finance }, 0, 0f);

            // E06 - Iptal Kulturu
            cards[5] = CreateEvent("E06_IptalKulturu", "Iptal Kulturu", Rarity.Rare,
                "1 tur: FBI >%30 olan oyuncunun musterileri -%40.", 1,
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

            // Genel
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

            // Ekonomi
            gb.taxRate = 0.15f;
            gb.reducedTaxRate = 0.075f;
            gb.minTaxRate = 0.03f;
            gb.sellRate = 0.4f;

            // FBI
            gb.fbiRaidPenalty = 300;
            gb.fbiStartingRisk = 0f;

            // Bolge
            gb.totalTerritories = 10;
            gb.winTerritories = 6;
            gb.loseTerritories = 7;

            // Market Havuzu
            gb.baseMarketCustomers = 60;
            gb.earlyGrowthPerTurn = 5;
            gb.midGrowthPerTurn = 6;
            gb.lateGrowthPerTurn = 8;
            gb.endGrowthPerTurn = 10;

            // Skor
            gb.territoryScoreMultiplier = 500;
            gb.moneyScoreMultiplier = 1;
            gb.comboScoreMultiplier = 200;
            gb.businessScoreMultiplier = 100;
            gb.earlyFinishBonusPerTurn = 300;
            gb.fbiEvasionBonus = 50;
            gb.winBonus = 1000;

            // Isletme Evrimi
            gb.evolutionCustomerThreshold = 40;
            gb.evolutionTurnRequirement = 15;

            // Calisan
            gb.employeeLeaveTurnThreshold = 8;

            return gb;
        }

        // ================================================================
        // STARTING DECK (14 cards)
        // ================================================================

        private static DeckPresetData CreateStartingDeck()
        {
            var deck = ScriptableObject.CreateInstance<DeckPresetData>();
            deck.name = "BaslangicDestesi_Runtime";

            deck.presetName = "Baslangic Destesi";
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
            rival.tagline = "Bu sektorde ikimize yer yok.";

            // Baslangic
            rival.startingMoney = 400;
            rival.startingIncome = 80;
            rival.startingCustomers = 5;
            rival.startingBusinessName = "MegaCorp HQ";

            // Davranis
            rival.actionsPerTurn = 2;
            rival.aggressionThreshold = 0.5f;
            rival.maxBusinesses = 4;
            rival.maxEmployeesPerBusiness = 3;

            // Buyume Parametreleri
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
                "Teknoloji Magazasi",
                "Supermarket",
                "Kafe Zinciri",
                "Fitness Salonu"
            };

            // Buyume Takvimi (GDD Section 8.3)
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

            // Diyalog
            rival.growingTaunts = new[]
            {
                "Pazar payimiz artiyor.",
                "Bu sadece baslangic."
            };
            rival.playerGrowingTaunts = new[]
            {
                "Ilginc bir hamle...",
                "Bunu beklemiyordum."
            };
            rival.aggressiveTaunts = new[]
            {
                "Bu sektorde ikimize yer yok.",
                "Rekabet sertlesiyor."
            };
            rival.losingTaunts = new[]
            {
                "Bu boyle bitmez.",
                "Geri donecegiz."
            };
            rival.winningTaunts = new[]
            {
                "Kacinilmazdi.",
                "Piyasa bizim."
            };

            return rival;
        }

        // ================================================================
        // 10 COMBOS
        // ================================================================

        private static ComboData[] CreateAllCombos()
        {
            var combos = new ComboData[10];

            // COMBO 01 - Latte Sanati (Easy)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_01_LatteSanati";
                c.comboId = "COMBO_01_LatteSanati";
                c.comboName = "Latte Sanati";
                c.displayText = "LATTE SANATI!";
                c.tier = ComboTier.Easy;
                c.description = "Kahveci + Barista = musteri x2, gelir +%50.";
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

            // COMBO 02 - Gurme Deneyimi (Easy)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_02_GurmeDeneyimi";
                c.comboId = "COMBO_02_GurmeDeneyimi";
                c.comboName = "Gurme Deneyimi";
                c.displayText = "GURME DENEYIMI!";
                c.tier = ComboTier.Easy;
                c.description = "Burger Zinciri + Sef = gelir +30, musteri +%50.";
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

            // COMBO 03 - Dijital Pazarlama (Easy)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_03_DijitalPazarlama";
                c.comboId = "COMBO_03_DijitalPazarlama";
                c.comboName = "Dijital Pazarlama";
                c.displayText = "DIJITAL PAZARLAMA!";
                c.tier = ComboTier.Easy;
                c.description = "Tech Startup + Marketing Gurusu = gelir x2.";
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

            // COMBO 04 - Gece Hayati Imparatoru (Medium)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_04_GeceHayatiImparatoru";
                c.comboId = "COMBO_04_GeceHayatiImparatoru";
                c.comboName = "Gece Hayati Imparatoru";
                c.displayText = "GECE HAYATI IMPARATORU!";
                c.tier = ComboTier.Medium;
                c.description = "Gece Kulubu + Influencer + Viral Trend eventi = musteri x3.";
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

            // COMBO 05 - Yeralti Ekonomisi (Medium)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_05_YeraltiEkonomisi";
                c.comboId = "COMBO_05_YeraltiEkonomisi";
                c.comboName = "Yeralti Ekonomisi";
                c.displayText = "YERALTI EKONOMISI!";
                c.tier = ComboTier.Medium;
                c.description = "Hacker + Dolandirici = +200 gelir/tur ama FBI +%8 ekstra.";
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

            // COMBO 06 - Vergi Cenneti (Medium)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_06_VergiCenneti";
                c.comboId = "COMBO_06_VergiCenneti";
                c.comboName = "Vergi Cenneti";
                c.displayText = "VERGI CENNETI!";
                c.tier = ComboTier.Medium;
                c.description = "Muhasebeci + Dolandirici = yasadisi gelir vergisiz.";
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

            // COMBO 07 - AI Devrimi (Hard)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_07_AIDevrimi";
                c.comboId = "COMBO_07_AIDevrimi";
                c.comboName = "AI Devrimi";
                c.displayText = "AI DEVRIMI!";
                c.tier = ComboTier.Hard;
                c.description = "Tech Startup + Otomasyon + Yapay Zeka Asistani = +1 aksiyon, gelir x2.";
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

            // COMBO 08 - Organik Zincir (Medium)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_08_OrganikZincir";
                c.comboId = "COMBO_08_OrganikZincir";
                c.comboName = "Organik Zincir";
                c.displayText = "ORGANIK ZINCIR!";
                c.tier = ComboTier.Medium;
                c.description = "Organik Ciftlik + Burger Zinciri + Sef = tum Food'a +50 gelir.";
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

            // COMBO 09 - Kriz Avcisi (Hard)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_09_KrizAvcisi";
                c.comboId = "COMBO_09_KrizAvcisi";
                c.comboName = "Kriz Avcisi";
                c.displayText = "KRIZ AVCISI!";
                c.tier = ComboTier.Hard;
                c.description = "Ekonomik Kriz aktifken 1000+ paran varsa: dukkan %50, rakipten 1 calisan.";
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

            // COMBO 10 - Monopol (Automatic)
            {
                var c = ScriptableObject.CreateInstance<ComboData>();
                c.name = "COMBO_10_Monopol";
                c.comboId = "COMBO_10_Monopol";
                c.comboName = "Monopol";
                c.displayText = "MONOPOL!";
                c.tier = ComboTier.Automatic;
                c.description = "4+ isletme, %55+ pazar payi = rakip -3 musteri/tur, gelir +%20.";
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
