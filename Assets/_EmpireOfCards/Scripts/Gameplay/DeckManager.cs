using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Manages the player's deck, hand, draw pile, and discard pile.
    /// Initializes from DeckPresetData (expanding DeckEntry counts into individual cards).
    /// Supports draw, discard, redraw (1/turn), burn, and deck recycling.
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        // --- Runtime State ---
        [Header("Deck State (Read Only)")]
        [SerializeField] private List<CardData> drawPile = new List<CardData>();
        [SerializeField] private List<CardData> hand = new List<CardData>();
        [SerializeField] private List<CardData> discardPile = new List<CardData>();

        [Header("Settings")]
        [SerializeField] private int handSize = 5;
        [SerializeField] private int redrawsPerTurn = 1;

        // --- Redraw tracking ---
        [Header("Redraw State (Read Only)")]
        [SerializeField] private int redrawsRemaining;

        // --- Properties ---
        public IReadOnlyList<CardData> DrawPile => drawPile;
        public IReadOnlyList<CardData> Hand => hand;
        public IReadOnlyList<CardData> DiscardPile => discardPile;
        public int RedrawsRemaining => redrawsRemaining;
        public int HandSize => handSize;

        // ----------------------------------------------------------------
        // Initialization
        // ----------------------------------------------------------------

        /// <summary>
        /// Creates the starting deck from a preset. Expands DeckEntry counts
        /// into individual CardData instances. Call once at the start of a run.
        /// DeckPresetData has DeckEntry[] cards, each with a card + count.
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

            // Expand DeckEntry counts into individual cards
            foreach (DeckEntry entry in preset.cards)
            {
                if (entry.card == null) continue;

                for (int i = 0; i < entry.count; i++)
                {
                    drawPile.Add(entry.card);
                }
            }

            Debug.Log($"[DeckManager] Deck initialized with {drawPile.Count} cards from preset '{preset.presetName}'.");

            ShuffleDeck();
            redrawsRemaining = redrawsPerTurn;
        }

        // ----------------------------------------------------------------
        // Shuffle
        // ----------------------------------------------------------------

        /// <summary>
        /// Fisher-Yates shuffle of the draw pile.
        /// </summary>
        public void ShuffleDeck()
        {
            for (int i = drawPile.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                CardData temp = drawPile[i];
                drawPile[i] = drawPile[j];
                drawPile[j] = temp;
            }
        }

        // ----------------------------------------------------------------
        // Draw
        // ----------------------------------------------------------------

        /// <summary>
        /// Draws the specified number of cards from the draw pile into the hand.
        /// If the draw pile is empty, recycled the discard pile back in (shuffled).
        /// </summary>
        public List<CardData> DrawCards(int count)
        {
            List<CardData> drawnCards = new List<CardData>();

            for (int i = 0; i < count; i++)
            {
                if (drawPile.Count == 0)
                {
                    RecycleDiscardPile();

                    if (drawPile.Count == 0)
                    {
                        Debug.Log("[DeckManager] No cards left to draw.");
                        break;
                    }
                }

                CardData drawn = drawPile[0];
                drawPile.RemoveAt(0);
                hand.Add(drawn);
                drawnCards.Add(drawn);
                EventBus.CardDrawn(drawn);
            }

            return drawnCards;
        }

        /// <summary>
        /// Draws cards up to handSize. Used at the start of draw phase.
        /// </summary>
        public List<CardData> DrawToHandSize()
        {
            int cardsToDraw = Mathf.Max(0, handSize - hand.Count);
            return DrawCards(cardsToDraw);
        }

        // ----------------------------------------------------------------
        // Discard
        // ----------------------------------------------------------------

        /// <summary>
        /// Moves a card from the hand to the discard pile.
        /// </summary>
        public bool DiscardCard(CardData card)
        {
            if (card == null || !hand.Contains(card))
                return false;

            hand.Remove(card);
            discardPile.Add(card);
            EventBus.CardDiscarded(card);
            return true;
        }

        /// <summary>
        /// Discards the entire hand at end of turn.
        /// </summary>
        public void DiscardHand()
        {
            while (hand.Count > 0)
            {
                CardData card = hand[0];
                hand.RemoveAt(0);
                discardPile.Add(card);
                EventBus.CardDiscarded(card);
            }
        }

        // ----------------------------------------------------------------
        // Redraw (GDD: 1/turn limit)
        // ----------------------------------------------------------------

        /// <summary>
        /// Discards a card from hand and draws a replacement.
        /// Limited to redrawsPerTurn per turn (default 1).
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
        /// Resets the redraw counter. Call at the start of each turn.
        /// </summary>
        public void ResetRedraws()
        {
            redrawsRemaining = redrawsPerTurn;
        }

        // ----------------------------------------------------------------
        // Add / Remove Cards
        // ----------------------------------------------------------------

        /// <summary>
        /// Adds a new card to the deck (goes to the discard pile so it is
        /// available next shuffle). Used when purchasing from the shop.
        /// </summary>
        public void AddCardToDeck(CardData card)
        {
            if (card == null) return;
            discardPile.Add(card);
        }

        /// <summary>
        /// Permanently removes a card from the game.
        /// Searches hand, draw pile, and discard pile.
        /// </summary>
        public void BurnCard(CardData card)
        {
            if (card == null) return;

            // Search hand first
            if (hand.Remove(card))
            {
                EventBus.CardBurned(card);
                return;
            }

            // Then draw pile
            if (drawPile.Remove(card))
            {
                EventBus.CardBurned(card);
                return;
            }

            // Then discard pile
            if (discardPile.Remove(card))
            {
                EventBus.CardBurned(card);
            }
        }

        // ----------------------------------------------------------------
        // Special: Draw Random Employee (Acil Ise Alim action card)
        // ----------------------------------------------------------------

        /// <summary>
        /// Searches draw pile and discard pile for a random Employee card,
        /// removes it from its pile, and returns it.
        /// Used by the "Acil Ise Alim" action card.
        /// </summary>
        public CardData DrawRandomEmployee()
        {
            // Collect all employee cards from draw and discard piles
            List<(CardData card, bool inDrawPile, int index)> candidates = new List<(CardData, bool, int)>();

            for (int i = 0; i < drawPile.Count; i++)
            {
                if (drawPile[i] != null && drawPile[i].cardType == CardType.Employee)
                    candidates.Add((drawPile[i], true, i));
            }

            for (int i = 0; i < discardPile.Count; i++)
            {
                if (discardPile[i] != null && discardPile[i].cardType == CardType.Employee)
                    candidates.Add((discardPile[i], false, i));
            }

            if (candidates.Count == 0)
            {
                Debug.Log("[DeckManager] No employee cards available to draw.");
                return null;
            }

            // Pick random
            int pick = UnityEngine.Random.Range(0, candidates.Count);
            var chosen = candidates[pick];

            // Remove from its pile
            if (chosen.inDrawPile)
                drawPile.RemoveAt(chosen.index);
            else
                discardPile.RemoveAt(chosen.index);

            return chosen.card;
        }

        // ----------------------------------------------------------------
        // Queries
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the number of cards remaining in the draw pile.
        /// </summary>
        public int GetDrawPileCount()
        {
            return drawPile.Count;
        }

        /// <summary>
        /// Returns the number of cards in the discard pile.
        /// </summary>
        public int GetDiscardPileCount()
        {
            return discardPile.Count;
        }

        /// <summary>
        /// Returns the total number of cards across all piles (draw + hand + discard).
        /// </summary>
        public int GetTotalCardCount()
        {
            return drawPile.Count + hand.Count + discardPile.Count;
        }

        // ----------------------------------------------------------------
        // First Turn Draw (Onboarding - constrained hand)
        // ----------------------------------------------------------------

        /// <summary>
        /// Draws a constrained first-turn hand to guide the player:
        /// 1 Business card + 1 Employee card + fill remaining to handSize.
        /// Searches the draw pile for the required types and pulls them to front.
        /// Falls back to normal draw if the types are not available.
        /// </summary>
        public List<CardData> DrawFirstTurnHand()
        {
            List<CardData> drawnCards = new List<CardData>();

            // Pull one Business card from draw pile to front
            PullCardTypeToFront(CardType.Business);
            // Pull one Employee card right after
            PullCardTypeToFront(CardType.Employee, 1);

            // Now draw normally -- the first 2 cards are guaranteed types
            int toDraw = Mathf.Max(0, handSize - hand.Count);
            for (int i = 0; i < toDraw; i++)
            {
                if (drawPile.Count == 0)
                {
                    RecycleDiscardPile();
                    if (drawPile.Count == 0)
                    {
                        Debug.Log("[DeckManager] No cards left to draw (first turn).");
                        break;
                    }
                }

                CardData drawn = drawPile[0];
                drawPile.RemoveAt(0);
                hand.Add(drawn);
                drawnCards.Add(drawn);
                EventBus.CardDrawn(drawn);
            }

            Debug.Log($"[DeckManager] First turn hand drawn: {drawnCards.Count} cards.");
            return drawnCards;
        }

        /// <summary>
        /// Finds the first card of the given type in the draw pile at or after
        /// startIndex, and moves it to that position.
        /// </summary>
        private void PullCardTypeToFront(CardType type, int insertAt = 0)
        {
            for (int i = insertAt; i < drawPile.Count; i++)
            {
                if (drawPile[i] != null && drawPile[i].cardType == type)
                {
                    if (i == insertAt) return; // Already in position
                    CardData card = drawPile[i];
                    drawPile.RemoveAt(i);
                    drawPile.Insert(insertAt, card);
                    return;
                }
            }
            // Type not found in draw pile -- normal draw will handle it
        }

        // ----------------------------------------------------------------
        // Recycle
        // ----------------------------------------------------------------

        /// <summary>
        /// Moves all discard pile cards into the draw pile and shuffles.
        /// </summary>
        private void RecycleDiscardPile()
        {
            if (discardPile.Count == 0) return;

            drawPile.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
            Debug.Log("[DeckManager] Discard pile recycled into draw pile and shuffled.");
        }

        /// <summary>
        /// Resets the deck manager for a new run.
        /// </summary>
        public void Reset()
        {
            drawPile.Clear();
            hand.Clear();
            discardPile.Clear();
            redrawsRemaining = 0;
        }
    }
}
