using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.UI;
using EmpireOfCards.Data;
using EmpireOfCards.World;
using TMPro;
using UnityEngine.UI;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Thin orchestrator for the 3D card game scene.
    /// Drop on an empty GameObject, press Play, game runs.
    /// All heavy lifting is delegated to:
    ///   - CardDataFactory  (40 cards, combos, balance, deck, rival, shop pool)
    ///   - ManagerFactory    (all runtime managers)
    ///   - HUDBuilder        (UI canvas, panels, buttons, popups)
    ///   - WiringService     (cross-references, button callbacks, 3D interaction)
    /// </summary>
    public class GameSceneBootstrap : MonoBehaviour
    {
        private TutorialManager _tutorialManager;
        private RunLaunchCoordinator _runLaunchCoordinator;
        private VentureSelectionUI _ventureSelectionUI;
        private VentureData[] _ventures;
        private TechCategoryProfile[] _techCategories;
        private GameObject _runNamePanel;
        private TMP_InputField _runNameInput;
        private TMP_Text _runNamePlaceholderText;
        private TMP_Text _runNameTitleText;
        private TMP_Text _runNameSubtitleText;
        private TMP_Text _runNameBodyText;
        private TMP_Text _runNameHintText;
        private TMP_Text _runNameConfirmLabel;
        private GameObject _runNameInputRoot;
        private GameObject _runCategoryRoot;
        private TMP_Text _runCategorySummaryText;
        private Button _runConfirmButton;
        private Button _runBackButton;
        private Image[] _categoryButtonImages;
        private TMP_Text[] _categoryButtonLabels;
        private VentureData _pendingVenture;
        private string _pendingRunName;
        private TechCategoryProfile _pendingTechCategory;
        private bool _showingCategoryStage;

        private void Awake()
        {
            var data = CardDataFactory.CreateAllData();
            var managers = ManagerFactory.CreateAll();
            var scene = SceneRuntimeFactory.Create();
            var hud = HUDBuilder.Build();
            WiringService.WireAll(data, managers, scene.Board3D, scene.CardFactory, scene.Hand3D, hud, scene.MainCamera);

            _runLaunchCoordinator = new RunLaunchCoordinator(managers.gameManager, managers.saveManager, scene.Board3D);
            _ventureSelectionUI = hud.ventureSelectionUI;
            _ventures = data.ventures;
            _techCategories = TechCategoryCatalog.CreateDefaults();

            if (_ventureSelectionUI != null && hud.ventureCards != null)
            {
                _ventureSelectionUI.SetUIReferences(
                    hud.ventureCards, hud.ventureCardImages,
                    hud.ventureNameTexts, hud.ventureDescTexts,
                    hud.ventureStartButton);
            }

            _tutorialManager = CreateTutorial(hud);
            CreateRunNamePrompt(hud);

            Debug.Log("[GameSceneBootstrap] 3D scene created successfully.");
        }

        private void Start()
        {
            if (RunLaunchConfig.LaunchMode == RunLaunchMode.LoadRun)
            {
                TryRestoreSavedRun();
                return;
            }

            // Show venture selection instead of starting immediately
            if (_ventureSelectionUI != null && _ventures != null && _ventures.Length > 0)
            {
                _ventureSelectionUI.Init(_ventures);
                _ventureSelectionUI.OnVentureSelected += OnVentureChosen;
                _ventureSelectionUI.Show();
            }
            else
            {
                // Fallback: start without venture selection
                StartRun(null);
            }
        }

        private void OnVentureChosen(VentureData venture)
        {
            _ventureSelectionUI.OnVentureSelected -= OnVentureChosen;
            _pendingRunName = null;
            _pendingTechCategory = null;
            ShowRunNamePrompt(venture);
        }

        private void StartRun(VentureData venture)
        {
            _runLaunchCoordinator?.StartNewRun(venture, _pendingRunName, _pendingTechCategory, _tutorialManager);
            _pendingRunName = null;
            _pendingTechCategory = null;
        }

        private void TryRestoreSavedRun()
        {
            if (_runLaunchCoordinator == null || !_runLaunchCoordinator.TryRestoreSavedRun(RunLaunchConfig.SelectedRunSlotId, _ventures))
            {
                RunLaunchConfig.PrepareNewRun();
                StartRun(null);
            }
        }

        /// <summary>
        /// Creates the TutorialUI overlay on the HUD canvas and wires
        /// it to a new TutorialManager MonoBehaviour.
        /// </summary>
        private TutorialManager CreateTutorial(HUDBundle hud)
        {
            // Find the HUD canvas (parent of any HUD element)
            Transform canvasParent = hud.uiManager != null
                ? hud.uiManager.transform
                : null;

            if (canvasParent == null)
            {
                Debug.LogWarning("[GameSceneBootstrap] No canvas found for TutorialUI.");
                return null;
            }

            // Build the overlay UI
            var tutorialUI = TutorialUI.Create(canvasParent);

            // Create the manager
            var tutGo = new GameObject("TutorialManager");
            var tutorialManager = tutGo.AddComponent<TutorialManager>();
            tutorialManager.Init(tutorialUI);

            return tutorialManager;
        }

        private void CreateRunNamePrompt(HUDBundle hud)
        {
            Transform canvasParent = hud.uiManager != null ? hud.uiManager.transform : null;
            if (canvasParent == null)
                return;

            _runNamePanel = new GameObject("RunNamePanel");
            _runNamePanel.transform.SetParent(canvasParent, false);
            var panelRt = _runNamePanel.AddComponent<RectTransform>();
            panelRt.anchorMin = Vector2.zero;
            panelRt.anchorMax = Vector2.one;
            panelRt.offsetMin = Vector2.zero;
            panelRt.offsetMax = Vector2.zero;
            var bg = _runNamePanel.AddComponent<Image>();
            bg.color = new Color(0.03f, 0.03f, 0.08f, 0.88f);

            var card = new GameObject("RunSetupCard");
            card.transform.SetParent(_runNamePanel.transform, false);
            var cardRt = card.AddComponent<RectTransform>();
            cardRt.anchorMin = new Vector2(0.5f, 0.5f);
            cardRt.anchorMax = new Vector2(0.5f, 0.5f);
            cardRt.pivot = new Vector2(0.5f, 0.5f);
            cardRt.anchoredPosition = new Vector2(0f, 0f);
            cardRt.sizeDelta = new Vector2(860f, 560f);
            var cardBg = card.AddComponent<Image>();
            cardBg.color = new Color(0.09f, 0.09f, 0.12f, 0.98f);

            var accent = new GameObject("AccentBar");
            accent.transform.SetParent(card.transform, false);
            var accentRt = accent.AddComponent<RectTransform>();
            accentRt.anchorMin = new Vector2(0f, 1f);
            accentRt.anchorMax = new Vector2(1f, 1f);
            accentRt.pivot = new Vector2(0.5f, 1f);
            accentRt.anchoredPosition = Vector2.zero;
            accentRt.sizeDelta = new Vector2(0f, 10f);
            var accentImg = accent.AddComponent<Image>();
            accentImg.color = new Color(0.22f, 0.58f, 0.90f, 1f);

            var titleGo = CreatePanelText("RunNameTitle", card.transform, new Vector2(760f, 54f), new Vector2(0f, 180f), 38, FontStyles.Bold);
            _runNameTitleText = titleGo;

            var subtitleGo = CreatePanelText("RunNameSubtitle", card.transform, new Vector2(740f, 36f), new Vector2(0f, 136f), 19, FontStyles.Normal);
            subtitleGo.color = new Color(0.86f, 0.86f, 0.92f, 1f);
            _runNameSubtitleText = subtitleGo;

            var bodyGo = CreatePanelText("RunNameBody", card.transform, new Vector2(720f, 140f), new Vector2(0f, 66f), 18, FontStyles.Normal);
            bodyGo.color = new Color(0.72f, 0.74f, 0.80f, 1f);
            bodyGo.textWrappingMode = TextWrappingModes.Normal;
            _runNameBodyText = bodyGo;

            _runNameInputRoot = new GameObject("RunNameInputRoot");
            _runNameInputRoot.transform.SetParent(card.transform, false);
            var inputRt = _runNameInputRoot.AddComponent<RectTransform>();
            inputRt.anchorMin = new Vector2(0.5f, 0.5f);
            inputRt.anchorMax = new Vector2(0.5f, 0.5f);
            inputRt.pivot = new Vector2(0.5f, 0.5f);
            inputRt.anchoredPosition = new Vector2(0f, -42f);
            inputRt.sizeDelta = new Vector2(620f, 74f);
            var inputBg = _runNameInputRoot.AddComponent<Image>();
            inputBg.color = new Color(0.15f, 0.15f, 0.19f, 1f);

            _runNameInput = _runNameInputRoot.AddComponent<TMP_InputField>();
            _runNameInput.lineType = TMP_InputField.LineType.SingleLine;
            _runNameInput.characterLimit = 28;

            var viewportGo = new GameObject("Viewport");
            viewportGo.transform.SetParent(_runNameInputRoot.transform, false);
            var viewportRt = viewportGo.AddComponent<RectTransform>();
            viewportRt.anchorMin = Vector2.zero;
            viewportRt.anchorMax = Vector2.one;
            viewportRt.offsetMin = new Vector2(18f, 10f);
            viewportRt.offsetMax = new Vector2(-18f, -10f);
            viewportGo.AddComponent<RectMask2D>();

            var placeholderGo = new GameObject("Placeholder");
            placeholderGo.transform.SetParent(viewportGo.transform, false);
            var placeholderRt = placeholderGo.AddComponent<RectTransform>();
            placeholderRt.anchorMin = Vector2.zero;
            placeholderRt.anchorMax = Vector2.one;
            placeholderRt.offsetMin = Vector2.zero;
            placeholderRt.offsetMax = Vector2.zero;
            var placeholder = placeholderGo.AddComponent<TextMeshProUGUI>();
            placeholder.text = "Enter a company or app name";
            placeholder.fontSize = 24;
            placeholder.color = new Color(0.62f, 0.62f, 0.68f);
            placeholder.alignment = TextAlignmentOptions.MidlineLeft;
            _runNamePlaceholderText = placeholder;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(viewportGo.transform, false);
            var textRt = textGo.AddComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;
            var inputText = textGo.AddComponent<TextMeshProUGUI>();
            inputText.fontSize = 24;
            inputText.color = Color.white;
            inputText.alignment = TextAlignmentOptions.MidlineLeft;

            _runNameInput.textViewport = viewportRt;
            _runNameInput.textComponent = inputText;
            _runNameInput.placeholder = placeholder;

            _runNameHintText = CreatePanelText("RunNameHint", card.transform, new Vector2(700f, 28f), new Vector2(0f, -100f), 17, FontStyles.Italic);
            _runNameHintText.color = new Color(0.58f, 0.78f, 0.92f, 1f);

            _runCategoryRoot = new GameObject("RunCategoryRoot");
            _runCategoryRoot.transform.SetParent(card.transform, false);
            var categoryRootRt = _runCategoryRoot.AddComponent<RectTransform>();
            categoryRootRt.anchorMin = new Vector2(0.5f, 0.5f);
            categoryRootRt.anchorMax = new Vector2(0.5f, 0.5f);
            categoryRootRt.pivot = new Vector2(0.5f, 0.5f);
            categoryRootRt.anchoredPosition = new Vector2(0f, -26f);
            categoryRootRt.sizeDelta = new Vector2(720f, 270f);

            _runCategorySummaryText = CreatePanelText("RunCategorySummary", _runCategoryRoot.transform, new Vector2(680f, 72f), new Vector2(0f, 92f), 18, FontStyles.Normal);
            _runCategorySummaryText.color = new Color(0.82f, 0.84f, 0.88f, 1f);
            _runCategorySummaryText.textWrappingMode = TextWrappingModes.Normal;

            _categoryButtonImages = new Image[_techCategories != null ? _techCategories.Length : 0];
            _categoryButtonLabels = new TMP_Text[_categoryButtonImages.Length];
            for (int i = 0; i < _categoryButtonImages.Length; i++)
            {
                int index = i;
                var option = _techCategories[i];
                var buttonGo = new GameObject($"CategoryButton_{option.categoryId}");
                buttonGo.transform.SetParent(_runCategoryRoot.transform, false);
                var buttonRt = buttonGo.AddComponent<RectTransform>();
                buttonRt.anchorMin = new Vector2(0.5f, 0.5f);
                buttonRt.anchorMax = new Vector2(0.5f, 0.5f);
                buttonRt.pivot = new Vector2(0.5f, 0.5f);
                int row = i / 2;
                int col = i % 2;
                buttonRt.anchoredPosition = new Vector2(col == 0 ? -170f : 170f, 10f - row * 74f);
                buttonRt.sizeDelta = new Vector2(300f, 58f);
                var buttonImg = buttonGo.AddComponent<Image>();
                buttonImg.color = new Color(0.16f, 0.16f, 0.20f, 1f);
                var button = buttonGo.AddComponent<Button>();
                button.targetGraphic = buttonImg;
                button.onClick.AddListener(() => SelectTechCategory(index));

                var label = CreatePanelText("Label", buttonGo.transform, new Vector2(260f, 36f), Vector2.zero, 21, FontStyles.Bold);
                label.text = GetTechCategoryName(option);
                label.color = Color.white;
                _categoryButtonImages[i] = buttonImg;
                _categoryButtonLabels[i] = label;
            }

            _runBackButton = CreateActionButton("RunSetupBack", card.transform, new Vector2(-156f, -214f), new Vector2(220f, 56f), new Color(0.31f, 0.23f, 0.18f, 1f), "BACK");
            _runBackButton.onClick.AddListener(ReturnToNameStage);
            _runBackButton.gameObject.SetActive(false);

            _runConfirmButton = CreateActionButton("RunNameConfirm", card.transform, new Vector2(156f, -214f), new Vector2(280f, 58f), new Color(0.20f, 0.56f, 0.34f, 1f), "START RUN");
            _runConfirmButton.onClick.AddListener(ConfirmRunName);
            _runNameConfirmLabel = _runConfirmButton.GetComponentInChildren<TextMeshProUGUI>();

            _runNamePanel.SetActive(false);
            _runCategoryRoot.SetActive(false);
        }

        private void ShowRunNamePrompt(VentureData venture)
        {
            if (_runNamePanel == null)
            {
                StartRun(venture);
                return;
            }

            _pendingVenture = venture;
            _pendingTechCategory = null;
            _showingCategoryStage = false;
            _runNameTitleText.text = $"Name your {GetNamingLabel(venture)}";
            _runNameSubtitleText.text = GetRunSetupSubtitle(venture);
            _runNameBodyText.text = GetRunSetupBody(venture);
            _runNameHintText.text = venture != null && venture.requiresRunCategorySelection
                ? "After naming the app, choose which market you want to attack first."
                : "This name will appear on your storefront sign and your saved run.";
            if (_runNamePlaceholderText != null)
                _runNamePlaceholderText.text = venture != null && venture.ventureType == VentureType.TechApp
                    ? "Enter your app name"
                    : "Enter your company name";
            _runNameConfirmLabel.text = venture != null && venture.requiresRunCategorySelection ? "NEXT: PICK CATEGORY" : "START RUN";
            _runConfirmButton.interactable = true;
            _runNameInput.text = !string.IsNullOrWhiteSpace(_pendingRunName)
                ? _pendingRunName
                : (venture != null ? venture.ventureName : "New Venture");
            _runNameInputRoot.SetActive(true);
            _runCategoryRoot.SetActive(false);
            _runBackButton.gameObject.SetActive(false);
            _runNamePanel.SetActive(true);
            _runNameInput.ActivateInputField();
            HighlightSelectedCategory(-1);
        }

        private void ConfirmRunName()
        {
            if (_pendingVenture == null)
                return;

            if (_showingCategoryStage)
            {
                if (_pendingTechCategory == null)
                    return;

                VentureData pendingVenture = _pendingVenture;
                _pendingVenture = null;
                _showingCategoryStage = false;
                _runNamePanel.SetActive(false);
                StartRun(pendingVenture);
                return;
            }

            string enteredName = _runNameInput != null ? _runNameInput.text : string.Empty;
            if (string.IsNullOrWhiteSpace(enteredName))
                enteredName = _pendingVenture.ventureName;

            _pendingRunName = enteredName;

            if (_pendingVenture.requiresRunCategorySelection)
            {
                ShowCategoryStage();
                return;
            }

            VentureData venture = _pendingVenture;
            _pendingVenture = null;
            _runNamePanel.SetActive(false);
            StartRun(venture);
        }

        private void ShowCategoryStage()
        {
            _showingCategoryStage = true;
            _runNameTitleText.text = "Choose your app category";
            _runNameSubtitleText.text = $"{_pendingRunName} needs a market before the first sprint starts.";
            _runNameBodyText.text = "Your category shapes retention pressure, crisis flavor, and the kind of rival tactics you will face first.";
            _runNameHintText.text = "Pick a category with a pressure profile you actually want to manage.";
            _runNameInputRoot.SetActive(false);
            _runCategoryRoot.SetActive(true);
            _runBackButton.gameObject.SetActive(true);
            _runConfirmButton.interactable = _pendingTechCategory != null;
            _runNameConfirmLabel.text = "START APP";
            UpdateSelectedCategorySummary();
        }

        private void ReturnToNameStage()
        {
            if (_pendingVenture == null)
                return;

            _showingCategoryStage = false;
            ShowRunNamePrompt(_pendingVenture);
        }

        private void SelectTechCategory(int index)
        {
            if (_techCategories == null || index < 0 || index >= _techCategories.Length)
                return;

            _pendingTechCategory = _techCategories[index];
            HighlightSelectedCategory(index);
            UpdateSelectedCategorySummary();
            if (_runConfirmButton != null)
                _runConfirmButton.interactable = true;
        }

        private void UpdateSelectedCategorySummary()
        {
            if (_runCategorySummaryText == null)
                return;

            if (_pendingTechCategory == null)
            {
                _runCategorySummaryText.text = "Graphic design, health & fitness, lifestyle, AI, casual games, or hyper casual. Each path changes how growth pressure hits your board.";
                return;
            }

            _runCategorySummaryText.text = $"{GetTechCategoryName(_pendingTechCategory)}\n{GetTechCategorySummary(_pendingTechCategory)}\n{_pendingTechCategory.scenarioNote}";
        }

        private void HighlightSelectedCategory(int selectedIndex)
        {
            if (_categoryButtonImages == null)
                return;

            for (int i = 0; i < _categoryButtonImages.Length; i++)
            {
                bool selected = i == selectedIndex;
                _categoryButtonImages[i].color = selected
                    ? new Color(0.24f, 0.44f, 0.74f, 1f)
                    : new Color(0.16f, 0.16f, 0.20f, 1f);

                if (_categoryButtonLabels != null && i < _categoryButtonLabels.Length && _categoryButtonLabels[i] != null)
                    _categoryButtonLabels[i].color = selected ? Color.white : new Color(0.88f, 0.88f, 0.92f, 1f);
            }
        }

        private static string GetNamingLabel(VentureData venture)
        {
            if (venture == null)
                return "venture";

            return venture.ventureType == VentureType.TechApp ? "app" : "company";
        }

        private static string GetRunSetupSubtitle(VentureData venture)
        {
            if (venture == null)
                return "A saved run starts with a name, a board, and a market to attack.";

            return venture.ventureType switch
            {
                VentureType.TechApp => "App name first. Market category second. Rival enters the same arena.",
                VentureType.FastFood => "Your first storefront, first staff hires, and first supplier pressure begin here.",
                VentureType.Cafe => "Neighborhood loyalty, speed, and drink consistency start with the identity you build here.",
                VentureType.ClothingStore => "Display, stock discipline, and trend pressure all start with the brand on the sign.",
                VentureType.GroceryStore => "Freshness, night traffic, and local trust start with a recognizable neighborhood name.",
                _ => "Give the run a clear identity before the market opens."
            };
        }

        private static string GetRunSetupBody(VentureData venture)
        {
            if (venture == null)
                return "The run name appears on the board sign and in your save file.";

            return venture.ventureType switch
            {
                VentureType.TechApp => "You will manage backend stability, growth spend, reviews, churn, and security shocks. After naming the app, choose whether you compete through creative tooling, health habits, lifestyle loops, AI features, or mobile-game style growth.",
                VentureType.FastFood => "Think beyond a logo. Garson flow, komi support, kitchen speed, store cleaning, pest control, Google reviews, delivery pressure, and meat quality all feed your first 10 turns.",
                VentureType.Cafe => "This run leans on barista rhythm, floor cleanliness, bean quality, pastry freshness, Maps reviews, and social proof. A weak name will not save weak service flow.",
                VentureType.ClothingStore => "Your sign now has to carry display pressure, seasonal inventory bets, return rates, fabric quality, tailoring, and influencer-led demand spikes.",
                VentureType.GroceryStore => "A grocery name should feel local because repeat traffic depends on trust, freshness, shelf discipline, late-night convenience, and neighborhood service reliability.",
                _ => "The name becomes the identity of this run."
            };
        }

        private static string GetTechCategoryName(TechCategoryProfile profile)
        {
            if (profile == null)
                return "Tech Category";

            return LocalizationManager.GetWithFallback(profile.labelKey, profile.displayName);
        }

        private static string GetTechCategorySummary(TechCategoryProfile profile)
        {
            if (profile == null)
                return string.Empty;

            return LocalizationManager.GetWithFallback(profile.summaryKey, profile.summary);
        }

        private static TMP_Text CreatePanelText(string objectName, Transform parent, Vector2 size, Vector2 anchoredPos, float fontSize, FontStyles style)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            var text = go.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.Normal;
            return text;
        }

        private static Button CreateActionButton(string objectName, Transform parent, Vector2 anchoredPos, Vector2 size, Color color, string label)
        {
            var go = new GameObject(objectName);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            var image = go.AddComponent<Image>();
            image.color = color;
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRt = labelGo.AddComponent<RectTransform>();
            labelRt.anchorMin = Vector2.zero;
            labelRt.anchorMax = Vector2.one;
            labelRt.offsetMin = Vector2.zero;
            labelRt.offsetMax = Vector2.zero;
            var text = labelGo.AddComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = 23;
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            return button;
        }

    }
}
