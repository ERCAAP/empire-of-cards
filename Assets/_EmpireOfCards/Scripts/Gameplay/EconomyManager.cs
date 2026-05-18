using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Full economy pipeline matching GDD Section 9.
    /// Income = business income + employee bonuses + synergy + upgrade multipliers + combo bonuses + illegal income
    /// Salaries = sum of all employee salaries (automatic)
    /// Tax = gross income x 15% (muhasebeci: 7.5%, 2x muhasebeci: 3%)
    /// Net = income - salaries - tax
    /// </summary>
    public class EconomyManager : MonoBehaviour
    {
        // --- Data Reference ---
        [Header("Balance Data")]
        [SerializeField] private GameBalanceData balanceData;

        // --- Manager References ---
        [Header("References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ComboSystem comboSystem;

        // --- Runtime State ---
        [Header("Turn Summary (Read Only)")]
        [SerializeField] private int grossIncome;
        [SerializeField] private int totalSalaries;
        [SerializeField] private int taxAmount;
        [SerializeField] private int netIncome;
        [SerializeField] private int totalCustomersThisTurn;

        // --- Investor debt tracking (Yatirimci Sunumu: +600 now, 3 turns x 15%) ---
        [Header("Debt Tracking")]
        [SerializeField] private int debtTurnsRemaining;
        [SerializeField] private float debtPercent;

        // --- Properties ---
        public int GrossIncome => grossIncome;
        public int TotalSalaries => totalSalaries;
        public int TaxAmount => taxAmount;
        public int NetIncome => netIncome;
        public int TotalCustomersThisTurn => totalCustomersThisTurn;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService instead of RuntimeWiring.SetField().
        /// </summary>
        public void Init(GameBalanceData balance, BoardManager board, ComboSystem combo)
        {
            this.balanceData = balance;
            this.boardManager = board;
            this.comboSystem = combo;
        }

        // ----------------------------------------------------------------
        // Income Calculation (GDD Section 9)
        // ----------------------------------------------------------------

        /// <summary>
        /// Calculates the total income from all businesses this turn.
        /// Handles special business mechanics:
        /// - Tech Startup: activationDelay (3 turns of 0 income)
        /// - Nightclub: requiresTrendToOperate (0 income if no trend event)
        /// - Crypto Exchange: hasRandomIncome (random from threshold table)
        /// - Organic Farm: foodBonusTag (+20 per food business)
        /// - Ad Agency: globalCustomerBonus (+2 customers to all businesses)
        /// </summary>
        public int CalculateTurnIncome(IReadOnlyList<ActiveBusiness> businesses, CardData activeEvent)
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
                    // Employee's incomeFlatBonus applies when incomeBonusTag matches the business
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
                    // IncomePercentSingle: +10%, IncomePercentWithSlotLoss: +30%
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
                    // Don't count self
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

            grossIncome = total;
            EventBus.IncomeReceived(grossIncome);
            return grossIncome;
        }

        /// <summary>
        /// Kripto Borsasi random income using the threshold table from CardData.
        /// Dice roll 1-6 mapped to randomIncomeThresholds array.
        /// </summary>
        private int CalculateRandomIncome(CardData card)
        {
            if (card.randomIncomeThresholds != null && card.randomIncomeThresholds.Length > 0)
            {
                int roll = UnityEngine.Random.Range(0, card.randomIncomeThresholds.Length);
                return card.randomIncomeThresholds[roll];
            }

            // Fallback to range
            return UnityEngine.Random.Range(card.randomIncomeMin, card.randomIncomeMax + 1);
        }

        // ----------------------------------------------------------------
        // Salaries (GDD Section 9)
        // ----------------------------------------------------------------

        /// <summary>
        /// Sums all employee salaries across every active business.
        /// Salaries are automatic and mandatory.
        /// </summary>
        public int CalculateSalaries(IReadOnlyList<ActiveBusiness> businesses)
        {
            int total = 0;

            foreach (var business in businesses)
            {
                if (business.isClosed) continue;

                foreach (var employee in business.employees)
                {
                    if (employee != null)
                        total += employee.salaryPerTurn;
                }
            }

            totalSalaries = total;
            return totalSalaries;
        }

        // ----------------------------------------------------------------
        // Tax (GDD Section 9)
        // ----------------------------------------------------------------

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

            taxAmount = Mathf.RoundToInt(income * effectiveRate);
            return taxAmount;
        }

        // ----------------------------------------------------------------
        // Market Pool (GDD Balance Table)
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the total customer market pool for the given turn.
        /// Base 60, +5/turn (1-5), +6 (6-10), +8 (11-15), +10 (16-20).
        /// </summary>
        public int GetMarketPool(int currentTurn)
        {
            if (balanceData != null)
                return balanceData.GetMarketPool(currentTurn);

            // Fallback to Constants
            int pool = Constants.BASE_MARKET_CUSTOMERS;
            for (int t = 1; t < currentTurn; t++)
            {
                if (t <= 5) pool += Constants.EARLY_GROWTH_PER_TURN;
                else if (t <= 10) pool += Constants.MID_GROWTH_PER_TURN;
                else if (t <= 15) pool += Constants.LATE_GROWTH_PER_TURN;
                else pool += Constants.END_GROWTH_PER_TURN;
            }
            return pool;
        }

        // ----------------------------------------------------------------
        // End of Turn Pipeline
        // ----------------------------------------------------------------

        /// <summary>
        /// Runs the full end-of-turn economy pipeline:
        /// 1. Calculate income from all businesses
        /// 2. Calculate salaries
        /// 3. Count accountants for tax
        /// 4. Calculate tax
        /// 5. Net = income - salaries - tax
        /// 6. Apply to GameManager
        /// 7. Tick investor debt
        /// 8. Track customers attracted per business (for evolution)
        /// </summary>
        public void ProcessEndOfTurn()
        {
            if (boardManager == null)
            {
                Debug.LogError("[EconomyManager] BoardManager reference is null.");
                return;
            }

            GameManager gm = GameManager.Instance;
            if (gm == null) return;

            IReadOnlyList<ActiveBusiness> businesses = boardManager.PlayerBusinesses;
            CardData activeEvent = boardManager.ActiveEvent;

            // Step 1: Income
            CalculateTurnIncome(businesses, activeEvent);

            // Step 2: Salaries
            CalculateSalaries(businesses);

            // Step 3: Count accountants (muhasebeci)
            int accountantCount = CountAccountants(businesses);

            // Step 4: Tax
            CalculateTax(grossIncome, accountantCount);

            // Step 5: Net
            netIncome = grossIncome - totalSalaries - taxAmount;

            // Step 6: Apply
            if (netIncome > 0)
            {
                gm.GainMoney(netIncome);
            }
            else if (netIncome < 0)
            {
                gm.SpendMoney(Mathf.Abs(netIncome));
            }

            // Step 7: Tick investor debt
            if (debtTurnsRemaining > 0)
            {
                debtTurnsRemaining--;
            }

            // Step 8: Track customers attracted per business for evolution
            totalCustomersThisTurn = 0;
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed) continue;
                int bizCustomers = CalculateBusinessCustomers(businesses[i]);
                boardManager.AddCustomersAttracted(i, bizCustomers);
                totalCustomersThisTurn += bizCustomers;
            }
        }

        // ----------------------------------------------------------------
        // Sell Price
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the sell price of a card: buyCost x 40% (GDD).
        /// </summary>
        public int GetSellPrice(CardData card)
        {
            if (card == null) return 0;

            float rate = balanceData != null ? balanceData.sellRate : Constants.SELL_RATE;
            return Mathf.Max(1, Mathf.RoundToInt(card.buyCost * rate));
        }

        // ----------------------------------------------------------------
        // Investor Debt (Yatirimci Sunumu action card)
        // ----------------------------------------------------------------

        /// <summary>
        /// Starts investor debt: for `duration` turns, `percent` of income is deducted.
        /// </summary>
        public void StartInvestorDebt(int duration, float percent)
        {
            debtTurnsRemaining = duration;
            debtPercent = percent;
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Counts how many accountant employees (muhasebeci) are on the board.
        /// Uses the taxReduction field: any employee with taxReduction > 0 is an accountant.
        /// </summary>
        private int CountAccountants(IReadOnlyList<ActiveBusiness> businesses)
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

        /// <summary>
        /// Checks if an employee has synergy with a business card
        /// (employee's synergyTag matches one of the business's tags).
        /// </summary>
        private bool CheckSynergy(CardData employee, CardData business)
        {
            if (business.tags == null) return false;

            foreach (var tag in business.tags)
            {
                if (tag == employee.synergyTag)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates customer count for a single business (for evolution tracking).
        /// </summary>
        private int CalculateBusinessCustomers(ActiveBusiness business)
        {
            if (business.businessCard == null) return 0;

            CardData card = business.businessCard;

            if (card.activationDelay > 0 && business.turnsActive < card.activationDelay)
                return 0;

            int customers = card.customersPerTurn;

            foreach (var emp in business.employees)
            {
                if (emp == null) continue;
                bool hasSynergy = CheckSynergy(emp, card);
                customers += hasSynergy ? emp.synergyCustomerBonus : emp.customerBonus;
            }

            return customers;
        }

        /// <summary>
        /// Counts businesses that have a specific tag.
        /// </summary>
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

        /// <summary>
        /// Applies event income effects (e.g. Ekonomik Kriz: all income -30%).
        /// </summary>
        private int ApplyEventIncomeEffects(int income, ActiveBusiness business, CardData eventCard)
        {
            if (eventCard == null) return income;

            switch (eventCard.eventEffectType)
            {
                case EventEffectType.AllIncomeReduction:
                    // Ekonomik Kriz: all income reduced by eventMultiplier (e.g. -0.3 = -30%)
                    income = Mathf.RoundToInt(income * (1f + eventCard.eventMultiplier));
                    break;

                case EventEffectType.TagDoubleEffect:
                case EventEffectType.TagDoubleEffectFinance:
                    // Marketing/Finance cards double: check if business has affected tag
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

        /// <summary>
        /// Gets total bonus income from active combos.
        /// </summary>
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

        /// <summary>
        /// Gets total income multiplier from active combos.
        /// </summary>
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

        /// <summary>
        /// Resets economy state for a new run.
        /// </summary>
        public void Reset()
        {
            grossIncome = 0;
            totalSalaries = 0;
            taxAmount = 0;
            netIncome = 0;
            totalCustomersThisTurn = 0;
            debtTurnsRemaining = 0;
            debtPercent = 0f;
        }
    }
}
