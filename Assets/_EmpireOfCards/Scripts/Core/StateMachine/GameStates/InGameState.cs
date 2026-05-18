using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.GameStates
{
    /// <summary>
    /// State active during the main gameplay loop.
    /// Delegates turn flow to the TurnManager.
    /// </summary>
    public class InGameState : IState
    {
        public void Enter()
        {
            Debug.Log("[InGameState] Enter - Starting gameplay");

            // Start the first turn via TurnManager
            GameManager gm = GameManager.Instance;
            if (gm != null && gm.TurnManager != null)
            {
                gm.TurnManager.StartTurn();
            }
        }

        public void Execute()
        {
            // Main game loop - the TurnManager drives the turn/phase sequence
            // via its own coroutine. We delegate to it here.

            GameManager gm = GameManager.Instance;
            if (gm == null)
                return;

            // Check for win/lose conditions each frame
            if (gm.CheckWinCondition() || gm.CheckLoseCondition())
            {
                return;
            }
        }

        public void Exit()
        {
            // Nothing to clean up; GameOverState or PausedState handles next steps
        }
    }
}
