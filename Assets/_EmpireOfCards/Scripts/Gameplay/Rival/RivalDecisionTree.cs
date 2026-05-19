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
            int rivalTerritories,
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

            // Patent Wall: if player has this upgrade, rival costs increase by 25%
            float patentWallMultiplier = 1f;
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.BoardManager != null)
            {
                foreach (var upgrade in gm.BoardManager.GlobalUpgrades)
                {
                    if (upgrade != null && upgrade.upgradeEffectType == UpgradeEffectType.RivalCostIncrease)
                    {
                        patentWallMultiplier = 1.25f;
                        break;
                    }
                }
            }

            // 2. Rival money >= business cost AND < 3 businesses => OPEN BUSINESS
            int effectiveBusinessCost = (int)(data.businessCostThreshold * patentWallMultiplier);
            if (rivalMoney >= effectiveBusinessCost && businessCount < 3)
            {
                return "open_business";
            }

            // 3. Empty employee slots => HIRE
            int effectiveHireCost = (int)(data.hireCostThreshold * patentWallMultiplier);
            if (hasEmptyEmployeeSlots && rivalMoney >= effectiveHireCost)
            {
                return "hire_employee";
            }

            // 4. Event benefits rival => EVENT BONUS
            if (currentTurn >= 12 && aggressionEnabled)
            {
                if (rivalTerritories < playerTerritories)
                    return "event_bonus";
            }

            // 5. Default => NORMAL GROWTH
            return "normal_growth";
        }
    }
}
