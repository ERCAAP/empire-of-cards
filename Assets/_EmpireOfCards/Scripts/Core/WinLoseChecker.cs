namespace EmpireOfCards.Core
{
    /// <summary>
    /// Pure-logic win/lose evaluation. No side effects, no dependencies.
    /// Extracted from GameManager so victory conditions are testable in isolation.
    ///
    /// GDD v3.0: Win/lose is based on customer share, not territory count.
    /// Player wins at 60 customers, rival wins (player loses) at 60 customers.
    ///
    /// GDD 13.3 loss conditions:
    /// 1. Bankruptcy (money <= 0)
    /// 2. Reputation collapse (rating <= 1.0 for 3+ consecutive turns)
    /// 3. Legal disaster (legalRisk >= 90)
    /// 4. Rival domination (rival market share >= 70)
    /// </summary>
    public static class WinLoseChecker
    {
        private static int _lowRatingConsecutiveTurns;

        /// <summary>
        /// WIN: Player has enough customers (GDD v3.0, default 60).
        /// </summary>
        public static bool CheckWin(int playerCustomers, int winThreshold)
        {
            return playerCustomers >= winThreshold;
        }

        /// <summary>
        /// LOSE: Any of the 4 GDD 13.3 conditions met.
        /// Turn timeout is handled separately by GameManager.EndCurrentTurn().
        /// </summary>
        public static bool CheckLose(int rivalCustomers, int loseThreshold, int playerMoney)
        {
            // 1. Bankruptcy
            if (playerMoney <= 0)
                return true;

            // 4. Rival domination (market share >= 70)
            if (rivalCustomers >= loseThreshold)
                return true;

            return false;
        }

        /// <summary>
        /// Extended loss check using EconomyManager snapshot values (GDD 13.3).
        /// Call once per turn end, after ProcessEndOfTurn has updated the snapshot.
        /// </summary>
        public static bool CheckExtendedLose(float rating, float legalRisk)
        {
            // 2. Reputation collapse: rating <= 1.0 for 3+ consecutive turns
            if (rating <= 1.0f)
            {
                _lowRatingConsecutiveTurns++;
                if (_lowRatingConsecutiveTurns >= 3)
                    return true;
            }
            else
            {
                _lowRatingConsecutiveTurns = 0;
            }

            // 3. Legal disaster: legalRisk >= 90
            if (legalRisk >= 90f)
                return true;

            return false;
        }

        /// <summary>
        /// Reset tracked state. Call on new run start.
        /// </summary>
        public static void Reset()
        {
            _lowRatingConsecutiveTurns = 0;
        }
    }
}
