using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.World
{
    public class Hand3D : MonoBehaviour
    {
        [Header("Hand Layout")]
        [SerializeField] private Transform handAnchor;
        [SerializeField] private Transform buildRailAnchor;
        [SerializeField] private Transform responseRailAnchor;
        [SerializeField] private float cardSpacing = 1.18f;
        [SerializeField] private float fanAngle = 8f;
        [SerializeField] private float verticalArc = 0.14f;

        private readonly List<Card3D> _cards = new List<Card3D>();

        [SerializeField] private CardFactory cardFactory;
        [SerializeField] private DeckManager deckManager;

        public IReadOnlyList<Card3D> Cards => _cards;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(CardFactory factory, DeckManager deck, Transform anchor)
        {
            Init(factory, deck, anchor, anchor);
        }

        public void Init(CardFactory factory, DeckManager deck, Transform buildAnchor, Transform responseAnchor)
        {
            this.cardFactory = factory;
            this.deckManager = deck;
            this.buildRailAnchor = buildAnchor;
            this.responseRailAnchor = responseAnchor != null ? responseAnchor : buildAnchor;
            this.handAnchor = buildRailAnchor;
        }

        private void OnEnable()
        {
            EventBus.OnCardDrawn += OnCardDrawn;
            EventBus.OnCardDiscarded += OnCardDiscarded;
            EventBus.OnCardPlayed += OnCardPlayed;
            EventBus.OnCardBurned += OnCardBurned;
            EventBus.OnTurnStarted += HandleTurnStarted;
        }

        private void OnDisable()
        {
            EventBus.OnCardDrawn -= OnCardDrawn;
            EventBus.OnCardDiscarded -= OnCardDiscarded;
            EventBus.OnCardPlayed -= OnCardPlayed;
            EventBus.OnCardBurned -= OnCardBurned;
            EventBus.OnTurnStarted -= HandleTurnStarted;
        }

        private void HandleTurnStarted(int turn)
        {
            RefreshLayout();
        }

        private void OnCardDrawn(CardData card)
        {
            if (cardFactory == null) return;

            Card3D card3D = cardFactory.CreateCard(card);
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
                if (_cards[i] == null) { _cards.RemoveAt(i); continue; }
                if (_cards[i].CardData != card) continue;

                // ONLY touch cards that are still in-hand.
                // Board-placed cards (IsInHand=false) must NEVER be destroyed or removed here.
                if (!_cards[i].IsInHand) continue;

                Destroy(_cards[i].gameObject);
                _cards.RemoveAt(i);
                break;
            }
            LayoutHand();
        }

        /// <summary>
        /// Public entry point called by bootstrap's Wire3DInteraction.
        /// Spawns a 3D card from CardData and adds it to the hand.
        /// NOTE: Hand3D also subscribes to EventBus.OnCardDrawn internally,
        /// so callers should avoid double-calling.
        /// </summary>
        public void AddCard(CardData card)
        {
            if (cardFactory == null) return;

            Card3D card3D = cardFactory.CreateCard(card);
            card3D.IsInHand = true;
            _cards.Add(card3D);
            LayoutHand();
        }

        /// <summary>
        /// Public entry point to remove a card by its CardData.
        /// Called by bootstrap's Wire3DInteraction.
        /// </summary>
        public void RemoveCard(CardData card)
        {
            RemoveCardFromHand(card);
        }

        /// <summary>
        /// Public entry point to re-layout the hand fan.
        /// Called by bootstrap on turn start.
        /// </summary>
        public void RefreshLayout()
        {
            LayoutHand();
        }

        public void ApplyPresentationProfile(BoardCameraProfile profile)
        {
            if (profile == null)
                return;

            cardSpacing = profile.handSpacing;
            fanAngle = profile.handFanAngle;
            verticalArc = profile.handVerticalArc;

            if (handAnchor != null)
            {
                handAnchor.position = profile.handAnchorPosition;
                handAnchor.rotation = Quaternion.Euler(profile.handAnchorEuler);
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
            if (buildRailAnchor == null && handAnchor != null)
                buildRailAnchor = handAnchor;
            if (responseRailAnchor == null)
                responseRailAnchor = buildRailAnchor;
            if (buildRailAnchor == null) return;

            var buildCards = new List<Card3D>();
            var responseCards = new List<Card3D>();
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i] != null && _cards[i].IsInHand)
                {
                    if (QuestionTagUtility.IsPersistentBuild(_cards[i].CardData))
                        buildCards.Add(_cards[i]);
                    else
                        responseCards.Add(_cards[i]);
                }
            }

            LayoutRail(buildCards, buildRailAnchor, -0.02f);
            LayoutRail(responseCards, responseRailAnchor, -0.02f);
        }

        private void LayoutRail(List<Card3D> handCards, Transform railAnchor, float depthStep)
        {
            if (railAnchor == null)
                return;

            int count = handCards.Count;
            if (count == 0) return;

            float spacing = count >= 4 ? Mathf.Max(0.92f, cardSpacing - 0.16f) : cardSpacing;
            float totalWidth = (count - 1) * spacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < count; i++)
            {
                float t = count > 1 ? (float)i / (count - 1) : 0.5f;
                float x = startX + i * spacing;
                float y = -Mathf.Pow(t - 0.5f, 2) * 4f * verticalArc + verticalArc;
                float angle = Mathf.Lerp(fanAngle, -fanAngle, t);

                Vector3 localPos = new Vector3(x, y, -i * Mathf.Abs(depthStep));
                Vector3 worldPos = railAnchor.TransformPoint(localPos);
                Quaternion rot = railAnchor.rotation * Quaternion.Euler(0, 0, angle);

                handCards[i].SetHandPosition(worldPos, rot);
            }
        }
    }
}
