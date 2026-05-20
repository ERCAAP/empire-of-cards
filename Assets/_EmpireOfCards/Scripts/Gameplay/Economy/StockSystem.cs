using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay.Economy
{
    public class StockSystem
    {
        public float CalculateSpoilage(VentureType venture, int turn)
        {
            switch (venture)
            {
                case VentureType.FastFood:
                case VentureType.Cafe:
                case VentureType.GroceryStore:
                    // Every 2-3 turns, 10-20% spoilage
                    bool isSpoilageTurn = (turn % Constants.STOCK_SPOILAGE_INTERVAL) == 0;
                    if (isSpoilageTurn)
                    {
                        return Random.Range(
                            Constants.STOCK_SPOILAGE_MIN,
                            Constants.STOCK_SPOILAGE_MAX);
                    }
                    return 0f;

                case VentureType.ClothingStore:
                    // Clothing has no regular spoilage -- uses season stock loss instead
                    return 0f;

                case VentureType.TechApp:
                    // Tech has no stock management
                    return 0f;

                default:
                    return 0f;
            }
        }

        public float CalculateSeasonStockLoss(VentureType venture, SeasonType from, SeasonType to)
        {
            if (venture == VentureType.ClothingStore)
            {
                // Season transition causes stock write-off for clothing
                if (from != to)
                {
                    return Constants.STOCK_CLOTHING_SEASON_LOSS;
                }
            }

            return 0f;
        }

        public int ApplySpoilage(int grossIncome, VentureType venture, int currentTurn)
        {
            float spoilageRate = CalculateSpoilage(venture, currentTurn);
            if (spoilageRate <= 0f) return 0;

            int spoilageCost = Mathf.RoundToInt(grossIncome * spoilageRate);
            if (spoilageCost > 0)
            {
                EventBus.StockSpoilageOccurred(venture, spoilageCost);
                Debug.Log($"[StockSystem] Spoilage for {venture}: -{spoilageCost} ({spoilageRate:P0})");
            }
            return spoilageCost;
        }

        public int ApplySeasonStockLoss(int grossIncome, VentureType venture, SeasonType from, SeasonType to)
        {
            float lossRate = CalculateSeasonStockLoss(venture, from, to);
            if (lossRate <= 0f) return 0;

            int lossCost = Mathf.RoundToInt(grossIncome * lossRate);
            if (lossCost > 0)
            {
                EventBus.StockSeasonLossOccurred(venture, lossCost);
                Debug.Log($"[StockSystem] Season stock loss for {venture}: -{lossCost} ({lossRate:P0})");
            }
            return lossCost;
        }
    }
}
