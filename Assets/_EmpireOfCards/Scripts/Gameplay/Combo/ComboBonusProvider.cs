using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay.Combo
{
    /// <summary>
    /// Query methods over active combos extracted from ComboSystem.
    /// Provides all bonus calculations from currently active combos.
    /// </summary>
    public class ComboBonusProvider
    {
        private readonly IReadOnlyList<ComboData> activeCombos;

        public ComboBonusProvider(IReadOnlyList<ComboData> activeCombos)
        {
            this.activeCombos = activeCombos;
        }

        /// <summary>
        /// Returns total bonus income from all active combos.
        /// </summary>
        public int GetTotalBonusIncome()
        {
            int total = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    total += combo.bonusIncome;
            }
            return total;
        }

        /// <summary>
        /// Returns total bonus customers from all active combos.
        /// </summary>
        public int GetTotalBonusCustomers()
        {
            int total = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    total += combo.bonusCustomers;
            }
            return total;
        }

        /// <summary>
        /// Returns the combined income multiplier from active combos.
        /// Base is 1.0; combos multiply on top.
        /// </summary>
        public float GetIncomeMultiplier()
        {
            float multiplier = 1f;
            foreach (var combo in activeCombos)
            {
                if (combo != null && combo.incomeMultiplier > 1f)
                    multiplier *= combo.incomeMultiplier;
            }
            return multiplier;
        }

        /// <summary>
        /// Returns the combined customer multiplier from active combos.
        /// </summary>
        public float GetCustomerMultiplier()
        {
            float multiplier = 1f;
            foreach (var combo in activeCombos)
            {
                if (combo != null && combo.customerMultiplier > 1f)
                    multiplier *= combo.customerMultiplier;
            }
            return multiplier;
        }

        /// <summary>
        /// Returns the shop discount rate from active combos (Kriz Avcisi: 50%).
        /// </summary>
        public float GetShopDiscount()
        {
            float discount = 0f;
            foreach (var combo in activeCombos)
            {
                if (combo != null && combo.shopDiscount > 0f)
                    discount = Mathf.Max(discount, combo.shopDiscount);
            }
            return discount;
        }

        /// <summary>
        /// Returns the extra action count from active combos (AI Devrimi: +1).
        /// </summary>
        public int GetExtraActions()
        {
            int extra = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    extra += combo.extraActions;
            }
            return extra;
        }

        /// <summary>
        /// Returns extra FBI risk from active combos (Yeralti: +8).
        /// </summary>
        public int GetExtraFBIRisk()
        {
            int risk = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    risk += combo.extraFBIRisk;
            }
            return risk;
        }

        /// <summary>
        /// Returns the total rival customer penalty from active combos (Monopol: -3/turn).
        /// </summary>
        public int GetRivalCustomerPenalty()
        {
            int penalty = 0;
            foreach (var combo in activeCombos)
            {
                if (combo != null)
                    penalty += combo.rivalCustomerPenalty;
            }
            return penalty;
        }
    }
}
