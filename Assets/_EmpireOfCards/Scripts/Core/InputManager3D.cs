using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.World;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Core
{
    public class InputManager3D : MonoBehaviour
    {
        // ── References (set via Init) ───────────────────────────────
        Hand3D _hand;
        Board3D _board;
        BoardManager _boardManager;
        Camera _cam;

        // ── Drag state ──────────────────────────────────────────────
        Card3D _dragCard;
        Vector3 _dragOffset;
        Vector3 _dragStartPos;
        Quaternion _dragStartRot;
        float _dragPlaneY = 0.5f;

        // ── Phase gate ──────────────────────────────────────────────
        bool _isPlayPhase;

        // ── Redraw tracking ─────────────────────────────────────────
        int _redrawsUsed;
        const int REDRAW_MAX = Constants.REDRAW_PER_TURN;

        // ── Hover tracking ──────────────────────────────────────────
        SlotZone3D _lastHoveredSlot;

        // ── Init ────────────────────────────────────────────────────

        public void Init(Hand3D hand, Board3D board, BoardManager boardManager)
        {
            _hand = hand;
            _board = board;
            _boardManager = boardManager;
            _cam = Camera.main;
        }

        // ── EventBus subscriptions ──────────────────────────────────

        void OnEnable()
        {
            EventBus.OnPhaseStarted += HandlePhaseStarted;
            EventBus.OnPhaseEnded += HandlePhaseEnded;
            EventBus.OnTurnStarted += HandleTurnStarted;
        }

        void OnDisable()
        {
            EventBus.OnPhaseStarted -= HandlePhaseStarted;
            EventBus.OnPhaseEnded -= HandlePhaseEnded;
            EventBus.OnTurnStarted -= HandleTurnStarted;
        }

        void HandlePhaseStarted(TurnPhase phase)
        {
            _isPlayPhase = phase == TurnPhase.PlayPhase;
        }

        void HandlePhaseEnded(TurnPhase phase)
        {
            if (phase == TurnPhase.PlayPhase)
            {
                _isPlayPhase = false;
                CancelDrag();
            }
        }

        void HandleTurnStarted(int turn)
        {
            _redrawsUsed = 0;
        }

        // ── Update loop ─────────────────────────────────────────────

        void Update()
        {
            if (!_isPlayPhase || _cam == null) return;

            HandleHover();

            if (Input.GetMouseButtonDown(0))
                TryStartDrag();

            if (Input.GetMouseButton(0) && _dragCard != null)
                UpdateDrag();

            if (Input.GetMouseButtonUp(0) && _dragCard != null)
                EndDrag();

            if (Input.GetMouseButtonDown(1))
                TryRedraw();
        }

        // ── Hover (card highlighting) ───────────────────────────────

        void HandleHover()
        {
            if (_dragCard != null) return;

            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                var card = hit.collider.GetComponent<Card3D>();
                if (card != null && card.State == Card3DState.InHand)
                {
                    _hand.SetHovered(card);
                    card.SetGlow(true);
                    return;
                }

                // Hover on slot zones during drag
                var slot = hit.collider.GetComponent<SlotZone3D>();
                if (slot != null && slot != _lastHoveredSlot)
                {
                    if (_lastHoveredSlot != null) _lastHoveredSlot.SetHovered(false);
                    slot.SetHovered(true);
                    _lastHoveredSlot = slot;
                    return;
                }
            }

            // Nothing hovered
            _hand.ClearHover();
            if (_lastHoveredSlot != null)
            {
                _lastHoveredSlot.SetHovered(false);
                _lastHoveredSlot = null;
            }
        }

        // ── Drag start ──────────────────────────────────────────────

        void TryStartDrag()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;

            var card = hit.collider.GetComponent<Card3D>();
            if (card == null || card.State != Card3DState.InHand) return;

            _dragCard = card;
            _dragStartPos = card.transform.position;
            _dragStartRot = card.transform.rotation;
            _dragCard.SetState(Card3DState.Dragging);
            _dragCard.SetGlow(true);

            _hand.RemoveCard(_dragCard);
        }

        // ── Drag update ─────────────────────────────────────────────

        void UpdateDrag()
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, new Vector3(0f, _dragPlaneY, 0f));

            if (plane.Raycast(ray, out float dist))
            {
                Vector3 point = ray.GetPoint(dist);
                _dragCard.transform.position = point;
                _dragCard.transform.rotation = Quaternion.identity;
            }

            // Highlight nearest slot
            if (_dragCard.Data != null)
            {
                var slot = _board.FindNearestEmptySlot(_dragCard.Data.targetSlot, _dragCard.transform.position);
                if (slot != _lastHoveredSlot)
                {
                    if (_lastHoveredSlot != null) _lastHoveredSlot.SetHovered(false);
                    if (slot != null) slot.SetHovered(true);
                    _lastHoveredSlot = slot;
                }
            }
        }

        // ── Drag end (place or return) ──────────────────────────────

        void EndDrag()
        {
            if (_dragCard == null) return;

            // Clear slot hover
            if (_lastHoveredSlot != null)
            {
                _lastHoveredSlot.SetHovered(false);
                _lastHoveredSlot = null;
            }

            // Check if over a valid slot
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            bool placed = false;

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                var slotZone = hit.collider.GetComponent<SlotZone3D>();
                if (slotZone != null && !slotZone.IsOccupied && _dragCard.Data != null)
                {
                    if (slotZone.SlotType == _dragCard.Data.targetSlot)
                    {
                        // Check player has actions
                        var gm = GameManager.Instance;
                        if (gm != null && gm.Resources.HasActions())
                        {
                            // Check money
                            if (gm.Resources.GetMoney() >= _dragCard.Data.buyCost)
                            {
                                bool boardOk = _boardManager.PlaceCard(_dragCard.Data, slotZone.SlotType);
                                if (boardOk)
                                {
                                    gm.Resources.UseAction();
                                    gm.Resources.AdjustMoney(-_dragCard.Data.buyCost);
                                    slotZone.SetOccupied(true);

                                    _dragCard.SetState(Card3DState.OnBoard);
                                    _dragCard.transform.position = slotZone.transform.position + Vector3.up * 0.15f;
                                    _dragCard.transform.rotation = Quaternion.identity;
                                    _dragCard.SetGlow(false);
                                    placed = true;
                                }
                            }
                        }
                    }
                }
            }

            if (!placed)
                ReturnCardToHand();

            _dragCard = null;
        }

        // ── Cancel / return ─────────────────────────────────────────

        void CancelDrag()
        {
            if (_dragCard == null) return;
            ReturnCardToHand();
            _dragCard = null;
        }

        void ReturnCardToHand()
        {
            _dragCard.SetState(Card3DState.InHand);
            _dragCard.SetGlow(false);
            _dragCard.transform.position = _dragStartPos;
            _dragCard.transform.rotation = _dragStartRot;
            _hand.AddCard(_dragCard);
        }

        // ── Redraw (right-click) ────────────────────────────────────

        void TryRedraw()
        {
            if (_redrawsUsed >= REDRAW_MAX) return;

            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;

            var card = hit.collider.GetComponent<Card3D>();
            if (card == null || card.State != Card3DState.InHand) return;

            _hand.RemoveCard(card);
            EventBus.CardDiscarded(card.Data);
            _redrawsUsed++;

            Debug.Log($"[InputManager3D] Redraw used ({_redrawsUsed}/{REDRAW_MAX})");
        }
    }
}
