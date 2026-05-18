using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "MetaProgression", menuName = "EmpireOfCards/Meta Progression")]
    public class MetaProgressionData : ScriptableObject
    {
        public UnlockTier[] unlockTiers;
        public AscensionLevel[] ascensionLevels;
    }

    [System.Serializable]
    public class UnlockTier
    {
        public int xpRequired;          // 50, 200, 500, 1000, 2000, 5000
        public string unlockDescription;// "Uncommon kartlar dükkan havuzuna girer"
        public CardData[] unlockedCards;
        public RivalData unlockedRival; // 500 XP → Shadow Inc., 2000 XP → The Cartel
    }

    [System.Serializable]
    public class AscensionLevel
    {
        public int level;               // 1, 2, 3
        public string description;      // "Rakip daha agresif"
        public float rivalAggressionMultiplier = 1f;  // 1.2, 1.5 etc
        public int startingMoneyModifier;             // -100 at Ascension 2
        public float crisisFrequencyMultiplier = 1f;  // Ascension 3: 1.5
    }
}
