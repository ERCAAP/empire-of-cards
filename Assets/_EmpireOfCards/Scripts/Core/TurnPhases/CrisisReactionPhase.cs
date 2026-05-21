using UnityEngine;
using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 5 (GDD v4 Section 9.1): Crises fire AFTER resolve, not before.
    /// Replaces the old EventPhase. Checks chain reaction state and board pressure
    /// for crisis triggers. Places crisis card in TempEffect slot if triggered.
    /// </summary>
    public class CrisisReactionPhase : IState
    {
        private readonly TurnManager _turnManager;
        private float _timer;
        private bool _crisisFired;

        public CrisisReactionPhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _timer = 0f;
            _crisisFired = false;

            var gm = GameManager.Instance;
            if (gm == null) return;

            // --- Deterministic crises from chain reaction state (GDD 12.2) ---
            CheckDeterministicCrises(gm);

            // --- Pressure-based crisis ---
            bool pressureTriggered = ShouldTriggerPressureCrisis(gm);

            // --- Interval-based random events ---
            int interval = gm.BalanceData != null
                ? gm.BalanceData.eventInterval
                : Constants.EVENT_INTERVAL;
            bool shouldFireEvent = _turnManager.CurrentTurnNumber > 0
                && _turnManager.CurrentTurnNumber % interval == 0;

            // --- Scripted venture events ---
            if (_turnManager.ActiveEvent == null && gm.ActiveVentureRuntime != null)
            {
                CardData scriptedEvent = gm.ActiveVentureRuntime.ResolveScriptedEvent(
                    _turnManager.CurrentTurnNumber,
                    gm.EconomyManager != null ? gm.EconomyManager.CurrentPressure : BoardPressureType.None);

                if (scriptedEvent != null)
                {
                    _turnManager.SetActiveEvent(scriptedEvent);
                    _crisisFired = true;
                    Debug.Log($"[CrisisReactionPhase] Scripted venture event: {scriptedEvent.cardName}");
                    return;
                }
            }

            // --- Random event draw ---
            if ((shouldFireEvent || pressureTriggered) && _turnManager.ActiveEvent == null)
            {
                var eventCard = _turnManager.DrawRandomEvent();
                if (eventCard != null)
                {
                    _turnManager.SetActiveEvent(eventCard);
                    _crisisFired = true;
                    Debug.Log($"[CrisisReactionPhase] Event activated: {eventCard.cardName} (duration: {eventCard.eventDuration} turns)");
                }
            }
        }

        private bool ShouldTriggerPressureCrisis(GameManager gm)
        {
            if (gm.EconomyManager == null || gm.EconomyManager.Snapshot == null)
                return false;

            var snapshot = gm.EconomyManager.Snapshot;
            return gm.EconomyManager.CurrentPressure == BoardPressureType.HighLegalRisk
                || gm.EconomyManager.CurrentPressure == BoardPressureType.LowRating
                || gm.EconomyManager.CurrentPressure == BoardPressureType.CapacityShortfall
                || gm.EconomyManager.CurrentPressure == BoardPressureType.StaffInstability
                || snapshot.legalRisk >= 32f
                || snapshot.rating <= 2.8f;
        }

        private void CheckDeterministicCrises(GameManager gm)
        {
            var chain = gm.ChainReactionSystem;
            if (chain == null) return;

            if (chain.CheapSupplierTurns >= Constants.CHAIN_CHEAP_SUPPLIER_THRESHOLD)
            {
                EventBus.DeterministicEventTriggered("QualityCrisis");
                _crisisFired = true;
                Debug.Log("[CrisisReactionPhase] Deterministic: QualityCrisis (cheap supplier chain)");
            }

            if (chain.SalaryDelayedTurns >= Constants.CHAIN_SALARY_DELAY_THRESHOLD)
            {
                EventBus.DeterministicEventTriggered("StaffStrike");
                _crisisFired = true;
                Debug.Log("[CrisisReactionPhase] Deterministic: StaffStrike (salary delay chain)");
            }

            if (chain.IsPlatformRatingCritical())
            {
                EventBus.DeterministicEventTriggered("ReputationCrisis");
                _crisisFired = true;
                Debug.Log("[CrisisReactionPhase] Deterministic: ReputationCrisis (low platform rating)");
            }

            if (chain.TaxUnpaidTurns >= Constants.CHAIN_TAX_UNPAID_THRESHOLD)
            {
                EventBus.DeterministicEventTriggered("TaxAudit");
                _crisisFired = true;
                Debug.Log("[CrisisReactionPhase] Deterministic: TaxAudit (unpaid tax chain)");
            }

            if (chain.UninsuredStaffTurns >= Constants.CHAIN_UNINSURED_STAFF_THRESHOLD)
            {
                EventBus.DeterministicEventTriggered("SGKAudit");
                _crisisFired = true;
                Debug.Log("[CrisisReactionPhase] Deterministic: SGKAudit (uninsured staff chain)");
            }
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            float requiredDuration = _crisisFired
                ? _turnManager.CrisisPhaseMinDuration
                : _turnManager.CrisisPhaseNoCrisisDuration;

            if (_timer >= requiredDuration)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit() { }
    }
}
