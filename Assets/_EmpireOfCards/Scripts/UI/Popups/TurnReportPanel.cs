using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI.Popups
{
    public class TurnReportPanel : MonoBehaviour
    {
        // ── UI refs ─────────────────────────────────────────────────
        GameObject _root;
        TextMeshProUGUI _reportText;
        float _hideTimer;

        const float AUTO_HIDE_SECONDS = 3f;

        // ── Build (called by HUDBuilder) ────────────────────────────

        public void Build(Transform canvasTransform)
        {
            _root = new GameObject("TurnReportPanel");
            _root.transform.SetParent(canvasTransform, false);

            var rootRect = _root.AddComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(350f, 200f);

            var bg = _root.AddComponent<Image>();
            bg.color = new Color(0.08f, 0.08f, 0.12f, 0.90f);

            // Report text
            var textGo = new GameObject("ReportText");
            textGo.transform.SetParent(_root.transform, false);

            var textRect = textGo.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = new Vector2(-20f, -20f);

            _reportText = textGo.AddComponent<TextMeshProUGUI>();
            _reportText.fontSize = 16f;
            _reportText.color = Color.white;
            _reportText.alignment = TextAlignmentOptions.Center;
            _reportText.enableWordWrapping = true;

            _root.SetActive(false);
        }

        // ── Show / Hide ─────────────────────────────────────────────

        public void Show(int income, int expense, int net, float ratingDelta, int served, int waited)
        {
            string netColor = net >= 0 ? "#4DDB4D" : "#E63333";
            string ratingColor = ratingDelta >= 0 ? "#4DDB4D" : "#E63333";

            string report =
                $"--- TUR RAPORU ---\n\n" +
                $"Gelir: <color=#4DDB4D>${income}</color>\n" +
                $"Gider: <color=#E63333>${expense}</color>\n" +
                $"Net: <color={netColor}>${net}</color>\n\n" +
                $"Rating: <color={ratingColor}>{(ratingDelta >= 0 ? "+" : "")}{ratingDelta:F1}</color>\n" +
                $"Musteri: {served} served / {waited} waited";

            if (_reportText != null) _reportText.text = report;
            if (_root != null) _root.SetActive(true);

            _hideTimer = AUTO_HIDE_SECONDS;
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
            _hideTimer = 0f;
        }

        // ── Auto-hide timer ─────────────────────────────────────────

        void Update()
        {
            if (_hideTimer <= 0f) return;

            _hideTimer -= Time.deltaTime;
            if (_hideTimer <= 0f)
                Hide();
        }
    }
}
