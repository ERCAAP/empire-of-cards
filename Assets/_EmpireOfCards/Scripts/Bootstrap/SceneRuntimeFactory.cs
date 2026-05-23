using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using EmpireOfCards.Data;
using EmpireOfCards.Helpers;
using EmpireOfCards.Presentation;
using EmpireOfCards.VFX;
using EmpireOfCards.World;

namespace EmpireOfCards.Bootstrap
{
    public static class SceneRuntimeFactory
    {
        public static SceneRuntimeBundle Create(BoardStageAuthoring authoredStage = null)
        {
            BoardStageAuthoring stage = ResolveStageAuthoring(authoredStage);
            Camera mainCamera = Setup3DCamera(stage);
            SetupLightingAndPostProcessing();
            Board3D board3D = Build3DBoard(stage);
            CardFactory cardFactory = CreateCardFactory();
            Hand3D hand3D = Build3DHand(cardFactory, stage);
            CreateVFX();

            return new SceneRuntimeBundle(mainCamera, board3D, cardFactory, hand3D, stage);
        }

        private static BoardStageAuthoring ResolveStageAuthoring(BoardStageAuthoring authoredStage)
        {
            if (authoredStage != null)
            {
                authoredStage.EnsureLayout();
                authoredStage.LogMissingReferences();
                return authoredStage;
            }

            var stageGo = new GameObject("[BoardStageAuthoring Runtime Dev]");
            var stage = stageGo.AddComponent<BoardStageAuthoring>();
            stage.EnsureLayout();
            Debug.LogError("[SceneRuntimeFactory] Game.unity has no authored BoardStageAuthoring assigned. Created a runtime development stage so the loop can run, but production scene authoring is still required.");
            return stage;
        }

        private static Camera Setup3DCamera(BoardStageAuthoring stage)
        {
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
            }

            cam.orthographic = false;
            cam.fieldOfView = 40f;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 50f;
            cam.transform.position = new Vector3(0f, 16.6f, -4.6f);
            cam.transform.rotation = Quaternion.Euler(68.5f, 0f, 0f);
            cam.backgroundColor = ControlDeskTheme.SceneBackground;
            cam.clearFlags = CameraClearFlags.SolidColor;

            if (cam.GetComponent<ScreenShake>() == null)
                cam.gameObject.AddComponent<ScreenShake>();

            var controller = cam.GetComponent<CameraController>();
            if (controller == null)
                controller = cam.gameObject.AddComponent<CameraController>();
            controller.ApplyStage(stage);

            return cam;
        }

        private static Board3D Build3DBoard(BoardStageAuthoring stage)
        {
            var boardGo = new GameObject("--- BOARD 3D ---");
            if (stage != null && stage.StageRoot != null)
                boardGo.transform.SetParent(stage.StageRoot, false);

            var board3D = boardGo.AddComponent<Board3D>();
            if (stage != null && stage.HasAuthoredBoardSockets)
                board3D.BindAuthoredStage(stage);
            else
                board3D.BuildBoard(stage);
            return board3D;
        }

        private static CardFactory CreateCardFactory()
        {
            var factoryGo = new GameObject("CardFactory");
            return factoryGo.AddComponent<CardFactory>();
        }

        private static Hand3D Build3DHand(CardFactory cardFactory, BoardStageAuthoring stage)
        {
            Transform buildAnchor = stage != null ? stage.BuildHandAnchor : null;
            Transform responseAnchor = stage != null ? stage.ResponseHandAnchor : null;
            if (buildAnchor == null || responseAnchor == null)
            {
                var handAnchor = new GameObject("HandAnchor");
                handAnchor.transform.position = new Vector3(0f, 0.84f, -3.06f);
                handAnchor.transform.rotation = Quaternion.Euler(-2f, 0f, 0f);
                buildAnchor = handAnchor.transform;
                responseAnchor = handAnchor.transform;
            }

            var handGo = new GameObject("Hand3D");
            handGo.transform.SetParent(stage != null && stage.HandRoot != null ? stage.HandRoot : buildAnchor);
            handGo.transform.localPosition = Vector3.zero;
            handGo.transform.localRotation = Quaternion.identity;
            var hand3D = handGo.AddComponent<Hand3D>();

            // Init() is called later by ManagerReferenceWiring with proper DeckManager
            return hand3D;
        }

        public static void ApplyCameraProfile(Camera camera, Hand3D hand3D, BoardCameraProfile profile)
        {
            if (camera == null || profile == null)
                return;

            camera.fieldOfView = profile.fieldOfView;
            camera.transform.position = profile.cameraPosition;
            camera.transform.rotation = Quaternion.Euler(profile.cameraEuler);
            hand3D?.ApplyPresentationProfile(profile);
        }

        private static void CreateVFX()
        {
            var vfxRoot = new GameObject("--- VFX ---");
            var poolParent = new GameObject("VFXPool");
            poolParent.transform.SetParent(vfxRoot.transform);
        }

        private static void SetupLightingAndPostProcessing()
        {
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = ControlDeskTheme.AmbientWarm;

            var sunGo = new GameObject("Directional Light (Sun)");
            var sunLight = sunGo.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(1.0f, 0.92f, 0.82f);
            sunLight.intensity = 1.35f;
            sunLight.shadows = LightShadows.Soft;
            sunGo.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

            var fillGo = new GameObject("Directional Light (Fill)");
            var fillLight = fillGo.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.color = new Color(0.48f, 0.58f, 0.72f);
            fillLight.intensity = 0.42f;
            fillLight.shadows = LightShadows.None;
            fillGo.transform.rotation = Quaternion.Euler(24f, 148f, 0f);

            var tableLightGo = new GameObject("Point Light (Table)");
            var tableLight = tableLightGo.AddComponent<Light>();
            tableLight.type = LightType.Point;
            tableLight.color = new Color(0.95f, 0.72f, 0.45f);
            tableLight.intensity = 2.4f;
            tableLight.range = 14f;
            tableLight.shadows = LightShadows.Soft;
            tableLightGo.transform.position = new Vector3(0f, 3.6f, 0.8f);

            var volumeGo = new GameObject("--- POST PROCESSING ---");
            var volume = volumeGo.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            volume.profile = profile;

            var bloom = profile.Add<Bloom>(true);
            bloom.intensity.Override(0.3f);
            bloom.threshold.Override(0.9f);

            var vignette = profile.Add<Vignette>(true);
            vignette.intensity.Override(0.18f);
            vignette.smoothness.Override(0.65f);

            var colorAdjustments = profile.Add<ColorAdjustments>(true);
            colorAdjustments.postExposure.Override(0.08f);
            colorAdjustments.contrast.Override(8f);
            colorAdjustments.saturation.Override(-6f);

            var tonemapping = profile.Add<Tonemapping>(true);
            tonemapping.mode.Override(TonemappingMode.ACES);
        }
    }
}
