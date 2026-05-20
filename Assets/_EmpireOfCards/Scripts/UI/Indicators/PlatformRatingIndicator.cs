using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;

namespace EmpireOfCards.UI.Indicators
{
    /// <summary>
    /// Displays platform rating (1.0-5.0) as a color-coded fill bar with numeric text.
    /// Subscribes to EventBus.OnPlatformRatingChanged.
    /// </summary>
    public class PlatformRatingIndicator : MonoBehaviour
    {
        private Image _barFill;
        private TMP_Text _valueText;

        // Bar dimensions — full width of the background track in pixels
        private const float BAR_FULL_WIDTH = 140f;

        private static readonly Color ColorLow  = new Color(0.9f, 0.35f, 0.2f);   // < 2.5: red-orange
        private static readonly Color ColorMid  = new Color(1f,   0.85f, 0.15f);  // 2.5-3.5: yellow
        private static readonly Color ColorHigh = new Color(0.3f, 0.75f, 1f);     // > 3.5: blue

        public void Init(Image barFill, TMP_Text valueText)
        {
            _barFill    = barFill;
            _valueText  = valueText;
            Refresh(Constants.PLATFORM_RATING_DEFAULT);
        }

        private void OnEnable()  => EventBus.OnPlatformRatingChanged += OnRatingChanged;
        private void OnDisable() => EventBus.OnPlatformRatingChanged -= OnRatingChanged;

        private void OnRatingChanged(float rating) => Refresh(rating);

        private void Refresh(float rating)
        {
            float clamped = Mathf.Clamp(rating,
                Constants.PLATFORM_RATING_MIN,
                Constants.PLATFORM_RATING_MAX);

            float t = (clamped - Constants.PLATFORM_RATING_MIN)
                    / (Constants.PLATFORM_RATING_MAX - Constants.PLATFORM_RATING_MIN);

            if (_barFill != null)
            {
                _barFill.rectTransform.sizeDelta =
                    new Vector2(BAR_FULL_WIDTH * t, _barFill.rectTransform.sizeDelta.y);
                _barFill.color = GetColor(t);
            }

            if (_valueText != null)
            {
                _valueText.text  = clamped.ToString("F1");
                _valueText.color = GetColor(t);
            }
        }

        private static Color GetColor(float t)
        {
            if (t < 0.3f)   return ColorLow;
            if (t < 0.6f)   return Color.Lerp(ColorLow, ColorMid, (t - 0.3f) / 0.3f);
            if (t < 0.8f)   return Color.Lerp(ColorMid, ColorHigh, (t - 0.6f) / 0.2f);
            return ColorHigh;
        }
    }
}
