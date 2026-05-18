using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.UI.Cards
{
    /// <summary>
    /// Type of drop zone on the board.
    /// </summary>
    public enum DropZoneType
    {
        BusinessSlot,
        EmployeeSlot,
        UpgradeSlot,
        ActionZone,
        SellZone,
        BurnZone
    }

    /// <summary>
    /// Drop target for card placement. Validates card type vs zone type
    /// and calls the appropriate BoardManager method on successful drop.
    /// Shows green highlight for valid drops, red for invalid while dragging.
    /// </summary>
    public class DropZone : MonoBehaviour
    {
        [Header("Zone Config")]
        [SerializeField] private DropZoneType zoneType;
        [SerializeField] private int slotIndex;                  // business slot or employee business index

        [Header("Highlight")]
        [SerializeField] private Image highlightImage;
        [SerializeField] private Color validColor = new Color(0f, 1f, 0.3f, 0.35f);
        [SerializeField] private Color invalidColor = new Color(1f, 0f, 0f, 0.25f);

        [Header("Manager Reference")]
        [SerializeField] private BoardManager boardManager;

        // --- Static registry for broadcast ---
        private static readonly List<DropZone> allZones = new List<DropZone>();

        // --- Properties ---
        public DropZoneType ZoneType => zoneType;
        public int SlotIndex => slotIndex;

        // ------------------------------------------------------------------
        // Lifecycle
        // ------------------------------------------------------------------

        private void OnEnable()
        {
            allZones.Add(this);
            SetHighlight(false, false);
        }

        private void OnDisable()
        {
            allZones.Remove(this);
        }

        // ------------------------------------------------------------------
        // Static broadcast helpers (called by CardDragHandler)
        // ------------------------------------------------------------------

        /// <summary>
        /// Tells every active DropZone to show valid/invalid highlight for the
        /// card currently being dragged.
        /// </summary>
        public static void BroadcastDragStarted(CardUI card)
        {
            foreach (var zone in allZones)
            {
                bool valid = zone.CanAccept(card);
                zone.SetHighlight(true, valid);
            }
        }

        /// <summary>
        /// Hides all highlights when dragging ends.
        /// </summary>
        public static void BroadcastDragEnded()
        {
            foreach (var zone in allZones)
            {
                zone.SetHighlight(false, false);
            }
        }

        // ------------------------------------------------------------------
        // Validation
        // ------------------------------------------------------------------

        /// <summary>
        /// Returns true if this zone can accept the given card.
        /// </summary>
        public bool CanAccept(CardUI cardUI)
        {
            if (cardUI == null || cardUI.Data == null)
                return false;

            CardData card = cardUI.Data;

            switch (zoneType)
            {
                case DropZoneType.BusinessSlot:
                    return card.cardType == CardType.Business && IsSlotEmpty();

                case DropZoneType.EmployeeSlot:
                    return card.cardType == CardType.Employee && HasRoomForEmployee();

                case DropZoneType.UpgradeSlot:
                    return card.cardType == CardType.Upgrade;

                case DropZoneType.ActionZone:
                    return card.cardType == CardType.Action;

                case DropZoneType.SellZone:
                    return true; // any card can be sold

                case DropZoneType.BurnZone:
                    return true; // any card can be burned

                default:
                    return false;
            }
        }

        /// <summary>
        /// Processes the dropped card. Calls the appropriate BoardManager method
        /// and fires EventBus events.
        /// </summary>
        public void AcceptCard(CardUI cardUI)
        {
            if (cardUI == null || cardUI.Data == null)
                return;

            CardData card = cardUI.Data;
            bool success = false;

            switch (zoneType)
            {
                case DropZoneType.BusinessSlot:
                    if (boardManager != null)
                        success = boardManager.PlaceBusiness(card, slotIndex);
                    if (success)
                        EventBus.BusinessPlaced(card, slotIndex);
                    break;

                case DropZoneType.EmployeeSlot:
                    if (boardManager != null)
                        success = boardManager.PlaceEmployee(card, slotIndex);
                    if (success)
                        EventBus.EmployeePlaced(card, slotIndex);
                    break;

                case DropZoneType.UpgradeSlot:
                    if (boardManager != null)
                        success = boardManager.PlaceUpgrade(card, slotIndex);
                    if (success)
                        EventBus.UpgradePlaced(card, slotIndex);
                    break;

                case DropZoneType.ActionZone:
                    success = true;
                    EventBus.ActionExecuted(card);
                    break;

                case DropZoneType.SellZone:
                {
                    int sellPrice = Mathf.RoundToInt(card.buyCost * Constants.SELL_RATE);
                    GameManager.Instance?.GainMoney(sellPrice);
                    success = true;
                    EventBus.CardDiscarded(card);
                    break;
                }

                case DropZoneType.BurnZone:
                    success = true;
                    EventBus.CardBurned(card);
                    break;
            }

            if (success)
            {
                // Use an action for placing / using cards (sell and burn are free)
                if (zoneType != DropZoneType.SellZone && zoneType != DropZoneType.BurnZone)
                    GameManager.Instance?.UseAction();

                EventBus.CardPlayed(card);

                // Destroy the card visual -- HandUI also gets notified via OnCardPlayed
                Destroy(cardUI.gameObject);
            }
        }

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        private bool IsSlotEmpty()
        {
            if (boardManager == null)
                return true;

            return slotIndex >= boardManager.PlayerBusinesses.Count
                || boardManager.PlayerBusinesses[slotIndex] == null;
        }

        private bool HasRoomForEmployee()
        {
            if (boardManager == null)
                return false;

            if (slotIndex < 0 || slotIndex >= boardManager.PlayerBusinesses.Count)
                return false;

            var business = boardManager.PlayerBusinesses[slotIndex];
            if (business == null || business.isClosed)
                return false;

            return business.employees.Count < business.businessCard.employeeSlots;
        }

        private void SetHighlight(bool visible, bool valid)
        {
            if (highlightImage == null)
                return;

            if (!visible)
            {
                highlightImage.enabled = false;
                return;
            }

            highlightImage.enabled = true;
            highlightImage.color = valid ? validColor : invalidColor;
        }
    }
}
