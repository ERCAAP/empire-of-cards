namespace EmpireOfCards.Core
{
    /// <summary>
    /// Pure-logic win/lose evaluation. No side effects, no dependencies.
    /// Extracted from GameManager so victory conditions are testable in isolation.
    /// </summary>
    public static class WinLoseChecker
    {
        /// <summary>
        /// WIN: Player has enough territories (GDD Section 6.3, default 6).
        /// </summary>
        public static bool CheckWin(int playerTerritories, int winRequirement)
        {
            return playerTerritories >= winRequirement;
        }

        /// <summary>
        /// LOSE: Rival dominates (default 7 territories) OR player is bankrupt.
        /// Turn-20 timeout is handled separately by GameManager.EndCurrentTurn().
        /// </summary>
        public static bool CheckLose(int rivalTerritories, int loseRequirement, int playerMoney)
        {
            // Rival dominates
            if (rivalTerritories >= loseRequirement)
                return true;

            // Bankruptcy (GDD: Money drops to 0 = BANKRUPT)
            if (playerMoney <= 0)
                return true;

            return false;
        }
    }
}
