using System.Collections.Generic;
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

    public static class LaunchVentureScope
    {
        private static readonly HashSet<EmpireOfCards.Core.VentureType> EnabledVentures = new HashSet<EmpireOfCards.Core.VentureType>
        {
            EmpireOfCards.Core.VentureType.FastFood,
            EmpireOfCards.Core.VentureType.Cafe,
            EmpireOfCards.Core.VentureType.GroceryStore
        };

        public static bool IsEnabled(EmpireOfCards.Core.VentureType ventureType)
        {
            return EnabledVentures.Contains(ventureType);
        }

        public static VentureData[] Filter(VentureData[] ventures)
        {
            if (ventures == null || ventures.Length == 0)
                return System.Array.Empty<VentureData>();

            var filtered = new List<VentureData>(ventures.Length);
            for (int i = 0; i < ventures.Length; i++)
            {
                VentureData venture = ventures[i];
                if (venture != null && IsEnabled(venture.ventureType))
                    filtered.Add(venture);
            }

            return filtered.ToArray();
        }
    }
}
