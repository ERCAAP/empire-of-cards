using UnityEngine;
using Unity.Cinemachine;
using EmpireOfCards.Core;
using DG.Tweening;

namespace EmpireOfCards.World
{
    /// <summary>
    /// Cinemachine 3.x camera controller. Creates Main Camera + CinemachineBrain
    /// and multiple CinemachineCamera instances for different views.
    /// Subscribes to EventBus for automatic camera transitions during phases.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        // ── Camera references ───────────────────────────────────────
        Camera _mainCamera;
        CinemachineBrain _brain;

        CinemachineCamera _overviewCam;
        CinemachineCamera _kitchenCam;
        CinemachineCamera _streetCam;
        CinemachineCamera _rivalCam;

        // ── Priority constants ──────────────────────────────────────
        const int PRIORITY_ACTIVE   = 10;
        const int PRIORITY_INACTIVE = 0;

        // ── Shake state ─────────────────────────────────────────────
        Vector3 _overviewOriginalPos;
        bool _isShaking;

        // ── Active camera tracking ──────────────────────────────────
        CinemachineCamera _activeCam;

        // ── Init ────────────────────────────────────────────────────

        public void Init()
        {
            CreateMainCamera();
            CreateOverviewCamera();
            CreateKitchenCamera();
            CreateStreetCamera();
            CreateRivalCamera();

            // Default to overview
            SetActiveCamera(_overviewCam, 0f);

            Debug.Log("[CameraController] Initialized with 4 Cinemachine cameras.");
        }

        // ── EventBus subscriptions ──────────────────────────────────

