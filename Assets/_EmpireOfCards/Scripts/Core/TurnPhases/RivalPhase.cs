using UnityEngine;
using EmpireOfCards.Core.StateMachine;
using EmpireOfCards.Data;
using System.Collections.Generic;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 5: AI opponent takes 2 actions (GDD Section 4.1, Step 5).
    /// </summary>
    public class RivalPhase : IState
    {
        private readonly TurnManager _turnManager;
        private float _timer;
        private bool _rivalActed;
        private int _actionIndex;
        private float _actionDelay;
        private List<RivalQueuedAction> _queuedActions = new List<RivalQueuedAction>();

        public RivalPhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _timer = 0f;
            _actionIndex = 0;
            _actionDelay = 0f;

            var gm = GameManager.Instance;
            var rival = gm != null ? gm.RivalAI : null;
            if (rival != null)
            {
                float playerShare = gm.EconomyManager != null && gm.EconomyManager.Snapshot != null
                    ? gm.EconomyManager.Snapshot.marketShare
                    : gm.PlayerCustomers;
                _queuedActions = rival.BuildQueuedActions(playerShare, _turnManager.CurrentTurnNumber);
            }
            _rivalActed = rival == null || _queuedActions.Count == 0;
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (!_rivalActed)
            {
                TickQueuedActions();
                return;
            }

            if (_timer >= _turnManager.RivalPhaseMinDuration && _rivalActed)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        private void TickQueuedActions()
        {
            var gm = GameManager.Instance;
            var rival = gm != null ? gm.RivalAI : null;
            if (gm == null || rival == null)
            {
                _rivalActed = true;
                return;
            }

            float playerShare = gm.EconomyManager != null && gm.EconomyManager.Snapshot != null
                ? gm.EconomyManager.Snapshot.marketShare
                : gm.PlayerCustomers;

            if (_actionIndex >= _queuedActions.Count)
            {
                rival.FinalizeQueuedTurn(playerShare);
                _rivalActed = true;
                _timer = 0f;
                return;
            }

            if (_actionDelay > 0f)
            {
                _actionDelay -= Time.deltaTime;
                return;
            }

            var action = _queuedActions[_actionIndex];
            EventBus.RivalMoodChanged(action.moodIcon);
            EventBus.RivalActionQueued(action);
            EventBus.RivalPressureVisualChanged(new RivalPressureViewModel
            {
                headline = action.displayName,
                laneLabel = action.laneLabel,
                shortDescription = action.shortDescription,
                moodIcon = action.moodIcon,
                pressureDelta = action.pressureDelta,
                demandSteal = action.demandSteal,
                ratingDelta = action.ratingDelta
            });
            EventBus.RivalActed($"{action.displayName} -> {action.laneLabel}");
            rival.ResolveQueuedAction(action, playerShare);
            _actionDelay = action.previewDelay + action.resolveDelay;
            _actionIndex++;
        }

        public void Exit() { }
    }
}
