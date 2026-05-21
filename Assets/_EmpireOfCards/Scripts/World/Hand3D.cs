using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.World
{
    public class Hand3D : MonoBehaviour
    {
        // ── Config ──────────────────────────────────────────────────
        const float FAN_ARC_HALF   = 30f;   // degrees from center to edge
        const float FAN_RADIUS     = 3.5f;  // arc radius
        const float HOVER_LIFT     = 0.4f;  // Y lift on hover
        const int   MAX_HAND_SIZE  = 5;

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
            foreach (var data in cards)
            {
                if (_handCards.Count >= MAX_HAND_SIZE) break;
                var card3d = _factory.CreateCard(data);
                AddCard(card3d);
            }
        }

        void HandleCardDrawn(CardData data)
        {
            if (_handCards.Count >= MAX_HAND_SIZE) return;
            var card3d = _factory.CreateCard(data);
            AddCard(card3d);
        }

        void HandleHandDiscarded()
        {
            ClearHand();
        }

        // ── Public API ──────────────────────────────────────────────

        public void AddCard(Card3D card)
        {
            if (card == null || _handCards.Count >= MAX_HAND_SIZE) return;

            card.SetState(Card3DState.InHand);
            card.transform.SetParent(transform);
            _handCards.Add(card);
            ArrangeCards();
        }

        public void RemoveCard(Card3D card)
        {
            if (card == null) return;

            _handCards.Remove(card);
            card.transform.SetParent(null);

            if (_hoveredCard == card)
                _hoveredCard = null;

            ArrangeCards();
        }

        public void SetHovered(Card3D card)
        {
            if (_hoveredCard == card) return;

            _hoveredCard = card;
            ArrangeCards();
        }

        public void ClearHover()
        {
            _hoveredCard = null;
            ArrangeCards();
        }

        // ── Fan layout ──────────────────────────────────────────────

        void ArrangeCards()
        {
            int count = _handCards.Count;
            if (count == 0) return;

            float totalArc = count > 1 ? FAN_ARC_HALF * 2f : 0f;
            float startAngle = -totalArc * 0.5f;
            float step = count > 1 ? totalArc / (count - 1) : 0f;

            for (int i = 0; i < count; i++)
            {
                var card = _handCards[i];
                float angle = startAngle + step * i;
                float rad = angle * Mathf.Deg2Rad;

                float x = Mathf.Sin(rad) * FAN_RADIUS;
                float z = (Mathf.Cos(rad) - 1f) * FAN_RADIUS * 0.3f;
                float y = 0.1f + i * 0.02f; // slight stacking

                // Hover lift
                if (card == _hoveredCard)
                    y += HOVER_LIFT;

                card.transform.localPosition = new Vector3(x, y, z);
                card.transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
            }
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
