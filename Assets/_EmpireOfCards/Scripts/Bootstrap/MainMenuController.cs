using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace EmpireOfCards.Bootstrap
{
    public class MainMenuController : MonoBehaviour
    {
        static readonly Color COL_GOLD = new Color(0.95f, 0.80f, 0.20f);
        static readonly Color COL_GREEN = new Color(0.20f, 0.70f, 0.25f);
        static readonly Color COL_BLUE = new Color(0.20f, 0.45f, 0.75f);
        static readonly Color COL_RED = new Color(0.75f, 0.20f, 0.20f);
        static readonly Color COL_BG = new Color(0.08f, 0.08f, 0.12f);

        void Awake()
        {
            BuildMenu();
        }

        void BuildMenu()
        {
            // Canvas
            var canvasGo = new GameObject("[MainMenuCanvas]");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            var bgGo = new GameObject("Background");
            bgGo.transform.SetParent(canvasGo.transform, false);
            var bgRect = bgGo.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            var bgImg = bgGo.AddComponent<Image>();
            bgImg.color = COL_BG;

            // Title
            CreateText(canvasGo.transform, "EMPIRE OF CARDS", 64f,
                new Vector2(0f, 200f), COL_GOLD, FontStyles.Bold);

            // Subtitle
            CreateText(canvasGo.transform, "Her kart bir karar. Her karar bir sonuc.", 22f,
                new Vector2(0f, 120f), new Color(0.7f, 0.7f, 0.7f), FontStyles.Italic);

            // Buttons
            CreateMenuButton(canvasGo.transform, "START GAME", new Vector2(0f, 0f),
                COL_GREEN, true, OnStartClicked);

            bool hasSave = PlayerPrefs.HasKey("SaveExists");
            CreateMenuButton(canvasGo.transform, "LOAD GAME", new Vector2(0f, -70f),
                COL_BLUE, hasSave, OnLoadClicked);

            CreateMenuButton(canvasGo.transform, "QUIT", new Vector2(0f, -140f),
                COL_RED, true, OnQuitClicked);

            // Version text
            var verText = CreateText(canvasGo.transform, "v5.0 MVP", 14f,
                Vector2.zero, new Color(0.5f, 0.5f, 0.5f));
            var verRect = verText.GetComponent<RectTransform>();
            verRect.anchorMin = new Vector2(1f, 0f);
            verRect.anchorMax = new Vector2(1f, 0f);
            verRect.pivot = new Vector2(1f, 0f);
            verRect.anchoredPosition = new Vector2(-20f, 10f);
            verRect.sizeDelta = new Vector2(200f, 30f);
            verText.alignment = TextAlignmentOptions.BottomRight;
        }

        void OnStartClicked()
        {
            SceneManager.LoadScene("Game");
        }

        void OnLoadClicked()
        {
            PlayerPrefs.SetInt("LoadFlag", 1);
            SceneManager.LoadScene("Game");
        }

        void OnQuitClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        TextMeshProUGUI CreateText(Transform parent, string text, float size,
            Vector2 pos, Color color, FontStyles style = FontStyles.Normal)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(800f, 80f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = false;
            return tmp;
        }

        void CreateMenuButton(Transform parent, string label, Vector2 pos,
            Color bgColor, bool interactable, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject($"Btn_{label}");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(300f, 50f);

            var img = go.AddComponent<Image>();
            img.color = interactable ? bgColor : bgColor * 0.4f;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.interactable = interactable;
            btn.onClick.AddListener(onClick);

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 20f;
            tmp.color = interactable ? Color.white : new Color(0.5f, 0.5f, 0.5f);
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
        }
    }
}
