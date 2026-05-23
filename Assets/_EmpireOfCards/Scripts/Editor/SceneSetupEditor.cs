#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EmpireOfCards.Editor
{
    /// <summary>
    /// Editor utility to create the 3 game scenes (Boot, MainMenu, Game)
    /// and register them in Build Settings.
    /// Run via the menu: EmpireOfCards > Create All Scenes
    /// </summary>
    public static class SceneSetupEditor
    {
        [MenuItem("EmpireOfCards/Create All Scenes")]
        public static void CreateAllScenes()
        {
            CreateBootScene();
            CreateMainMenuScene();
            CreateGameScene();

            // Add scenes to build settings
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/Boot.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true),
            };
            EditorBuildSettings.scenes = scenes;

            Debug.Log("[SceneSetup] All 3 scenes created and added to Build Settings!");
        }

        [MenuItem("EmpireOfCards/Create Boot Scene")]
        public static void CreateBootScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Add BootSceneController
            var controllerGo = new GameObject("BootController");
            controllerGo.AddComponent<Bootstrap.BootSceneController>();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Boot.unity");
            Debug.Log("[SceneSetup] Boot.unity created.");
        }

        [MenuItem("EmpireOfCards/Create MainMenu Scene")]
        public static void CreateMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Setup camera
            var cam = Camera.main;
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = 5;
                cam.backgroundColor = new Color(0.08f, 0.06f, 0.12f);
            }

            // Add MainMenuController
            var controllerGo = new GameObject("MainMenuController");
            controllerGo.AddComponent<Bootstrap.MainMenuController>();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
            Debug.Log("[SceneSetup] MainMenu.unity created.");
        }

        [MenuItem("EmpireOfCards/Create Game Scene")]
        public static void CreateGameScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Setup camera
            var cam = Camera.main;
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = 6;
                cam.backgroundColor = new Color(0.15f, 0.12f, 0.1f);
            }

            var stageGo = new GameObject("[BoardStage]");
            var stage = stageGo.AddComponent<EmpireOfCards.World.BoardStageAuthoring>();
            stage.EnsureLayout();

            // Add GameSceneBootstrap
            var bootstrapGo = new GameObject("[GameBootstrap]");
            var bootstrap = bootstrapGo.AddComponent<Bootstrap.GameSceneBootstrap>();
            var serializedBootstrap = new SerializedObject(bootstrap);
            serializedBootstrap.FindProperty("boardStageAuthoring").objectReferenceValue = stage;
            serializedBootstrap.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Game.unity");
            Debug.Log("[SceneSetup] Game.unity created.");
        }

        [MenuItem("EmpireOfCards/Rebuild Game Stage Contract")]
        public static void RebuildGameStageContract()
        {
            var scene = EditorSceneManager.GetActiveScene();
            if (!scene.IsValid())
                return;

            var stageGo = GetOrCreateRoot(scene, "[BoardStage]");
            var stage = stageGo.GetComponent<EmpireOfCards.World.BoardStageAuthoring>();
            if (stage == null)
                stage = stageGo.AddComponent<EmpireOfCards.World.BoardStageAuthoring>();
            stage.EnsureLayout();

            var bootstrapGo = GetOrCreateRoot(scene, "[GameBootstrap]");
            var bootstrap = bootstrapGo.GetComponent<Bootstrap.GameSceneBootstrap>();
            if (bootstrap == null)
                bootstrap = bootstrapGo.AddComponent<Bootstrap.GameSceneBootstrap>();

            var serializedBootstrap = new SerializedObject(bootstrap);
            serializedBootstrap.FindProperty("boardStageAuthoring").objectReferenceValue = stage;
            serializedBootstrap.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("[SceneSetup] Game stage authoring contract rebuilt.");
        }

        private static GameObject GetOrCreateRoot(Scene scene, string objectName)
        {
            var roots = scene.GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i] != null && roots[i].name == objectName)
                    return roots[i];
            }

            return new GameObject(objectName);
        }
    }
}
#endif
