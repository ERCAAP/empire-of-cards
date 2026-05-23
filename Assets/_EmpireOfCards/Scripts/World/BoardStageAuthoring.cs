using UnityEngine;

namespace EmpireOfCards.World
{
    public class BoardStageAuthoring : MonoBehaviour
    {
        [Header("Stage Roots")]
        [SerializeField] private Transform stageRoot;
        [SerializeField] private Transform rivalZoneRoot;
        [SerializeField] private Transform marketZoneRoot;
        [SerializeField] private Transform questionStageRoot;
        [SerializeField] private Transform playerZoneRoot;
        [SerializeField] private Transform handRoot;
        [SerializeField] private Transform effectRailRoot;
        [SerializeField] private Transform cameraTargetsRoot;

        [Header("Question Trays")]
        [SerializeField] private Transform questionTrayA;
        [SerializeField] private Transform questionTrayB;
        [SerializeField] private QuestionDropZone3D[] authoredQuestionZones;

        [Header("Business Anchors")]
        [SerializeField] private SlotZone3D[] operationAnchors;
        [SerializeField] private SlotZone3D[] staffAnchors;
        [SerializeField] private SlotZone3D[] marketingAnchors;
        [SerializeField] private SlotZone3D[] supplierAnchors;
        [SerializeField] private SlotZone3D[] tempEffectAnchors;

        [Header("Customer Targets")]
        [SerializeField] private Transform neutralPoolRoot;
        [SerializeField] private Transform playerFlowRoot;
        [SerializeField] private Transform rivalFlowRoot;
        [SerializeField] private Transform loyalPlayerClusterRoot;
        [SerializeField] private Transform loyalRivalClusterRoot;
        [SerializeField] private Transform[] customerTokens;

        [Header("Hand Anchors")]
        [SerializeField] private Transform buildHandAnchor;
        [SerializeField] private Transform responseHandAnchor;

        [Header("Camera Poses")]
        [SerializeField] private Transform overviewCameraPose;
        [SerializeField] private Transform questionCameraPose;
        [SerializeField] private Transform resolveCameraPose;
        [SerializeField] private Transform rivalCameraPose;
        [SerializeField] private Transform introCameraPose;

        public Transform StageRoot => stageRoot != null ? stageRoot : transform;
        public Transform RivalZoneRoot => rivalZoneRoot;
        public Transform MarketZoneRoot => marketZoneRoot;
        public Transform QuestionStageRoot => questionStageRoot;
        public Transform PlayerZoneRoot => playerZoneRoot;
        public Transform HandRoot => handRoot;
        public Transform EffectRailRoot => effectRailRoot;
        public Transform CameraTargetsRoot => cameraTargetsRoot;
        public Transform QuestionTrayA => questionTrayA;
        public Transform QuestionTrayB => questionTrayB;
        public Transform NeutralPoolRoot => neutralPoolRoot;
        public Transform PlayerFlowRoot => playerFlowRoot;
        public Transform RivalFlowRoot => rivalFlowRoot;
        public Transform LoyalPlayerClusterRoot => loyalPlayerClusterRoot;
        public Transform LoyalRivalClusterRoot => loyalRivalClusterRoot;
        public Transform BuildHandAnchor => buildHandAnchor;
        public Transform ResponseHandAnchor => responseHandAnchor;
        public Transform OverviewCameraPose => overviewCameraPose;
        public Transform QuestionCameraPose => questionCameraPose;
        public Transform ResolveCameraPose => resolveCameraPose;
        public Transform RivalCameraPose => rivalCameraPose;
        public Transform IntroCameraPose => introCameraPose;
        public QuestionDropZone3D[] AuthoredQuestionZones => authoredQuestionZones;
        public SlotZone3D[] OperationAnchors => operationAnchors;
        public SlotZone3D[] StaffAnchors => staffAnchors;
        public SlotZone3D[] MarketingAnchors => marketingAnchors;
        public SlotZone3D[] SupplierAnchors => supplierAnchors;
        public SlotZone3D[] TempEffectAnchors => tempEffectAnchors;
        public Transform[] CustomerTokens => customerTokens;
        public bool HasAuthoredBoardSockets => HasAny(operationAnchors)
            || HasAny(staffAnchors)
            || HasAny(marketingAnchors)
            || HasAny(supplierAnchors)
            || HasAny(tempEffectAnchors)
            || HasAny(authoredQuestionZones);

        private void Reset()
        {
            EnsureLayout();
        }

        private void Awake()
        {
            EnsureLayout();
        }

