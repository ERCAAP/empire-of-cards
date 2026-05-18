using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.World;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.VFX;

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

            Debug.Log("[GameSceneBootstrap] 3D scene created successfully.");
        }

        private void Start()
        {
            GameManager.Instance?.StartNewRun();
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
            cam.fieldOfView = 60f;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 100f;
            cam.transform.position = new Vector3(0f, 10f, -5f);
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
            handAnchor.transform.localPosition = new Vector3(0f, -1.5f, 5f);
            handAnchor.transform.localRotation = Quaternion.Euler(-55f, 0f, 0f);

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
    }
}
