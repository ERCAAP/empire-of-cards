using UnityEngine;
using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 1: Check if a world event triggers this turn (GDD Section 4.1).
    /// Events fire on turns: 3, 6, 9, 12, 15, 18.
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
            int interval = gm.BalanceData != null
                ? gm.BalanceData.eventInterval
                : Constants.EVENT_INTERVAL;

            // Events fire every EVENT_INTERVAL turns (3, 6, 9, 12, 15, 18)
            bool shouldFireEvent = _turnManager.CurrentTurnNumber > 0
                && _turnManager.CurrentTurnNumber % interval == 0;

            if (shouldFireEvent && _turnManager.ActiveEvent == null)
            {
                var eventCard = _turnManager.DrawRandomEvent();
                if (eventCard != null)
                {
                    _turnManager.SetActiveEvent(eventCard);
                    Debug.Log($"[EventPhase] Event activated: {eventCard.cardName} (duration: {eventCard.eventDuration} turns)");
                }
                else
                {
                    Debug.LogWarning("[EventPhase] Event should fire but event deck is empty.");
                }
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
