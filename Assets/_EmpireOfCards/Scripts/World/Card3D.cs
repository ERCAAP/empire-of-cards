using UnityEngine;
using TMPro;
using DG.Tweening;
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
        private bool _isSnapping; // True while DOTween snap animation is running

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
            _targetScale = transform.localScale;

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
                    CardType.Business => $"${_cardData.incomePerTurn}/turn  {_cardData.customersPerTurn} cust.",
                    CardType.Employee => $"Salary: ${_cardData.salaryPerTurn}/turn",
                    CardType.Action => GetActionLabel(_cardData.actionEffectType),
                    CardType.Upgrade => GetUpgradeLabel(_cardData.upgradeEffectType),
                    CardType.Event => _cardData.eventDuration > 1
                        ? $"Lasts {_cardData.eventDuration} turns"
                        : "Lasts 1 turn",
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

        /// <summary>
        /// Sets the target scale for a card placed on the board.
        /// The card lerps smoothly to this scale in Update().
        /// </summary>
        public void SetBoardScale(float uniformScale)
        {
            _targetScale = Vector3.one * uniformScale;
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

        /// <summary>
        /// Returns a player-friendly label for action effect types.
        /// </summary>
        private static string GetActionLabel(ActionEffectType effect)
        {
            return effect switch
            {
                ActionEffectType.AddCustomersToRandom   => "Instant: +Customers",
                ActionEffectType.AddMoneyInstant        => "Instant: +Money",
                ActionEffectType.MultiplyAllCustomers   => "Instant: x2 Customers",
                ActionEffectType.CloseRivalWeakestBusiness => "Attack: Close Rival",
                ActionEffectType.AddCustomersWithFBI    => "Risky: +Customers",
                ActionEffectType.StealCustomersHalfIncome => "Trade: Steal Cust.",
                ActionEffectType.DisableRivalOneTurn    => "Sabotage: Stun Rival",
                ActionEffectType.MoneyNowPayLater       => "Loan: +Money Now",
                ActionEffectType.DrawAndPlayEmployee    => "Instant: Free Hire",
                ActionEffectType.SacrificeBusiness      => "Sell: 2x Value",
                _                                       => "Action"
            };
        }

        /// <summary>
        /// Returns a player-friendly label for upgrade effect types.
        /// </summary>
        private static string GetUpgradeLabel(UpgradeEffectType effect)
        {
            return effect switch
            {
                UpgradeEffectType.IncomePercentSingle         => "Boost: +10% Income",
                UpgradeEffectType.IncomePercentWithSlotLoss   => "Boost: +30%, -1 Slot",
                UpgradeEffectType.GlobalCustomerPerTurn        => "All: +Cust./Turn",
                UpgradeEffectType.GlobalCustomerFlat           => "All: +Customers",
                UpgradeEffectType.ReduceFBIRisk                => "Safety: -25% FBI",
                UpgradeEffectType.ExtraAction                  => "Bonus: +1 Action",
                _                                              => "Upgrade"
            };
        }
    }
}
