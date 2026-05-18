using UnityEngine;

namespace EmpireOfCards.Data
{
    using EmpireOfCards.Core;

    [CreateAssetMenu(fileName = "NewRival", menuName = "EmpireOfCards/Rival Data")]
    public class RivalData : ScriptableObject
    {
        [Tooltip("Unique rival identifier")]
        public string rivalId;

        [Tooltip("Display name, e.g. MegaCorp")]
        public string rivalName;

        public RivalPersonality personality;
        public Sprite portrait;

        [Header("Economy")]
        public int startingMoney;
        public int incomeGrowthPerTurn;
        public int startingCustomers;

        [Header("Behavior")]
        [Tooltip("Actions the rival takes per turn (2 for Normal)")]
        public int actionsPerTurn;

        [Tooltip("Player territory count above which the rival becomes aggressive")]
        public float aggressionThreshold;

        [Range(0f, 1f)]
        [Tooltip("Chance to expand each turn")]
        public float expansionChance;

        [Range(0f, 1f)]
        [Tooltip("Chance to sabotage the player each turn")]
        public float sabotageChance;

        [Header("Dialogue")]
        [TextArea(1, 2)]
        [Tooltip("Taunts when the rival is growing")]
        public string[] growingTaunts;

        [TextArea(1, 2)]
        [Tooltip("Taunts when the player is growing")]
        public string[] playerGrowingTaunts;

        [TextArea(1, 2)]
        [Tooltip("Taunts when the rival is in aggressive mode")]
        public string[] aggressiveTaunts;

        [TextArea(1, 2)]
        [Tooltip("Taunts when the rival is losing")]
        public string[] losingTaunts;
    }
}
