using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Save;

namespace EmpireOfCards.Bootstrap
{
    /// <summary>
    /// Main menu controller. Builds the menu UI programmatically on Start().
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        private enum SaveListMode
        {
            Load,
            Delete
        }

        [SerializeField] private string gameScene = "Game";

        private Button _loadButton;
        private Button _deleteButton;
        private TMP_Text _loadHintText;
        private GameObject _saveListPanel;
        private RectTransform _saveListContent;
        private TMP_Text _saveListTitle;
        private SaveListMode _saveListMode;

        private void Start()
        {
            EnsureLocalization();
            EnsureSaveManager();
            CreateMenuUI();
        }

        private void CreateMenuUI()
        {
            GameObject canvasGo = BuildCanvas();
            BuildBackground(canvasGo.transform);
            BuildHeader(canvasGo.transform);

            CreateMenuButton(canvasGo.transform, "StartButton", new Vector2(0, -18), new Vector2(320, 62), new Color(0.20f, 0.62f, 0.31f), "ui.start_game", "START GAME", OnStartClicked);
            _loadButton = CreateMenuButton(canvasGo.transform, "LoadButton", new Vector2(0, -102), new Vector2(320, 56), new Color(0.20f, 0.33f, 0.58f), "ui.load_game", "LOAD GAME", OnLoadClicked);
            _deleteButton = CreateMenuButton(canvasGo.transform, "DeleteButton", new Vector2(0, -174), new Vector2(320, 50), new Color(0.45f, 0.26f, 0.16f), "menu.delete_save", "DELETE SAVE", OnDeleteClicked);
            CreateMenuButton(canvasGo.transform, "QuitButton", new Vector2(0, -246), new Vector2(220, 48), new Color(0.50f, 0.20f, 0.20f), "ui.quit", "QUIT", OnQuitClicked);

            GameObject loadHintGo = new GameObject("LoadHint");
            loadHintGo.transform.SetParent(canvasGo.transform, false);
            RectTransform loadHintRT = loadHintGo.AddComponent<RectTransform>();
            loadHintRT.anchoredPosition = new Vector2(0, -312);
            loadHintRT.sizeDelta = new Vector2(720, 48);
            _loadHintText = loadHintGo.AddComponent<TextMeshProUGUI>();
            _loadHintText.fontSize = 18;
            _loadHintText.alignment = TextAlignmentOptions.Center;
            _loadHintText.color = new Color(0.72f, 0.72f, 0.76f);

            BuildVersionLabel(canvasGo.transform);
            EnsureEventSystem();
            CreateSaveListPanel(canvasGo.transform);
            RefreshSaveButtons();
        }

        private static GameObject BuildCanvas()
        {
            GameObject canvasGo = new GameObject("MenuCanvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();
            return canvasGo;
        }

        private static void BuildBackground(Transform parent)
        {
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(parent, false);
            RectTransform bgRT = bg.AddComponent<RectTransform>();
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = Vector2.zero;
            bgRT.offsetMax = Vector2.zero;
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.08f, 0.06f, 0.12f);
        }

        private static void BuildHeader(Transform parent)
        {
            TMP_Text titleText = CreateText(parent, "Title", new Vector2(0, 150), new Vector2(860, 120), 72, FontStyles.Bold, new Color(1f, 0.85f, 0.4f));
            titleText.text = LocalizationManager.GetWithFallback("menu.title", "EMPIRE OF CARDS");

            TMP_Text subText = CreateText(parent, "Subtitle", new Vector2(0, 78), new Vector2(700, 52), 24, FontStyles.Normal, new Color(0.7f, 0.7f, 0.7f));
            subText.text = LocalizationManager.GetWithFallback("menu.subtitle", "Pick a venture, build your board, and dominate the market.");
        }

        private static void BuildVersionLabel(Transform parent)
        {
            TMP_Text verText = CreateText(parent, "Version", new Vector2(-20, 10), new Vector2(220, 30), 16, FontStyles.Normal, new Color(0.4f, 0.4f, 0.4f));
            RectTransform verRT = verText.rectTransform;
            verRT.anchorMin = new Vector2(1, 0);
            verRT.anchorMax = new Vector2(1, 0);
            verRT.pivot = new Vector2(1, 0);
            verText.alignment = TextAlignmentOptions.Right;
            verText.text = LocalizationManager.GetWithFallback("menu.version", "v0.1 MVP");
        }

