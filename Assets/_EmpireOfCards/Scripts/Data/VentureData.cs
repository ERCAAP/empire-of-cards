using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewVenture", menuName = "EmpireOfCards/Venture")]
    public class VentureData : ScriptableObject
    {
        public EmpireOfCards.Core.VentureType ventureType;
        public string ventureName;
        [TextArea] public string description;
        public CardData startingBusiness;        // null for KaranlikPazar
        public CardData bonusDeckCard;           // extra card added to deck
        public int bonusMoney;                   // extra starting money (0 for most, 200 for KaranlikPazar)
        public Sprite ventureIcon;
        public VentureBoardProfile boardProfile;
        public VentureDeckProfile deckProfile;
        public VentureEconomyProfile economyProfile;
        [TextArea] public string playstyleSummary;
    }
}
