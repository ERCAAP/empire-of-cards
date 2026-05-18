using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Main menu controller. Builds the menu UI programmatically on Start().
    /// Place on a GameObject in MainMenu.unity.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameScene = "Game";

        private void Start()
        {
            CreateMenuUI();
        }

        private void CreateMenuUI()
        {
            // Canvas
            var canvasGo = new GameObject("MenuCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            var bg = new GameObject("Background");
            bg.transform.SetParent(canvasGo.transform, false);
            var bgRT = bg.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.sizeDelta = Vector2.zero;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.08f, 0.06f, 0.12f);

            // Title
            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(canvasGo.transform, false);
            var titleRT = titleGo.AddComponent<RectTransform>();
            titleRT.anchoredPosition = new Vector2(0, 150);
            titleRT.sizeDelta = new Vector2(800, 120);
            var titleText = titleGo.AddComponent<TextMeshProUGUI>();
            titleText.text = "EMPIRE OF CARDS";
            titleText.fontSize = 72;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = new Color(1f, 0.85f, 0.4f); // Gold

            // Subtitle
            var subGo = new GameObject("Subtitle");
            subGo.transform.SetParent(canvasGo.transform, false);
            var subRT = subGo.AddComponent<RectTransform>();
            subRT.anchoredPosition = new Vector2(0, 70);
            subRT.sizeDelta = new Vector2(600, 40);
            var subText = subGo.AddComponent<TextMeshProUGUI>();
            subText.text = "Build businesses with cards, defeat your rival.";
            subText.fontSize = 24;
            subText.alignment = TextAlignmentOptions.Center;
            subText.color = new Color(0.7f, 0.7f, 0.7f);

            // Start Button
            var startBtnGo = new GameObject("StartButton");
            startBtnGo.transform.SetParent(canvasGo.transform, false);
            var startRT = startBtnGo.AddComponent<RectTransform>();
            startRT.anchoredPosition = new Vector2(0, -50);
            startRT.sizeDelta = new Vector2(300, 60);
            var startImg = startBtnGo.AddComponent<Image>();
            startImg.color = new Color(0.2f, 0.6f, 0.3f);
            var startBtn = startBtnGo.AddComponent<Button>();
            startBtn.targetGraphic = startImg;
            startBtn.onClick.AddListener(OnStartClicked);

            var startLabelGo = new GameObject("Label");
            startLabelGo.transform.SetParent(startBtnGo.transform, false);
            var startLabelRT = startLabelGo.AddComponent<RectTransform>();
            startLabelRT.anchorMin = Vector2.zero;
            startLabelRT.anchorMax = Vector2.one;
            startLabelRT.sizeDelta = Vector2.zero;
            startLabelRT.offsetMin = Vector2.zero;
            startLabelRT.offsetMax = Vector2.zero;
            var startLabel = startLabelGo.AddComponent<TextMeshProUGUI>();
            startLabel.text = "START GAME";
            startLabel.fontSize = 28;
            startLabel.alignment = TextAlignmentOptions.Center;
            startLabel.color = Color.white;

            // Quit Button
            var quitBtnGo = new GameObject("QuitButton");
            quitBtnGo.transform.SetParent(canvasGo.transform, false);
            var quitRT = quitBtnGo.AddComponent<RectTransform>();
            quitRT.anchoredPosition = new Vector2(0, -140);
            quitRT.sizeDelta = new Vector2(200, 50);
            var quitImg = quitBtnGo.AddComponent<Image>();
            quitImg.color = new Color(0.5f, 0.2f, 0.2f);
            var quitBtn = quitBtnGo.AddComponent<Button>();
            quitBtn.targetGraphic = quitImg;
            quitBtn.onClick.AddListener(OnQuitClicked);

            var quitLabelGo = new GameObject("QuitLabel");
            quitLabelGo.transform.SetParent(quitBtnGo.transform, false);
            var quitLabelRT = quitLabelGo.AddComponent<RectTransform>();
            quitLabelRT.anchorMin = Vector2.zero;
            quitLabelRT.anchorMax = Vector2.one;
            quitLabelRT.sizeDelta = Vector2.zero;
            quitLabelRT.offsetMin = Vector2.zero;
            quitLabelRT.offsetMax = Vector2.zero;
            var quitLabel = quitLabelGo.AddComponent<TextMeshProUGUI>();
            quitLabel.text = "QUIT";
            quitLabel.fontSize = 22;
            quitLabel.alignment = TextAlignmentOptions.Center;
            quitLabel.color = Color.white;

            // Version
            var verGo = new GameObject("Version");
            verGo.transform.SetParent(canvasGo.transform, false);
            var verRT = verGo.AddComponent<RectTransform>();
            verRT.anchorMin = new Vector2(1, 0);
            verRT.anchorMax = new Vector2(1, 0);
            verRT.pivot = new Vector2(1, 0);
            verRT.anchoredPosition = new Vector2(-20, 10);
            verRT.sizeDelta = new Vector2(200, 30);
            var verText = verGo.AddComponent<TextMeshProUGUI>();
            verText.text = "v0.1 MVP";
            verText.fontSize = 16;
            verText.alignment = TextAlignmentOptions.Right;
            verText.color = new Color(0.4f, 0.4f, 0.4f);

            // EventSystem
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var esGo = new GameObject("EventSystem");
                esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        private void OnStartClicked()
        {
            SceneManager.LoadScene(gameScene);
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
