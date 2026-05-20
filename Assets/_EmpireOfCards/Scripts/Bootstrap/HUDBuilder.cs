using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.UI;
using EmpireOfCards.UI.Indicators;
using EmpireOfCards.Presentation;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Builds the entire HUD canvas and UI panels at runtime.
    /// Returns a bundle with all UI references needed for wiring.
    /// </summary>
    public static class HUDBuilder
    {
        private static readonly Color GoldColor = ControlDeskTheme.MoneyGold;
        private static readonly Color LightGold = ControlDeskTheme.Lighten(ControlDeskTheme.MoneyGold, 0.1f);
        /// <summary>
        /// Creates the HUD canvas, all panels, buttons, and popups.
        /// Returns a HUDBundle with every reference the wiring step needs.
        /// </summary>
        public static HUDBundle Build()
        {
            var hud = new HUDBundle();
            string Loc(string key, string fallback) => EmpireOfCards.Core.LocalizationManager.GetWithFallback(key, fallback);

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

            var topBar = CreateUIPanel("TopBar", canvasGo.transform);
            SetAnchors(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            topBar.sizeDelta = new Vector2(0, 180);
            hud.topBarUI = topBar.gameObject.AddComponent<TopBarUI>();

            var statusPanel = CreateShellPanel("StatusPanel", topBar, new Vector2(320, 98), ControlDeskTheme.Panel);
            SetAnchors(statusPanel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1));
            statusPanel.anchoredPosition = new Vector2(30, -22);

            var moneyObj = CreateTextElement("MoneyDisplay", statusPanel, "$500", 38);
            SetAnchors(moneyObj, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1));
            moneyObj.anchoredPosition = new Vector2(18, -18);
            moneyObj.sizeDelta = new Vector2(240, 42);
            hud.moneyText = moneyObj.GetComponent<TMP_Text>();
            hud.moneyText.color = GoldColor;
            hud.moneyText.fontStyle = FontStyles.Bold;
            hud.moneyText.alignment = TextAlignmentOptions.Left;

            var tierChip = CreateShellPanel("TierChip", statusPanel, new Vector2(138, 28), ControlDeskTheme.WithAlpha(ControlDeskTheme.PanelLine, 0.6f));
            SetAnchors(tierChip, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1));
            tierChip.anchoredPosition = new Vector2(18, -70);

            var tierText = CreateTextElement("CompanyTierText", tierChip, "TRADER", 13);
            tierText.anchorMin = Vector2.zero;
            tierText.anchorMax = Vector2.one;
            tierText.offsetMin = Vector2.zero;
            tierText.offsetMax = Vector2.zero;
            tierText.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;
            tierText.GetComponent<TMP_Text>().color = ControlDeskTheme.TextPrimary;
            hud.companyTierText = tierText.GetComponent<TMP_Text>();

            var fbiLabel = CreateTextElement("PressureLabel", statusPanel, Loc("hud.legal_pressure", "LEGAL PRESSURE"), 11);
            SetAnchors(fbiLabel, new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1));
            fbiLabel.anchoredPosition = new Vector2(18, -76);
            fbiLabel.sizeDelta = new Vector2(110, 18);
            fbiLabel.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
            fbiLabel.GetComponent<TMP_Text>().color = ControlDeskTheme.TextMuted;

            var fbiBarBg = CreateShellPanel("FBIRiskBar", statusPanel, new Vector2(160, 10), ControlDeskTheme.WithAlpha(Color.black, 0.45f));
            SetAnchors(fbiBarBg, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
            fbiBarBg.anchoredPosition = new Vector2(-18, -78);
            RemoveOutline(fbiBarBg);

            var fbiBarFill = CreateUIPanel("Fill", fbiBarBg);
            fbiBarFill.anchorMin = new Vector2(0, 0);
            fbiBarFill.anchorMax = new Vector2(1, 1);
            fbiBarFill.offsetMin = Vector2.zero;
            fbiBarFill.offsetMax = Vector2.zero;
            hud.fbiBarFillImg = fbiBarFill.GetComponent<Image>();
            hud.fbiBarFillImg.type = Image.Type.Filled;
            hud.fbiBarFillImg.fillMethod = Image.FillMethod.Horizontal;
            hud.fbiBarFillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
            hud.fbiBarFillImg.fillAmount = 0f;
            hud.fbiBarFillImg.color = ControlDeskTheme.AccentRed;

            var runPanel = CreateShellPanel("RunPanel", topBar, new Vector2(280, 84), ControlDeskTheme.Panel);
            SetAnchors(runPanel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            runPanel.anchoredPosition = new Vector2(0, -22);

            var runLabel = CreateTextElement("RunLabel", runPanel, Loc("hud.run_state", "RUN STATE"), 11);
            runLabel.anchoredPosition = new Vector2(0, -16);
            runLabel.sizeDelta = new Vector2(140, 18);
            runLabel.GetComponent<TMP_Text>().color = ControlDeskTheme.TextMuted;

            var turnObj = CreateTextElement("TurnCounter", runPanel, "Turn 1", 28);
            turnObj.anchoredPosition = new Vector2(0, -42);
            turnObj.sizeDelta = new Vector2(230, 34);
            hud.turnText = turnObj.GetComponent<TMP_Text>();
            hud.turnText.fontStyle = FontStyles.Bold;
            hud.turnText.color = ControlDeskTheme.TextPrimary;

            var seasonPanel = CreateUIPanel("SeasonPanel", runPanel);
            SetAnchors(seasonPanel, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            seasonPanel.anchoredPosition = new Vector2(0, 16);
            seasonPanel.sizeDelta = new Vector2(220, 28);

            var seasonNameRt = CreateTextElement("SeasonName", seasonPanel, "SPRING", 18);
            seasonNameRt.anchoredPosition = new Vector2(-28, 0);
            seasonNameRt.sizeDelta = new Vector2(132, 24);
            hud.seasonNameText = seasonNameRt.GetComponent<TMP_Text>();
            hud.seasonNameText.color = ControlDeskTheme.AccentGreen;
            hud.seasonNameText.fontStyle = FontStyles.Bold;

            var turnProgressRt = CreateTextElement("TurnProgress", seasonPanel, "1/25", 15);
            turnProgressRt.anchoredPosition = new Vector2(74, 0);
            turnProgressRt.sizeDelta = new Vector2(70, 22);
            hud.seasonTurnText = turnProgressRt.GetComponent<TMP_Text>();
            hud.seasonTurnText.color = ControlDeskTheme.TextMuted;

            var seasonIndicator = seasonPanel.gameObject.AddComponent<SeasonIndicator>();
            seasonIndicator.Init(hud.seasonNameText, hud.seasonTurnText);
            hud.seasonIndicator = seasonIndicator;

            var metricsPanel = CreateShellPanel("MetricsPanel", topBar, new Vector2(360, 98), ControlDeskTheme.Panel);
            SetAnchors(metricsPanel, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
            metricsPanel.anchoredPosition = new Vector2(-30, -22);

            var platformRatingPanel = CreateShellPanel("PlatformRatingPanel", metricsPanel, new Vector2(156, 64), ControlDeskTheme.PanelSoft);
            SetAnchors(platformRatingPanel, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f));
            platformRatingPanel.anchoredPosition = new Vector2(18, -6);

            var platformLabelRt = CreateTextElement("PlatformLabel", platformRatingPanel, Loc("hud.rating", "RATING"), 12);
            platformLabelRt.anchoredPosition = new Vector2(0, 16);
            platformLabelRt.sizeDelta = new Vector2(160, 18);
            platformLabelRt.GetComponent<TMP_Text>().color = ControlDeskTheme.TextMuted;

            var platformBarBg = CreateUIPanel("PlatformBarBg", platformRatingPanel);
            platformBarBg.anchoredPosition = new Vector2(0, -4);
            platformBarBg.sizeDelta = new Vector2(140, 12);
            platformBarBg.GetComponent<Image>().color = ControlDeskTheme.WithAlpha(Color.black, 0.4f);

            var platformBarFill = CreateUIPanel("PlatformBarFill", platformBarBg);
            platformBarFill.anchorMin = new Vector2(0, 0);
            platformBarFill.anchorMax = new Vector2(0, 1);
            platformBarFill.pivot = new Vector2(0, 0.5f);
            platformBarFill.sizeDelta = new Vector2(70f, 0); // 50% default (3.0/5.0)
            platformBarFill.anchoredPosition = Vector2.zero;
            hud.platformRatingBarFill = platformBarFill.GetComponent<Image>();
            hud.platformRatingBarFill.color = ControlDeskTheme.AccentBlue;

            var platformValueRt = CreateTextElement("PlatformValue", platformRatingPanel, "3.0", 18);
            platformValueRt.anchoredPosition = new Vector2(0, -18);
            platformValueRt.sizeDelta = new Vector2(160, 22);
            hud.platformRatingText = platformValueRt.GetComponent<TMP_Text>();
            hud.platformRatingText.color = ControlDeskTheme.AccentBlue;
            hud.platformRatingText.fontStyle = FontStyles.Bold;

            var platformIndicator = topBar.gameObject.AddComponent<PlatformRatingIndicator>();
            platformIndicator.Init(hud.platformRatingBarFill, hud.platformRatingText);
            hud.platformRatingIndicator = platformIndicator;

            var legalRiskPanel = CreateShellPanel("LegalRiskPanel", metricsPanel, new Vector2(156, 64), ControlDeskTheme.PanelSoft);
            SetAnchors(legalRiskPanel, new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f));
            legalRiskPanel.anchoredPosition = new Vector2(-18, -6);

            var legalLabelRt = CreateTextElement("LegalLabel", legalRiskPanel, Loc("hud.legal_risk", "LEGAL RISK"), 12);
            legalLabelRt.anchoredPosition = new Vector2(0, 16);
            legalLabelRt.sizeDelta = new Vector2(160, 18);
            legalLabelRt.GetComponent<TMP_Text>().color = ControlDeskTheme.TextMuted;

            var legalBarBg = CreateUIPanel("LegalBarBg", legalRiskPanel);
            legalBarBg.anchoredPosition = new Vector2(0, -4);
            legalBarBg.sizeDelta = new Vector2(140, 12);
            legalBarBg.GetComponent<Image>().color = ControlDeskTheme.WithAlpha(Color.black, 0.4f);

            var legalBarFill = CreateUIPanel("LegalBarFill", legalBarBg);
            legalBarFill.anchorMin = new Vector2(0, 0);
            legalBarFill.anchorMax = new Vector2(0, 1);
            legalBarFill.pivot = new Vector2(0, 0.5f);
            legalBarFill.sizeDelta = new Vector2(0f, 0); // 0% default
            legalBarFill.anchoredPosition = Vector2.zero;
            hud.legalRiskBarFill = legalBarFill.GetComponent<Image>();
            hud.legalRiskBarFill.color = ControlDeskTheme.AccentGreen;

            var legalValueRt = CreateTextElement("LegalValue", legalRiskPanel, "0", 18);
            legalValueRt.anchoredPosition = new Vector2(0, -18);
            legalValueRt.sizeDelta = new Vector2(160, 22);
            hud.legalRiskText = legalValueRt.GetComponent<TMP_Text>();
            hud.legalRiskText.color = ControlDeskTheme.AccentGreen;
            hud.legalRiskText.fontStyle = FontStyles.Bold;

            var legalIndicator = topBar.gameObject.AddComponent<LegalRiskIndicator>();
            legalIndicator.Init(hud.legalRiskBarFill, hud.legalRiskText);
            hud.legalRiskIndicator = legalIndicator;

            var marketBarPanel = CreateUIPanel("CustomerMarketPanel", canvasGo.transform);
            SetAnchors(marketBarPanel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            marketBarPanel.anchoredPosition = new Vector2(0, -122);
            marketBarPanel.sizeDelta = new Vector2(440, 34);
            StylePanel(marketBarPanel, ControlDeskTheme.WithAlpha(ControlDeskTheme.PanelSoft, 0.8f));

            var marketLabelRt = CreateTextElement("MarketLabel", marketBarPanel, Loc("hud.market_share", "MARKET"), 11);
            marketLabelRt.anchoredPosition = new Vector2(-176, 0);
            marketLabelRt.sizeDelta = new Vector2(64, 22);
            marketLabelRt.GetComponent<TMP_Text>().color = ControlDeskTheme.TextMuted;

            var marketTrackBg = CreateUIPanel("MarketTrack", marketBarPanel);
            marketTrackBg.anchoredPosition = new Vector2(18, 0);
            marketTrackBg.sizeDelta = new Vector2(286, 12);
            marketTrackBg.GetComponent<Image>().color = ControlDeskTheme.WithAlpha(Color.black, 0.42f);

            var playerFill = CreateUIPanel("PlayerFill", marketTrackBg);
            playerFill.anchorMin = new Vector2(0, 0);
            playerFill.anchorMax = new Vector2(0, 1);
            playerFill.pivot = new Vector2(0, 0.5f);
            playerFill.sizeDelta = new Vector2(0f, 0);
            playerFill.anchoredPosition = Vector2.zero;
            hud.marketSharePlayerFill = playerFill.GetComponent<Image>();
            hud.marketSharePlayerFill.color = ControlDeskTheme.PlayerBlock;

            var rivalFill = CreateUIPanel("RivalFill", marketTrackBg);
            rivalFill.anchorMin = new Vector2(1, 0);
            rivalFill.anchorMax = new Vector2(1, 1);
            rivalFill.pivot = new Vector2(1, 0.5f);
            rivalFill.sizeDelta = new Vector2(0f, 0);
            rivalFill.anchoredPosition = Vector2.zero;
            hud.marketShareRivalFill = rivalFill.GetComponent<Image>();
            hud.marketShareRivalFill.color = ControlDeskTheme.RivalBlock;

            var marketScoreRt = CreateTextElement("MarketScore", marketBarPanel, "0 / 0", 14);
            marketScoreRt.anchoredPosition = new Vector2(164, 0);
            marketScoreRt.sizeDelta = new Vector2(90, 22);
            hud.marketShareText = marketScoreRt.GetComponent<TMP_Text>();
            hud.marketShareText.color = ControlDeskTheme.TextPrimary;

            var marketIndicator = marketBarPanel.gameObject.AddComponent<CustomerMarketIndicator>();
            marketIndicator.Init(hud.marketSharePlayerFill, hud.marketShareRivalFill, hud.marketShareText, marketTrackBg);
            hud.customerMarketIndicator = marketIndicator;

            var neglectWarning = CreateTextElement("NeglectWarning", topBar, "", 19);
            SetAnchors(neglectWarning, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            neglectWarning.anchoredPosition = new Vector2(0, -160);
            neglectWarning.sizeDelta = new Vector2(540, 28);
            var neglectTmp = neglectWarning.GetComponent<TMP_Text>();
            neglectTmp.color = ControlDeskTheme.AccentAmber;
            neglectTmp.fontStyle = FontStyles.Bold;
            hud.neglectWarningText = neglectTmp;

            var flowPanel = CreateShellPanel("TurnFlowPanel", canvasGo.transform, new Vector2(620, 88), ControlDeskTheme.WithAlpha(ControlDeskTheme.Panel, 0.88f));
            SetAnchors(flowPanel, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            flowPanel.anchoredPosition = new Vector2(0, -168);

            var briefRt = CreateTextElement("TurnBriefText", flowPanel, "BRIEF", 18);
            briefRt.anchoredPosition = new Vector2(0, 18);
            briefRt.sizeDelta = new Vector2(580, 30);
            var briefTmp = briefRt.GetComponent<TMP_Text>();
            briefTmp.alignment = TextAlignmentOptions.Center;
            briefTmp.color = ControlDeskTheme.AccentAmber;
            briefTmp.fontStyle = FontStyles.Bold;
            hud.turnBriefText = briefTmp;

            var reportRt = CreateTextElement("TurnReportText", flowPanel, "REPORT", 15);
            reportRt.anchoredPosition = new Vector2(0, -18);
            reportRt.sizeDelta = new Vector2(580, 32);
            var reportTmp = reportRt.GetComponent<TMP_Text>();
            reportTmp.alignment = TextAlignmentOptions.Center;
            reportTmp.color = ControlDeskTheme.TextMuted;
            hud.turnReportText = reportTmp;

            var actionBar = CreateShellPanel("ActionBar", canvasGo.transform, new Vector2(250, 54), ControlDeskTheme.WithAlpha(ControlDeskTheme.Panel, 0.92f));
            SetAnchors(actionBar, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            actionBar.anchoredPosition = new Vector2(0, 124);
            hud.actionBarUI = actionBar.gameObject.AddComponent<ActionBarUI>();

            hud.actionDotImages = new Image[5];
            for (int i = 0; i < 5; i++)
            {
                var dot = CreateUIPanel($"ActionDot_{i + 1}", actionBar);
                dot.sizeDelta = new Vector2(22, 22);
                dot.localPosition = new Vector3((i - 2) * 35f, 0, 0);
                hud.actionDotImages[i] = dot.GetComponent<Image>();
                hud.actionDotImages[i].color = i < 3 ? ControlDeskTheme.AccentGreen : ControlDeskTheme.WithAlpha(ControlDeskTheme.TextMuted, 0.35f);
                if (i >= 3) dot.gameObject.SetActive(false);
            }

            var utilityPanel = CreateShellPanel("UtilityPanel", canvasGo.transform, new Vector2(162, 108), ControlDeskTheme.WithAlpha(ControlDeskTheme.Panel, 0.92f));
            SetAnchors(utilityPanel, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            utilityPanel.anchoredPosition = new Vector2(30, 34);

            var shopBtn = CreateButton("ShopButton", utilityPanel, "SHOP");
            SetAnchors(shopBtn, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1));
            shopBtn.anchoredPosition = new Vector2(0, -16);
            shopBtn.sizeDelta = new Vector2(126, 42);
            StyleButton(shopBtn, ControlDeskTheme.WithAlpha(ControlDeskTheme.AccentGreen, 0.88f));
            hud.shopButton = shopBtn.GetComponent<Button>();

            var deckBtn = CreateButton("DeckButton", utilityPanel, "DECK: 14");
            SetAnchors(deckBtn, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            deckBtn.anchoredPosition = new Vector2(0, 16);
            deckBtn.sizeDelta = new Vector2(126, 34);
            StyleButton(deckBtn, ControlDeskTheme.WithAlpha(ControlDeskTheme.PanelLine, 0.85f));

            var endTurnBtn = CreateButton("EndTurnButton", canvasGo.transform, "END TURN");
            SetAnchors(endTurnBtn, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
            endTurnBtn.anchoredPosition = new Vector2(-30, 34);
            endTurnBtn.sizeDelta = new Vector2(180, 60);
            endTurnBtn.GetComponentInChildren<TMP_Text>().fontSize = 24;
            endTurnBtn.GetComponentInChildren<TMP_Text>().fontStyle = FontStyles.Bold;
            StyleButton(endTurnBtn, ControlDeskTheme.WithAlpha(ControlDeskTheme.AccentAmber, 0.92f));
            hud.endTurnButton = endTurnBtn.GetComponent<Button>();

            // ============================================================
            // SHOP PANEL (hidden by default)
            // ============================================================
            var shopPanel = CreateUIPanel("ShopPanel", canvasGo.transform);
            shopPanel.anchorMin = new Vector2(0.2f, 0.2f);
            shopPanel.anchorMax = new Vector2(0.8f, 0.8f);
            shopPanel.sizeDelta = Vector2.zero;
            shopPanel.offsetMin = Vector2.zero;
            shopPanel.offsetMax = Vector2.zero;
            StylePanel(shopPanel, ControlDeskTheme.WithAlpha(ControlDeskTheme.Panel, 0.96f));
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
                cardBgImg.color = ControlDeskTheme.WithAlpha(ControlDeskTheme.PanelSoft, 0.96f);

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
                descRt.anchoredPosition = new Vector2(0, 24);
                descRt.sizeDelta = new Vector2(140, 70);
                var shopDescTmp = descRt.GetComponent<TMP_Text>();
                shopDescTmp.alignment = TextAlignmentOptions.Center;
                shopDescTmp.color = new Color(0.85f, 0.85f, 0.85f);
                shopDescTmp.enableAutoSizing = true;
                shopDescTmp.fontSizeMin = 10;
                shopDescTmp.fontSizeMax = 14;

                // Price text (below description) - bold gold
                var priceRt = CreateTextElement("PriceText", shopCard, "$0", 24);
                priceRt.anchoredPosition = new Vector2(0, -32);
                priceRt.sizeDelta = new Vector2(140, 46);
                var shopPriceTmp = priceRt.GetComponent<TMP_Text>();
                shopPriceTmp.alignment = TextAlignmentOptions.Center;
                shopPriceTmp.color = GoldColor;
                shopPriceTmp.fontStyle = FontStyles.Bold;
                shopPriceTmp.enableAutoSizing = true;
                shopPriceTmp.fontSizeMin = 12;
                shopPriceTmp.fontSizeMax = 24;
                shopPriceTexts[i] = shopPriceTmp;

                // Buy button (bottom of card)
                var buyBtn = CreateButton("BuyButton", shopCard, "BUY");
                buyBtn.anchoredPosition = new Vector2(0, -80);
                buyBtn.sizeDelta = new Vector2(100, 35);
                StyleButton(buyBtn, ControlDeskTheme.WithAlpha(ControlDeskTheme.AccentGreen, 0.9f));
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
            StyleButton(shopCloseBtn, ControlDeskTheme.WithAlpha(ControlDeskTheme.PanelLine, 0.85f));
            hud.shopCloseButton = shopCloseBtn.GetComponent<Button>();

            // ============================================================
            // POPUP OVERLAYS (hidden by default)
            // ============================================================

            // Combo popup (CanvasGroup + text for animated show/fade)
            var comboPopup = CreateUIPanel("ComboPopup", canvasGo.transform);
            SetAnchors(comboPopup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            comboPopup.sizeDelta = new Vector2(600, 100);
            StylePanel(comboPopup, ControlDeskTheme.WithAlpha(ControlDeskTheme.Panel, 0.92f));
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
            StylePanel(eventPopup, ControlDeskTheme.WithAlpha(ControlDeskTheme.Panel, 0.94f));
            var eventCg = eventPopup.gameObject.AddComponent<CanvasGroup>();
            eventCg.alpha = 0f;
            hud.eventPopup = eventPopup.gameObject.AddComponent<EventPopup>();
            hud.eventPopup.SetReferences(eventCg);

            // Rival popup (CanvasGroup + text elements for fade animation)
            var rivalPopup = CreateUIPanel("RivalPopup", canvasGo.transform);
            SetAnchors(rivalPopup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            rivalPopup.sizeDelta = new Vector2(500, 200);
            StylePanel(rivalPopup, ControlDeskTheme.WithAlpha(ControlDeskTheme.Panel, 0.94f));
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
            StylePanel(tierPopupRt, ControlDeskTheme.WithAlpha(ControlDeskTheme.Panel, 0.94f));
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
            var ventureTitle = CreateTextElement("VentureTitle", venturePanel, Loc("venture.title", "CHOOSE YOUR FIRST VENTURE"), 42);
            ventureTitle.anchoredPosition = new Vector2(0, 350);
            ventureTitle.sizeDelta = new Vector2(800, 60);

            // 5 Venture cards in a row (GDD v3.0 Section 1.5)
            var ventureCards = new RectTransform[5];
            var ventureCardImages = new Image[5];
            var ventureNameTexts = new TMP_Text[5];
            var ventureDescTexts = new TMP_Text[5];
            string[] ventureNames = {
                "FAST FOOD",
                "CAFE",
                "TECH APP",
                "CLOTHING STORE",
                "GROCERY STORE"
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
            var ventureStartBtn = CreateButton("VentureStartButton", venturePanel, Loc("venture.start_button", "START"));
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

        private static RectTransform CreateShellPanel(string name, Transform parent, Vector2 size, Color fillColor)
        {
            var panel = CreateUIPanel(name, parent);
            panel.sizeDelta = size;
            StylePanel(panel, fillColor);
            return panel;
        }

        private static void StylePanel(RectTransform rt, Color fillColor)
        {
            var img = rt.GetComponent<Image>();
            if (img != null)
                img.color = fillColor;

            var outline = rt.GetComponent<Outline>();
            if (outline == null)
                outline = rt.gameObject.AddComponent<Outline>();
            outline.effectColor = ControlDeskTheme.WithAlpha(ControlDeskTheme.PanelLine, 0.9f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }

        private static void RemoveOutline(RectTransform rt)
        {
            var outline = rt.GetComponent<Outline>();
            if (outline != null)
                outline.enabled = false;
        }

        private static RectTransform CreateButton(string name, Transform parent, string label)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = ControlDeskTheme.WithAlpha(ControlDeskTheme.PanelLine, 0.9f);
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            var outline = go.AddComponent<Outline>();
            outline.effectColor = ControlDeskTheme.WithAlpha(ControlDeskTheme.TextPrimary, 0.12f);
            outline.effectDistance = new Vector2(1f, -1f);

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
            text.fontSize = 18;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = ControlDeskTheme.TextPrimary;

            return rt;
        }

        private static void StyleButton(RectTransform rt, Color fillColor)
        {
            var img = rt.GetComponent<Image>();
            if (img != null)
                img.color = fillColor;

            var outline = rt.GetComponent<Outline>();
            if (outline != null)
                outline.effectColor = ControlDeskTheme.WithAlpha(Color.black, 0.25f);
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
            text.color = ControlDeskTheme.TextPrimary;
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
        public TMP_Text companyTierText;
        public Image fbiBarFillImg;
        public TMP_Text neglectWarningText;
        public TMP_Text shopBiasText;
        public TMP_Text turnBriefText;
        public TMP_Text turnReportText;

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
