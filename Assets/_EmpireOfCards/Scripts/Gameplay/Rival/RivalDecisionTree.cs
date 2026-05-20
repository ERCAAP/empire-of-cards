using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class RivalDecisionTree
    {
        private readonly RivalData data;

        public RivalDecisionTree(RivalData data)
        {
            this.data = data;
        }

        /// <summary>
        /// New decision method returning a typed RivalMove.
        /// Evaluates rival state and picks the best move.
        /// </summary>
        public RivalMove DecideMove(
            int playerBlocks,
            int rivalBlocks,
            int currentTurn,
            int rivalMoney,
            List<RivalBusiness> rivalBusinesses,
            bool aggressionEnabled)
        {
            if (data == null) return RivalMove.QualityImprove;

            int businessCount = rivalBusinesses.Count;
            bool hasFunds = rivalMoney >= data.businessCostThreshold;
            bool rivalBehind = rivalBlocks < playerBlocks;
            float avgQuality = CalculateAverageQuality(rivalBusinesses);
            float avgPrice = CalculateAveragePrice(rivalBusinesses);
            int avgLegalRisk = CalculateAverageLegalRisk(rivalBusinesses);

            // High aggression: player is way ahead and aggression enabled
            if (aggressionEnabled && playerBlocks > 5)
            {
                // High legal risk -> avoid Sabotage
                if (avgLegalRisk < 50)
                    return RivalMove.Sabotage;
                else
                    return RivalMove.StaffPoach;
            }

            // Rival behind on territories: aggressive economic moves
            if (rivalBehind && aggressionEnabled)
            {
                if (avgPrice < 6f)
                    return RivalMove.PriceWar;
                if (avgQuality < 6f)
                    return RivalMove.QualityImprove;
                return RivalMove.MarketingBlitz;
            }

            // Expand if we have money and room
            if (hasFunds && businessCount < data.maxBusinesses)
                return RivalMove.OpenBranch;

            // Need funds
            if (rivalMoney < data.hireCostThreshold)
                return RivalMove.SeekInvestment;

            // Late game: push on marketing
            if (currentTurn >= 12)
                return RivalMove.MarketingBlitz;

            // Quality is low: improve
            if (avgQuality < 5f)
                return RivalMove.QualityImprove;

            // Price is low: price war
            if (avgPrice < 5f)
                return RivalMove.PriceWar;

            // Default: balanced improvement
            return RivalMove.QualityImprove;
        }

        /// <summary>
        /// Legacy decision method. Returns a string action for backward compatibility.
        /// </summary>
        public string DecideAction(
            int playerBlocks,
            int rivalBlocks,
            int currentTurn,
            int rivalMoney,
            int businessCount,
            bool aggressionEnabled,
            bool hasEmptyEmployeeSlots)
        {
            if (data == null) return "normal_growth";

            if (playerBlocks > 5 && aggressionEnabled)
                return "aggressive";

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

            int effectiveBusinessCost = (int)(data.businessCostThreshold * patentWallMultiplier);
            if (rivalMoney >= effectiveBusinessCost && businessCount < 3)
                return "open_business";

            int effectiveHireCost = (int)(data.hireCostThreshold * patentWallMultiplier);
            if (hasEmptyEmployeeSlots && rivalMoney >= effectiveHireCost)
                return "hire_employee";

            if (currentTurn >= 12 && aggressionEnabled)
            {
                if (rivalBlocks < playerBlocks)
                    return "event_bonus";
            }

            return "normal_growth";
        }

        private float CalculateAverageQuality(List<RivalBusiness> businesses)
        {
            if (businesses.Count == 0) return 5f;
            float total = 0f;
            foreach (var biz in businesses)
                total += biz.qualityScore;
            return total / businesses.Count;
        }

        private float CalculateAveragePrice(List<RivalBusiness> businesses)
        {
            if (businesses.Count == 0) return 5f;
            float total = 0f;
            foreach (var biz in businesses)
                total += biz.priceScore;
            return total / businesses.Count;
        }

        private int CalculateAverageLegalRisk(List<RivalBusiness> businesses)
        {
            if (businesses.Count == 0) return 0;
            int total = 0;
            foreach (var biz in businesses)
                total += biz.legalRisk;
            return total / businesses.Count;
        }
    }
}
