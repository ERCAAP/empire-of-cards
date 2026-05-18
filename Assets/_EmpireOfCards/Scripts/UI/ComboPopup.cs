using UnityEngine;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Animated popup that appears when a combo is triggered.
    /// Text flies upward with a scale punch, then fades out.
    /// All animation driven by Update() polling -- no coroutines.
    /// </summary>
    public class ComboPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text comboText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Timing")]
        [SerializeField] private float punchDuration = 0.2f;
        [SerializeField] private float displayDuration = 2f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        [Header("Animation")]
        [SerializeField] private float riseDistance = 80f;
        [SerializeField] private float punchScale = 1.4f;

        // Animation state machine
        private enum State { Idle, Punch, Rise, FadeOut }

        private State state = State.Idle;
        private float stateTimer;
        private RectTransform rectTransform;
        private Vector3 startPosition;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }

        private void Update()
        {
            if (state == State.Idle)
                return;

            stateTimer += Time.deltaTime;

            switch (state)
            {
                case State.Punch:
                    UpdatePunch();
                    break;
                case State.Rise:
                    UpdateRise();
                    break;
                case State.FadeOut:
                    UpdateFadeOut();
                    break;
            }
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Displays the combo text with fly-up, scale punch, and fade.
        /// </summary>
        public void Show(string text, Color color)
        {
            if (comboText != null)
            {
                comboText.text = text;
                comboText.color = color;
            }

            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            startPosition = rectTransform.localPosition;
            rectTransform.localScale = Vector3.one * punchScale;

            state = State.Punch;
            stateTimer = 0f;
        }

        // ------------------------------------------------------------------
        // State updates
        // ------------------------------------------------------------------

        private void UpdatePunch()
        {
            float t = Mathf.Clamp01(stateTimer / punchDuration);
            float scale = Mathf.Lerp(punchScale, 1f, t * t); // ease-in
            rectTransform.localScale = Vector3.one * scale;

            if (t >= 1f)
            {
                rectTransform.localScale = Vector3.one;
                state = State.Rise;
                stateTimer = 0f;
            }
        }

        private void UpdateRise()
        {
            float t = Mathf.Clamp01(stateTimer / displayDuration);
            rectTransform.localPosition = startPosition + Vector3.up * (riseDistance * t);

            if (t >= 1f)
            {
                state = State.FadeOut;
                stateTimer = 0f;
            }
        }

        private void UpdateFadeOut()
        {
            float t = Mathf.Clamp01(stateTimer / fadeOutDuration);

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - t;

            if (t >= 1f)
            {
                if (canvasGroup != null)
                    canvasGroup.alpha = 0f;

                rectTransform.localPosition = startPosition;
                state = State.Idle;
            }
        }
    }
}
