using System;
using EmpireOfCards.Data;

namespace EmpireOfCards.Core
{
    /// <summary>
    /// Central event hub. All game systems publish/subscribe through here.
    /// Organized by domain with matching invoke helpers.
    /// Call ClearAll() on scene unload to prevent stale subscriptions.
    /// </summary>
    public static class EventBus
    {
        // ======================================================================
        //  EVENTS
        // ======================================================================

        #region Card Events
        public static event Action<CardData> OnCardPlayed;
        public static event Action<CardData> OnCardDrawn;
        public static event Action<CardData> OnCardDiscarded;
        public static event Action<CardData> OnCardBurned;
        #endregion

        #region Board Events
        public static event Action<CardData, int> OnBusinessPlaced;        // card, slotIndex
        public static event Action<CardData, int> OnEmployeePlaced;        // card, businessIndex
        public static event Action<CardData, int> OnUpgradePlaced;         // card, businessIndex (-1=global)
        public static event Action<CardData> OnActionExecuted;
        #endregion

        #region Combo Events
        public static event Action<ComboData> OnComboTriggered;
        public static event Action<ComboData> OnComboDeactivated;
        #endregion

        #region World Events
        public static event Action<CardData> OnEventActivated;             // event card
        public static event Action<CardData> OnEventExpired;
        #endregion

        #region FBI Events
        public static event Action<int> OnFBIRaid;                         // penalty amount
        public static event Action OnFBIRaidAvoided;
        public static event Action<float> OnFBIRiskChanged;                // new risk value
        #endregion

        #region Business Lifecycle Events
        public static event Action<int> OnBusinessClosed;                   // businessIndex
        public static event Action<int> OnBusinessReopened;
        public static event Action<CardData, int> OnEmployeeLeft;          // employee, businessIndex
        public static event Action<int, BusinessLevel> OnBusinessEvolved;   // businessIndex, newLevel
        public static event Action<int, int> OnBusinessNeglected;          // businessIndex, neglectTurns
        #endregion

        #region Territory Events
        public static event Action<int, int> OnTerritoryChanged;           // playerCount, rivalCount
        #endregion

        #region Economy Events
        public static event Action<int> OnMoneyChanged;                    // newAmount
        public static event Action<int> OnIncomeReceived;                  // amount
        public static event Action<int> OnMoneySpent;                      // amount
        #endregion

        #region Turn Flow Events
        public static event Action<int> OnTurnStarted;                     // turnNumber
        public static event Action<int> OnTurnEnded;                       // turnNumber
        public static event Action<TurnPhase> OnPhaseStarted;
        public static event Action<TurnPhase> OnPhaseEnded;
        #endregion

        #region Game State Events
        public static event Action<GameState> OnGameStateChanged;
        public static event Action<bool> OnGameOver;                       // won
        #endregion

        #region Rival Events
        public static event Action<string> OnRivalAction;                  // description
        public static event Action<string> OnRivalTaunt;                   // taunt text
        #endregion

        #region Company Tier Events
        public static event Action<CompanyTier> OnCompanyTierChanged;      // new tier
        #endregion

        // ======================================================================
        //  INVOKE HELPERS
        // ======================================================================

        #region Card Invoke Helpers
        public static void CardPlayed(CardData card) => OnCardPlayed?.Invoke(card);
        public static void CardDrawn(CardData card) => OnCardDrawn?.Invoke(card);
        public static void CardDiscarded(CardData card) => OnCardDiscarded?.Invoke(card);
        public static void CardBurned(CardData card) => OnCardBurned?.Invoke(card);
        #endregion

        #region Board Invoke Helpers
        public static void BusinessPlaced(CardData card, int slot) => OnBusinessPlaced?.Invoke(card, slot);
        public static void EmployeePlaced(CardData card, int bizIdx) => OnEmployeePlaced?.Invoke(card, bizIdx);
        public static void UpgradePlaced(CardData card, int bizIdx) => OnUpgradePlaced?.Invoke(card, bizIdx);
        public static void ActionExecuted(CardData card) => OnActionExecuted?.Invoke(card);
        #endregion

        #region Combo Invoke Helpers
        public static void ComboTriggered(ComboData combo) => OnComboTriggered?.Invoke(combo);
        public static void ComboDeactivated(ComboData combo) => OnComboDeactivated?.Invoke(combo);
        #endregion

