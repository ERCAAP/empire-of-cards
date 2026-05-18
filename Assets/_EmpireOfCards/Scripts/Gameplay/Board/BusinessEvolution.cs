using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Gameplay.Board
{
    /// <summary>
    /// Handles business evolution logic (GDD Section 3.1).
    /// Tracks totalCustomersAttracted and turnsActive to determine
    /// when a business qualifies for evolution:
    ///   Diner(50/3) -> Restaurant(80/5) -> Chain(120/8)
    /// </summary>
    public class BusinessEvolution
    {
        /// <summary>
        /// Adds customers attracted this turn to a business's evolution tracker.
        /// Called from EconomyManager during resolve.
        /// </summary>
        public void AddCustomersAttracted(ActiveBusiness business, int customers)
        {
            if (business == null) return;
            business.totalCustomersAttracted += customers;
        }

        /// <summary>
        /// Checks if a business meets evolution requirements:
        /// - 40+ total customers attracted (or card-specific override)
        /// - 15+ turns active (or card-specific override)
        /// - Business card has canEvolve = true and evolvedForm != null
        /// On evolution: replaces business card with evolved form,
        /// advances BusinessLevel, and resets tracking for next tier.
        /// </summary>
        public bool CheckEvolution(int businessIndex, ActiveBusiness business)
        {
            if (business.businessCard == null) return false;
            if (!business.businessCard.canEvolve) return false;
            if (business.businessCard.evolvedForm == null) return false;

            int customerReq = business.businessCard.evolutionCustomerReq > 0
                ? business.businessCard.evolutionCustomerReq
                : Constants.EVOLUTION_CUSTOMER_THRESHOLD;

            int turnReq = business.businessCard.evolutionTurnReq > 0
                ? business.businessCard.evolutionTurnReq
                : Constants.EVOLUTION_TURN_REQUIREMENT;

            if (business.totalCustomersAttracted >= customerReq && business.turnsActive >= turnReq)
            {
                EvolveBusiness(businessIndex, business);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs the actual evolution: swaps the card, advances the level,
        /// resets tracking counters, and fires the event.
        /// </summary>
        private void EvolveBusiness(int businessIndex, ActiveBusiness business)
        {
            CardData oldCard = business.businessCard;
            business.businessCard = oldCard.evolvedForm;

            // Advance level
            if (business.currentLevel == BusinessLevel.Level1)
                business.currentLevel = BusinessLevel.Level2;
            else if (business.currentLevel == BusinessLevel.Level2)
                business.currentLevel = BusinessLevel.Level3;

            // Reset tracking for next evolution
            business.totalCustomersAttracted = 0;
            business.turnsActive = 0;

            Debug.Log($"[BusinessEvolution] Business evolved! {oldCard.cardName} -> {business.businessCard.cardName}");
            EventBus.BusinessEvolved(businessIndex, business.currentLevel);
        }
    }
}
