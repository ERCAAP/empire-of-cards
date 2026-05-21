using System;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewSectorProfile", menuName = "EmpireOfCards/SectorProfile")]
    public class SectorProfile : ScriptableObject
    {
        [Header("Identity")]
        public SectorType sectorType;
        public string displayName;
        [TextArea(2, 4)]
        public string description;

        [Header("Starting Stats")]
        public int startingCash;
        public float startingDemand;
        public float startingCapacity;
        public float startingQuality;
        public float startingRating;
        public float startingStability;
        public float startingLegalRisk;
        public float startingMarketShare;
        public float startingHygiene;

        [Header("Sub-Slot Names")]
        public string[] kitchenSubSlots;
        public string[] salonSubSlots;
        public string[] storageSubSlots;
        public string[] marketingSubSlots;

        [Header("Slot Counts Per Era (index 0=Era1, 1=Era2, 2=Era3, 3=Era4)")]
        public EraSlotLayout[] eraSlotLayouts;

        [Header("Season Multipliers (Spring, Summer, Autumn, Winter, Ramadan)")]
        public float[] seasonMultipliers;

        [Header("Derived Metric Names")]
        public string[] derivedMetricNames;
    }

    [Serializable]
    public struct EraSlotLayout
    {
        public Era era;
        public int kitchenSlots;
        public int salonSlots;
        public int storageSlots;
        public int marketingSlots;
        public int tempEffectSlots;
        public int actionsPerTurn;

        public int TotalSlots =>
            kitchenSlots + salonSlots + storageSlots + marketingSlots + tempEffectSlots;
    }
}