        private static Button CreateMenuButton(Transform parent, string name, Vector2 position, Vector2 size, Color color, string labelKey, string fallbackLabel, UnityEngine.Events.UnityAction action)
        {
            GameObject buttonGo = new GameObject(name);
            buttonGo.transform.SetParent(parent, false);
            RectTransform buttonRT = buttonGo.AddComponent<RectTransform>();
            buttonRT.anchoredPosition = position;
            buttonRT.sizeDelta = size;
            Image buttonImg = buttonGo.AddComponent<Image>();
            buttonImg.color = color;
            Button button = buttonGo.AddComponent<Button>();
            button.targetGraphic = buttonImg;
            button.onClick.AddListener(action);

            TMP_Text label = CreateText(buttonGo.transform, "Label", Vector2.zero, size, 24, FontStyles.Bold, Color.white);
            label.text = LocalizationManager.GetWithFallback(labelKey, fallbackLabel);
            return button;
        }

        private void CreateSaveListPanel(Transform parent)
        {
            _saveListPanel = new GameObject("SaveListPanel");
            _saveListPanel.transform.SetParent(parent, false);
            RectTransform panelRT = _saveListPanel.AddComponent<RectTransform>();
            panelRT.anchorMin = Vector2.zero;
            panelRT.anchorMax = Vector2.one;
            panelRT.offsetMin = Vector2.zero;
            panelRT.offsetMax = Vector2.zero;
            Image panelBg = _saveListPanel.AddComponent<Image>();
            panelBg.color = new Color(0.03f, 0.03f, 0.08f, 0.92f);

            GameObject card = new GameObject("SaveListCard");
            card.transform.SetParent(_saveListPanel.transform, false);
            RectTransform cardRT = card.AddComponent<RectTransform>();
            cardRT.anchorMin = new Vector2(0.5f, 0.5f);
            cardRT.anchorMax = new Vector2(0.5f, 0.5f);
            cardRT.pivot = new Vector2(0.5f, 0.5f);
            cardRT.anchoredPosition = Vector2.zero;
            cardRT.sizeDelta = new Vector2(980f, 640f);
            Image cardBg = card.AddComponent<Image>();
            cardBg.color = new Color(0.09f, 0.09f, 0.12f, 0.98f);

            _saveListTitle = CreateText(card.transform, "SaveListTitle", new Vector2(0, 262), new Vector2(760, 56), 34, FontStyles.Bold, Color.white);
            TMP_Text subtitle = CreateText(card.transform, "SaveListSubtitle", new Vector2(0, 220), new Vector2(820, 34), 18, FontStyles.Normal, new Color(0.76f, 0.78f, 0.84f));
            subtitle.text = LocalizationManager.GetWithFallback("menu.save_list_hint", "Choose a saved company to load or remove.");

            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(card.transform, false);
            RectTransform viewportRT = viewport.AddComponent<RectTransform>();
            viewportRT.anchorMin = new Vector2(0.5f, 0.5f);
            viewportRT.anchorMax = new Vector2(0.5f, 0.5f);
            viewportRT.pivot = new Vector2(0.5f, 0.5f);
            viewportRT.anchoredPosition = new Vector2(0, -10);
            viewportRT.sizeDelta = new Vector2(860f, 430f);
            Image viewportBg = viewport.AddComponent<Image>();
            viewportBg.color = new Color(0.13f, 0.13f, 0.17f, 1f);
            Mask mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            _saveListContent = content.AddComponent<RectTransform>();
            _saveListContent.anchorMin = new Vector2(0f, 1f);
            _saveListContent.anchorMax = new Vector2(1f, 1f);
            _saveListContent.pivot = new Vector2(0.5f, 1f);
            _saveListContent.anchoredPosition = new Vector2(0f, -16f);
            _saveListContent.sizeDelta = new Vector2(0f, 0f);

            Button closeButton = CreateMenuButton(card.transform, "CloseSaveList", new Vector2(0, -276), new Vector2(220, 50), new Color(0.28f, 0.22f, 0.18f), "ui.back", "BACK", CloseSaveListPanel);
            closeButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 22;

            _saveListPanel.SetActive(false);
        }

