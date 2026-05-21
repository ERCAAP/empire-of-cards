using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI
{
    public class GameSetupPanel : MonoBehaviour
    {
        // ── UI refs ─────────────────────────────────────────────────
        GameObject _root;
        GameObject _step1;
        GameObject _step2;
        TMP_InputField _nameInput;
        Button _continueButton;
        Button _startButton;
        SectorType _selectedSector;
        bool _sectorChosen;
        Image _selectedCard;

        // ── Colors ──────────────────────────────────────────────────
        static readonly Color COL_BG = new Color(0.08f, 0.08f, 0.12f, 0.95f);
        static readonly Color COL_GOLD = new Color(0.95f, 0.80f, 0.20f);
        static readonly Color COL_GREEN = new Color(0.20f, 0.70f, 0.25f);
        static readonly Color COL_LOCKED = new Color(0.3f, 0.3f, 0.3f, 0.6f);
        static readonly Color COL_CARD = new Color(0.15f, 0.15f, 0.22f);
        static readonly Color COL_SELECTED = new Color(0.25f, 0.50f, 0.25f);

        // ── Build ───────────────────────────────────────────────────

        public void Build(Transform canvasTransform)
        {
            // Root overlay
            _root = new GameObject("GameSetupPanel");
            _root.transform.SetParent(canvasTransform, false);
            var rootRect = _root.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;
            var overlay = _root.AddComponent<Image>();
            overlay.color = COL_BG;

            BuildStep1();
            BuildStep2();

            _step1.SetActive(true);
            _step2.SetActive(false);
            _root.SetActive(false);
        }

        // ── Step 1: Business Name ───────────────────────────────────

        void BuildStep1()
        {
            _step1 = new GameObject("Step1_Name");
            _step1.transform.SetParent(_root.transform, false);
            var s1Rect = _step1.AddComponent<RectTransform>();
            s1Rect.anchorMin = Vector2.zero;
            s1Rect.anchorMax = Vector2.one;
            s1Rect.sizeDelta = Vector2.zero;

            // Title
            CreateText(_step1.transform, "ISLETMENE BIR AD VER", 32f,
                new Vector2(0f, 150f), COL_GOLD, FontStyles.Bold);

            // Input field background
            var inputBg = new GameObject("InputBg");
            inputBg.transform.SetParent(_step1.transform, false);
            var ibRect = inputBg.AddComponent<RectTransform>();
            ibRect.anchoredPosition = new Vector2(0f, 30f);
            ibRect.sizeDelta = new Vector2(400f, 50f);
            var ibImg = inputBg.AddComponent<Image>();
            ibImg.color = new Color(0.18f, 0.18f, 0.25f);

            // TMP_InputField
            _nameInput = inputBg.AddComponent<TMP_InputField>();
            _nameInput.characterLimit = 30;

            // Text area
            var textArea = new GameObject("TextArea");
            textArea.transform.SetParent(inputBg.transform, false);
            var taRect = textArea.AddComponent<RectTransform>();
            taRect.anchorMin = Vector2.zero;
            taRect.anchorMax = Vector2.one;
            taRect.offsetMin = new Vector2(10f, 5f);
            taRect.offsetMax = new Vector2(-10f, -5f);
            textArea.AddComponent<RectMask2D>();

            // Input text child
            var inputTextGo = new GameObject("Text");
            inputTextGo.transform.SetParent(textArea.transform, false);
            var itRect = inputTextGo.AddComponent<RectTransform>();
            itRect.anchorMin = Vector2.zero;
            itRect.anchorMax = Vector2.one;
            itRect.sizeDelta = Vector2.zero;
            var inputTMP = inputTextGo.AddComponent<TextMeshProUGUI>();
            inputTMP.fontSize = 18f;
            inputTMP.color = Color.white;
            inputTMP.alignment = TextAlignmentOptions.MidlineLeft;
            _nameInput.textComponent = inputTMP;

            // Placeholder
            var placeholderGo = new GameObject("Placeholder");
            placeholderGo.transform.SetParent(textArea.transform, false);
            var phRect = placeholderGo.AddComponent<RectTransform>();
            phRect.anchorMin = Vector2.zero;
            phRect.anchorMax = Vector2.one;
            phRect.sizeDelta = Vector2.zero;
            var phTMP = placeholderGo.AddComponent<TextMeshProUGUI>();
            phTMP.text = "Ornek: Omer'in Doneri";
            phTMP.fontSize = 18f;
            phTMP.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
            phTMP.fontStyle = FontStyles.Italic;
            phTMP.alignment = TextAlignmentOptions.MidlineLeft;
            _nameInput.placeholder = phTMP;

            // Continue button
            _continueButton = CreateButton(_step1.transform, "DEVAM", new Vector2(0f, -60f),
                COL_GREEN, 200f, 45f);
            _continueButton.interactable = false;
            _continueButton.onClick.AddListener(OnContinueClicked);

            // Listen for input changes
            _nameInput.onValueChanged.AddListener(OnNameChanged);
        }

        // ── Step 2: Sector Selection ────────────────────────────────

        void BuildStep2()
        {
            _step2 = new GameObject("Step2_Sector");
            _step2.transform.SetParent(_root.transform, false);
            var s2Rect = _step2.AddComponent<RectTransform>();
            s2Rect.anchorMin = Vector2.zero;
            s2Rect.anchorMax = Vector2.one;
            s2Rect.sizeDelta = Vector2.zero;

            CreateText(_step2.transform, "SEKTOR SEC", 32f,
                new Vector2(0f, 220f), COL_GOLD, FontStyles.Bold);

            // Sector cards
            var sectors = new[]
            {
                (SectorType.Restaurant, "Restoran", "Yemek isleri ile basla", true, ""),
                (SectorType.TechApp,    "Teknoloji", "Uygulama gelistir", false, "1 basarili oyun"),
                (SectorType.Fashion,    "Moda",     "Giyim markasi kur",   false, "2 basarili oyun"),
                (SectorType.Grocery,    "Market",   "Bakkal zinciri yonet", false, "3 basarili oyun"),
                (SectorType.Fintech,    "Fintek",   "Finans teknolojisi",  false, "5 basarili oyun")
            };

            float startX = -360f;
            for (int i = 0; i < sectors.Length; i++)
            {
                var s = sectors[i];
                float xPos = startX + i * 180f;
                CreateSectorCard(_step2.transform, s.Item1, s.Item2, s.Item3,
                    s.Item4, s.Item5, new Vector2(xPos, 40f));
            }

            // Start button
            _startButton = CreateButton(_step2.transform, "BASLA", new Vector2(0f, -180f),
                COL_GREEN, 200f, 45f);
            _startButton.interactable = false;
            _startButton.onClick.AddListener(OnStartClicked);
        }

        // ── Sector Card ─────────────────────────────────────────────

        void CreateSectorCard(Transform parent, SectorType sector, string name,
            string desc, bool unlocked, string unlockText, Vector2 pos)
        {
            var go = new GameObject($"Card_{sector}");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(160f, 220f);

            var cardImg = go.AddComponent<Image>();
            cardImg.color = unlocked ? COL_CARD : COL_LOCKED;

            // Sector name
            CreateText(go.transform, name, 18f, new Vector2(0f, 70f),
                unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f), FontStyles.Bold,
                new Vector2(140f, 30f));

            // Description
            CreateText(go.transform, desc, 13f, new Vector2(0f, 30f),
                unlocked ? new Color(0.8f, 0.8f, 0.8f) : new Color(0.4f, 0.4f, 0.4f),
                FontStyles.Normal, new Vector2(140f, 40f));

            if (!unlocked)
            {
                // Lock icon text
                CreateText(go.transform, "[KILITLI]", 14f, new Vector2(0f, -20f),
                    new Color(0.8f, 0.3f, 0.3f), FontStyles.Bold, new Vector2(140f, 25f));
                CreateText(go.transform, unlockText, 11f, new Vector2(0f, -50f),
                    new Color(0.5f, 0.5f, 0.5f), FontStyles.Italic, new Vector2(140f, 25f));
            }
            else
            {
                var btn = go.AddComponent<Button>();
                btn.targetGraphic = cardImg;
                btn.onClick.AddListener(() => OnSectorSelected(sector, cardImg));
            }
        }

        // ── Callbacks ───────────────────────────────────────────────

        void OnNameChanged(string value)
        {
            bool valid = value != null && value.Trim().Length > 2;
            _continueButton.interactable = valid;
            var colors = _continueButton.colors;
            var img = _continueButton.GetComponent<Image>();
            if (img != null) img.color = valid ? COL_GREEN : COL_GREEN * 0.4f;
        }

        void OnContinueClicked()
        {
            _step1.SetActive(false);
            _step2.SetActive(true);
        }

        void OnSectorSelected(SectorType sector, Image cardImage)
        {
            if (_selectedCard != null)
                _selectedCard.color = COL_CARD;

            _selectedCard = cardImage;
            _selectedCard.color = COL_SELECTED;
            _selectedSector = sector;
            _sectorChosen = true;

            _startButton.interactable = true;
            var img = _startButton.GetComponent<Image>();
            if (img != null) img.color = COL_GREEN;
        }

        void OnStartClicked()
        {
            if (!_sectorChosen) return;

            string name = _nameInput.text.Trim();
            EventBus.SetupComplete(name, _selectedSector);
            Hide();
        }

        // ── Show / Hide ─────────────────────────────────────────────

        public void Show()
        {
            _step1.SetActive(true);
            _step2.SetActive(false);
            _sectorChosen = false;
            _selectedCard = null;
            if (_nameInput != null) _nameInput.text = "";
            if (_continueButton != null) _continueButton.interactable = false;
            if (_startButton != null) _startButton.interactable = false;
            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        // ── Helpers ─────────────────────────────────────────────────

        TextMeshProUGUI CreateText(Transform parent, string text, float size,
            Vector2 pos, Color color, FontStyles style = FontStyles.Normal,
            Vector2? customSize = null)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = customSize ?? new Vector2(600f, 50f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = true;
            return tmp;
        }

        Button CreateButton(Transform parent, string label, Vector2 pos,
            Color bgColor, float w, float h)
        {
            var go = new GameObject($"Btn_{label}");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(w, h);
            var img = go.AddComponent<Image>();
            img.color = bgColor * 0.4f;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 18f;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            return btn;
        }
    }
}
