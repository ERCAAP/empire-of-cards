using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay.Economy
{
    /// <summary>
    /// Calculates customer market pool size per turn.
    /// Base 60, +5/turn (1-5), +6 (6-10), +8 (11-15), +10 (16-20).
    /// </summary>
    public class MarketPool
    {
        private readonly GameBalanceData balanceData;

        public MarketPool(GameBalanceData balanceData)
        {
            this.balanceData = balanceData;
        }

        /// <summary>
        /// Returns the total customer market pool for the given turn.
        /// Base 60, +5/turn (1-5), +6 (6-10), +8 (11-15), +10 (16-20).
        /// </summary>
        public int GetMarketPool(int currentTurn)
        {
            if (balanceData != null)
                return balanceData.GetMarketPool(currentTurn);

            // Fallback to Constants
            int pool = Constants.BASE_MARKET_CUSTOMERS;
            for (int t = 1; t < currentTurn; t++)
            {
                if (t <= 5) pool += Constants.EARLY_GROWTH_PER_TURN;
                else if (t <= 10) pool += Constants.MID_GROWTH_PER_TURN;
                else if (t <= 15) pool += Constants.LATE_GROWTH_PER_TURN;
                else pool += Constants.END_GROWTH_PER_TURN;
            }
            return pool;
        }
    }
}
