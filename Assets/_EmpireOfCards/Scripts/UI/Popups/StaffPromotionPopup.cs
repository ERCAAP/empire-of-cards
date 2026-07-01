using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI.Popups
{
    public class StaffPromotionPopup : MonoBehaviour
    {
        // ── UI refs ─────────────────────────────────────────────────
        GameObject _root;
        TextMeshProUGUI _titleText;
        TextMeshProUGUI _staffInfoText;
        Button _promoteWithRaise;
        Button _promoteNoRaise;
        Button _declineButton;

        CardData _activeStaff;

        // ── Colors ──────────────────────────────────────────────────
        static readonly Color COL_BG = new Color(0.10f, 0.10f, 0.15f, 0.95f);
        static readonly Color COL_OVERLAY = new Color(0f, 0f, 0f, 0.7f);
        static readonly Color COL_GOLD = new Color(0.95f, 0.80f, 0.20f);
        static readonly Color COL_GREEN = new Color(0.25f, 0.65f, 0.25f);
        static readonly Color COL_BLUE = new Color(0.20f, 0.45f, 0.75f);
        static readonly Color COL_RED = new Color(0.65f, 0.22f, 0.22f);

        // ── Build ───────────────────────────────────────────────────

        public void Build(Transform canvasTransform)
        {
            // Dark overlay
            _root = new GameObject("StaffPromotionPopup");
            _root.transform.SetParent(canvasTransform, false);

            var rootRect = _root.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;

            var overlay = _root.AddComponent<Image>();
            overlay.color = COL_OVERLAY;

            // Center panel
            var panel = CreatePanel(_root.transform, 440f, 320f, COL_BG);

            // Title
            _titleText = CreateText(panel.transform, "TERFI FIRSATI", 26f,
                new Vector2(0f, 120f), COL_GOLD, FontStyles.Bold);

            // Staff info
            _staffInfoText = CreateText(panel.transform, "", 16f,
                new Vector2(0f, 55f), Color.white, FontStyles.Normal,
                new Vector2(400f, 60f));

            // Choice 1: Promote + Raise
            _promoteWithRaise = CreateChoiceButton(panel.transform,
                "Terfi + Zam Ver", "Moral yukselir, maas artar",
                new Vector2(0f, -10f), COL_GREEN);
            _promoteWithRaise.onClick.AddListener(() => OnChoice(0));

            // Choice 2: Promote, No Raise
            _promoteNoRaise = CreateChoiceButton(panel.transform,
                "Terfi Ver (Zamsiz)", "Moral biraz artar, risk: kusmek",
                new Vector2(0f, -65f), COL_BLUE);
            _promoteNoRaise.onClick.AddListener(() => OnChoice(1));

            // Choice 3: Decline
            _declineButton = CreateChoiceButton(panel.transform,
                "Terfi Verme", "Moral duser, istifa riski",
                new Vector2(0f, -120f), COL_RED);
            _declineButton.onClick.AddListener(() => OnChoice(2));

            _root.SetActive(false);
        }

        // ── EventBus ────────────────────────────────────────────────

        void OnEnable()
        {
            EventBus.OnStaffPromotionAvailable += HandlePromotionAvailable;
        }

        void OnDisable()
        {
            EventBus.OnStaffPromotionAvailable -= HandlePromotionAvailable;
        }

        // ── Handlers ────────────────────────────────────────────────

        void HandlePromotionAvailable(CardData staff, StaffTier currentTier)
        {
            _activeStaff = staff;

            string staffName = staff != null ? staff.cardName : "Personel";
            string tierName = currentTier.ToString();
            StaffTier nextTier = currentTier + 1;

            if (_titleText != null)
                _titleText.text = "TERFI FIRSATI";

            if (_staffInfoText != null)
                _staffInfoText.text = $"{staffName}\n{tierName} -> {nextTier}";

            Show();
        }

        void OnChoice(int choice)
        {
            if (_activeStaff != null)
                EventBus.PromotionChoiceMade(_activeStaff, choice);

            Hide();
        }

        // ── Show / Hide ─────────────────────────────────────────────

        public void Show()
        {
            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
            _activeStaff = null;
        }

        // ── UI Helpers ──────────────────────────────────────────────

        GameObject CreatePanel(Transform parent, float w, float h, Color color)
        {
            var go = new GameObject("Panel");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(w, h);
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        Button CreateChoiceButton(Transform parent, string label, string desc,
            Vector2 pos, Color bgColor)
        {
            var go = new GameObject($"Btn_{label}");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(380f, 44f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            // Label
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var lRect = labelGo.AddComponent<RectTransform>();
            lRect.anchorMin = new Vector2(0f, 0f);
            lRect.anchorMax = new Vector2(0.55f, 1f);
            lRect.sizeDelta = Vector2.zero;
            lRect.offsetMin = new Vector2(12f, 0f);
            var lTmp = labelGo.AddComponent<TextMeshProUGUI>();
            lTmp.text = label;
            lTmp.fontSize = 15f;
            lTmp.color = Color.white;
            lTmp.fontStyle = FontStyles.Bold;
            lTmp.alignment = TextAlignmentOptions.MidlineLeft;

            // Description
            var descGo = new GameObject("Desc");
            descGo.transform.SetParent(go.transform, false);
            var dRect = descGo.AddComponent<RectTransform>();
            dRect.anchorMin = new Vector2(0.55f, 0f);
            dRect.anchorMax = new Vector2(1f, 1f);
            dRect.sizeDelta = Vector2.zero;
            dRect.offsetMax = new Vector2(-8f, 0f);
            var dTmp = descGo.AddComponent<TextMeshProUGUI>();
            dTmp.text = desc;
            dTmp.fontSize = 12f;
            dTmp.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            dTmp.fontStyle = FontStyles.Italic;
            dTmp.alignment = TextAlignmentOptions.MidlineRight;
            dTmp.enableWordWrapping = true;

            return btn;
        }

        TextMeshProUGUI CreateText(Transform parent, string text, float size,
            Vector2 pos, Color color, FontStyles style = FontStyles.Normal,
            Vector2? customSize = null)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = customSize ?? new Vector2(400f, 40f);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = true;
            return tmp;
        }
    }
}
