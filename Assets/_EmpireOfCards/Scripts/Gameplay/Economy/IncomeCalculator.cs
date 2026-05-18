using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Gameplay.Economy
{
    /// <summary>
    /// Calculates total turn income from all businesses.
    /// Handles: base income, employee bonuses, synergy, upgrade multipliers,
    /// combo bonuses, illegal income, event effects, trend bonuses,
    /// random income (Crypto), food bonus (Organic Farm), global customer bonus (Ad Agency).
    /// </summary>
    public class IncomeCalculator
    {
        private readonly ComboSystem comboSystem;

        public IncomeCalculator(ComboSystem comboSystem)
        {
            this.comboSystem = comboSystem;
        }

        /// <summary>
        /// Calculates the total income from all businesses this turn.
        /// Handles special business mechanics:
        /// - Tech Startup: activationDelay (3 turns of 0 income)
        /// - Nightclub: requiresTrendToOperate (0 income if no trend event)
        /// - Crypto Exchange: hasRandomIncome (random from threshold table)
        /// - Organic Farm: foodBonusTag (+20 per food business)
        /// - Ad Agency: globalCustomerBonus (+2 customers to all businesses)
        /// </summary>
        public int CalculateTurnIncome(
            IReadOnlyList<ActiveBusiness> businesses,
            CardData activeEvent,
            float debtPercent,
            int debtTurnsRemaining)
        {
            int total = 0;
            int comboIncomeBonus = GetComboIncomeBonus();
            float comboIncomeMultiplier = GetComboIncomeMultiplier();

            for (int i = 0; i < businesses.Count; i++)
            {
                ActiveBusiness business = businesses[i];
                if (business.isClosed) continue;
                if (business.businessCard == null) continue;

                CardData card = business.businessCard;

                // --- Tech Startup delay: 0 income until activationDelay turns pass ---
                if (card.activationDelay > 0 && business.turnsActive < card.activationDelay)
                    continue;

                // --- Nightclub: needs trend event to operate ---
                if (card.requiresTrendToOperate)
                {
                    bool trendActive = activeEvent != null && HasTag(activeEvent, CardTag.Trendy);
                    if (!trendActive)
                        continue;
                }

                int businessIncome;

                // --- Crypto Exchange: random income ---
                if (card.hasRandomIncome)
                {
                    businessIncome = CalculateRandomIncome(card);
                }
                else
                {
                    businessIncome = card.incomePerTurn;
                }

                // --- Trend bonus (Coffee Shop: hasTrendBonus + trendIncomeMultiplier) ---
                if (card.hasTrendBonus && activeEvent != null)
                {
                    bool trendEventActive = HasTag(activeEvent, CardTag.Trendy) ||
                                            HasTag(activeEvent, CardTag.Coffee);
                    if (trendEventActive)
                    {
                        businessIncome = Mathf.RoundToInt(businessIncome * card.trendIncomeMultiplier);
                    }
                }

                // --- Employee income contributions ---
                foreach (var emp in business.employees)
                {
                    if (emp == null) continue;

                    // Synergy flat bonus: Sef in food business = +30
                    if (emp.incomeFlatBonus > 0f && HasTag(card, emp.incomeBonusTag))
                    {
                        businessIncome += (int)emp.incomeFlatBonus;
                    }

                    // Income multiplier: Marketing Guru +25%
                    if (emp.incomeMultiplier > 0f)
                    {
                        businessIncome = Mathf.RoundToInt(businessIncome * (1f + emp.incomeMultiplier));
                    }

                    // Illegal income: Dolandirici +120/turn
                    if (emp.illegalIncomePerTurn > 0)
                    {
                        businessIncome += emp.illegalIncomePerTurn;
                    }
                }

                // --- Upgrade multipliers ---
                float upgradeMultiplier = 1f;
                foreach (var upgrade in business.upgrades)
                {
                    if (upgrade == null) continue;
                    upgradeMultiplier += upgrade.upgradeValue / 100f;
                }

                businessIncome = Mathf.RoundToInt(businessIncome * upgradeMultiplier);

                // --- Event effects on income ---
                if (activeEvent != null)
                {
                    businessIncome = ApplyEventIncomeEffects(businessIncome, business, activeEvent);
                }

                // --- Organik Ciftlik: bonus to all food businesses ---
                if (card.foodBonusAmount > 0 && !string.IsNullOrEmpty(card.foodBonusTag))
                {
                    int foodBusinessCount = CountBusinessesWithTag(businesses, CardTag.Food);
                    if (HasTag(card, CardTag.Food)) foodBusinessCount--;
                    businessIncome += card.foodBonusAmount * foodBusinessCount;
                }

                total += businessIncome;
            }

            // --- Add combo income bonuses ---
            total += comboIncomeBonus;

            // --- Apply combo income multiplier ---
            if (comboIncomeMultiplier > 1f)
            {
                total = Mathf.RoundToInt(total * comboIncomeMultiplier);
            }

            // --- Investor debt: subtract percentage ---
            if (debtTurnsRemaining > 0)
            {
                int debtPayment = Mathf.RoundToInt(total * debtPercent);
                total -= debtPayment;
            }

            return total;
        }

        /// <summary>
        /// Kripto Borsasi random income using the threshold table from CardData.
        /// Dice roll 1-6 mapped to randomIncomeThresholds array.
        /// </summary>
        private int CalculateRandomIncome(CardData card)
        {
            if (card.randomIncomeThresholds != null && card.randomIncomeThresholds.Length > 0)
            {
                int roll = Random.Range(0, card.randomIncomeThresholds.Length);
                return card.randomIncomeThresholds[roll];
            }

            return Random.Range(card.randomIncomeMin, card.randomIncomeMax + 1);
        }

        /// <summary>
        /// Applies event income effects (e.g. Ekonomik Kriz: all income -30%).
        /// </summary>
        private int ApplyEventIncomeEffects(int income, ActiveBusiness business, CardData eventCard)
        {
            if (eventCard == null) return income;

            switch (eventCard.eventEffectType)
            {
                case EventEffectType.AllIncomeReduction:
                    income = Mathf.RoundToInt(income * (1f + eventCard.eventMultiplier));
                    break;

                case EventEffectType.TagDoubleEffect:
                case EventEffectType.TagDoubleEffectFinance:
                    if (eventCard.affectedTags != null && business.businessCard != null)
                    {
                        foreach (var affectedTag in eventCard.affectedTags)
                        {
                            if (HasTag(business.businessCard, affectedTag))
                            {
                                income = Mathf.RoundToInt(income * (1f + eventCard.eventMultiplier));
                                break;
                            }
                        }
                    }
                    break;
            }

            return income;
        }

        private int GetComboIncomeBonus()
        {
            if (comboSystem == null) return 0;

            int bonus = 0;
            foreach (var combo in comboSystem.ActiveCombos)
            {
                if (combo != null)
                    bonus += combo.bonusIncome;
            }
            return bonus;
        }

        private float GetComboIncomeMultiplier()
        {
            if (comboSystem == null) return 1f;

            float multiplier = 1f;
            foreach (var combo in comboSystem.ActiveCombos)
            {
                if (combo != null && combo.incomeMultiplier > 1f)
                    multiplier *= combo.incomeMultiplier;
            }
            return multiplier;
        }

        private int CountBusinessesWithTag(IReadOnlyList<ActiveBusiness> businesses, CardTag tag)
        {
            int count = 0;
            foreach (var biz in businesses)
            {
                if (biz.isClosed || biz.businessCard == null) continue;
                if (HasTag(biz.businessCard, tag)) count++;
            }
            return count;
        }

        private bool HasTag(CardData card, CardTag tag)
        {
            if (card == null || card.tags == null) return false;
            foreach (var t in card.tags) { if (t == tag) return true; }
            return false;
        }
    }
}
