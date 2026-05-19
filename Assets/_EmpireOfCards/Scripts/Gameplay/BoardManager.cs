using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Board;

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
        public int neglectTurns;                                     // Consecutive turns without employee/upgrade added
        public BusinessLevel currentLevel = BusinessLevel.Level1;

        /// <summary>
        /// Returns the effective employee slot count, considering upgrades
        /// like Automation that close slots.
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
    /// Thin coordinator for board state. Owns the ActiveBusiness list and
    /// delegates evolution, tenure, closure, and queries to sub-systems.
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        // --- Runtime State ---
        [Header("Board State")]
        [SerializeField] private List<ActiveBusiness> playerBusinesses = new List<ActiveBusiness>();
        [SerializeField] private List<CardData> globalUpgrades = new List<CardData>();
        [SerializeField] private int maxSlots = 3;

        // --- Sabotage State ---
        [Header("Sabotage")]
        [SerializeField] private bool productionDisabledNextTurn;

        // --- Active Event ---
        [Header("Active Event")]
        [SerializeField] private CardData activeEvent;
        [SerializeField] private int activeEventTurnsRemaining;

        // --- Sub-Systems ---
        private BusinessEvolution _evolution;
        private EmployeeTenure _tenure;
        private ClosureManager _closure;

        // --- Properties ---
        public IReadOnlyList<ActiveBusiness> PlayerBusinesses => playerBusinesses;
        public IReadOnlyList<CardData> GlobalUpgrades => globalUpgrades;
        public int MaxSlots => maxSlots;
        public CardData ActiveEvent => activeEvent;
        public int ActiveEventTurnsRemaining => activeEventTurnsRemaining;

        // ----------------------------------------------------------------
        // Initialization
        // ----------------------------------------------------------------

        private void Awake()
        {
            _evolution = new BusinessEvolution();
            _tenure = new EmployeeTenure(RemoveEmployee);
            _closure = new ClosureManager(ReopenBusiness);
        }

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
            business.employeeTenure.Add(0);
            business.neglectTurns = 0;
            EventBus.EmployeePlaced(card, businessIndex);
            return true;
        }

        /// <summary>
        /// Places an upgrade card on a specific business, or globally if businessIndex is -1.
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
            business.neglectTurns = 0;
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

            if (activeEvent != null)
                EventBus.EventExpired(activeEvent);

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
        // Business Lifecycle (delegates to sub-systems)
        // ----------------------------------------------------------------

        public void CloseBusiness(int businessIndex, int turns)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count) return;
            _closure.CloseBusiness(playerBusinesses[businessIndex], businessIndex, turns);
        }

        public void ReopenBusiness(int businessIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count) return;
            _closure.ReopenBusiness(playerBusinesses[businessIndex], businessIndex);
        }

        public void AddCustomersAttracted(int businessIndex, int customers)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count) return;
            _evolution.AddCustomersAttracted(playerBusinesses[businessIndex], customers);
        }

        // ----------------------------------------------------------------
        // Turn Ticking
        // ----------------------------------------------------------------

        /// <summary>
        /// Advances all businesses by one turn. Delegates closure ticking,
        /// employee tenure, and evolution checks to sub-systems.
        /// </summary>
        public void TickBusinesses()
        {
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                ActiveBusiness business = playerBusinesses[i];

                if (business.isClosed)
                {
                    business.closedTurnsRemaining--;
                    if (business.closedTurnsRemaining <= 0)
                        ReopenBusiness(i);
                    continue;
                }

                business.turnsActive++;
                business.neglectTurns++;

                if (business.neglectTurns == Constants.NEGLECT_THRESHOLD_MINOR ||
                    business.neglectTurns == Constants.NEGLECT_THRESHOLD_MAJOR)
                    EventBus.BusinessNeglected(i, business.neglectTurns);

                _tenure.TickTenure(i, business);
                _evolution.CheckEvolution(i, business);
            }

            TickActiveEvent();
        }

        public void TickClosedBusinesses()
        {
            _closure.TickClosures(playerBusinesses);
        }

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
        // Queries (delegates to BoardQueries)
        // ----------------------------------------------------------------

        public int GetActiveBusinessCount()
            => BoardQueries.GetActiveBusinessCount(playerBusinesses);

        public List<string> GetAllActiveCardIds()
            => BoardQueries.GetAllActiveCardIds(playerBusinesses, globalUpgrades, activeEvent);

        public HashSet<CardTag> GetAllActiveTags()
            => BoardQueries.GetAllActiveTags(playerBusinesses, globalUpgrades, activeEvent);

        public bool HasTag(CardTag tag)
            => BoardQueries.HasTag(playerBusinesses, globalUpgrades, activeEvent, tag);

        public int CalculatePlayerCustomers()
            => BoardQueries.CalculatePlayerCustomers(playerBusinesses, globalUpgrades);

        public int FindBusinessWithEmployee(CardData employee)
            => BoardQueries.FindBusinessWithEmployee(playerBusinesses, employee);

        public int CountEmployeesById(string cardId)
            => BoardQueries.CountEmployeesById(playerBusinesses, cardId);

        public List<(CardData employee, int businessIndex, int employeeIndex)> GetAllIllegalEmployees()
            => BoardQueries.GetAllIllegalEmployees(playerBusinesses);

        // ----------------------------------------------------------------
        // Utility
        // ----------------------------------------------------------------

        public void Reset()
        {
            playerBusinesses.Clear();
            globalUpgrades.Clear();
            activeEvent = null;
            activeEventTurnsRemaining = 0;
            productionDisabledNextTurn = false;
            maxSlots = Constants.STARTING_SLOTS;
        }

        public void SetMaxSlots(int slots)
        {
            maxSlots = Mathf.Clamp(slots, 1, Constants.MAX_SLOTS);
        }

        // ----------------------------------------------------------------
        // Sabotage (Rival aggressive action)
        // ----------------------------------------------------------------

        /// <summary>
        /// When true, the next Resolve phase skips business production for 1 turn.
        /// Automatically resets after being consumed.
        /// </summary>
        public bool IsProductionDisabled => productionDisabledNextTurn;

        /// <summary>
        /// Sets or clears the production-disabled flag. Called by RivalAI
        /// when an aggressive sabotage action fires.
        /// </summary>
        public void SetProductionDisabledNextTurn(bool disabled)
        {
            productionDisabledNextTurn = disabled;
            if (disabled)
                Debug.Log("[BoardManager] Production will be disabled next resolve phase.");
        }

        /// <summary>
        /// Consumes and clears the production-disabled flag.
        /// Call this at the start of the Resolve phase.
        /// Returns true if production was disabled (and should be skipped).
        /// </summary>
        public bool ConsumeProductionDisabled()
        {
            if (!productionDisabledNextTurn) return false;
            productionDisabledNextTurn = false;
            Debug.Log("[BoardManager] Production disabled this turn due to rival sabotage.");
            return true;
        }
    }
}
