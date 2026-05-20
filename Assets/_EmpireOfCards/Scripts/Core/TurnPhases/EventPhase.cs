using UnityEngine;
using EmpireOfCards.Core.StateMachine;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 1: Check if a world event triggers this turn (GDD Section 4.1, 12.2).
    /// Deterministic events fire when conditions are met (always).
    /// Random events fire on turns: 3, 6, 9, 12, 15, 18.
    /// </summary>
    public class EventPhase : IState
    {
        private readonly TurnManager _turnManager;
        private float _timer;

        public EventPhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _timer = 0f;

            var gm = GameManager.Instance;

            // --- Deterministic Events (GDD 12.2) - always check, fire when conditions met ---
            CheckDeterministicEvents(gm);

            // --- Random Events - fire every EVENT_INTERVAL turns ---
            int interval = gm.BalanceData != null
                ? gm.BalanceData.eventInterval
                : Constants.EVENT_INTERVAL;

            bool shouldFireEvent = _turnManager.CurrentTurnNumber > 0
                && _turnManager.CurrentTurnNumber % interval == 0;

            if (shouldFireEvent && _turnManager.ActiveEvent == null)
            {
                var eventCard = _turnManager.DrawRandomEvent();
                if (eventCard != null)
                {
                    _turnManager.SetActiveEvent(eventCard);
                    Debug.Log($"[EventPhase] Random event activated: {eventCard.cardName} (duration: {eventCard.eventDuration} turns)");
                }
                else
                {
                    Debug.LogWarning("[EventPhase] Event should fire but event deck is empty.");
                }
            }
        }

        /// <summary>
        /// Checks deterministic event triggers based on chain reaction state (GDD 12.2).
        /// These fire ON TOP of random events when conditions are met.
        /// </summary>
        private void CheckDeterministicEvents(GameManager gm)
        {
            if (gm == null) return;

            var chain = gm.ChainReactionSystem;
            if (chain == null) return;

            // Cheap supplier 4+ turns -> QualityCrisis
            if (chain.CheapSupplierTurns >= Constants.CHAIN_CHEAP_SUPPLIER_THRESHOLD)
            {
                EventBus.DeterministicEventTriggered("QualityCrisis");
                Debug.Log("[EventPhase] Deterministic: QualityCrisis triggered (cheap supplier chain)");
            }

            // Salary delayed 3+ turns -> StaffStrike
            if (chain.SalaryDelayedTurns >= Constants.CHAIN_SALARY_DELAY_THRESHOLD)
            {
                EventBus.DeterministicEventTriggered("StaffStrike");
                Debug.Log("[EventPhase] Deterministic: StaffStrike triggered (salary delay chain)");
            }

            // Platform rating <= 3.0 -> ReputationCrisis
            if (chain.IsPlatformRatingCritical())
            {
                EventBus.DeterministicEventTriggered("ReputationCrisis");
                Debug.Log("[EventPhase] Deterministic: ReputationCrisis triggered (low platform rating)");
            }

            // Tax unpaid 2+ turns -> TaxAudit
            if (chain.TaxUnpaidTurns >= Constants.CHAIN_TAX_UNPAID_THRESHOLD)
            {
                EventBus.DeterministicEventTriggered("TaxAudit");
                Debug.Log("[EventPhase] Deterministic: TaxAudit triggered (unpaid tax chain)");
            }

            // Uninsured staff 3+ turns -> SGKAudit
            if (chain.UninsuredStaffTurns >= Constants.CHAIN_UNINSURED_STAFF_THRESHOLD)
            {
                EventBus.DeterministicEventTriggered("SGKAudit");
                Debug.Log("[EventPhase] Deterministic: SGKAudit triggered (uninsured staff chain)");
            }
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _turnManager.EventPhaseMinDuration)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit() { }
    }
}
