using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace EmpireOfCards.Core
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField] private string menuScene = "MainMenu";
        [SerializeField] private string gameScene = "Game";

        private bool _isLoading;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadMainMenu() { if (!_isLoading) StartCoroutine(LoadSceneAsync(menuScene)); }
        public void LoadGame() { if (!_isLoading) StartCoroutine(LoadSceneAsync(gameScene)); }
        public void RestartGame() { if (!_isLoading) StartCoroutine(LoadSceneAsync(gameScene)); }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            _isLoading = true;
            EventBus.GameStateChanged(GameState.Boot);

            var op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone)
                yield return null;

            _isLoading = false;
        }
    }
}
