using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Tracks which cards are on the board and in which slots.
    /// No business logic -- just card tracking, validation, and temp-effect timers.
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        SlotManager _slots;

        // SlotType -> list of cards currently in that slot
        readonly Dictionary<SlotType, List<CardData>> _board = new();

        // Temp-effect timers: card -> remaining turns
        readonly Dictionary<CardData, int> _tempTimers = new();

        public void Init(SlotManager slotManager)
        {
            _slots = slotManager;

            // Initialize empty lists for every slot type
            foreach (SlotType slot in System.Enum.GetValues(typeof(SlotType)))
                _board[slot] = new List<CardData>();
        }

        // ── Place / Remove ──────────────────────────────────────────

        /// <summary>
        /// Validates slot capacity, places the card, fires OnCardPlaced.
        /// Returns false if the slot is full.
        /// </summary>
        public bool PlaceCard(CardData card, SlotType slot)
        {
            if (!_slots.HasRoom(slot))
            {
                Debug.LogWarning($"[Board] No room in {slot} for {card.cardName}");
                return false;
            }

            _board[slot].Add(card);
            _slots.Occupy(slot, 1);

            // Start temp timer if card has a duration
            if (card.duration > 0)
                _tempTimers[card] = card.duration;

            EventBus.CardPlaced(card, slot);
            Debug.Log($"[Board] Placed {card.cardName} in {slot}");
            return true;
        }

        /// <summary>
        /// Finds and removes a card from the board, fires OnCardRemoved.
        /// Returns false if card was not on the board.
        /// </summary>
        public bool RemoveCard(CardData card)
        {
            foreach (var kvp in _board)
            {
                if (kvp.Value.Remove(card))
                {
                    _slots.Vacate(kvp.Key, 1);
                    _tempTimers.Remove(card);
                    EventBus.CardRemoved(card, kvp.Key);
                    Debug.Log($"[Board] Removed {card.cardName} from {kvp.Key}");
                    return true;
                }
            }

            Debug.LogWarning($"[Board] {card.cardName} not found on board");
            return false;
        }

        // ── Queries ─────────────────────────────────────────────────

        public List<CardData> GetCardsInSlot(SlotType slot)
        {
            return _board.TryGetValue(slot, out var list) ? list : new List<CardData>();
        }

        public List<CardData> GetAllActiveCards()
        {
            var all = new List<CardData>();
            foreach (var list in _board.Values)
                all.AddRange(list);
            return all;
        }

        public bool IsOnBoard(CardData card)
        {
            foreach (var list in _board.Values)
                if (list.Contains(card))
                    return true;
            return false;
        }

        // ── Resolve Tick ────────────────────────────────────────────

        /// <summary>
        /// Called each ResolvePhase. Decrements temp timers and removes expired cards.
        /// </summary>
        public void TickBoard()
        {
            var expired = new List<CardData>();

            foreach (var kvp in _tempTimers)
            {
                if (kvp.Value <= 1)
                    expired.Add(kvp.Key);
            }

            // Decrement remaining timers
            var keys = new List<CardData>(_tempTimers.Keys);
            foreach (var key in keys)
            {
                if (!expired.Contains(key))
                    _tempTimers[key]--;
            }

            // Remove expired temp cards
            foreach (var card in expired)
            {
                Debug.Log($"[Board] Temp effect expired: {card.cardName}");
                RemoveCard(card);
            }

            EventBus.BoardTicked();
        }

        // ── Stat Aggregation Helpers ────────────────────────────────

        public float SumStat(System.Func<CardData, float> selector)
        {
            float total = 0f;
            foreach (var card in GetAllActiveCards())
                total += selector(card);
            return total;
        }

        public int TotalUpkeep()
        {
            int total = 0;
            foreach (var card in GetAllActiveCards())
                total += card.upkeepPerTurn;
            return total;
        }
    }
}
