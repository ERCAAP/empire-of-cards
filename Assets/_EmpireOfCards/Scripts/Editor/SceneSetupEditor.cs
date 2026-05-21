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

            // Add GameSceneBootstrap
            var bootstrapGo = new GameObject("[GameBootstrap]");
            bootstrapGo.AddComponent<Bootstrap.GameSceneBootstrap>();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Game.unity");
            Debug.Log("[SceneSetup] Game.unity created.");
        }
    }
}
#endif
