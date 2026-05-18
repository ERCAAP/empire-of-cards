using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Gameplay.Combo
{
    /// <summary>
    /// Pure combo requirement matching logic extracted from ComboSystem.
    /// Evaluates whether combos are active based on board state.
    /// </summary>
    public class ComboEvaluator
    {
        private readonly BoardManager boardManager;

        public ComboEvaluator(BoardManager boardManager)
        {
            this.boardManager = boardManager;
        }

        /// <summary>
        /// Checks if a specific combo is active in the given active combos list.
        /// </summary>
        public bool IsComboActive(string comboId, IReadOnlyList<ComboData> activeCombos)
        {
            foreach (var combo in activeCombos)
            {
                if (combo != null && combo.comboId == comboId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Collects all active card IDs from the board.
        /// </summary>
        public HashSet<string> CollectActiveCardIds()
        {
            if (boardManager == null) return new HashSet<string>();
            return new HashSet<string>(boardManager.GetAllActiveCardIds());
        }

        /// <summary>
        /// Collects all active tags from the board.
        /// </summary>
        public HashSet<CardTag> CollectActiveTags()
        {
            if (boardManager == null) return new HashSet<CardTag>();
            return boardManager.GetAllActiveTags();
        }

        /// <summary>
        /// Checks whether all requirements for a specific combo are met.
        /// Requirements can include:
        /// - Required card IDs (all must be on board)
        /// - Required tags (all must be present)
        /// - Specific placement (employee X in business Y)
        /// - Active event requirement
        /// - Minimum money (Kriz Avcisi: 1000)
        /// - Minimum territories / businesses / market share (Monopol)
        /// </summary>
        public bool EvaluateCombo(
            ComboData combo,
            HashSet<string> activeCardIds,
            HashSet<CardTag> activeTags,
            string activeEventId,
            int playerMoney,
            int playerTerritories,
            int activeBusinessCount,
            float marketShare)
        {
            // --- Required card IDs ---
            if (combo.requiredCardIds != null && combo.requiredCardIds.Length > 0)
            {
                foreach (string reqId in combo.requiredCardIds)
                {
                    if (string.IsNullOrEmpty(reqId)) continue;
                    if (!activeCardIds.Contains(reqId))
                        return false;
                }
            }

            // --- Required tags ---
            if (combo.requiredTags != null && combo.requiredTags.Length > 0)
            {
                foreach (CardTag reqTag in combo.requiredTags)
                {
                    if (!activeTags.Contains(reqTag))
                        return false;
                }
            }

            // --- Specific placement: employee must be in specific business ---
            if (combo.requiresSpecificPlacement)
            {
                if (!CheckSpecificPlacement(combo.employeeCardId, combo.businessCardId))
                    return false;
            }

            // --- Active event requirement ---
            if (combo.requiresActiveEvent)
            {
                if (string.IsNullOrEmpty(activeEventId))
                    return false;

                if (!string.IsNullOrEmpty(combo.requiredEventId) && activeEventId != combo.requiredEventId)
                    return false;
            }

            // --- Minimum money ---
            if (combo.requiresMinMoney && playerMoney < combo.minMoneyRequired)
                return false;

            // --- Minimum territory ---
            if (combo.requiresMinTerritory && playerTerritories < combo.minTerritoryRequired)
                return false;

            // --- Minimum active businesses ---
            if (combo.minActiveBusinesses > 0 && activeBusinessCount < combo.minActiveBusinesses)
                return false;

            // --- Minimum market share ---
            if (combo.minMarketShare > 0f && marketShare < combo.minMarketShare)
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a specific employee is placed in a specific business.
        /// Used for combos like "Latte Art" (Barista in Coffee Shop).
        /// </summary>
        private bool CheckSpecificPlacement(string employeeCardId, string businessCardId)
        {
            if (boardManager == null) return false;
            if (string.IsNullOrEmpty(employeeCardId) || string.IsNullOrEmpty(businessCardId))
                return false;

            foreach (var business in boardManager.PlayerBusinesses)
            {
                if (business.isClosed) continue;
                if (business.businessCard == null) continue;
                if (business.businessCard.cardId != businessCardId) continue;

                // Found the target business, check if employee is here
                foreach (var emp in business.employees)
                {
                    if (emp != null && emp.cardId == employeeCardId)
                        return true;
                }
            }

            return false;
        }
    }
}
