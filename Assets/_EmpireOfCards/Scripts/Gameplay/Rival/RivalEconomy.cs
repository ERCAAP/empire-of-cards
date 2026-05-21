using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public class RivalEconomy
    {
        private readonly RivalData data;

        public RivalEconomy(RivalData data)
        {
            this.data = data;
        }

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
        /// Market share formula (GDD):
        /// rivalCustomerShare = (quality*0.30 + price*0.20 + platformRating_norm*0.20
        ///                       + marketing*0.15 + speed*0.10 + loyalty*0.05) * 100
        ///
        /// platformRating is 1-5, normalized to 0-1 range.
        /// marketing, speed, loyalty are derived from business stats.
        /// Returns total rival customers across all businesses.
        /// </summary>
        public int CalculateRivalCustomers(List<RivalBusiness> businesses)
        {
            int total = 0;
            foreach (var biz in businesses)
            {
                float qualityNorm = Mathf.Clamp01(biz.qualityScore / 10f);
                float priceNorm = Mathf.Clamp01(biz.priceScore / 10f);
                float platformNorm = Mathf.Clamp01((biz.platformRating - 1f) / 4f);

                // marketing: employee count contributes (more staff = more marketing reach)
                float marketingScore = Mathf.Clamp01(biz.employeeCount / (float)biz.maxEmployees);

                // speed: inversely related to business size (smaller = faster)
                float speedScore = Mathf.Clamp01(1f - (biz.customers / 50f));

                // loyalty: long-term quality builds loyalty
                float loyaltyScore = Mathf.Clamp01(qualityNorm * platformNorm);

                float share = (qualityNorm * 0.30f
                             + priceNorm * 0.20f
                             + platformNorm * 0.20f
                             + marketingScore * 0.15f
                             + speedScore * 0.10f
                             + loyaltyScore * 0.05f) * 100f;

                // Blend: base customers + market share contribution
                // The formula outputs a share percentage; combine with base customers
                int marketShareCustomers = Mathf.RoundToInt(share * 0.1f);
                total += Mathf.Max(0, biz.customers + marketShareCustomers);
            }
            return Mathf.Max(0, total);
        }

        public int CalculateRivalIncome(List<RivalBusiness> businesses)
        {
            int total = 0;
            foreach (var biz in businesses)
            {
                // Base income with venture-specific quality multiplier
                float qualityMult = 1f + (biz.qualityScore - 5f) * 0.05f;
                total += Mathf.RoundToInt(biz.income * Mathf.Max(0.5f, qualityMult));
            }
            return total;
        }

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
