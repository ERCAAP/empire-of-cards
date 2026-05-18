using UnityEngine;
using EmpireOfCards.Data;

namespace EmpireOfCards.Core
{
    /// <summary>
    /// Owns all mutable player-economy state: money, actions, business slots.
    /// Extracted from GameManager to give it a single responsibility.
    /// </summary>
    [System.Serializable]
    public class PlayerResources
    {
        [SerializeField] private int money;
        [SerializeField] private int actions;
        [SerializeField] private int maxActions;
        [SerializeField] private int businessSlots;

        public int Money => money;
        public int Actions => actions;
        public int MaxActions => maxActions;
        public int BusinessSlots => businessSlots;

        // === Initialization ===

        /// <summary>
        /// Resets all resource values to their starting state from balance data (or Constants fallback).
        /// </summary>
        public void Reset(GameBalanceData balanceData)
        {
            money = balanceData != null ? balanceData.startingMoney : Constants.STARTING_MONEY;
            actions = balanceData != null ? balanceData.startingActions : Constants.STARTING_ACTIONS;
            maxActions = actions;
            businessSlots = balanceData != null ? balanceData.startingBusinessSlots : Constants.STARTING_SLOTS;
        }

        // === Money ===

        /// <summary>
        /// Attempts to spend the given amount. Returns true on success.
        /// </summary>
        public bool SpendMoney(int amount)
        {
            if (amount < 0 || money < amount) return false;
            money -= amount;
            EventBus.MoneyUpdated(money);
            EventBus.MoneySpent(amount);
            return true;
        }

        /// <summary>
        /// Adds money to the player's total.
        /// </summary>
        public void GainMoney(int amount)
        {
            if (amount <= 0) return;
            money += amount;
            EventBus.MoneyUpdated(money);
            EventBus.IncomeReceived(amount);
        }

        /// <summary>
        /// Net change that can go negative (salary payments, penalties, etc).
        /// </summary>
        public void AdjustMoney(int netAmount)
        {
            money += netAmount;
            EventBus.MoneyUpdated(money);
        }

        // === Actions ===

        /// <summary>
        /// Consumes one action point. Returns true if the player had actions remaining.
        /// </summary>
        public bool UseAction()
        {
            if (actions <= 0) return false;
            actions--;
            return true;
        }

        /// <summary>
        /// Resets action points to maxActions at the start of a turn.
        /// </summary>
        public void ResetActions()
        {
            actions = maxActions;
        }

        /// <summary>
        /// Adds extra action capacity (e.g. from AI Assistant upgrade).
        /// Capped at balance max.
        /// </summary>
        public void AddExtraAction(int count, GameBalanceData balanceData)
        {
            int cap = balanceData != null ? balanceData.maxActions : Constants.MAX_ACTIONS;
            maxActions = Mathf.Min(maxActions + count, cap);
            actions = Mathf.Min(actions + count, maxActions);
        }

        // === Business Slots ===

        /// <summary>
        /// Adds one business slot, capped at balance max.
        /// </summary>
        public void AddBusinessSlot(GameBalanceData balanceData)
        {
            int max = balanceData != null ? balanceData.maxBusinessSlots : Constants.MAX_SLOTS;
            businessSlots = Mathf.Min(businessSlots + 1, max);
        }
    }
}
