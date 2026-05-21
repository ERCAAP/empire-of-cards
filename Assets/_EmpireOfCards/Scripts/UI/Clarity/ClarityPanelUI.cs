using TMPro;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.World;
using EmpireOfCards.UI.Cards;

namespace EmpireOfCards.UI.Clarity
{
    public class ClarityPanelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text footerText;

        public void Init(TMP_Text title, TMP_Text subtitle, TMP_Text body, TMP_Text footer)
        {
            titleText = title;
            subtitleText = subtitle;
            bodyText = body;
            footerText = footer;
            Hide();
        }

        public void ShowCard(CardData card, string buildIdentity)
        {
            if (card == null)
            {
                Hide();
                return;
            }

            gameObject.SetActive(true);
            if (titleText != null)
                titleText.text = $"{card.cardName}  •  {GameClarityFormatter.GetRoleLabel(card)}";
            if (subtitleText != null)
                subtitleText.text = GameClarityFormatter.GetProblemSolved(card);
            if (bodyText != null)
                bodyText.text = $"{GameClarityFormatter.GetWhyPlayThis(card)}\n{GameClarityFormatter.BuildProjectedDeltaLine(card)}";
            if (footerText != null)
                footerText.text = $"Current Build: {buildIdentity}";
        }

        public void ShowSlotPreview(CardData card, SlotZone3D slot, bool valid, string buildIdentity)
        {
            if (card == null || slot == null)
            {
                Hide();
                return;
            }

            gameObject.SetActive(true);
            SlotType slotType = slot.ZoneType switch
            {
                DropZoneType.OperationSlot => SlotType.Operation,
                DropZoneType.StaffSlot => SlotType.Staff,
                DropZoneType.MarketingSlot => SlotType.Marketing,
                DropZoneType.SupplierSlot => SlotType.Supplier,
                DropZoneType.TempEffectSlot => SlotType.TempEffect,
                _ => SlotType.Operation
            };

            if (titleText != null)
                titleText.text = valid
                    ? $"{card.cardName} -> {GameClarityFormatter.GetSlotPurposeTitle(slotType)}"
                    : $"{card.cardName} -> Invalid Placement";
            if (subtitleText != null)
                subtitleText.text = GameClarityFormatter.GetSlotPurposeDetail(slotType);
            if (bodyText != null)
                bodyText.text = valid
                    ? GameClarityFormatter.BuildPreview(card, slot, true)
                    : $"{GameClarityFormatter.GetSlotFailureDetail(slotType)}\n{GameClarityFormatter.BuildProjectedDeltaLine(card)}";
            if (footerText != null)
                footerText.text = $"Current Build: {buildIdentity}";
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
