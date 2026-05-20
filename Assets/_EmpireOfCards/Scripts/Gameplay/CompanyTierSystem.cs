using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class CompanyTierSystem : MonoBehaviour
    {
        [SerializeField] private CompanyTier currentTier = CompanyTier.Trader;

        private BoardManager _board;
        private ComboSystem _combo;

        public CompanyTier CurrentTier => currentTier;

        public void Init(BoardManager board, ComboSystem combo)
        {
            _board = board;
            _combo = combo;
        }

        public void Reset()
        {
            currentTier = CompanyTier.Trader;
        }

        /// <summary>
        /// Called at end of Resolve Phase, after combo check.
        /// Evaluates customer count + combos + occupancy and promotes tier if conditions met.
        /// Tier never goes down. On tier-up, fires slot expansion via BoardManager.
        /// </summary>
        public void EvaluateTier(int playerCustomers)
        {
            if (_board == null || _combo == null) return;

            int activeCombos = _combo.ActiveComboCount;
            float operationOccupancy = GetOperationOccupancy();

            CompanyTier newTier = CalculateTier(playerCustomers, activeCombos, operationOccupancy);

            if (newTier > currentTier)
            {
                CompanyTier oldTier = currentTier;
                currentTier = newTier;
                ApplySlotExpansion(oldTier, newTier);
                EventBus.CompanyTierChanged(currentTier);
                Debug.Log($"[CompanyTierSystem] Promoted to {currentTier}!");
            }
        }

        private CompanyTier CalculateTier(int customers, int combos, float operationOccupancy)
        {
            // Check from highest to lowest
            if (customers >= Constants.TIER_CONGLOMERATE_CUSTOMERS &&
                combos >= Constants.TIER_CONGLOMERATE_COMBOS)
                return CompanyTier.Conglomerate;

            if (customers >= Constants.TIER_CORPORATION_CUSTOMERS &&
                combos >= Constants.TIER_CORPORATION_COMBOS &&
                operationOccupancy >= Constants.TIER_CORPORATION_OPERATION_OCCUPANCY)
                return CompanyTier.Corporation;

            if (customers >= Constants.TIER_ENTREPRENEUR_CUSTOMERS &&
                combos >= Constants.TIER_ENTREPRENEUR_COMBOS)
                return CompanyTier.Entrepreneur;

            return CompanyTier.Trader;
        }

        /// <summary>
        /// Returns the ratio of active (non-closed) businesses to max slots.
        /// Used as a proxy for Operation slot occupancy until SlotManager is wired.
        /// </summary>
        private float GetOperationOccupancy()
        {
            if (_board == null || _board.MaxSlots <= 0) return 0f;
            return (float)_board.GetActiveBusinessCount() / _board.MaxSlots;
        }

        /// <summary>
        /// Applies slot expansion rewards when tier changes (GDD Section 4.5).
        /// Tier 1->2: +1 business slot
        /// Tier 2->3: +2 business slots
        /// Tier 3->4: +1 business slot
        /// Uses BoardManager.SetMaxSlots via GameManager until SlotManager is wired.
        /// </summary>
        private void ApplySlotExpansion(CompanyTier oldTier, CompanyTier newTier)
        {
            var gm = GameManager.Instance;
            if (gm == null) return;

            // Apply all tier transitions that were skipped (e.g., jumping Trader -> Corporation)
            if (oldTier < CompanyTier.Entrepreneur && newTier >= CompanyTier.Entrepreneur)
            {
                gm.AddBusinessSlot(); // Tier 1->2: +1
                Debug.Log("[CompanyTierSystem] Tier 1->2 expansion: +1 business slot");
            }

            if (oldTier < CompanyTier.Corporation && newTier >= CompanyTier.Corporation)
            {
                gm.AddBusinessSlot(); // Tier 2->3: +2
                gm.AddBusinessSlot();
                Debug.Log("[CompanyTierSystem] Tier 2->3 expansion: +2 business slots");
            }

            if (oldTier < CompanyTier.Conglomerate && newTier >= CompanyTier.Conglomerate)
            {
                gm.AddBusinessSlot(); // Tier 3->4: +1
                Debug.Log("[CompanyTierSystem] Tier 3->4 expansion: +1 business slot");
            }
        }

        public int GetTierScoreBonus()
        {
            return currentTier switch
            {
                CompanyTier.Entrepreneur => Constants.TIER_SCORE_ENTREPRENEUR,
                CompanyTier.Corporation => Constants.TIER_SCORE_CORPORATION,
                CompanyTier.Conglomerate => Constants.TIER_SCORE_CONGLOMERATE,
                _ => 0
            };
        }
    }
}
