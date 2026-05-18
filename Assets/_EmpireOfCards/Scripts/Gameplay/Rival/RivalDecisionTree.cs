using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Pure decision logic for the rival AI (GDD Section 8).
    /// Stateless: takes game state in, returns an action string.
    ///
    /// Decision tree:
    /// 1. Player territories > 5 AND aggression enabled => "aggressive"
    /// 2. Rival money >= business cost AND less than 3 businesses => "open_business"
    /// 3. Empty employee slots AND enough money => "hire_employee"
    /// 4. Late game, aggression enabled, rival behind => "event_bonus"
    /// 5. Default => "normal_growth"
    /// </summary>
    public class RivalDecisionTree
    {
        private readonly RivalData data;

        public RivalDecisionTree(RivalData data)
        {
            this.data = data;
        }

        /// <summary>
        /// Evaluates the decision tree and returns the action the rival should take.
        /// </summary>
        public string DecideAction(
            int playerTerritories,
            int currentTurn,
            int rivalMoney,
            int businessCount,
            bool aggressionEnabled,
            bool hasEmptyEmployeeSlots)
        {
            if (data == null) return "normal_growth";

            // 1. Player territories > 5 => AGGRESSIVE
            if (playerTerritories > 5 && aggressionEnabled)
            {
                return "aggressive";
            }

            // 2. Rival money >= business cost AND < 3 businesses => OPEN BUSINESS
            if (rivalMoney >= data.businessCostThreshold && businessCount < 3)
            {
                return "open_business";
            }

            // 3. Empty employee slots => HIRE
            if (hasEmptyEmployeeSlots && rivalMoney >= data.hireCostThreshold)
            {
                return "hire_employee";
            }

            // 4. Event benefits rival => EVENT BONUS
            if (currentTurn >= 12 && aggressionEnabled)
            {
                int rivalTerritories = GameManager.Instance != null
                    ? GameManager.Instance.RivalTerritories : 0;
                if (rivalTerritories < playerTerritories)
                    return "event_bonus";
            }

            // 5. Default => NORMAL GROWTH (+50 money, +2 customers)
            return "normal_growth";
        }
    }
}
