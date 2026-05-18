using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay.Economy
{
    /// <summary>
    /// Calculates tax based on gross income and accountant count.
    /// Base: 15%. 1 muhasebeci: 7.5%. 2+ muhasebeci: 3%.
    /// </summary>
    public class TaxCalculator
    {
        private readonly GameBalanceData balanceData;

        public TaxCalculator(GameBalanceData balanceData)
        {
            this.balanceData = balanceData;
        }

        /// <summary>
        /// Applies the tax rate to gross income.
        /// Base: 15%. 1 muhasebeci: 7.5%. 2+ muhasebeci: 3%.
        /// Tax rate values come from GameBalanceData.
        /// </summary>
        public int CalculateTax(int income, int accountantCount)
        {
            float effectiveRate;

            if (balanceData != null)
            {
                if (accountantCount >= 2)
                    effectiveRate = balanceData.minTaxRate;      // 3%
                else if (accountantCount == 1)
                    effectiveRate = balanceData.reducedTaxRate;  // 7.5%
                else
                    effectiveRate = balanceData.taxRate;         // 15%
            }
            else
            {
                // Fallback to Constants
                if (accountantCount >= 2)
                    effectiveRate = Constants.MIN_TAX_RATE;
                else if (accountantCount == 1)
                    effectiveRate = Constants.REDUCED_TAX_RATE;
                else
                    effectiveRate = Constants.TAX_RATE;
            }

            return Mathf.RoundToInt(income * effectiveRate);
        }

        /// <summary>
        /// Counts how many accountant employees (muhasebeci) are on the board.
        /// Uses the taxReduction field: any employee with taxReduction > 0 is an accountant.
        /// </summary>
        public int CountAccountants(IReadOnlyList<ActiveBusiness> businesses)
        {
            int count = 0;
            foreach (var biz in businesses)
            {
                if (biz.isClosed) continue;
                foreach (var emp in biz.employees)
                {
                    if (emp != null && emp.taxReduction > 0f)
                        count++;
                }
            }
            return count;
        }
    }
}
