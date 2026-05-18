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

            // Show pause menu overlay
            if (gm.UIManager != null)
                gm.UIManager.ShowPauseMenu();
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

            // Hide pause menu overlay
            var gm = GameManager.Instance;
            if (gm.UIManager != null)
                gm.UIManager.HidePauseMenu();
        }
    }
}
