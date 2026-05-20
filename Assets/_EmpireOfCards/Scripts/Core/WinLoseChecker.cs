namespace EmpireOfCards.Core
{
    /// <summary>
    /// Pure-logic win/lose evaluation. No side effects, no dependencies.
    /// Extracted from GameManager so victory conditions are testable in isolation.
    ///
    /// GDD v3.0: Win/lose is based on customer share, not territory count.
    /// Player wins at 60 customers, rival wins (player loses) at 60 customers.
    /// Bankruptcy (money <= 0) is still a lose condition.
    /// </summary>
    public static class WinLoseChecker
    {
        /// <summary>
        /// WIN: Player has enough customers (GDD v3.0, default 60).
        /// </summary>
        public static bool CheckWin(int playerCustomers, int winThreshold)
        {
            return playerCustomers >= winThreshold;
        }

        /// <summary>
        /// LOSE: Rival has enough customers OR player is bankrupt.
        /// Turn timeout is handled separately by GameManager.EndCurrentTurn().
        /// </summary>
        public static bool CheckLose(int rivalCustomers, int loseThreshold, int playerMoney)
        {
            if (rivalCustomers >= loseThreshold)
                return true;

            if (playerMoney <= 0)
                return true;

            return false;
        }
    }
}
