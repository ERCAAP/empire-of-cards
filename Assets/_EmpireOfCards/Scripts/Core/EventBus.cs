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

        #region World Events
        public static event Action<CardData> OnEventActivated;             // event card
        public static event Action<CardData> OnEventExpired;
        #endregion

        #region Business Lifecycle Events
        public static event Action<int> OnBusinessClosed;                   // businessIndex
        public static event Action<int> OnBusinessReopened;
        public static event Action<CardData, int> OnEmployeeLeft;          // employee, businessIndex
        public static event Action<int, BusinessLevel> OnBusinessEvolved;   // businessIndex, newLevel
        public static event Action<int, int> OnBusinessNeglected;          // businessIndex, neglectTurns
        #endregion

        #region Market Block Events
        public static event Action<int, int> OnMarketBlocksChanged;        // playerBlocks, rivalBlocks
        #endregion

        #region Board Slot Events
        public static event Action<int> OnBusinessSlotsChanged;            // newMaxSlots (legacy)
        public static event Action<CardData, SlotType> OnCardPlacedInSlot; // card, slotType
        public static event Action<CardData, SlotType> OnCardRemovedFromSlot;
        public static event Action<SlotType, int> OnSlotExpanded;          // slotType, newCount
        #endregion

        #region Legal Risk Events
        public static event Action<int> OnLegalRiskChanged;                // newRiskScore (0-100)
        public static event Action<LegalRiskLevel> OnLegalRiskLevelChanged;
        public static event Action<int> OnLegalRaidOccurred;               // penalty amount
        #endregion

        #region Platform Rating Events
        public static event Action<float> OnPlatformRatingChanged;         // newRating (1.0-5.0)
        #endregion

        #region Cash Flow Events
        public static event Action<int> OnCashBalanceChanged;              // newBalance
        public static event Action OnCashCrisisStarted;
        public static event Action OnCashCrisisResolved;
        #endregion

        #region Season Events
        public static event Action<SeasonType, int> OnSeasonChanged;       // season, seasonIndex
        #endregion

        #region Customer Market Events
        public static event Action<int, int> OnMarketShareChanged;         // playerCount, rivalCount
        #endregion

        #region Turn Messaging Events
        public static event Action<TurnBriefData> OnTurnBriefGenerated;
        public static event Action<TurnReportData> OnTurnReportGenerated;
        #endregion

        #region Question System Events
        public static event Action<QuestionRuntimeState[]> OnQuestionsGenerated;
        public static event Action<int, QuestionRuntimeState> OnQuestionAnswered;
        public static event Action<int, QuestionRuntimeState> OnQuestionResolved;
        public static event Action<DecisionRecord> OnDecisionRecorded;
        public static event Action<CustomerFlowSnapshot> OnCustomerFlowResolved;
        public static event Action<CameraFocusRequest> OnQuestionFocusRequested;
        public static event Action OnQuestionFocusReleased;
        public static event Action<BoardAnchorViewModel> OnBoardAnchorStateChanged;
        public static event Action<RivalPressureViewModel> OnRivalPressureVisualChanged;
        public static event Action<TurnResolutionReport> OnTurnResolutionReady;
        #endregion

        #region Economy Events
        public static event Action<int> OnMoneyChanged;                    // newAmount
        public static event Action<int> OnIncomeReceived;                  // amount
        public static event Action<int> OnMoneySpent;                      // amount
        public static event Action<IncomeBreakdown> OnIncomeBreakdown;     // detailed cascade
        public static event Action<SalaryChoice, int> OnSalaryPaid;        // choice, amountPaid
        public static event Action<int> OnSalaryChoiceRequired;            // totalSalaries
        public static event Action<CreditType, int> OnCreditTaken;         // type, amount
        public static event Action<CreditType> OnCreditRepaid;             // type
        public static event Action<int> OnRentCharged;                     // rent amount
        public static event Action<int> OnPayrollDefaulted;                // unpaid payroll amount
        public static event Action<VentureType, int> OnStockSpoilageOccurred;    // venture, cost
        public static event Action<VentureType, int> OnStockSeasonLossOccurred;  // venture, cost
        public static event Action<int, float> OnInflationOccurred;        // currentTurn, increase
        public static event Action<VentureType, int> OnSupplierFailed;     // venture, penalty cost
        public static event Action<string, int> OnInspectionTriggered;     // label, turns
        public static event Action<string> OnInspectionResolved;           // label
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
        public static event Action<string> OnRivalMoodChanged;             // mood icon string
        public static event Action<string> OnRivalStrategyComment;         // one-time strategy remark
        public static event Action<RivalQueuedAction> OnRivalActionQueued; // lane/card preview
        #endregion

        #region Staff State Events (GDD Section 6)
        public static event Action<CardData, int, int, int, int> OnStaffStateUpdated; // card, moral, fatigue, loyalty, xp
        public static event Action<CardData, int> OnStaffLeveledUp;                    // card, newLevel
        public static event Action<CardData> OnStaffStrike;                             // striking employee
        public static event Action<CardData> OnStaffStolenByRival;                      // stolen employee
        public static event Action OnOvertimeApplied;                                   // overtime used this turn
        public static event Action<StaffApplicant[]> OnApplicantPoolGenerated;
        public static event Action<HiringDecisionResult> OnHiringDecisionResolved;
        public static event Action<StaffWorkloadReport> OnStaffWorkloadChanged;
        public static event Action<CardData, float> OnStaffBurnoutRiskChanged;
        public static event Action<CardData, QuitReason> OnStaffQuit;
        public static event Action<CardData> OnStaffTrialCompleted;
        #endregion

        #region Chain Reaction Events (GDD Section 11, 12)
        public static event Action<string, int> OnChainReactionTriggered;  // chainId, severity/turns
        public static event Action<string> OnDeterministicEventTriggered;  // eventId (QualityCrisis, StaffStrike, etc.)
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

        #region World Event Invoke Helpers
        public static void EventActivated(CardData card) => OnEventActivated?.Invoke(card);
        public static void EventExpired(CardData card) => OnEventExpired?.Invoke(card);
        #endregion

        #region Business Lifecycle Invoke Helpers
        public static void BusinessClosed(int idx) => OnBusinessClosed?.Invoke(idx);
        public static void BusinessReopened(int idx) => OnBusinessReopened?.Invoke(idx);
        public static void EmployeeLeft(CardData emp, int bizIdx) => OnEmployeeLeft?.Invoke(emp, bizIdx);
        public static void BusinessEvolved(int idx, BusinessLevel lvl) => OnBusinessEvolved?.Invoke(idx, lvl);
        public static void BusinessNeglected(int idx, int turns) => OnBusinessNeglected?.Invoke(idx, turns);
        #endregion

        #region Market Block Invoke Helpers
        public static void MarketBlocksUpdated(int p, int r) => OnMarketBlocksChanged?.Invoke(p, r);
        #endregion

        #region Board Slot Invoke Helpers
        public static void BusinessSlotsChanged(int maxSlots) => OnBusinessSlotsChanged?.Invoke(maxSlots);
        public static void CardPlacedInSlot(CardData card, SlotType slot) => OnCardPlacedInSlot?.Invoke(card, slot);
        public static void CardRemovedFromSlot(CardData card, SlotType slot) => OnCardRemovedFromSlot?.Invoke(card, slot);
        public static void SlotExpanded(SlotType slot, int newCount) => OnSlotExpanded?.Invoke(slot, newCount);
        #endregion

        #region Legal Risk Invoke Helpers
        public static void LegalRiskUpdated(int score) => OnLegalRiskChanged?.Invoke(score);
        public static void LegalRiskLevelUpdated(LegalRiskLevel level) => OnLegalRiskLevelChanged?.Invoke(level);
        public static void LegalRaidOccurred(int penalty) => OnLegalRaidOccurred?.Invoke(penalty);
        #endregion

        #region Season Invoke Helpers
        public static void SeasonChanged(SeasonType season, int index) => OnSeasonChanged?.Invoke(season, index);
        #endregion

        #region Platform Rating Invoke Helpers
        public static void PlatformRatingChanged(float rating) => OnPlatformRatingChanged?.Invoke(rating);
        #endregion

        #region Cash Flow Invoke Helpers
        public static void CashBalanceChanged(int balance) => OnCashBalanceChanged?.Invoke(balance);
        public static void CashCrisisStarted() => OnCashCrisisStarted?.Invoke();
        public static void CashCrisisResolved() => OnCashCrisisResolved?.Invoke();
        #endregion

        #region Customer Market Invoke Helpers
        public static void MarketShareUpdated(int player, int rival) => OnMarketShareChanged?.Invoke(player, rival);
        #endregion

        #region Turn Messaging Invoke Helpers
        public static void TurnBriefGenerated(TurnBriefData brief) => OnTurnBriefGenerated?.Invoke(brief);
        public static void TurnReportGenerated(TurnReportData report) => OnTurnReportGenerated?.Invoke(report);
        #endregion

        #region Question System Invoke Helpers
        public static void QuestionsGenerated(QuestionRuntimeState[] questions) => OnQuestionsGenerated?.Invoke(questions);
        public static void QuestionAnswered(int index, QuestionRuntimeState question) => OnQuestionAnswered?.Invoke(index, question);
        public static void QuestionResolved(int index, QuestionRuntimeState question) => OnQuestionResolved?.Invoke(index, question);
        public static void DecisionRecorded(DecisionRecord record) => OnDecisionRecorded?.Invoke(record);
        public static void CustomerFlowResolved(CustomerFlowSnapshot snapshot) => OnCustomerFlowResolved?.Invoke(snapshot);
        public static void QuestionFocusRequested(CameraFocusRequest request) => OnQuestionFocusRequested?.Invoke(request);
        public static void QuestionFocusReleased() => OnQuestionFocusReleased?.Invoke();
        public static void BoardAnchorStateChanged(BoardAnchorViewModel state) => OnBoardAnchorStateChanged?.Invoke(state);
        public static void RivalPressureVisualChanged(RivalPressureViewModel state) => OnRivalPressureVisualChanged?.Invoke(state);
        public static void TurnResolutionReady(TurnResolutionReport report) => OnTurnResolutionReady?.Invoke(report);
        #endregion

        #region Economy Invoke Helpers
        public static void MoneyUpdated(int amount) => OnMoneyChanged?.Invoke(amount);
        public static void IncomeReceived(int amount) => OnIncomeReceived?.Invoke(amount);
        public static void MoneySpent(int amount) => OnMoneySpent?.Invoke(amount);
        public static void IncomeBreakdownReported(IncomeBreakdown breakdown) => OnIncomeBreakdown?.Invoke(breakdown);
        public static void SalaryPaid(SalaryChoice choice, int amountPaid) => OnSalaryPaid?.Invoke(choice, amountPaid);
        public static void SalaryChoiceRequired(int totalSalaries) => OnSalaryChoiceRequired?.Invoke(totalSalaries);
        public static void CreditTaken(CreditType type, int amount) => OnCreditTaken?.Invoke(type, amount);
        public static void CreditRepaid(CreditType type) => OnCreditRepaid?.Invoke(type);
        public static void RentCharged(int amount) => OnRentCharged?.Invoke(amount);
        public static void PayrollDefaulted(int unpaidAmount) => OnPayrollDefaulted?.Invoke(unpaidAmount);
        public static void StockSpoilageOccurred(VentureType venture, int cost) => OnStockSpoilageOccurred?.Invoke(venture, cost);
        public static void StockSeasonLossOccurred(VentureType venture, int cost) => OnStockSeasonLossOccurred?.Invoke(venture, cost);
        public static void InflationOccurred(int currentTurn, float increase) => OnInflationOccurred?.Invoke(currentTurn, increase);
        public static void SupplierFailed(VentureType venture, int penaltyCost) => OnSupplierFailed?.Invoke(venture, penaltyCost);
        public static void InspectionTriggered(string label, int turns) => OnInspectionTriggered?.Invoke(label, turns);
        public static void InspectionResolved(string label) => OnInspectionResolved?.Invoke(label);
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
        public static void RivalMoodChanged(string moodIcon) => OnRivalMoodChanged?.Invoke(moodIcon);
        public static void RivalStrategyCommented(string comment) => OnRivalStrategyComment?.Invoke(comment);
        public static void RivalActionQueued(RivalQueuedAction action) => OnRivalActionQueued?.Invoke(action);
        #endregion

        #region Staff State Invoke Helpers
        public static void StaffStateUpdated(CardData card, int moral, int fatigue, int loyalty, int xp) => OnStaffStateUpdated?.Invoke(card, moral, fatigue, loyalty, xp);
        public static void StaffLeveledUp(CardData card, int level) => OnStaffLeveledUp?.Invoke(card, level);
        public static void StaffStrikeTriggered(CardData card) => OnStaffStrike?.Invoke(card);
        public static void StaffStolenByRival(CardData card) => OnStaffStolenByRival?.Invoke(card);
        public static void OvertimeApplied() => OnOvertimeApplied?.Invoke();
        public static void ApplicantPoolGenerated(StaffApplicant[] applicants) => OnApplicantPoolGenerated?.Invoke(applicants);
        public static void HiringDecisionResolved(HiringDecisionResult result) => OnHiringDecisionResolved?.Invoke(result);
        public static void StaffWorkloadChanged(StaffWorkloadReport report) => OnStaffWorkloadChanged?.Invoke(report);
        public static void StaffBurnoutRiskChanged(CardData card, float risk) => OnStaffBurnoutRiskChanged?.Invoke(card, risk);
        public static void StaffQuit(CardData card, QuitReason reason) => OnStaffQuit?.Invoke(card, reason);
        public static void StaffTrialCompleted(CardData card) => OnStaffTrialCompleted?.Invoke(card);
        #endregion

        #region Chain Reaction Invoke Helpers
        public static void ChainReactionTriggered(string chainId, int severity) => OnChainReactionTriggered?.Invoke(chainId, severity);
        public static void DeterministicEventTriggered(string eventId) => OnDeterministicEventTriggered?.Invoke(eventId);
        #endregion

        #region Headhunting Invoke Helpers (GDD 6.5)
        public static event Action<CardData, int> OnStaffPoachAttempted;
        public static event Action<CardData> OnStaffPoachAccepted;
        public static event Action<CardData, int> OnStaffPoachCountered;
        public static event Action<CardData, int> OnStaffPoachRejected;
        public static void StaffPoachAttempted(CardData card, int offer) => OnStaffPoachAttempted?.Invoke(card, offer);
        public static void StaffPoachAccepted(CardData card) => OnStaffPoachAccepted?.Invoke(card);
        public static void StaffPoachCountered(CardData card, int cost) => OnStaffPoachCountered?.Invoke(card, cost);
        public static void StaffPoachRejected(CardData card, int cost) => OnStaffPoachRejected?.Invoke(card, cost);
        #endregion

        #region Tax Period Invoke Helpers (GDD 5.8)
        public static event Action<int, int> OnTaxPeriodProcessed;
        public static event Action<int> OnTaxAuditTriggered;
        public static void TaxPeriodProcessed(int taxOwed, int amountPaid) => OnTaxPeriodProcessed?.Invoke(taxOwed, amountPaid);
        public static void TaxAuditTriggered(int unpaidDebt) => OnTaxAuditTriggered?.Invoke(unpaidDebt);
        #endregion

        #region Customer Loyalty Invoke Helpers (GDD 7.3)
        public static event Action<float> OnLoyaltyScoreChanged;
        public static event Action<int> OnOrganicCustomersGained;
        public static void LoyaltyScoreChanged(float score) => OnLoyaltyScoreChanged?.Invoke(score);
        public static void OrganicCustomersGained(int count) => OnOrganicCustomersGained?.Invoke(count);
        #endregion

        #region Rating Recovery Invoke Helpers (GDD 8.4)
        public static event Action<float> OnRatingRecoveryApplied;
        public static void RatingRecoveryApplied(float delta) => OnRatingRecoveryApplied?.Invoke(delta);
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

            // World events
            OnEventActivated = null;
            OnEventExpired = null;

            // Business lifecycle
            OnBusinessClosed = null;
            OnBusinessReopened = null;
            OnEmployeeLeft = null;
            OnBusinessEvolved = null;
            OnBusinessNeglected = null;

            // Market Blocks
            OnMarketBlocksChanged = null;

            // Board Slots
            OnBusinessSlotsChanged = null;
            OnCardPlacedInSlot = null;
            OnCardRemovedFromSlot = null;
            OnSlotExpanded = null;

            // Legal Risk
            OnLegalRiskChanged = null;
            OnLegalRiskLevelChanged = null;
            OnLegalRaidOccurred = null;

            // Seasons
            OnSeasonChanged = null;

            // Customer Market
            OnMarketShareChanged = null;

            // Turn messaging
            OnTurnBriefGenerated = null;
            OnTurnReportGenerated = null;

            // Question system
            OnQuestionsGenerated = null;
            OnQuestionAnswered = null;
            OnQuestionResolved = null;
            OnDecisionRecorded = null;
            OnCustomerFlowResolved = null;
            OnQuestionFocusRequested = null;
            OnQuestionFocusReleased = null;
            OnBoardAnchorStateChanged = null;
            OnRivalPressureVisualChanged = null;
            OnTurnResolutionReady = null;

            // Platform Rating
            OnPlatformRatingChanged = null;

            // Cash Flow
            OnCashBalanceChanged = null;
            OnCashCrisisStarted = null;
            OnCashCrisisResolved = null;

            // Economy
            OnMoneyChanged = null;
            OnIncomeReceived = null;
            OnMoneySpent = null;
            OnIncomeBreakdown = null;
            OnSalaryPaid = null;
            OnSalaryChoiceRequired = null;
            OnCreditTaken = null;
            OnCreditRepaid = null;
            OnRentCharged = null;
            OnPayrollDefaulted = null;
            OnStockSpoilageOccurred = null;
            OnStockSeasonLossOccurred = null;
            OnInflationOccurred = null;
            OnSupplierFailed = null;
            OnInspectionTriggered = null;
            OnInspectionResolved = null;

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
            OnRivalMoodChanged = null;
            OnRivalStrategyComment = null;
            OnRivalActionQueued = null;

            // Staff State
            OnStaffStateUpdated = null;
            OnStaffLeveledUp = null;
            OnStaffStrike = null;
            OnStaffStolenByRival = null;
            OnOvertimeApplied = null;
            OnApplicantPoolGenerated = null;
            OnHiringDecisionResolved = null;
            OnStaffWorkloadChanged = null;
            OnStaffBurnoutRiskChanged = null;
            OnStaffQuit = null;
            OnStaffTrialCompleted = null;

            // Chain Reaction
            OnChainReactionTriggered = null;
            OnDeterministicEventTriggered = null;

            // Headhunting
            OnStaffPoachAttempted = null;
            OnStaffPoachAccepted = null;
            OnStaffPoachCountered = null;
            OnStaffPoachRejected = null;

            // Tax Period
            OnTaxPeriodProcessed = null;
            OnTaxAuditTriggered = null;

            // Customer Loyalty
            OnLoyaltyScoreChanged = null;
            OnOrganicCustomersGained = null;

            // Rating Recovery
            OnRatingRecoveryApplied = null;
        }
    }
}
