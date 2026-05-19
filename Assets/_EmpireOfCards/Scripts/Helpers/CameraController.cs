using UnityEngine;
using Unity.Cinemachine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Helpers
{
    /// <summary>
    /// Camera controller with Cinemachine-based phase transitions and screen shake.
    /// Attach to the main camera GameObject.
    /// Creates a CinemachineBrain on the camera and spawns three virtual cameras
    /// (Play, Resolve, Rival) that switch automatically via EventBus phase events.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Shake Settings")]
        [SerializeField] private float shakeIntensity;
        [SerializeField] private float shakeDuration;

        private Vector3 originalPosition;
        private float shakeTimer;

        // Cinemachine virtual cameras for each game phase
        private CinemachineCamera playCamera;
        private CinemachineCamera resolveCamera;
        private CinemachineCamera rivalCamera;

        private void Awake()
        {
            originalPosition = transform.localPosition;
            SetupCinemachineBrain();
            CreateVirtualCameras();
        }

        private void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
        }

        private void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
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
        /// Spawns three virtual cameras for Play, Resolve, and Rival phases.
        /// Only PlayCamera starts at priority 10 (active); others begin at 0.
        /// </summary>
        private void CreateVirtualCameras()
        {
            playCamera = CreateVCam(
                "PlayCamera",
                new Vector3(0f, 8.5f, -5.5f),
                new Vector3(0f, 0f, 0f),
                45f,
                10);

            resolveCamera = CreateVCam(
                "ResolveCamera",
                new Vector3(0f, 7.0f, -4.0f),
                new Vector3(0f, 0f, -1.0f),
                40f,
                0);

            rivalCamera = CreateVCam(
                "RivalCamera",
                new Vector3(0f, 8.0f, -3.0f),
                new Vector3(0f, 0f, 2.5f),
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
                case TurnPhase.PlayPhase:
                    ActivateCamera(playCamera);
                    break;
                case TurnPhase.ResolvePhase:
                    ActivateCamera(resolveCamera);
                    break;
                case TurnPhase.RivalPhase:
                    ActivateCamera(rivalCamera);
                    break;
                default:
                    // EventPhase, DrawPhase, or any other -> default overview
                    ActivateCamera(playCamera);
                    break;
            }
        }

        /// <summary>
        /// Sets the target camera to priority 10 and all others to 0.
        /// The CinemachineBrain will blend to the highest-priority camera.
        /// </summary>
        private void ActivateCamera(CinemachineCamera target)
        {
            playCamera.Priority = new PrioritySettings { Value = target == playCamera ? 10 : 0 };
            resolveCamera.Priority = new PrioritySettings { Value = target == resolveCamera ? 10 : 0 };
            rivalCamera.Priority = new PrioritySettings { Value = target == rivalCamera ? 10 : 0 };
        }
    }
}
