using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Checks for active combos each turn and applies their bonuses.
    /// </summary>
    public class ComboSystem : MonoBehaviour
    {
        // --- Data ---
        [Header("Combo Database")]
        [SerializeField] private ComboData[] allCombos;

        // --- Runtime State ---
        [Header("Active Combos (Read Only)")]
        [SerializeField] private List<ComboData> activeCombos = new List<ComboData>();

        // --- Events ---
        public event Action<ComboData> OnComboTriggered;
        public event Action<ComboData> OnComboDeactivated;

        // --- Properties ---
        public IReadOnlyList<ComboData> ActiveCombos => activeCombos;

        /// <summary>
        /// Evaluates every combo in the database against the current game state.
        /// Call this once each turn during the Resolve phase.
        /// </summary>
        public void CheckCombos(
            List<ActiveBusiness> businesses,
            List<CardData> activeUpgrades,
            string activeEventId,
            int playerMoney,
            int playerTerritories)
        {
            List<ComboData> previouslyActive = new List<ComboData>(activeCombos);
            activeCombos.Clear();

            foreach (var combo in allCombos)
            {
                if (combo == null)
                    continue;

                if (IsComboActive(combo, businesses, activeUpgrades, activeEventId, playerMoney, playerTerritories))
                {
                    activeCombos.Add(combo);

                    if (!previouslyActive.Contains(combo))
                    {
                        TriggerCombo(combo);
                    }
                }
            }

            // Fire deactivation events for combos that are no longer active
            foreach (var prev in previouslyActive)
            {
                if (!activeCombos.Contains(prev))
                {
                    OnComboDeactivated?.Invoke(prev);
                }
            }
        }

        /// <summary>
        /// Checks whether all requirements for a specific combo are met.
        /// </summary>
        public bool IsComboActive(
            ComboData combo,
            List<ActiveBusiness> businesses,
            List<CardData> activeUpgrades,
            string activeEventId,
            int playerMoney,
            int playerTerritories)
        {
            if (combo == null)
                return false;

            // Collect all active card IDs for requirement matching
            HashSet<string> activeCardIds = new HashSet<string>();
            HashSet<string> activeTags = new HashSet<string>();

            foreach (var business in businesses)
            {
                if (business.isClosed)
                    continue;

                if (business.businessCard != null)
                {
                    activeCardIds.Add(business.businessCard.cardId);
                    AddTags(activeTags, business.businessCard.tags);
                }

                foreach (var employee in business.employees)
                {
                    if (employee != null)
                    {
                        activeCardIds.Add(employee.cardId);
                        AddTags(activeTags, employee.tags);
                    }
                }

                foreach (var upgrade in business.upgrades)
                {
                    if (upgrade != null)
                    {
                        activeCardIds.Add(upgrade.cardId);
                        AddTags(activeTags, upgrade.tags);
                    }
                }
            }

            foreach (var upgrade in activeUpgrades)
            {
                if (upgrade != null)
                {
                    activeCardIds.Add(upgrade.cardId);
                    AddTags(activeTags, upgrade.tags);
                }
            }

            // Check required card IDs
            if (combo.requiredCardIds != null)
            {
                foreach (string requiredId in combo.requiredCardIds)
                {
                    if (!activeCardIds.Contains(requiredId))
                        return false;
                }
            }

            // Check required tags
            if (combo.requiredTags != null)
            {
                foreach (string requiredTag in combo.requiredTags)
                {
                    if (!activeTags.Contains(requiredTag))
                        return false;
                }
            }

            // Check required event
            if (!string.IsNullOrEmpty(combo.requiredEventId))
            {
                if (activeEventId != combo.requiredEventId)
                    return false;
            }

            // Check minimum money requirement
            if (combo.minMoneyRequired > 0 && playerMoney < combo.minMoneyRequired)
                return false;

            // Check minimum territory requirement
            if (combo.minTerritoriesRequired > 0 && playerTerritories < combo.minTerritoriesRequired)
                return false;

            return true;
        }

        /// <summary>
        /// Applies the combo bonuses and fires the triggered event.
        /// </summary>
        public void TriggerCombo(ComboData combo)
        {
            if (combo == null)
                return;

            Debug.Log($"[ComboSystem] Combo triggered: {combo.comboName}");

            // Bonus application is handled by the systems that read active combos
            // (e.g., EconomyManager checks for income bonuses, ShopManager checks for discounts)
            OnComboTriggered?.Invoke(combo);
        }

        /// <summary>
        /// Returns the current list of active combos.
        /// </summary>
        public List<ComboData> GetActiveCombos()
        {
            return new List<ComboData>(activeCombos);
        }

        private void AddTags(HashSet<string> tagSet, string[] tags)
        {
            if (tags == null)
                return;

            foreach (string tag in tags)
            {
                tagSet.Add(tag);
            }
        }
    }
}
