using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Checks all 10 combos from the GDD Combo Matrix each turn.
    /// Collects all active card IDs and tags from the board, then matches against
    /// ComboData requirements. Tracks newly triggered vs already active combos
    /// and fires events for UI popups.
    /// </summary>
    public class ComboSystem : MonoBehaviour
    {
        // --- Data ---
        [Header("Combo Database")]
        [SerializeField] private ComboData[] allCombos;

        // --- Manager References ---
        [Header("References")]
        [SerializeField] private BoardManager boardManager;

        // --- Runtime State ---
        [Header("Active Combos (Read Only)")]
        [SerializeField] private List<ComboData> activeCombos = new List<ComboData>();

        // --- Properties ---
        public IReadOnlyList<ComboData> ActiveCombos => activeCombos;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService instead of RuntimeWiring.SetField().
        /// </summary>
        public void Init(ComboData[] combos, BoardManager board)
        {
            this.allCombos = combos;
            this.boardManager = board;
        }

        // ----------------------------------------------------------------
        // Main Check
        // ----------------------------------------------------------------

        /// <summary>
        /// Evaluates every combo in the database against the current board state.
        /// Call this once each turn during the Resolve phase.
        ///
        /// Collects:
        /// - All active card IDs from board (businesses + employees + upgrades + active event)
        /// - All active tags
        /// - Specific placement info (employee X in business Y)
        /// Then matches each combo's requirements.
        /// </summary>
        public void CheckCombos(int playerMoney, int playerTerritories, int activeBusinessCount, float marketShare)
        {
            if (allCombos == null || allCombos.Length == 0) return;
            if (boardManager == null)
            {
                Debug.LogWarning("[ComboSystem] BoardManager reference is null.");
                return;
            }

            // Snapshot previous state
            List<ComboData> previouslyActive = new List<ComboData>(activeCombos);
            activeCombos.Clear();

            // Gather board state
            HashSet<string> activeCardIds = new HashSet<string>(boardManager.GetAllActiveCardIds());
            HashSet<CardTag> activeTags = boardManager.GetAllActiveTags();
            string activeEventId = boardManager.ActiveEvent != null ? boardManager.ActiveEvent.cardId : null;

            // Evaluate each combo
            foreach (var combo in allCombos)
            {
                if (combo == null) continue;

                bool met = EvaluateCombo(
                    combo, activeCardIds, activeTags, activeEventId,
                    playerMoney, playerTerritories, activeBusinessCount, marketShare
                );

                if (met)
                {
                    activeCombos.Add(combo);

                    // Check if this is newly triggered (not in previous frame)
                    if (!previouslyActive.Contains(combo))
                    {
                        Debug.Log($"[ComboSystem] NEW combo triggered: {combo.comboName}");
                        EventBus.ComboTriggered(combo);
                    }
                }
            }

            // Fire deactivation events for combos that were active but no longer are
            foreach (var prev in previouslyActive)
            {
                if (!activeCombos.Contains(prev))
                {
                    Debug.Log($"[ComboSystem] Combo deactivated: {prev.comboName}");
                    EventBus.ComboDeactivated(prev);
                }
            }
        }

        // ----------------------------------------------------------------
        // Combo Evaluation
        // ----------------------------------------------------------------

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
        private bool EvaluateCombo(
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
        /// Used for combos like "Latte Sanati" (Barista in Kahveci).
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

        // ----------------------------------------------------------------
        // Bonus Queries (used by EconomyManager, ShopManager, etc.)
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns total bonus income from all active combos.
        /// </summary>
        public int GetTotalBonusIncome()
        {
            int total = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    total += combo.bonusIncome;
            }
            return total;
        }

        /// <summary>
        /// Returns total bonus customers from all active combos.
        /// </summary>
        public int GetTotalBonusCustomers()
        {
            int total = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    total += combo.bonusCustomers;
            }
            return total;
        }

        /// <summary>
        /// Returns the combined income multiplier from active combos.
        /// Base is 1.0; combos multiply on top.
        /// </summary>
        public float GetIncomeMultiplier()
        {
            float multiplier = 1f;
            foreach (var combo in activeCombos)
            {
                if (combo != null && combo.incomeMultiplier > 1f)
                    multiplier *= combo.incomeMultiplier;
            }
            return multiplier;
        }

        /// <summary>
        /// Returns the combined customer multiplier from active combos.
        /// </summary>
        public float GetCustomerMultiplier()
        {
            float multiplier = 1f;
            foreach (var combo in activeCombos)
            {
                if (combo != null && combo.customerMultiplier > 1f)
                    multiplier *= combo.customerMultiplier;
            }
            return multiplier;
        }

        /// <summary>
        /// Returns the shop discount rate from active combos (Kriz Avcisi: 50%).
        /// </summary>
        public float GetShopDiscount()
        {
            float discount = 0f;
            foreach (var combo in activeCombos)
            {
                if (combo != null && combo.shopDiscount > 0f)
                    discount = Mathf.Max(discount, combo.shopDiscount);
            }
            return discount;
        }

        /// <summary>
        /// Returns the extra action count from active combos (AI Devrimi: +1).
        /// </summary>
        public int GetExtraActions()
        {
            int extra = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    extra += combo.extraActions;
            }
            return extra;
        }

        /// <summary>
        /// Returns the total rival customer penalty from active combos (Monopol: -3/turn).
        /// </summary>
        public int GetRivalCustomerPenalty()
        {
            int penalty = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    penalty += combo.rivalCustomerPenalty;
            }
            return penalty;
        }

        /// <summary>
        /// Returns extra FBI risk from active combos (Yeralti: +8).
        /// </summary>
        public int GetExtraFBIRisk()
        {
            int risk = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    risk += combo.extraFBIRisk;
            }
            return risk;
        }

        /// <summary>
        /// Checks if a specific combo is currently active by ID.
        /// </summary>
        public bool IsComboActive(string comboId)
        {
            foreach (var combo in activeCombos)
            {
                if (combo != null && combo.comboId == comboId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a copy of the active combos list.
        /// </summary>
        public List<ComboData> GetActiveCombos()
        {
            return new List<ComboData>(activeCombos);
        }

        /// <summary>
        /// Resets combo state for a new run.
        /// </summary>
        public void Reset()
        {
            activeCombos.Clear();
        }
    }
}
