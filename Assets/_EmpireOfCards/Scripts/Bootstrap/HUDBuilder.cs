using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.UI;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Builds the entire HUD canvas and UI panels at runtime.
    /// Returns a bundle with all UI references needed for wiring.
    /// </summary>
    public static class HUDBuilder
    {
        /// <summary>
        /// Creates the HUD canvas, all panels, buttons, and popups.
        /// Returns a HUDBundle with every reference the wiring step needs.
        /// </summary>
        public static HUDBundle Build()
        {
            var hud = new HUDBundle();

            // --- Main Canvas (Screen Space Overlay) ---
            var canvasGo = new GameObject("HUDCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // --- UIManager on canvas ---
            hud.uiManager = canvasGo.AddComponent<UIManager>();

            // ============================================================
            // TOP BAR -- money, turn counter, FBI risk
            // ============================================================
            var topBar = CreateUIPanel("TopBar", canvasGo.transform);
            SetAnchors(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            topBar.sizeDelta = new Vector2(0, 80);
            hud.topBarUI = topBar.gameObject.AddComponent<TopBarUI>();

            // Money display
            var moneyObj = CreateTextElement("MoneyDisplay", topBar, "$500", 36);
            moneyObj.localPosition = new Vector3(-700, -40, 0);
            hud.moneyText = moneyObj.GetComponent<TMP_Text>();

            // Turn counter
            var turnObj = CreateTextElement("TurnCounter", topBar, "Turn 1/20", 24);
            turnObj.localPosition = new Vector3(0, -40, 0);
            hud.turnText = turnObj.GetComponent<TMP_Text>();

            // FBI Risk bar
            var fbiBarBg = CreateUIPanel("FBIRiskBar", topBar);
            fbiBarBg.localPosition = new Vector3(700, -40, 0);
            fbiBarBg.sizeDelta = new Vector2(200, 20);
            var fbiBarBgImg = fbiBarBg.GetComponent<Image>();
            fbiBarBgImg.color = new Color(0.2f, 0.2f, 0.2f);

            var fbiBarFill = CreateUIPanel("Fill", fbiBarBg);
            fbiBarFill.sizeDelta = new Vector2(200, 20);
            hud.fbiBarFillImg = fbiBarFill.GetComponent<Image>();
            hud.fbiBarFillImg.color = Color.green;

            // ============================================================
            // ACTION BAR -- action dots
            // ============================================================
            var actionBar = CreateUIPanel("ActionBar", canvasGo.transform);
            SetAnchors(actionBar, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            actionBar.anchoredPosition = new Vector2(0, 200);
            actionBar.sizeDelta = new Vector2(200, 40);
            hud.actionBarUI = actionBar.gameObject.AddComponent<ActionBarUI>();

            // Action dots (3-5)
            hud.actionDotImages = new Image[5];
            for (int i = 0; i < 5; i++)
            {
                var dot = CreateUIPanel($"ActionDot_{i + 1}", actionBar);
                dot.sizeDelta = new Vector2(30, 30);
                dot.localPosition = new Vector3((i - 2) * 35f, 0, 0);
                hud.actionDotImages[i] = dot.GetComponent<Image>();
                hud.actionDotImages[i].color = i < 3 ? Color.green : Color.gray;
                if (i >= 3) dot.gameObject.SetActive(false);
            }

            // ============================================================
            // BUTTONS -- End Turn, Shop, Deck
            // ============================================================

            // End Turn Button
            var endTurnBtn = CreateButton("EndTurnButton", canvasGo.transform, "END TURN");
            SetAnchors(endTurnBtn, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
            endTurnBtn.anchoredPosition = new Vector2(-100, 200);
            endTurnBtn.sizeDelta = new Vector2(160, 50);
            hud.endTurnButton = endTurnBtn.GetComponent<Button>();

            // Shop Button
            var shopBtn = CreateButton("ShopButton", canvasGo.transform, "SHOP");
            SetAnchors(shopBtn, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            shopBtn.anchoredPosition = new Vector2(100, 200);
            shopBtn.sizeDelta = new Vector2(140, 50);
            hud.shopButton = shopBtn.GetComponent<Button>();

            // Deck Button
            var deckBtn = CreateButton("DeckButton", canvasGo.transform, "DECK: 14");
            SetAnchors(deckBtn, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            deckBtn.anchoredPosition = new Vector2(100, 140);
            deckBtn.sizeDelta = new Vector2(140, 40);

            // ============================================================
            // SHOP PANEL (hidden by default)
            // ============================================================
            var shopPanel = CreateUIPanel("ShopPanel", canvasGo.transform);
            shopPanel.anchorMin = new Vector2(0.2f, 0.2f);
            shopPanel.anchorMax = new Vector2(0.8f, 0.8f);
            shopPanel.sizeDelta = Vector2.zero;
            shopPanel.offsetMin = Vector2.zero;
            shopPanel.offsetMax = Vector2.zero;
            shopPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            hud.shopPanel = shopPanel.gameObject.AddComponent<ShopPanel>();
            shopPanel.gameObject.SetActive(false);

            // Shop title
            CreateTextElement("ShopTitle", shopPanel, "SHOP", 32);

            // Shop card slots
            for (int i = 0; i < 3; i++)
            {
                var shopCard = CreateUIPanel($"ShopCard_{i + 1}", shopPanel);
                shopCard.sizeDelta = new Vector2(150, 220);
                shopCard.localPosition = new Vector3((i - 1) * 200f, 0, 0);
                shopCard.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.35f);
            }

            var shopCloseBtn = CreateButton("CloseButton", shopPanel, "CLOSE");
            shopCloseBtn.anchoredPosition = new Vector2(0, -200);
            shopCloseBtn.sizeDelta = new Vector2(120, 40);
            hud.shopCloseButton = shopCloseBtn.GetComponent<Button>();

            // ============================================================
            // POPUP OVERLAYS (hidden by default)
            // ============================================================

            // Combo popup
            var comboPopup = CreateUIPanel("ComboPopup", canvasGo.transform);
            comboPopup.sizeDelta = new Vector2(600, 100);
            hud.comboPopup = comboPopup.gameObject.AddComponent<ComboPopup>();
            comboPopup.gameObject.SetActive(false);

            // Event popup
            var eventPopup = CreateUIPanel("EventPopup", canvasGo.transform);
            eventPopup.sizeDelta = new Vector2(400, 300);
            hud.eventPopup = eventPopup.gameObject.AddComponent<EventPopup>();
            eventPopup.gameObject.SetActive(false);

            // Rival popup
            var rivalPopup = CreateUIPanel("RivalPopup", canvasGo.transform);
            SetAnchors(rivalPopup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            rivalPopup.sizeDelta = new Vector2(500, 200);
            hud.rivalPopup = rivalPopup.gameObject.AddComponent<RivalPopup>();
            rivalPopup.gameObject.SetActive(false);

            // ============================================================
            // SCORE & GAMEOVER OVERLAYS (hidden by default)
            // ============================================================

            // Score overlay
            var scoreOverlay = CreateUIPanel("ScoreOverlay", canvasGo.transform);
            scoreOverlay.anchorMin = Vector2.zero;
            scoreOverlay.anchorMax = Vector2.one;
            scoreOverlay.sizeDelta = Vector2.zero;
            scoreOverlay.offsetMin = Vector2.zero;
            scoreOverlay.offsetMax = Vector2.zero;
            scoreOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);
            hud.scoreScreen = scoreOverlay.gameObject.AddComponent<ScoreScreen>();
            scoreOverlay.gameObject.SetActive(false);

            // GameOver overlay
            var gameOverOverlay = CreateUIPanel("GameOverOverlay", canvasGo.transform);
            gameOverOverlay.anchorMin = Vector2.zero;
            gameOverOverlay.anchorMax = Vector2.one;
            gameOverOverlay.sizeDelta = Vector2.zero;
            gameOverOverlay.offsetMin = Vector2.zero;
            gameOverOverlay.offsetMax = Vector2.zero;
            gameOverOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);
            hud.gameOverScreen = gameOverOverlay.gameObject.AddComponent<GameOverScreen>();
            gameOverOverlay.gameObject.SetActive(false);

            // ============================================================
            // EVENT SYSTEM (required for UI interaction)
            // ============================================================
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            Debug.Log("[HUDBuilder] HUD canvas and all panels created.");

            return hud;
        }

        // ================================================================
        // UI Helper Methods
        // ================================================================

        private static RectTransform CreateUIPanel(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0); // Transparent by default
            return rt;
        }

        private static RectTransform CreateButton(string name, Transform parent, string label)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.5f, 0.3f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var textObj = new GameObject("Label");
            textObj.transform.SetParent(go.transform, false);
            var textRT = textObj.AddComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.sizeDelta = Vector2.zero;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
            var text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 20;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;

            return rt;
        }

        private static RectTransform CreateTextElement(string name, Transform parent, string content, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, 50);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            return rt;
        }

        private static void SetAnchors(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
        }
    }

    /// <summary>
    /// Holds references to all HUD elements created by HUDBuilder.
    /// </summary>
    public class HUDBundle
    {
        // Managers placed on canvas
        public UIManager uiManager;

        // Panel components
        public TopBarUI topBarUI;
        public ActionBarUI actionBarUI;
        public ShopPanel shopPanel;
        public ComboPopup comboPopup;
        public EventPopup eventPopup;
        public RivalPopup rivalPopup;
        public ScoreScreen scoreScreen;
        public GameOverScreen gameOverScreen;

        // TopBar sub-elements
        public TMP_Text moneyText;
        public TMP_Text turnText;
        public Image fbiBarFillImg;

        // ActionBar sub-elements
        public Image[] actionDotImages;

        // Buttons
        public Button endTurnButton;
        public Button shopButton;
        public Button shopCloseButton;
    }
}
