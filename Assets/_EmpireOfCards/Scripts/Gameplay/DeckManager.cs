using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Manages the player's deck, hand, draw pile, and discard pile.
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        // --- Runtime State ---
        [Header("Deck State (Read Only)")]
        [SerializeField] private List<CardData> drawPile = new List<CardData>();
        [SerializeField] private List<CardData> hand = new List<CardData>();
        [SerializeField] private List<CardData> discardPile = new List<CardData>();

        [Header("Redraw")]
        [SerializeField] private int redrawsRemaining;
        [SerializeField] private int redrawsPerTurn = 1;

        // --- Events ---
        public event Action<CardData> OnCardDrawn;
        public event Action<CardData> OnCardDiscarded;
        public event Action OnDeckShuffled;

        // --- Properties ---
        public IReadOnlyList<CardData> Hand => hand;
        public int RedrawsRemaining => redrawsRemaining;

        /// <summary>
        /// Creates the starting deck from a preset. Call once at the start of a run.
        /// </summary>
        public void InitializeDeck(DeckPresetData preset)
        {
            drawPile.Clear();
            hand.Clear();
            discardPile.Clear();

            if (preset == null)
            {
                Debug.LogError("[DeckManager] DeckPresetData is null. Cannot initialize deck.");
                return;
            }

            foreach (var card in preset.startingCards)
            {
                drawPile.Add(card);
            }

            ShuffleDeck();
            redrawsRemaining = redrawsPerTurn;
        }

        /// <summary>
        /// Fisher-Yates shuffle of the draw pile.
        /// </summary>
        public void ShuffleDeck()
        {
            for (int i = drawPile.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (drawPile[i], drawPile[j]) = (drawPile[j], drawPile[i]);
            }

            OnDeckShuffled?.Invoke();
        }

        /// <summary>
        /// Draws the specified number of cards from the draw pile into the hand.
        /// If the draw pile is empty, shuffles the discard pile back in first.
        /// </summary>
        public void DrawCards(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (drawPile.Count == 0)
                {
                    if (discardPile.Count == 0)
                    {
                        Debug.Log("[DeckManager] No cards left to draw.");
                        return;
                    }

                    // Recycle discard pile into draw pile
                    drawPile.AddRange(discardPile);
                    discardPile.Clear();
                    ShuffleDeck();
                }

                CardData drawn = drawPile[0];
                drawPile.RemoveAt(0);
                hand.Add(drawn);
                OnCardDrawn?.Invoke(drawn);
            }
        }

        /// <summary>
        /// Moves a card from the hand to the discard pile.
        /// </summary>
        public void DiscardCard(CardData card)
        {
            if (card == null || !hand.Contains(card))
                return;

            hand.Remove(card);
            discardPile.Add(card);
            OnCardDiscarded?.Invoke(card);
        }

        /// <summary>
        /// Discards a card from hand and draws a replacement. Limited to redrawsPerTurn per turn.
        /// Returns true if the redraw succeeded.
        /// </summary>
        public bool RedrawCard(CardData card)
        {
            if (redrawsRemaining <= 0)
            {
                Debug.Log("[DeckManager] No redraws remaining this turn.");
                return false;
            }

            if (card == null || !hand.Contains(card))
                return false;

            DiscardCard(card);
            DrawCards(1);
            redrawsRemaining--;
            return true;
        }

        /// <summary>
        /// Adds a new card to the deck (goes to the discard pile so it is available next shuffle).
        /// Used when purchasing from the shop.
        /// </summary>
        public void AddCardToDeck(CardData card)
        {
            if (card == null)
                return;

            discardPile.Add(card);
        }

        /// <summary>
        /// Permanently removes a card from the game. Checks hand, draw pile, and discard pile.
        /// </summary>
        public void BurnCard(CardData card)
        {
            if (card == null)
                return;

            if (hand.Contains(card))
            {
                hand.Remove(card);
                return;
            }

            if (drawPile.Contains(card))
            {
                drawPile.Remove(card);
                return;
            }

            if (discardPile.Contains(card))
            {
                discardPile.Remove(card);
            }
        }

        /// <summary>
        /// Returns the number of cards remaining in the draw pile.
        /// </summary>
        public int GetDeckCount()
        {
            return drawPile.Count;
        }

        /// <summary>
        /// Returns the number of cards in the discard pile.
        /// </summary>
        public int GetDiscardCount()
        {
            return discardPile.Count;
        }

        /// <summary>
        /// Resets the redraw counter. Call at the start of each turn.
        /// </summary>
        public void ResetRedraws()
        {
            redrawsRemaining = redrawsPerTurn;
        }

        /// <summary>
        /// Discards the entire hand at end of turn.
        /// </summary>
        public void DiscardHand()
        {
            while (hand.Count > 0)
            {
                DiscardCard(hand[0]);
            }
        }
    }
}
