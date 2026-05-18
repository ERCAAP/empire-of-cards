using System;
using UnityEngine;
using EmpireOfCards.Core.StateMachine;
using EmpireOfCards.Core.TurnPhases;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Core
{
    public class TurnManager : MonoBehaviour
    {
        // Phase state machine - polled every frame
        private StateMachine.StateMachine _phaseStateMachine;

        [Header("=== Faz Durumu ===")]
        [SerializeField] private TurnPhase currentPhase;
        [SerializeField] private int currentTurn;
        [SerializeField] private bool isTurnActive;

        [Header("=== Faz Zamanlama ===")]
        [SerializeField] private float eventPhaseMinDuration = 2f;
        [SerializeField] private float drawPhaseMinDuration = 1f;
        [SerializeField] private float resolveStepDelay = 0.5f;
        [SerializeField] private float rivalPhaseMinDuration = 2f;

        // Play phase control
        private bool _playerEndedPlayPhase;

        // Active event tracking (GDD Section 4.1)
        private CardData _activeEvent;
        private int _activeEventTurnsRemaining;

        // === Properties ===
        public TurnPhase CurrentPhase => currentPhase;
        public bool IsTurnActive => isTurnActive;
        public CardData ActiveEvent => _activeEvent;
        public int ActiveEventTurnsRemaining => _activeEventTurnsRemaining;

        // Properties exposed for external phase handlers
        public int CurrentTurnNumber => currentTurn;
        public float EventPhaseMinDuration => eventPhaseMinDuration;
        public float DrawPhaseMinDuration => drawPhaseMinDuration;
        public float ResolveStepDelay => resolveStepDelay;
        public float RivalPhaseMinDuration => rivalPhaseMinDuration;
        public bool PlayerEndedPlayPhase => _playerEndedPlayPhase;

        private void Awake()
        {
            _phaseStateMachine = new StateMachine.StateMachine();
        }

        // POLLING: every frame, tick the phase state machine
        private void Update()
        {
            if (!isTurnActive) return;
            _phaseStateMachine?.Tick();
        }

        // === Turn Control ===

        /// <summary>
        /// Starts a new turn. Resets actions, ticks active event duration,
        /// then enters the EventPhase.
        /// </summary>
        public void BeginTurn(int turnNumber)
        {
            currentTurn = turnNumber;
            isTurnActive = true;
            _playerEndedPlayPhase = false;

            GameManager.Instance.ResetActions();

            // Tick active event duration down
            if (_activeEvent != null)
            {
                _activeEventTurnsRemaining--;
                if (_activeEventTurnsRemaining <= 0)
                {
                    EventBus.EventExpired(_activeEvent);
                    _activeEvent = null;
                }
            }

            // Start with event phase (GDD turn flow step 1)
            TransitionToPhase(TurnPhase.EventPhase);
        }

        /// <summary>
        /// Called by the End Turn button during the Play Phase.
        /// </summary>
        public void EndPlayPhase()
        {
            _playerEndedPlayPhase = true;
        }

        /// <summary>
        /// Resets the flag that tracks whether the player ended the play phase.
        /// Called by PlayPhase.Enter().
        /// </summary>
        public void ResetPlayerEndedPlayPhase()
        {
            _playerEndedPlayPhase = false;
        }

        /// <summary>
        /// Sets the active world event and its remaining duration.
        /// </summary>
        public void SetActiveEvent(CardData eventCard)
        {
            _activeEvent = eventCard;
            _activeEventTurnsRemaining = eventCard != null ? eventCard.eventDuration : 0;
            if (eventCard != null)
                EventBus.EventActivated(eventCard);
        }

        // === Phase Transitions (State Machine) ===

        private void TransitionToPhase(TurnPhase phase)
        {
            currentPhase = phase;
            EventBus.PhaseStarted(phase);

            switch (phase)
            {
                case TurnPhase.EventPhase:
                    _phaseStateMachine.Initialize(new TurnPhases.EventPhase(this));
                    break;
                case TurnPhase.DrawPhase:
                    _phaseStateMachine.Initialize(new TurnPhases.DrawPhase(this));
                    break;
                case TurnPhase.PlayPhase:
                    _phaseStateMachine.Initialize(new TurnPhases.PlayPhase(this));
                    break;
                case TurnPhase.ResolvePhase:
                    _phaseStateMachine.Initialize(new TurnPhases.ResolvePhase(this));
                    break;
                case TurnPhase.RivalPhase:
                    _phaseStateMachine.Initialize(new TurnPhases.RivalPhase(this));
                    break;
            }
        }

        /// <summary>
        /// Called by phase handlers to signal the current phase is done.
        /// Transitions to the next phase in sequence.
        /// </summary>
        public void CompleteCurrentPhase()
        {
            EventBus.PhaseEnded(currentPhase);

            switch (currentPhase)
            {
                case TurnPhase.EventPhase:
                    TransitionToPhase(TurnPhase.DrawPhase);
                    break;
                case TurnPhase.DrawPhase:
                    TransitionToPhase(TurnPhase.PlayPhase);
                    break;
                case TurnPhase.PlayPhase:
                    TransitionToPhase(TurnPhase.ResolvePhase);
                    break;
                case TurnPhase.ResolvePhase:
                    TransitionToPhase(TurnPhase.RivalPhase);
                    break;
                case TurnPhase.RivalPhase:
                    FinishTurn();
                    break;
            }
        }

        private void FinishTurn()
        {
            isTurnActive = false;
            GameManager.Instance.EndCurrentTurn();
        }
    }
}
