using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewCombo", menuName = "EmpireOfCards/Combo Data")]
    public class ComboData : ScriptableObject
    {
        [Header("--- Identity ---")]
        public string comboId;              // "COMBO_01_LatteArt"
        public string comboName;            // "Latte Art"
        public string displayText;          // "LATTE ART!" (shown on screen)
        public ComboTier tier;
        [TextArea(2, 4)]
        public string description;

        [Header("--- Requirements ---")]
        public string[] requiredCardIds;    // Specific card IDs (internal): ["B02_Kahveci", "C03_Barista"]
        public CardTag[] requiredTags;      // Tag requirements
        public bool requiresSpecificPlacement;  // Must Barista be in Coffee Shop
        public string employeeCardId;       // Which employee
        public string businessCardId;       // Which business

        [Header("--- Event Dependency ---")]
        public bool requiresActiveEvent;
        public string requiredEventId;      // "E03_ViralTrend"

        [Header("--- State Requirements ---")]
        public bool requiresMinMoney;
        public int minMoneyRequired;        // Crisis Hunter: 1000
        public bool requiresMinTerritory;
        public int minTerritoryRequired;    // Monopoly: territory >= some number
        public int minActiveBusinesses;     // Monopoly: 4 businesses
        public float minMarketShare;        // Monopoly: 55%

        [Header("--- Bonus ---")]
        public int bonusIncome;             // Extra income/turn
        public int bonusCustomers;          // Extra customers/turn
        public float incomeMultiplier;      // 1.0 = normal, 2.0 = x2
        public float customerMultiplier;    // Customer multiplier
        public int extraActions;            // AI Revolution: +1
        public int rivalCustomerPenalty;    // Monopoly: rival -3/turn
        public float shopDiscount;          // Crisis Hunter: 0.5 (50%)
        public bool transferRivalEmployee;  // Crisis Hunter: 1 employee transfer
        public int extraFBIRisk;            // Underground: +8

        [Header("--- Visual Feedback ---")]
        public Color glowColor = Color.yellow;
        public string comboSoundId;         // "combo_trigger" audio reference
        [Range(0f, 1f)]
        public float screenShakeIntensity = 0.3f;
        public float screenShakeDuration = 0.3f;
    }
}
