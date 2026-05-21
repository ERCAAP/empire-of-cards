using UnityEngine;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Event card reveal popup. Card flip animation, glow, hold, then fade.
    /// All animation driven by Update() polling -- no coroutines.
    /// </summary>
    public class EventPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CardUI eventCardUI;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject glowEffect;

        [Header("Timing")]
        [SerializeField] private float flipDuration = 0.4f;
        [SerializeField] private float glowPulseDuration = 0.6f;
        [SerializeField] private float holdDuration = 3f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        // State machine
        private enum State { Idle, FlipClose, FlipOpen, GlowPulse, Hold, FadeOut }

        private State state = State.Idle;
        private float stateTimer;
        private RectTransform cardRect;
        private Vector3 cardOriginalScale;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void Awake()
        {
            if (eventCardUI != null)
                cardRect = eventCardUI.GetComponent<RectTransform>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            if (glowEffect != null)
                glowEffect.SetActive(false);

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (state == State.Idle)
                return;

            stateTimer += Time.deltaTime;

            switch (state)
            {
                case State.FlipClose: UpdateFlipClose(); break;
                case State.FlipOpen:  UpdateFlipOpen();  break;
                case State.GlowPulse: UpdateGlowPulse(); break;
                case State.Hold:      UpdateHold();      break;
                case State.FadeOut:   UpdateFadeOut();   break;
            }
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Assigns UI element references at runtime.
        /// Called by HUDBuilder after creating child elements.
        /// </summary>
        public void SetReferences(CanvasGroup cg)
        {
            canvasGroup = cg;
        }

        /// <summary>
        /// Shows the event card with a flip animation followed by a glow pulse.
        /// </summary>
        public void Show(CardData eventCard)
        {
            gameObject.SetActive(true);

            if (eventCardUI != null)
                eventCardUI.SetupCard(eventCard);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            if (cardRect != null)
                cardOriginalScale = cardRect.localScale;

            if (glowEffect != null)
                glowEffect.SetActive(false);

            state = State.FlipClose;
            stateTimer = 0f;
        }

        // ------------------------------------------------------------------
        // State updates
        // ------------------------------------------------------------------

        private void UpdateFlipClose()
        {
            float halfFlip = flipDuration * 0.5f;
            float t = Mathf.Clamp01(stateTimer / halfFlip);

            if (cardRect != null)
            {
                cardRect.localScale = new Vector3(
                    Mathf.Lerp(cardOriginalScale.x, 0f, t),
                    cardOriginalScale.y,
                    cardOriginalScale.z);
            }

            if (t >= 1f)
            {
                state = State.FlipOpen;
                stateTimer = 0f;
            }
        }

        private void UpdateFlipOpen()
        {
            float halfFlip = flipDuration * 0.5f;
            float t = Mathf.Clamp01(stateTimer / halfFlip);

            if (cardRect != null)
            {
                cardRect.localScale = new Vector3(
                    Mathf.Lerp(0f, cardOriginalScale.x, t),
                    cardOriginalScale.y,
                    cardOriginalScale.z);
            }

            if (t >= 1f)
            {
                if (cardRect != null)
                    cardRect.localScale = cardOriginalScale;

                if (glowEffect != null)
                    glowEffect.SetActive(true);

                state = State.GlowPulse;
                stateTimer = 0f;
            }
        }

        private void UpdateGlowPulse()
        {
            if (stateTimer >= glowPulseDuration)
            {
                if (glowEffect != null)
                    glowEffect.SetActive(false);

                state = State.Hold;
                stateTimer = 0f;
            }
        }

        private void UpdateHold()
        {
            if (stateTimer >= holdDuration)
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

                state = State.Idle;
                gameObject.SetActive(false);
            }
        }
    }
}
