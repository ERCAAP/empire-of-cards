using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Handles all rival economy calculations: income collection,
    /// customer totals, and penalty distribution.
    /// </summary>
    public class RivalEconomy
    {
        private readonly RivalData data;

        public RivalEconomy(RivalData data)
        {
            this.data = data;
        }

        /// <summary>
        /// Collects income from all rival businesses.
        /// Returns the total income collected this turn and updates rivalMoney.
        /// </summary>
        public int CollectIncome(List<RivalBusiness> businesses, ref int rivalMoney)
        {
            int totalIncome = 0;
            foreach (var biz in businesses)
            {
                totalIncome += biz.income;
            }
            rivalMoney += totalIncome;
            return totalIncome;
        }

        /// <summary>
        /// Recalculates the total number of customers across all rival businesses.
        /// </summary>
        public int CalculateRivalCustomers(List<RivalBusiness> businesses)
        {
            int total = 0;
            foreach (var biz in businesses)
            {
                total += biz.customers;
            }
            return Mathf.Max(0, total);
        }

        /// <summary>
        /// Recalculates total rival income from all businesses.
        /// </summary>
        public int CalculateRivalIncome(List<RivalBusiness> businesses)
        {
            int total = 0;
            foreach (var biz in businesses)
            {
                total += biz.income;
            }
            return total;
        }

        /// <summary>
        /// Distributes a customer penalty evenly across businesses
        /// (e.g. from player combos like Monopol: -3/turn).
        /// </summary>
        public void ApplyCustomerPenalty(List<RivalBusiness> businesses, int penalty)
        {
            if (penalty <= 0 || businesses.Count == 0) return;

            int perBiz = Mathf.Max(1, penalty / businesses.Count);
            int remaining = penalty;

            foreach (var biz in businesses)
            {
                int loss = Mathf.Min(perBiz, remaining);
                biz.customers = Mathf.Max(0, biz.customers - loss);
                remaining -= loss;
                if (remaining <= 0) break;
            }
        }

        /// <summary>
        /// Returns the total income that would be lost if production is disabled
        /// for one turn (Sabotage action card).
        /// </summary>
        public int CalculateDisabledProductionLoss(List<RivalBusiness> businesses)
        {
            int lostIncome = 0;
            foreach (var biz in businesses)
            {
                lostIncome += biz.income;
            }
            return lostIncome;
        }
    }
}
