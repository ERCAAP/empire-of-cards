using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Tracks customer loyalty and word-of-mouth organic growth (GDD Section 7.3-7.4).
    /// Good experience builds loyalty; loyalty generates free organic customers.
    /// </summary>
    public class CustomerLoyaltySystem : MonoBehaviour
    {
        [Header("Loyalty State (Read Only)")]
        [SerializeField] private float loyaltyScore;
        [SerializeField] private int loyalCustomerCount;
        [SerializeField] private int organicNewCustomers;

        private float _previousQualityAvg;
        private EconomyManager _economyManager;

        public float LoyaltyScore => loyaltyScore;
        public int LoyalCustomerCount => loyalCustomerCount;
        public int OrganicNewCustomers => organicNewCustomers;

        public void Init(EconomyManager economyManager)
        {
            _economyManager = economyManager;
        }

        public void Reset()
        {
            loyaltyScore = 0f;
            loyalCustomerCount = 0;
            organicNewCustomers = 0;
            _previousQualityAvg = 0f;
        }

        /// <summary>
        /// Called each resolve phase to update loyalty based on current performance.
        /// </summary>
        public void Tick(float platformRating, float qualityAvg, int playerCustomers)
        {
            bool qualityDropped = _previousQualityAvg > 0f
                && qualityAvg < _previousQualityAvg - 2f;

            if (qualityDropped)
            {
                loyaltyScore -= Constants.LOYALTY_DECAY_PER_BAD_TURN;
                Debug.Log($"[CustomerLoyalty] Quality dropped sharply ({_previousQualityAvg:F1} -> {qualityAvg:F1}). Loyalty -{Constants.LOYALTY_DECAY_PER_BAD_TURN}");
            }
            else if (qualityAvg >= 6f && platformRating >= 3.0f)
            {
                loyaltyScore += Constants.LOYALTY_PER_GOOD_TURN;
                Debug.Log($"[CustomerLoyalty] Good turn (quality={qualityAvg:F1}, rating={platformRating:F1}). Loyalty +{Constants.LOYALTY_PER_GOOD_TURN}");
            }
            else if (qualityAvg < 4f)
            {
                loyaltyScore -= Constants.LOYALTY_DECAY_PER_BAD_TURN * 0.5f;
                Debug.Log($"[CustomerLoyalty] Low quality turn ({qualityAvg:F1}). Loyalty -{Constants.LOYALTY_DECAY_PER_BAD_TURN * 0.5f}");
            }

            loyaltyScore = Mathf.Clamp(loyaltyScore, Constants.LOYALTY_MIN, Constants.LOYALTY_MAX);
            _previousQualityAvg = qualityAvg;

            loyalCustomerCount = Mathf.FloorToInt(loyaltyScore / 20f);

            organicNewCustomers = loyalCustomerCount / Constants.LOYALTY_CUSTOMERS_PER_GROUP;
            if (platformRating >= 4.5f)
                organicNewCustomers += 2;
            else if (platformRating >= 4.0f)
                organicNewCustomers += 1;

            EventBus.LoyaltyScoreChanged(loyaltyScore);

            if (organicNewCustomers > 0)
                EventBus.OrganicCustomersGained(organicNewCustomers);

            Debug.Log($"[CustomerLoyalty] Score={loyaltyScore:F1}, LoyalCustomers={loyalCustomerCount}, Organic={organicNewCustomers}");
        }

        /// <summary>
        /// Returns a 0.0-1.0 bonus that feeds into the market share calculation.
        /// </summary>
        public float GetLoyaltyMarketShareBonus()
        {
            return Mathf.Clamp01(loyaltyScore / Constants.LOYALTY_MAX);
        }

        /// <summary>
        /// External penalty: e.g., rival strong campaign erodes loyalty.
        /// </summary>
        public void ApplyLoyaltyPenalty(float amount)
        {
            loyaltyScore = Mathf.Max(Constants.LOYALTY_MIN, loyaltyScore - amount);
            EventBus.LoyaltyScoreChanged(loyaltyScore);
        }
    }
}
