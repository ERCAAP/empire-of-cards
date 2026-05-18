using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Shows what the rival AI just did, with an optional taunt line.
    /// </summary>
    public class RivalPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private TMP_Text tauntText;
        [SerializeField] private Image rivalPortrait;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Timing")]
        [SerializeField] private float displayDuration = 2.5f;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.4f;

        private Coroutine activeCoroutine;

        private void Awake()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// Displays the rival popup with an action description, optional taunt, and portrait.
        /// </summary>
        public void Show(string action, string taunt, Sprite portrait)
        {
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }

            if (actionText != null)
                actionText.text = action;

            if (tauntText != null)
            {
                tauntText.text = taunt;
                tauntText.gameObject.SetActive(!string.IsNullOrEmpty(taunt));
            }

            if (rivalPortrait != null && portrait != null)
            {
                rivalPortrait.sprite = portrait;
                rivalPortrait.enabled = true;
            }
            else if (rivalPortrait != null)
            {
                rivalPortrait.enabled = false;
            }

            activeCoroutine = StartCoroutine(AnimatePopupCoroutine());
        }

        private IEnumerator AnimatePopupCoroutine()
        {
            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeInDuration);
                if (canvasGroup != null)
                    canvasGroup.alpha = t;
                yield return null;
            }

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            // Hold
            yield return new WaitForSeconds(displayDuration);

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

            activeCoroutine = null;
        }
    }
}
