using System.Text;
using TMPro;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Presentation;
using EmpireOfCards.UI.Clarity;

namespace EmpireOfCards.UI
{
    public class BoardGuidePanelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text ventureText;
        [SerializeField] private TMP_Text operationText;
        [SerializeField] private TMP_Text staffText;
        [SerializeField] private TMP_Text marketingText;
        [SerializeField] private TMP_Text supplierText;
        [SerializeField] private TMP_Text tempText;
        [SerializeField] private TMP_Text footerText;

        public void Init(
            TMP_Text title,
            TMP_Text venture,
            TMP_Text operation,
            TMP_Text staff,
            TMP_Text marketing,
            TMP_Text supplier,
            TMP_Text temp,
            TMP_Text footer)
        {
            titleText = title;
            ventureText = venture;
            operationText = operation;
            staffText = staff;
            marketingText = marketing;
            supplierText = supplier;
            tempText = temp;
            footerText = footer;
        }

        public void Refresh(GameManager gm)
        {
            if (titleText != null)
                titleText.text = "BOARD GUIDE";

            if (gm == null)
                return;

            VentureBoardProfile profile = gm.ActiveBoardProfile;
            BoardPressureType pressure = gm.EconomyManager != null
                ? gm.EconomyManager.CurrentPressure
                : BoardPressureType.None;

            if (ventureText != null)
            {
                string boardName = profile != null && !string.IsNullOrWhiteSpace(profile.displayName)
                    ? profile.displayName
                    : gm.RunDisplayName;
                ventureText.text = $"{boardName.ToUpperInvariant()}  ·  WHAT GOES WHERE";
            }

            if (operationText != null)
                operationText.text = BuildLaneText("OPS", SlotType.Operation, profile != null ? profile.operationSubSlots : null, ControlDeskTheme.OperationSlot);
            if (staffText != null)
                staffText.text = BuildLaneText("TEAM", SlotType.Staff, profile != null ? profile.staffSubSlots : null, ControlDeskTheme.StaffSlot);
            if (marketingText != null)
                marketingText.text = BuildLaneText("GROWTH", SlotType.Marketing, profile != null ? profile.marketingSubSlots : null, ControlDeskTheme.MarketingSlot);
            if (supplierText != null)
                supplierText.text = BuildLaneText("SUPPLY", SlotType.Supplier, profile != null ? profile.supplierSubSlots : null, ControlDeskTheme.SupplierSlot);
            if (tempText != null)
                tempText.text = BuildTempLaneText();

            if (footerText != null)
            {
                footerText.text =
                    $"NOW  {GameClarityFormatter.GetPressureLabel(pressure)}\n" +
                    GameClarityFormatter.GetPressureDetail(pressure);
            }
        }

        private static string BuildLaneText(string label, SlotType slotType, BoardSubSlotDefinition[] defs, Color accent)
        {
            var sb = new StringBuilder();
            sb.Append(Colorize(label, accent));
            sb.Append("  ");
            sb.Append(GameClarityFormatter.GetSlotPurposeTitle(slotType).ToUpperInvariant());
            sb.Append('\n');
            sb.Append(BuildExamples(defs, slotType));
            return sb.ToString();
        }

        private static string BuildTempLaneText()
        {
            string accent = ColorUtility.ToHtmlStringRGB(ControlDeskTheme.EventSlot);
            return $"<color=#{accent}>PATCH</color>  {GameClarityFormatter.GetSlotPurposeTitle(SlotType.TempEffect).ToUpperInvariant()}\n" +
                   "RECOVER RATING / CUT RISK / ABSORB A CRISIS";
        }

        private static string BuildExamples(BoardSubSlotDefinition[] defs, SlotType slotType)
        {
            if (defs != null && defs.Length > 0)
            {
                int count = Mathf.Min(defs.Length, 3);
                var sb = new StringBuilder();
                for (int i = 0; i < count; i++)
                {
                    if (i > 0)
                        sb.Append(" / ");

                    string fallback = string.IsNullOrWhiteSpace(defs[i].fallbackLabel) ? defs[i].id : defs[i].fallbackLabel;
                    string label = LocalizationManager.GetWithFallback(defs[i].labelKey, fallback);
                    sb.Append(label.ToUpperInvariant());
                }

                return sb.ToString();
            }

            return slotType switch
            {
                SlotType.Operation => "PRODUCT / BACKEND / THROUGHPUT",
                SlotType.Staff => "HIRES / EXECUTION / STABILITY",
                SlotType.Marketing => "ADS / ASO / DEMAND",
                SlotType.Supplier => "TOOLS / INPUTS / QUALITY",
                _ => "PATCH / RECOVER / SURVIVE"
            };
        }

        private static string Colorize(string value, Color color)
        {
            string html = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{html}>{value}</color>";
        }
    }
}
