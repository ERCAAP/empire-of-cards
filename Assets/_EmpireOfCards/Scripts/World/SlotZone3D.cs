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
        private Color _occupiedColor;
        private bool _isOccupied;
        private Card3D _placedCard;
        private bool _isPulsing;
        private Color _pulseColor;
        private Color _validHighlightColor = new Color(0.2f, 0.9f, 0.3f, 1f);
        private Color _invalidHighlightColor = new Color(0.9f, 0.2f, 0.2f, 1f);
        private GameObject _buildingVisual;

        public DropZoneType ZoneType => zoneType;
        public int SlotIndex => slotIndex;
        public int ParentBusinessIndex => parentBusinessIndex;
        public bool IsOccupied => _isOccupied;
        public Card3D PlacedCard => _placedCard;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            if (_renderer != null)
            {
                _baseColor = _renderer.material.color;
                _occupiedColor = _baseColor;
            }
            _pulseColor = new Color(0.2f, 0.9f, 0.3f, 0.5f);
        }

        private void Update()
        {
            if (!_isPulsing || _renderer == null) return;

            // Oscillate alpha between 0.3 and 0.7 at 1.5 Hz
            float alpha = 0.5f + 0.2f * Mathf.Sin(Time.time * 1.5f * 2f * Mathf.PI);
            Color c = _pulseColor;
            c.a = alpha;
            _renderer.material.color = c;
        }

        /// <summary>
        /// Enables a green pulsing glow on valid drop zones while a card is being dragged.
        /// </summary>
        public void SetPulse(bool on)
        {
            _isPulsing = on;
            if (!on && _renderer != null)
                _renderer.material.color = _isOccupied ? _occupiedColor : _baseColor;
            else if (on)
                _pulseColor = new Color(_pulseColor.r, _pulseColor.g, _pulseColor.b, 0.55f);
        }

        public void ConfigureVisuals(Color baseColor, Color occupiedColor, Color pulseColor, Color validHighlightColor, Color invalidHighlightColor)
        {
            _baseColor = baseColor;
            _occupiedColor = occupiedColor;
            _pulseColor = pulseColor;
            _validHighlightColor = validHighlightColor;
            _invalidHighlightColor = invalidHighlightColor;

            if (_renderer != null)
                _renderer.material.color = _isOccupied ? _occupiedColor : _baseColor;
        }

        public bool CanAccept(CardData card)
        {
            if (card == null || _isOccupied) return false;

            switch (zoneType)
            {
                case DropZoneType.BusinessSlot:
                    return card.cardType == CardType.Business && !_isOccupied;

                case DropZoneType.EmployeeSlot:
                    if (card.cardType != CardType.Employee || _isOccupied) return false;
                    // Only accept if parent business slot has a business placed
                    var gm = GameManager.Instance;
                    if (gm == null || gm.BoardManager == null) return false;
                    var businesses = gm.BoardManager.PlayerBusinesses;
                    return parentBusinessIndex >= 0 && parentBusinessIndex < businesses.Count
                           && businesses[parentBusinessIndex] != null
                           && !businesses[parentBusinessIndex].isClosed;

                case DropZoneType.UpgradeSlot:
                    return card.cardType == CardType.Upgrade && !_isOccupied;

                case DropZoneType.ActionZone:
                    return card.cardType == CardType.Action;

                case DropZoneType.SellZone:
                case DropZoneType.BurnZone:
                    return true;

                // === Slot System v2 (GDD v3.0 Section 4) ===
                // Uses targetSlotType from CardData v2 when available, falls back to CardType check
                case DropZoneType.OperationSlot:
                    if (card.targetSlotType == SlotType.Operation) return true;
                    return card.cardType == CardType.Business;

                case DropZoneType.StaffSlot:
                    if (card.targetSlotType == SlotType.Staff) return true;
                    return card.cardType == CardType.Employee;

                case DropZoneType.MarketingSlot:
                    if (card.targetSlotType == SlotType.Marketing) return true;
                    return card.cardType == CardType.Action
                           || (card.tags != null && System.Array.IndexOf(card.tags, CardTag.Marketing) >= 0);

                case DropZoneType.SupplierSlot:
                    if (card.targetSlotType == SlotType.Supplier) return true;
                    return card.cardType == CardType.Upgrade;

                case DropZoneType.TempEffectSlot:
                    if (card.targetSlotType == SlotType.TempEffect) return true;
                    return card.cardType == CardType.Event;

                default:
                    return false;
            }
        }

        public void PlaceCard(Card3D card)
        {
            _isOccupied = true;
            _placedCard = card;
            card.IsInHand = false;

            // Position card above slot and snap for juicy feedback
            Vector3 target = transform.position + Vector3.up * 0.15f;
            card.SnapToPosition(target, Quaternion.identity);

            // Scale card to fit slot
            float scale = zoneType switch
            {
                DropZoneType.EmployeeSlot   => 0.5f,
                DropZoneType.StaffSlot      => 0.5f,
                DropZoneType.UpgradeSlot    => 0.6f,
                DropZoneType.SupplierSlot   => 0.6f,
                DropZoneType.ActionZone     => 0.7f,
                DropZoneType.MarketingSlot  => 0.7f,
                DropZoneType.SellZone       => 0.7f,
                DropZoneType.BurnZone       => 0.7f,
                DropZoneType.TempEffectSlot => 0.65f,
                _                           => 0.8f   // OperationSlot / BusinessSlot
            };
            card.SetBoardScale(scale);

            // Spawn a visual building placeholder based on slot type
            SpawnBuildingVisual(card);

            if (_renderer != null)
                _renderer.material.color = _occupiedColor;
        }

        public void RemoveCard()
        {
            _isOccupied = false;
            _placedCard = null;

            if (_buildingVisual != null)
            {
                Destroy(_buildingVisual);
                _buildingVisual = null;
            }

            if (_renderer != null)
                _renderer.material.color = _baseColor;
        }

        private void SpawnBuildingVisual(Card3D card)
        {
            if (_buildingVisual != null)
            {
                Destroy(_buildingVisual);
                _buildingVisual = null;
            }

            if (card == null || card.CardData == null) return;
            CardData data = card.CardData;

            var building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.name = $"Building_{data.cardId}";
            building.transform.SetParent(transform);
            building.transform.localPosition = new Vector3(0f, 0.3f, 0f);

            // Use CardData visual settings (ScriptableObject driven)
            building.transform.localScale = data.buildingScale;
            building.GetComponent<MeshRenderer>().material.color = data.buildingColor;

            // Optional label on building
            if (!string.IsNullOrEmpty(data.buildingLabel))
            {
                var labelGo = new GameObject("BuildingLabel");
                labelGo.transform.SetParent(building.transform);
                labelGo.transform.localPosition = new Vector3(0f, 0.8f, 0f);
                labelGo.transform.localScale = Vector3.one * 0.5f;
                var tmp = labelGo.AddComponent<TMPro.TextMeshPro>();
                tmp.text = data.buildingLabel;
                tmp.fontSize = 3f;
                tmp.alignment = TMPro.TextAlignmentOptions.Center;
                tmp.color = Color.white;
            }

            Destroy(building.GetComponent<Collider>());
            _buildingVisual = building;
        }

        public void SetHighlight(bool on, bool valid = true)
        {
            if (_renderer == null) return;

            if (on)
            {
                // Direct hover highlight overrides pulse temporarily
                _isPulsing = false;
                _renderer.material.color = valid ? _validHighlightColor : _invalidHighlightColor;
            }
            else
            {
                _renderer.material.color = _isOccupied ? _occupiedColor : _baseColor;
            }
        }

        public void RuntimeInit(DropZoneType type, int slot, int parentBiz = -1)
        {
            zoneType = type;
            slotIndex = slot;
            parentBusinessIndex = parentBiz;
        }
    }
}
