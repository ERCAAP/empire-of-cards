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
        /// Evaluates board state and promotes tier if conditions met.
        /// Tier never goes down.
        /// </summary>
        public void EvaluateTier(int playerTerritories)
        {
            if (_board == null || _combo == null) return;

            int activeBusinesses = _board.GetActiveBusinessCount();
            int activeCombos = _combo.ActiveComboCount;

            CompanyTier newTier = CalculateTier(activeBusinesses, activeCombos, playerTerritories);

            if (newTier > currentTier)
            {
                currentTier = newTier;
                EventBus.CompanyTierChanged(currentTier);
                Debug.Log($"[CompanyTierSystem] Promoted to {currentTier}!");
            }
        }

        private CompanyTier CalculateTier(int businesses, int combos, int territories)
        {
            // Check from highest to lowest
            if (businesses >= Constants.TIER_CONGLOMERATE_BUSINESSES &&
                combos >= Constants.TIER_CONGLOMERATE_COMBOS &&
                territories >= Constants.TIER_CONGLOMERATE_TERRITORIES)
                return CompanyTier.Conglomerate;

            if (businesses >= Constants.TIER_CORPORATION_BUSINESSES &&
                combos >= Constants.TIER_CORPORATION_COMBOS &&
                territories >= Constants.TIER_CORPORATION_TERRITORIES)
                return CompanyTier.Corporation;

            if (businesses >= Constants.TIER_ENTREPRENEUR_BUSINESSES &&
                combos >= Constants.TIER_ENTREPRENEUR_COMBOS)
                return CompanyTier.Entrepreneur;

            return CompanyTier.Trader;
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
