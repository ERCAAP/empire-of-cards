using System;

namespace EmpireOfCards.Core
{
    /// <summary>
    /// Static event bus for decoupled cross-system communication.
    /// Subscribe in OnEnable, unsubscribe in OnDisable to avoid leaks.
    /// </summary>
    public static class EventBus
    {
        // --- Card Events ---

        /// <summary>Fired when any card is played from hand. Parameter: card instance id.</summary>
        public static event Action<int> OnCardPlayed;

        /// <summary>Fired when a card is drawn into the hand. Parameter: card instance id.</summary>
        public static event Action<int> OnCardDrawn;

        // --- Combo Events ---

        /// <summary>Fired when a combo is triggered. Parameter: combo name.</summary>
        public static event Action<string> OnComboTriggered;

        // --- Placement Events ---

        /// <summary>Fired when a business card is placed on the board. Parameter: card instance id.</summary>
        public static event Action<int> OnBusinessPlaced;

        /// <summary>Fired when an employee card is assigned. Parameter: card instance id.</summary>
        public static event Action<int> OnEmployeePlaced;

        /// <summary>Fired when an upgrade is applied to a business. Parameters: upgrade id, target business id.</summary>
        public static event Action<int, int> OnUpgradeApplied;

        /// <summary>Fired when an action card is played. Parameter: card instance id.</summary>
        public static event Action<int> OnActionPlayed;

        // --- World Events ---

        /// <summary>Fired when a random event is triggered. Parameter: event name.</summary>
        public static event Action<string> OnEventTriggered;

        /// <summary>Fired when an FBI raid occurs. Parameter: penalty amount.</summary>
        public static event Action<int> OnFBIRaid;

        /// <summary>Fired when a business is shut down. Parameter: card instance id.</summary>
        public static event Action<int> OnBusinessClosed;

        /// <summary>Fired when an employee leaves. Parameter: card instance id.</summary>
        public static event Action<int> OnEmployeeLeft;

        // --- Territory Events ---

        /// <summary>Fired when territory ownership changes. Parameters: player territories, rival territories.</summary>
        public static event Action<int, int> OnTerritoryChanged;

        // --- Invoke Helpers ---

        public static void CardPlayed(int cardId) => OnCardPlayed?.Invoke(cardId);
        public static void CardDrawn(int cardId) => OnCardDrawn?.Invoke(cardId);
        public static void ComboTriggered(string comboName) => OnComboTriggered?.Invoke(comboName);
        public static void BusinessPlaced(int cardId) => OnBusinessPlaced?.Invoke(cardId);
        public static void EmployeePlaced(int cardId) => OnEmployeePlaced?.Invoke(cardId);
        public static void UpgradeApplied(int upgradeId, int businessId) => OnUpgradeApplied?.Invoke(upgradeId, businessId);
        public static void ActionPlayed(int cardId) => OnActionPlayed?.Invoke(cardId);
        public static void EventTriggered(string eventName) => OnEventTriggered?.Invoke(eventName);
        public static void FBIRaid(int penalty) => OnFBIRaid?.Invoke(penalty);
        public static void BusinessClosed(int cardId) => OnBusinessClosed?.Invoke(cardId);
        public static void EmployeeLeft(int cardId) => OnEmployeeLeft?.Invoke(cardId);
        public static void TerritoryChanged(int playerTerritories, int rivalTerritories) => OnTerritoryChanged?.Invoke(playerTerritories, rivalTerritories);

        /// <summary>
        /// Clears all subscribers. Call when returning to the main menu or ending a run.
        /// </summary>
        public static void ClearAll()
        {
            OnCardPlayed = null;
            OnCardDrawn = null;
            OnComboTriggered = null;
            OnBusinessPlaced = null;
            OnEmployeePlaced = null;
            OnUpgradeApplied = null;
            OnActionPlayed = null;
            OnEventTriggered = null;
            OnFBIRaid = null;
            OnBusinessClosed = null;
            OnEmployeeLeft = null;
            OnTerritoryChanged = null;
        }
    }
}
