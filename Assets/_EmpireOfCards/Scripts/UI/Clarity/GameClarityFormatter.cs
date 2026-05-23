using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;
using EmpireOfCards.World;
using EmpireOfCards.UI.Cards;
using EmpireOfCards.Presentation;

namespace EmpireOfCards.UI.Clarity
{
    public static class GameClarityFormatter
    {
        public static string GetRoleLabel(CardData card)
        {
            if (card == null)
                return string.Empty;

            if (card.cardFamily == CardFamily.Reaction)
                return "RECOVER RATING";

            if (card.cardFamily == CardFamily.Risk)
                return "RISKY SHORTCUT";

            if (card.targetSlotType == SlotType.Marketing)
                return "CREATE DEMAND";

            if (card.targetSlotType == SlotType.Supplier)
                return card.qualityDelta > 0.3f ? "IMPROVE QUALITY" : "IMPROVE MARGIN";

            if (card.targetSlotType == SlotType.Staff)
                return card.capacityDelta >= 1f ? "FIX CAPACITY" : "STABILIZE TEAM";

            if (card.targetSlotType == SlotType.Operation)
                return card.capacityDelta >= card.demandDelta ? "BUILD CAPACITY" : "SET UP CORE";

            if (card.targetSlotType == SlotType.TempEffect)
                return card.cardFamily == CardFamily.Crisis ? "CRISIS PRESSURE" : "PATCH THE TURN";

            return card.cardFamily switch
            {
                CardFamily.Setup => "BUILD CAPACITY",
                CardFamily.Growth => "CREATE DEMAND",
                CardFamily.Reaction => "RECOVER RATING",
                CardFamily.Risk => "RISKY SHORTCUT",
                CardFamily.Crisis => "CRISIS PRESSURE",
                _ => "BOARD TOOL"
            };
        }

        public static Color GetRoleTone(CardData card)
        {
            string role = GetRoleLabel(card);
            return role switch
            {
                "BUILD CAPACITY" => ControlDeskTheme.OperationSlot,
                "SET UP CORE" => ControlDeskTheme.OperationSlot,
                "FIX CAPACITY" => ControlDeskTheme.StaffSlot,
                "STABILIZE TEAM" => ControlDeskTheme.StaffSlot,
                "CREATE DEMAND" => ControlDeskTheme.MarketingSlot,
                "IMPROVE QUALITY" => ControlDeskTheme.SupplierSlot,
                "IMPROVE MARGIN" => ControlDeskTheme.SupplierSlot,
                "RECOVER RATING" => ControlDeskTheme.AccentBlue,
                "RISKY SHORTCUT" => ControlDeskTheme.AccentRed,
                "CRISIS PRESSURE" => ControlDeskTheme.AccentRed,
                "PATCH THE TURN" => ControlDeskTheme.AccentAmber,
                _ => ControlDeskTheme.PanelLine
            };
        }

        public static string GetWhyPlayThis(CardData card)
        {
            if (card == null)
                return string.Empty;

            BoardPressureType pressure = GameManager.Instance != null && GameManager.Instance.EconomyManager != null
                ? GameManager.Instance.EconomyManager.CurrentPressure
                : BoardPressureType.None;

            if (MatchesPressure(card, pressure))
            {
                return pressure switch
                {
                    BoardPressureType.CapacityShortfall => "Use when demand is outrunning service.",
                    BoardPressureType.LowDemand => "Use when traffic is too weak.",
                    BoardPressureType.LowRating => "Use when trust or reviews are slipping.",
                    BoardPressureType.HighLegalRisk => "Use when risk is snowballing.",
                    BoardPressureType.LowCash => "Use when margin is getting squeezed.",
                    BoardPressureType.WeakQuality => "Use when quality is lagging behind.",
                    BoardPressureType.StaffInstability => "Use when the team is cracking under pressure.",
                    _ => "Use when this is your biggest pressure."
                };
            }

            if (card.targetSlotType == SlotType.Marketing)
                return "Use when the board can absorb more traffic.";
            if (card.targetSlotType == SlotType.Operation)
                return "Use before heavy marketing or rush turns.";
            if (card.targetSlotType == SlotType.Staff)
                return "Use when service, stability, or throughput needs help.";
            if (card.targetSlotType == SlotType.Supplier)
                return "Use when quality or margin needs a backbone.";
            if (card.targetSlotType == SlotType.TempEffect)
                return card.cardFamily == CardFamily.Crisis
                    ? "This punishes an existing weakness on your board."
                    : "Use when you need a short-term patch.";

            return "Use when this supports your current build.";
        }

