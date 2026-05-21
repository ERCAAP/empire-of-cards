using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.UI.Cards;
using TMPro;
using EmpireOfCards.Presentation;

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
        private TextMeshPro _previewLabel;
        private bool _placementFlashActive;
        private float _placementFlashTimer;
        private const float PlacementFlashDuration = 0.22f;

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
            CreatePreviewLabel();
        }

        private void Update()
        {
            if (_placementFlashActive && _renderer != null)
            {
                _placementFlashTimer -= Time.deltaTime;
                float t = Mathf.Clamp01(_placementFlashTimer / PlacementFlashDuration);
                _renderer.material.color = Color.Lerp(_occupiedColor, ControlDeskTheme.Lighten(_occupiedColor, 0.35f), t);
                if (_placementFlashTimer <= 0f)
                {
                    _placementFlashActive = false;
                    _renderer.material.color = _isOccupied ? _occupiedColor : _baseColor;
                }
                return;
            }

            if (!_isPulsing || _renderer == null) return;

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

            TriggerPlacementFlash();
        }

        public void RemoveCard()
        {
            _isOccupied = false;
            _placedCard = null;
            ClearPreview();

            if (_buildingVisual != null)
            {
                Destroy(_buildingVisual);
                _buildingVisual = null;
            }

            if (_renderer != null)
                _renderer.material.color = _baseColor;
        }

        public void ApplyRestoredCard(CardData data)
        {
            _isOccupied = data != null;
            _placedCard = null;
            ClearPreview();

            if (_buildingVisual != null)
            {
                Destroy(_buildingVisual);
                _buildingVisual = null;
            }

            if (data != null)
                SpawnBuildingVisual(data);

            if (_renderer != null)
                _renderer.material.color = _isOccupied ? _occupiedColor : _baseColor;
        }

        private void SpawnBuildingVisual(Card3D card)
        {
            if (_buildingVisual != null)
            {
                Destroy(_buildingVisual);
                _buildingVisual = null;
            }

            if (card == null || card.CardData == null) return;
            SpawnBuildingVisual(card.CardData);
        }

        private void SpawnBuildingVisual(CardData data)
        {
            if (_buildingVisual != null)
            {
                Destroy(_buildingVisual);
                _buildingVisual = null;
            }

            if (data == null) return;

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

        public void ShowPreview(string text, bool valid)
        {
            if (_previewLabel == null)
                return;

            _previewLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(text));
            if (!_previewLabel.gameObject.activeSelf)
                return;

            _previewLabel.text = text;
            if (!valid)
                _previewLabel.color = new Color(1f, 0.58f, 0.58f);
            else if (text != null && text.Contains("Risk +"))
                _previewLabel.color = new Color(1f, 0.84f, 0.52f);
            else
                _previewLabel.color = new Color(0.62f, 1f, 0.72f);
        }

        public void ClearPreview()
        {
            if (_previewLabel != null)
                _previewLabel.gameObject.SetActive(false);
        }

        public void RuntimeInit(DropZoneType type, int slot, int parentBiz = -1)
        {
            zoneType = type;
            slotIndex = slot;
            parentBusinessIndex = parentBiz;
        }

        private void CreatePreviewLabel()
        {
            var go = new GameObject("PreviewLabel");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            go.transform.localScale = Vector3.one * 0.055f;

            _previewLabel = go.AddComponent<TextMeshPro>();
            _previewLabel.fontSize = 4.8f;
            _previewLabel.alignment = TextAlignmentOptions.Center;
            _previewLabel.textWrappingMode = TextWrappingModes.Normal;
            _previewLabel.overflowMode = TextOverflowModes.Overflow;
            _previewLabel.outlineWidth = 0.15f;
            _previewLabel.outlineColor = new Color(0f, 0f, 0f, 0.7f);
            _previewLabel.gameObject.SetActive(false);
        }

        private void TriggerPlacementFlash()
        {
            _placementFlashActive = true;
            _placementFlashTimer = PlacementFlashDuration;
        }
    }
}
