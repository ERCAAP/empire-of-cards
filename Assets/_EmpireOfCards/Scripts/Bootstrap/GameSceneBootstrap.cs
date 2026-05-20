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
using EmpireOfCards.Save;
using TMPro;
using UnityEngine.UI;

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
        private TutorialManager _tutorialManager;
        private VentureSelectionUI _ventureSelectionUI;
        private VentureData[] _ventures;
        private Board3D _board3D;
        private GameDataBundle _data;
        private GameObject _runNamePanel;
        private TMP_InputField _runNameInput;
        private TMP_Text _runNameTitleText;
        private VentureData _pendingVenture;
        private string _pendingRunName;

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
            _data = data;

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
            CreateRunNamePrompt(hud);

            Debug.Log("[GameSceneBootstrap] 3D scene created successfully.");
        }

        private void Start()
        {
            if (RunLaunchConfig.LaunchMode == RunLaunchMode.LoadRun)
            {
                TryRestoreSavedRun();
                return;
            }

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
            ShowRunNamePrompt(venture);
        }

        private void StartRun(VentureData venture)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            if (venture != null)
                gm.SetSelectedVenture(venture);
            if (!string.IsNullOrWhiteSpace(_pendingRunName))
                gm.SetRunDisplayName(_pendingRunName);

            gm.StartNewRun();
            _pendingRunName = null;

            if (_tutorialManager != null)
                _tutorialManager.TryStartTutorial();
        }

        private void TryRestoreSavedRun()
        {
            var gm = GameManager.Instance;
            var save = SaveManager.Instance;
            if (gm == null || save == null || !save.HasRunSave())
            {
                RunLaunchConfig.PrepareNewRun();
                StartRun(null);
                return;
            }

            RunSaveData run = save.LoadRun();
            VentureData venture = FindVenture((VentureType)run.ventureType);
            if (venture == null)
            {
                RunLaunchConfig.PrepareNewRun();
                StartRun(null);
                return;
            }

            gm.SetSelectedVenture(venture);
            gm.SetRunDisplayName(run.runName);
            gm.RestoreRunCheckpoint(run);
            _board3D?.RefreshSlotOccupancyVisuals();
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
            _board3D = board3D;
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
            handAnchor.transform.position = new Vector3(0f, 1.02f, -3.55f);
            handAnchor.transform.rotation = Quaternion.Euler(-12f, 0f, 0f);

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

        private void CreateRunNamePrompt(HUDBundle hud)
        {
            Transform canvasParent = hud.uiManager != null ? hud.uiManager.transform : null;
            if (canvasParent == null)
                return;

            _runNamePanel = new GameObject("RunNamePanel");
            _runNamePanel.transform.SetParent(canvasParent, false);
            var panelRt = _runNamePanel.AddComponent<RectTransform>();
            panelRt.anchorMin = Vector2.zero;
            panelRt.anchorMax = Vector2.one;
            panelRt.offsetMin = Vector2.zero;
            panelRt.offsetMax = Vector2.zero;
            var bg = _runNamePanel.AddComponent<Image>();
            bg.color = new Color(0.03f, 0.03f, 0.08f, 0.94f);

            var titleGo = new GameObject("RunNameTitle");
            titleGo.transform.SetParent(_runNamePanel.transform, false);
            var titleRt = titleGo.AddComponent<RectTransform>();
            titleRt.anchoredPosition = new Vector2(0f, 110f);
            titleRt.sizeDelta = new Vector2(700f, 60f);
            _runNameTitleText = titleGo.AddComponent<TextMeshProUGUI>();
            _runNameTitleText.fontSize = 36;
            _runNameTitleText.alignment = TextAlignmentOptions.Center;
            _runNameTitleText.color = Color.white;

            var inputRoot = new GameObject("RunNameInput");
            inputRoot.transform.SetParent(_runNamePanel.transform, false);
            var inputRt = inputRoot.AddComponent<RectTransform>();
            inputRt.anchoredPosition = new Vector2(0f, 20f);
            inputRt.sizeDelta = new Vector2(560f, 64f);
            var inputBg = inputRoot.AddComponent<Image>();
            inputBg.color = new Color(0.14f, 0.14f, 0.18f, 1f);

            _runNameInput = inputRoot.AddComponent<TMP_InputField>();

            var viewportGo = new GameObject("Viewport");
            viewportGo.transform.SetParent(inputRoot.transform, false);
            var viewportRt = viewportGo.AddComponent<RectTransform>();
            viewportRt.anchorMin = Vector2.zero;
            viewportRt.anchorMax = Vector2.one;
            viewportRt.offsetMin = new Vector2(18f, 10f);
            viewportRt.offsetMax = new Vector2(-18f, -10f);
            viewportGo.AddComponent<RectMask2D>();

            var placeholderGo = new GameObject("Placeholder");
            placeholderGo.transform.SetParent(viewportGo.transform, false);
            var placeholderRt = placeholderGo.AddComponent<RectTransform>();
            placeholderRt.anchorMin = Vector2.zero;
            placeholderRt.anchorMax = Vector2.one;
            placeholderRt.offsetMin = Vector2.zero;
            placeholderRt.offsetMax = Vector2.zero;
            var placeholder = placeholderGo.AddComponent<TextMeshProUGUI>();
            placeholder.text = "Enter a company or app name";
            placeholder.fontSize = 24;
            placeholder.color = new Color(0.62f, 0.62f, 0.68f);
            placeholder.alignment = TextAlignmentOptions.MidlineLeft;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(viewportGo.transform, false);
            var textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;
            var inputText = textGo.AddComponent<TextMeshProUGUI>();
            inputText.fontSize = 24;
            inputText.color = Color.white;
            inputText.alignment = TextAlignmentOptions.MidlineLeft;

            _runNameInput.textViewport = viewportRt;
            _runNameInput.textComponent = inputText;
            _runNameInput.placeholder = placeholder;

            var buttonGo = new GameObject("RunNameConfirm");
            buttonGo.transform.SetParent(_runNamePanel.transform, false);
            var buttonRt = buttonGo.AddComponent<RectTransform>();
            buttonRt.anchoredPosition = new Vector2(0f, -90f);
            buttonRt.sizeDelta = new Vector2(260f, 58f);
            var buttonBg = buttonGo.AddComponent<Image>();
            buttonBg.color = new Color(0.20f, 0.56f, 0.34f, 1f);
            var button = buttonGo.AddComponent<Button>();
            button.targetGraphic = buttonBg;
            button.onClick.AddListener(ConfirmRunName);

            var buttonLabelGo = new GameObject("Label");
            buttonLabelGo.transform.SetParent(buttonGo.transform, false);
            var buttonLabelRt = buttonLabelGo.AddComponent<RectTransform>();
            buttonLabelRt.anchorMin = Vector2.zero;
            buttonLabelRt.anchorMax = Vector2.one;
            buttonLabelRt.offsetMin = Vector2.zero;
            buttonLabelRt.offsetMax = Vector2.zero;
            var buttonLabel = buttonLabelGo.AddComponent<TextMeshProUGUI>();
            buttonLabel.text = "START RUN";
            buttonLabel.fontSize = 26;
            buttonLabel.color = Color.white;
            buttonLabel.alignment = TextAlignmentOptions.Center;

            _runNamePanel.SetActive(false);
        }

        private void ShowRunNamePrompt(VentureData venture)
        {
            if (_runNamePanel == null)
            {
                StartRun(venture);
                return;
            }

            _pendingVenture = venture;
            _runNameTitleText.text = $"Name your {GetNamingLabel(venture)}";
            _runNameInput.text = venture != null ? venture.ventureName : "New Venture";
            _runNamePanel.SetActive(true);
            _runNameInput.ActivateInputField();
        }

        private void ConfirmRunName()
        {
            if (_pendingVenture == null)
                return;

            string enteredName = _runNameInput != null ? _runNameInput.text : string.Empty;
            if (string.IsNullOrWhiteSpace(enteredName))
                enteredName = _pendingVenture.ventureName;

            _pendingRunName = enteredName;
            _runNamePanel.SetActive(false);

            VentureData venture = _pendingVenture;
            _pendingVenture = null;
            StartRun(venture);
        }

        private VentureData FindVenture(VentureType type)
        {
            if (_data == null || _data.ventures == null)
                return null;

            for (int i = 0; i < _data.ventures.Length; i++)
            {
                var venture = _data.ventures[i];
                if (venture != null && venture.ventureType == type)
                    return venture;
            }

            return null;
        }

        private static string GetNamingLabel(VentureData venture)
        {
            if (venture == null)
                return "venture";

            return venture.ventureType == VentureType.TechApp ? "app" : "company";
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
