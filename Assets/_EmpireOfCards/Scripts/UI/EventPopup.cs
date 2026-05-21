using TMPro;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Clarity;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Lightweight top-banner event notification.
    /// Intentionally non-blocking: no modal card reveal, no center-screen panel.
    /// </summary>
    public class EventPopup : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text detailText;
        [SerializeField] private TMP_Text contextText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Timing")]
        [SerializeField] private float fadeInDuration = 0.18f;
        [SerializeField] private float holdDuration = 2.8f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private enum State { Idle, FadeIn, Hold, FadeOut }

        private State state = State.Idle;
        private float stateTimer;

        private void Awake()
        {
            HideImmediate();
        }

        private void Update()
        {
            if (state == State.Idle)
                return;

            stateTimer += Time.deltaTime;

            switch (state)
            {
                case State.FadeIn: UpdateFadeIn(); break;
                case State.Hold:      UpdateHold();      break;
                case State.FadeOut:   UpdateFadeOut();   break;
            }
        }

        public void SetReferences(CanvasGroup cg, TMP_Text title, TMP_Text detail, TMP_Text context)
        {
            canvasGroup = cg;
            titleText = title;
            detailText = detail;
            contextText = context;
        }

        public void Show(CardData eventCard)
        {
            if (eventCard == null)
            {
                HideImmediate();
                return;
            }

            string title = string.IsNullOrWhiteSpace(eventCard.cardName) ? "EVENT" : eventCard.cardName.ToUpperInvariant();
            string detail = !string.IsNullOrWhiteSpace(eventCard.description)
                ? eventCard.description
                : GameClarityFormatter.BuildCardFrontSummary(eventCard);
            string context = BuildContextLine(eventCard);

            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(detail))
            {
                HideImmediate();
                return;
            }

            gameObject.SetActive(true);

            if (titleText != null)
                titleText.text = title;
            if (detailText != null)
                detailText.text = detail;
            if (contextText != null)
            {
                contextText.text = context;
                contextText.gameObject.SetActive(!string.IsNullOrWhiteSpace(context));
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }

            state = State.FadeIn;
            stateTimer = 0f;
        }

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
                HideImmediate();
        }

        private void HideImmediate()
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

        private static string BuildContextLine(CardData eventCard)
        {
            if (eventCard == null)
                return string.Empty;

            string slot = eventCard.targetSlotType switch
            {
                SlotType.Operation => "OPERATIONS HIT",
                SlotType.Staff => "STAFF HIT",
                SlotType.Marketing => "GROWTH HIT",
                SlotType.Supplier => "SUPPLY HIT",
                SlotType.TempEffect => "BOARD PRESSURE",
                _ => "BOARD PRESSURE"
            };

            string duration = eventCard.eventDuration > 0 ? $" · {eventCard.eventDuration} TURN" : string.Empty;
            return $"{slot}{duration}";
        }
    }
}
