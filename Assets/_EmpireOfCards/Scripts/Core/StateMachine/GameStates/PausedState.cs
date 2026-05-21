using UnityEngine;
using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.GameStates
{
    /// <summary>
    /// State active while the game is paused.
    /// Enter freezes time and shows pause UI.
    /// Tick waits for resume input (driven by UIManager button callbacks).
    /// Exit restores time and hides pause UI.
    /// </summary>
    public class PausedState : IState
    {
        public void Enter()
        {
            Time.timeScale = 0f;

            var gm = GameManager.Instance;
            gm.SetGameState(GameState.Paused);

            // Pause UI will be handled when pause menu panel is implemented
        }

        public void Tick()
        {
            // Polling: waiting for Resume / Quit button press.
            // UIManager handles button clicks and calls
            // GameManager.GetGameStateMachine().ChangeState(...) to resume.
        }

        public void Exit()
        {
            Time.timeScale = 1f;

            // Pause UI will be handled when pause menu panel is implemented
        }
    }
}
