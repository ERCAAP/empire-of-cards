using System;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "VentureBoardProfile", menuName = "EmpireOfCards/V4/Venture Board Profile")]
    public class VentureBoardProfile : ScriptableObject
    {
        public VentureType ventureType;
        public string displayName;
        public BoardSubSlotDefinition[] operationSubSlots;
        public BoardSubSlotDefinition[] staffSubSlots;
        public BoardSubSlotDefinition[] marketingSubSlots;
        public BoardSubSlotDefinition[] supplierSubSlots;
        public int startingOperationSlots = Constants.STARTING_OPERATION_SLOTS;
        public int startingStaffSlots = Constants.STARTING_STAFF_SLOTS;
        public int startingMarketingSlots = Constants.STARTING_MARKETING_SLOTS;
        public int startingSupplierSlots = Constants.STARTING_SUPPLIER_SLOTS;
        public int maxOperationSlots = Constants.MAX_OPERATION_SLOTS;
        public int maxStaffSlots = Constants.MAX_STAFF_SLOTS;
        public int maxMarketingSlots = Constants.MAX_MARKETING_SLOTS;
        public int maxSupplierSlots = Constants.MAX_SUPPLIER_SLOTS;
        public SlotUnlockStep[] unlockSteps;
    }

    [CreateAssetMenu(fileName = "VentureDeckProfile", menuName = "EmpireOfCards/V4/Venture Deck Profile")]
    public class VentureDeckProfile : ScriptableObject
    {
        public VentureType ventureType;
        public string[] starterCardIds;
        public string[] earlyPoolCardIds;
        public string[] midPoolCardIds;
        public string[] latePoolCardIds;
        public string[] neutralCardIds;
        public string[] crisisCardIds;
        public DeckBiasRule[] drawBiasRules;
    }

    [CreateAssetMenu(fileName = "VentureEconomyProfile", menuName = "EmpireOfCards/V4/Venture Economy Profile")]
    public class VentureEconomyProfile : ScriptableObject
    {
        public VentureType ventureType;
        public float startingCash = Constants.STARTING_MONEY;
        public float startingDemand = 2f;
        public float startingCapacity = 3f;
        public float startingQuality = 3f;
        public float startingRating = Constants.PLATFORM_RATING_DEFAULT;
        public float startingStaffStability = 6f;
        public float startingLegalRisk = 0f;
        public float startingMarketShare = 10f;
        public float baseDemand = 2f;
        public float baseRevenuePerDemand = 18f;
        public float capacityPenaltyMultiplier = 0.35f;
        public float qualityToRatingWeight = 0.18f;
        public float ratingToOrganicDemandWeight = 0.6f;
        public float staffInstabilityPenalty = 0.20f;
        public float legalRiskTriggerWeight = 0.15f;
        public float marketShareGainWeight = 1f;
        public float demandDecayPerTurn = 0.5f;
        public float legalRiskDecayPerTurn = 2f;
        public float minRating = Constants.PLATFORM_RATING_MIN;
        public float maxRating = Constants.PLATFORM_RATING_MAX;
        public DerivedMetricRule[] derivedMetrics;
    }

    [Serializable]
    public class BoardSubSlotDefinition
    {
        public string id;
        public string labelKey;
        public string fallbackLabel;
    }

    [Serializable]
    public class SlotUnlockStep
    {
        public string id;
        public int requiredTurn;
        public int operationDelta;
        public int staffDelta;
        public int marketingDelta;
        public int supplierDelta;
    }

    [Serializable]
    public class DerivedMetricRule
    {
        public string id;
        public string labelKey;
        public string fallbackLabel;
        public float startingValue;
    }

    [Serializable]
    public class DeckBiasRule
    {
        public BoardPressureType pressure;
        public CardFamily preferredFamily;
        public float bonusWeight = 2f;
    }

    [Serializable]
    public class DerivedMetricValue
    {
        public string id;
        public float value;
    }

    [Serializable]
    public class VentureBoardSnapshot
    {
        public VentureType ventureType;
        public float cash;
        public float demand;
        public float capacity;
        public float quality;
        public float rating;
        public float staffStability;
        public float legalRisk;
        public float marketShare;
        public DerivedMetricValue[] derivedMetrics = Array.Empty<DerivedMetricValue>();
        public string[] activeCrisisTags = Array.Empty<string>();
    }
}
