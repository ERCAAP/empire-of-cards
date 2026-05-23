using UnityEngine;
using Unity.Cinemachine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.World;

namespace EmpireOfCards.Helpers
{
    /// <summary>
    /// Camera controller with Cinemachine-based phase transitions and screen shake.
    /// Attach to the main camera GameObject.
    /// Creates a CinemachineBrain on the camera and switches board states through EventBus focus events.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private float shakeIntensity;
        [SerializeField] private float shakeDuration;

        private Vector3 originalPosition;
        private float shakeTimer;

        private CinemachineCamera overviewCamera;
        private CinemachineCamera questionFocusCamera;
        private CinemachineCamera resolveCamera;
        private CinemachineCamera rivalPressureCamera;
        private CinemachineCamera ventureIntroCamera;
        private BoardCameraState currentState = BoardCameraState.Overview;
        private bool questionFocusActive;

        private void Awake()
        {
            originalPosition = transform.localPosition;
            SetupCinemachineBrain();
            CreateVirtualCameras();
        }

        private void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnQuestionFocusRequested += HandleQuestionFocusRequested;
            EventBus.OnQuestionFocusReleased += HandleQuestionFocusReleased;
        }

        private void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnQuestionFocusRequested -= HandleQuestionFocusRequested;
            EventBus.OnQuestionFocusReleased -= HandleQuestionFocusReleased;
        }

        private void Update()
        {
            if (shakeTimer > 0f)
            {
                // Apply random offset based on current intensity
                float currentIntensity = shakeIntensity * (shakeTimer / shakeDuration);
                Vector3 offset = new Vector3(
                    Random.Range(-currentIntensity, currentIntensity),
                    Random.Range(-currentIntensity, currentIntensity),
                    0f
                );

                transform.localPosition = originalPosition + offset;

                shakeTimer -= Time.deltaTime;

                // Snap back when shake is done
                if (shakeTimer <= 0f)
                {
                    transform.localPosition = originalPosition;
                }
            }
        }

        /// <summary>
        /// Triggers a screen shake effect with the given intensity and duration.
        /// </summary>
        /// <param name="intensity">Maximum offset in units.</param>
        /// <param name="duration">Duration in seconds.</param>
        public void Shake(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
            shakeTimer = duration;
            originalPosition = transform.localPosition;
        }

        /// <summary>
        /// Immediately stops any active shake and snaps the camera back to its original position.
        /// </summary>
        public void ResetPosition()
        {
            shakeTimer = 0f;
            transform.localPosition = originalPosition;
        }

        // ==============================================================
        //  Cinemachine Setup
        // ==============================================================

        /// <summary>
        /// Ensures a CinemachineBrain is attached to this camera with a
        /// 0.5s EaseInOut default blend for smooth phase transitions.
        /// </summary>
        private void SetupCinemachineBrain()
        {
            var brain = GetComponent<CinemachineBrain>();
            if (brain == null)
                brain = gameObject.AddComponent<CinemachineBrain>();

            brain.DefaultBlend = new CinemachineBlendDefinition(
                CinemachineBlendDefinition.Styles.EaseInOut, 0.5f);
        }

        /// <summary>
        /// Spawns the board-state cameras. Overview starts active; focus states are event-driven.
        /// </summary>
        private void CreateVirtualCameras()
        {
            overviewCamera = CreateVCam(
                "OverviewCamera",
                new Vector3(0f, 16.6f, -4.6f),
                new Vector3(0f, 0f, 1.7f),
                40f,
                10);

            questionFocusCamera = CreateVCam(
                "QuestionFocusCamera",
                new Vector3(0f, 15.2f, -2.8f),
                new Vector3(0f, 0f, 2.55f),
                34f,
                0);

            resolveCamera = CreateVCam(
                "ResolveCamera",
                new Vector3(0f, 16.0f, -4.0f),
                new Vector3(0f, 0f, 2.0f),
                37f,
                0);

            rivalPressureCamera = CreateVCam(
                "RivalPressureCamera",
                new Vector3(0f, 16.9f, -2.7f),
                new Vector3(0f, 0f, 4.8f),
                38.5f,
                0);

            ventureIntroCamera = CreateVCam(
                "VentureIntroCamera",
                new Vector3(0f, 18.8f, -7.3f),
                new Vector3(0f, 0f, 1.0f),
                42f,
                0);
        }

        /// <summary>
        /// Helper: creates a CinemachineCamera with the given position, look-at
        /// point, FOV, and priority. The camera faces the look-at point via
        /// Transform.LookAt so Cinemachine drives position/rotation passively.
        /// </summary>
        private CinemachineCamera CreateVCam(
            string vcamName,
            Vector3 position,
            Vector3 lookAtPoint,
            float fov,
            int priority)
        {
            var go = new GameObject(vcamName);
            go.transform.position = position;
            go.transform.LookAt(lookAtPoint);

            var vcam = go.AddComponent<CinemachineCamera>();

            vcam.Lens = new LensSettings
            {
                FieldOfView = fov,
                NearClipPlane = 0.3f,
                FarClipPlane = 1000f,
                ModeOverride = LensSettings.OverrideModes.Perspective
            };

            vcam.Priority = new PrioritySettings { Value = priority };

            return vcam;
        }

        public void ApplyStage(BoardStageAuthoring stage)
        {
            if (stage == null)
                return;

            stage.EnsureLayout();
            ApplyPose(overviewCamera, stage.OverviewCameraPose, new Vector3(0f, 16.6f, -4.6f), new Vector3(0f, 0f, 1.7f), 40f);
            ApplyPose(questionFocusCamera, stage.QuestionCameraPose, new Vector3(0f, 15.2f, -2.8f), new Vector3(0f, 0f, 2.55f), 34f);
            ApplyPose(resolveCamera, stage.ResolveCameraPose, new Vector3(0f, 16.0f, -4.0f), new Vector3(0f, 0f, 2.0f), 37f);
            ApplyPose(rivalPressureCamera, stage.RivalCameraPose, new Vector3(0f, 16.9f, -2.7f), new Vector3(0f, 0f, 4.8f), 38.5f);
            ApplyPose(ventureIntroCamera, stage.IntroCameraPose, new Vector3(0f, 18.8f, -7.3f), new Vector3(0f, 0f, 1.0f), 42f);
            ActivateCameraState(currentState);
        }

        private static void ApplyPose(CinemachineCamera vcam, Transform pose, Vector3 fallbackPosition, Vector3 fallbackLookAt, float fov)
        {
            if (vcam == null)
                return;

            if (pose != null)
            {
                vcam.transform.position = pose.position;
                vcam.transform.rotation = pose.rotation;
            }
            else
            {
                vcam.transform.position = fallbackPosition;
                vcam.transform.LookAt(fallbackLookAt);
            }

            var lens = vcam.Lens;
            lens.FieldOfView = fov;
            vcam.Lens = lens;
        }

        // ==============================================================
        //  Phase Camera Switching
        // ==============================================================

        /// <summary>
        /// Switches the active virtual camera based on the current turn phase.
        /// Cinemachine blends between cameras automatically via the Brain.
        /// </summary>
        private void HandlePhaseStarted(TurnPhase phase)
        {
            switch (phase)
            {
                case TurnPhase.ResolvePhase:
                    questionFocusActive = false;
                    ActivateCameraState(BoardCameraState.Resolve);
                    break;
                case TurnPhase.RivalPhase:
                    questionFocusActive = false;
                    ActivateCameraState(BoardCameraState.RivalPressure);
                    break;
                case TurnPhase.MarketUpdatePhase:
                    questionFocusActive = false;
                    ActivateCameraState(BoardCameraState.Overview);
                    break;
                case TurnPhase.DrawPhase:
                case TurnPhase.PlanningPhase:
                case TurnPhase.PlayPhase:
                    if (!questionFocusActive)
                        ActivateCameraState(BoardCameraState.Overview);
                    break;
                default:
                    if (!questionFocusActive)
                        ActivateCameraState(BoardCameraState.Overview);
                    break;
            }
        }

        private void HandleQuestionFocusRequested(CameraFocusRequest request)
        {
            BoardCameraState nextState = request != null ? request.state : BoardCameraState.QuestionFocus;
            questionFocusActive = nextState == BoardCameraState.QuestionFocus;
            ActivateCameraState(nextState);
        }

        private void HandleQuestionFocusReleased()
        {
            questionFocusActive = false;
            ActivateCameraState(BoardCameraState.Overview);
        }

        private void ActivateCameraState(BoardCameraState state)
        {
            currentState = state;
            CinemachineCamera target = state switch
            {
                BoardCameraState.QuestionFocus => questionFocusCamera,
                BoardCameraState.Resolve => resolveCamera,
                BoardCameraState.RivalPressure => rivalPressureCamera,
                BoardCameraState.VentureIntro => ventureIntroCamera,
                _ => overviewCamera
            };

            SetPriority(overviewCamera, target);
            SetPriority(questionFocusCamera, target);
            SetPriority(resolveCamera, target);
            SetPriority(rivalPressureCamera, target);
            SetPriority(ventureIntroCamera, target);
        }

        private static void SetPriority(CinemachineCamera camera, CinemachineCamera active)
        {
            if (camera == null)
                return;

            camera.Priority = new PrioritySettings { Value = camera == active ? 10 : 0 };
        }
    }
}
