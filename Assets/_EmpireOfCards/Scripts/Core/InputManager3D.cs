using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.World;

namespace EmpireOfCards.Core
{
    public class InputManager3D : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float dragHeight = 1.15f;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AbilitySystem abilitySystem;

        // State
        private Card3D _hoveredCard;
        private Card3D _draggedCard;
        private SlotZone3D _hoveredSlot;
        private QuestionDropZone3D _hoveredQuestionZone;
        private bool _isDragging;
        private bool _inputEnabled;
        private readonly List<SlotZone3D> _slotZones = new List<SlotZone3D>();
        private readonly List<QuestionDropZone3D> _questionZones = new List<QuestionDropZone3D>();

        // Events
        public event Action<Card3D> OnCardHoverEnter;
        public event Action<Card3D> OnCardHoverExit;
        public event Action<Card3D> OnCardPickedUp;
        public event Action<Card3D, SlotZone3D> OnCardDropped;
        public event Action<Card3D, QuestionDropZone3D> OnCardDroppedOnQuestion;
        public event Action<Card3D> OnCardReturnedToHand;
        public event Action<Card3D> OnAbilityUsed;
        public event Action<Card3D, SlotZone3D, bool> OnDragSlotHoverChanged;
        public event Action<Card3D, QuestionDropZone3D, bool> OnDragQuestionHoverChanged;

        public bool InputEnabled { get => _inputEnabled; set => _inputEnabled = value; }
        public Card3D DraggedCard => _draggedCard;

        public void InitRuntime(Camera cameraRef, GameManager gameManagerRef, AbilitySystem abilitySystemRef, IReadOnlyList<SlotZone3D> slotZones, IReadOnlyList<QuestionDropZone3D> questionZones)
        {
            mainCamera = cameraRef;
            gameManager = gameManagerRef;
            abilitySystem = abilitySystemRef;

            _slotZones.Clear();
            if (slotZones != null)
            {
                for (int i = 0; i < slotZones.Count; i++)
                {
                    if (slotZones[i] != null)
                        _slotZones.Add(slotZones[i]);
                }
            }

            _questionZones.Clear();
            if (questionZones != null)
            {
                for (int i = 0; i < questionZones.Count; i++)
                {
                    if (questionZones[i] != null)
                        _questionZones.Add(questionZones[i]);
                }
            }
        }

