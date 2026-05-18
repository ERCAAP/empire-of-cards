using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.TurnPhases
{
    /// <summary>
    /// Turn phase 3: Player plays cards with 3 actions (GDD Section 4.1, Step 3).
    /// Polls for player input -- waits until player ends turn or runs out of actions.
    /// </summary>
    public class PlayPhase : IState
    {
        private readonly TurnManager _turnManager;

        public PlayPhase(TurnManager tm) { _turnManager = tm; }

        public void Enter()
        {
            _turnManager.ResetPlayerEndedPlayPhase();
            // UI enables card dragging, action dots, end turn button
        }

        public void Tick()
        {
            var gm = GameManager.Instance;

            // POLLING: check if player ended turn or ran out of actions
            if (_turnManager.PlayerEndedPlayPhase || gm.PlayerActions <= 0)
            {
                _turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit()
        {
            // UI disables card dragging
        }
    }
}
