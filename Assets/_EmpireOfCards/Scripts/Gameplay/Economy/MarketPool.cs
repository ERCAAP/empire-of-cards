using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay.Economy
{
    /// <summary>
    /// Status: legacy-leaning helper. It reflects the older pooled-customer
    /// scoring model; the active v4 runtime primarily resolves share through
    /// EconomyManager snapshot values.
    /// </summary>
    public class MarketPool
    {
        private readonly GameBalanceData balanceData;

        public MarketPool(GameBalanceData balanceData)
        {
            this.balanceData = balanceData;
        }

        /// <summary>
        /// Calculates the player's share of the 100-customer market pool (GDD v3.0 Section 7).
        /// All score parameters are on a 0-10 scale. platformRating is 1.0-5.0 (normalized internally).
        /// Returns a 0.0-1.0 fractional share of the market.
        /// </summary>
        public float CalculatePlayerMarketShare(
            float qualityScore,
            float priceScore,
            float platformRating,
            float marketingScore,
            float serviceSpeed,
            float loyaltyScore)
        {
            // Normalize platformRating from [1,5] to [0,10]
            float normalizedRating = (platformRating - 1f) / (5f - 1f) * 10f;

            float weightedSum =
                qualityScore   * Constants.CUSTOMER_WEIGHT_QUALITY           +
                priceScore     * Constants.CUSTOMER_WEIGHT_PRICE             +
                normalizedRating * Constants.CUSTOMER_WEIGHT_PLATFORM_RATING +
                marketingScore * Constants.CUSTOMER_WEIGHT_MARKETING         +
                serviceSpeed   * Constants.CUSTOMER_WEIGHT_SPEED             +
                loyaltyScore   * Constants.CUSTOMER_WEIGHT_LOYALTY;

            // Max possible score: all inputs at 10
            float maxScore = 10f * (
                Constants.CUSTOMER_WEIGHT_QUALITY +
                Constants.CUSTOMER_WEIGHT_PRICE +
                Constants.CUSTOMER_WEIGHT_PLATFORM_RATING +
                Constants.CUSTOMER_WEIGHT_MARKETING +
                Constants.CUSTOMER_WEIGHT_SPEED +
                Constants.CUSTOMER_WEIGHT_LOYALTY);

            return UnityEngine.Mathf.Clamp01(weightedSum / maxScore);
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