        private void OpenSaveListPanel(SaveListMode mode)
        {
            _saveListMode = mode;
            _saveListTitle.text = mode == SaveListMode.Load
                ? LocalizationManager.GetWithFallback("menu.load_game_title", "LOAD GAME")
                : LocalizationManager.GetWithFallback("menu.delete_save", "DELETE SAVE");

            PopulateSaveList();
            _saveListPanel.SetActive(true);
        }

        private void PopulateSaveList()
        {
            if (_saveListContent == null)
                return;

            for (int i = _saveListContent.childCount - 1; i >= 0; i--)
                Destroy(_saveListContent.GetChild(i).gameObject);

            List<RunSaveData> runs = SaveManager.Instance != null ? SaveManager.Instance.ListRuns() : new List<RunSaveData>();
            runs.Sort((a, b) => b.savedAtUnixSeconds.CompareTo(a.savedAtUnixSeconds));

            if (runs.Count == 0)
            {
                TMP_Text emptyText = CreateText(_saveListContent, "Empty", new Vector2(0, -70), new Vector2(760, 40), 22, FontStyles.Italic, new Color(0.72f, 0.72f, 0.78f));
                emptyText.text = LocalizationManager.GetWithFallback("menu.no_save", "No saved company yet.");
                _saveListContent.sizeDelta = new Vector2(0f, 120f);
                return;
            }

            const float rowHeight = 112f;
            for (int i = 0; i < runs.Count; i++)
            {
                RunSaveData run = runs[i];
                GameObject row = new GameObject($"RunRow_{run.slotId}");
                row.transform.SetParent(_saveListContent, false);
                RectTransform rowRT = row.AddComponent<RectTransform>();
                rowRT.anchorMin = new Vector2(0f, 1f);
                rowRT.anchorMax = new Vector2(1f, 1f);
                rowRT.pivot = new Vector2(0.5f, 1f);
                rowRT.anchoredPosition = new Vector2(0f, -i * rowHeight);
                rowRT.sizeDelta = new Vector2(0f, 96f);
                Image rowBg = row.AddComponent<Image>();
                rowBg.color = i % 2 == 0 ? new Color(0.17f, 0.17f, 0.21f, 1f) : new Color(0.14f, 0.14f, 0.18f, 1f);

                string ventureName = FormatVentureName((VentureType)run.ventureType);
                string metaLine = ventureName;
                if (!string.IsNullOrWhiteSpace(run.runCategoryLabel))
                    metaLine += $"  |  {run.runCategoryLabel}";
                metaLine += $"  |  {LocalizationManager.GetWithFallback("ui.turn", "Turn")} {Mathf.Max(1, run.currentTurn)}";

                TMP_Text nameText = CreateText(row.transform, "RunName", new Vector2(-250f, 20f), new Vector2(500f, 30f), 26, FontStyles.Bold, Color.white);
                nameText.alignment = TextAlignmentOptions.MidlineLeft;
                nameText.text = run.runName;

                TMP_Text metaText = CreateText(row.transform, "RunMeta", new Vector2(-250f, -12f), new Vector2(560f, 26f), 17, FontStyles.Normal, new Color(0.78f, 0.80f, 0.86f));
                metaText.alignment = TextAlignmentOptions.MidlineLeft;
                metaText.text = metaLine;

                TMP_Text timeText = CreateText(row.transform, "RunTime", new Vector2(-250f, -40f), new Vector2(560f, 22f), 15, FontStyles.Italic, new Color(0.62f, 0.66f, 0.74f));
                timeText.alignment = TextAlignmentOptions.MidlineLeft;
                timeText.text = BuildSaveTimestamp(run);

                string actionKey = _saveListMode == SaveListMode.Load ? "ui.load_game" : "menu.delete_save";
                string actionFallback = _saveListMode == SaveListMode.Load ? "LOAD GAME" : "DELETE SAVE";
                Color actionColor = _saveListMode == SaveListMode.Load ? new Color(0.20f, 0.40f, 0.68f) : new Color(0.58f, 0.23f, 0.19f);
                Button actionButton = CreateMenuButton(row.transform, "ActionButton", new Vector2(300f, 0f), new Vector2(180f, 48f), actionColor, actionKey, actionFallback, () => HandleRunAction(run));
                actionButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 20;
            }

            _saveListContent.sizeDelta = new Vector2(0f, Mathf.Max(430f, runs.Count * rowHeight + 24f));
        }

