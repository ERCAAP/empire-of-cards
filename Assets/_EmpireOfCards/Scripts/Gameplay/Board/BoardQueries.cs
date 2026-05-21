using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Gameplay.Board
{
    /// <summary>
    /// Pure read-only query methods over the board state.
    /// All methods are static and take the data they need as parameters,
    /// making them easy to test and free of side effects.
    /// </summary>
    public static class BoardQueries
    {
        /// <summary>
        /// Returns the number of open (non-closed) businesses on the board.
        /// </summary>
        public static int GetActiveBusinessCount(IReadOnlyList<ActiveBusiness> businesses)
        {
            int count = 0;
            foreach (var business in businesses)
            {
                if (!business.isClosed)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Returns all card IDs currently active on the board (businesses, employees, upgrades).
        /// Used for combo checking. Includes global upgrades and the active event.
        /// </summary>
        public static List<string> GetAllActiveCardIds(
            IReadOnlyList<ActiveBusiness> businesses,
            IReadOnlyList<CardData> globalUpgrades,
            CardData activeEvent)
        {
            List<string> ids = new List<string>();

            foreach (var business in businesses)
            {
                if (business.isClosed) continue;

                if (business.businessCard != null)
                    ids.Add(business.businessCard.cardId);

                foreach (var employee in business.employees)
                {
                    if (employee != null)
                        ids.Add(employee.cardId);
                }

                foreach (var upgrade in business.upgrades)
                {
                    if (upgrade != null)
                        ids.Add(upgrade.cardId);
                }
            }

            foreach (var upgrade in globalUpgrades)
            {
                if (upgrade != null)
                    ids.Add(upgrade.cardId);
            }

            if (activeEvent != null)
                ids.Add(activeEvent.cardId);

            return ids;
        }

        /// <summary>
        /// Returns all tags currently active on the board.
        /// </summary>
        public static HashSet<CardTag> GetAllActiveTags(
            IReadOnlyList<ActiveBusiness> businesses,
            IReadOnlyList<CardData> globalUpgrades,
            CardData activeEvent)
        {
            HashSet<CardTag> tags = new HashSet<CardTag>();

            foreach (var business in businesses)
            {
                if (business.isClosed) continue;

                AddCardTags(tags, business.businessCard);
                foreach (var emp in business.employees) AddCardTags(tags, emp);
                foreach (var upg in business.upgrades) AddCardTags(tags, upg);
            }

            foreach (var upg in globalUpgrades) AddCardTags(tags, upg);
            if (activeEvent != null) AddCardTags(tags, activeEvent);

            return tags;
        }

        /// <summary>
        /// Checks if any active card on the board has the specified tag.
        /// </summary>
        public static bool HasTag(
            IReadOnlyList<ActiveBusiness> businesses,
            IReadOnlyList<CardData> globalUpgrades,
            CardData activeEvent,
            CardTag tag)
        {
            return GetAllActiveTags(businesses, globalUpgrades, activeEvent).Contains(tag);
        }

        /// <summary>
        /// Calculates the total customers the player is attracting this turn.
        /// Sums base customers from businesses + employee bonuses + synergy.
        /// Does NOT include combo or event modifiers (those are in EconomyManager).
        /// </summary>
        public static int CalculatePlayerCustomers(
            IReadOnlyList<ActiveBusiness> businesses,
            IReadOnlyList<CardData> globalUpgrades)
        {
            int total = 0;

            // Sum all globalCustomerBonus values from active businesses (e.g. Ad Agency: +2)
            // This bonus applies to EVERY active business, not just the one that has it
            int totalGlobalCustomerBonus = 0;
            foreach (var biz in businesses)
            {
                if (biz.isClosed || biz.businessCard == null) continue;
                totalGlobalCustomerBonus += biz.businessCard.globalCustomerBonus;
            }

            foreach (var business in businesses)
            {
                if (business.isClosed) continue;
                if (business.businessCard == null) continue;

                // Skip businesses with activation delay not yet passed (Tech Startup)
                if (business.businessCard.activationDelay > 0 &&
                    business.turnsActive < business.businessCard.activationDelay)
                    continue;

                int bizCustomers = business.businessCard.customersPerTurn;

                // Add employee customer bonuses with synergy
                foreach (var emp in business.employees)
                {
                    if (emp == null) continue;

                    bool hasSynergy = false;
                    if (business.businessCard.tags != null)
                    {
                        foreach (var bizTag in business.businessCard.tags)
                        {
                            if (bizTag == emp.synergyTag)
                            {
                                hasSynergy = true;
                                break;
                            }
                        }
                    }

                    bizCustomers += hasSynergy ? emp.synergyCustomerBonus : emp.customerBonus;
                }

                // Apply global customer bonus from all sources (Ad Agency etc.) to every business
                bizCustomers += totalGlobalCustomerBonus;

                total += bizCustomers;
            }

            // Add global upgrade customer bonuses
            foreach (var upgrade in globalUpgrades)
            {
                if (upgrade == null) continue;
                total += (int)upgrade.upgradeValue;
            }

            return total;
        }

        /// <summary>
        /// Finds the business index that contains a specific employee card.
        /// Returns -1 if not found.
        /// </summary>
        public static int FindBusinessWithEmployee(IReadOnlyList<ActiveBusiness> businesses, CardData employee)
        {
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].employees.Contains(employee))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Counts how many employees with a specific card ID are active on the board.
        /// </summary>
        public static int CountEmployeesById(IReadOnlyList<ActiveBusiness> businesses, string cardId)
        {
            int count = 0;
            foreach (var biz in businesses)
            {
                if (biz.isClosed) continue;
                foreach (var emp in biz.employees)
                {
                    if (emp != null && emp.cardId == cardId)
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Returns all employees on the board that have the Illegal tag.
        /// </summary>
        public static List<(CardData employee, int businessIndex, int employeeIndex)> GetAllIllegalEmployees(
            IReadOnlyList<ActiveBusiness> businesses)
        {
            var result = new List<(CardData, int, int)>();

            for (int b = 0; b < businesses.Count; b++)
            {
                ActiveBusiness biz = businesses[b];
                if (biz.isClosed) continue;

                for (int e = 0; e < biz.employees.Count; e++)
                {
                    CardData emp = biz.employees[e];
                    if (emp != null && CardHasTag(emp, CardTag.Illegal))
                    {
                        result.Add((emp, b, e));
                    }
                }
            }

            return result;
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        private static void AddCardTags(HashSet<CardTag> tagSet, CardData card)
        {
            if (card == null || card.tags == null) return;
            foreach (var tag in card.tags)
            {
                tagSet.Add(tag);
            }
        }

        private static bool CardHasTag(CardData card, CardTag tag)
        {
            if (card == null || card.tags == null) return false;
            foreach (var t in card.tags)
            {
                if (t == tag) return true;
            }
            return false;
        }
    }
}