        public void EnsureLayout()
        {
            stageRoot = EnsureChild("StageRoot", transform, Vector3.zero, Vector3.zero);
            rivalZoneRoot = EnsureChild("RivalZoneRoot", stageRoot, new Vector3(0f, 0f, 6.05f), Vector3.zero);
            marketZoneRoot = EnsureChild("MarketZoneRoot", stageRoot, new Vector3(0f, 0f, 2.75f), Vector3.zero);
            questionStageRoot = EnsureChild("QuestionStageRoot", stageRoot, new Vector3(0f, 0f, 2.55f), Vector3.zero);
            playerZoneRoot = EnsureChild("PlayerZoneRoot", stageRoot, new Vector3(0f, 0f, -0.92f), Vector3.zero);
            handRoot = EnsureChild("HandRoot", stageRoot, new Vector3(0f, 0.84f, -3.06f), new Vector3(-2f, 0f, 0f));
            effectRailRoot = EnsureChild("EffectRailRoot", stageRoot, new Vector3(6.52f, 0f, -1.00f), Vector3.zero);
            cameraTargetsRoot = EnsureChild("CameraTargetsRoot", stageRoot, Vector3.zero, Vector3.zero);

            questionTrayA = EnsureChild("QuestionTrayA", questionStageRoot, new Vector3(-2.45f, 0.12f, 0f), Vector3.zero);
            questionTrayB = EnsureChild("QuestionTrayB", questionStageRoot, new Vector3(2.45f, 0.12f, 0f), Vector3.zero);

            neutralPoolRoot = EnsureChild("NeutralPoolRoot", marketZoneRoot, new Vector3(0f, 0.24f, -0.03f), Vector3.zero);
            playerFlowRoot = EnsureChild("PlayerFlowRoot", marketZoneRoot, new Vector3(-3.25f, 0.24f, -0.38f), Vector3.zero);
            rivalFlowRoot = EnsureChild("RivalFlowRoot", marketZoneRoot, new Vector3(3.25f, 0.24f, 0.38f), Vector3.zero);
            loyalPlayerClusterRoot = EnsureChild("LoyalPlayerClusterRoot", marketZoneRoot, new Vector3(-4.55f, 0.24f, -0.65f), Vector3.zero);
            loyalRivalClusterRoot = EnsureChild("LoyalRivalClusterRoot", marketZoneRoot, new Vector3(4.55f, 0.24f, 0.65f), Vector3.zero);

            buildHandAnchor = EnsureChild("BuildHandAnchor", handRoot, new Vector3(-2.55f, 0f, 0f), Vector3.zero);
            responseHandAnchor = EnsureChild("ResponseHandAnchor", handRoot, new Vector3(2.55f, 0f, 0.10f), Vector3.zero);

            overviewCameraPose = EnsureChild("OverviewCameraPose", cameraTargetsRoot, new Vector3(0f, 16.6f, -4.6f), new Vector3(68.5f, 0f, 0f));
            questionCameraPose = EnsureChild("QuestionCameraPose", cameraTargetsRoot, new Vector3(0f, 15.2f, -2.8f), new Vector3(72.5f, 0f, 0f));
            resolveCameraPose = EnsureChild("ResolveCameraPose", cameraTargetsRoot, new Vector3(0f, 16.0f, -4.0f), new Vector3(69.5f, 0f, 0f));
            rivalCameraPose = EnsureChild("RivalCameraPose", cameraTargetsRoot, new Vector3(0f, 16.9f, -2.7f), new Vector3(63.5f, 0f, 0f));
            introCameraPose = EnsureChild("IntroCameraPose", cameraTargetsRoot, new Vector3(0f, 18.8f, -7.3f), new Vector3(61f, 0f, 0f));
        }

        public bool HasRequiredStageReferences()
        {
            return stageRoot != null
                && rivalZoneRoot != null
                && marketZoneRoot != null
                && questionStageRoot != null
                && playerZoneRoot != null
                && handRoot != null
                && effectRailRoot != null
                && cameraTargetsRoot != null
                && questionTrayA != null
                && questionTrayB != null
                && buildHandAnchor != null
                && responseHandAnchor != null
                && overviewCameraPose != null
                && questionCameraPose != null
                && resolveCameraPose != null
                && rivalCameraPose != null
                && introCameraPose != null;
        }

        public void LogMissingReferences()
        {
            if (HasRequiredStageReferences())
                return;

            Debug.LogError("[BoardStageAuthoring] Missing authored stage references. Game.unity must contain StageRoot, zone roots, two question trays, two hand anchors, and camera poses before production play.");
        }

        public Transform GetQuestionTray(int index)
        {
            return index == 0 ? questionTrayA : questionTrayB;
        }

        private static Transform EnsureChild(string childName, Transform parent, Vector3 localPosition, Vector3 localEuler)
        {
            Transform child = null;
            bool created = false;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform candidate = parent.GetChild(i);
                if (candidate != null && candidate.name == childName)
                {
                    child = candidate;
                    break;
                }
            }

            if (child == null)
            {
                var go = new GameObject(childName);
                child = go.transform;
                child.SetParent(parent, false);
                created = true;
            }

            if (created)
            {
                child.localPosition = localPosition;
                child.localRotation = Quaternion.Euler(localEuler);
                child.localScale = Vector3.one;
            }

            return child;
        }

        private static bool HasAny<T>(T[] entries) where T : Object
        {
            if (entries == null)
                return false;

            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i] != null)
                    return true;
            }

            return false;
        }
    }
}