        private void HandleRunAction(RunSaveData run)
        {
            if (run == null || string.IsNullOrWhiteSpace(run.slotId))
                return;

            if (_saveListMode == SaveListMode.Load)
            {
                RunLaunchConfig.PrepareLoadRun(run.slotId);
                SceneManager.LoadScene(gameScene);
                return;
            }

            SaveManager.Instance?.DeleteRunSave(run.slotId);
            RefreshSaveButtons();
            PopulateSaveList();
        }

        private void CloseSaveListPanel()
        {
            if (_saveListPanel != null)
                _saveListPanel.SetActive(false);
        }

        private void OnStartClicked()
        {
            RunLaunchConfig.PrepareNewRun();
            SceneManager.LoadScene(gameScene);
        }

        private void OnLoadClicked()
        {
            if (SaveManager.Instance == null || !SaveManager.Instance.HasRunSave())
                return;

            OpenSaveListPanel(SaveListMode.Load);
        }

        private void OnDeleteClicked()
        {
            if (SaveManager.Instance == null || !SaveManager.Instance.HasRunSave())
                return;

            OpenSaveListPanel(SaveListMode.Delete);
        }

        private static void EnsureLocalization()
        {
            if (LocalizationManager.Instance != null)
                return;

            GameObject go = new GameObject("[LocalizationManager]");
            go.AddComponent<LocalizationManager>();
        }

        private static void EnsureSaveManager()
        {
            if (SaveManager.Instance != null)
                return;

            GameObject go = new GameObject("[SaveManager]");
            go.AddComponent<SaveManager>();
        }

        private void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void RefreshSaveButtons()
        {
            bool hasRuns = SaveManager.Instance != null && SaveManager.Instance.HasRunSave();
            if (_loadButton != null)
                _loadButton.interactable = hasRuns;
            if (_deleteButton != null)
                _deleteButton.interactable = hasRuns;

            if (_loadHintText == null)
                return;

            if (!hasRuns)
            {
                _loadHintText.text = LocalizationManager.GetWithFallback("menu.no_save", "No saved company yet.");
                return;
            }

            RunSaveData latestRun = SaveManager.Instance.LoadRun();
            string runName = latestRun != null ? latestRun.runName : LocalizationManager.GetWithFallback("menu.saved_run", "Saved Run");
            string ventureLabel = latestRun != null ? FormatVentureName((VentureType)latestRun.ventureType) : string.Empty;
            _loadHintText.text = $"{LocalizationManager.GetWithFallback("menu.saved_run", "Latest save")}: {runName} ({ventureLabel})";
        }

        private static TMP_Text CreateText(Transform parent, string name, Vector2 position, Vector2 size, float fontSize, FontStyles style, Color color)
        {
            GameObject textGo = new GameObject(name);
            textGo.transform.SetParent(parent, false);
            RectTransform rt = textGo.AddComponent<RectTransform>();
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
            TMP_Text text = textGo.AddComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = TextAlignmentOptions.Center;
            text.color = color;
            return text;
        }

        private static void EnsureEventSystem()
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                GameObject esGo = new GameObject("EventSystem");
                esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGo.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }

        private static string FormatVentureName(VentureType ventureType)
        {
            switch (ventureType)
            {
                case VentureType.FastFood:
                    return "Fast Food";
                case VentureType.Cafe:
                    return "Cafe";
                case VentureType.TechApp:
                    return "Tech App";
                case VentureType.ClothingStore:
                    return "Clothing Store";
                case VentureType.GroceryStore:
                    return "Grocery Store";
                default:
                    return ventureType.ToString();
            }
        }

        private static string BuildSaveTimestamp(RunSaveData run)
        {
            if (run == null || run.savedAtUnixSeconds <= 0)
                return LocalizationManager.GetWithFallback("menu.just_now", "Saved recently");

            DateTimeOffset localTime = DateTimeOffset.FromUnixTimeSeconds(run.savedAtUnixSeconds).ToLocalTime();
            return $"{LocalizationManager.GetWithFallback("menu.last_saved", "Last saved")}: {localTime:dd MMM yyyy HH:mm}";
        }
    }
}
