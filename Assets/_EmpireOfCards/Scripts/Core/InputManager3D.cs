using System;
using UnityEngine;
using UnityEngine.InputSystem;
using EmpireOfCards.Data;
using EmpireOfCards.World;

namespace EmpireOfCards.Core
{
    public class InputManager3D : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float dragHeight = 1.5f;
        [SerializeField] private float hoverHeight = 0.3f;

        // State
        private Card3D _hoveredCard;
        private Card3D _draggedCard;
        private SlotZone3D _hoveredSlot;
        private Vector3 _dragOffset;
        private Vector3 _cardOriginalPos;
        private Quaternion _cardOriginalRot;
        private bool _isDragging;
        private bool _inputEnabled;

        // Events
        public event Action<Card3D> OnCardHoverEnter;
        public event Action<Card3D> OnCardHoverExit;
        public event Action<Card3D> OnCardPickedUp;
        public event Action<Card3D, SlotZone3D> OnCardDropped;
        public event Action<Card3D> OnCardReturnedToHand;

        public bool InputEnabled { get => _inputEnabled; set => _inputEnabled = value; }
        public Card3D DraggedCard => _draggedCard;

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

            // Pick up
            var mouse = Mouse.current;
            if (mouse == null) return;

            if (mouse.leftButton.wasPressedThisFrame && !_isDragging && _hoveredCard != null)
                PickUpCard(_hoveredCard);

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
                    _draggedCard.transform.position, worldPos, Time.deltaTime * 20f);
            }

            // Check slot hover - raycast all layers, check for SlotZone3D component
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                var slot = hit.collider.GetComponent<SlotZone3D>();
                if (slot == null) slot = hit.collider.GetComponentInParent<SlotZone3D>();

                if (slot != null)
                {
                    if (slot != _hoveredSlot)
                    {
                        _hoveredSlot?.SetHighlight(false);
                        _hoveredSlot = slot;
                        bool valid = _hoveredSlot.CanAccept(_draggedCard.CardData);
                        _hoveredSlot.SetHighlight(true, valid);
                    }
                }
                else if (_hoveredSlot != null)
                {
                    _hoveredSlot.SetHighlight(false);
                    _hoveredSlot = null;
                }
            }
            else if (_hoveredSlot != null)
            {
                _hoveredSlot.SetHighlight(false);
                _hoveredSlot = null;
            }
        }

        private void PickUpCard(Card3D card)
        {
            if (!card.IsInHand) return;

            var gm = GameManager.Instance;
            if (gm == null || gm.PlayerActions <= 0) return;
            if (gm.TurnManager == null || gm.TurnManager.CurrentPhase != TurnPhase.PlayPhase) return;

            _isDragging = true;
            _draggedCard = card;
            _cardOriginalPos = card.transform.position;
            _cardOriginalRot = card.transform.rotation;

            card.SetDragging(true);
            card.transform.rotation = Quaternion.Euler(0, 0, 0);

            // Highlight valid slots
            var allSlots = FindObjectsByType<SlotZone3D>(FindObjectsSortMode.None);
            foreach (var s in allSlots)
            {
                if (s.CanAccept(card.CardData))
                    s.SetHighlight(true, true);
            }

            OnCardPickedUp?.Invoke(card);
        }

        private void DropCard()
        {
            if (_draggedCard == null) return;

            // Clear all highlights
            var allSlots = FindObjectsByType<SlotZone3D>(FindObjectsSortMode.None);
            foreach (var s in allSlots)
                s.SetHighlight(false);

            if (_hoveredSlot != null && _hoveredSlot.CanAccept(_draggedCard.CardData))
            {
                // Valid drop
                OnCardDropped?.Invoke(_draggedCard, _hoveredSlot);
                _draggedCard.SetDragging(false);
            }
            else
            {
                // Return to hand
                _draggedCard.ReturnToOriginal(_cardOriginalPos, _cardOriginalRot);
                _draggedCard.SetDragging(false);
                OnCardReturnedToHand?.Invoke(_draggedCard);
            }

            _isDragging = false;
            _draggedCard = null;
            _hoveredSlot = null;
        }

        public void SetCamera(Camera cam) { mainCamera = cam; }
    }
}
