using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core.StateMachine;
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

        // Phase timer for minimum durations
        private float _phaseTimer;
        private bool _phaseMinDurationReached;

        // Active event tracking (GDD Section 4.1)
        private CardData _activeEvent;
        private int _activeEventTurnsRemaining;

        // Properties
        public TurnPhase CurrentPhase => currentPhase;
        public bool IsTurnActive => isTurnActive;
        public CardData ActiveEvent => _activeEvent;
        public int ActiveEventTurnsRemaining => _activeEventTurnsRemaining;

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
            _phaseTimer = 0f;
            _phaseMinDurationReached = false;
            EventBus.PhaseStarted(phase);

            switch (phase)
            {
                case TurnPhase.EventPhase:
                    _phaseStateMachine.Initialize(new EventPhaseHandler(this));
                    break;
                case TurnPhase.DrawPhase:
                    _phaseStateMachine.Initialize(new DrawPhaseHandler(this));
                    break;
                case TurnPhase.PlayPhase:
                    _phaseStateMachine.Initialize(new PlayPhaseHandler(this));
                    break;
                case TurnPhase.ResolvePhase:
                    _phaseStateMachine.Initialize(new ResolvePhaseHandler(this));
                    break;
                case TurnPhase.RivalPhase:
                    _phaseStateMachine.Initialize(new RivalPhaseHandler(this));
                    break;
            }
        }

        private void CompleteCurrentPhase()
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

        // ================================================================
        // PHASE HANDLER CLASSES (Inner classes for state machine polling)
        // ================================================================

        // --- EVENT PHASE: Check if event triggers this turn (GDD Step 1) ---
        // Events fire on turns: 3, 6, 9, 12, 15, 18
        private class EventPhaseHandler : IState
        {
            private readonly TurnManager _tm;
            private float _timer;

            public EventPhaseHandler(TurnManager tm) { _tm = tm; }

            public void Enter()
            {
                _timer = 0f;

                var gm = GameManager.Instance;
                int interval = gm.BalanceData != null
                    ? gm.BalanceData.eventInterval
                    : Constants.EVENT_INTERVAL;

                // Events fire every EVENT_INTERVAL turns (3, 6, 9, 12, 15, 18)
                bool shouldFireEvent = _tm.currentTurn > 0
                    && _tm.currentTurn % interval == 0;

                if (shouldFireEvent && _tm._activeEvent == null)
                {
                    // TODO: Draw random event from event deck via DeckManager or EventDeck
                    // When implemented: draw card, call _tm.SetActiveEvent(card),
                    // and show popup via UIManager.ShowEventPopup(card)
                }
            }

            public void Tick()
            {
                _timer += Time.deltaTime;
                if (_timer >= _tm.eventPhaseMinDuration)
                {
                    _tm.CompleteCurrentPhase();
                }
            }

            public void Exit() { }
        }

        // --- DRAW PHASE: Draw 5 cards, allow 1 redraw (GDD Step 2) ---
        private class DrawPhaseHandler : IState
        {
            private readonly TurnManager _tm;
            private float _timer;
            private bool _cardsDrawn;

            public DrawPhaseHandler(TurnManager tm) { _tm = tm; }

            public void Enter()
            {
                _timer = 0f;
                _cardsDrawn = false;

                var dm = GameManager.Instance.DeckManager;
                if (dm != null)
                {
                    dm.DiscardHand();
                    dm.ResetRedraws();
                    dm.DrawCards(Constants.HAND_SIZE);
                    _cardsDrawn = true;
                }
            }

            public void Tick()
            {
                _timer += Time.deltaTime;
                // Wait for draw animation minimum duration
                if (_timer >= _tm.drawPhaseMinDuration && _cardsDrawn)
                {
                    _tm.CompleteCurrentPhase();
                }
            }

            public void Exit() { }
        }

        // --- PLAY PHASE: Player plays cards, 3 actions (GDD Step 3) ---
        // POLLS for player input - waits until player ends turn or runs out of actions
        private class PlayPhaseHandler : IState
        {
            private readonly TurnManager _tm;

            public PlayPhaseHandler(TurnManager tm) { _tm = tm; }

            public void Enter()
            {
                _tm._playerEndedPlayPhase = false;
                // UI enables card dragging, action dots, end turn button
            }

            public void Tick()
            {
                var gm = GameManager.Instance;

                // POLLING: check if player ended turn or ran out of actions
                if (_tm._playerEndedPlayPhase || gm.PlayerActions <= 0)
                {
                    _tm.CompleteCurrentPhase();
                }
            }

            public void Exit()
            {
                // UI disables card dragging
            }
        }

        // --- RESOLVE PHASE: System calculates everything step by step (GDD Step 4) ---
        // Sub-steps: 4a BusinessProduce -> 4b CustomerFlow -> 4c ComboCheck
        //         -> 4d IncomeCalculation -> 4e DeteriorationCheck
        private class ResolvePhaseHandler : IState
        {
            private readonly TurnManager _tm;
            private readonly ResolveStep[] _steps;
            private int _stepIndex;
            private float _stepTimer;
            private bool _stepExecuted;

            public ResolvePhaseHandler(TurnManager tm)
            {
                _tm = tm;
                _steps = (ResolveStep[])Enum.GetValues(typeof(ResolveStep));
            }

            public void Enter()
            {
                _stepIndex = 0;
                _stepTimer = 0f;
                _stepExecuted = false;
            }

            public void Tick()
            {
                if (_stepIndex >= _steps.Length)
                {
                    _tm.CompleteCurrentPhase();
                    return;
                }

                if (!_stepExecuted)
                {
                    ExecuteStep(_steps[_stepIndex]);
                    _stepExecuted = true;
                    _stepTimer = 0f;
                }

                _stepTimer += Time.deltaTime;
                if (_stepTimer >= _tm.resolveStepDelay)
                {
                    _stepIndex++;
                    _stepExecuted = false;
                }
            }

            private void ExecuteStep(ResolveStep step)
            {
                var gm = GameManager.Instance;

                switch (step)
                {
                    // 4a: Businesses produce products/customers
                    case ResolveStep.BusinessProduce:
                        if (gm.BoardManager != null)
                            gm.BoardManager.TickClosedBusinesses();
                        break;

                    // 4b: Calculate total customers and territory distribution
                    case ResolveStep.CustomerFlow:
                        if (gm.TerritoryManager != null)
                        {
                            int marketPool = gm.BalanceData != null
                                ? gm.BalanceData.GetMarketPool(gm.CurrentTurn)
                                : Constants.BASE_MARKET_CUSTOMERS;
                            gm.TerritoryManager.CalculateTerritories(
                                gm.PlayerCustomers, gm.RivalCustomers, marketPool);
                        }
                        break;

                    // 4c: Check and trigger combos
                    case ResolveStep.ComboCheck:
                        if (gm.ComboSystem != null && gm.BoardManager != null)
                        {
                            int activeBizCount = gm.BoardManager.GetActiveBusinessCount();
                            int totalMarket = gm.BalanceData != null
                                ? gm.BalanceData.GetMarketPool(gm.CurrentTurn)
                                : Constants.BASE_MARKET_CUSTOMERS;
                            float marketShare = totalMarket > 0
                                ? ((float)gm.PlayerCustomers / totalMarket) * 100f
                                : 0f;

                            gm.ComboSystem.CheckCombos(
                                gm.PlayerMoney,
                                gm.PlayerTerritories,
                                activeBizCount,
                                marketShare);
                        }
                        break;

                    // 4d: Calculate and apply income, salaries, tax
                    case ResolveStep.IncomeCalculation:
                        if (gm.EconomyManager != null)
                        {
                            gm.EconomyManager.ProcessEndOfTurn();
                        }
                        break;

                    // 4e: FBI check, business closure countdown, employee leaving
                    case ResolveStep.DeteriorationCheck:
                        if (gm.FBISystem != null)
                            gm.FBISystem.CheckForRaid();
                        if (gm.BoardManager != null)
                            gm.BoardManager.TickClosedBusinesses();
                        break;
                }
            }

            public void Exit() { }
        }

        // --- RIVAL PHASE: AI opponent takes 2 actions (GDD Step 5) ---
        private class RivalPhaseHandler : IState
        {
            private readonly TurnManager _tm;
            private float _timer;
            private bool _rivalActed;

            public RivalPhaseHandler(TurnManager tm) { _tm = tm; }

            public void Enter()
            {
                _timer = 0f;
                _rivalActed = false;

                var rival = GameManager.Instance.RivalAI;
                if (rival != null)
                {
                    rival.TakeTurn(GameManager.Instance.PlayerTerritories, _tm.currentTurn);
                    _rivalActed = true;
                }
            }

            public void Tick()
            {
                _timer += Time.deltaTime;
                if (_timer >= _tm.rivalPhaseMinDuration && _rivalActed)
                {
                    _tm.CompleteCurrentPhase();
                }
            }

            public void Exit() { }
        }
    }
}
