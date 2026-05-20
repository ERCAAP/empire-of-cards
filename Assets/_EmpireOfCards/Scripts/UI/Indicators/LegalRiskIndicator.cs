using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI.Indicators
{
    /// <summary>
    /// Displays legal risk (0-100) as a color-coded fill bar with numeric text.
    /// Colors:  0-25  green, 26-50  yellow, 51-100  red.
    /// Subscribes to EventBus.OnLegalRiskChanged.
    /// </summary>
    public class LegalRiskIndicator : MonoBehaviour
    {
        private Image   _barFill;
        private TMP_Text _valueText;

        private const float BAR_FULL_WIDTH = 140f;

        private static readonly Color ColorSafe    = new Color(0.2f,  0.85f, 0.35f); // green
        private static readonly Color ColorCaution = new Color(1f,    0.85f, 0.1f);  // yellow
        private static readonly Color ColorDanger  = new Color(0.9f,  0.35f, 0.1f);  // red

        public void Init(Image barFill, TMP_Text valueText)
        {
            _barFill   = barFill;
            _valueText = valueText;
            Refresh(Constants.LEGAL_RISK_MIN);
        }

        private void OnEnable()  => EventBus.OnLegalRiskChanged += OnRiskChanged;
        private void OnDisable() => EventBus.OnLegalRiskChanged -= OnRiskChanged;

        private void OnRiskChanged(int score) => Refresh(score);

        private void Refresh(int score)
        {
            int clamped = Mathf.Clamp(score,
                Constants.LEGAL_RISK_MIN,
                Constants.LEGAL_RISK_MAX);

            float t = clamped / (float)Constants.LEGAL_RISK_MAX;

            if (_barFill != null)
            {
                _barFill.rectTransform.sizeDelta =
                    new Vector2(BAR_FULL_WIDTH * t, _barFill.rectTransform.sizeDelta.y);
                _barFill.color = GetColor(clamped);
            }

            if (_valueText != null)
            {
                _valueText.text  = clamped.ToString();
                _valueText.color = GetColor(clamped);
            }
        }

        private static Color GetColor(int score)
        {
            // 0-25: safe (green)
            if (score <= Constants.LEGAL_RISK_CAUTION_THRESHOLD - 1)
                return ColorSafe;

            // 26-50: caution — lerp from green to yellow
            if (score <= Constants.LEGAL_RISK_DANGER_THRESHOLD - 1)
            {
                float t = (score - Constants.LEGAL_RISK_CAUTION_THRESHOLD)
                        / (float)(Constants.LEGAL_RISK_DANGER_THRESHOLD - Constants.LEGAL_RISK_CAUTION_THRESHOLD);
                return Color.Lerp(ColorSafe, ColorCaution, t);
            }

            // 51-100: danger -> red (lerp deeper red as it climbs)
            {
                float t = (score - Constants.LEGAL_RISK_DANGER_THRESHOLD)
                        / (float)(Constants.LEGAL_RISK_MAX - Constants.LEGAL_RISK_DANGER_THRESHOLD);
                return Color.Lerp(ColorCaution, ColorDanger, t);
            }
        }
    }
}
