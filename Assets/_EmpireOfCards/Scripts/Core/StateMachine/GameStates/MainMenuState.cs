using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.GameStates
{
    /// <summary>
    /// State active while the player is on the main menu screen.
    /// Shows main menu UI and plays calm background music.
    /// Tick polls for nothing — UI buttons drive transitions via GameManager.
    /// </summary>
    public class MainMenuState : IState
    {
        public void Enter()
        {
            var gm = GameManager.Instance;
            gm.SetGameState(GameState.MainMenu);

            // Show main menu UI, play calm music
            if (gm.AudioManager != null)
                gm.AudioManager.PlayMusic(false); // calm
        }

        public void Tick()
        {
            // Polling: waiting for UI button press to start game.
            // UIManager handles button clicks and calls GameManager.StartNewRun().
        }

        public void Exit()
        {
            // Hide main menu — UIManager handles canvas toggling
        }
    }
}
