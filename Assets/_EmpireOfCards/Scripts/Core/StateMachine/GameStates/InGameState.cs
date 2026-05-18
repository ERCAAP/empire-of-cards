using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.GameStates
{
    /// <summary>
    /// The main gameplay state. TurnManager handles turn/phase flow internally.
    /// Tick polls win/lose conditions every frame so the game can end
    /// the moment a threshold is crossed — no event subscription needed.
    /// </summary>
    public class InGameState : IState
    {
        public void Enter()
        {
            var gm = GameManager.Instance;
            gm.SetGameState(GameState.Playing);

            // TurnManager is already running; ensure game flag is on
            if (!gm.GameIsRunning)
                return;
        }

        public void Tick()
        {
            var gm = GameManager.Instance;
            if (gm == null || !gm.GameIsRunning)
                return;

            // Poll win/lose every frame
            int winReq = gm.BalanceData != null ? gm.BalanceData.winTerritories : Constants.WIN_TERRITORIES;
            int loseReq = gm.BalanceData != null ? gm.BalanceData.loseTerritories : Constants.LOSE_TERRITORIES;

            if (WinLoseChecker.CheckWin(gm.PlayerTerritories, winReq))
            {
                gm.EndRun(true);
                return;
            }
            if (WinLoseChecker.CheckLose(gm.RivalTerritories, loseReq, gm.PlayerMoney))
            {
                gm.EndRun(false);
                return;
            }
        }

        public void Exit()
        {
            // TurnManager stops itself when GameManager.EndRun() is called
        }
    }
}
