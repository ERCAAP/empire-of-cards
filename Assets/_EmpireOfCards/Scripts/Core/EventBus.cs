using System;
using EmpireOfCards.Data;

namespace EmpireOfCards.Core
{
    public static class EventBus
    {
        // ── Turn Flow ───────────────────────────────────────────────

        public static event Action<int> OnTurnStarted;
        public static void TurnStarted(int turnNumber) => OnTurnStarted?.Invoke(turnNumber);

        public static event Action<int> OnTurnEnded;
        public static void TurnEnded(int turnNumber) => OnTurnEnded?.Invoke(turnNumber);

        public static event Action<TurnPhase> OnPhaseStarted;
        public static void PhaseStarted(TurnPhase phase) => OnPhaseStarted?.Invoke(phase);

        public static event Action<TurnPhase> OnPhaseEnded;
        public static void PhaseEnded(TurnPhase phase) => OnPhaseEnded?.Invoke(phase);

        public static event Action<Era> OnEraChanged;
        public static void EraChanged(Era newEra) => OnEraChanged?.Invoke(newEra);

        // ── Economy ─────────────────────────────────────────────────

        public static event Action<int> OnMoneyChanged;
        public static void MoneyChanged(int newAmount) => OnMoneyChanged?.Invoke(newAmount);

        public static event Action<int, int, int> OnIncomeCalculated;
        public static void IncomeCalculated(int gross, int expenses, int net)
            => OnIncomeCalculated?.Invoke(gross, expenses, net);

        public static event Action<float, float> OnMarketShareChanged;
        public static void MarketShareChanged(float playerShare, float rivalShare)
            => OnMarketShareChanged?.Invoke(playerShare, rivalShare);

        // ── Board ───────────────────────────────────────────────────

        public static event Action<CardData, SlotType> OnCardPlaced;
        public static void CardPlaced(CardData card, SlotType slot)
            => OnCardPlaced?.Invoke(card, slot);

        public static event Action<CardData, SlotType> OnCardRemoved;
        public static void CardRemoved(CardData card, SlotType slot)
            => OnCardRemoved?.Invoke(card, slot);

        public static event Action OnBoardTicked;
        public static void BoardTicked() => OnBoardTicked?.Invoke();

        public static event Action<ResolveStep> OnResolveStep;
        public static void ResolveStepFired(ResolveStep step) => OnResolveStep?.Invoke(step);

        public static event Action<SlotType, int> OnSlotUnlocked;
        public static void SlotUnlocked(SlotType slot, int newMax)
            => OnSlotUnlocked?.Invoke(slot, newMax);

        // ── Stats ───────────────────────────────────────────────────

        public static event Action<float> OnRatingChanged;
        public static void RatingChanged(float newRating) => OnRatingChanged?.Invoke(newRating);

        public static event Action<float> OnDemandChanged;
        public static void DemandChanged(float newDemand) => OnDemandChanged?.Invoke(newDemand);

        public static event Action<float> OnCapacityChanged;
        public static void CapacityChanged(float newCapacity) => OnCapacityChanged?.Invoke(newCapacity);

        public static event Action<float> OnQualityChanged;
        public static void QualityChanged(float newQuality) => OnQualityChanged?.Invoke(newQuality);

        public static event Action<float> OnHygieneChanged;
        public static void HygieneChanged(float newHygiene) => OnHygieneChanged?.Invoke(newHygiene);

        public static event Action<float> OnLegalRiskChanged;
        public static void LegalRiskChanged(float newRisk) => OnLegalRiskChanged?.Invoke(newRisk);

        public static event Action<float> OnStaffStabilityChanged;
        public static void StaffStabilityChanged(float newStability)
            => OnStaffStabilityChanged?.Invoke(newStability);

        // ── Staff ───────────────────────────────────────────────────

        public static event Action<string> OnStaffHired;
        public static void StaffHired(string staffName) => OnStaffHired?.Invoke(staffName);

        public static event Action<string, string> OnStaffQuit;
        public static void StaffQuit(string staffName, string reason)
            => OnStaffQuit?.Invoke(staffName, reason);

        public static event Action<string, StaffTier> OnStaffPromoted;
        public static void StaffPromoted(string staffName, StaffTier newTier)
            => OnStaffPromoted?.Invoke(staffName, newTier);

