using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Rival action summary popup. Fade in, display action and taunt, fade out.
    /// All animation driven by Update() polling -- no coroutines.
    /// </summary>
    public class RivalPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private TMP_Text tauntText;
        [SerializeField] private Image rivalPortrait;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Timing")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float displayDuration = 2.5f;
        [SerializeField] private float fadeOutDuration = 0.4f;

        // State machine
        private enum State { Idle, FadeIn, Hold, FadeOut }

        private State state = State.Idle;
        private float stateTimer;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
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
                case State.FadeIn:  UpdateFadeIn();  break;
                case State.Hold:    UpdateHold();    break;
                case State.FadeOut: UpdateFadeOut(); break;
            }
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Assigns UI element references at runtime.
        /// Called by HUDBuilder after creating child elements.
        /// </summary>
        public void SetReferences(TMP_Text action, TMP_Text taunt, CanvasGroup cg)
        {
            actionText = action;
            tauntText = taunt;
            canvasGroup = cg;
        }

        /// <summary>
        /// Displays the rival popup with an action description and optional taunt.
        /// </summary>
        public void Show(string action, string taunt)
        {
            if (actionText != null)
                actionText.text = action;

            if (tauntText != null)
            {
                tauntText.text = taunt;
                tauntText.gameObject.SetActive(!string.IsNullOrEmpty(taunt));
            }

            state = State.FadeIn;
            stateTimer = 0f;
        }

        // ------------------------------------------------------------------
        // State updates
        // ------------------------------------------------------------------

        private void UpdateFadeIn()
        {
            float t = Mathf.Clamp01(stateTimer / fadeInDuration);

            if (canvasGroup != null)
                canvasGroup.alpha = t;

            if (t >= 1f)
            {
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f;

                state = State.Hold;
                stateTimer = 0f;
            }
        }

        private void UpdateHold()
        {
            if (stateTimer >= displayDuration)
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

                state = State.Idle;
            }
        }
    }
}
