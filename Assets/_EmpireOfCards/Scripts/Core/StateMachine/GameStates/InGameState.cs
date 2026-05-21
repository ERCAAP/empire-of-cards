using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.GameStates
{
    /// <summary>
    /// The main gameplay state. TurnManager handles turn/phase flow internally.
    /// Win/lose checks are handled exclusively by GameManager.EndCurrentTurn()
    /// at the end of each turn to avoid double-triggering GameOverState.
    /// </summary>
    public class InGameState : IState
    {
        public void Enter()
        {
            var gm = GameManager.Instance;
            gm.SetGameState(GameState.Playing);
        }

        public void Tick()
        {
            // State machine and TurnManager drive game flow.
            // Win/lose evaluation happens only in GameManager.EndCurrentTurn().
        }

        public void Exit()
        {
            // TurnManager stops itself when GameManager.EndRun() is called
        }
    }
}
