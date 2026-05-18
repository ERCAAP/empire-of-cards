using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.UI
{
    /// <summary>
    /// Manages the visual representation of the player's hand.
    /// </summary>
    public class HandUI : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private Transform[] cardSlots;
        [SerializeField] private CardUI cardPrefab;

        [Header("Animation")]
        [SerializeField] private float drawAnimDuration = 0.3f;
        [SerializeField] private float drawStagger = 0.1f;

        private readonly List<CardUI> currentCards = new List<CardUI>();

        /// <summary>
        /// Current cards displayed in hand.
        /// </summary>
        public List<CardUI> CurrentCards => currentCards;

        /// <summary>
        /// Clears the hand and displays new cards.
        /// </summary>
        public void DisplayHand(List<CardData> cards)
        {
            ClearHand();

            for (int i = 0; i < cards.Count && i < cardSlots.Length; i++)
            {
                CardUI card = Instantiate(cardPrefab, cardSlots[i]);
                card.SetupCard(cards[i]);
                currentCards.Add(card);
            }
        }

        /// <summary>
        /// Removes all card visuals from hand slots.
        /// </summary>
        public void ClearHand()
        {
            foreach (CardUI card in currentCards)
            {
                if (card != null)
                {
                    Destroy(card.gameObject);
                }
            }

            currentCards.Clear();
        }

        /// <summary>
        /// Adds a single card to the next available hand slot.
        /// </summary>
        public void AddCard(CardData card)
        {
            if (currentCards.Count >= cardSlots.Length)
                return;

            int slotIndex = currentCards.Count;
            CardUI cardUI = Instantiate(cardPrefab, cardSlots[slotIndex]);
            cardUI.SetupCard(card);
            currentCards.Add(cardUI);
        }

        /// <summary>
        /// Removes the card at the given index from the hand.
        /// </summary>
        public void RemoveCard(int index)
        {
            if (index < 0 || index >= currentCards.Count)
                return;

            CardUI card = currentCards[index];
            currentCards.RemoveAt(index);

            if (card != null)
            {
                Destroy(card.gameObject);
            }

            // Re-parent remaining cards to fill the gap
            for (int i = 0; i < currentCards.Count && i < cardSlots.Length; i++)
            {
                currentCards[i].transform.SetParent(cardSlots[i], false);
            }
        }

        /// <summary>
        /// Plays a staggered draw animation for all cards currently in hand.
        /// </summary>
        public void AnimateDrawCards()
        {
            StartCoroutine(DrawCardsCoroutine());
        }

        private IEnumerator DrawCardsCoroutine()
        {
            foreach (CardUI card in currentCards)
            {
                if (card == null)
                    continue;

                CanvasGroup cg = card.GetComponent<CanvasGroup>();
                RectTransform rt = card.GetComponent<RectTransform>();

                if (cg != null)
                    cg.alpha = 0f;

                Vector3 targetPos = rt.localPosition;
                rt.localPosition = targetPos + Vector3.down * 200f;

                StartCoroutine(AnimateSingleDraw(rt, cg, targetPos));
                yield return new WaitForSeconds(drawStagger);
            }
        }

        private IEnumerator AnimateSingleDraw(RectTransform rt, CanvasGroup cg, Vector3 targetPos)
        {
            float elapsed = 0f;
            Vector3 startPos = rt.localPosition;

            while (elapsed < drawAnimDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / drawAnimDuration);
                // Ease-out
                float eased = 1f - Mathf.Pow(1f - t, 3f);

                rt.localPosition = Vector3.Lerp(startPos, targetPos, eased);
                if (cg != null)
                    cg.alpha = eased;

                yield return null;
            }

            rt.localPosition = targetPos;
            if (cg != null)
                cg.alpha = 1f;
        }
    }
}
