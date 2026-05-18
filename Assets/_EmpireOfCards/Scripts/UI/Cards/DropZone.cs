using System;
using UnityEngine;
using UnityEngine.UI;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.UI.Cards
{
    /// <summary>
    /// Defines a region where cards can be dropped. Each zone has a type
    /// that determines which cards are valid.
    /// </summary>
    public class DropZone : MonoBehaviour
    {
        public enum DropZoneType
        {
            BusinessSlot,
            EmployeeSlot,
            UpgradeSlot,
            ActionZone,
            SellZone,
            BurnZone
        }

        [Header("Zone Config")]
        [SerializeField] private DropZoneType zoneType;
        [SerializeField] private int slotIndex;
        [SerializeField] private int parentBusinessIndex;

        [Header("Visuals")]
        [SerializeField] private Image highlight;

        // --- Events ---
        public event Action<CardData, DropZoneType, int> OnCardReceived;

        /// <summary>
        /// The type of this drop zone.
        /// </summary>
        public DropZoneType ZoneType => zoneType;

        /// <summary>
        /// The slot index within its parent (e.g. employee slot 0, 1, 2).
        /// </summary>
        public int SlotIndex => slotIndex;

        /// <summary>
        /// The index of the parent business this slot belongs to (-1 for global zones).
        /// </summary>
        public int ParentBusinessIndex => parentBusinessIndex;

        /// <summary>
        /// Shows or hides the highlight overlay on this zone.
        /// </summary>
        public void ShowHighlight(bool show, Color color)
        {
            if (highlight == null)
                return;

            highlight.enabled = show;
            if (show)
            {
                highlight.color = new Color(color.r, color.g, color.b, 0.35f);
            }
        }

        /// <summary>
        /// Returns true if the given card can legally be dropped on this zone.
        /// </summary>
        public bool IsValidDrop(CardData card)
        {
            if (card == null)
                return false;

            switch (zoneType)
            {
                case DropZoneType.BusinessSlot:
                    return card.cardType == CardType.Business;

                case DropZoneType.EmployeeSlot:
                    return card.cardType == CardType.Employee;

                case DropZoneType.UpgradeSlot:
                    return card.cardType == CardType.Upgrade;

                case DropZoneType.ActionZone:
                    return card.cardType == CardType.Action;

                case DropZoneType.SellZone:
                    // Any card can be sold
                    return true;

                case DropZoneType.BurnZone:
                    // Any card can be burned/discarded
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Called when a valid card is dropped on this zone.
        /// </summary>
        public void OnCardDropped(CardData card)
        {
            if (!IsValidDrop(card))
                return;

            ShowHighlight(false, Color.clear);
            OnCardReceived?.Invoke(card, zoneType, slotIndex);
        }
    }
}
