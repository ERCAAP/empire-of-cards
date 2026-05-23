using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [Serializable]
    public class QuestionDefinition
    {
        public string questionId;
        public VentureType ventureType;
        public string headline;
        public string detail;
        public string primaryTag;
        public string optionalSupportTag;
        public string riskWarning;
        public string questionFamily;
        public string[] followUpIds = Array.Empty<string>();
        public string cameraFocusHint;
        public string presentationStyle;
        public bool supportAllowed;
        public bool forced;
        public float penaltyDemandDelta;
        public float penaltyCapacityDelta;
        public float penaltyQualityDelta;
        public float penaltyRatingDelta;
        public float penaltyRiskDelta;
        public float penaltyCashDelta;
    }

    public enum QuestionResolutionState
    {
        Pending,
        SolvedCleanly,
        SolvedExpensively,
        SolvedDangerously,
        PartiallySolved,
        Ignored
    }

    [Serializable]
    public class QuestionRuntimeState
    {
        public QuestionDefinition definition;
        public int spawnedTurn;
        public CardData committedPrimaryCard;
        public CardData committedSupportCard;
        public QuestionResolutionState resolutionState;
        public string outcomeLabel;

        public bool HasPrimary => committedPrimaryCard != null;
        public bool HasSupport => committedSupportCard != null;
    }

    [Serializable]
    public class DecisionRecord
    {
        public int turnNumber;
        public string questionId;
        public string questionHeadline;
        public string primaryCardId;
        public string supportCardId;
        public string buildCardId;
        public string placementType;
        public string laneId;
        public string questionZoneId;
        public string outcomeLabel;
        public List<string> resolvedEffects = new List<string>();
        public List<string> delayedRiskFlags = new List<string>();
        public List<string> carriedEffects = new List<string>();
    }

    [Serializable]
    public class CustomerFlowSnapshot
    {
        public int turnNumber;
        public int neutralCount;
        public int movedToPlayer;
        public int movedToRival;
        public int loyalPlayerCount;
        public int loyalRivalCount;
        public string dominantReason;
    }

    [Serializable]
    public class TurnResolutionReport
    {
        public int turnNumber;
        public int cashDelta;
        public float ratingDelta;
        public float riskDelta;
        public float marketShareDelta;
        public int customersToPlayer;
        public int customersToRival;
        public string dominantReason;
        public List<DecisionRecord> records = new List<DecisionRecord>();
        public List<string> reasons = new List<string>();
    }
}
