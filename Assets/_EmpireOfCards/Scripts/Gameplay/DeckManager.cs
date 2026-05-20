using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class DeckManager : MonoBehaviour
    {
        [Header("Deck State")]
        [SerializeField] private List<CardData> drawPile = new List<CardData>();
        [SerializeField] private List<CardData> hand = new List<CardData>();
        [SerializeField] private List<CardData> discardPile = new List<CardData>();

        [Header("Settings")]
        [SerializeField] private int handSize = Constants.HAND_SIZE;
        [SerializeField] private int redrawsPerTurn = Constants.REDRAWS_PER_TURN;

        [Header("Redraw State")]
        [SerializeField] private int redrawsRemaining;

        private VentureDeckProfile _activeDeckProfile;
        private Dictionary<string, CardData> _lookup;

        public IReadOnlyList<CardData> DrawPile => drawPile;
        public IReadOnlyList<CardData> Hand => hand;
        public IReadOnlyList<CardData> DiscardPile => discardPile;
        public int RedrawsRemaining => redrawsRemaining;
        public int HandSize => handSize;
        public VentureDeckProfile ActiveDeckProfile => _activeDeckProfile;

        public List<string> GetDrawPileIds() => ToIds(drawPile);
        public List<string> GetHandIds() => ToIds(hand);
        public List<string> GetDiscardPileIds() => ToIds(discardPile);

        public void InitializeDeck(DeckPresetData preset)
        {
            Reset();
            if (preset == null) return;

            foreach (DeckEntry entry in preset.cards)
            {
                if (entry.card == null) continue;
                for (int i = 0; i < entry.count; i++)
                    drawPile.Add(entry.card);
            }

            ShuffleDeck();
            redrawsRemaining = redrawsPerTurn;
        }

        public void InitializeDeck(VentureDeckProfile profile, Dictionary<string, CardData> lookup)
        {
            Reset();
            _activeDeckProfile = profile;
            _lookup = lookup;

            if (_activeDeckProfile == null || _lookup == null)
                return;

            AddIdsToPile(_activeDeckProfile.starterCardIds, drawPile, 1);
            AddIdsToPile(_activeDeckProfile.neutralCardIds, drawPile, 1);

            ShuffleDeck();
            redrawsRemaining = redrawsPerTurn;
        }

        public void FilterByVenture(VentureType chosenVenture)
        {
            drawPile.RemoveAll(card => card != null && !card.isGeneralCard && card.ventureType != chosenVenture);
            discardPile.RemoveAll(card => card != null && !card.isGeneralCard && card.ventureType != chosenVenture);
            hand.RemoveAll(card => card != null && !card.isGeneralCard && card.ventureType != chosenVenture);
        }

        public void ShuffleDeck()
        {
            for (int i = drawPile.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (drawPile[i], drawPile[j]) = (drawPile[j], drawPile[i]);
            }
        }

        public List<CardData> DrawCards(int count)
        {
            EnsureDynamicPoolIfNeeded();

            var drawn = new List<CardData>();
            for (int i = 0; i < count; i++)
            {
                if (drawPile.Count == 0)
                {
                    RecycleDiscardPile();
                    EnsureDynamicPoolIfNeeded();
                    if (drawPile.Count == 0)
                        break;
                }

                var card = drawPile[0];
                drawPile.RemoveAt(0);
                hand.Add(card);
                drawn.Add(card);
                EventBus.CardDrawn(card);
            }

            return drawn;
        }

        public List<CardData> DrawToHandSize()
        {
            return DrawCards(Mathf.Max(0, handSize - hand.Count));
        }

        public bool DiscardCard(CardData card)
        {
            if (card == null || !hand.Remove(card))
                return false;

            discardPile.Add(card);
            EventBus.CardDiscarded(card);
            return true;
        }

        public void DiscardHand()
        {
            while (hand.Count > 0)
            {
                var card = hand[0];
                hand.RemoveAt(0);
                discardPile.Add(card);
                EventBus.CardDiscarded(card);
            }
        }

        public bool RedrawCard(CardData card)
        {
            if (redrawsRemaining <= 0)
                return false;

            if (!DiscardCard(card))
                return false;

            DrawCards(1);
            redrawsRemaining--;
            return true;
        }

        public void ResetRedraws()
        {
            redrawsRemaining = redrawsPerTurn;
        }

        public void AddCardToDeck(CardData card)
        {
            if (card != null)
                discardPile.Add(card);
        }

        public void RemoveFromHand(CardData card)
        {
            if (card != null)
                hand.Remove(card);
        }

        public void BurnCard(CardData card)
        {
            if (card == null) return;
            if (hand.Remove(card) || drawPile.Remove(card) || discardPile.Remove(card))
                EventBus.CardBurned(card);
        }

        public CardData DrawRandomEmployee()
        {
            var candidates = new List<CardData>();
            foreach (var card in drawPile)
                if (card != null && card.targetSlotType == SlotType.Staff)
                    candidates.Add(card);
            foreach (var card in discardPile)
                if (card != null && card.targetSlotType == SlotType.Staff)
                    candidates.Add(card);

            if (candidates.Count == 0)
                return null;

            var chosen = candidates[Random.Range(0, candidates.Count)];
            drawPile.Remove(chosen);
            discardPile.Remove(chosen);
            return chosen;
        }

        public int GetDrawPileCount() => drawPile.Count;
        public int GetHandCount() => hand.Count;
        public int GetDiscardPileCount() => discardPile.Count;
        public int GetTotalCardCount() => drawPile.Count + hand.Count + discardPile.Count;

        public List<CardData> DrawFirstTurnHand()
        {
            PullCardBySlotTypeToFront(SlotType.Operation, 0);
            PullCardBySlotTypeToFront(SlotType.Staff, 1);
            return DrawCards(handSize);
        }

        public void Reset()
        {
            drawPile.Clear();
            hand.Clear();
            discardPile.Clear();
            redrawsRemaining = 0;
        }

        public void RestoreState(
            VentureDeckProfile profile,
            Dictionary<string, CardData> lookup,
            IList<string> drawIds,
            IList<string> handIds,
            IList<string> discardIds,
            int restoredRedraws)
        {
            Reset();
            _activeDeckProfile = profile;
            _lookup = lookup;

            RestorePile(drawIds, drawPile);
            RestorePile(handIds, hand);
            RestorePile(discardIds, discardPile);
            redrawsRemaining = Mathf.Clamp(restoredRedraws, 0, redrawsPerTurn);

            for (int i = 0; i < hand.Count; i++)
                EventBus.CardDrawn(hand[i]);
        }

        private void PullCardBySlotTypeToFront(SlotType slotType, int insertAt)
        {
            for (int i = insertAt; i < drawPile.Count; i++)
            {
                if (drawPile[i] != null && drawPile[i].targetSlotType == slotType)
                {
                    if (i == insertAt) return;
                    var card = drawPile[i];
                    drawPile.RemoveAt(i);
                    drawPile.Insert(insertAt, card);
                    return;
                }
            }
        }

        private void RecycleDiscardPile()
        {
            if (discardPile.Count == 0)
                return;

            drawPile.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }

        private void EnsureDynamicPoolIfNeeded()
        {
            if (_activeDeckProfile == null || _lookup == null)
                return;

            if (drawPile.Count > 0)
                return;

            var gm = GameManager.Instance;
            int currentTurn = gm != null ? gm.CurrentTurn : 1;
            string[] basePool = currentTurn < 8
                ? _activeDeckProfile.earlyPoolCardIds
                : currentTurn < 16
                    ? _activeDeckProfile.midPoolCardIds
                    : _activeDeckProfile.latePoolCardIds;

            var generated = new List<CardData>();
            var combinedIds = new List<string>();
            if (basePool != null) combinedIds.AddRange(basePool);
            if (_activeDeckProfile.neutralCardIds != null) combinedIds.AddRange(_activeDeckProfile.neutralCardIds);
            if (combinedIds.Count == 0) return;

            for (int i = 0; i < 6; i++)
            {
                var card = ChooseBiasedCard(combinedIds);
                if (card != null)
                    generated.Add(card);
            }

            drawPile.AddRange(generated);
            ShuffleDeck();
        }

        private CardData ChooseBiasedCard(List<string> candidateIds)
        {
            var gm = GameManager.Instance;
            BoardPressureType pressure = gm != null && gm.EconomyManager != null
                ? gm.EconomyManager.CurrentPressure
                : BoardPressureType.None;

            CardFamily preferred = CardFamily.Neutral;
            float bonusWeight = 1f;
            if (_activeDeckProfile.drawBiasRules != null)
            {
                foreach (var rule in _activeDeckProfile.drawBiasRules)
                {
                    if (rule.pressure != pressure) continue;
                    preferred = rule.preferredFamily;
                    bonusWeight = Mathf.Max(1f, rule.bonusWeight);
                    break;
                }
            }

            float totalWeight = 0f;
            var weightedCards = new List<(CardData card, float weight)>();
            foreach (var id in candidateIds)
            {
                if (!_lookup.TryGetValue(id, out var card) || card == null)
                    continue;

                float weight = 1f;
                if (card.cardFamily == preferred)
                    weight *= bonusWeight;

                if (card.preferredPressures != null)
                {
                    foreach (var p in card.preferredPressures)
                    {
                        if (p == pressure)
                        {
                            weight *= 1.5f;
                            break;
                        }
                    }
                }

                weightedCards.Add((card, weight));
                totalWeight += weight;
            }

            if (weightedCards.Count == 0)
                return null;

            float roll = Random.Range(0f, totalWeight);
            float cursor = 0f;
            foreach (var item in weightedCards)
            {
                cursor += item.weight;
                if (roll <= cursor)
                    return item.card;
            }

            return weightedCards[weightedCards.Count - 1].card;
        }

        private void RestorePile(IList<string> ids, List<CardData> target)
        {
            if (ids == null || _lookup == null)
                return;

            for (int i = 0; i < ids.Count; i++)
            {
                string id = ids[i];
                if (string.IsNullOrWhiteSpace(id))
                    continue;
                if (_lookup.TryGetValue(id, out var card) && card != null)
                    target.Add(card);
            }
        }

        private static List<string> ToIds(List<CardData> cards)
        {
            var ids = new List<string>(cards.Count);
            for (int i = 0; i < cards.Count; i++)
                ids.Add(cards[i] != null ? cards[i].cardId : string.Empty);
            return ids;
        }

        private void AddIdsToPile(string[] ids, List<CardData> target, int copies)
        {
            if (ids == null || _lookup == null)
                return;

            foreach (string id in ids)
            {
                if (!_lookup.TryGetValue(id, out var card) || card == null)
                    continue;

                for (int i = 0; i < copies; i++)
                    target.Add(card);
            }
        }
    }
}
