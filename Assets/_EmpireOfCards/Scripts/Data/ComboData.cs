using UnityEngine;

namespace EmpireOfCards.Data
{
    using EmpireOfCards.Core;

    [CreateAssetMenu(fileName = "NewCombo", menuName = "EmpireOfCards/Combo Data")]
    public class ComboData : ScriptableObject
    {
        [Header("Combo Info")]
        [Tooltip("Unique combo identifier, e.g. COMBO_01")]
        public string comboId;

        [Tooltip("Display name, e.g. Latte Sanati")]
        public string comboName;

        [Tooltip("In-game display text, e.g. LATTE SANATI!")]
        public string displayText;

        public ComboTier tier;

        [TextArea(2, 4)]
        public string description;

        [Header("Requirements")]
        [Tooltip("Specific card IDs needed to trigger this combo")]
        public string[] requiredCardIds;

        [Tooltip("Tag requirements for combo activation")]
        public CardTag[] requiredTags;

        public bool requiresActiveEvent;

        [Tooltip("Specific event ID that must be active")]
        public string requiredEventId;

        public bool requiresMinMoney;
        public int minMoneyRequired;

        public bool requiresMinTerritory;
        public int minTerritoryRequired;

        [Tooltip("Minimum number of active businesses required")]
        public int minActiveBusinesses;

        [Header("Bonus")]
        public int bonusIncome;
        public int bonusCustomers;

        [Tooltip("Income multiplier (1.0 = no change)")]
        public float incomeMultiplier = 1.0f;

        [Tooltip("Customer multiplier (1.0 = no change)")]
        public float customerMultiplier = 1.0f;

        public int extraActions;

        [Tooltip("Rival customer penalty (e.g. Monopol: -3)")]
        public int rivalCustomerPenalty;

        [Tooltip("Shop discount (e.g. Kriz Avcisi: 0.5)")]
        public float shopDiscount;

        [Tooltip("Whether this combo transfers a rival employee")]
        public bool transferRivalEmployee;

        [Tooltip("Extra FBI risk from this combo")]
        public int fbiRiskBonus;

        [Header("Visual Feedback")]
        public Color glowColor = Color.white;

        [Tooltip("Audio clip reference ID")]
        public string comboSoundId;

        public float screenShakeIntensity;
    }
}
