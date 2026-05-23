using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    public enum BoardCameraState
    {
        Overview,
        QuestionFocus,
        Resolve,
        RivalPressure,
        VentureIntro
    }

    [Serializable]
    public class CameraFocusRequest
    {
        public BoardCameraState state = BoardCameraState.Overview;
        public int questionIndex = -1;
        public string reason;
        public float blendDuration = 0.45f;
    }

    [Serializable]
    public class QuestionPresentationModel
    {
        public int questionIndex;
        public string headline;
        public string detail;
        public string needTag;
        public string supportTag;
        public string riskWarning;
        public string outcomeLabel;
        public string primaryCardName;
        public string supportCardName;
        public string questionFamily;
        public bool supportAllowed;
    }

    [Serializable]
    public class BoardAnchorViewModel
    {
        public SlotType slotType;
        public int slotIndex;
        public string laneId;
        public string label;
        public bool occupied;
        public bool highlighted;
        public string residentCardId;
    }

    [Serializable]
    public class RivalPressureViewModel
    {
        public string headline;
        public string laneLabel;
        public string shortDescription;
        public string moodIcon;
        public float pressureDelta;
        public float demandSteal;
        public float ratingDelta;
    }

    [Serializable]
    public class CustomerTokenTargetSet
    {
        public List<Vector3> neutralPositions = new List<Vector3>();
        public List<Vector3> playerPositions = new List<Vector3>();
        public List<Vector3> rivalPositions = new List<Vector3>();
        public List<Vector3> loyalPlayerPositions = new List<Vector3>();
        public List<Vector3> loyalRivalPositions = new List<Vector3>();
    }
}