        public static event Action<string, float> OnMoraleChanged;
        public static void MoraleChanged(string staffName, float newMorale)
            => OnMoraleChanged?.Invoke(staffName, newMorale);

        // ── Deck / Hand ─────────────────────────────────────────────

        public static event Action<CardData[]> OnHandDrawn;
        public static void HandDrawn(CardData[] hand) => OnHandDrawn?.Invoke(hand);

        public static event Action OnHandDiscarded;
        public static void HandDiscarded() => OnHandDiscarded?.Invoke();

        public static event Action<CardData> OnCardDrawn;
        public static void CardDrawn(CardData card) => OnCardDrawn?.Invoke(card);

        public static event Action<CardData> OnCardDiscarded;
        public static void CardDiscarded(CardData card) => OnCardDiscarded?.Invoke(card);

        // ── Turn Report ─────────────────────────────────────────────

        public static event Action<int, int, int, float, int, int> OnTurnReport;
        public static void TurnReport(int income, int expense, int net, float ratingDelta, int served, int waited)
            => OnTurnReport?.Invoke(income, expense, net, ratingDelta, served, waited);

        // ── Customer ────────────────────────────────────────────────

        public static event Action<int, int, int> OnCustomersServed;
        public static void CustomersServed(int served, int waited, int left)
            => OnCustomersServed?.Invoke(served, waited, left);

        // ── Hygiene ─────────────────────────────────────────────────

        public static event Action<string> OnHygieneWarning;
        public static void HygieneWarning(string message) => OnHygieneWarning?.Invoke(message);

        // ── Crisis ──────────────────────────────────────────────────

        public static event Action<CrisisType> OnCrisisTriggered;
        public static void CrisisTriggered(CrisisType crisis) => OnCrisisTriggered?.Invoke(crisis);

        public static event Action<CrisisType, string> OnCrisisResolved;
        public static void CrisisResolved(CrisisType crisis, string choiceId)
            => OnCrisisResolved?.Invoke(crisis, choiceId);

        // ── Rival ───────────────────────────────────────────────────

        public static event Action<RivalMove, string> OnRivalAction;
        public static void RivalAction(RivalMove move, string description)
            => OnRivalAction?.Invoke(move, description);

        // ── Season ──────────────────────────────────────────────────

        public static event Action<SeasonType> OnSeasonChanged;
        public static void SeasonChanged(SeasonType newSeason) => OnSeasonChanged?.Invoke(newSeason);

        // ── Actions ─────────────────────────────────────────────────

        public static event Action<int> OnActionsChanged;
        public static void ActionsChanged(int remaining) => OnActionsChanged?.Invoke(remaining);

        // ── Game ────────────────────────────────────────────────────

        public static event Action<bool, string> OnGameOver;
        public static void GameOver(bool won, string reason) => OnGameOver?.Invoke(won, reason);

        // ── Cleanup ─────────────────────────────────────────────────

        public static void ClearAll()
        {
            // Turn Flow
            OnTurnStarted = null;
            OnTurnEnded = null;
            OnPhaseStarted = null;
            OnPhaseEnded = null;
            OnEraChanged = null;

            // Economy
            OnMoneyChanged = null;
            OnIncomeCalculated = null;
            OnMarketShareChanged = null;

            // Board
            OnCardPlaced = null;
            OnCardRemoved = null;
            OnBoardTicked = null;
            OnResolveStep = null;
            OnSlotUnlocked = null;

            // Stats
            OnRatingChanged = null;
            OnDemandChanged = null;
            OnCapacityChanged = null;
            OnQualityChanged = null;
            OnHygieneChanged = null;
            OnLegalRiskChanged = null;
            OnStaffStabilityChanged = null;

            // Staff
            OnStaffHired = null;
            OnStaffQuit = null;
            OnStaffPromoted = null;
            OnMoraleChanged = null;

            // Deck / Hand
            OnHandDrawn = null;
            OnHandDiscarded = null;
            OnCardDrawn = null;
            OnCardDiscarded = null;

            // Turn Report
            OnTurnReport = null;

            // Customer
            OnCustomersServed = null;

            // Hygiene
            OnHygieneWarning = null;

            // Crisis
            OnCrisisTriggered = null;
            OnCrisisResolved = null;

            // Rival
            OnRivalAction = null;

            // Season
            OnSeasonChanged = null;

            // Actions
            OnActionsChanged = null;

            // Game
            OnGameOver = null;
        }
    }
}