        private void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
        }

        private void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
        }

        private void HandlePhaseStarted(TurnPhase phase)
        {
            _inputEnabled = (phase == TurnPhase.PlayPhase);
        }

        private void Start()
        {
            if (mainCamera == null) mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!_inputEnabled) return;
            if (Mouse.current == null) return;

            if (_isDragging)
                HandleDrag();
            else
                HandleHover();

            // Pick up / Ability activation
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame && !_isDragging && _hoveredCard != null)
            {
                if (_hoveredCard.IsInHand)
                    PickUpCard(_hoveredCard);
                else
                    TryActivateAbility(_hoveredCard);
            }

            // Right-click: redraw a card in hand (P1 #8)
            if (mouse.rightButton.wasPressedThisFrame && _hoveredCard != null && _hoveredCard.IsInHand)
            {
                var dm = gameManager != null ? gameManager.DeckManager : null;
                if (dm != null && dm.RedrawsRemaining > 0)
                {
                    dm.RedrawCard(_hoveredCard.CardData);
                    // Card removal from hand is handled via EventBus.OnCardDiscarded -> Hand3D
                }
            }

            // Drop
            if (mouse.leftButton.wasReleasedThisFrame && _isDragging)
                DropCard();
        }

        private void HandleHover()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            // Raycast against ALL layers then check for Card3D component
            // (LayerMask fields default to 0/Nothing when AddComponent'd at runtime)
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                var card = hit.collider.GetComponent<Card3D>();
                if (card == null) card = hit.collider.GetComponentInParent<Card3D>();

                if (card != null && card != _hoveredCard)
                {
                    if (_hoveredCard != null)
                    {
                        _hoveredCard.SetHovered(false);
                        OnCardHoverExit?.Invoke(_hoveredCard);
                    }
                    _hoveredCard = card;
                    _hoveredCard.SetHovered(true);
                    OnCardHoverEnter?.Invoke(_hoveredCard);
                }
                else if (card == null && _hoveredCard != null)
                {
                    _hoveredCard.SetHovered(false);
                    OnCardHoverExit?.Invoke(_hoveredCard);
                    _hoveredCard = null;
                }
            }
            else if (_hoveredCard != null)
            {
                _hoveredCard.SetHovered(false);
                OnCardHoverExit?.Invoke(_hoveredCard);
                _hoveredCard = null;
            }
        }

        private void HandleDrag()
        {
            if (_draggedCard == null) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane dragPlane = new Plane(Vector3.up, new Vector3(0, dragHeight, 0));

            if (dragPlane.Raycast(ray, out float dist))
            {
                Vector3 worldPos = ray.GetPoint(dist);
                _draggedCard.transform.position = Vector3.Lerp(
                    _draggedCard.transform.position, worldPos, Time.deltaTime * 22f);
            }

            // Check question hover first, then slot hover.
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                var question = hit.collider.GetComponent<QuestionDropZone3D>();
                if (question == null) question = hit.collider.GetComponentInParent<QuestionDropZone3D>();

                if (question != null)
                {
                    if (_hoveredSlot != null)
                    {
                        _hoveredSlot.SetHighlight(false);
                        if (_hoveredSlot.CanAccept(_draggedCard.CardData))
                            _hoveredSlot.SetPulse(true);
                        OnDragSlotHoverChanged?.Invoke(_draggedCard, null, false);
                        _hoveredSlot = null;
                    }

                    if (question != _hoveredQuestionZone)
                    {
                        if (_hoveredQuestionZone != null)
                            _hoveredQuestionZone.SetHighlight(false, false);
                        _hoveredQuestionZone = question;
                        bool valid = _hoveredQuestionZone.CanAccept(_draggedCard.CardData);
                        _hoveredQuestionZone.SetHighlight(true, valid);
                        OnDragQuestionHoverChanged?.Invoke(_draggedCard, _hoveredQuestionZone, valid);
                    }

                    return;
                }
                else if (_hoveredQuestionZone != null)
                {
                    _hoveredQuestionZone.SetHighlight(false, false);
                    OnDragQuestionHoverChanged?.Invoke(_draggedCard, null, false);
                    _hoveredQuestionZone = null;
                }

                var slot = hit.collider.GetComponent<SlotZone3D>();
                if (slot == null) slot = hit.collider.GetComponentInParent<SlotZone3D>();

                if (slot != null)
                {
                    if (slot != _hoveredSlot)
                    {
                        // Restore previous hovered slot back to pulse if valid
                        if (_hoveredSlot != null)
                        {
                            _hoveredSlot.SetHighlight(false);
                            if (_hoveredSlot.CanAccept(_draggedCard.CardData))
                                _hoveredSlot.SetPulse(true);
                        }
                        _hoveredSlot = slot;
                        bool valid = _hoveredSlot.CanAccept(_draggedCard.CardData);
                        _hoveredSlot.SetHighlight(true, valid);
                        OnDragSlotHoverChanged?.Invoke(_draggedCard, _hoveredSlot, valid);
                    }
                }
                else if (_hoveredSlot != null)
                {
                    _hoveredSlot.SetHighlight(false);
                    if (_hoveredSlot.CanAccept(_draggedCard.CardData))
                        _hoveredSlot.SetPulse(true);
                    OnDragSlotHoverChanged?.Invoke(_draggedCard, null, false);
                    _hoveredSlot = null;
                }
            }
            else if (_hoveredSlot != null)
            {
                _hoveredSlot.SetHighlight(false);
                if (_hoveredSlot.CanAccept(_draggedCard.CardData))
                    _hoveredSlot.SetPulse(true);
                OnDragSlotHoverChanged?.Invoke(_draggedCard, null, false);
                _hoveredSlot = null;
            }

            if (!Physics.Raycast(ray, out _, 100f) && _hoveredQuestionZone != null)
            {
                _hoveredQuestionZone.SetHighlight(false, false);
                OnDragQuestionHoverChanged?.Invoke(_draggedCard, null, false);
                _hoveredQuestionZone = null;
            }
        }

        /// <summary>
        /// Attempts to activate a placed employee's active ability (P0 #3).
        /// Only works on Employee-type cards that are placed on the board (not in hand).
        /// </summary>
        private void TryActivateAbility(Card3D card)
        {
            if (card.CardData == null) return;
            if (card.CardData.cardType != CardType.Employee) return;
            if (card.CardData.activeAbilityType == ActiveAbilityType.None) return;

            var gm = gameManager;
            if (gm == null || gm.PlayerActions <= 0) return;
            if (gm.TurnManager == null || gm.TurnManager.CurrentPhase != TurnPhase.PlayPhase) return;
            if (abilitySystem == null) return;

            int businessIndex = gm.BoardManager != null
                ? gm.BoardManager.FindBusinessWithEmployee(card.CardData)
                : -1;

            if (abilitySystem.TryUseAbility(card.CardData, businessIndex))
            {
                OnAbilityUsed?.Invoke(card);
            }
        }

        private void PickUpCard(Card3D card)
        {
            var gm = gameManager;
            if (gm == null || gm.PlayerActions <= 0) return;
            if (gm.TurnManager == null || gm.TurnManager.CurrentPhase != TurnPhase.PlayPhase) return;
            _isDragging = true;
            _draggedCard = card;

            card.SetDragging(true);
            card.transform.rotation = Quaternion.Euler(18f, 0f, 0f);

            // Pulse all valid drop zones green while dragging
            foreach (var s in _slotZones)
            {
                if (s != null && s.CanAccept(card.CardData))
                    s.SetPulse(true);
            }
            foreach (var q in _questionZones)
            {
                if (q != null && q.CanAccept(card.CardData))
                    q.SetPulse(true);
            }

            OnCardPickedUp?.Invoke(card);
        }

        private void DropCard()
        {
            if (_draggedCard == null) return;

            // Clear all highlights and pulses
            foreach (var s in _slotZones)
            {
                if (s == null)
                    continue;
                s.SetHighlight(false);
                s.SetPulse(false);
            }
            foreach (var q in _questionZones)
            {
                if (q == null)
                    continue;
                q.SetHighlight(false, false);
                q.SetPulse(false);
            }

            if (_hoveredQuestionZone != null && _hoveredQuestionZone.CanAccept(_draggedCard.CardData))
            {
                _draggedCard.SetDragging(false);
                OnCardDroppedOnQuestion?.Invoke(_draggedCard, _hoveredQuestionZone);
            }
            else if (_hoveredSlot != null && _hoveredSlot.CanAccept(_draggedCard.CardData))
            {
                // Valid drop -- snap card to slot with overshoot animation
                _draggedCard.SetDragging(false);
                Vector3 slotTarget = _hoveredSlot.transform.position + Vector3.up * 0.15f;
                _draggedCard.SnapToPosition(slotTarget, Quaternion.identity);
                OnCardDropped?.Invoke(_draggedCard, _hoveredSlot);
            }
            else
            {
                // Return to hand with snap animation
                _draggedCard.SetDragging(false);
                _draggedCard.ReturnToHand();
                OnCardReturnedToHand?.Invoke(_draggedCard);
            }

            _isDragging = false;
            _draggedCard = null;
            _hoveredSlot = null;
            _hoveredQuestionZone = null;
            OnDragSlotHoverChanged?.Invoke(null, null, false);
            OnDragQuestionHoverChanged?.Invoke(null, null, false);
        }

        public void SetCamera(Camera cam) { mainCamera = cam; }
    }
}
