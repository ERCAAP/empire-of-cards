using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "EmpireOfCards/CardData")]
    public class CardData : ScriptableObject
    {
        [Header("Identity")]
        public string cardId;
        public string cardName;
        [TextArea(2, 4)]
        public string description;
        public CardType cardType;
        public CardFamily cardFamily;
        public Rarity rarity;
        public SectorType sector;
        public bool isNeutral;

        [Header("Cost")]
        public int buyCost;
        public int upkeepPerTurn;

        [Header("Target")]
        public SlotType targetSlot;
        public string targetSubSlot;

        [Header("Stat Effects (per turn while on board)")]
        public float demandDelta;
        public float capacityDelta;
        public float qualityDelta;
        public float ratingDelta;
        public float hygieneDelta;
        public float staffStabilityDelta;
        public float legalRiskDelta;
        public float cashPerTurn;

        [Header("Staff Properties (only for Staff cards)")]
        public StaffTier startingTier;
        public float moraleEffect;
        public bool canPromote;

        [Header("Risk Properties (only for Risk cards)")]
        public float shortTermBenefit;
        public float longTermRisk;
        public CrisisType triggersCrisis;

        [Header("Crisis Properties (only for Crisis cards)")]
        public CrisisType crisisType;
        public int crisisDuration;
        public string[] solutionTags;

        [Header("Temp Effect")]
        public int duration;
        public bool isSeasonal;

        [Header("Tags")]
        public string[] tags;
    }
}
