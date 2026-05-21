using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI.Popups
{
    public class CrisisPopup : MonoBehaviour
    {
        // ── UI refs ─────────────────────────────────────────────────
        GameObject _root;
        TextMeshProUGUI _titleText;
        TextMeshProUGUI _descText;
        Button _choiceAButton;
        TextMeshProUGUI _choiceAText;
        Button _choiceBButton;
        TextMeshProUGUI _choiceBText;

        CrisisType _activeCrisis;

        // ── Build (called by HUDBuilder) ────────────────────────────

        public void Build(Transform canvasTransform)
        {
            // Dark overlay
            _root = new GameObject("CrisisPopup");
            _root.transform.SetParent(canvasTransform, false);

            var rootRect = _root.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.sizeDelta = Vector2.zero;

            var overlay = _root.AddComponent<Image>();
            overlay.color = new Color(0f, 0f, 0f, 0.7f);

            // Center panel
            var panel = CreatePanel(_root.transform, 400f, 280f, new Color(0.15f, 0.10f, 0.10f, 0.95f));

            // Title
            _titleText = CreateText(panel.transform, "KRIZ!", 24f, new Vector2(0f, 100f),
                new Color(0.95f, 0.20f, 0.20f));

            // Description
            _descText = CreateText(panel.transform, "Kriz aciklamasi...", 16f, new Vector2(0f, 30f),
                Color.white);

            // Choice A button
            _choiceAButton = CreateButton(panel.transform, "Secim A", new Vector2(-90f, -80f),
                new Color(0.25f, 0.60f, 0.25f));
            _choiceAText = _choiceAButton.GetComponentInChildren<TextMeshProUGUI>();
            _choiceAButton.onClick.AddListener(() => OnChoiceSelected("A"));

            // Choice B button
            _choiceBButton = CreateButton(panel.transform, "Secim B", new Vector2(90f, -80f),
                new Color(0.60f, 0.25f, 0.25f));
            _choiceBText = _choiceBButton.GetComponentInChildren<TextMeshProUGUI>();
            _choiceBButton.onClick.AddListener(() => OnChoiceSelected("B"));

            _root.SetActive(false);
        }

        // ── Show / Hide ─────────────────────────────────────────────

        public void Show(CrisisType crisis, string description)
        {
            _activeCrisis = crisis;

            if (_titleText != null) _titleText.text = $"KRIZ: {crisis}";
            if (_descText != null) _descText.text = description;
            if (_choiceAText != null) _choiceAText.text = "Mudahale Et ($)";
            if (_choiceBText != null) _choiceBText.text = "Gormezden Gel";

            if (_root != null) _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        // ── Choice callback ─────────────────────────────────────────

        void OnChoiceSelected(string choiceId)
        {
            EventBus.CrisisResolved(_activeCrisis, choiceId);
            Hide();
        }

        // ── UI helpers ──────────────────────────────────────────────

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

        TextMeshProUGUI CreateText(Transform parent, string text, float size, Vector2 pos, Color color)
        {
            var go = new GameObject("Text");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(360f, 60f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableWordWrapping = true;

            return tmp;
        }

        Button CreateButton(Transform parent, string label, Vector2 pos, Color bgColor)
        {
            var go = new GameObject($"Btn_{label}");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(150f, 40f);

            var img = go.AddComponent<Image>();
            img.color = bgColor;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            // Button text
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);

            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            var tmp = textGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 14f;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            return btn;
        }
    }
}
