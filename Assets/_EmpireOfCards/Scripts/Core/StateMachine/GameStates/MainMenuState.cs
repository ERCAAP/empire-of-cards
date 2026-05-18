using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.GameStates
{
    /// <summary>
    /// State active while the player is on the main menu screen.
    /// Shows main menu UI and plays calm background music.
    /// </summary>
    public class MainMenuState : IState
    {
        public void Enter()
        {
            Debug.Log("[MainMenuState] Enter - Showing main menu UI");

            // Show the main menu canvas
            // TODO: UIManager.Instance.ShowMainMenu();

            // Play calm main menu music
            // TODO: AudioManager.Instance.PlayMusic("MainMenuTheme");
        }

        public void Execute()
        {
            // Wait for player input - UI buttons handle transitions
            // e.g. "New Game" button calls GameManager to change to GameSetupState
        }

        public void Exit()
        {
            Debug.Log("[MainMenuState] Exit - Hiding main menu");

            // Hide the main menu canvas
            // TODO: UIManager.Instance.HideMainMenu();
        }
    }
}
