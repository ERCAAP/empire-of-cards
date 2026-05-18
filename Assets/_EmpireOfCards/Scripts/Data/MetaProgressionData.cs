using System;
using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "MetaProgression", menuName = "EmpireOfCards/Meta Progression")]
    public class MetaProgressionData : ScriptableObject
    {
        public UnlockTier[] unlockTiers;
        public AscensionLevel[] ascensionLevels;
    }

    [Serializable]
    public class UnlockTier
    {
        [Tooltip("XP needed to reach this unlock tier")]
        public int xpRequired;

        [TextArea(1, 3)]
        public string unlockDescription;

        [Tooltip("Cards unlocked at this tier")]
        public CardData[] unlockedCards;

        [Tooltip("Rival unlocked at this tier (optional)")]
        public RivalData unlockedRival;
    }

    [Serializable]
    public class AscensionLevel
    {
        public int level;

        [TextArea(1, 3)]
        public string description;

        [Tooltip("Multiplier applied to rival aggression at this ascension level")]
        public float rivalAggressionMultiplier;

        [Tooltip("Modifier to starting money (can be negative)")]
        public int startingMoneyModifier;

        [Tooltip("Multiplier applied to crisis event frequency")]
        public float crisisFrequencyMultiplier;
    }
}
