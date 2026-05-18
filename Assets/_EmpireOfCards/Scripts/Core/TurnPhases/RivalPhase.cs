using UnityEngine;
using EmpireOfCards.Core.StateMachine;

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

        public RivalPhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _timer = 0f;

            var rival = GameManager.Instance.RivalAI;
            if (rival != null)
            {
                rival.TakeTurn(GameManager.Instance.PlayerTerritories, _turnManager.CurrentTurnNumber);
            }
            // Always mark as acted so the phase can complete even if RivalAI is null
            _rivalActed = true;
        }

        public void Tick()
        {
            _timer += Time.deltaTime;
            if (_timer >= _turnManager.RivalPhaseMinDuration && _rivalActed)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit() { }
    }
}
