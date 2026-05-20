using UnityEngine;
using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.GameStates
{
    /// <summary>
    /// State active when the game ends (win or lose).
    /// Enter shows the score / game-over screen and calculates the final score.
    /// Tick waits for Play Again or Main Menu button press.
    /// Exit clears EventBus and performs cleanup.
    /// </summary>
    public class GameOverState : IState
    {
        public void Enter()
        {
            var gm = GameManager.Instance;
            gm.SetGameState(GameState.GameOver);

            // Calculate and log final score
            int finalMoney = gm.PlayerMoney;
            int playerBlocks = gm.PlayerMarketBlocks;
            int rivalBlocks = gm.RivalMarketBlocks;
            int turn = gm.CurrentTurn;
            int maxTurn = gm.MaxTurns;

            Debug.Log($"[GameOverState] Final — Money: {finalMoney}, " +
                      $"Market Blocks: {playerBlocks}/{rivalBlocks}, " +
                      $"Turn: {turn}/{maxTurn}");

            // Show game over / score screen UI
            if (gm.UIManager != null)
                gm.UIManager.ShowGameOverScreen();

            // Play game-over music
            if (gm.AudioManager != null)
                gm.AudioManager.PlayMusic(false); // calm / melancholic
        }

        public void Tick()
        {
            // Polling: waiting for Play Again or Main Menu button.
            // UIManager handles button clicks and calls
            // GameManager.GetGameStateMachine().ChangeState(...) accordingly.
        }

        public void Exit()
        {
            // Clear EventBus subscriptions to prevent stale references
            EventBus.ClearAll();

            // Hide game over UI
            var gm = GameManager.Instance;
            if (gm != null && gm.UIManager != null)
                gm.UIManager.HideGameOverScreen();
        }
    }
}
