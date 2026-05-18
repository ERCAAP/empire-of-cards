using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.World
{
    [RequireComponent(typeof(BoxCollider))]
    public class SlotZone3D : MonoBehaviour
    {
        [SerializeField] private DropZoneType zoneType;
        [SerializeField] private int slotIndex;
        [SerializeField] private int parentBusinessIndex = -1;

        private MeshRenderer _renderer;
        private Color _baseColor;
        private bool _isOccupied;
        private Card3D _placedCard;

        public DropZoneType ZoneType => zoneType;
        public int SlotIndex => slotIndex;
        public int ParentBusinessIndex => parentBusinessIndex;
        public bool IsOccupied => _isOccupied;
        public Card3D PlacedCard => _placedCard;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            if (_renderer != null) _baseColor = _renderer.material.color;
        }

        public bool CanAccept(CardData card)
        {
            if (card == null || _isOccupied) return false;

            return zoneType switch
            {
                DropZoneType.BusinessSlot => card.cardType == CardType.Business,
                DropZoneType.EmployeeSlot => card.cardType == CardType.Employee,
                DropZoneType.UpgradeSlot => card.cardType == CardType.Upgrade,
                DropZoneType.ActionZone => card.cardType == CardType.Action,
                DropZoneType.SellZone => true,
                DropZoneType.BurnZone => true,
                _ => false
            };
        }

        public void PlaceCard(Card3D card)
        {
            _isOccupied = true;
            _placedCard = card;
            card.IsInHand = false;
            card.SetTargetPosition(transform.position + Vector3.up * 0.15f, Quaternion.identity);
        }

        public void RemoveCard()
        {
            _isOccupied = false;
            _placedCard = null;
        }

        public void SetHighlight(bool on, bool valid = true)
        {
            if (_renderer == null) return;

            if (on)
                _renderer.material.color = valid
                    ? new Color(0.2f, 0.9f, 0.3f, 1f)
                    : new Color(0.9f, 0.2f, 0.2f, 1f);
            else
                _renderer.material.color = _baseColor;
        }

        public void RuntimeInit(DropZoneType type, int slot, int parentBiz = -1)
        {
            zoneType = type;
            slotIndex = slot;
            parentBusinessIndex = parentBiz;
        }
    }
}
