using UnityEngine;
using TMPro;
using DG.Tweening;
using EmpireOfCards.Data;
using EmpireOfCards.Core;
using EmpireOfCards.Presentation;
using EmpireOfCards.UI.Clarity;
using EmpireOfCards.Gameplay;

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
        private Vector3 _baseScale = Vector3.one;
        private float _lerpSpeed = 12f;
        private bool _isSnapping; // True while DOTween snap animation is running

        // Colors per card type
        private static readonly Color BusinessColor = ControlDeskTheme.OperationSlot;
        private static readonly Color EmployeeColor = ControlDeskTheme.StaffSlot;
        private static readonly Color ActionColor = ControlDeskTheme.ActionSlot;
        private static readonly Color UpgradeColor = ControlDeskTheme.SupplierSlot;
        private static readonly Color EventColor = ControlDeskTheme.EventSlot;

        // Properties
        public CardData CardData => _cardData;
        public bool IsInHand { get => _isInHand; set => _isInHand = value; }
        public bool IsDragging => _isDragging;

        /// <summary>
        /// Returns the display color for a given card type.
        /// Used by Board3D for employee marker cubes.
        /// </summary>
        public static Color GetCardTypeColor(CardType type)
        {
            return type switch
            {
                CardType.Business => BusinessColor,
                CardType.Employee => EmployeeColor,
                CardType.Action   => ActionColor,
                CardType.Upgrade  => UpgradeColor,
                CardType.Event    => EventColor,
                _                 => Color.gray
            };
        }

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
            _baseScale = transform.localScale;
            _targetScale = _baseScale;

            ApplyCardVisuals();
        }

        private void Update()
        {
            if (!_isDragging && !_isSnapping)
            {
                transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * _lerpSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, _targetRot, Time.deltaTime * _lerpSpeed);
            }
            if (!_isSnapping)
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
                _ => ControlDeskTheme.PanelLine
            };

            if (_meshRenderer != null)
                _meshRenderer.material.color = Color.Lerp(ControlDeskTheme.PanelSoft, cardColor, 0.45f);

            if (_nameText != null) _nameText.text = _cardData.cardName;
            if (_costText != null) _costText.text = BuildCostChip(_cardData);
            if (_descText != null) _descText.text = GameClarityFormatter.BuildCardFrontSummary(_cardData);

            if (_statsText != null)
                _statsText.text = $"{GameClarityFormatter.BuildProjectedDeltaLine(_cardData)}\n{GameClarityFormatter.BuildCostSummary(_cardData)}";
        }

        public void SetHovered(bool hovered)
        {
            _isHovered = hovered;
            _targetScale = hovered ? _baseScale * 1.18f : _baseScale;
            if (_glowOutline != null) _glowOutline.SetActive(hovered);
        }

        public void SetDragging(bool dragging)
        {
            _isDragging = dragging;
            _targetScale = dragging ? _baseScale * 1.26f : (_isHovered ? _baseScale * 1.18f : _baseScale);

            var col = GetComponent<Collider>();
            if (col != null) col.enabled = !dragging;
        }

        public void SetTargetPosition(Vector3 pos, Quaternion rot)
        {
            _targetPos = pos;
            _targetRot = rot;
        }

        /// <summary>
        /// Sets the target scale for a card placed on the board.
        /// The card lerps smoothly to this scale in Update().
        /// </summary>
        public void SetBoardScale(float uniformScale)
        {
            _baseScale = Vector3.one * uniformScale;
            _targetScale = _isHovered ? _baseScale * 1.18f : _baseScale;
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
            _baseScale = Vector3.one * 1.22f;
            _targetScale = _isHovered ? _baseScale * 1.18f : _baseScale;
        }

        /// <summary>
        /// Called when a card drop fails and the card needs to animate back
        /// to its previous hand position. Uses snap-overshoot for crisp feedback.
        /// </summary>
        public void ReturnToHand()
        {
            _isDragging = false;
            _isInHand = true;
            SnapToPosition(_targetPos, _targetRot);
        }

        /// <summary>
        /// Snaps card to target with overshoot bounce and scale punch via DOTween.
        /// Used for slot placement and return-to-hand to give crisp, juicy feedback.
        /// </summary>
        public void SnapToPosition(Vector3 target, Quaternion rotation)
        {
            if (this == null || gameObject == null) return;

            _isSnapping = true;
            _targetPos = target;
            _targetRot = rotation;

            // Kill any existing tweens on this transform to avoid conflicts
            transform.DOKill();

            // Move with OutBack ease (overshoots ~5% then settles)
            transform.DOMove(target, 0.12f).SetEase(Ease.OutBack).SetLink(gameObject);
            transform.DORotateQuaternion(rotation, 0.12f).SetEase(Ease.OutBack).SetLink(gameObject);

            // Scale punch: briefly inflate to 1.08x then return to 1.0x
            transform.DOPunchScale(Vector3.one * 0.08f, 0.15f, 6, 0.5f)
                .SetLink(gameObject)
                .OnComplete(() =>
                {
                    if (this != null)
                    {
                        _isSnapping = false;
                        transform.localScale = _targetScale;
                    }
                });
        }

        private static string BuildCostChip(CardData card)
        {
            if (card == null)
                return string.Empty;

            string buy = $"BUY {Mathf.Max(0, card.buyCost)}";
            string play = card.playCost > 0 ? $" / PLAY {card.playCost}" : string.Empty;
            return $"{BuildRoleChip(card)} | {buy}{play}";
        }

        private static string BuildRoleChip(CardData card)
        {
            if (card == null)
                return "CARD";

            if (card.cardFamily == CardFamily.Risk || card.cardFamily == CardFamily.Crisis || card.legalRiskOnPlay > 0 || card.legalRiskPerTurn > 0 || card.legalRiskDeltaPerTurn > 0f)
                return "RISK";
            if (QuestionTagUtility.IsPersistentBuild(card))
                return "BUILD";
            if (QuestionTagUtility.IsResponseCard(card))
                return "RESPONSE";

            return "CARD";
        }
    }
}
