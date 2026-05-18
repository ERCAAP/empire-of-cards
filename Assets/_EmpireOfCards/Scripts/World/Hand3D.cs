using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.World
{
    public class Hand3D : MonoBehaviour
    {
        [Header("Hand Layout")]
        [SerializeField] private Transform handAnchor;
        [SerializeField] private float cardSpacing = 0.8f;
        [SerializeField] private float fanAngle = 8f;
        [SerializeField] private float verticalArc = 0.15f;

        private readonly List<Card3D> _cards = new List<Card3D>();
        private CardFactory _factory;

        public IReadOnlyList<Card3D> Cards => _cards;

        public void SetFactory(CardFactory factory) { _factory = factory; }
        public void SetAnchor(Transform anchor) { handAnchor = anchor; }

        private void OnEnable()
        {
            EventBus.OnCardDrawn += OnCardDrawn;
            EventBus.OnCardDiscarded += OnCardDiscarded;
            EventBus.OnCardPlayed += OnCardPlayed;
            EventBus.OnCardBurned += OnCardBurned;
        }

        private void OnDisable()
        {
            EventBus.OnCardDrawn -= OnCardDrawn;
            EventBus.OnCardDiscarded -= OnCardDiscarded;
            EventBus.OnCardPlayed -= OnCardPlayed;
            EventBus.OnCardBurned -= OnCardBurned;
        }

        private void OnCardDrawn(CardData card)
        {
            if (_factory == null) return;

            Card3D card3D = _factory.CreateCard(card);
            card3D.IsInHand = true;
            _cards.Add(card3D);
            LayoutHand();
        }

        private void OnCardDiscarded(CardData card) { RemoveCardFromHand(card); }
        private void OnCardPlayed(CardData card) { RemoveCardFromHand(card); }
        private void OnCardBurned(CardData card) { RemoveCardFromHand(card); }

        private void RemoveCardFromHand(CardData card)
        {
            for (int i = _cards.Count - 1; i >= 0; i--)
            {
                if (_cards[i].CardData == card)
                {
                    Destroy(_cards[i].gameObject);
                    _cards.RemoveAt(i);
                    break;
                }
            }
            LayoutHand();
        }

        public void ClearHand()
        {
            foreach (var c in _cards)
                if (c != null) Destroy(c.gameObject);
            _cards.Clear();
        }

        private void LayoutHand()
        {
            if (handAnchor == null) return;

            int count = _cards.Count;
            if (count == 0) return;

            float totalWidth = (count - 1) * cardSpacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < count; i++)
            {
                float t = count > 1 ? (float)i / (count - 1) : 0.5f;
                float x = startX + i * cardSpacing;
                float y = -Mathf.Pow(t - 0.5f, 2) * 4f * verticalArc + verticalArc;
                float angle = Mathf.Lerp(fanAngle, -fanAngle, t);

                Vector3 localPos = new Vector3(x, y, -i * 0.02f);
                Vector3 worldPos = handAnchor.TransformPoint(localPos);
                Quaternion rot = handAnchor.rotation * Quaternion.Euler(0, 0, angle);

                _cards[i].SetHandPosition(worldPos, rot);
            }
        }
    }
}