        public static string GetProblemSolved(CardData card)
        {
            if (card == null)
                return string.Empty;

            var solved = new List<string>();
            if (card.capacityDelta > 0.6f) solved.Add("capacity");
            if (card.demandDelta > 0.6f) solved.Add("demand");
            if (card.qualityDelta > 0.4f) solved.Add("quality");
            if (card.ratingDeltaPerTurn > 0.15f) solved.Add("rating");
            if (card.staffStabilityDelta > 0.25f) solved.Add("stability");
            if (card.cashDeltaPerTurn > 8f || (card.upkeepCostPerTurn <= 0f && card.salaryPerTurn <= 0 && card.buyCost <= 40)) solved.Add("margin");
            if (card.legalRiskDeltaPerTurn < -0.1f || card.cardFamily == CardFamily.Reaction) solved.Add("risk");

            if (solved.Count == 0)
                solved.Add(card.cardFamily == CardFamily.Risk ? "tempo" : "positioning");

            return "Solves: " + string.Join(", ", solved);
        }

        public static string BuildCardBody(CardData card)
        {
            if (card == null)
                return string.Empty;

            return $"{GetWhyPlayThis(card)}\n{GetProblemSolved(card)}";
        }

        public static string BuildCardFrontSummary(CardData card)
        {
            if (card == null)
                return string.Empty;

            if (card.cardFamily == CardFamily.Risk)
                return "Fast tempo now. Future pressure later.";

            if (card.cardFamily == CardFamily.Reaction)
                return "Stabilize the weak side of your board.";

            return card.targetSlotType switch
            {
                SlotType.Operation => "Build throughput before traffic spikes.",
                SlotType.Staff => "Protect service flow and team stability.",
                SlotType.Marketing => "Pull more traffic only if the board can absorb it.",
                SlotType.Supplier => "Strengthen quality or margin discipline.",
                SlotType.TempEffect => "Patch a weak turn or blunt a crisis.",
                _ => "Support the board this turn."
            };
        }

        public static string BuildCardStats(CardData card)
        {
            if (card == null)
                return string.Empty;

            var parts = new List<string>();
            if (Mathf.Abs(card.demandDelta) > 0.05f) parts.Add($"Demand {Signed(card.demandDelta)}");
            if (Mathf.Abs(card.capacityDelta) > 0.05f) parts.Add($"Cap {Signed(card.capacityDelta)}");
            if (Mathf.Abs(card.qualityDelta) > 0.05f) parts.Add($"Qual {Signed(card.qualityDelta)}");
            if (Mathf.Abs(card.ratingDeltaPerTurn) > 0.05f) parts.Add($"Rate {Signed(card.ratingDeltaPerTurn)}");
            if (Mathf.Abs(card.staffStabilityDelta) > 0.05f) parts.Add($"Stab {Signed(card.staffStabilityDelta)}");
            if (Mathf.Abs(card.cashDeltaPerTurn) > 0.05f) parts.Add($"Cash {Signed(card.cashDeltaPerTurn)}");
            if (Mathf.Abs(card.legalRiskDeltaPerTurn) > 0.05f) parts.Add($"Risk {Signed(card.legalRiskDeltaPerTurn)}");

            float upkeep = Mathf.Max(card.upkeepCostPerTurn, card.salaryPerTurn);
            parts.Add($"B {Mathf.Max(0, card.buyCost)}");
            if (card.playCost > 0) parts.Add($"P {card.playCost}");
            if (upkeep > 0f) parts.Add($"U {Mathf.RoundToInt(upkeep)}");

            return string.Join(" | ", parts);
        }

        public static string GetSlotPurposeTitle(SlotType slotType)
        {
            return slotType switch
            {
                SlotType.Operation => "Build Capacity",
                SlotType.Staff => "Run the Machine",
                SlotType.Marketing => "Create Demand",
                SlotType.Supplier => "Control Quality & Cost",
                SlotType.TempEffect => "Patch or Survive Pressure",
                _ => "Board Tool"
            };
        }

