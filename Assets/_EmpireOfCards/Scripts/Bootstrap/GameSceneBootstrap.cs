using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.UI;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.Audio;
using EmpireOfCards.VFX;
using EmpireOfCards.Save;
using EmpireOfCards.Helpers;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// SELF-CONTAINED bootstrap: drop on an empty GameObject, press Play, game runs.
    /// Creates ALL 40 CardData, GameBalanceData, DeckPresetData, RivalData,
    /// 10 ComboData, shop pool, managers, board, UI, VFX -- everything in memory.
    /// No Inspector assignments required.
    /// </summary>
    public class GameSceneBootstrap : MonoBehaviour
    {
        // ================================================================
        // IN-MEMORY DATA (created in Awake, no Inspector needed)
        // ================================================================
        private GameBalanceData _balanceData;
        private DeckPresetData _startingDeck;
        private RivalData _megaCorpData;
        private ComboData[] _allCombos;
        private CardData[] _shopPool;
        private CardData[] _allCards;

        // Card lookup by ID for deck/combo wiring
        private Dictionary<string, CardData> _cardLookup;

        // References created at runtime
        private GameManager _gameManager;
        private TurnManager _turnManager;
        private EconomyManager _economyManager;
        private DeckManager _deckManager;
        private BoardManager _boardManager;
        private ComboSystem _comboSystem;
        private TerritoryManager _territoryManager;
        private FBISystem _fbiSystem;
        private RivalAI _rivalAI;
        private ShopManager _shopManager;
        private UIManager _uiManager;
        private AudioManager _audioManager;
        private VFXManager _vfxManager;
        private SaveManager _saveManager;

        // UI sub-element references captured during CreateUI
        private TopBarUI _topBarUI;
        private ActionBarUI _actionBarUI;
        private HandUI _handUI;
        private ShopPanel _shopPanel;
        private ComboPopup _comboPopup;
        private EventPopup _eventPopup;
        private RivalPopup _rivalPopup;
        private ScoreScreen _scoreScreen;
        private GameOverScreen _gameOverScreen;

        // Card prefab template (created at runtime, inactive)
        private GameObject _cardPrefabInstance;

        // TopBar sub-elements
        private TMP_Text _moneyText;
        private TMP_Text _turnText;
        private Image _fbiBarFillImg;

        // ActionBar sub-elements
        private Image[] _actionDotImages;

        // Button references captured during CreateUI
        private Button _endTurnButton;
        private Button _shopButton;
        private Button _shopCloseButton;

        // AudioManager sources
        private AudioSource _musicSourceA;
        private AudioSource _musicSourceB;
        private AudioSource _sfxSource;

        private void Awake()
        {
            // ====== CREATE ALL DATA IN MEMORY ======
            CreateAllData();

            // ====== MANAGERS ======
            CreateManagers();

            // ====== CAMERA ======
            SetupCamera();

            // ====== BOARD ======
            CreateBoard();

            // ====== UI CANVAS ======
            CreateUI();

            // ====== VFX ======
            CreateVFX();

            // ====== CARD PREFAB (runtime template for HandUI) ======
            CreateCardPrefab();

            // ====== WIRE REFERENCES ======
            WireManagerReferences();

            // ====== WIRE BUTTON CALLBACKS ======
            WireButtonCallbacks();

            Debug.Log("[GameSceneBootstrap] Scene hierarchy created successfully (self-contained).");
        }

        private void Start()
        {
            // Start the game after everything is wired
            if (_gameManager != null)
                _gameManager.StartNewRun();
        }

        // ================================================================
        // DATA CREATION -- ALL 40 CARDS + BALANCE + DECK + RIVAL + COMBOS
        // ================================================================

        private void CreateAllData()
        {
            _cardLookup = new Dictionary<string, CardData>();

            // 1. All 40 cards
            _allCards = CreateAllCards();

            // 2. Game balance
            _balanceData = CreateGameBalance();

            // 3. Starting deck (14 cards)
            _startingDeck = CreateStartingDeck();

            // 4. MegaCorp rival
            _megaCorpData = CreateMegaCorpRival();

            // 5. 10 Combos
            _allCombos = CreateAllCombos();

            // 6. Shop pool (all non-starter cards)
            _shopPool = CreateShopPool();

            Debug.Log($"[GameSceneBootstrap] Data created: {_allCards.Length} cards, " +
                      $"{_allCombos.Length} combos, shop pool: {_shopPool.Length} cards.");
        }

        // ================================================================
        // CARD FACTORY HELPERS
        // ================================================================

        private CardData CreateCard(string id, string name, CardType type, Rarity rarity,
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
            // Register in lookup
            _cardLookup[id] = card;
            return card;
        }

        private CardData CreateBusiness(string id, string name, Rarity rarity,
            string desc, int cost, int income, int customers, int slots, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Business, rarity, cost, desc, tags);
            c.incomePerTurn = income;
            c.customersPerTurn = customers;
            c.employeeSlots = slots;
            // Defaults
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

        private CardData CreateEmployee(string id, string name, Rarity rarity,
            string desc, int salary, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Employee, rarity, 0, desc, tags);
            c.salaryPerTurn = salary;
            // Defaults
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

        private CardData CreateAction(string id, string name, Rarity rarity,
            string desc, int cost, ActionEffectType effect, int value, float multiplier,
            int fbiRisk, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Action, rarity, cost, desc, tags);
            c.actionEffectType = effect;
            c.actionValue = value;
            c.actionMultiplier = multiplier;
            c.actionFBIRisk = fbiRisk;
            // Defaults
            c.actionDebtDuration = 0;
            c.actionDebtPercent = 0f;
            c.actionIncomeSacrifice = 0f;
            return c;
        }

        private CardData CreateUpgrade(string id, string name, Rarity rarity,
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

        private CardData CreateEvent(string id, string name, Rarity rarity,
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

        private CardData FindCard(string id)
        {
            if (_cardLookup.TryGetValue(id, out var card)) return card;
            Debug.LogError($"[GameSceneBootstrap] Card not found: {id}");
            return null;
        }

        // ================================================================
        // ALL 40 CARDS
        // ================================================================

        private CardData[] CreateAllCards()
        {
            var all = new List<CardData>(40);

            // --- 8 Business Cards ---
            all.AddRange(CreateBusinessCards());

            // --- 10 Employee Cards ---
            all.AddRange(CreateEmployeeCards());

            // --- 10 Action Cards ---
            all.AddRange(CreateActionCards());

            // --- 6 Upgrade Cards ---
            all.AddRange(CreateUpgradeCards());

            // --- 6 Event Cards ---
            all.AddRange(CreateEventCards());

            return all.ToArray();
        }

        // ----------------------------------------------------------------
        // 8 BUSINESS CARDS
        // ----------------------------------------------------------------

        private CardData[] CreateBusinessCards()
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

        private CardData[] CreateEmployeeCards()
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

        private CardData[] CreateActionCards()
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

        private CardData[] CreateUpgradeCards()
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

        private CardData[] CreateEventCards()
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

        private GameBalanceData CreateGameBalance()
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

        private DeckPresetData CreateStartingDeck()
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

        private RivalData CreateMegaCorpRival()
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

        private ComboData[] CreateAllCombos()
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

        private CardData[] CreateShopPool()
        {
            // Starter card IDs that should NOT be in the shop
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
            for (int i = 0; i < _allCards.Length; i++)
            {
                var card = _allCards[i];
                // Exclude starters and events from shop
                if (card.cardType == CardType.Event) continue;
                if (starterIds.Contains(card.cardId)) continue;
                pool.Add(card);
            }

            return pool.ToArray();
        }

        // ================================================================
        // MANAGERS (GDD 12.2 - Managers section)
        // ================================================================

        private void CreateManagers()
        {
            // --- GameManager (Singleton, DontDestroyOnLoad) ---
            var gmGo = new GameObject("[GameManager]");
            _gameManager = gmGo.AddComponent<GameManager>();
            // GameManager handles its own DontDestroyOnLoad in Awake

            // --- TurnManager ---
            var tmGo = new GameObject("TurnManager");
            tmGo.transform.SetParent(gmGo.transform);
            _turnManager = tmGo.AddComponent<TurnManager>();

            // --- EconomyManager ---
            var emGo = new GameObject("EconomyManager");
            emGo.transform.SetParent(gmGo.transform);
            _economyManager = emGo.AddComponent<EconomyManager>();

            // --- DeckManager ---
            var dmGo = new GameObject("DeckManager");
            dmGo.transform.SetParent(gmGo.transform);
            _deckManager = dmGo.AddComponent<DeckManager>();

            // --- BoardManager ---
            var bmGo = new GameObject("BoardManager");
            bmGo.transform.SetParent(gmGo.transform);
            _boardManager = bmGo.AddComponent<BoardManager>();

            // --- ComboSystem ---
            var csGo = new GameObject("ComboSystem");
            csGo.transform.SetParent(gmGo.transform);
            _comboSystem = csGo.AddComponent<ComboSystem>();

            // --- TerritoryManager ---
            var terGo = new GameObject("TerritoryManager");
            terGo.transform.SetParent(gmGo.transform);
            _territoryManager = terGo.AddComponent<TerritoryManager>();

            // --- FBISystem ---
            var fbiGo = new GameObject("FBISystem");
            fbiGo.transform.SetParent(gmGo.transform);
            _fbiSystem = fbiGo.AddComponent<FBISystem>();

            // --- RivalAI ---
            var aiGo = new GameObject("RivalAI");
            aiGo.transform.SetParent(gmGo.transform);
            _rivalAI = aiGo.AddComponent<RivalAI>();

            // --- ShopManager ---
            var shopGo = new GameObject("ShopManager");
            shopGo.transform.SetParent(gmGo.transform);
            _shopManager = shopGo.AddComponent<ShopManager>();

            // --- AudioManager ---
            var audioGo = new GameObject("[AudioManager]");
            _audioManager = audioGo.AddComponent<AudioManager>();
            // Add audio sources (need two music sources for crossfade + one SFX)
            _musicSourceA = audioGo.AddComponent<AudioSource>();
            _musicSourceA.loop = true;
            _musicSourceA.playOnAwake = false;
            _musicSourceB = audioGo.AddComponent<AudioSource>();
            _musicSourceB.loop = true;
            _musicSourceB.playOnAwake = false;
            _sfxSource = audioGo.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;

            // --- VFXManager ---
            var vfxGo = new GameObject("VFXManager");
            _vfxManager = vfxGo.AddComponent<VFXManager>();

            // --- SaveManager ---
            var saveGo = new GameObject("[SaveManager]");
            _saveManager = saveGo.AddComponent<SaveManager>();
        }

        // ================================================================
        // CAMERA (GDD 12.2 - Kamera section)
        // ================================================================

        private void SetupCamera()
        {
            var cam = Camera.main;
            if (cam != null)
            {
                // Set to orthographic for 2D card game
                cam.orthographic = true;
                cam.orthographicSize = 6f;
                cam.transform.position = new Vector3(0, 0, -10);
                cam.backgroundColor = new Color(0.15f, 0.12f, 0.1f); // Warm dark wood color

                // Add screen shake component
                if (cam.GetComponent<ScreenShake>() == null)
                    cam.gameObject.AddComponent<ScreenShake>();

                if (cam.GetComponent<CameraController>() == null)
                    cam.gameObject.AddComponent<CameraController>();
            }
        }

        // ================================================================
        // BOARD (GDD 12.2 - Masa section)
        // ================================================================

        private void CreateBoard()
        {
            var boardRoot = new GameObject("--- BOARD ---");

            // --- Background ---
            var bg = new GameObject("Background");
            bg.transform.SetParent(boardRoot.transform);
            var bgRenderer = bg.AddComponent<SpriteRenderer>();
            bgRenderer.color = new Color(0.35f, 0.25f, 0.15f); // Wood color placeholder
            bgRenderer.sortingOrder = -10;

            // --- Territory Map (10 bolge) ---
            var territoryMap = new GameObject("TerritoryMap");
            territoryMap.transform.SetParent(boardRoot.transform);
            territoryMap.transform.localPosition = new Vector3(0, 2.5f, 0);

            for (int i = 0; i < 10; i++)
            {
                var slot = new GameObject($"Territory_{i + 1:D2}");
                slot.transform.SetParent(territoryMap.transform);
                float x = (i - 4.5f) * 0.9f;
                slot.transform.localPosition = new Vector3(x, 0, 0);

                var sr = slot.AddComponent<SpriteRenderer>();
                sr.color = Color.gray; // Empty territory
                sr.sortingOrder = 1;
            }

            // --- Event Area ---
            var eventArea = new GameObject("EventArea");
            eventArea.transform.SetParent(boardRoot.transform);
            eventArea.transform.localPosition = new Vector3(0, 1.5f, 0);

            // --- Player Area (3-5 Business Slots) ---
            var playerArea = new GameObject("PlayerArea");
            playerArea.transform.SetParent(boardRoot.transform);
            playerArea.transform.localPosition = new Vector3(0, -0.5f, 0);

            for (int i = 0; i < 5; i++)
            {
                var bizSlot = new GameObject($"BusinessSlot_{i + 1:D2}");
                bizSlot.transform.SetParent(playerArea.transform);
                float x = (i - 2f) * 2.5f;
                bizSlot.transform.localPosition = new Vector3(x, 0, 0);

                var bizDZ = bizSlot.AddComponent<DropZone>();
                RuntimeWiring.SetField(bizDZ, "zoneType", DropZoneType.BusinessSlot);
                RuntimeWiring.SetField(bizDZ, "slotIndex", i);

                // Employee sub-slots (max 3 per business)
                for (int e = 0; e < 3; e++)
                {
                    var empSlot = new GameObject($"EmpSlot_{e + 1:D2}");
                    empSlot.transform.SetParent(bizSlot.transform);
                    empSlot.transform.localPosition = new Vector3(0, -(e + 1) * 0.6f, 0);

                    var empDZ = empSlot.AddComponent<DropZone>();
                    RuntimeWiring.SetField(empDZ, "zoneType", DropZoneType.EmployeeSlot);
                    RuntimeWiring.SetField(empDZ, "slotIndex", i); // employee belongs to business at index i
                }

                // Hide slots 4 and 5 initially
                if (i >= 3)
                    bizSlot.SetActive(false);
            }

            // --- Rival Area ---
            var rivalArea = new GameObject("RivalArea");
            rivalArea.transform.SetParent(boardRoot.transform);
            rivalArea.transform.localPosition = new Vector3(0, 3.5f, 0);

            for (int i = 0; i < 3; i++)
            {
                var rivalSlot = new GameObject($"RivalSlot_{i + 1:D2}");
                rivalSlot.transform.SetParent(rivalArea.transform);
                float x = (i - 1f) * 2.5f;
                rivalSlot.transform.localPosition = new Vector3(x, 0, 0);
            }

            // --- Upgrade Area ---
            var upgradeArea = new GameObject("UpgradeArea");
            upgradeArea.transform.SetParent(boardRoot.transform);
            upgradeArea.transform.localPosition = new Vector3(-5f, 0, 0);

            // --- Discard / Sell Zone ---
            var discardPile = new GameObject("DiscardPile");
            discardPile.transform.SetParent(boardRoot.transform);
            discardPile.transform.localPosition = new Vector3(5.5f, -3f, 0);
            var discardDZ = discardPile.AddComponent<DropZone>();
            RuntimeWiring.SetField(discardDZ, "zoneType", DropZoneType.SellZone);
            RuntimeWiring.SetField(discardDZ, "slotIndex", 0);
        }

        // ================================================================
        // UI (GDD 12.2 - UI Canvas section)
        // ================================================================

        private void CreateUI()
        {
            // --- Main Canvas (Screen Space Overlay) ---
            var canvasGo = new GameObject("GameCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // --- UIManager on canvas ---
            _uiManager = canvasGo.AddComponent<UIManager>();

            // --- TopBar ---
            var topBar = CreateUIPanel("TopBar", canvasGo.transform);
            SetAnchors(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            topBar.sizeDelta = new Vector2(0, 80);
            _topBarUI = topBar.gameObject.AddComponent<TopBarUI>();

            // Money display
            var moneyObj = CreateTextElement("MoneyDisplay", topBar, "$500", 36);
            moneyObj.localPosition = new Vector3(-700, -40, 0);
            _moneyText = moneyObj.GetComponent<TMP_Text>();

            // Turn counter
            var turnObj = CreateTextElement("TurnCounter", topBar, "Tur 1/20", 24);
            turnObj.localPosition = new Vector3(0, -40, 0);
            _turnText = turnObj.GetComponent<TMP_Text>();

            // FBI Risk bar
            var fbiBarBg = CreateUIPanel("FBIRiskBar", topBar);
            fbiBarBg.localPosition = new Vector3(700, -40, 0);
            fbiBarBg.sizeDelta = new Vector2(200, 20);
            var fbiBarBgImg = fbiBarBg.GetComponent<Image>();
            fbiBarBgImg.color = new Color(0.2f, 0.2f, 0.2f);

            var fbiBarFill = CreateUIPanel("Fill", fbiBarBg);
            fbiBarFill.sizeDelta = new Vector2(200, 20);
            _fbiBarFillImg = fbiBarFill.GetComponent<Image>();
            _fbiBarFillImg.color = Color.green;

            // --- ActionBar ---
            var actionBar = CreateUIPanel("ActionBar", canvasGo.transform);
            SetAnchors(actionBar, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            actionBar.anchoredPosition = new Vector2(0, 200);
            actionBar.sizeDelta = new Vector2(200, 40);
            _actionBarUI = actionBar.gameObject.AddComponent<ActionBarUI>();

            // Action dots (3-5)
            _actionDotImages = new Image[5];
            for (int i = 0; i < 5; i++)
            {
                var dot = CreateUIPanel($"ActionDot_{i + 1}", actionBar);
                dot.sizeDelta = new Vector2(30, 30);
                dot.localPosition = new Vector3((i - 2) * 35f, 0, 0);
                _actionDotImages[i] = dot.GetComponent<Image>();
                _actionDotImages[i].color = i < 3 ? Color.green : Color.gray;
                if (i >= 3) dot.gameObject.SetActive(false); // Hidden until upgrade
            }

            // --- Hand Area (bottom of screen) ---
            var handArea = CreateUIPanel("HandArea", canvasGo.transform);
            SetAnchors(handArea, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
            handArea.anchoredPosition = new Vector2(0, 80);
            handArea.sizeDelta = new Vector2(0, 160);
            _handUI = handArea.gameObject.AddComponent<HandUI>();

            // --- Buttons ---
            // End Turn Button
            var endTurnBtn = CreateButton("EndTurnButton", canvasGo.transform, "TUR BITIR");
            SetAnchors(endTurnBtn, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
            endTurnBtn.anchoredPosition = new Vector2(-100, 200);
            endTurnBtn.sizeDelta = new Vector2(160, 50);
            _endTurnButton = endTurnBtn.GetComponent<Button>();

            // Shop Button
            var shopBtn = CreateButton("ShopButton", canvasGo.transform, "DUKKAN");
            SetAnchors(shopBtn, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            shopBtn.anchoredPosition = new Vector2(100, 200);
            shopBtn.sizeDelta = new Vector2(140, 50);
            _shopButton = shopBtn.GetComponent<Button>();

            // Deck Button
            var deckBtn = CreateButton("DeckButton", canvasGo.transform, "DESTE: 14");
            SetAnchors(deckBtn, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            deckBtn.anchoredPosition = new Vector2(100, 140);
            deckBtn.sizeDelta = new Vector2(140, 40);

            // --- Shop Panel (hidden by default) ---
            var shopPanel = CreateUIPanel("ShopPanel", canvasGo.transform);
            shopPanel.anchorMin = new Vector2(0.2f, 0.2f);
            shopPanel.anchorMax = new Vector2(0.8f, 0.8f);
            shopPanel.sizeDelta = Vector2.zero;
            shopPanel.offsetMin = Vector2.zero;
            shopPanel.offsetMax = Vector2.zero;
            shopPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            _shopPanel = shopPanel.gameObject.AddComponent<ShopPanel>();
            shopPanel.gameObject.SetActive(false);

            // Shop title
            CreateTextElement("ShopTitle", shopPanel, "DUKKAN", 32);

            // Shop card slots
            for (int i = 0; i < 3; i++)
            {
                var shopCard = CreateUIPanel($"ShopCard_{i + 1}", shopPanel);
                shopCard.sizeDelta = new Vector2(150, 220);
                shopCard.localPosition = new Vector3((i - 1) * 200f, 0, 0);
                shopCard.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.35f);
            }

            var shopCloseBtn = CreateButton("CloseButton", shopPanel, "KAPAT");
            shopCloseBtn.anchoredPosition = new Vector2(0, -200);
            shopCloseBtn.sizeDelta = new Vector2(120, 40);
            _shopCloseButton = shopCloseBtn.GetComponent<Button>();

            // --- Popup overlays (hidden by default) ---
            var comboPopup = CreateUIPanel("ComboPopup", canvasGo.transform);
            comboPopup.sizeDelta = new Vector2(600, 100);
            _comboPopup = comboPopup.gameObject.AddComponent<ComboPopup>();
            comboPopup.gameObject.SetActive(false);

            var eventPopup = CreateUIPanel("EventPopup", canvasGo.transform);
            eventPopup.sizeDelta = new Vector2(400, 300);
            _eventPopup = eventPopup.gameObject.AddComponent<EventPopup>();
            eventPopup.gameObject.SetActive(false);

            var rivalPopup = CreateUIPanel("RivalPopup", canvasGo.transform);
            SetAnchors(rivalPopup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            rivalPopup.sizeDelta = new Vector2(500, 200);
            _rivalPopup = rivalPopup.gameObject.AddComponent<RivalPopup>();
            rivalPopup.gameObject.SetActive(false);

            // --- Score Overlay (hidden by default) ---
            var scoreOverlay = CreateUIPanel("ScoreOverlay", canvasGo.transform);
            scoreOverlay.anchorMin = Vector2.zero;
            scoreOverlay.anchorMax = Vector2.one;
            scoreOverlay.sizeDelta = Vector2.zero;
            scoreOverlay.offsetMin = Vector2.zero;
            scoreOverlay.offsetMax = Vector2.zero;
            scoreOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);
            _scoreScreen = scoreOverlay.gameObject.AddComponent<ScoreScreen>();
            scoreOverlay.gameObject.SetActive(false);

            // --- GameOver Overlay (hidden by default) ---
            var gameOverOverlay = CreateUIPanel("GameOverOverlay", canvasGo.transform);
            gameOverOverlay.anchorMin = Vector2.zero;
            gameOverOverlay.anchorMax = Vector2.one;
            gameOverOverlay.sizeDelta = Vector2.zero;
            gameOverOverlay.offsetMin = Vector2.zero;
            gameOverOverlay.offsetMax = Vector2.zero;
            gameOverOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);
            _gameOverScreen = gameOverOverlay.gameObject.AddComponent<GameOverScreen>();
            gameOverOverlay.gameObject.SetActive(false);

            // --- EventSystem (required for UI interaction) ---
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        // ================================================================
        // VFX (GDD 12.2 - Efektler section)
        // ================================================================

        private void CreateVFX()
        {
            var vfxRoot = new GameObject("--- VFX ---");
            var poolParent = new GameObject("VFXPool");
            poolParent.transform.SetParent(vfxRoot.transform);
        }

        // ================================================================
        // CARD PREFAB (runtime template for HandUI instantiation)
        // ================================================================

        private void CreateCardPrefab()
        {
            // --- Root: CardPrefab (RectTransform 150x220 + CanvasGroup) ---
            var root = new GameObject("CardPrefab");
            var rootRT = root.AddComponent<RectTransform>();
            rootRT.sizeDelta = new Vector2(150, 220);
            var rootCG = root.AddComponent<CanvasGroup>();

            // --- Background child: fills parent ---
            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(root.transform, false);
            var bgRT = bgGo.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = new Color(0.25f, 0.25f, 0.3f);

            // --- Icon child: 80x80, centered upper ---
            var iconGo = new GameObject("Icon");
            iconGo.transform.SetParent(root.transform, false);
            var iconRT = iconGo.AddComponent<RectTransform>();
            iconRT.anchorMin = new Vector2(0.5f, 0.5f);
            iconRT.anchorMax = new Vector2(0.5f, 0.5f);
            iconRT.pivot = new Vector2(0.5f, 0.5f);
            iconRT.sizeDelta = new Vector2(80, 80);
            iconRT.anchoredPosition = new Vector2(0, 30); // upper-center
            var iconImg = iconGo.AddComponent<Image>();
            iconImg.color = Color.white;

            // --- HighlightBorder child: fills parent, outline-style, initially transparent ---
            var hlGo = new GameObject("HighlightBorder");
            hlGo.transform.SetParent(root.transform, false);
            var hlRT = hlGo.AddComponent<RectTransform>();
            hlRT.anchorMin = Vector2.zero;
            hlRT.anchorMax = Vector2.one;
            hlRT.sizeDelta = Vector2.zero;
            hlRT.offsetMin = Vector2.zero;
            hlRT.offsetMax = Vector2.zero;
            var hlImg = hlGo.AddComponent<Image>();
            hlImg.color = new Color(0, 0, 0, 0); // transparent initially
            hlImg.raycastTarget = false;

            // --- NameText child: top area, 16pt ---
            var nameGo = new GameObject("NameText");
            nameGo.transform.SetParent(root.transform, false);
            var nameRT = nameGo.AddComponent<RectTransform>();
            nameRT.anchorMin = new Vector2(0, 1);
            nameRT.anchorMax = new Vector2(0.75f, 1);
            nameRT.pivot = new Vector2(0, 1);
            nameRT.sizeDelta = new Vector2(0, 30);
            nameRT.anchoredPosition = new Vector2(5, -5);
            var nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
            nameTmp.text = "";
            nameTmp.fontSize = 16;
            nameTmp.alignment = TextAlignmentOptions.TopLeft;
            nameTmp.color = Color.white;
            nameTmp.raycastTarget = false;

            // --- CostText child: top-right corner, 20pt, bold ---
            var costGo = new GameObject("CostText");
            costGo.transform.SetParent(root.transform, false);
            var costRT = costGo.AddComponent<RectTransform>();
            costRT.anchorMin = new Vector2(1, 1);
            costRT.anchorMax = new Vector2(1, 1);
            costRT.pivot = new Vector2(1, 1);
            costRT.sizeDelta = new Vector2(50, 30);
            costRT.anchoredPosition = new Vector2(-5, -5);
            var costTmp = costGo.AddComponent<TextMeshProUGUI>();
            costTmp.text = "";
            costTmp.fontSize = 20;
            costTmp.fontStyle = FontStyles.Bold;
            costTmp.alignment = TextAlignmentOptions.TopRight;
            costTmp.color = Color.yellow;
            costTmp.raycastTarget = false;

            // --- DescriptionText child: middle area, 12pt ---
            var descGo = new GameObject("DescriptionText");
            descGo.transform.SetParent(root.transform, false);
            var descRT = descGo.AddComponent<RectTransform>();
            descRT.anchorMin = new Vector2(0, 0.25f);
            descRT.anchorMax = new Vector2(1, 0.55f);
            descRT.sizeDelta = Vector2.zero;
            descRT.offsetMin = new Vector2(8, 0);
            descRT.offsetMax = new Vector2(-8, 0);
            var descTmp = descGo.AddComponent<TextMeshProUGUI>();
            descTmp.text = "";
            descTmp.fontSize = 12;
            descTmp.alignment = TextAlignmentOptions.TopLeft;
            descTmp.color = Color.white;
            descTmp.enableWordWrapping = true;
            descTmp.raycastTarget = false;

            // --- StatsText child: bottom area, 11pt ---
            var statsGo = new GameObject("StatsText");
            statsGo.transform.SetParent(root.transform, false);
            var statsRT = statsGo.AddComponent<RectTransform>();
            statsRT.anchorMin = new Vector2(0, 0);
            statsRT.anchorMax = new Vector2(1, 0.25f);
            statsRT.sizeDelta = Vector2.zero;
            statsRT.offsetMin = new Vector2(8, 5);
            statsRT.offsetMax = new Vector2(-8, 0);
            var statsTmp = statsGo.AddComponent<TextMeshProUGUI>();
            statsTmp.text = "";
            statsTmp.fontSize = 11;
            statsTmp.alignment = TextAlignmentOptions.BottomLeft;
            statsTmp.color = new Color(0.8f, 0.8f, 0.8f);
            statsTmp.enableWordWrapping = true;
            statsTmp.raycastTarget = false;

            // --- Add CardUI + CardDragHandler components ---
            var cardUI = root.AddComponent<CardUI>();
            root.AddComponent<CardDragHandler>();

            // --- Wire all child references into CardUI via RuntimeWiring ---
            RuntimeWiring.SetField(cardUI, "cardBackground", bgImg);
            RuntimeWiring.SetField(cardUI, "cardIcon", iconImg);
            RuntimeWiring.SetField(cardUI, "highlightBorder", hlImg);
            RuntimeWiring.SetField(cardUI, "nameText", (TMP_Text)nameTmp);
            RuntimeWiring.SetField(cardUI, "costText", (TMP_Text)costTmp);
            RuntimeWiring.SetField(cardUI, "descriptionText", (TMP_Text)descTmp);
            RuntimeWiring.SetField(cardUI, "statsText", (TMP_Text)statsTmp);
            RuntimeWiring.SetField(cardUI, "canvasGroup", rootCG);
            // tooltipPanel, tooltipText, synergyGlow left null initially

            // --- Deactivate: this is a template/prefab ---
            root.SetActive(false);

            _cardPrefabInstance = root;

            Debug.Log("[GameSceneBootstrap] Card prefab template created at runtime.");
        }

        // ================================================================
        // WIRE REFERENCES (Connect all managers via RuntimeWiring)
        // ================================================================

        private void WireManagerReferences()
        {
            // === GameManager: data + all manager references ===
            RuntimeWiring.SetField(_gameManager, "balanceData", _balanceData);
            RuntimeWiring.SetField(_gameManager, "startingDeck", _startingDeck);
            RuntimeWiring.SetField(_gameManager, "turnManager", _turnManager);
            RuntimeWiring.SetField(_gameManager, "economyManager", _economyManager);
            RuntimeWiring.SetField(_gameManager, "deckManager", _deckManager);
            RuntimeWiring.SetField(_gameManager, "boardManager", _boardManager);
            RuntimeWiring.SetField(_gameManager, "comboSystem", _comboSystem);
            RuntimeWiring.SetField(_gameManager, "territoryManager", _territoryManager);
            RuntimeWiring.SetField(_gameManager, "fbiSystem", _fbiSystem);
            RuntimeWiring.SetField(_gameManager, "rivalAI", _rivalAI);
            RuntimeWiring.SetField(_gameManager, "shopManager", _shopManager);
            RuntimeWiring.SetField(_gameManager, "uiManager", _uiManager);
            RuntimeWiring.SetField(_gameManager, "audioManager", _audioManager);
            RuntimeWiring.SetField(_gameManager, "vfxManager", _vfxManager);
            RuntimeWiring.SetField(_gameManager, "saveManager", _saveManager);

            // === EconomyManager: balanceData, boardManager, comboSystem ===
            RuntimeWiring.SetField(_economyManager, "balanceData", _balanceData);
            RuntimeWiring.SetField(_economyManager, "boardManager", _boardManager);
            RuntimeWiring.SetField(_economyManager, "comboSystem", _comboSystem);

            // === ComboSystem: allCombos, boardManager ===
            RuntimeWiring.SetField(_comboSystem, "allCombos", _allCombos);
            RuntimeWiring.SetField(_comboSystem, "boardManager", _boardManager);

            // === FBISystem: balanceData, boardManager, comboSystem ===
            RuntimeWiring.SetField(_fbiSystem, "balanceData", _balanceData);
            RuntimeWiring.SetField(_fbiSystem, "boardManager", _boardManager);
            RuntimeWiring.SetField(_fbiSystem, "comboSystem", _comboSystem);

            // === RivalAI: data (RivalData) ===
            RuntimeWiring.SetField(_rivalAI, "data", _megaCorpData);

            // === ShopManager: shopPool, deckManager, economyManager, comboSystem ===
            RuntimeWiring.SetField(_shopManager, "shopPool", _shopPool);
            RuntimeWiring.SetField(_shopManager, "deckManager", _deckManager);
            RuntimeWiring.SetField(_shopManager, "economyManager", _economyManager);
            RuntimeWiring.SetField(_shopManager, "comboSystem", _comboSystem);

            // === AudioManager: musicSourceA, musicSourceB, sfxSource ===
            RuntimeWiring.SetField(_audioManager, "musicSourceA", _musicSourceA);
            RuntimeWiring.SetField(_audioManager, "musicSourceB", _musicSourceB);
            RuntimeWiring.SetField(_audioManager, "sfxSource", _sfxSource);

            // === UIManager: all panel references ===
            RuntimeWiring.SetField(_uiManager, "topBar", _topBarUI);
            RuntimeWiring.SetField(_uiManager, "actionBar", _actionBarUI);
            RuntimeWiring.SetField(_uiManager, "shopPanel", _shopPanel);
            RuntimeWiring.SetField(_uiManager, "handUI", _handUI);
            RuntimeWiring.SetField(_uiManager, "comboPopup", _comboPopup);
            RuntimeWiring.SetField(_uiManager, "eventPopup", _eventPopup);
            RuntimeWiring.SetField(_uiManager, "rivalPopup", _rivalPopup);
            RuntimeWiring.SetField(_uiManager, "scoreScreen", _scoreScreen);
            RuntimeWiring.SetField(_uiManager, "gameOverScreen", _gameOverScreen);

            // === TopBarUI: TMP_Text and Image sub-elements ===
            RuntimeWiring.SetField(_topBarUI, "moneyText", _moneyText);
            RuntimeWiring.SetField(_topBarUI, "turnText", _turnText);
            RuntimeWiring.SetField(_topBarUI, "fbiBarFill", _fbiBarFillImg);

            // === ActionBarUI: action dot Image[] ===
            RuntimeWiring.SetField(_actionBarUI, "actionDots", _actionDotImages);

            // === HandUI: handRoot + cardPrefab ===
            RuntimeWiring.SetField(_handUI, "handRoot", _handUI.transform);
            RuntimeWiring.SetField(_handUI, "cardPrefab", _cardPrefabInstance.GetComponent<CardUI>());

            // === ShopPanel: shopManager reference ===
            RuntimeWiring.SetField(_shopPanel, "shopManager", _shopManager);

            // === DropZones: wire boardManager to all instances ===
            var allDropZones = Object.FindObjectsByType<DropZone>(FindObjectsSortMode.None);
            foreach (var dz in allDropZones)
            {
                RuntimeWiring.SetField(dz, "boardManager", _boardManager);
            }

            Debug.Log("[GameSceneBootstrap] All manager and UI references wired via RuntimeWiring.");
        }

        // ================================================================
        // BUTTON CALLBACKS (Wire onClick after managers and UI exist)
        // ================================================================

        private void WireButtonCallbacks()
        {
            // End Turn button -> TurnManager.EndPlayPhase()
            if (_endTurnButton != null && _turnManager != null)
                _endTurnButton.onClick.AddListener(() => _turnManager.EndPlayPhase());

            // Shop button -> toggle shop
            if (_shopButton != null && _uiManager != null)
            {
                bool shopOpen = false;
                _shopButton.onClick.AddListener(() => {
                    shopOpen = !shopOpen;
                    if (shopOpen) _uiManager.ShowShop();
                    else _uiManager.HideShop();
                });
            }

            // Shop close -> hide shop
            if (_shopCloseButton != null && _uiManager != null)
                _shopCloseButton.onClick.AddListener(() => _uiManager.HideShop());

            // UIManager end turn event -> TurnManager
            if (_uiManager != null && _turnManager != null)
                _uiManager.OnEndTurnClicked += () => _turnManager.EndPlayPhase();

            Debug.Log("[GameSceneBootstrap] Button callbacks wired.");
        }

        // ================================================================
        // UI Helper Methods
        // ================================================================

        private RectTransform CreateUIPanel(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0); // Transparent by default
            return rt;
        }

        private RectTransform CreateButton(string name, Transform parent, string label)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 0.3f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var textObj = new GameObject("Label");
            textObj.transform.SetParent(go.transform, false);
            var textRT = textObj.AddComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
            var text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 20;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            return rt;
        }

        private RectTransform CreateTextElement(string name, Transform parent, string content, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 50);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            return rt;
        }

        private void SetAnchors(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
        }
    }
}
