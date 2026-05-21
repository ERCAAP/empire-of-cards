using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay
{
    public class CustomerSystem : MonoBehaviour
    {
        BoardManager _board;
        EconomyManager _economy;

        public void Init(BoardManager board, EconomyManager economy)
        {
            _board = board;
            _economy = economy;
            Debug.Log("[CustomerSystem] Initialized.");
        }

        // ── Main Calculation ───────────────────────────────────────

        public (int served, int waited, int left) CalculateCustomerFlow(
            float demand, float capacity, float quality,
            float rating, float hygiene, MenuPricing pricing)
        {
            int totalDemand = Mathf.RoundToInt(
                demand * Constants.TOTAL_MARKET_CUSTOMERS * 0.1f);

            totalDemand = ApplyCustomerTypeFilter(totalDemand, rating, hygiene, pricing);
            int maxCapacity = Mathf.RoundToInt(capacity * 3f);

            int served = Mathf.Min(totalDemand, maxCapacity);
            int overflow = totalDemand - served;
            int waited = Mathf.RoundToInt(overflow * 0.4f);
            int left = overflow - waited;

            return (served, waited, left);
        }

        // ── Satisfaction ───────────────────────────────────────────

        public float CalculateSatisfaction(float quality, float speed,
            float hygiene, float priceScore, float ambiance)
        {
            float satisfaction =
                quality * Constants.SATISFACTION_QUALITY_WEIGHT
                + speed * Constants.SATISFACTION_SPEED_WEIGHT
                + hygiene * Constants.SATISFACTION_HYGIENE_WEIGHT
                + priceScore * Constants.SATISFACTION_PRICE_WEIGHT
                + ambiance * Constants.SATISFACTION_AMBIANCE_WEIGHT;

            return Mathf.Clamp(satisfaction, 0f, 10f);
        }

        public float SatisfactionToRatingDelta(float satisfaction)
        {
            if (satisfaction > 7f)
                return (satisfaction - 7f) * 0.1f;
            if (satisfaction < 4f)
                return (satisfaction - 4f) * 0.1f;
            return 0f;
        }

        // ── Customer Type Filtering ────────────────────────────────

        int ApplyCustomerTypeFilter(int baseDemand, float rating,
            float hygiene, MenuPricing pricing)
        {
            float[] dist = GetTypeDistribution(rating, hygiene, pricing);
            float totalMult = 0f;

            for (int i = 0; i < dist.Length; i++)
                totalMult += dist[i] * GetTypeMultiplier((CustomerType)i);

            return Mathf.RoundToInt(baseDemand * Mathf.Max(totalMult, 0.3f));
        }

        float[] GetTypeDistribution(float rating, float hygiene, MenuPricing pricing)
        {
            float[] dist = new float[6];

            dist[(int)CustomerType.Random] = 0.3f;
            dist[(int)CustomerType.BargainHunter] = pricing == MenuPricing.Economy ? 0.25f : 0.10f;
            dist[(int)CustomerType.Gourmet] = rating > 4.2f ? 0.15f : 0.03f;
            dist[(int)CustomerType.Loyal] = 0.15f;
            dist[(int)CustomerType.Influencer] = rating > 4.2f ? 0.10f : 0.02f;
            dist[(int)CustomerType.Family] = hygiene >= 5f ? 0.15f : 0.02f;

            NormalizeDistribution(dist);
            return dist;
        }

        float GetTypeMultiplier(CustomerType type)
        {
            switch (type)
            {
                case CustomerType.BargainHunter: return 0.8f;
                case CustomerType.Gourmet:       return 1.5f;
                case CustomerType.Loyal:         return 1.2f;
                case CustomerType.Influencer:    return 1.3f;
                case CustomerType.Family:        return 1.1f;
                case CustomerType.Random:        return 1.0f;
                default:                         return 1.0f;
            }
        }

        // ── Utility ────────────────────────────────────────────────

        static void NormalizeDistribution(float[] dist)
        {
            float sum = 0f;
            for (int i = 0; i < dist.Length; i++) sum += dist[i];
            if (sum <= 0f) return;
            for (int i = 0; i < dist.Length; i++) dist[i] /= sum;
        }
    }
}
