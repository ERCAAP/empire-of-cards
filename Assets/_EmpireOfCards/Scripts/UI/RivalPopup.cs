using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Rival action summary popup. Fade in, display action and taunt, fade out.
    /// All animation driven by Update() polling -- no coroutines.
    ///
    /// Supports three display modes:
    /// 1. Action + Taunt: standard rival turn summary
    /// 2. Mood Icon: brief mood "tell" shown before rival acts
    /// 3. Strategy Comment: one-time reaction to player's strategy
    /// </summary>
    public class RivalPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private TMP_Text tauntText;
        [SerializeField] private TMP_Text moodIconText;
        [SerializeField] private Image rivalPortrait;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Timing")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float displayDuration = 2.5f;
        [SerializeField] private float fadeOutDuration = 0.4f;

        [Header("Mood Timing")]
        [SerializeField] private float moodDisplayDuration = 1.0f;

        // State machine
        private enum State { Idle, FadeIn, Hold, FadeOut }

        private State state = State.Idle;
        private float stateTimer;
        private float activeDisplayDuration;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            if (moodIconText != null)
                moodIconText.gameObject.SetActive(false);

            gameObject.SetActive(false);
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
        /// Assigns the mood icon text reference at runtime.
        /// Called by HUDBuilder after creating the mood icon element.
        /// </summary>
        public void SetMoodIconReference(TMP_Text moodIcon)
        {
            moodIconText = moodIcon;
        }

        /// <summary>
        /// Displays the rival popup with an action description and optional taunt.
        /// </summary>
        public void Show(string action, string taunt)
        {
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            // Hide mood icon when showing action/taunt
            if (moodIconText != null)
                moodIconText.gameObject.SetActive(false);

            if (actionText != null)
            {
                actionText.text = action;
                actionText.gameObject.SetActive(!string.IsNullOrEmpty(action));
            }

            if (tauntText != null)
            {
                tauntText.text = taunt;
                tauntText.gameObject.SetActive(!string.IsNullOrEmpty(taunt));
            }

            activeDisplayDuration = displayDuration;
            state = State.FadeIn;
            stateTimer = 0f;
        }

        /// <summary>
        /// Displays a brief mood icon "tell" before the rival acts.
        /// Shows for moodDisplayDuration (1 second) then fades out.
        /// </summary>
        public void ShowMoodIcon(string moodIcon)
        {
            if (string.IsNullOrEmpty(moodIcon)) return;
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            // Hide action/taunt texts during mood display
            if (actionText != null)
                actionText.gameObject.SetActive(false);
            if (tauntText != null)
                tauntText.gameObject.SetActive(false);

            if (moodIconText != null)
            {
                moodIconText.text = moodIcon;
                moodIconText.gameObject.SetActive(true);
            }

            activeDisplayDuration = moodDisplayDuration;
            state = State.FadeIn;
            stateTimer = 0f;
        }

        /// <summary>
        /// Displays a strategy reaction comment from the rival.
        /// Uses the taunt text slot with standard timing.
        /// </summary>
        public void ShowStrategyComment(string comment)
        {
            if (string.IsNullOrEmpty(comment)) return;
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            if (moodIconText != null)
                moodIconText.gameObject.SetActive(false);

            if (actionText != null)
                actionText.gameObject.SetActive(false);

            if (tauntText != null)
            {
                tauntText.text = comment;
                tauntText.gameObject.SetActive(true);
            }

            activeDisplayDuration = displayDuration;
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
            if (stateTimer >= activeDisplayDuration)
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
                {
                    canvasGroup.alpha = 0f;
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;
                }

                // Clean up mood icon after fade
                if (moodIconText != null)
                    moodIconText.gameObject.SetActive(false);

                state = State.Idle;
                gameObject.SetActive(false);
            }
        }
    }
}
