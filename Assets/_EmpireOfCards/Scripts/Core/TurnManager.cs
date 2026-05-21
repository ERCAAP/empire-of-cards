using System;
using System.Collections.Generic;
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

        [Header("=== Phase State ===")]
        [SerializeField] private TurnPhase currentPhase;
        [SerializeField] private int currentTurn;
        [SerializeField] private bool isTurnActive;

        [Header("=== Phase Timing ===")]
        [SerializeField] private float drawPhaseMinDuration = 1f;
        [SerializeField] private float planningPhaseMinDuration = 1.5f;
        [SerializeField] private float resolveStepDelay = 0.5f;
        [SerializeField] private float crisisPhaseMinDuration = 2f;
        [SerializeField] private float crisisPhaseNoCrisisDuration = 0.5f;
        [SerializeField] private float rivalPhaseMinDuration = 2f;
        [SerializeField] private float marketUpdateMinDuration = 1f;

        // Play phase control
        private bool _playerEndedPlayPhase;

        // Active event tracking (GDD Section 4.1)
        private CardData _activeEvent;
        private int _activeEventTurnsRemaining;

        // Event deck (populated by WiringService from CardDataFactory event cards)
        private List<CardData> _eventDeck = new List<CardData>();

        // === Properties ===
        public TurnPhase CurrentPhase => currentPhase;
        public bool IsTurnActive => isTurnActive;
        public CardData ActiveEvent => _activeEvent;
        public int ActiveEventTurnsRemaining => _activeEventTurnsRemaining;

        // Properties exposed for external phase handlers
        public IReadOnlyList<CardData> EventDeck => _eventDeck;
        public int CurrentTurnNumber => currentTurn;
        public float DrawPhaseMinDuration => drawPhaseMinDuration;
        public float PlanningPhaseMinDuration => planningPhaseMinDuration;
        public float ResolveStepDelay => resolveStepDelay;
        public float CrisisPhaseMinDuration => crisisPhaseMinDuration;
        public float CrisisPhaseNoCrisisDuration => crisisPhaseNoCrisisDuration;
        public float RivalPhaseMinDuration => rivalPhaseMinDuration;
        public float MarketUpdateMinDuration => marketUpdateMinDuration;
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
        /// then enters the DrawPhase (GDD v4 Section 9.1).
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

            // GDD v4: turn starts with DrawPhase
            TransitionToPhase(TurnPhase.DrawPhase);
        }

        public void ResumePlayPhase(int turnNumber)
        {
            currentTurn = turnNumber;
            isTurnActive = true;
            _playerEndedPlayPhase = false;
            currentPhase = TurnPhase.PlayPhase;
            EventBus.PhaseStarted(TurnPhase.PlayPhase);
            _phaseStateMachine.Initialize(new TurnPhases.PlayPhase(this));
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
            {
                GameManager.Instance?.BoardManager?.SetActiveEvent(eventCard);
                GameManager.Instance?.ActiveVentureRuntime?.RegisterEventFired(eventCard, currentTurn);
                EventBus.EventActivated(eventCard);
            }
        }

        /// <summary>
        /// Populates the event deck used by CrisisReactionPhase to draw random events.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void SetEventDeck(List<CardData> events)
        {
            _eventDeck = events ?? new List<CardData>();
            Debug.Log($"[TurnManager] Event deck set with {_eventDeck.Count} event cards.");
        }

        /// <summary>
        /// Picks a random event card from the event deck.
        /// Returns null if the deck is empty.
        /// </summary>
        public CardData DrawRandomEvent()
        {
            if (_eventDeck == null || _eventDeck.Count == 0)
                return null;

            var gm = GameManager.Instance;
            VentureType activeVenture = gm != null && gm.ActiveEconomyProfile != null
                ? gm.ActiveEconomyProfile.ventureType
                : VentureType.FastFood;
            TechCategoryProfile techCategory = gm != null ? gm.ActiveTechCategoryProfile : null;

            List<CardData> filtered = new List<CardData>();
            foreach (var card in _eventDeck)
            {
                if (card == null) continue;
                if (card.isGeneralCard || card.ventureType == activeVenture)
                    filtered.Add(card);
            }

            if (activeVenture == VentureType.TechApp && techCategory != null && techCategory.crisisCardIds != null && techCategory.crisisCardIds.Length > 0)
            {
                filtered.RemoveAll(card => card != null && card.cardType == CardType.Event && card.cardFamily == CardFamily.Crisis && card.cardId != "TC09" && System.Array.IndexOf(techCategory.crisisCardIds, card.cardId) < 0);
            }

            if (filtered.Count == 0)
                filtered = _eventDeck;

            int index = UnityEngine.Random.Range(0, filtered.Count);
            return filtered[index];
        }

        // === Phase Transitions (State Machine) ===

        private void TransitionToPhase(TurnPhase phase)
        {
            currentPhase = phase;
            EventBus.PhaseStarted(phase);

            IState newState = phase switch
            {
                TurnPhase.DrawPhase => new TurnPhases.DrawPhase(this),
                TurnPhase.PlanningPhase => new TurnPhases.PlanningPhase(this),
                TurnPhase.PlayPhase => new TurnPhases.PlayPhase(this),
                TurnPhase.ResolvePhase => new TurnPhases.ResolvePhase(this),
                TurnPhase.CrisisReactionPhase => new TurnPhases.CrisisReactionPhase(this),
                TurnPhase.RivalPhase => new TurnPhases.RivalPhase(this),
                TurnPhase.MarketUpdatePhase => new TurnPhases.MarketUpdatePhase(this),
                _ => null
            };

            if (newState == null) return;

            // Use ChangeState for subsequent phases so Exit() is called on the previous phase.
            // Use Initialize only for the very first phase of the turn.
            if (_phaseStateMachine.CurrentState == null)
                _phaseStateMachine.Initialize(newState);
            else
                _phaseStateMachine.ChangeState(newState);
        }

        /// <summary>
        /// Called by phase handlers to signal the current phase is done.
        /// Transitions to the next phase in sequence.
        /// </summary>
        public void CompleteCurrentPhase()
        {
            EventBus.PhaseEnded(currentPhase);

            // GDD v4 Section 9.1 phase order:
            // Draw -> Planning -> Play -> Resolve -> CrisisReaction -> Rival -> MarketUpdate
            switch (currentPhase)
            {
                case TurnPhase.DrawPhase:
                    TransitionToPhase(TurnPhase.PlanningPhase);
                    break;
                case TurnPhase.PlanningPhase:
                    TransitionToPhase(TurnPhase.PlayPhase);
                    break;
                case TurnPhase.PlayPhase:
                    TransitionToPhase(TurnPhase.ResolvePhase);
                    break;
                case TurnPhase.ResolvePhase:
                    TransitionToPhase(TurnPhase.CrisisReactionPhase);
                    break;
                case TurnPhase.CrisisReactionPhase:
                    TransitionToPhase(TurnPhase.RivalPhase);
                    break;
                case TurnPhase.RivalPhase:
                    TransitionToPhase(TurnPhase.MarketUpdatePhase);
                    break;
                case TurnPhase.MarketUpdatePhase:
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
