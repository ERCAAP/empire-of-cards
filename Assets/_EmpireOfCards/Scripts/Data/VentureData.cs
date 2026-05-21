using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewVenture", menuName = "EmpireOfCards/Venture")]
    public class VentureData : ScriptableObject
    {
        public EmpireOfCards.Core.VentureType ventureType;
        public string ventureName;
        [TextArea] public string description;
        [TextArea] public string openingFantasy;
        [TextArea] public string openingPlanSummary;
        public bool requiresCustomName = true;
        public bool requiresRunCategorySelection;
        public CardData startingBusiness;        // null for KaranlikPazar
        public CardData bonusDeckCard;           // extra card added to deck
        public int bonusMoney;                   // extra starting money (0 for most, 200 for KaranlikPazar)
        public Sprite ventureIcon;
        public VentureBoardProfile boardProfile;
        public VentureDeckProfile deckProfile;
        public VentureEconomyProfile economyProfile;
        public VenturePlaybook playbook;
        public VentureProgressionArc progressionArc;
        public VentureBoardThemeProfile themeProfile;
        [TextArea] public string playstyleSummary;
        public string[] openingSequenceCardIds;
    }
}
