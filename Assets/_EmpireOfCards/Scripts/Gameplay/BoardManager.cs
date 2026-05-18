using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Represents an active business on the board with its employees, upgrades,
    /// evolution tracking, and employee tenure tracking.
    /// </summary>
    [Serializable]
    public class ActiveBusiness
    {
        public CardData businessCard;
        public List<CardData> employees = new List<CardData>();
        public List<int> employeeTenure = new List<int>();          // Turns each employee has been here
        public List<CardData> upgrades = new List<CardData>();
        public int turnsActive;
        public int totalCustomersAttracted;                         // For evolution (GDD 3.1: 40 threshold)
        public bool isClosed;
        public int closedTurnsRemaining;
        public BusinessLevel currentLevel = BusinessLevel.Level1;

        /// <summary>
        /// Returns the effective employee slot count, considering upgrades
        /// like Otomasyon that close slots.
        /// </summary>
        public int GetAvailableEmployeeSlots()
        {
            int baseSlots = businessCard != null ? businessCard.employeeSlots : 2;
            int closedSlots = 0;

            foreach (var upgrade in upgrades)
            {
                if (upgrade != null)
                    closedSlots += upgrade.closedEmployeeSlots;
            }

            return Mathf.Max(0, baseSlots - closedSlots);
        }
    }

    /// <summary>
    /// Manages the physical board state: business slots, employee placement, upgrades,
    /// business evolution (GDD 3.1), and employee leaving mechanic (GDD 4.2).
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        // --- Runtime State ---
        [Header("Board State")]
        [SerializeField] private List<ActiveBusiness> playerBusinesses = new List<ActiveBusiness>();
        [SerializeField] private List<CardData> globalUpgrades = new List<CardData>();
        [SerializeField] private int maxSlots = 3;

        // --- Active Event ---
        [Header("Active Event")]
        [SerializeField] private CardData activeEvent;
        [SerializeField] private int activeEventTurnsRemaining;

        // --- Properties ---
        public IReadOnlyList<ActiveBusiness> PlayerBusinesses => playerBusinesses;
        public IReadOnlyList<CardData> GlobalUpgrades => globalUpgrades;
        public int MaxSlots => maxSlots;
        public CardData ActiveEvent => activeEvent;
        public int ActiveEventTurnsRemaining => activeEventTurnsRemaining;

        // ----------------------------------------------------------------
        // Placement
        // ----------------------------------------------------------------

        /// <summary>
        /// Places a business card into the next available slot. Returns true on success.
        /// </summary>
        public bool PlaceBusiness(CardData card, int slotIndex)
        {
            if (card == null || card.cardType != CardType.Business)
            {
                Debug.LogWarning("[BoardManager] Card is null or not a Business type.");
                return false;
            }

            if (GetActiveBusinessCount() >= maxSlots)
            {
                Debug.Log("[BoardManager] No available business slots.");
                return false;
            }

            if (slotIndex < 0)
                slotIndex = playerBusinesses.Count;

            ActiveBusiness newBusiness = new ActiveBusiness
            {
                businessCard = card,
                turnsActive = 0,
                totalCustomersAttracted = 0,
                isClosed = false,
                closedTurnsRemaining = 0,
                currentLevel = BusinessLevel.Level1
            };

            if (slotIndex >= playerBusinesses.Count)
                playerBusinesses.Add(newBusiness);
            else
                playerBusinesses.Insert(slotIndex, newBusiness);

            EventBus.BusinessPlaced(card, slotIndex);
            return true;
        }

        /// <summary>
        /// Assigns an employee card to a specific business. Returns true on success.
        /// Checks employee slot limits including Otomasyon upgrade reductions.
        /// </summary>
        public bool PlaceEmployee(CardData card, int businessIndex)
        {
            if (card == null || card.cardType != CardType.Employee)
            {
                Debug.LogWarning("[BoardManager] Card is null or not an Employee type.");
                return false;
            }

            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
            {
                Debug.LogWarning("[BoardManager] Invalid business index.");
                return false;
            }

            ActiveBusiness business = playerBusinesses[businessIndex];

            if (business.isClosed)
            {
                Debug.Log("[BoardManager] Cannot assign employees to a closed business.");
                return false;
            }

            int availableSlots = business.GetAvailableEmployeeSlots();
            if (business.employees.Count >= availableSlots)
            {
                Debug.Log("[BoardManager] Business has reached maximum employee slots.");
                return false;
            }

            business.employees.Add(card);
            business.employeeTenure.Add(0); // Fresh hire, 0 turns tenure
            EventBus.EmployeePlaced(card, businessIndex);
            return true;
        }

        /// <summary>
        /// Places an upgrade card on a specific business, or globally if businessIndex is -1.
        /// Returns true on success.
        /// </summary>
        public bool PlaceUpgrade(CardData card, int businessIndex)
        {
            if (card == null || card.cardType != CardType.Upgrade)
            {
                Debug.LogWarning("[BoardManager] Card is null or not an Upgrade type.");
                return false;
            }

            if (card.isGlobalUpgrade || businessIndex == -1)
            {
                globalUpgrades.Add(card);
                EventBus.UpgradePlaced(card, -1);
                return true;
            }

            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
            {
                Debug.LogWarning("[BoardManager] Invalid business index for upgrade.");
                return false;
            }

            ActiveBusiness business = playerBusinesses[businessIndex];

            if (business.isClosed)
            {
                Debug.Log("[BoardManager] Cannot place upgrades on a closed business.");
                return false;
            }

            business.upgrades.Add(card);
            EventBus.UpgradePlaced(card, businessIndex);
            return true;
        }

        /// <summary>
        /// Sets the active world event. Replaces any previous event.
        /// </summary>
        public void SetActiveEvent(CardData eventCard)
        {
            if (eventCard == null || eventCard.cardType != CardType.Event)
                return;

            // Expire previous event if any
            if (activeEvent != null)
            {
                EventBus.EventExpired(activeEvent);
            }

            activeEvent = eventCard;
            activeEventTurnsRemaining = eventCard.eventDuration;
            EventBus.EventActivated(eventCard);
        }

        // ----------------------------------------------------------------
        // Removal
        // ----------------------------------------------------------------

        /// <summary>
        /// Removes an employee from a business by index. Returns the removed card.
        /// </summary>
        public CardData RemoveEmployee(int businessIndex, int employeeIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return null;

            ActiveBusiness business = playerBusinesses[businessIndex];

            if (employeeIndex < 0 || employeeIndex >= business.employees.Count)
                return null;

            CardData removed = business.employees[employeeIndex];
            business.employees.RemoveAt(employeeIndex);

            if (employeeIndex < business.employeeTenure.Count)
                business.employeeTenure.RemoveAt(employeeIndex);

            Debug.Log($"[BoardManager] Removed employee '{(removed != null ? removed.cardName : "null")}' from business {businessIndex}.");
            return removed;
        }

        /// <summary>
        /// Removes a specific employee card from the given business. Returns true if found.
        /// </summary>
        public bool RemoveEmployeeByCard(int businessIndex, CardData employee)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return false;

            ActiveBusiness business = playerBusinesses[businessIndex];
            int idx = business.employees.IndexOf(employee);
            if (idx < 0) return false;

            RemoveEmployee(businessIndex, idx);
            return true;
        }

        /// <summary>
        /// Permanently removes a business from the board.
        /// </summary>
        public void RemoveBusiness(int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return;

            playerBusinesses.RemoveAt(businessIndex);
        }

        // ----------------------------------------------------------------
        // Business Lifecycle
        // ----------------------------------------------------------------

        /// <summary>
        /// Closes a business for the specified number of turns.
        /// </summary>
        public void CloseBusiness(int businessIndex, int turns)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return;

            ActiveBusiness business = playerBusinesses[businessIndex];
            business.isClosed = true;
            business.closedTurnsRemaining = turns;
            EventBus.BusinessClosed(businessIndex);
        }

        /// <summary>
        /// Reopens a previously closed business.
        /// </summary>
        public void ReopenBusiness(int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return;

            ActiveBusiness business = playerBusinesses[businessIndex];
            business.isClosed = false;
            business.closedTurnsRemaining = 0;
            EventBus.BusinessReopened(businessIndex);
        }

        // ----------------------------------------------------------------
        // Turn Ticking
        // ----------------------------------------------------------------

        /// <summary>
        /// Advances all businesses by one turn. Handles:
        /// - Closed business countdowns
        /// - TurnsActive increment for open businesses
        /// - Business evolution check (GDD 3.1)
        /// - Employee tenure increment and leaving check (GDD 4.2)
        /// - Active event countdown
        /// </summary>
        public void TickBusinesses()
        {
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                ActiveBusiness business = playerBusinesses[i];

                // Tick closed businesses
                if (business.isClosed)
                {
                    business.closedTurnsRemaining--;
                    if (business.closedTurnsRemaining <= 0)
                    {
                        ReopenBusiness(i);
                    }
                    continue;
                }

                // Advance turns active
                business.turnsActive++;

                // Tick employee tenure and check for leaving (GDD 4.2)
                TickEmployeeTenure(i, business);

                // Check for business evolution (GDD 3.1)
                CheckBusinessEvolution(i, business);
            }

            // Tick active event
            TickActiveEvent();
        }

        /// <summary>
        /// Ticks closed businesses only. Use if you need to separate ticking steps.
        /// </summary>
        public void TickClosedBusinesses()
        {
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                ActiveBusiness business = playerBusinesses[i];
                if (!business.isClosed) continue;

                business.closedTurnsRemaining--;
                if (business.closedTurnsRemaining <= 0)
                {
                    ReopenBusiness(i);
                }
            }
        }

        /// <summary>
        /// Adds customers attracted this turn to a business's evolution tracker.
        /// Call from EconomyManager during resolve.
        /// </summary>
        public void AddCustomersAttracted(int businessIndex, int customers)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return;

            playerBusinesses[businessIndex].totalCustomersAttracted += customers;
        }

        // ----------------------------------------------------------------
        // Evolution (GDD Section 3.1)
        // ----------------------------------------------------------------

        /// <summary>
        /// Checks if a business meets evolution requirements:
        /// - 40+ total customers attracted
        /// - 15+ turns active
        /// - Business card has canEvolve = true and evolvedForm != null
        /// On evolution: replaces business card with evolved form,
        /// new stats: Bufe(50/3) -> Restoran(80/5) -> Zincir(120/8)
        /// </summary>
        private void CheckBusinessEvolution(int index, ActiveBusiness business)
        {
            if (business.businessCard == null) return;
            if (!business.businessCard.canEvolve) return;
            if (business.businessCard.evolvedForm == null) return;

            int customerReq = business.businessCard.evolutionCustomerReq > 0
                ? business.businessCard.evolutionCustomerReq
                : Constants.EVOLUTION_CUSTOMER_THRESHOLD;

            int turnReq = business.businessCard.evolutionTurnReq > 0
                ? business.businessCard.evolutionTurnReq
                : Constants.EVOLUTION_TURN_REQUIREMENT;

            if (business.totalCustomersAttracted >= customerReq && business.turnsActive >= turnReq)
            {
                CardData oldCard = business.businessCard;
                business.businessCard = oldCard.evolvedForm;

                // Advance level
                if (business.currentLevel == BusinessLevel.Level1)
                    business.currentLevel = BusinessLevel.Level2;
                else if (business.currentLevel == BusinessLevel.Level2)
                    business.currentLevel = BusinessLevel.Level3;

                // Reset tracking for next evolution
                business.totalCustomersAttracted = 0;
                business.turnsActive = 0;

                Debug.Log($"[BoardManager] Business evolved! {oldCard.cardName} -> {business.businessCard.cardName}");
                EventBus.BusinessEvolved(index, business.currentLevel);
            }
        }

        // ----------------------------------------------------------------
        // Employee Tenure & Leaving (GDD Section 4.2)
        // ----------------------------------------------------------------

        /// <summary>
        /// Increments employee tenure and checks if any employee leaves.
        /// Employees leave after 8+ turns without salary raise (GDD says the threshold
        /// is stored in Constants.EMPLOYEE_LEAVE_TURN_THRESHOLD).
        /// When an employee leaves, they are removed and an event is fired.
        /// </summary>
        private void TickEmployeeTenure(int businessIndex, ActiveBusiness business)
        {
            // Iterate backwards to safely remove during iteration
            for (int e = business.employeeTenure.Count - 1; e >= 0; e--)
            {
                business.employeeTenure[e]++;

                if (business.employeeTenure[e] >= Constants.EMPLOYEE_LEAVE_TURN_THRESHOLD)
                {
                    // Employee may leave - random chance increases with tenure
                    int overThreshold = business.employeeTenure[e] - Constants.EMPLOYEE_LEAVE_TURN_THRESHOLD;
                    // 20% base + 10% per extra turn over threshold
                    float leaveChance = 0.20f + (overThreshold * 0.10f);

                    if (UnityEngine.Random.value < leaveChance)
                    {
                        CardData leavingEmployee = business.employees[e];

                        // Sadik Mudur prevents transfer/leaving
                        if (leavingEmployee != null && leavingEmployee.preventsTransfer)
                            continue;

                        RemoveEmployee(businessIndex, e);
                        Debug.Log($"[BoardManager] Employee '{(leavingEmployee != null ? leavingEmployee.cardName : "?")}' left due to tenure ({business.employeeTenure.Count} turns).");
                        EventBus.EmployeeLeft(leavingEmployee, businessIndex);
                    }
                }
            }
        }

        // ----------------------------------------------------------------
        // Active Event Ticking
        // ----------------------------------------------------------------

        private void TickActiveEvent()
        {
            if (activeEvent == null) return;

            activeEventTurnsRemaining--;
            if (activeEventTurnsRemaining <= 0)
            {
                CardData expired = activeEvent;
                activeEvent = null;
                activeEventTurnsRemaining = 0;
                EventBus.EventExpired(expired);
            }
        }

        // ----------------------------------------------------------------
        // Queries
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the number of open (non-closed) businesses on the board.
        /// </summary>
        public int GetActiveBusinessCount()
        {
            int count = 0;
            foreach (var business in playerBusinesses)
            {
                if (!business.isClosed)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Returns all card IDs currently active on the board (businesses, employees, upgrades).
        /// Used for combo checking.
        /// </summary>
        public List<string> GetAllActiveCardIds()
        {
            List<string> ids = new List<string>();

            foreach (var business in playerBusinesses)
            {
                if (business.isClosed)
                    continue;

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

            // Include active event
            if (activeEvent != null)
                ids.Add(activeEvent.cardId);

            return ids;
        }

        /// <summary>
        /// Returns all tags currently active on the board.
        /// </summary>
        public HashSet<CardTag> GetAllActiveTags()
        {
            HashSet<CardTag> tags = new HashSet<CardTag>();

            foreach (var business in playerBusinesses)
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
        public bool HasTag(CardTag tag)
        {
            return GetAllActiveTags().Contains(tag);
        }

        /// <summary>
        /// Calculates the total customers the player is attracting this turn.
        /// Sums base customers from businesses + employee bonuses + synergy.
        /// Does NOT include combo or event modifiers (those are in EconomyManager).
        /// </summary>
        public int CalculatePlayerCustomers()
        {
            int total = 0;

            foreach (var business in playerBusinesses)
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

                    // Check synergy: if employee's synergy tag matches business tags
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

                // Global customer bonus from Reklam Ajansı
                bizCustomers += business.businessCard.globalCustomerBonus;

                total += bizCustomers;
            }

            // Add global upgrade customer bonuses
            foreach (var upgrade in globalUpgrades)
            {
                if (upgrade == null) continue;
                total += (int)upgrade.upgradeValue; // GlobalCustomerFlat adds flat customers
            }

            return total;
        }

        /// <summary>
        /// Resets the board to empty state for a new run.
        /// </summary>
        public void Reset()
        {
            playerBusinesses.Clear();
            globalUpgrades.Clear();
            activeEvent = null;
            activeEventTurnsRemaining = 0;
            maxSlots = Constants.STARTING_SLOTS;
        }

        /// <summary>
        /// Sets the maximum number of business slots the player can use.
        /// </summary>
        public void SetMaxSlots(int slots)
        {
            maxSlots = Mathf.Clamp(slots, 1, Constants.MAX_SLOTS);
        }

        /// <summary>
        /// Finds the business index that contains a specific employee card.
        /// Returns -1 if not found.
        /// </summary>
        public int FindBusinessWithEmployee(CardData employee)
        {
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                if (playerBusinesses[i].employees.Contains(employee))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Counts how many employees with a specific card ID are active on the board.
        /// </summary>
        public int CountEmployeesById(string cardId)
        {
            int count = 0;
            foreach (var biz in playerBusinesses)
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
        public List<(CardData employee, int businessIndex, int employeeIndex)> GetAllIllegalEmployees()
        {
            var result = new List<(CardData, int, int)>();

            for (int b = 0; b < playerBusinesses.Count; b++)
            {
                ActiveBusiness biz = playerBusinesses[b];
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

        private void AddCardTags(HashSet<CardTag> tagSet, CardData card)
        {
            if (card == null || card.tags == null) return;
            foreach (var tag in card.tags)
            {
                tagSet.Add(tag);
            }
        }

        private bool CardHasTag(CardData card, CardTag tag)
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
