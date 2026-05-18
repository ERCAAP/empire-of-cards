using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI.Cards
{
    /// <summary>
    /// Visual representation of a single card. Handles setup, highlight,
    /// and synergy display.
    /// </summary>
    public class CardUI : MonoBehaviour
    {
        [Header("Card Visuals")]
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardIcon;

        [Header("Text Fields")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statsText;

        [Header("State")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Highlight")]
        [SerializeField] private GameObject highlightBorder;
        [SerializeField] private GameObject synergyGlow;
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private Color synergyColor = Color.cyan;

        private CardData data;

        /// <summary>
        /// Returns the CardData bound to this card visual.
        /// </summary>
        public CardData GetCardData()
        {
            return data;
        }

        /// <summary>
        /// Populates the card UI with data from a CardData ScriptableObject.
        /// </summary>
        public void SetupCard(CardData cardData)
        {
            data = cardData;

            if (data == null)
                return;

            if (nameText != null)
                nameText.text = data.cardName;

            if (costText != null)
                costText.text = $"${data.buyCost}";

            if (descriptionText != null)
                descriptionText.text = data.description;

            if (cardIcon != null && data.icon != null)
                cardIcon.sprite = data.icon;

            if (cardBackground != null && data.cardFrame != null)
                cardBackground.sprite = data.cardFrame;

            UpdateStatsText();
        }

        /// <summary>
        /// Enables or disables interaction (dimming the card when not interactable).
        /// </summary>
        public void SetInteractable(bool value)
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = value;
                canvasGroup.blocksRaycasts = value;
                canvasGroup.alpha = value ? 1f : 0.5f;
            }
        }

        /// <summary>
        /// Toggles the highlight border around the card.
        /// </summary>
        public void ShowHighlight(bool show)
        {
            if (highlightBorder != null)
            {
                highlightBorder.SetActive(show);
            }
        }

        /// <summary>
        /// Toggles the synergy glow effect on the card.
        /// </summary>
        public void ShowSynergy(bool show)
        {
            if (synergyGlow != null)
            {
                synergyGlow.SetActive(show);
            }
        }

        private void UpdateStatsText()
        {
            if (statsText == null || data == null)
                return;

            switch (data.cardType)
            {
                case Core.CardType.Business:
                    statsText.text = $"+${data.incomePerTurn}/turn  +{data.customersPerTurn} cust";
                    break;
                case Core.CardType.Employee:
                    statsText.text = $"-${data.salaryPerTurn}/turn  +{data.customerBonus} cust";
                    break;
                case Core.CardType.Action:
                    statsText.text = data.actionValue != 0
                        ? $"Effect: {data.actionValue}"
                        : $"x{data.actionMultiplier}";
                    break;
                case Core.CardType.Upgrade:
                    statsText.text = $"+{data.upgradeValue * 100f:F0}%";
                    break;
                case Core.CardType.Event:
                    statsText.text = $"{data.eventDuration} turn(s)";
                    break;
                default:
                    statsText.text = string.Empty;
                    break;
            }
        }
    }
}
