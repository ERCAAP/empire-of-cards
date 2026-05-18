using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Gameplay;
using EmpireOfCards.UI;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.Audio;
using EmpireOfCards.VFX;
using EmpireOfCards.Save;
using EmpireOfCards.Helpers;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Place this on a single GameObject in Game.unity.
    /// On Awake it builds the entire game hierarchy from scratch.
    /// Assign ScriptableObject data assets in the Inspector.
    /// </summary>
    public class GameSceneBootstrap : MonoBehaviour
    {
        [Header("=== Veri Atamalari (Inspector'dan ata) ===")]
        [SerializeField] private GameBalanceData balanceData;
        [SerializeField] private DeckPresetData startingDeck;
        [SerializeField] private RivalData megaCorpData;
        [SerializeField] private ComboData[] allCombos;
        [SerializeField] private CardData[] shopPool;

        [Header("=== Prefab References ===")]
        [SerializeField] private GameObject cardPrefab;

        // References created at runtime
        private GameManager _gameManager;
        private TurnManager _turnManager;
        private EconomyManager _economyManager;
        private DeckManager _deckManager;
        private BoardManager _boardManager;
        private ComboSystem _comboSystem;
        private TerritoryManager _territoryManager;
        private FBISystem _fbiSystem;
        private RivalAI _rivalAI;
        private ShopManager _shopManager;
        private UIManager _uiManager;
        private AudioManager _audioManager;
        private VFXManager _vfxManager;
        private SaveManager _saveManager;

        private void Awake()
        {
            // ====== MANAGERS ======
            CreateManagers();

            // ====== CAMERA ======
            SetupCamera();

            // ====== BOARD ======
            CreateBoard();

            // ====== UI CANVAS ======
            CreateUI();

            // ====== VFX ======
            CreateVFX();

            // ====== WIRE REFERENCES ======
            WireManagerReferences();

            Debug.Log("[GameSceneBootstrap] Scene hierarchy created successfully.");
        }

        private void Start()
        {
            // Start the game after everything is wired
            if (_gameManager != null)
                _gameManager.StartNewRun();
        }

        // ================================================================
        // MANAGERS (GDD 12.2 - Managers section)
        // ================================================================

        private void CreateManagers()
        {
            // --- GameManager (Singleton, DontDestroyOnLoad) ---
            var gmGo = new GameObject("[GameManager]");
            _gameManager = gmGo.AddComponent<GameManager>();
            // GameManager handles its own DontDestroyOnLoad in Awake

            // --- TurnManager ---
            var tmGo = new GameObject("TurnManager");
            tmGo.transform.SetParent(gmGo.transform);
            _turnManager = tmGo.AddComponent<TurnManager>();

            // --- EconomyManager ---
            var emGo = new GameObject("EconomyManager");
            emGo.transform.SetParent(gmGo.transform);
            _economyManager = emGo.AddComponent<EconomyManager>();

            // --- DeckManager ---
            var dmGo = new GameObject("DeckManager");
            dmGo.transform.SetParent(gmGo.transform);
            _deckManager = dmGo.AddComponent<DeckManager>();

            // --- BoardManager ---
            var bmGo = new GameObject("BoardManager");
            bmGo.transform.SetParent(gmGo.transform);
            _boardManager = bmGo.AddComponent<BoardManager>();

            // --- ComboSystem ---
            var csGo = new GameObject("ComboSystem");
            csGo.transform.SetParent(gmGo.transform);
            _comboSystem = csGo.AddComponent<ComboSystem>();

            // --- TerritoryManager ---
            var terGo = new GameObject("TerritoryManager");
            terGo.transform.SetParent(gmGo.transform);
            _territoryManager = terGo.AddComponent<TerritoryManager>();

            // --- FBISystem ---
            var fbiGo = new GameObject("FBISystem");
            fbiGo.transform.SetParent(gmGo.transform);
            _fbiSystem = fbiGo.AddComponent<FBISystem>();

            // --- RivalAI ---
            var aiGo = new GameObject("RivalAI");
            aiGo.transform.SetParent(gmGo.transform);
            _rivalAI = aiGo.AddComponent<RivalAI>();

            // --- ShopManager ---
            var shopGo = new GameObject("ShopManager");
            shopGo.transform.SetParent(gmGo.transform);
            _shopManager = shopGo.AddComponent<ShopManager>();

            // --- AudioManager ---
            var audioGo = new GameObject("[AudioManager]");
            _audioManager = audioGo.AddComponent<AudioManager>();
            // Add audio sources
            var musicSource = audioGo.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            var sfxSource = audioGo.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;

            // --- VFXManager ---
            var vfxGo = new GameObject("VFXManager");
            _vfxManager = vfxGo.AddComponent<VFXManager>();

            // --- SaveManager ---
            var saveGo = new GameObject("[SaveManager]");
            _saveManager = saveGo.AddComponent<SaveManager>();
        }

        // ================================================================
        // CAMERA (GDD 12.2 - Kamera section)
        // ================================================================

        private void SetupCamera()
        {
            var cam = Camera.main;
            if (cam != null)
            {
                // Set to orthographic for 2D card game
                cam.orthographic = true;
                cam.orthographicSize = 6f;
                cam.transform.position = new Vector3(0, 0, -10);
                cam.backgroundColor = new Color(0.15f, 0.12f, 0.1f); // Warm dark wood color

                // Add screen shake component
                if (cam.GetComponent<ScreenShake>() == null)
                    cam.gameObject.AddComponent<ScreenShake>();

                if (cam.GetComponent<CameraController>() == null)
                    cam.gameObject.AddComponent<CameraController>();
            }
        }

        // ================================================================
        // BOARD (GDD 12.2 - Masa section)
        // ================================================================

        private void CreateBoard()
        {
            var boardRoot = new GameObject("--- BOARD ---");

            // --- Background ---
            var bg = new GameObject("Background");
            bg.transform.SetParent(boardRoot.transform);
            var bgRenderer = bg.AddComponent<SpriteRenderer>();
            bgRenderer.color = new Color(0.35f, 0.25f, 0.15f); // Wood color placeholder
            bgRenderer.sortingOrder = -10;

            // --- Territory Map (10 bolge) ---
            var territoryMap = new GameObject("TerritoryMap");
            territoryMap.transform.SetParent(boardRoot.transform);
            territoryMap.transform.localPosition = new Vector3(0, 2.5f, 0);

            for (int i = 0; i < 10; i++)
            {
                var slot = new GameObject($"Territory_{i + 1:D2}");
                slot.transform.SetParent(territoryMap.transform);
                float x = (i - 4.5f) * 0.9f;
                slot.transform.localPosition = new Vector3(x, 0, 0);

                var sr = slot.AddComponent<SpriteRenderer>();
                sr.color = Color.gray; // Empty territory
                sr.sortingOrder = 1;
            }

            // --- Event Area ---
            var eventArea = new GameObject("EventArea");
            eventArea.transform.SetParent(boardRoot.transform);
            eventArea.transform.localPosition = new Vector3(0, 1.5f, 0);

            // --- Player Area (3-5 Business Slots) ---
            var playerArea = new GameObject("PlayerArea");
            playerArea.transform.SetParent(boardRoot.transform);
            playerArea.transform.localPosition = new Vector3(0, -0.5f, 0);

            for (int i = 0; i < 5; i++)
            {
                var bizSlot = new GameObject($"BusinessSlot_{i + 1:D2}");
                bizSlot.transform.SetParent(playerArea.transform);
                float x = (i - 2f) * 2.5f;
                bizSlot.transform.localPosition = new Vector3(x, 0, 0);

                bizSlot.AddComponent<DropZone>();

                // Employee sub-slots (max 3 per business)
                for (int e = 0; e < 3; e++)
                {
                    var empSlot = new GameObject($"EmpSlot_{e + 1:D2}");
                    empSlot.transform.SetParent(bizSlot.transform);
                    empSlot.transform.localPosition = new Vector3(0, -(e + 1) * 0.6f, 0);

                    empSlot.AddComponent<DropZone>();
                }

                // Hide slots 4 and 5 initially
                if (i >= 3)
                    bizSlot.SetActive(false);
            }

            // --- Rival Area ---
            var rivalArea = new GameObject("RivalArea");
            rivalArea.transform.SetParent(boardRoot.transform);
            rivalArea.transform.localPosition = new Vector3(0, 3.5f, 0);

            for (int i = 0; i < 3; i++)
            {
                var rivalSlot = new GameObject($"RivalSlot_{i + 1:D2}");
                rivalSlot.transform.SetParent(rivalArea.transform);
                float x = (i - 1f) * 2.5f;
                rivalSlot.transform.localPosition = new Vector3(x, 0, 0);
            }

            // --- Upgrade Area ---
            var upgradeArea = new GameObject("UpgradeArea");
            upgradeArea.transform.SetParent(boardRoot.transform);
            upgradeArea.transform.localPosition = new Vector3(-5f, 0, 0);

            // --- Discard / Sell Zone ---
            var discardPile = new GameObject("DiscardPile");
            discardPile.transform.SetParent(boardRoot.transform);
            discardPile.transform.localPosition = new Vector3(5.5f, -3f, 0);
            discardPile.AddComponent<DropZone>();
        }

        // ================================================================
        // UI (GDD 12.2 - UI Canvas section)
        // ================================================================

        private void CreateUI()
        {
            // --- Main Canvas (Screen Space Overlay) ---
            var canvasGo = new GameObject("GameCanvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            canvasGo.AddComponent<GraphicRaycaster>();

            // --- UIManager on canvas ---
            _uiManager = canvasGo.AddComponent<UIManager>();

            // --- TopBar ---
            var topBar = CreateUIPanel("TopBar", canvasGo.transform);
            SetAnchors(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
            topBar.sizeDelta = new Vector2(0, 80);
            topBar.gameObject.AddComponent<TopBarUI>();

            // Money display
            var moneyObj = CreateTextElement("MoneyDisplay", topBar, "$500", 36);
            moneyObj.localPosition = new Vector3(-700, -40, 0);

            // Turn counter
            var turnObj = CreateTextElement("TurnCounter", topBar, "Tur 1/20", 24);
            turnObj.localPosition = new Vector3(0, -40, 0);

            // FBI Risk bar
            var fbiBarBg = CreateUIPanel("FBIRiskBar", topBar);
            fbiBarBg.localPosition = new Vector3(700, -40, 0);
            fbiBarBg.sizeDelta = new Vector2(200, 20);
            var fbiBarBgImg = fbiBarBg.GetComponent<Image>();
            fbiBarBgImg.color = new Color(0.2f, 0.2f, 0.2f);

            var fbiBarFill = CreateUIPanel("Fill", fbiBarBg);
            fbiBarFill.sizeDelta = new Vector2(200, 20);
            var fbiBarFillImg = fbiBarFill.GetComponent<Image>();
            fbiBarFillImg.color = Color.green;

            // --- ActionBar ---
            var actionBar = CreateUIPanel("ActionBar", canvasGo.transform);
            SetAnchors(actionBar, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
            actionBar.anchoredPosition = new Vector2(0, 200);
            actionBar.sizeDelta = new Vector2(200, 40);
            actionBar.gameObject.AddComponent<ActionBarUI>();

            // Action dots (3-5)
            for (int i = 0; i < 5; i++)
            {
                var dot = CreateUIPanel($"ActionDot_{i + 1}", actionBar);
                dot.sizeDelta = new Vector2(30, 30);
                dot.localPosition = new Vector3((i - 2) * 35f, 0, 0);
                dot.GetComponent<Image>().color = i < 3 ? Color.green : Color.gray;
                if (i >= 3) dot.gameObject.SetActive(false); // Hidden until upgrade
            }

            // --- Hand Area (bottom of screen) ---
            var handArea = CreateUIPanel("HandArea", canvasGo.transform);
            SetAnchors(handArea, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
            handArea.anchoredPosition = new Vector2(0, 80);
            handArea.sizeDelta = new Vector2(0, 160);
            handArea.gameObject.AddComponent<HandUI>();

            // --- Buttons ---
            // End Turn Button
            var endTurnBtn = CreateButton("EndTurnButton", canvasGo.transform, "TUR BITIR");
            SetAnchors(endTurnBtn, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
            endTurnBtn.anchoredPosition = new Vector2(-100, 200);
            endTurnBtn.sizeDelta = new Vector2(160, 50);

            // Shop Button
            var shopBtn = CreateButton("ShopButton", canvasGo.transform, "DUKKAN");
            SetAnchors(shopBtn, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            shopBtn.anchoredPosition = new Vector2(100, 200);
            shopBtn.sizeDelta = new Vector2(140, 50);

            // Deck Button
            var deckBtn = CreateButton("DeckButton", canvasGo.transform, "DESTE: 14");
            SetAnchors(deckBtn, new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0));
            deckBtn.anchoredPosition = new Vector2(100, 140);
            deckBtn.sizeDelta = new Vector2(140, 40);

            // --- Shop Panel (hidden by default) ---
            var shopPanel = CreateUIPanel("ShopPanel", canvasGo.transform);
            shopPanel.anchorMin = new Vector2(0.2f, 0.2f);
            shopPanel.anchorMax = new Vector2(0.8f, 0.8f);
            shopPanel.sizeDelta = Vector2.zero;
            shopPanel.offsetMin = Vector2.zero;
            shopPanel.offsetMax = Vector2.zero;
            shopPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            shopPanel.gameObject.AddComponent<ShopPanel>();
            shopPanel.gameObject.SetActive(false);

            // Shop title
            CreateTextElement("ShopTitle", shopPanel, "DUKKAN", 32);

            // Shop card slots
            for (int i = 0; i < 3; i++)
            {
                var shopCard = CreateUIPanel($"ShopCard_{i + 1}", shopPanel);
                shopCard.sizeDelta = new Vector2(150, 220);
                shopCard.localPosition = new Vector3((i - 1) * 200f, 0, 0);
                shopCard.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.35f);
            }

            var shopCloseBtn = CreateButton("CloseButton", shopPanel, "KAPAT");
            shopCloseBtn.anchoredPosition = new Vector2(0, -200);
            shopCloseBtn.sizeDelta = new Vector2(120, 40);

            // --- Popup overlays (hidden by default) ---
            var comboPopup = CreateUIPanel("ComboPopup", canvasGo.transform);
            comboPopup.sizeDelta = new Vector2(600, 100);
            comboPopup.gameObject.AddComponent<ComboPopup>();
            comboPopup.gameObject.SetActive(false);

            var eventPopup = CreateUIPanel("EventPopup", canvasGo.transform);
            eventPopup.sizeDelta = new Vector2(400, 300);
            eventPopup.gameObject.AddComponent<EventPopup>();
            eventPopup.gameObject.SetActive(false);

            var rivalPopup = CreateUIPanel("RivalPopup", canvasGo.transform);
            SetAnchors(rivalPopup, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            rivalPopup.sizeDelta = new Vector2(500, 200);
            rivalPopup.gameObject.AddComponent<RivalPopup>();
            rivalPopup.gameObject.SetActive(false);

            // --- Score Overlay (hidden by default) ---
            var scoreOverlay = CreateUIPanel("ScoreOverlay", canvasGo.transform);
            scoreOverlay.anchorMin = Vector2.zero;
            scoreOverlay.anchorMax = Vector2.one;
            scoreOverlay.sizeDelta = Vector2.zero;
            scoreOverlay.offsetMin = Vector2.zero;
            scoreOverlay.offsetMax = Vector2.zero;
            scoreOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);
            scoreOverlay.gameObject.AddComponent<ScoreScreen>();
            scoreOverlay.gameObject.SetActive(false);

            // --- GameOver Overlay (hidden by default) ---
            var gameOverOverlay = CreateUIPanel("GameOverOverlay", canvasGo.transform);
            gameOverOverlay.anchorMin = Vector2.zero;
            gameOverOverlay.anchorMax = Vector2.one;
            gameOverOverlay.sizeDelta = Vector2.zero;
            gameOverOverlay.offsetMin = Vector2.zero;
            gameOverOverlay.offsetMax = Vector2.zero;
            gameOverOverlay.GetComponent<Image>().color = new Color(0, 0, 0, 0.85f);
            gameOverOverlay.gameObject.AddComponent<GameOverScreen>();
            gameOverOverlay.gameObject.SetActive(false);

            // --- EventSystem (required for UI interaction) ---
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                var eventSystemGo = new GameObject("EventSystem");
                eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        // ================================================================
        // VFX (GDD 12.2 - Efektler section)
        // ================================================================

        private void CreateVFX()
        {
            var vfxRoot = new GameObject("--- VFX ---");
            var poolParent = new GameObject("VFXPool");
            poolParent.transform.SetParent(vfxRoot.transform);
        }

        // ================================================================
        // WIRE REFERENCES (Connect all managers to each other via reflection or SerializeField)
        // ================================================================

        private void WireManagerReferences()
        {
            // Since we can't set [SerializeField] at runtime, we need to use
            // a public method or reflection. The cleanest approach is to add
            // an Initialize method to GameManager that accepts all references.

            // For now, log that wiring needs to happen via the Inspector
            // after the first run, OR we create a dedicated wiring method.
            Debug.Log("[GameSceneBootstrap] Manager hierarchy created. " +
                      "Wire SerializeField references in Inspector or via GameManager.Initialize().");
        }

        // ================================================================
        // UI Helper Methods
        // ================================================================

        private RectTransform CreateUIPanel(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            var img = go.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0); // Transparent by default
            return rt;
        }

        private RectTransform CreateButton(string name, Transform parent, string label)
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

        private RectTransform CreateTextElement(string name, Transform parent, string content, int fontSize)
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

        private void SetAnchors(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
        }
    }
}
