using UnityEngine;
using TMPro;
using EmpireOfCards.Data;
using EmpireOfCards.Core;

namespace EmpireOfCards.World
{
    [RequireComponent(typeof(BoxCollider))]
    public class Card3D : MonoBehaviour
    {
        // Visual references (set by factory)
        private MeshRenderer _meshRenderer;
        private TMP_Text _nameText;
        private TMP_Text _costText;
        private TMP_Text _descText;
        private TMP_Text _statsText;
        private GameObject _glowOutline;

        // Data
        private CardData _cardData;
        private bool _isInHand;
        private bool _isDragging;
        private bool _isHovered;

        // Animation
        private Vector3 _targetPos;
        private Quaternion _targetRot;
        private Vector3 _targetScale;
        private float _lerpSpeed = 12f;

        // Colors per card type
        private static readonly Color BusinessColor = new Color(0.2f, 0.4f, 0.8f);
        private static readonly Color EmployeeColor = new Color(0.2f, 0.7f, 0.3f);
        private static readonly Color ActionColor = new Color(0.85f, 0.2f, 0.2f);
        private static readonly Color UpgradeColor = new Color(0.6f, 0.2f, 0.8f);
        private static readonly Color EventColor = new Color(0.9f, 0.8f, 0.15f);

        // Properties
        public CardData CardData => _cardData;
        public bool IsInHand { get => _isInHand; set => _isInHand = value; }
        public bool IsDragging => _isDragging;

        public void Initialize(CardData data, MeshRenderer mr, TMP_Text nameT, TMP_Text costT, TMP_Text descT, TMP_Text statsT, GameObject glow)
        {
            _cardData = data;
            _meshRenderer = mr;
            _nameText = nameT;
            _costText = costT;
            _descText = descT;
            _statsText = statsT;
            _glowOutline = glow;

            _targetPos = transform.position;
            _targetRot = transform.rotation;
            _targetScale = transform.localScale;

            ApplyCardVisuals();
        }

        private void Update()
        {
            if (!_isDragging)
            {
                transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * _lerpSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRot, Time.deltaTime * _lerpSpeed);
            }
            transform.localScale = Vector3.Lerp(transform.localScale, _targetScale, Time.deltaTime * _lerpSpeed);
        }

        public void ApplyCardVisuals()
        {
            if (_cardData == null) return;

            Color cardColor = _cardData.cardType switch
            {
                CardType.Business => BusinessColor,
                CardType.Employee => EmployeeColor,
                CardType.Action => ActionColor,
                CardType.Upgrade => UpgradeColor,
                CardType.Event => EventColor,
                _ => Color.gray
            };

            if (_meshRenderer != null)
                _meshRenderer.material.color = cardColor;

            if (_nameText != null) _nameText.text = _cardData.cardName;
            if (_costText != null) _costText.text = _cardData.buyCost > 0 ? $"${_cardData.buyCost}" : "FREE";
            if (_descText != null) _descText.text = _cardData.description ?? "";

            if (_statsText != null)
            {
                _statsText.text = _cardData.cardType switch
                {
                    CardType.Business => $"${_cardData.incomePerTurn}/tur  {_cardData.customersPerTurn} musteri",
                    CardType.Employee => $"Maas: ${_cardData.salaryPerTurn}/tur",
                    CardType.Action => _cardData.actionEffectType.ToString(),
                    CardType.Upgrade => _cardData.upgradeEffectType.ToString(),
                    CardType.Event => $"{_cardData.eventDuration} tur",
                    _ => ""
                };
            }
        }

        public void SetHovered(bool hovered)
        {
            _isHovered = hovered;
            _targetScale = hovered ? Vector3.one * 1.15f : Vector3.one;
            if (_glowOutline != null) _glowOutline.SetActive(hovered);
        }

        public void SetDragging(bool dragging)
        {
            _isDragging = dragging;
            _targetScale = dragging ? Vector3.one * 1.2f : Vector3.one;

            var col = GetComponent<Collider>();
            if (col != null) col.enabled = !dragging;
        }

        public void SetTargetPosition(Vector3 pos, Quaternion rot)
        {
            _targetPos = pos;
            _targetRot = rot;
        }

        public void ReturnToOriginal(Vector3 pos, Quaternion rot)
        {
            _targetPos = pos;
            _targetRot = rot;
            _isDragging = false;
        }

        public void SetHandPosition(Vector3 pos, Quaternion rot)
        {
            _targetPos = pos;
            _targetRot = rot;
            _isInHand = true;
        }
    }
}
