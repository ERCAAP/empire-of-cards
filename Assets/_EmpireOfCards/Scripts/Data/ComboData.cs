using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewCombo", menuName = "EmpireOfCards/Combo Data")]
    public class ComboData : ScriptableObject
    {
        [Header("--- Kimlik ---")]
        public string comboId;              // "COMBO_01_LatteSanati"
        public string comboName;            // "Latte Sanatı"
        public string displayText;          // "LATTE SANATI!" (ekranda gösterilecek)
        public ComboTier tier;
        [TextArea(2, 4)]
        public string description;

        [Header("--- Gereksinimler ---")]
        public string[] requiredCardIds;    // Spesifik kart ID'leri: ["B02_Kahveci", "C03_Barista"]
        public CardTag[] requiredTags;      // Tag gereksinimleri
        public bool requiresSpecificPlacement;  // Barista Kahveci'de mi olmalı
        public string employeeCardId;       // Hangi çalışan
        public string businessCardId;       // Hangi işletmede

        [Header("--- Event Bağımlılık ---")]
        public bool requiresActiveEvent;
        public string requiredEventId;      // "E03_ViralTrend"

        [Header("--- Durum Gereksinimleri ---")]
        public bool requiresMinMoney;
        public int minMoneyRequired;        // Kriz Avcısı: 1000
        public bool requiresMinTerritory;
        public int minTerritoryRequired;    // Monopol: bölge >= bazı sayı
        public int minActiveBusinesses;     // Monopol: 4 işletme
        public float minMarketShare;        // Monopol: %55

        [Header("--- Bonus ---")]
        public int bonusIncome;             // Ek gelir/tur
        public int bonusCustomers;          // Ek müşteri/tur
        public float incomeMultiplier;      // 1.0 = normal, 2.0 = x2
        public float customerMultiplier;    // Müşteri çarpanı
        public int extraActions;            // AI Devrimi: +1
        public int rivalCustomerPenalty;    // Monopol: rakip -3/tur
        public float shopDiscount;          // Kriz Avcısı: 0.5 (%50)
        public bool transferRivalEmployee;  // Kriz Avcısı: 1 çalışan transfer
        public int extraFBIRisk;            // Yeraltı: +8

        [Header("--- Görsel Feedback ---")]
        public Color glowColor = Color.yellow;
        public string comboSoundId;         // "combo_trigger" audio reference
        [Range(0f, 1f)]
        public float screenShakeIntensity = 0.3f;
        public float screenShakeDuration = 0.3f;
    }
}
