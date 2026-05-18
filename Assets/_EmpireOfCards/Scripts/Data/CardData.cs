using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "EmpireOfCards/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("--- Kimlik ---")]
        public string cardId;               // "B02_Kahveci", "C03_Barista" etc
        public string cardName;             // Display name: "Kahveci"
        public CardType cardType;
        public Rarity rarity;
        [TextArea(2, 4)]
        public string description;          // Turkish flavor text
        public Sprite icon;
        public Sprite cardFrame;
        public CardTag[] tags;              // food, coffee, trendy etc.

        [Header("--- Maliyet ---")]
        public int buyCost;                 // Dükkan/oynama maliyeti

        // ====== BUSINESS (Mavi) ======
        [Header("--- İşletme Özellikleri ---")]
        public int incomePerTurn;           // Base gelir/tur
        public int customersPerTurn;        // Base müşteri/tur
        public int employeeSlots;           // Çalışan yuvası sayısı (1-3)
        public bool hasTrendBonus;          // Kahveci: trend aktifken bonus
        [Range(1f, 3f)]
        public float trendIncomeMultiplier = 1f;  // Kahveci: 1.5
        public int activationDelay;         // Tech Startup: 3 tur delay
        public bool requiresTrendToOperate; // Gece Kulübü: trend yoksa 0 gelir
        public bool hasRandomIncome;        // Kripto Borsası
        public int randomIncomeMin;         // 0
        public int randomIncomeMax;         // 250
        public int[] randomIncomeThresholds;// [0, 0, 100, 100, 250, 250] (zar 1-6)
        public string foodBonusTag;         // Organik Çiftlik: "food" tag'li herkese bonus
        public int foodBonusAmount;         // +20 per food business
        public int globalCustomerBonus;     // Reklam Ajansı: tüm işletmelere +2

        // İşletme Evrimi (GDD Bölüm 3.1)
        public bool canEvolve;
        public CardData evolvedForm;        // Büfe → Restoran → Zincir
        public int evolutionCustomerReq;    // 40 toplam müşteri
        public int evolutionTurnReq;        // 15 tur

        // ====== EMPLOYEE (Yeşil) ======
        [Header("--- Çalışan Özellikleri ---")]
        public int salaryPerTurn;           // Maaş/tur
        public int customerBonus;           // Base müşteri katkısı
        public int synergyCustomerBonus;    // Doğru işletmede ekstra bonus (Barista coffee: +6 vs +3)
        public CardTag synergyTag;          // Hangi tag ile sinerji (Coffee for Barista)
        public float incomeMultiplier;      // Marketing Guru: 0.25 (+%25)
        public float incomeFlatBonus;       // Şef food'da: +30
        public CardTag incomeBonusTag;      // Hangi tag'de flat bonus aktif
        public int fbiRiskPerTurn;          // Hacker: 10, Dolandırıcı: 12
        public int illegalIncomePerTurn;    // Dolandırıcı: 120
        public bool preventsTransfer;       // Sadık Müdür: transfer koruması
        public float taxReduction;          // Muhasebeci: 0.5 (%50 azaltma)

        // Aktif Yetenek (GDD Bölüm 3.2)
        public ActiveAbilityType activeAbilityType;
        public string activeAbilityName;    // "Latte Festivali"
        [TextArea(1, 3)]
        public string activeAbilityDesc;    // Tooltip açıklaması
        public float abilityValue1;         // Primary param (multiplier or flat)
        public int abilityValue2;           // Secondary param (customers, money etc)

        // ====== ACTION (Kırmızı) ======
        [Header("--- Aksiyon Özellikleri ---")]
        public ActionEffectType actionEffectType;
        public int actionValue;             // Flat value (+150 money, +3 customers)
        public float actionMultiplier;      // Viral Pazarlama: 2.0 (x2)
        public int actionFBIRisk;           // Sahte Yorumlar: 12, Sabotaj: 15
        public int actionDebtDuration;      // Yatırımcı Sunumu: 3 tur
        public float actionDebtPercent;     // 0.15 (%15 gelir yatırımcıya)
        public float actionIncomeSacrifice; // Fiyat Kırma: 0.5 (%50 gelir feda)

        // ====== UPGRADE (Mor) ======
        [Header("--- Upgrade Özellikleri ---")]
        public UpgradeEffectType upgradeEffectType;
        public float upgradeValue;          // +%10, +%30, +2 müşteri etc
        public bool isGlobalUpgrade;        // Tüm masayı etkiler mi
        public int closedEmployeeSlots;     // Otomasyon: 1 slot kapanır
        public int extraActions;            // Yapay Zeka Asistanı: +1

        // ====== EVENT (Sarı) ======
        [Header("--- Event Özellikleri ---")]
        public EventEffectType eventEffectType;
        public int eventDuration;           // 1-2 tur
        public float eventMultiplier;       // +%50 = 0.5, -%30 = -0.3
        public CardTag[] affectedTags;      // Hangi tag'ler etkilenir
        public int eventCustomerPenalty;    // Veri Sızıntısı: -5
        public float eventFBIThreshold;     // İptal Kültürü: 0.30 (%30)
    }

    public enum ActiveAbilityType
    {
        None,
        MultiplyCustomers,
        AddCustomers,
        MultiplyIncome,
        StealCustomers,
        AddCustomersAll,
        NullifyTax,
        BonusIncomeWithPenalty,
        AddCustomersToAll
    }

    public enum ActionEffectType
    {
        None,
        AddCustomers,
        AddMoney,
        MultiplyAllCustomers,
        CloseRivalBusiness,
        AddCustomersWithFBI,
        StealCustomers,
        DisableRival,
        MoneyWithDebt,
        DrawAndPlayEmployee,
        SacrificeForMoney
    }

    public enum UpgradeEffectType
    {
        None,
        IncomePercent,
        IncomePercentWithSlotLoss,
        GlobalCustomerBonus,
        GlobalCustomerFlat,
        ReduceFBIRisk,
        ExtraAction
    }

    public enum EventEffectType
    {
        None,
        TagCustomerBoost,
        AllIncomeReduction,
        TagDoubleEffect,
        TagCustomerPenalty,
        TagDoubleEffectFinance,
        HighFBIPenalty
    }
}
