using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using DG.Tweening;

namespace EmpireOfCards.World
{
    public class Hand3D : MonoBehaviour
    {
        // ── Config ──────────────────────────────────────────────────
        const float FAN_ARC_HALF   = 15f;   // degrees from center to edge (tighter fan)
        const float FAN_RADIUS     = 1.5f;  // arc radius (smaller, cards stay closer)
        const float CARD_SPACING   = 1.4f;  // horizontal spacing between cards
        const float HOVER_LIFT     = 0.5f;  // Y lift on hover
        const int   MAX_HAND_SIZE  = 5;
        const float REARRANGE_DURATION = 0.3f;

        // ── State ───────────────────────────────────────────────────
        readonly List<Card3D> _handCards = new();
        Card3D _hoveredCard;

        // ── References ──────────────────────────────────────────────
        CardFactory3D _factory;

        // ── Properties ──────────────────────────────────────────────
        public IReadOnlyList<Card3D> HandCards => _handCards;
        public int Count => _handCards.Count;

        // ── Init ────────────────────────────────────────────────────

        public void Init(CardFactory3D factory)
        {
            _factory = factory;
        }

        // ── EventBus ────────────────────────────────────────────────

        void OnEnable()
        {
            EventBus.OnHandDrawn += HandleHandDrawn;
            EventBus.OnHandDiscarded += HandleHandDiscarded;
            EventBus.OnCardDrawn += HandleCardDrawn;
        }

        void OnDisable()
        {
            EventBus.OnHandDrawn -= HandleHandDrawn;
            EventBus.OnHandDiscarded -= HandleHandDiscarded;
            EventBus.OnCardDrawn -= HandleCardDrawn;
        }

        void HandleHandDrawn(CardData[] cards)
        {
            ClearHand();
            for (int i = 0; i < cards.Length; i++)
            {
                if (_handCards.Count >= MAX_HAND_SIZE) break;
                var card3d = _factory.CreateCard(cards[i]);
                AddCardWithDrawAnimation(card3d, i);
            }
        }

        void HandleCardDrawn(CardData data)
        {
            if (_handCards.Count >= MAX_HAND_SIZE) return;
            var card3d = _factory.CreateCard(data);
            AddCardWithDrawAnimation(card3d, _handCards.Count);
        }

        void HandleHandDiscarded()
        {
            DiscardAllAnimated();
        }

        // ── Public API ──────────────────────────────────────────────

        public void AddCard(Card3D card)
        {
            if (card == null || _handCards.Count >= MAX_HAND_SIZE) return;

            card.SetState(Card3DState.InHand);
            card.transform.SetParent(transform);
            _handCards.Add(card);
            ArrangeCardsAnimated();
        }

        /// <summary>
        /// Add card with draw animation (card flies in from above).
        /// </summary>
        public void AddCardWithDrawAnimation(Card3D card, int index)
        {
            if (card == null || _handCards.Count >= MAX_HAND_SIZE) return;

            card.SetState(Card3DState.InHand);
            card.transform.SetParent(transform);
            _handCards.Add(card);

            // Calculate target position in the fan
            Vector3 targetLocalPos = CalculateCardPosition(index, _handCards.Count);
            Vector3 targetWorldPos = transform.TransformPoint(targetLocalPos);

            // Trigger draw animation
            DOTweenAnimations.CardDrawAnimation(card.transform, targetWorldPos, index);

            // Rearrange all cards after a brief delay to let draw animation start
            DOVirtual.DelayedCall(0.1f, ArrangeCardsAnimated);
        }

        public void RemoveCard(Card3D card)
        {
            if (card == null) return;

            _handCards.Remove(card);
            card.transform.SetParent(null);

            if (_hoveredCard == card)
                _hoveredCard = null;

            ArrangeCardsAnimated();
        }

        public void SetHovered(Card3D card)
        {
            if (_hoveredCard == card) return;

            // Unhover previous
            if (_hoveredCard != null)
                _hoveredCard.AnimateUnhover();

            _hoveredCard = card;

            // Hover new
            if (_hoveredCard != null)
                _hoveredCard.AnimateHover();
        }

        public void ClearHover()
        {
            if (_hoveredCard != null)
                _hoveredCard.AnimateUnhover();

            _hoveredCard = null;
        }

        // ── Animated discard all ────────────────────────────────────

        void DiscardAllAnimated()
        {
            for (int i = _handCards.Count - 1; i >= 0; i--)
            {
                var card = _handCards[i];
                if (card != null && card.gameObject != null)
                {
                    float delay = ((_handCards.Count - 1) - i) * 0.08f;
                    var c = card;
                    var f = _factory;
                    DOVirtual.DelayedCall(delay, () =>
                    {
                        if (c != null && c.gameObject != null)
                        {
                            c.AnimateDiscard();
                            DOVirtual.DelayedCall(0.5f, () => f.ReturnCard(c));
                        }
                    });
                }
            }
            _handCards.Clear();
            _hoveredCard = null;
        }

        // ── Fan layout ──────────────────────────────────────────────

        void ArrangeCards()
        {
            int count = _handCards.Count;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                var card = _handCards[i];
                Vector3 pos = CalculateCardPosition(i, count);

                // Hover lift
                if (card == _hoveredCard)
                    pos.y += HOVER_LIFT;

                float angle = CalculateCardAngle(i, count);
                card.transform.localPosition = pos;
                card.transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
                card.SetRestPosition(pos, card.transform.localScale);
            }
        }

        /// <summary>
        /// Animated version of ArrangeCards -- smoothly DOTweens cards to position.
        /// </summary>
        void ArrangeCardsAnimated()
        {
            int count = _handCards.Count;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                var card = _handCards[i];
                Vector3 targetPos = CalculateCardPosition(i, count);

                // Hover lift
                if (card == _hoveredCard)
                    targetPos.y += HOVER_LIFT;

                float angle = CalculateCardAngle(i, count);
                Quaternion targetRot = Quaternion.Euler(0f, -angle, 0f);

                card.transform.DOLocalMove(targetPos, REARRANGE_DURATION).SetEase(Ease.OutBack);
                card.transform.DOLocalRotateQuaternion(targetRot, REARRANGE_DURATION).SetEase(Ease.OutQuad);
                card.SetRestPosition(targetPos, card.transform.localScale);
            }
        }

        // ── Position/angle calculation helpers ──────────────────────

        Vector3 CalculateCardPosition(int index, int count)
        {
            // Simple horizontal row layout, centered
            float totalWidth = (count - 1) * CARD_SPACING;
            float startX = -totalWidth * 0.5f;
            float x = startX + index * CARD_SPACING;
            float y = 0.15f + index * 0.02f; // slight Y offset for depth
            float z = 0f;

            return new Vector3(x, y, z);
        }

        float CalculateCardAngle(int index, int count)
        {
            // Slight tilt per card for visual depth
            float totalArc = count > 1 ? FAN_ARC_HALF * 2f : 0f;
            float startAngle = -totalArc * 0.5f;
            float step = count > 1 ? totalArc / (count - 1) : 0f;
            return (startAngle + step * index) * 0.3f; // reduced rotation
        }

        // ── Cleanup ─────────────────────────────────────────────────

        void ClearHand()
        {
            foreach (var card in _handCards)
            {
                if (card != null && card.gameObject != null)
                    _factory.ReturnCard(card);
            }
            _handCards.Clear();
            _hoveredCard = null;
        }
    }
}
