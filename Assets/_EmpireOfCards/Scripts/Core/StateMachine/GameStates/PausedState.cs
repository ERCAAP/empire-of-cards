using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.GameStates
{
    /// <summary>
    /// State active while the game is paused.
    /// Freezes time and shows the pause menu overlay.
    /// </summary>
    public class PausedState : IState
    {
        public void Enter()
        {
            Debug.Log("[PausedState] Enter - Game paused");

            // Freeze game time
            Time.timeScale = 0f;

            // Show pause menu UI
            // TODO: UIManager.Instance.ShowPauseMenu();
        }

        public void Execute()
        {
            // Wait for the player to press Resume, Settings, or Quit
            // UI buttons handle transitions back to InGameState or MainMenuState
        }

        public void Exit()
        {
            Debug.Log("[PausedState] Exit - Game resumed");

            // Hide pause menu UI
            // TODO: UIManager.Instance.HidePauseMenu();

            // Restore game time
            Time.timeScale = 1f;
        }
    }
}
