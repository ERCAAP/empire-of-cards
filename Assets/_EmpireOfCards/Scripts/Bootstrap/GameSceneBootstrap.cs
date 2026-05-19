using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.World;
using EmpireOfCards.UI;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.VFX;
using EmpireOfCards.Helpers;
using EmpireOfCards.Data;

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
            cam.fieldOfView = 45f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 50f;
            cam.transform.position = new Vector3(0f, 8.5f, -5.5f);
            cam.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
            cam.backgroundColor = new Color(0.15f, 0.12f, 0.1f);
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
            handAnchor.transform.SetParent(mainCamera.transform);
            handAnchor.transform.localPosition = new Vector3(0f, -1.2f, 4f);
            handAnchor.transform.localRotation = Quaternion.Euler(-50f, 0f, 0f);

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
    }
}
