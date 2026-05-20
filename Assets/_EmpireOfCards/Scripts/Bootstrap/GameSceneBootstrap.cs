using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using EmpireOfCards.Core;
using EmpireOfCards.World;
using EmpireOfCards.UI;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.VFX;
using EmpireOfCards.Helpers;
using EmpireOfCards.Data;
using EmpireOfCards.Presentation;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Thin orchestrator for the 3D card game scene.
    /// Drop on an empty GameObject, press Play, game runs.
    /// All heavy lifting is delegated to:
    ///   - CardDataFactory  (40 cards, combos, balance, deck, rival, shop pool)
    ///   - ManagerFactory    (all runtime managers)
    ///   - HUDBuilder        (UI canvas, panels, buttons, popups)
    ///   - WiringService     (cross-references, button callbacks, 3D interaction)
    /// </summary>
    public class GameSceneBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            // 1. Create all game data in memory
            var data = CardDataFactory.CreateAllData();

            // 2. Create all managers
            var managers = ManagerFactory.CreateAll();

            // 3. Setup 3D camera
            var mainCamera = Setup3DCamera();

            // 3.5. Setup lighting and post-processing
            SetupLightingAndPostProcessing();

            // 4. Build 3D board
            var board3D = Build3DBoard();

            // 5. Create card factory
            var cardFactory = CreateCardFactory();

            // 6. Build 3D hand (anchored to camera)
            var hand3D = Build3DHand(mainCamera, cardFactory);

            // 7. Build HUD overlay
            var hud = HUDBuilder.Build();

            // 8. Create VFX root
            CreateVFX();

            // 9. Wire everything together
            WiringService.WireAll(data, managers, board3D, cardFactory, hand3D, hud, mainCamera);

            // 9.5. Store venture selection references
            _ventureSelectionUI = hud.ventureSelectionUI;
            _ventures = data.ventures;

            // Wire venture UI card buttons and start button
            if (_ventureSelectionUI != null && hud.ventureCards != null)
            {
                // Set card references via fields approach - use a simple init
                _ventureSelectionUI.SetUIReferences(
                    hud.ventureCards, hud.ventureCardImages,
                    hud.ventureNameTexts, hud.ventureDescTexts,
                    hud.ventureStartButton);
            }

            // 10. Create Tutorial system (after all wiring is complete)
            _tutorialManager = CreateTutorial(hud);

            Debug.Log("[GameSceneBootstrap] 3D scene created successfully.");
        }

        // Tutorial reference kept for Start()
        private TutorialManager _tutorialManager;
        private VentureSelectionUI _ventureSelectionUI;
        private VentureData[] _ventures;

        private void Start()
        {
            // Show venture selection instead of starting immediately
            if (_ventureSelectionUI != null && _ventures != null && _ventures.Length > 0)
            {
                _ventureSelectionUI.Init(_ventures);
                _ventureSelectionUI.OnVentureSelected += OnVentureChosen;
                _ventureSelectionUI.Show();
            }
            else
            {
                // Fallback: start without venture selection
                StartRun(null);
            }
        }

        private void OnVentureChosen(VentureData venture)
        {
            _ventureSelectionUI.OnVentureSelected -= OnVentureChosen;
            StartRun(venture);
        }

        private void StartRun(VentureData venture)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (venture != null)
                gm.SetSelectedVenture(venture);

            gm.StartNewRun();

            if (_tutorialManager != null)
                _tutorialManager.TryStartTutorial();
        }

        // ================================================================
        // Scene-specific setup that doesn't fit a pure static factory
        // ================================================================

        private Camera Setup3DCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
            }

            cam.orthographic = false;
            cam.fieldOfView = 49f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 50f;
            cam.transform.position = new Vector3(0f, 14.3f, -8.6f);
            cam.transform.rotation = Quaternion.Euler(50f, 0f, 0f);
            cam.backgroundColor = ControlDeskTheme.SceneBackground;
            cam.clearFlags = CameraClearFlags.SolidColor;

            if (cam.GetComponent<ScreenShake>() == null)
                cam.gameObject.AddComponent<ScreenShake>();

            if (cam.GetComponent<CameraController>() == null)
                cam.gameObject.AddComponent<CameraController>();

            return cam;
        }

        private Board3D Build3DBoard()
        {
            var boardGo = new GameObject("--- BOARD 3D ---");
            var board3D = boardGo.AddComponent<Board3D>();
            board3D.BuildBoard();
            return board3D;
        }

        private CardFactory CreateCardFactory()
        {
            var factoryGo = new GameObject("CardFactory");
            return factoryGo.AddComponent<CardFactory>();
        }

        private Hand3D Build3DHand(Camera mainCamera, CardFactory cardFactory)
        {
            var handAnchor = new GameObject("HandAnchor");
            // DO NOT parent to camera — use world space on the board surface
            handAnchor.transform.position = new Vector3(0f, 0.92f, -3.30f);
            handAnchor.transform.rotation = Quaternion.Euler(-16f, 0f, 0f);

            var handGo = new GameObject("Hand3D");
            handGo.transform.SetParent(handAnchor.transform);
            handGo.transform.localPosition = Vector3.zero;
            handGo.transform.localRotation = Quaternion.identity;
            var hand3D = handGo.AddComponent<Hand3D>();

            hand3D.Init(cardFactory, null, handAnchor.transform);

            return hand3D;
        }

        private void CreateVFX()
        {
            var vfxRoot = new GameObject("--- VFX ---");
            var poolParent = new GameObject("VFXPool");
            poolParent.transform.SetParent(vfxRoot.transform);
        }

        /// <summary>
        /// Creates the TutorialUI overlay on the HUD canvas and wires
        /// it to a new TutorialManager MonoBehaviour.
        /// </summary>
        private TutorialManager CreateTutorial(HUDBundle hud)
        {
            // Find the HUD canvas (parent of any HUD element)
            Transform canvasParent = hud.uiManager != null
                ? hud.uiManager.transform
                : null;

            if (canvasParent == null)
            {
                Debug.LogWarning("[GameSceneBootstrap] No canvas found for TutorialUI.");
                return null;
            }

            // Build the overlay UI
            var tutorialUI = TutorialUI.Create(canvasParent);

            // Create the manager
            var tutGo = new GameObject("TutorialManager");
            var tutorialManager = tutGo.AddComponent<TutorialManager>();
            tutorialManager.Init(tutorialUI);

            return tutorialManager;
        }

        // ================================================================
        // Lighting & Post-Processing
        // ================================================================

        private void SetupLightingAndPostProcessing()
        {
            // --- Ambient Light ---
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = ControlDeskTheme.AmbientWarm;

            // --- 1. Main Directional Light (Sun) ---
            var sunGo = new GameObject("Directional Light (Sun)");
            var sunLight = sunGo.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(1.0f, 0.92f, 0.82f);
            sunLight.intensity = 1.35f;
            sunLight.shadows = LightShadows.Soft;
            sunGo.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

            // --- 2. Fill Light ---
            var fillGo = new GameObject("Directional Light (Fill)");
            var fillLight = fillGo.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.color = new Color(0.48f, 0.58f, 0.72f);
            fillLight.intensity = 0.42f;
            fillLight.shadows = LightShadows.None;
            fillGo.transform.rotation = Quaternion.Euler(24f, 148f, 0f);

            // --- 3. Point Light on Table Center ---
            var tableLightGo = new GameObject("Point Light (Table)");
            var tableLight = tableLightGo.AddComponent<Light>();
            tableLight.type = LightType.Point;
            tableLight.color = new Color(0.95f, 0.72f, 0.45f);
            tableLight.intensity = 2.4f;
            tableLight.range = 14f;
            tableLight.shadows = LightShadows.Soft;
            tableLightGo.transform.position = new Vector3(0f, 3.6f, 0.8f);

            // --- 4. Global Volume (Post Processing) ---
            var volumeGo = new GameObject("--- POST PROCESSING ---");
            var volume = volumeGo.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            volume.profile = profile;

            // Bloom
            var bloom = profile.Add<Bloom>(overrides: true);
            bloom.intensity.Override(0.3f);
            bloom.threshold.Override(0.9f);
            bloom.scatter.Override(0.65f);

            // Color Adjustments
            var colorAdj = profile.Add<ColorAdjustments>(overrides: true);
            colorAdj.postExposure.Override(0.1f);
            colorAdj.contrast.Override(10f);
            colorAdj.saturation.Override(15f);

            // Vignette
            var vignette = profile.Add<Vignette>(overrides: true);
            vignette.intensity.Override(0.25f);
            vignette.smoothness.Override(0.4f);

            // Tonemapping
            var tonemap = profile.Add<Tonemapping>(overrides: true);
            tonemap.mode.Override(TonemappingMode.ACES);

            // Film Grain
            var grain = profile.Add<FilmGrain>(overrides: true);
            grain.intensity.Override(0.1f);
            grain.type.Override(FilmGrainLookup.Thin1);

            Debug.Log("[GameSceneBootstrap] Lighting & post-processing configured.");
        }
    }
}