        public static string GetSlotPurposeDetail(SlotType slotType)
        {
            return slotType switch
            {
                SlotType.Operation => "Use this lane to add throughput before traffic spikes.",
                SlotType.Staff => "Use this lane when service, reliability, or morale is weak.",
                SlotType.Marketing => "Use this lane when demand is too low and the board can absorb more traffic.",
                SlotType.Supplier => "Use this lane to improve ingredients, inputs, or margin discipline.",
                SlotType.TempEffect => "Use this lane to recover rating, reduce risk, or absorb a crisis.",
                _ => "Use this to stabilize your board."
            };
        }

        public static string GetSlotFailureDetail(SlotType slotType)
        {
            return slotType switch
            {
                SlotType.Operation => "If you skip this, marketing can overload the board and kill rating.",
                SlotType.Staff => "If you skip this, burnout and slow execution spill into trust.",
                SlotType.Marketing => "If you overdo this, demand rises faster than capacity and reviews collapse.",
                SlotType.Supplier => "If you skip this, quality and margin become unstable.",
                SlotType.TempEffect => "If you ignore this, crises and weak stats keep compounding.",
                _ => "If you ignore this, pressure compounds."
            };
        }

        public static string BuildPreview(CardData card, SlotZone3D slot, bool valid)
        {
            if (card == null)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append(valid ? "Valid placement" : "Wrong lane or slot full");
            if (slot != null)
            {
                sb.Append("  •  ");
                sb.Append(GetSlotPurposeTitle(MapZoneToSlot(slot.ZoneType)));
            }
            sb.Append('\n');
            sb.Append(GetProblemSolved(card));
            sb.Append('\n');
            sb.Append(BuildProjectedDeltaLine(card));
            return sb.ToString();
        }

        public static string BuildProjectedDeltaLine(CardData card)
        {
            if (card == null)
                return string.Empty;

            var tokens = new List<string>();
            if (card.demandDelta > 0.05f) tokens.Add($"Demand {Signed(card.demandDelta)}");
            if (card.capacityDelta > 0.05f) tokens.Add($"Capacity {Signed(card.capacityDelta)}");
            if (card.qualityDelta > 0.05f) tokens.Add($"Quality {Signed(card.qualityDelta)}");
            if (card.ratingDeltaPerTurn > 0.05f) tokens.Add($"Rating {Signed(card.ratingDeltaPerTurn)}");
            if (card.staffStabilityDelta > 0.05f) tokens.Add($"Stability {Signed(card.staffStabilityDelta)}");
            if (card.legalRiskDeltaPerTurn > 0.05f) tokens.Add($"Risk {Signed(card.legalRiskDeltaPerTurn)}");

            float upkeep = Mathf.Max(card.upkeepCostPerTurn, card.salaryPerTurn);
            if (upkeep > 0f) tokens.Add($"Upkeep +{Mathf.RoundToInt(upkeep)}");
            if (card.playCost > 0) tokens.Add($"Play ${card.playCost}");
            return tokens.Count > 0 ? string.Join("  |  ", tokens) : "Small board adjustment.";
        }

        public static string BuildCostSummary(CardData card)
        {
            if (card == null)
                return string.Empty;

            var parts = new List<string> { $"Buy ${Mathf.Max(0, card.buyCost)}" };
            if (card.playCost > 0)
                parts.Add($"Play ${card.playCost}");

            float upkeep = Mathf.Max(card.upkeepCostPerTurn, card.salaryPerTurn);
            if (upkeep > 0f)
                parts.Add($"Upkeep ${Mathf.RoundToInt(upkeep)}");

            return string.Join("  ·  ", parts);
        }

        public static string GetPressureLabel(BoardPressureType pressure)
        {
            return pressure switch
            {
                BoardPressureType.CapacityShortfall => "DEMAND PRESSURE",
                BoardPressureType.LowCash => "MARGIN PRESSURE",
                BoardPressureType.LowRating => "TRUST PRESSURE",
                BoardPressureType.HighLegalRisk => "RISK PRESSURE",
                BoardPressureType.WeakQuality => "QUALITY PRESSURE",
                BoardPressureType.StaffInstability => "TEAM PRESSURE",
                BoardPressureType.LowDemand => "DISCOVERY PRESSURE",
                _ => "BOARD STABLE"
            };
        }

