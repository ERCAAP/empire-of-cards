using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.GameStates
{
    /// <summary>
    /// State active when the game ends (win or lose).
    /// Displays the score screen and waits for the player to choose next action.
    /// </summary>
    public class GameOverState : IState
    {
        public void Enter()
        {
            Debug.Log("[GameOverState] Enter - Showing game over screen");

            // Show game over / score screen UI
            // TODO: UIManager.Instance.ShowGameOverScreen();

            // Display final stats: money, territories, turns survived, combos triggered
            GameManager gm = GameManager.Instance;
            if (gm != null)
            {
                Debug.Log($"[GameOverState] Final Score - Money: {gm.PlayerMoney}, " +
                          $"Territories: {gm.PlayerTerritories}/{gm.RivalTerritories}, " +
                          $"Turn: {gm.CurrentTurn}/{gm.MaxTurns}");
            }
        }

        public void Execute()
        {
            // Wait for player to choose:
            // - Play Again -> transition to GameSetupState
            // - Main Menu  -> transition to MainMenuState
            // UI buttons handle the transitions
        }

        public void Exit()
        {
            Debug.Log("[GameOverState] Exit - Cleaning up");

            // Clear event bus subscriptions to prevent stale references
            EventBus.ClearAll();

            // Hide game over UI
            // TODO: UIManager.Instance.HideGameOverScreen();
        }
    }
}
