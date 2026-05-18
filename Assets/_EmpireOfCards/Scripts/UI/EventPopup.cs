using System.Collections;
using UnityEngine;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Displays an event card with a flip-and-glow animation.
    /// </summary>
    public class EventPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CardUI eventCardUI;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Timing")]
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float flipDuration = 0.4f;
        [SerializeField] private float glowPulseDuration = 0.6f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        [Header("Glow")]
        [SerializeField] private GameObject glowEffect;

        private Coroutine activeCoroutine;
        private RectTransform cardRect;

        private void Awake()
        {
            if (eventCardUI != null)
            {
                cardRect = eventCardUI.GetComponent<RectTransform>();
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }

            if (glowEffect != null)
            {
                glowEffect.SetActive(false);
            }
        }

        /// <summary>
        /// Shows the event card with a flip animation followed by a glow pulse.
        /// </summary>
        public void Show(CardData eventCard)
        {
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }

            if (eventCardUI != null)
            {
                eventCardUI.SetupCard(eventCard);
            }

            activeCoroutine = StartCoroutine(AnimateEventCoroutine());
        }

        private IEnumerator AnimateEventCoroutine()
        {
            // Fade in
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            // Card flip animation (scale X from 0 to 1)
            if (cardRect != null)
            {
                float elapsed = 0f;
                Vector3 scale = cardRect.localScale;

                // First half: scale X to 0 (card back)
                while (elapsed < flipDuration * 0.5f)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / (flipDuration * 0.5f);
                    cardRect.localScale = new Vector3(Mathf.Lerp(1f, 0f, t), scale.y, scale.z);
                    yield return null;
                }

                // Second half: scale X from 0 to 1 (card front revealed)
                elapsed = 0f;
                while (elapsed < flipDuration * 0.5f)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / (flipDuration * 0.5f);
                    cardRect.localScale = new Vector3(Mathf.Lerp(0f, 1f, t), scale.y, scale.z);
                    yield return null;
                }

                cardRect.localScale = scale;
            }

            // Glow pulse
            if (glowEffect != null)
            {
                glowEffect.SetActive(true);

                float elapsed = 0f;
                while (elapsed < glowPulseDuration)
                {
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                glowEffect.SetActive(false);
            }

            // Hold on screen
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            float fadeElapsed = 0f;
            while (fadeElapsed < fadeOutDuration)
            {
                fadeElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(fadeElapsed / fadeOutDuration);
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f - t;
                yield return null;
            }

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;

            activeCoroutine = null;
        }
    }
}
