using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class DeckManager : MonoBehaviour
    {
        CardData[] _allCards;
        CardData[] _shopPool;

        readonly List<CardData> _drawPile = new();
        readonly List<CardData> _hand = new();
        readonly List<CardData> _discardPile = new();

        int _redrawsRemaining;

        public IReadOnlyList<CardData> Hand => _hand;
        public int DrawPileCount => _drawPile.Count;
        public int DiscardPileCount => _discardPile.Count;

        public void Init(CardData[] allCards, CardData[] shopPool)
        {
            _allCards = allCards;
            _shopPool = shopPool;

            _drawPile.Clear();
            _drawPile.AddRange(shopPool);
            Shuffle(_drawPile);

            Debug.Log($"[DeckManager] Initialized with {allCards.Length} cards, {shopPool.Length} in shop.");
        }

        // ── EventBus subscriptions ─────────────────────────────────

        void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
        }

        void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
        }

        void HandlePhaseStarted(TurnPhase phase)
        {
            if (phase == TurnPhase.DrawPhase)
            {
                DiscardHand();
                DrawHand(Constants.HAND_SIZE);
                _redrawsRemaining = Constants.REDRAW_PER_TURN;
            }
        }

        // ── Draw ───────────────────────────────────────────────────

        public void DrawHand(int count)
        {
            _hand.Clear();

            var weights = GetBiasWeights();

            for (int i = 0; i < count; i++)
            {
                if (_drawPile.Count == 0) RecycleDiscard();
                if (_drawPile.Count == 0) break;

                int index = PickWeightedIndex(weights);
                var card = _drawPile[index];
                _drawPile.RemoveAt(index);
                _hand.Add(card);
            }

            EventBus.HandDrawn(_hand.ToArray());
            Debug.Log($"[DeckManager] Drew {_hand.Count} cards. Draw pile: {_drawPile.Count}");
        }

        // ── Discard ────────────────────────────────────────────────

        public void DiscardHand()
        {
            if (_hand.Count == 0) return;

            _discardPile.AddRange(_hand);
            _hand.Clear();
            EventBus.HandDiscarded();
        }

        // ── Recycle ────────────────────────────────────────────────

        public void RecycleDiscard()
        {
            if (_discardPile.Count == 0) return;

            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
            Shuffle(_drawPile);
            Debug.Log($"[DeckManager] Recycled discard into draw pile ({_drawPile.Count} cards).");
        }

        // ── Redraw ─────────────────────────────────────────────────

        public bool Redraw(int index)
        {
            if (_redrawsRemaining <= 0) return false;
            if (index < 0 || index >= _hand.Count) return false;

            var discarded = _hand[index];
            _discardPile.Add(discarded);
            EventBus.CardDiscarded(discarded);

            if (_drawPile.Count == 0) RecycleDiscard();

            if (_drawPile.Count > 0)
            {
                var weights = GetBiasWeights();
                int drawIndex = PickWeightedIndex(weights);
                var newCard = _drawPile[drawIndex];
                _drawPile.RemoveAt(drawIndex);
                _hand[index] = newCard;
                EventBus.CardDrawn(newCard);
            }
            else
            {
                _hand.RemoveAt(index);
            }

            _redrawsRemaining--;
            return true;
        }

        // ── Bias Weights ───────────────────────────────────────────

        Dictionary<CardType, float> GetBiasWeights()
        {
            var weights = new Dictionary<CardType, float>();
            foreach (CardType ct in System.Enum.GetValues(typeof(CardType)))
                weights[ct] = Constants.BIAS_WEIGHT_NORMAL;

            var gm = GameManager.Instance;
            if (gm == null) return weights;
            var res = gm.Resources;

            if (res.GetCapacity() < res.GetDemand())
            {
                weights[CardType.Staff] = Constants.BIAS_WEIGHT_BOOSTED;
                weights[CardType.Operation] = Constants.BIAS_WEIGHT_BOOSTED;
            }

            if (res.GetRating() < Constants.CRISIS_RATING_THRESHOLD)
                weights[CardType.Reaction] = Constants.BIAS_WEIGHT_BOOSTED;

            if (res.GetHygiene() < Constants.CRISIS_QUALITY_HYGIENE_THRESHOLD)
                weights[CardType.Supplier] = Constants.BIAS_WEIGHT_BOOSTED;

            if (res.GetStaffStability() < Constants.CRISIS_STABILITY_THRESHOLD)
                weights[CardType.Staff] = Constants.BIAS_WEIGHT_BOOSTED;

            if (res.GetMoney() < 50)
                weights[CardType.Marketing] = Constants.BIAS_WEIGHT_NORMAL * 0.5f;

            return weights;
        }

        // ── Utility ────────────────────────────────────────────────

        int PickWeightedIndex(Dictionary<CardType, float> weights)
        {
            if (_drawPile.Count == 0) return -1;
            if (_drawPile.Count == 1) return 0;

            float totalWeight = 0f;
            for (int i = 0; i < _drawPile.Count; i++)
                totalWeight += weights.TryGetValue(_drawPile[i].cardType, out float w) ? w : 1f;

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < _drawPile.Count; i++)
            {
                float w = weights.TryGetValue(_drawPile[i].cardType, out float wt) ? wt : 1f;
                cumulative += w;
                if (roll <= cumulative) return i;
            }

            return _drawPile.Count - 1;
        }

        static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
