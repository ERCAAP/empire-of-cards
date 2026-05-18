using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Displays the player's hand of cards in a fan layout.
    /// Subscribes to EventBus card events -- never reads DeckManager directly.
    /// Creates / destroys CardUI instances from a prefab.
    /// </summary>
    public class HandUI : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private CardUI cardPrefab;

        [Header("Fan Layout")]
        [SerializeField] private Transform handRoot;
        [SerializeField] private float cardSpacing = 160f;
        [SerializeField] private float fanAngle = 5f;       // degrees per card offset from center
        [SerializeField] private float verticalArc = 20f;   // parabolic lift at center

        // Runtime
        private readonly List<CardUI> cardInstances = new List<CardUI>();
        private bool interactable;

        /// <summary>
        /// Current card visuals in hand.
        /// </summary>
        public IReadOnlyList<CardUI> CurrentCards => cardInstances;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            EventBus.OnCardDrawn += OnCardDrawn;
            EventBus.OnCardDiscarded += OnCardDiscarded;
            EventBus.OnCardBurned += OnCardBurned;
            EventBus.OnCardPlayed += OnCardPlayed;
            EventBus.OnPhaseStarted += OnPhaseStarted;
        }

        private void OnDisable()
        {
            EventBus.OnCardDrawn -= OnCardDrawn;
            EventBus.OnCardDiscarded -= OnCardDiscarded;
            EventBus.OnCardBurned -= OnCardBurned;
            EventBus.OnCardPlayed -= OnCardPlayed;
            EventBus.OnPhaseStarted -= OnPhaseStarted;
        }

        // ------------------------------------------------------------------
        // EventBus handlers
        // ------------------------------------------------------------------

        private void OnCardDrawn(CardData card)
        {
            AddCard(card);
        }

        private void OnCardDiscarded(CardData card)
        {
            RemoveCard(card);
        }

        private void OnCardBurned(CardData card)
        {
            RemoveCard(card);
        }

        private void OnCardPlayed(CardData card)
        {
            RemoveCard(card);
        }

        private void OnPhaseStarted(TurnPhase phase)
        {
            // Cards are interactable only during the PlayPhase
            SetInteractable(phase == TurnPhase.PlayPhase);

            // At the start of the EventPhase (new turn) clear visuals;
            // the DrawPhase will repopulate via OnCardDrawn events.
            if (phase == TurnPhase.EventPhase)
            {
                ClearHand();
            }
        }

        // ------------------------------------------------------------------
        // Public
        // ------------------------------------------------------------------

        /// <summary>
        /// Enable or disable card interaction (dragging, hover).
        /// </summary>
        public void SetInteractable(bool value)
        {
            interactable = value;

            foreach (var card in cardInstances)
            {
                if (card != null)
                    card.SetInteractable(value);
            }
        }

        /// <summary>
        /// Removes a specific CardUI that has been played / discarded.
        /// Called by CardDragHandler after a successful drop.
        /// </summary>
        public void RemoveCardUI(CardUI cardUI)
        {
            if (cardUI == null)
                return;

            cardInstances.Remove(cardUI);
            Destroy(cardUI.gameObject);
            LayoutHand();
        }

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        private void AddCard(CardData card)
        {
            if (cardPrefab == null || handRoot == null)
                return;

            CardUI instance = Instantiate(cardPrefab, handRoot);
            instance.SetupCard(card);
            instance.SetInteractable(interactable);

            cardInstances.Add(instance);
            LayoutHand();
        }

        private void RemoveCard(CardData card)
        {
            for (int i = cardInstances.Count - 1; i >= 0; i--)
            {
                if (cardInstances[i] != null && cardInstances[i].Data == card)
                {
                    Destroy(cardInstances[i].gameObject);
                    cardInstances.RemoveAt(i);
                    break;
                }
            }

            LayoutHand();
        }

        private void ClearHand()
        {
            foreach (var card in cardInstances)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }

            cardInstances.Clear();
        }

        /// <summary>
        /// Positions cards in a fan arc centered on handRoot.
        /// </summary>
        private void LayoutHand()
        {
            int count = cardInstances.Count;
            if (count == 0)
                return;

            float totalWidth = (count - 1) * cardSpacing;
            float startX = -totalWidth * 0.5f;

            for (int i = 0; i < count; i++)
            {
                if (cardInstances[i] == null)
                    continue;

                RectTransform rt = cardInstances[i].GetComponent<RectTransform>();
                if (rt == null)
                    continue;

                float t = count > 1 ? (float)i / (count - 1) : 0.5f;

                // Horizontal position
                float x = startX + i * cardSpacing;

                // Parabolic vertical arc (higher at center)
                float normalizedOffset = (t - 0.5f) * 2f; // -1..1
                float y = verticalArc * (1f - normalizedOffset * normalizedOffset);

                // Fan rotation
                float angle = Mathf.Lerp(
                    fanAngle * (count - 1) * 0.5f,
                    -fanAngle * (count - 1) * 0.5f,
                    t);

                rt.anchoredPosition = new Vector2(x, y);
                rt.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
    }
}