        void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnPhaseEnded += HandlePhaseEnded;
            EventBus.OnCrisisTriggered += HandleCrisis;
            EventBus.OnResolveStep += HandleResolveStep;
            EventBus.OnRivalAction += HandleRivalAction;
            EventBus.OnEraChanged += HandleEraChanged;
        }

        void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnPhaseEnded -= HandlePhaseEnded;
            EventBus.OnCrisisTriggered -= HandleCrisis;
            EventBus.OnResolveStep -= HandleResolveStep;
            EventBus.OnRivalAction -= HandleRivalAction;
            EventBus.OnEraChanged -= HandleEraChanged;
        }

        // ── Phase auto-transitions ──────────────────────────────────

        void HandlePhaseStarted(TurnPhase phase)
        {
            switch (phase)
            {
                case TurnPhase.DrawPhase:
                case TurnPhase.PlanningPhase:
                case TurnPhase.PlayPhase:
                    SetActiveCamera(_overviewCam, 1.2f);
                    break;

                case TurnPhase.ResolvePhase:
                    // Start with kitchen focus, auto-return handled by resolve steps
                    SetActiveCamera(_kitchenCam, 1.5f);
                    break;

                case TurnPhase.RivalPhase:
                    SetActiveCamera(_rivalCam, 1.5f);
                    break;

                case TurnPhase.MarketUpdatePhase:
                    SetActiveCamera(_overviewCam, 1f);
                    break;
            }
        }

        void HandlePhaseEnded(TurnPhase phase)
        {
            // After any non-overview phase, ease back to overview
            if (phase == TurnPhase.ResolvePhase || phase == TurnPhase.RivalPhase)
            {
                SetActiveCamera(_overviewCam, 1f);
            }
        }

        void HandleResolveStep(ResolveStep step)
        {
            switch (step)
            {
                case ResolveStep.CustomerFlow:
                    SetActiveCamera(_streetCam, 1f);
                    break;

                case ResolveStep.CalculateIncome:
                    SetActiveCamera(_overviewCam, 0.8f);
                    break;
            }
        }

        void HandleCrisis(CrisisType crisis)
        {
            ShakeCamera(0.15f, 0.8f);
        }

        void HandleRivalAction(RivalMove move, string description)
        {
            // Brief shake on aggressive rival moves
            if (move == RivalMove.PriceWar || move == RivalMove.Sabotage || move == RivalMove.StaffPoach)
            {
                ShakeCamera(0.08f, 0.4f);
            }
        }

        void HandleEraChanged(Era era)
        {
            // Dramatic zoom on era change
            if (_overviewCam != null)
            {
                float originalFov = _overviewCam.Lens.FieldOfView;
                DOTween.Sequence()
                    .Append(DOTween.To(() => _overviewCam.Lens.FieldOfView,
                        v => { var lens = _overviewCam.Lens; lens.FieldOfView = v; _overviewCam.Lens = lens; },
                        originalFov - 8f, 0.5f).SetEase(Ease.InOutQuad))
                    .Append(DOTween.To(() => _overviewCam.Lens.FieldOfView,
                        v => { var lens = _overviewCam.Lens; lens.FieldOfView = v; _overviewCam.Lens = lens; },
                        originalFov, 0.5f).SetEase(Ease.OutBack));
            }
        }

        // ── Camera creation ─────────────────────────────────────────

        void CreateMainCamera()
        {
            // Destroy any existing Main Camera to avoid conflicts
            var existing = Camera.main;
            if (existing != null)
                Destroy(existing.gameObject);

            var go = new GameObject("MainCamera");
            go.tag = "MainCamera";
            go.transform.SetParent(transform);

            _mainCamera = go.AddComponent<Camera>();
            _mainCamera.clearFlags = CameraClearFlags.SolidColor;
            _mainCamera.backgroundColor = new Color(0.08f, 0.08f, 0.1f);
            _mainCamera.nearClipPlane = 0.1f;
            _mainCamera.farClipPlane = 100f;
            _mainCamera.fieldOfView = 40f;

            // Cinemachine 3.x brain
            _brain = go.AddComponent<CinemachineBrain>();

            // Default position (will be overridden by Cinemachine)
            go.transform.position = new Vector3(0f, 18f, 0f);
            go.transform.rotation = Quaternion.Euler(70f, 0f, 0f);
        }

        void CreateOverviewCamera()
        {
            var go = new GameObject("CM_Overview");
            go.transform.SetParent(transform);
            // Higher up + centered so hand cards AND street both visible
            go.transform.position = new Vector3(0f, 20f, 2f);
            go.transform.rotation = Quaternion.Euler(72f, 0f, 0f);

            _overviewCam = go.AddComponent<CinemachineCamera>();
            var lens = _overviewCam.Lens;
            lens.FieldOfView = 50f; // wider to see everything
            _overviewCam.Lens = lens;
            _overviewCam.Priority = PRIORITY_ACTIVE;

            _overviewOriginalPos = go.transform.position;
        }

        void CreateKitchenCamera()
        {
            var go = new GameObject("CM_Kitchen");
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(-3f, 10f, 1f);
            go.transform.rotation = Quaternion.Euler(65f, 10f, 0f);

            _kitchenCam = go.AddComponent<CinemachineCamera>();
            var lens = _kitchenCam.Lens;
            lens.FieldOfView = 40f;
            _kitchenCam.Lens = lens;
            _kitchenCam.Priority = PRIORITY_INACTIVE;
        }

        void CreateStreetCamera()
        {
            var go = new GameObject("CM_Street");
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(0f, 12f, 9f);
            go.transform.rotation = Quaternion.Euler(60f, 0f, 0f);

            _streetCam = go.AddComponent<CinemachineCamera>();
            var lens = _streetCam.Lens;
            lens.FieldOfView = 45f;
            _streetCam.Lens = lens;
            _streetCam.Priority = PRIORITY_INACTIVE;
        }

        void CreateRivalCamera()
        {
            var go = new GameObject("CM_Rival");
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(0f, 10f, 8f);
            go.transform.rotation = Quaternion.Euler(55f, 0f, 0f);

            _rivalCam = go.AddComponent<CinemachineCamera>();
            var lens = _rivalCam.Lens;
            lens.FieldOfView = 42f;
            _rivalCam.Lens = lens;
            _rivalCam.Priority = PRIORITY_INACTIVE;
        }

        // ── Public API ──────────────────────────────────────────────

        public void SetActiveCamera(CinemachineCamera cam, float blendTime)
        {
            if (cam == null || cam == _activeCam) return;

            // Deactivate current
            if (_activeCam != null)
                _activeCam.Priority = PRIORITY_INACTIVE;

            cam.Priority = PRIORITY_ACTIVE;
            _activeCam = cam;
        }

        public void FocusOnZone(SlotType zone)
        {
            switch (zone)
            {
                case SlotType.Kitchen:
                    SetActiveCamera(_kitchenCam, 1f);
                    break;
                default:
                    SetActiveCamera(_overviewCam, 0.8f);
                    break;
            }
        }

        public void ShakeCamera(float intensity, float duration)
        {
            if (_isShaking || _overviewCam == null) return;
            _isShaking = true;

            var camTransform = _overviewCam.transform;
            var originalPos = camTransform.localPosition;

            camTransform.DOShakePosition(duration, intensity, 15, 90f, false)
                .OnComplete(() =>
                {
                    camTransform.localPosition = originalPos;
                    _isShaking = false;
                });
        }

        public void ReturnToOverview(float blendTime = 1f)
        {
            SetActiveCamera(_overviewCam, blendTime);
        }

        // ── Accessor ────────────────────────────────────────────────

        public Camera MainCamera => _mainCamera;
    }
}
