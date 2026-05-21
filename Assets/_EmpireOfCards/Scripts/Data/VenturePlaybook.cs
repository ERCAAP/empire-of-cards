using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "VenturePlaybook", menuName = "EmpireOfCards/V4/Venture Playbook")]
    public class VenturePlaybook : ScriptableObject
    {
        public VentureType ventureType;
        [TextArea] public string fantasySummary;
        [TextArea] public string economySummary;
        public string[] primaryMetrics = Array.Empty<string>();
        public string[] openingSequenceCardIds = Array.Empty<string>();
        public VentureProgressionArc progressionArc;
        public VentureBoardThemeProfile themeProfile;
        public VentureEventChain eventChain;
    }

    [CreateAssetMenu(fileName = "VentureProgressionArc", menuName = "EmpireOfCards/V4/Venture Progression Arc")]
    public class VentureProgressionArc : ScriptableObject
    {
        public VentureType ventureType;
        public TurnScriptBeat[] openingBeats = Array.Empty<TurnScriptBeat>();
        public string[] openingPoolCardIds = Array.Empty<string>();
        public string[] stabilizePoolCardIds = Array.Empty<string>();
        public string[] scalePoolCardIds = Array.Empty<string>();
        public string[] crisisPoolCardIds = Array.Empty<string>();
        public string[] recoverPoolCardIds = Array.Empty<string>();
        public string[] latePoolCardIds = Array.Empty<string>();
    }

    [Serializable]
    public class TurnScriptBeat
    {
        public string beatId;
        public int turnNumber;
        [TextArea] public string headline;
        [TextArea] public string detail;
        [TextArea] public string recommendedMove;
        public string[] priorityCardIds = Array.Empty<string>();
        public string[] highlightPropIds = Array.Empty<string>();
    }

    [CreateAssetMenu(fileName = "VentureBoardThemeProfile", menuName = "EmpireOfCards/V4/Venture Board Theme")]
    public class VentureBoardThemeProfile : ScriptableObject
    {
        public VentureType ventureType;
        public string marketFocusLabel = "TRAFFIC WINDOW";
        public string trustFocusLabel = "TRUST LOOP";
        public string pullFocusLabel = "MARKET PULL";
        public string rivalPressureLabel = "RIVAL PRESSURE";
        public BoardCameraProfile cameraProfile = new BoardCameraProfile();
        public VentureBoardProp[] props = Array.Empty<VentureBoardProp>();
    }

    [Serializable]
    public class BoardCameraProfile
    {
        public Vector3 cameraPosition = new Vector3(0f, 14.8f, -9.2f);
        public Vector3 cameraEuler = new Vector3(51f, 0f, 0f);
        public float fieldOfView = 53f;
        public Vector3 handAnchorPosition = new Vector3(0f, 0.98f, -3.92f);
        public Vector3 handAnchorEuler = new Vector3(-9f, 0f, 0f);
        public float handSpacing = 1.22f;
        public float handFanAngle = 7f;
        public float handVerticalArc = 0.10f;
    }

    [Serializable]
    public class VentureBoardProp
    {
        public string propId;
        public string triggerCardId;
        public SlotType slotType;
        public Vector3 localPosition;
        public Vector3 localScale = Vector3.one;
        public Color idleColor = new Color(0.18f, 0.18f, 0.18f, 1f);
        public Color activeColor = new Color(0.80f, 0.72f, 0.50f, 1f);
        public string label;
    }

    [CreateAssetMenu(fileName = "VentureEventChain", menuName = "EmpireOfCards/V4/Venture Event Chain")]
    public class VentureEventChain : ScriptableObject
    {
        public VentureType ventureType;
        public VentureEventWindow[] windows = Array.Empty<VentureEventWindow>();
    }

    [Serializable]
    public class VentureEventWindow
    {
        public string windowId;
        public int turnStart;
        public int turnEnd;
        public BoardPressureType requiredPressure = BoardPressureType.None;
        public string[] preferredEventCardIds = Array.Empty<string>();
        [TextArea] public string title;
        [TextArea] public string detail;
    }

    [Serializable]
    public class VentureRuntimeState
    {
        public int currentTurn;
        public string buildStage;
        public List<string> seenBeatIds = new List<string>();
        public List<string> unlockedPropIds = new List<string>();
    }

    [Serializable]
    public class OpeningArcState
    {
        public int currentBeatTurn;
        public string currentBeatId;
        public List<string> completedBeatIds = new List<string>();
    }

    [Serializable]
    public class EventChainState
    {
        public List<string> firedWindowIds = new List<string>();
        public string lastEventCardId;
    }

    [Serializable]
    public class RivalRuntimeState
    {
        public int escalationLevel;
        public int campaignsLaunched;
        public float pressureBank;
        public string activePlan;
        public int lastResolvedTurn;
    }
}
