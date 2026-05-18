using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Handles all money, tax, income, and salary calculations each turn.
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        // --- Data Reference ---
        [Header("Balance Data")]
        [SerializeField] private GameBalanceData balanceData;

        // --- Runtime State ---
        [Header("Turn Summary (Read Only)")]
        [SerializeField] private int grossIncome;
        [SerializeField] private int totalSalaries;
        [SerializeField] private int taxAmount;
        [SerializeField] private int netIncome;

        // --- Events ---
        public event Action<int> OnIncomeCalculated;
        public event Action<int> OnSalariesPaid;
        public event Action<int> OnTaxCollected;

        // --- Properties ---
        public int GrossIncome => grossIncome;
        public int TotalSalaries => totalSalaries;
        public int TaxAmount => taxAmount;
        public int NetIncome => netIncome;

        /// <summary>
        /// Sums up all business income including employee bonuses and upgrade multipliers.
        /// </summary>
        public int CalculateTurnIncome(List<ActiveBusiness> businesses)
        {
            int total = 0;

            foreach (var business in businesses)
            {
                if (business.isClosed)
                    continue;

                int businessIncome = business.businessCard.income;

                // Add employee income bonuses
                foreach (var employee in business.employees)
                {
                    businessIncome += employee.income;
                }

                // Apply upgrade multipliers
                float upgradeMultiplier = 1f;
                foreach (var upgrade in business.upgrades)
                {
                    upgradeMultiplier += upgrade.incomeMultiplier;
                }

                businessIncome = Mathf.RoundToInt(businessIncome * upgradeMultiplier);
                total += businessIncome;
            }

            grossIncome = total;
            OnIncomeCalculated?.Invoke(grossIncome);
            return grossIncome;
        }

        /// <summary>
        /// Sums all employee salaries across every active business.
        /// </summary>
        public int CalculateSalaries(List<ActiveBusiness> businesses)
        {
            int total = 0;

            foreach (var business in businesses)
            {
                if (business.isClosed)
                    continue;

                foreach (var employee in business.employees)
                {
                    total += employee.salary;
                }
            }

            totalSalaries = total;
            OnSalariesPaid?.Invoke(totalSalaries);
            return totalSalaries;
        }

        /// <summary>
        /// Applies the tax rate to gross income. Accountants reduce the effective rate.
        /// </summary>
        public int CalculateTax(int income, bool hasAccountant, int accountantCount)
        {
            if (balanceData == null)
            {
                Debug.LogWarning("[EconomyManager] balanceData is null, using default tax rate.");
                taxAmount = Mathf.RoundToInt(income * 0.2f);
                OnTaxCollected?.Invoke(taxAmount);
                return taxAmount;
            }

            float effectiveRate = balanceData.baseTaxRate;

            if (hasAccountant && accountantCount > 0)
            {
                float reduction = accountantCount * balanceData.accountantTaxReduction;
                effectiveRate = Mathf.Max(0f, effectiveRate - reduction);
            }

            taxAmount = Mathf.RoundToInt(income * effectiveRate);
            OnTaxCollected?.Invoke(taxAmount);
            return taxAmount;
        }

        /// <summary>
        /// Runs the full end-of-turn economy pipeline: income -> salaries -> tax -> net.
        /// Call this from TurnManager during the Resolve phase.
        /// </summary>
        public void ProcessEndOfTurn(List<ActiveBusiness> businesses, bool hasAccountant, int accountantCount)
        {
            CalculateTurnIncome(businesses);
            CalculateSalaries(businesses);
            CalculateTax(grossIncome, hasAccountant, accountantCount);

            netIncome = grossIncome - totalSalaries - taxAmount;

            if (netIncome > 0)
            {
                GameManager.Instance.GainMoney(netIncome);
            }
            else if (netIncome < 0)
            {
                GameManager.Instance.SpendMoney(Mathf.Abs(netIncome));
            }
        }

        /// <summary>
        /// Returns the sell price of a card based on its buy cost and the balance sell rate.
        /// </summary>
        public int GetSellPrice(CardData card)
        {
            if (card == null)
                return 0;

            float sellRate = balanceData != null ? balanceData.sellRate : 0.5f;
            return Mathf.RoundToInt(card.buyCost * sellRate);
        }

        /// <summary>
        /// Returns the total customer market pool size based on the current turn.
        /// The pool grows as the game progresses.
        /// </summary>
        public int CalculateMarketPool(int currentTurn)
        {
            if (balanceData == null)
            {
                Debug.LogWarning("[EconomyManager] balanceData is null, using default market pool.");
                return 100 + (currentTurn * 20);
            }

            return balanceData.baseMarketPool + (currentTurn * balanceData.marketGrowthPerTurn);
        }
    }
}
