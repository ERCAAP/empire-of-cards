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

            // Neglect warning (shows briefly when a business is neglected)
            var neglectWarning = CreateTextElement("NeglectWarning", topBar, "", 18);
            neglectWarning.anchoredPosition = new Vector2(0, -65);
            neglectWarning.sizeDelta = new Vector2(400, 30);
            neglectWarning.GetComponent<TMP_Text>().color = new Color(1f, 0.5f, 0.2f); // Orange warning
            hud.neglectWarningText = neglectWarning.GetComponent<TMP_Text>();

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

            // Shop bias indicator
            var biasText = CreateTextElement("BiasIndicator", shopPanel, "", 14);
            biasText.anchoredPosition = new Vector2(0, 120);
            biasText.sizeDelta = new Vector2(300, 25);
            biasText.GetComponent<TMP_Text>().color = new Color(0.7f, 0.9f, 0.7f);
            hud.shopBiasText = biasText.GetComponent<TMP_Text>();

            // Shop card slots -- full card UI with name, cost, and buy button
            var shopCardUIs = new EmpireOfCards.UI.Cards.CardUI[3];
            var shopPriceTexts = new TMP_Text[3];
            var shopBuyButtons = new Button[3];

            for (int i = 0; i < 3; i++)
            {
                // Card container
                var shopCard = CreateUIPanel($"ShopCard_{i + 1}", shopPanel);
                shopCard.sizeDelta = new Vector2(160, 220);
                shopCard.localPosition = new Vector3((i - 1) * 200f, -10, 0);
                var cardBgImg = shopCard.GetComponent<Image>();
                cardBgImg.color = new Color(0.25f, 0.25f, 0.3f);

                // CanvasGroup for affordability dimming
                shopCard.gameObject.AddComponent<CanvasGroup>();

                // CardUI component -- wire its background reference
                var cardUI = shopCard.gameObject.AddComponent<EmpireOfCards.UI.Cards.CardUI>();
                shopCardUIs[i] = cardUI;

                // Card name text (top of card)
                var nameRt = CreateTextElement("CardName", shopCard, "", 18);
                nameRt.anchoredPosition = new Vector2(0, 70);
                nameRt.sizeDelta = new Vector2(140, 50);
                nameRt.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

                // Card description / stats (middle)
                var descRt = CreateTextElement("CardDesc", shopCard, "", 12);
                descRt.anchoredPosition = new Vector2(0, 15);
                descRt.sizeDelta = new Vector2(140, 60);
                descRt.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
                descRt.GetComponent<TMP_Text>().color = new Color(0.8f, 0.8f, 0.8f);

                // Price text (below description)
                var priceRt = CreateTextElement("PriceText", shopCard, "$0", 22);
                priceRt.anchoredPosition = new Vector2(0, -35);
                priceRt.sizeDelta = new Vector2(140, 30);
                priceRt.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
                priceRt.GetComponent<TMP_Text>().color = new Color(1f, 0.9f, 0.3f);
                shopPriceTexts[i] = priceRt.GetComponent<TMP_Text>();

                // Buy button (bottom of card)
                var buyBtn = CreateButton("BuyButton", shopCard, "BUY");
                buyBtn.anchoredPosition = new Vector2(0, -80);
                buyBtn.sizeDelta = new Vector2(100, 35);
                buyBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 0.3f);
                shopBuyButtons[i] = buyBtn.GetComponent<Button>();

                // Wire CardUI serialized fields via reflection-free approach:
                // CardUI.SetupCard() uses nameText, costText, descriptionText, cardBackground.
                // We set them through a lightweight init helper.
                cardUI.InitShopSlot(cardBgImg, nameRt.GetComponent<TMP_Text>(),
                    priceRt.GetComponent<TMP_Text>(), descRt.GetComponent<TMP_Text>());
            }

            // Pass slot references into ShopPanel
            hud.shopPanel.SetSlotReferences(shopCardUIs, shopPriceTexts, shopBuyButtons);

            var shopCloseBtn = CreateButton("CloseButton", shopPanel, "CLOSE");
            shopCloseBtn.anchoredPosition = new Vector2(0, -200);
            shopCloseBtn.sizeDelta = new Vector2(120, 40);
            hud.shopCloseButton = shopCloseBtn.GetComponent<Button>();

            // ============================================================
            // POPUP OVERLAYS (hidden by default)
            // ============================================================

            // Combo popup (CanvasGroup + text for animated show/fade)
            var comboPopup = CreateUIPanel("ComboPopup", canvasGo.transform);
            SetAnchors(comboPopup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            comboPopup.sizeDelta = new Vector2(600, 100);
            var comboCg = comboPopup.gameObject.AddComponent<CanvasGroup>();
            comboCg.alpha = 0f;
            hud.comboPopup = comboPopup.gameObject.AddComponent<ComboPopup>();

            var comboTextRt = CreateTextElement("ComboText", comboPopup, "", 40);
            comboTextRt.anchorMin = Vector2.zero;
            comboTextRt.anchorMax = Vector2.one;
            comboTextRt.sizeDelta = Vector2.zero;
            comboTextRt.offsetMin = Vector2.zero;
            comboTextRt.offsetMax = Vector2.zero;

            hud.comboPopup.SetReferences(comboTextRt.GetComponent<TMP_Text>(), comboCg);

            // Event popup (CanvasGroup for fade animation)
            var eventPopup = CreateUIPanel("EventPopup", canvasGo.transform);
            SetAnchors(eventPopup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            eventPopup.sizeDelta = new Vector2(400, 300);
            var eventCg = eventPopup.gameObject.AddComponent<CanvasGroup>();
            eventCg.alpha = 0f;
            hud.eventPopup = eventPopup.gameObject.AddComponent<EventPopup>();
            hud.eventPopup.SetReferences(eventCg);

            // Rival popup (CanvasGroup + text elements for fade animation)
            var rivalPopup = CreateUIPanel("RivalPopup", canvasGo.transform);
            SetAnchors(rivalPopup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            rivalPopup.sizeDelta = new Vector2(500, 200);
            rivalPopup.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
            var rivalCg = rivalPopup.gameObject.AddComponent<CanvasGroup>();
            rivalCg.alpha = 0f;
            hud.rivalPopup = rivalPopup.gameObject.AddComponent<RivalPopup>();

            var rivalActionRt = CreateTextElement("RivalActionText", rivalPopup, "", 26);
            rivalActionRt.anchoredPosition = new Vector2(0, 30);
            rivalActionRt.sizeDelta = new Vector2(460, 40);

            var rivalTauntRt = CreateTextElement("RivalTauntText", rivalPopup, "", 20);
            rivalTauntRt.anchoredPosition = new Vector2(0, -30);
            rivalTauntRt.sizeDelta = new Vector2(460, 40);
            rivalTauntRt.GetComponent<TMP_Text>().fontStyle = FontStyles.Italic;
            rivalTauntRt.GetComponent<TMP_Text>().color = new Color(0.8f, 0.7f, 0.5f);

            hud.rivalPopup.SetReferences(
                rivalActionRt.GetComponent<TMP_Text>(),
                rivalTauntRt.GetComponent<TMP_Text>(),
                rivalCg);

            // Tier promotion popup
            var tierPopupRt = CreateUIPanel("TierPopup", canvasGo.transform);
            SetAnchors(tierPopupRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            tierPopupRt.sizeDelta = new Vector2(600, 200);
            var tierCg = tierPopupRt.gameObject.AddComponent<CanvasGroup>();
            tierCg.alpha = 0f;
            var tierPopup = tierPopupRt.gameObject.AddComponent<TierPopup>();

            var tierTitleText = CreateTextElement("TierTitle", tierPopupRt, "GİRİŞİMCİ", 48);
            tierTitleText.anchoredPosition = new Vector2(0, 30);
            tierTitleText.sizeDelta = new Vector2(500, 60);

            var tierSubText = CreateTextElement("TierSubtitle", tierPopupRt, "", 22);
            tierSubText.anchoredPosition = new Vector2(0, -30);
            tierSubText.sizeDelta = new Vector2(500, 40);

            tierPopup.SetReferences(
                tierTitleText.GetComponent<TMP_Text>(),
                tierSubText.GetComponent<TMP_Text>(),
                tierCg);
            hud.tierPopup = tierPopup;

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
            // VENTURE SELECTION PANEL (shown at game start, hidden after)
            // ============================================================
            var venturePanel = CreateUIPanel("VentureSelectionPanel", canvasGo.transform);
            venturePanel.anchorMin = Vector2.zero;
            venturePanel.anchorMax = Vector2.one;
            venturePanel.sizeDelta = Vector2.zero;
            venturePanel.offsetMin = Vector2.zero;
            venturePanel.offsetMax = Vector2.zero;
            venturePanel.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.95f);

            // Title
            var ventureTitle = CreateTextElement("VentureTitle", venturePanel, "İLK GİRİŞİMİNİ SEÇ", 42);
            ventureTitle.anchoredPosition = new Vector2(0, 350);
            ventureTitle.sizeDelta = new Vector2(800, 60);

            // 4 Venture cards in a row
            var ventureCards = new RectTransform[4];
            var ventureCardImages = new Image[4];
            var ventureNameTexts = new TMP_Text[4];
            var ventureDescTexts = new TMP_Text[4];
            string[] ventureNames = { "🍔 BÜFE", "💻 TECH STARTUP", "📢 REKLAM AJANSI", "🕶️ KARANLIK PAZAR" };
            string[] ventureDescs = {
                "Büfe tahtada hazır.\nDestene +1 Şef.\nFood combo'lara yön verir.",
                "Tech Startup tahtada hazır.\nDestene +1 Hacker.\nGeç oyun gücü.",
                "Reklam Ajansı tahtada hazır.\nDestene +1 Marketing Guru.\nMüşteri odaklı.",
                "İşletme yok, +200 ekstra para.\nDestene +1 Dolandırıcı.\nRiskli ama hızlı."
            };
            Color[] ventureColors = {
                new Color(0.9f, 0.5f, 0.2f),
                new Color(0.3f, 0.6f, 0.9f),
                new Color(0.9f, 0.3f, 0.5f),
                new Color(0.4f, 0.4f, 0.4f)
            };

            for (int i = 0; i < 4; i++)
            {
                var card = CreateUIPanel($"VentureCard_{i}", venturePanel);
                card.sizeDelta = new Vector2(320, 400);
                card.anchoredPosition = new Vector2((i - 1.5f) * 340f, -20);
                var cardImg = card.GetComponent<Image>();
                cardImg.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);
                card.gameObject.AddComponent<Button>().targetGraphic = cardImg;

                // Accent bar at top of card
                var accent = CreateUIPanel("Accent", card);
                SetAnchors(accent, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
                accent.sizeDelta = new Vector2(0, 8);
                accent.offsetMin = new Vector2(0, 0);
                accent.offsetMax = new Vector2(0, 0);
                accent.anchoredPosition = new Vector2(0, 0);
                accent.GetComponent<Image>().color = ventureColors[i];

                // Name text
                var nameText = CreateTextElement("Name", card, ventureNames[i], 26);
                nameText.anchoredPosition = new Vector2(0, 140);
                nameText.sizeDelta = new Vector2(280, 40);

                // Description text
                var descText = CreateTextElement("Desc", card, ventureDescs[i], 16);
                descText.anchoredPosition = new Vector2(0, 0);
                descText.sizeDelta = new Vector2(280, 200);
                descText.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

                ventureCards[i] = card;
                ventureCardImages[i] = cardImg;
                ventureNameTexts[i] = nameText.GetComponent<TMP_Text>();
                ventureDescTexts[i] = descText.GetComponent<TMP_Text>();
            }

            // START button (disabled until selection)
            var ventureStartBtn = CreateButton("VentureStartButton", venturePanel, "BAŞLA");
            ventureStartBtn.anchoredPosition = new Vector2(0, -280);
            ventureStartBtn.sizeDelta = new Vector2(200, 60);
            ventureStartBtn.GetComponent<Image>().color = new Color(0.2f, 0.5f, 0.3f);
            ventureStartBtn.GetComponent<Button>().interactable = false;

            // Add VentureSelectionUI component and assign references
            var ventureUI = venturePanel.gameObject.AddComponent<VentureSelectionUI>();
            // We need to assign via serialized fields - use reflection-free Init pattern
            hud.ventureSelectionUI = ventureUI;
            hud.ventureCards = ventureCards;
            hud.ventureCardImages = ventureCardImages;
            hud.ventureNameTexts = ventureNameTexts;
            hud.ventureDescTexts = ventureDescTexts;
            hud.ventureStartButton = ventureStartBtn.GetComponent<Button>();

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
        public TierPopup tierPopup;
        public ScoreScreen scoreScreen;
        public GameOverScreen gameOverScreen;

        // TopBar sub-elements
        public TMP_Text moneyText;
        public TMP_Text turnText;
        public Image fbiBarFillImg;
        public TMP_Text neglectWarningText;
        public TMP_Text shopBiasText;

        // ActionBar sub-elements
        public Image[] actionDotImages;

        // Buttons
        public Button endTurnButton;
        public Button shopButton;
        public Button shopCloseButton;

        // Venture Selection
        public VentureSelectionUI ventureSelectionUI;
        public RectTransform[] ventureCards;
        public Image[] ventureCardImages;
        public TMP_Text[] ventureNameTexts;
        public TMP_Text[] ventureDescTexts;
        public Button ventureStartButton;
    }
}
