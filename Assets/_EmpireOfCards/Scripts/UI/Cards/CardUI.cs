using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI.Cards
{
    /// <summary>
    /// Single card visual. Populates UI elements from CardData.
    /// Frame color reflects card type: Blue=Business, Green=Employee,
    /// Red=Action, Purple=Upgrade, Yellow=Event.
    /// </summary>
    public class CardUI : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardIcon;
        [SerializeField] private Image highlightBorder;

        [Header("Text Fields")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text statsText;

        [Header("State")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Frame Colors")]
        [SerializeField] private Color businessColor = new Color(0.2f, 0.5f, 1f);    // Blue
        [SerializeField] private Color employeeColor = new Color(0.2f, 0.8f, 0.3f);  // Green
        [SerializeField] private Color actionColor = new Color(1f, 0.25f, 0.25f);     // Red
        [SerializeField] private Color upgradeColor = new Color(0.65f, 0.2f, 0.85f);  // Purple
        [SerializeField] private Color eventColor = new Color(1f, 0.85f, 0.15f);      // Yellow

        [Header("Highlight")]
        [SerializeField] private Color validDropHighlight = new Color(0f, 1f, 0.3f, 0.6f);
        [SerializeField] private Color invalidDropHighlight = new Color(1f, 0f, 0f, 0f);

        [Header("Tooltip")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private TMP_Text tooltipText;

        [Header("Synergy")]
        [SerializeField] private GameObject synergyGlow;

        // Runtime
        private CardData data;
        private bool interactable;

        // --- Properties ---
        public CardData Data => data;
        public bool IsInteractable => interactable;

        // ------------------------------------------------------------------
        // Setup
        // ------------------------------------------------------------------

        /// <summary>
        /// Populates all UI elements from the supplied CardData.
        /// </summary>
        public void SetupCard(CardData cardData)
        {
            data = cardData;

            if (data == null)
                return;

            // Name and description
            if (nameText != null)
                nameText.text = data.cardName;

            if (descriptionText != null)
                descriptionText.text = data.description;

            if (costText != null)
                costText.text = data.buyCost > 0 ? $"${data.buyCost}" : "";

            // Icon
            if (cardIcon != null && data.icon != null)
                cardIcon.sprite = data.icon;

            // Frame: use the card's custom frame sprite if available,
            // then tint by card type
            if (cardBackground != null)
            {
                if (data.cardFrame != null)
                    cardBackground.sprite = data.cardFrame;

                cardBackground.color = GetFrameColor(data.cardType);
            }

            // Stats line based on card type
            if (statsText != null)
                statsText.text = BuildStatsLine(data);

            // Hide highlight and tooltip by default
            SetHighlight(false);
            HideTooltip();
            ShowSynergy(false);
        }

        // ------------------------------------------------------------------
        // Highlight for valid drop targets
        // ------------------------------------------------------------------

        /// <summary>
        /// Shows or hides the green valid-drop border.
        /// </summary>
        public void SetHighlight(bool valid)
        {
            if (highlightBorder == null)
                return;

            highlightBorder.color = valid ? validDropHighlight : invalidDropHighlight;
        }

        // ------------------------------------------------------------------
        // Synergy glow
        // ------------------------------------------------------------------

        /// <summary>
        /// Toggles the synergy glow effect on the card.
        /// </summary>
        public void ShowSynergy(bool show)
        {
            if (synergyGlow != null)
                synergyGlow.SetActive(show);
        }

        // ------------------------------------------------------------------
        // Tooltip
        // ------------------------------------------------------------------

        /// <summary>
        /// Displays hover-info tooltip with extended card details.
        /// </summary>
        public void ShowTooltip()
        {
            if (tooltipPanel == null || data == null)
                return;

            tooltipPanel.SetActive(true);

            if (tooltipText != null)
            {
                string tip = $"<b>{data.cardName}</b>\n{data.description}";

                if (data.tags != null && data.tags.Length > 0)
                {
                    tip += "\n<i>";
                    for (int i = 0; i < data.tags.Length; i++)
                    {
                        if (i > 0) tip += ", ";
                        tip += data.tags[i].ToString();
                    }
                    tip += "</i>";
                }

                tooltipText.text = tip;
            }
        }

        /// <summary>
        /// Hides the tooltip panel.
        /// </summary>
        public void HideTooltip()
        {
            if (tooltipPanel != null)
                tooltipPanel.SetActive(false);
        }

        // ------------------------------------------------------------------
        // Interactability
        // ------------------------------------------------------------------

        /// <summary>
        /// Enables or disables interaction (drag, hover) and dims the card.
        /// </summary>
        public void SetInteractable(bool value)
        {
            interactable = value;

            if (canvasGroup != null)
            {
                canvasGroup.interactable = value;
                canvasGroup.blocksRaycasts = value;
                canvasGroup.alpha = value ? 1f : 0.5f;
            }
        }

        // ------------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------------

        private Color GetFrameColor(CardType type)
        {
            switch (type)
            {
                case CardType.Business: return businessColor;
                case CardType.Employee: return employeeColor;
                case CardType.Action:   return actionColor;
                case CardType.Upgrade:  return upgradeColor;
                case CardType.Event:    return eventColor;
                default:                return Color.white;
            }
        }

        private string BuildStatsLine(CardData card)
        {
            switch (card.cardType)
            {
                case CardType.Business:
                    return $"Income: {card.incomePerTurn}/turn  Customers: {card.customersPerTurn}/turn  Slots: {card.employeeSlots}";

                case CardType.Employee:
                    return $"Salary: {card.salaryPerTurn}/turn  Customers: +{card.customerBonus}";

                case CardType.Action:
                    return card.actionFBIRisk > 0
                        ? $"Effect: {card.actionValue}  FBI: +{card.actionFBIRisk}%"
                        : $"Effect: {card.actionValue}";

                case CardType.Upgrade:
                    return card.isGlobalUpgrade ? "Global Upgrade" : "Business Upgrade";

                case CardType.Event:
                    return $"Duration: {card.eventDuration} turns";

                default:
                    return "";
            }
        }
    }
}