        #region World Event Invoke Helpers
        public static void EventActivated(CardData card) => OnEventActivated?.Invoke(card);
        public static void EventExpired(CardData card) => OnEventExpired?.Invoke(card);
        #endregion

        #region FBI Invoke Helpers
        public static void FBIRaidOccurred(int penalty) => OnFBIRaid?.Invoke(penalty);
        public static void FBIRaidWasAvoided() => OnFBIRaidAvoided?.Invoke();
        public static void FBIRiskUpdated(float risk) => OnFBIRiskChanged?.Invoke(risk);
        #endregion

        #region Business Lifecycle Invoke Helpers
        public static void BusinessClosed(int idx) => OnBusinessClosed?.Invoke(idx);
        public static void BusinessReopened(int idx) => OnBusinessReopened?.Invoke(idx);
        public static void EmployeeLeft(CardData emp, int bizIdx) => OnEmployeeLeft?.Invoke(emp, bizIdx);
        public static void BusinessEvolved(int idx, BusinessLevel lvl) => OnBusinessEvolved?.Invoke(idx, lvl);
        public static void BusinessNeglected(int idx, int turns) => OnBusinessNeglected?.Invoke(idx, turns);
        #endregion

        #region Territory Invoke Helpers
        public static void TerritoryUpdated(int p, int r) => OnTerritoryChanged?.Invoke(p, r);
        #endregion

        #region Economy Invoke Helpers
        public static void MoneyUpdated(int amount) => OnMoneyChanged?.Invoke(amount);
        public static void IncomeReceived(int amount) => OnIncomeReceived?.Invoke(amount);
        public static void MoneySpent(int amount) => OnMoneySpent?.Invoke(amount);
        #endregion

        #region Turn Flow Invoke Helpers
        public static void TurnStarted(int turn) => OnTurnStarted?.Invoke(turn);
        public static void TurnEnded(int turn) => OnTurnEnded?.Invoke(turn);
        public static void PhaseStarted(TurnPhase phase) => OnPhaseStarted?.Invoke(phase);
        public static void PhaseEnded(TurnPhase phase) => OnPhaseEnded?.Invoke(phase);
        #endregion

        #region Game State Invoke Helpers
        public static void GameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
        public static void GameEnded(bool won) => OnGameOver?.Invoke(won);
        #endregion

        #region Rival Invoke Helpers
        public static void RivalActed(string desc) => OnRivalAction?.Invoke(desc);
        public static void RivalTaunted(string taunt) => OnRivalTaunt?.Invoke(taunt);
        #endregion

        #region Company Tier Invoke Helpers
        public static void CompanyTierChanged(CompanyTier tier) => OnCompanyTierChanged?.Invoke(tier);
        #endregion

        // ======================================================================
        //  CLEANUP
        // ======================================================================

        /// <summary>
        /// Nulls every event delegate. Call on scene unload to avoid
        /// stale references from destroyed MonoBehaviours.
        /// </summary>
        public static void ClearAll()
        {
            // Card
            OnCardPlayed = null;
            OnCardDrawn = null;
            OnCardDiscarded = null;
            OnCardBurned = null;

            // Board
            OnBusinessPlaced = null;
            OnEmployeePlaced = null;
            OnUpgradePlaced = null;
            OnActionExecuted = null;

            // Combo
            OnComboTriggered = null;
            OnComboDeactivated = null;

            // World events
            OnEventActivated = null;
            OnEventExpired = null;

            // FBI
            OnFBIRaid = null;
            OnFBIRaidAvoided = null;
            OnFBIRiskChanged = null;

            // Business lifecycle
            OnBusinessClosed = null;
            OnBusinessReopened = null;
            OnEmployeeLeft = null;
            OnBusinessEvolved = null;
            OnBusinessNeglected = null;

            // Territory
            OnTerritoryChanged = null;

            // Economy
            OnMoneyChanged = null;
            OnIncomeReceived = null;
            OnMoneySpent = null;

            // Turn flow
            OnTurnStarted = null;
            OnTurnEnded = null;
            OnPhaseStarted = null;
            OnPhaseEnded = null;

            // Game state
            OnGameStateChanged = null;
            OnGameOver = null;

            // Rival
            OnRivalAction = null;
            OnRivalTaunt = null;

            // Company Tier
            OnCompanyTierChanged = null;
        }
    }
}
