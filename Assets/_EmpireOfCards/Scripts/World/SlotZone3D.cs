using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Helpers;
using DG.Tweening;

namespace EmpireOfCards.World
{
    public class SlotZone3D : MonoBehaviour
    {
        // ── State ───────────────────────────────────────────────────
        SlotType _slotType;
        int _slotIndex;
        bool _isOccupied;
        bool _isHovered;

        Renderer _renderer;
        Material _mat;
        Tween _emptyPulseTween;

        // ── Colors ──────────────────────────────────────────────────
        static readonly Color COL_EMPTY     = new Color(0.25f, 0.25f, 0.25f);
        static readonly Color COL_OCCUPIED  = new Color(0.90f, 0.90f, 0.70f);
        static readonly Color COL_HIGHLIGHT = new Color(1f, 1f, 0.5f);

        // ── Properties ──────────────────────────────────────────────
        public SlotType SlotType  => _slotType;
        public int SlotIndex      => _slotIndex;
        public bool IsOccupied    => _isOccupied;

        // ── Init ────────────────────────────────────────────────────

        public void RuntimeInit(SlotType type, int index)
        {
            _slotType = type;
            _slotIndex = index;
            _isOccupied = false;

            transform.localScale = new Vector3(0.8f, 0.1f, 1.0f);

            _renderer = GetComponent<Renderer>();
            _mat = MaterialHelper.Create(COL_EMPTY);
            _renderer.material = _mat;
        }

        // ── Public API ──────────────────────────────────────────────

        public void SetOccupied(bool occupied)
        {
            _isOccupied = occupied;
            UpdateVisual();
        }

        public void SetHovered(bool hovered)
        {
            _isHovered = hovered;
            UpdateVisual();
        }

        // ── Visual update ───────────────────────────────────────────

        void UpdateVisual()
        {
            if (_mat == null) return;

            Color targetColor;
            if (_isHovered && !_isOccupied)
                targetColor = COL_HIGHLIGHT;
            else if (_isOccupied)
                targetColor = COL_OCCUPIED;
            else
                targetColor = COL_EMPTY;

            // Animate color transition
            _mat.DOColor(targetColor, 0.2f);

            // Slot scale animation on hover
            if (_isHovered && !_isOccupied)
            {
                DOTweenAnimations.SlotHighlight(transform, true);
            }
            else if (!_isHovered)
            {
                DOTweenAnimations.SlotHighlight(transform, false);
            }

            // Manage empty pulse
            if (_isOccupied)
            {
                _emptyPulseTween?.Kill();
                _emptyPulseTween = null;
            }
            else if (_emptyPulseTween == null && !_isHovered)
            {
                _emptyPulseTween = DOTweenAnimations.SlotEmptyPulse(transform);
            }
        }

        void OnDestroy()
        {
            _emptyPulseTween?.Kill();
        }
    }
}
