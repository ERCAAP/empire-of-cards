using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.UI;
using EmpireOfCards.UI.Indicators;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Builds the entire HUD canvas and UI panels at runtime.
    /// Returns a bundle with all UI references needed for wiring.
    /// </summary>
    public static class HUDBuilder
    {
        // HUD color palette
        private static readonly Color GoldColor = new Color(1f, 0.85f, 0.2f);
        private static readonly Color LightGold = new Color(1f, 0.9f, 0.3f);
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

            // Money display - large gold text for emphasis
            var moneyObj = CreateTextElement("MoneyDisplay", topBar, "$500", 42);
            moneyObj.localPosition = new Vector3(-700, -40, 0);
            moneyObj.sizeDelta = new Vector2(280, 60);
            hud.moneyText = moneyObj.GetComponent<TMP_Text>();
            hud.moneyText.color = GoldColor;
            hud.moneyText.fontStyle = FontStyles.Bold;

            // Turn counter - clear and readable
            var turnObj = CreateTextElement("TurnCounter", topBar, "Turn 1/20", 28);
            turnObj.localPosition = new Vector3(0, -40, 0);
            turnObj.sizeDelta = new Vector2(240, 50);
            hud.turnText = turnObj.GetComponent<TMP_Text>();
            hud.turnText.fontStyle = FontStyles.Bold;

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
            // PLATFORM RATING -- 1.0-5.0 star bar, top bar right area
            // ============================================================
            var platformRatingPanel = CreateUIPanel("PlatformRatingPanel", topBar);
            platformRatingPanel.localPosition = new Vector3(450, -40, 0);
            platformRatingPanel.sizeDelta = new Vector2(160, 40);

            var platformLabelRt = CreateTextElement("PlatformLabel", platformRatingPanel, "RATING", 12);
            platformLabelRt.anchoredPosition = new Vector2(0, 16);
            platformLabelRt.sizeDelta = new Vector2(160, 18);
            platformLabelRt.GetComponent<TMP_Text>().color = new Color(0.7f, 0.7f, 0.7f);

            var platformBarBg = CreateUIPanel("PlatformBarBg", platformRatingPanel);
            platformBarBg.anchoredPosition = new Vector2(0, -4);
            platformBarBg.sizeDelta = new Vector2(140, 12);
            platformBarBg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);

            var platformBarFill = CreateUIPanel("PlatformBarFill", platformBarBg);
            platformBarFill.anchorMin = new Vector2(0, 0);
            platformBarFill.anchorMax = new Vector2(0, 1);
            platformBarFill.pivot = new Vector2(0, 0.5f);
            platformBarFill.sizeDelta = new Vector2(70f, 0); // 50% default (3.0/5.0)
            platformBarFill.anchoredPosition = Vector2.zero;
            hud.platformRatingBarFill = platformBarFill.GetComponent<Image>();
            hud.platformRatingBarFill.color = new Color(0.3f, 0.75f, 1f);

            var platformValueRt = CreateTextElement("PlatformValue", platformRatingPanel, "3.0", 18);
            platformValueRt.anchoredPosition = new Vector2(0, -18);
            platformValueRt.sizeDelta = new Vector2(160, 22);
            hud.platformRatingText = platformValueRt.GetComponent<TMP_Text>();
            hud.platformRatingText.color = new Color(0.3f, 0.75f, 1f);
            hud.platformRatingText.fontStyle = FontStyles.Bold;

            var platformIndicator = topBar.gameObject.AddComponent<PlatformRatingIndicator>();
            platformIndicator.Init(hud.platformRatingBarFill, hud.platformRatingText);
            hud.platformRatingIndicator = platformIndicator;

            // ============================================================
            // LEGAL RISK BAR -- 0-100, color-coded, top bar right
            // ============================================================
            var legalRiskPanel = CreateUIPanel("LegalRiskPanel", topBar);
            legalRiskPanel.localPosition = new Vector3(620, -40, 0);
            legalRiskPanel.sizeDelta = new Vector2(160, 40);

            var legalLabelRt = CreateTextElement("LegalLabel", legalRiskPanel, "LEGAL RISK", 12);
            legalLabelRt.anchoredPosition = new Vector2(0, 16);
            legalLabelRt.sizeDelta = new Vector2(160, 18);
            legalLabelRt.GetComponent<TMP_Text>().color = new Color(0.7f, 0.7f, 0.7f);

            var legalBarBg = CreateUIPanel("LegalBarBg", legalRiskPanel);
            legalBarBg.anchoredPosition = new Vector2(0, -4);
            legalBarBg.sizeDelta = new Vector2(140, 12);
            legalBarBg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);

            var legalBarFill = CreateUIPanel("LegalBarFill", legalBarBg);
            legalBarFill.anchorMin = new Vector2(0, 0);
            legalBarFill.anchorMax = new Vector2(0, 1);
            legalBarFill.pivot = new Vector2(0, 0.5f);
            legalBarFill.sizeDelta = new Vector2(0f, 0); // 0% default
            legalBarFill.anchoredPosition = Vector2.zero;
            hud.legalRiskBarFill = legalBarFill.GetComponent<Image>();
            hud.legalRiskBarFill.color = new Color(0.2f, 0.85f, 0.35f); // Green at start

            var legalValueRt = CreateTextElement("LegalValue", legalRiskPanel, "0", 18);
            legalValueRt.anchoredPosition = new Vector2(0, -18);
            legalValueRt.sizeDelta = new Vector2(160, 22);
            hud.legalRiskText = legalValueRt.GetComponent<TMP_Text>();
            hud.legalRiskText.color = new Color(0.2f, 0.85f, 0.35f);
            hud.legalRiskText.fontStyle = FontStyles.Bold;

            var legalIndicator = topBar.gameObject.AddComponent<LegalRiskIndicator>();
            legalIndicator.Init(hud.legalRiskBarFill, hud.legalRiskText);
            hud.legalRiskIndicator = legalIndicator;

            // ============================================================
            // SEASON INDICATOR -- season name + turn X/25, below top bar
            // ============================================================
            var seasonPanel = CreateUIPanel("SeasonPanel", canvasGo.transform);
            SetAnchors(seasonPanel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            seasonPanel.anchoredPosition = new Vector2(0, -80);
            seasonPanel.sizeDelta = new Vector2(300, 28);

            var seasonNameRt = CreateTextElement("SeasonName", seasonPanel, "SPRING", 20);
            seasonNameRt.anchoredPosition = new Vector2(-40, 0);
            seasonNameRt.sizeDelta = new Vector2(160, 26);
            hud.seasonNameText = seasonNameRt.GetComponent<TMP_Text>();
            hud.seasonNameText.color = new Color(0.5f, 0.95f, 0.5f);
            hud.seasonNameText.fontStyle = FontStyles.Bold;

            var turnProgressRt = CreateTextElement("TurnProgress", seasonPanel, "1/25", 16);
            turnProgressRt.anchoredPosition = new Vector2(90, 0);
            turnProgressRt.sizeDelta = new Vector2(100, 26);
            hud.seasonTurnText = turnProgressRt.GetComponent<TMP_Text>();
            hud.seasonTurnText.color = new Color(0.75f, 0.75f, 0.75f);

            var seasonIndicator = seasonPanel.gameObject.AddComponent<SeasonIndicator>();
            seasonIndicator.Init(hud.seasonNameText, hud.seasonTurnText);
            hud.seasonIndicator = seasonIndicator;

            // ============================================================
            // CUSTOMER MARKET BAR -- player (blue) vs rival (red), 100 customers
            // Placed below the season indicator
            // ============================================================
            var marketBarPanel = CreateUIPanel("CustomerMarketPanel", canvasGo.transform);
            SetAnchors(marketBarPanel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            marketBarPanel.anchoredPosition = new Vector2(0, -114);
            marketBarPanel.sizeDelta = new Vector2(500, 24);

            var marketLabelRt = CreateTextElement("MarketLabel", marketBarPanel, "MARKET", 11);
            marketLabelRt.anchoredPosition = new Vector2(-230, 0);
            marketLabelRt.sizeDelta = new Vector2(60, 22);
            marketLabelRt.GetComponent<TMP_Text>().color = new Color(0.6f, 0.6f, 0.6f);

            // Background track (neutral gray)
            var marketTrackBg = CreateUIPanel("MarketTrack", marketBarPanel);
            marketTrackBg.anchoredPosition = new Vector2(20, 0);
            marketTrackBg.sizeDelta = new Vector2(380, 14);
            marketTrackBg.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f);

            // Player fill (left side, blue)
            var playerFill = CreateUIPanel("PlayerFill", marketTrackBg);
            playerFill.anchorMin = new Vector2(0, 0);
            playerFill.anchorMax = new Vector2(0, 1);
            playerFill.pivot = new Vector2(0, 0.5f);
            playerFill.sizeDelta = new Vector2(0f, 0);
            playerFill.anchoredPosition = Vector2.zero;
            hud.marketSharePlayerFill = playerFill.GetComponent<Image>();
            hud.marketSharePlayerFill.color = new Color(0.25f, 0.55f, 1f); // Player blue

            // Rival fill (right side, red) -- anchored to right edge
            var rivalFill = CreateUIPanel("RivalFill", marketTrackBg);
            rivalFill.anchorMin = new Vector2(1, 0);
            rivalFill.anchorMax = new Vector2(1, 1);
            rivalFill.pivot = new Vector2(1, 0.5f);
            rivalFill.sizeDelta = new Vector2(0f, 0);
            rivalFill.anchoredPosition = Vector2.zero;
            hud.marketShareRivalFill = rivalFill.GetComponent<Image>();
            hud.marketShareRivalFill.color = new Color(0.9f, 0.2f, 0.2f); // Rival red

            // Score text
            var marketScoreRt = CreateTextElement("MarketScore", marketBarPanel, "0 vs 0", 14);
            marketScoreRt.anchoredPosition = new Vector2(230, 0);
            marketScoreRt.sizeDelta = new Vector2(80, 22);
            hud.marketShareText = marketScoreRt.GetComponent<TMP_Text>();
            hud.marketShareText.color = new Color(0.85f, 0.85f, 0.85f);

            var marketIndicator = marketBarPanel.gameObject.AddComponent<CustomerMarketIndicator>();
            marketIndicator.Init(hud.marketSharePlayerFill, hud.marketShareRivalFill, hud.marketShareText, marketTrackBg);
            hud.customerMarketIndicator = marketIndicator;

            // Neglect warning (shows briefly when a business is neglected)
            var neglectWarning = CreateTextElement("NeglectWarning", topBar, "", 20);
            neglectWarning.anchoredPosition = new Vector2(0, -65);
            neglectWarning.sizeDelta = new Vector2(500, 35);
            var neglectTmp = neglectWarning.GetComponent<TMP_Text>();
            neglectTmp.color = new Color(1f, 0.5f, 0.2f); // Orange warning
            neglectTmp.fontStyle = FontStyles.Bold;
            hud.neglectWarningText = neglectTmp;

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

            // End Turn Button - prominent and readable
            var endTurnBtn = CreateButton("EndTurnButton", canvasGo.transform, "END TURN");
            SetAnchors(endTurnBtn, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
            endTurnBtn.anchoredPosition = new Vector2(-100, 200);
            endTurnBtn.sizeDelta = new Vector2(180, 55);
            endTurnBtn.GetComponentInChildren<TMP_Text>().fontSize = 24;
            endTurnBtn.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
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

                // Card name text (top of card) - auto-sized to fit
                var nameRt = CreateTextElement("CardName", shopCard, "", 20);
                nameRt.anchoredPosition = new Vector2(0, 70);
                nameRt.sizeDelta = new Vector2(140, 50);
                var shopNameTmp = nameRt.GetComponent<TMP_Text>();
                shopNameTmp.alignment = TextAlignmentOptions.Center;
                shopNameTmp.fontStyle = FontStyles.Bold;
                shopNameTmp.enableAutoSizing = true;
                shopNameTmp.fontSizeMin = 12;
                shopNameTmp.fontSizeMax = 20;

                // Card description / stats (middle)
                var descRt = CreateTextElement("CardDesc", shopCard, "", 14);
                descRt.anchoredPosition = new Vector2(0, 15);
                descRt.sizeDelta = new Vector2(140, 60);
                var shopDescTmp = descRt.GetComponent<TMP_Text>();
                shopDescTmp.alignment = TextAlignmentOptions.Center;
                shopDescTmp.color = new Color(0.85f, 0.85f, 0.85f);
                shopDescTmp.enableAutoSizing = true;
                shopDescTmp.fontSizeMin = 10;
                shopDescTmp.fontSizeMax = 14;

                // Price text (below description) - bold gold
                var priceRt = CreateTextElement("PriceText", shopCard, "$0", 24);
                priceRt.anchoredPosition = new Vector2(0, -35);
                priceRt.sizeDelta = new Vector2(140, 30);
                var shopPriceTmp = priceRt.GetComponent<TMP_Text>();
                shopPriceTmp.alignment = TextAlignmentOptions.Center;
                shopPriceTmp.color = GoldColor;
                shopPriceTmp.fontStyle = FontStyles.Bold;
                shopPriceTexts[i] = shopPriceTmp;

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

            var comboTextRt = CreateTextElement("ComboText", comboPopup, "", 48);
            comboTextRt.anchorMin = Vector2.zero;
            comboTextRt.anchorMax = Vector2.one;
            comboTextRt.sizeDelta = Vector2.zero;
            comboTextRt.offsetMin = Vector2.zero;
            comboTextRt.offsetMax = Vector2.zero;
            var comboTmp = comboTextRt.GetComponent<TMP_Text>();
            comboTmp.fontStyle = FontStyles.Bold;
            comboTmp.color = LightGold;

            hud.comboPopup.SetReferences(comboTmp, comboCg);

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

            var rivalActionRt = CreateTextElement("RivalActionText", rivalPopup, "", 30);
            rivalActionRt.anchoredPosition = new Vector2(0, 30);
            rivalActionRt.sizeDelta = new Vector2(460, 45);
            rivalActionRt.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;

            var rivalTauntRt = CreateTextElement("RivalTauntText", rivalPopup, "", 22);
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

            var tierTitleText = CreateTextElement("TierTitle", tierPopupRt, "ENTREPRENEUR", 52);
            tierTitleText.anchoredPosition = new Vector2(0, 30);
            tierTitleText.sizeDelta = new Vector2(500, 70);
            var tierTitleTmp = tierTitleText.GetComponent<TMP_Text>();
            tierTitleTmp.fontStyle = FontStyles.Bold;
            tierTitleTmp.color = GoldColor;

            var tierSubText = CreateTextElement("TierSubtitle", tierPopupRt, "", 24);
            tierSubText.anchoredPosition = new Vector2(0, -30);
            tierSubText.sizeDelta = new Vector2(500, 45);

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
            var ventureTitle = CreateTextElement("VentureTitle", venturePanel, "CHOOSE YOUR FIRST VENTURE", 42);
            ventureTitle.anchoredPosition = new Vector2(0, 350);
            ventureTitle.sizeDelta = new Vector2(800, 60);

            // 5 Venture cards in a row (GDD v3.0 Section 1.5)
            var ventureCards = new RectTransform[5];
            var ventureCardImages = new Image[5];
            var ventureNameTexts = new TMP_Text[5];
            var ventureDescTexts = new TMP_Text[5];
            string[] ventureNames = {
                "🍔 FAST FOOD",
                "☕ CAFE",
                "📱 TECH APP",
                "👗 CLOTHING STORE",
                "🛒 GROCERY STORE"
            };
            string[] ventureDescs = {
                "High volume, low margin.\n+1 Cook in starter deck.\nFood combo potential.",
                "Loyalty-driven income.\n+1 Barista in starter deck.\nPlatform rating matters.",
                "Viral growth possible.\n+1 Developer in starter deck.\nPlatform fees apply.",
                "Seasonal demand cycles.\n+1 Sales Associate in deck.\nTrend-sensitive.",
                "Steady local demand.\n+1 Cashier in starter deck.\nSpoilage risk mechanic."
            };
            Color[] ventureColors = {
                new Color(0.9f, 0.4f, 0.1f),   // FastFood - red-orange
                new Color(0.6f, 0.38f, 0.22f),  // Cafe - coffee brown
                new Color(0.25f, 0.55f, 0.95f), // TechApp - blue
                new Color(0.85f, 0.25f, 0.55f), // ClothingStore - pink
                new Color(0.3f, 0.7f, 0.35f)    // GroceryStore - green
            };

            for (int i = 0; i < 5; i++)
            {
                var card = CreateUIPanel($"VentureCard_{i}", venturePanel);
                card.sizeDelta = new Vector2(260, 400);
                card.anchoredPosition = new Vector2((i - 2f) * 275f, -20);
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
            var ventureStartBtn = CreateButton("VentureStartButton", venturePanel, "START");
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

        // Platform Rating Indicator
        public PlatformRatingIndicator platformRatingIndicator;
        public Image platformRatingBarFill;
        public TMP_Text platformRatingText;

        // Legal Risk Indicator
        public LegalRiskIndicator legalRiskIndicator;
        public Image legalRiskBarFill;
        public TMP_Text legalRiskText;

        // Season Indicator
        public SeasonIndicator seasonIndicator;
        public TMP_Text seasonNameText;
        public TMP_Text seasonTurnText;

        // Customer Market Bar
        public CustomerMarketIndicator customerMarketIndicator;
        public Image marketSharePlayerFill;
        public Image marketShareRivalFill;
        public TMP_Text marketShareText;
    }
}
