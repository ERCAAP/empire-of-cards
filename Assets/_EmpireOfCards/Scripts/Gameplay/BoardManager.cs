using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Represents an active business on the board with its employees and upgrades.
    /// </summary>
    [Serializable]
    public class ActiveBusiness
    {
        public CardData businessCard;
        public List<CardData> employees = new List<CardData>();
        public List<CardData> upgrades = new List<CardData>();
        public int turnsActive;
        public bool isClosed;
        public int closedTurnsRemaining;
    }

    /// <summary>
    /// Manages the physical board state: business slots, employee slots, and upgrades.
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        // --- Runtime State ---
        [Header("Board State")]
        [SerializeField] private List<ActiveBusiness> playerBusinesses = new List<ActiveBusiness>();
        [SerializeField] private List<CardData> globalUpgrades = new List<CardData>();
        [SerializeField] private int maxSlots = 3;

        // --- Events ---
        public event Action<ActiveBusiness> OnBusinessPlaced;
        public event Action<CardData, int> OnEmployeePlaced;
        public event Action<CardData, int> OnUpgradePlaced;
        public event Action<int> OnBusinessClosed;

        // --- Properties ---
        public IReadOnlyList<ActiveBusiness> PlayerBusinesses => playerBusinesses;
        public IReadOnlyList<CardData> GlobalUpgrades => globalUpgrades;
        public int MaxSlots => maxSlots;

        /// <summary>
        /// Places a business card into the specified slot. Returns true on success.
        /// </summary>
        public bool PlaceBusiness(CardData card, int slotIndex)
        {
            if (card == null || card.cardType != CardType.Business)
            {
                Debug.LogWarning("[BoardManager] Card is null or not a Business type.");
                return false;
            }

            if (slotIndex < 0 || slotIndex > playerBusinesses.Count)
            {
                Debug.LogWarning("[BoardManager] Invalid slot index.");
                return false;
            }

            if (GetActiveBusinessCount() >= maxSlots)
            {
                Debug.Log("[BoardManager] No available business slots.");
                return false;
            }

            ActiveBusiness newBusiness = new ActiveBusiness
            {
                businessCard = card,
                turnsActive = 0,
                isClosed = false,
                closedTurnsRemaining = 0
            };

            if (slotIndex >= playerBusinesses.Count)
                playerBusinesses.Add(newBusiness);
            else
                playerBusinesses.Insert(slotIndex, newBusiness);

            OnBusinessPlaced?.Invoke(newBusiness);
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

            // Check max employees per business (from card data)
            if (business.businessCard.maxEmployees > 0 &&
                business.employees.Count >= business.businessCard.maxEmployees)
            {
                Debug.Log("[BoardManager] Business has reached maximum employees.");
                return false;
            }

            business.employees.Add(card);
            OnEmployeePlaced?.Invoke(card, businessIndex);
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

            if (businessIndex == -1)
            {
                // Global upgrade
                globalUpgrades.Add(card);
                OnUpgradePlaced?.Invoke(card, -1);
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
            OnUpgradePlaced?.Invoke(card, businessIndex);
            return true;
        }

        /// <summary>
        /// Removes an employee from a business by index.
        /// </summary>
        public void RemoveEmployee(int businessIndex, int employeeIndex)
        {
            if (businessIndex < 0 || businessIndex >= playerBusinesses.Count)
                return;

            ActiveBusiness business = playerBusinesses[businessIndex];

            if (employeeIndex < 0 || employeeIndex >= business.employees.Count)
                return;

            CardData removed = business.employees[employeeIndex];
            business.employees.RemoveAt(employeeIndex);

            Debug.Log($"[BoardManager] Removed employee '{(removed != null ? removed.cardName : "null")}' from business {businessIndex}.");
        }

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

            OnBusinessClosed?.Invoke(businessIndex);
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
        }

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
        /// Checks if any active card on the board has the specified tag.
        /// </summary>
        public bool HasTag(string tag)
        {
            foreach (var business in playerBusinesses)
            {
                if (business.isClosed)
                    continue;

                if (CardHasTag(business.businessCard, tag))
                    return true;

                foreach (var employee in business.employees)
                {
                    if (CardHasTag(employee, tag))
                        return true;
                }

                foreach (var upgrade in business.upgrades)
                {
                    if (CardHasTag(upgrade, tag))
                        return true;
                }
            }

            foreach (var upgrade in globalUpgrades)
            {
                if (CardHasTag(upgrade, tag))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns all card IDs currently active on the board. Useful for combo checking.
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

            return ids;
        }

        /// <summary>
        /// Ticks all closed businesses. Call at end of turn to count down closure timers.
        /// </summary>
        public void TickClosedBusinesses()
        {
            for (int i = 0; i < playerBusinesses.Count; i++)
            {
                var business = playerBusinesses[i];

                if (business.isClosed)
                {
                    business.closedTurnsRemaining--;

                    if (business.closedTurnsRemaining <= 0)
                    {
                        ReopenBusiness(i);
                    }
                }

                if (!business.isClosed)
                {
                    business.turnsActive++;
                }
            }
        }

        /// <summary>
        /// Sets the maximum number of business slots the player can use.
        /// </summary>
        public void SetMaxSlots(int slots)
        {
            maxSlots = Mathf.Max(1, slots);
        }

        private bool CardHasTag(CardData card, string tag)
        {
            if (card == null || card.tags == null)
                return false;

            foreach (string t in card.tags)
            {
                if (t == tag)
                    return true;
            }

            return false;
        }
    }
}
