using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Combo;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Checks all 10 combos from the GDD Combo Matrix each turn.
    /// Coordinator that delegates evaluation to ComboEvaluator
    /// and bonus queries to ComboBonusProvider.
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

        // --- Extracted Helpers ---
        private ComboEvaluator evaluator;
        private ComboBonusProvider bonusProvider;

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

            evaluator = new ComboEvaluator(boardManager);
            bonusProvider = new ComboBonusProvider(activeCombos);
        }

        // ----------------------------------------------------------------
        // Main Check
        // ----------------------------------------------------------------

        /// <summary>
        /// Evaluates every combo in the database against the current board state.
        /// Call this once each turn during the Resolve phase.
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

            // Gather board state via evaluator
            HashSet<string> activeCardIds = evaluator.CollectActiveCardIds();
            HashSet<CardTag> activeTags = evaluator.CollectActiveTags();
            string activeEventId = boardManager.ActiveEvent != null ? boardManager.ActiveEvent.cardId : null;

            // Evaluate each combo
            foreach (var combo in allCombos)
            {
                if (combo == null) continue;

                bool met = evaluator.EvaluateCombo(
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
        // Bonus Queries (delegated to ComboBonusProvider)
        // ----------------------------------------------------------------

        public int GetTotalBonusIncome() => bonusProvider.GetTotalBonusIncome();
        public int GetTotalBonusCustomers() => bonusProvider.GetTotalBonusCustomers();
        public float GetIncomeMultiplier() => bonusProvider.GetIncomeMultiplier();
        public float GetCustomerMultiplier() => bonusProvider.GetCustomerMultiplier();
        public float GetShopDiscount() => bonusProvider.GetShopDiscount();
        public int GetExtraActions() => bonusProvider.GetExtraActions();
        public int GetExtraFBIRisk() => bonusProvider.GetExtraFBIRisk();
        public int GetRivalCustomerPenalty() => bonusProvider.GetRivalCustomerPenalty();

        // ----------------------------------------------------------------
        // Queries
        // ----------------------------------------------------------------

        public bool IsComboActive(string comboId) => evaluator.IsComboActive(comboId, activeCombos);

        public List<ComboData> GetActiveCombos() => new List<ComboData>(activeCombos);

        /// <summary>
        /// Resets combo state for a new run.
        /// </summary>
        public void Reset()
        {
            activeCombos.Clear();
        }
    }
}
