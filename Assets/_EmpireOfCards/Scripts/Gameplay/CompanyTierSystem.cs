using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class CompanyTierSystem : MonoBehaviour
    {
        [SerializeField] private CompanyTier currentTier = CompanyTier.Esnaf;

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
            currentTier = CompanyTier.Esnaf;
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
            if (businesses >= Constants.TIER_HOLDING_BUSINESSES &&
                combos >= Constants.TIER_HOLDING_COMBOS &&
                territories >= Constants.TIER_HOLDING_TERRITORIES)
                return CompanyTier.Holding;

            if (businesses >= Constants.TIER_SIRKET_BUSINESSES &&
                combos >= Constants.TIER_SIRKET_COMBOS &&
                territories >= Constants.TIER_SIRKET_TERRITORIES)
                return CompanyTier.Sirket;

            if (businesses >= Constants.TIER_GIRISIMCI_BUSINESSES &&
                combos >= Constants.TIER_GIRISIMCI_COMBOS)
                return CompanyTier.Girisimci;

            return CompanyTier.Esnaf;
        }

        public int GetTierScoreBonus()
        {
            return currentTier switch
            {
                CompanyTier.Girisimci => Constants.TIER_SCORE_GIRISIMCI,
                CompanyTier.Sirket => Constants.TIER_SCORE_SIRKET,
                CompanyTier.Holding => Constants.TIER_SCORE_HOLDING,
                _ => 0
            };
        }
    }
}
