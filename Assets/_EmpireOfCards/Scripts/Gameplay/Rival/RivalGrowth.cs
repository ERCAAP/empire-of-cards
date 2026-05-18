using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Handles rival growth actions: opening businesses, hiring employees,
    /// aggressive moves, normal passive growth, and milestone catch-up (GDD 8.3).
    /// </summary>
    public class RivalGrowth
    {
        private readonly RivalData data;

        public RivalGrowth(RivalData data)
        {
            this.data = data;
        }

        // ----------------------------------------------------------------
        // Actions
        // ----------------------------------------------------------------

        /// <summary>
        /// Opens a new rival business. Costs businessCostThreshold money.
        /// </summary>
        public void OpenBusiness(
            List<RivalBusiness> businesses,
            ref int rivalMoney)
        {
            if (data == null) return;
            if (businesses.Count >= data.maxBusinesses) return;

            int cost = data.businessCostThreshold;
            if (rivalMoney < cost) return;

            rivalMoney -= cost;

            string businessName = PickRandomBusinessName("Rival Business");

            RivalBusiness newBiz = new RivalBusiness
            {
                name = businessName,
                income = data.baseBusinessIncome,
                customers = data.baseBusinessCustomers,
                employeeCount = 0,
                maxEmployees = data.maxEmployeesPerBusiness
            };

            businesses.Add(newBiz);
            EventBus.RivalActed($"Rival opened a new business: {businessName}");
        }

        /// <summary>
        /// Hires an employee for the business with the fewest employees.
        /// </summary>
        public void HireEmployee(
            List<RivalBusiness> businesses,
            ref int rivalMoney,
            ref int totalRivalEmployees)
        {
            if (data == null) return;

            int cost = data.hireCostThreshold;
            if (rivalMoney < cost) return;

            RivalBusiness target = FindBusinessWithFewestEmployees(businesses);
            if (target == null) return;

            rivalMoney -= cost;
            target.employeeCount++;
            target.income += data.employeeIncomeBoost;
            target.customers += data.employeeCustomerBoost;
            totalRivalEmployees++;

            EventBus.RivalActed($"Rival hired an employee for {target.name}.");
        }

        private const float AGGRESSIVE_INVESTMENT_RATE = 0.3f;

        /// <summary>
        /// Aggressive action: invests 30% of money into the strongest business.
        /// </summary>
        public void AggressiveAction(
            List<RivalBusiness> businesses,
            ref int rivalMoney)
        {
            if (data == null || businesses.Count == 0) return;

            RivalBusiness strongest = businesses[0];
            foreach (var biz in businesses)
            {
                if (biz.customers > strongest.customers)
                    strongest = biz;
            }

            int investmentCost = Mathf.RoundToInt(rivalMoney * AGGRESSIVE_INVESTMENT_RATE);
            if (investmentCost <= 0) return;

            rivalMoney -= investmentCost;
            strongest.customers += data.aggressiveCustomerBoost;
            strongest.income += data.aggressiveIncomeBoost;

            EventBus.RivalActed($"Rival made an aggressive move: {strongest.name}!");
        }

        /// <summary>
        /// Normal growth: +passiveMoneyGrowth money, +passiveCustomerGrowth customers per business.
        /// </summary>
        public void NormalGrowth(
            List<RivalBusiness> businesses,
            ref int rivalMoney)
        {
            int moneyGrowth = data != null ? data.passiveMoneyGrowth : 50;
            rivalMoney += moneyGrowth;

            int customerGrowth = data != null ? data.passiveCustomerGrowth : 2;
            foreach (var biz in businesses)
            {
                biz.customers += customerGrowth;
            }

            EventBus.RivalActed("Rival businesses grew steadily.");
        }

        /// <summary>
        /// Event bonus: moderate boost to all businesses.
        /// </summary>
        public void EventBonusAction(List<RivalBusiness> businesses)
        {
            int custBonus = data != null ? data.eventBonusCustomers : 3;
            int incBonus = data != null ? data.eventBonusIncome : 15;

            foreach (var biz in businesses)
            {
                biz.customers += custBonus;
                biz.income += incBonus;
            }

            EventBus.RivalActed("Rakip event bonusunu kullandi.");
        }

        // ----------------------------------------------------------------
        // Milestone Catch-up (GDD Section 8.3)
        // ----------------------------------------------------------------

        /// <summary>
        /// Checks growth milestones and ensures the rival catches up if behind.
        /// Returns true if aggression was enabled by a milestone.
        /// </summary>
        public bool ApplyGrowthMilestones(
            int currentTurn,
            List<RivalBusiness> businesses,
            ref int totalRivalEmployees)
        {
            if (data == null || data.growthMilestones == null) return false;

            RivalMilestone currentMilestone = null;

            foreach (var milestone in data.growthMilestones)
            {
                if (currentTurn >= milestone.turn)
                    currentMilestone = milestone;
            }

            if (currentMilestone == null) return false;

            bool aggressionEnabled = currentMilestone.enableAggression;

            // Catch up businesses if behind target
            while (businesses.Count < currentMilestone.targetBusinesses &&
                   businesses.Count < data.maxBusinesses)
            {
                string bizName = PickRandomBusinessName("Milestone Business");

                businesses.Add(new RivalBusiness
                {
                    name = bizName,
                    income = data.baseBusinessIncome,
                    customers = data.baseBusinessCustomers,
                    employeeCount = 1,
                    maxEmployees = data.maxEmployeesPerBusiness
                });

                totalRivalEmployees++;
            }

            // Catch up employees if behind target
            int employeeDeficit = currentMilestone.targetEmployees - totalRivalEmployees;
            for (int i = 0; i < employeeDeficit; i++)
            {
                RivalBusiness target = FindBusinessWithFewestEmployees(businesses);
                if (target == null) break;

                target.employeeCount++;
                target.income += data.employeeIncomeBoost;
                target.customers += data.employeeCustomerBoost;
                totalRivalEmployees++;
            }

            return aggressionEnabled;
        }

        /// <summary>
        /// Closes the weakest rival business (player action cards like Dusmanca Devralma).
        /// </summary>
        public void CloseWeakestBusiness(
            List<RivalBusiness> businesses,
            ref int totalRivalEmployees)
        {
            if (businesses.Count <= 1) return;

            int weakestIdx = 0;
            int weakestCustomers = int.MaxValue;

            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].customers < weakestCustomers)
                {
                    weakestCustomers = businesses[i].customers;
                    weakestIdx = i;
                }
            }

            RivalBusiness removed = businesses[weakestIdx];
            totalRivalEmployees -= removed.employeeCount;
            businesses.RemoveAt(weakestIdx);

            EventBus.RivalActed($"Rival's weakest business was shut down: {removed.name}");
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns true if any business has room for more employees.
        /// </summary>
        public bool HasEmptyEmployeeSlots(List<RivalBusiness> businesses)
        {
            foreach (var biz in businesses)
            {
                if (biz.employeeCount < biz.maxEmployees)
                    return true;
            }
            return false;
        }

        private RivalBusiness FindBusinessWithFewestEmployees(List<RivalBusiness> businesses)
        {
            RivalBusiness target = null;
            int minEmployees = int.MaxValue;

            foreach (var biz in businesses)
            {
                if (biz.employeeCount < biz.maxEmployees && biz.employeeCount < minEmployees)
                {
                    minEmployees = biz.employeeCount;
                    target = biz;
                }
            }

            return target;
        }

        private string PickRandomBusinessName(string fallback)
        {
            if (data.possibleBusinessNames != null && data.possibleBusinessNames.Length > 0)
            {
                return data.possibleBusinessNames[
                    Random.Range(0, data.possibleBusinessNames.Length)];
            }
            return fallback;
        }
    }
}