        public static string GetPressureDetail(BoardPressureType pressure)
        {
            return pressure switch
            {
                BoardPressureType.CapacityShortfall => "Traffic is outrunning capacity.",
                BoardPressureType.LowCash => "Runway is getting squeezed.",
                BoardPressureType.LowRating => "Trust is slipping and organic pull is weak.",
                BoardPressureType.HighLegalRisk => "Shortcuts can chain into crises.",
                BoardPressureType.WeakQuality => "Quality is lagging behind the promise.",
                BoardPressureType.StaffInstability => "The team is starting to crack.",
                BoardPressureType.LowDemand => "The board needs a cleaner demand engine.",
                _ => "You can push your edge this turn."
            };
        }

        public static string GetBuildIdentity(GameManager gm)
        {
            if (gm == null || gm.BoardManager == null || gm.EconomyManager == null)
                return "Balanced Board";

            var snapshot = gm.EconomyManager.Snapshot;
            int ops = gm.BoardManager.GetCardsInSlotType(SlotType.Operation).Count;
            int staff = gm.BoardManager.GetCardsInSlotType(SlotType.Staff).Count;
            int marketing = gm.BoardManager.GetCardsInSlotType(SlotType.Marketing).Count;
            int suppliers = gm.BoardManager.GetCardsInSlotType(SlotType.Supplier).Count;
            int temp = gm.BoardManager.GetCardsInSlotType(SlotType.TempEffect).Count;

            if (snapshot.legalRisk >= 10f)
                return "Risky Shortcut";
            if (temp >= 2 || snapshot.rating < 3.4f)
                return "Safe Recovery";
            if (gm.SelectedVenture != null && gm.SelectedVenture.ventureType == VentureType.Cafe && suppliers + staff >= 4 && snapshot.rating >= 4.0f)
                return "Loyalty Cafe";
            if (gm.SelectedVenture != null && gm.SelectedVenture.ventureType == VentureType.GroceryStore && suppliers >= 2 && staff >= 2)
                return "Neighborhood Grocery";
            if (gm.SelectedVenture != null && gm.SelectedVenture.ventureType == VentureType.TechApp && marketing >= ops + 1)
                return "Tech Growth Push";
            if (marketing >= ops + staff)
                return "Aggressive Marketing";
            if (suppliers >= 2 && snapshot.quality >= 6f)
                return "Premium Quality";
            if (ops + staff >= 5 && snapshot.cash < 150f)
                return "Cheap Expansion";
            return "Balanced Board";
        }

        public static string GetBuildIdentityDetail(string buildIdentity)
        {
            return buildIdentity switch
            {
                "Aggressive Marketing" => "You are leaning into traffic. Capacity and rating are the watch-outs.",
                "Premium Quality" => "You are winning on trust. Margin and demand pace are the watch-outs.",
                "Cheap Expansion" => "You are stretching the board quickly. Cash and quality are fragile.",
                "Safe Recovery" => "You are stabilizing weak stats before pushing harder.",
                "Risky Shortcut" => "You are trading safety for tempo. Crises can chain if you overstay.",
                "Tech Growth Push" => "You are pushing installs ahead of stability. Backend and reviews decide if it sticks.",
                "Loyalty Cafe" => "You are building repeat trust. Slow service or burnout can still break the loop.",
                "Neighborhood Grocery" => "You are leaning on consistency and local trust. Waste and thin margin are the danger.",
                _ => "You are not overcommitted yet. The board can still pivot."
            };
        }

        private static bool MatchesPressure(CardData card, BoardPressureType pressure)
        {
            if (card == null || card.preferredPressures == null)
                return false;

            for (int i = 0; i < card.preferredPressures.Length; i++)
            {
                if (card.preferredPressures[i] == pressure)
                    return true;
            }
            return false;
        }

        private static SlotType MapZoneToSlot(DropZoneType zoneType)
        {
            return zoneType switch
            {
                DropZoneType.OperationSlot => SlotType.Operation,
                DropZoneType.StaffSlot => SlotType.Staff,
                DropZoneType.MarketingSlot => SlotType.Marketing,
                DropZoneType.SupplierSlot => SlotType.Supplier,
                DropZoneType.TempEffectSlot => SlotType.TempEffect,
                _ => SlotType.Operation
            };
        }

        private static string Signed(float value)
        {
            return value >= 0f ? $"+{value:0.0}" : $"{value:0.0}";
        }
    }
}
