using System.Collections;
using UnityEngine;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Animated popup that appears when a combo is triggered.
    /// Text flies upward with a scale punch, then fades out.
    /// </summary>
    public class ComboPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Timing")]
        [SerializeField] private float displayDuration = 2f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        [Header("Animation")]
        [SerializeField] private float riseDistance = 80f;
        [SerializeField] private float punchScale = 1.4f;
        [SerializeField] private float punchDuration = 0.2f;

        private Coroutine activeCoroutine;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// Displays the combo text with a fly-up and scale animation, then fades out.
        /// </summary>
        public void Show(string text, Color color)
        {
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }

            if (comboText != null)
            {
                comboText.text = text;
                comboText.color = color;
            }

            activeCoroutine = StartCoroutine(AnimateComboCoroutine());
        }

        private IEnumerator AnimateComboCoroutine()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            Vector3 startPos = rectTransform.localPosition;
            Vector3 originalScale = Vector3.one;
            float elapsed;

            // Punch scale in
            elapsed = 0f;
            while (elapsed < punchDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / punchDuration;
                float scale = Mathf.Lerp(punchScale, 1f, t * t);
                rectTransform.localScale = originalScale * scale;
                yield return null;
            }
            rectTransform.localScale = originalScale;

            // Hold and rise
            elapsed = 0f;
            while (elapsed < displayDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / displayDuration;
                rectTransform.localPosition = startPos + Vector3.up * (riseDistance * t);
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutDuration);
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f - t;
                yield return null;
            }

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;

            // Reset position
            rectTransform.localPosition = startPos;
            activeCoroutine = null;
        }
    }
}
