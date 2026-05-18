using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "GameBalance", menuName = "EmpireOfCards/Game Balance")]
    public class GameBalanceData : ScriptableObject
    {
        [Header("--- Genel ---")]
        public int startingMoney = 500;
        public int maxTurns = 20;
        public int startingActions = 3;
        public int maxActions = 5;
        public int startingBusinessSlots = 3;
        public int maxBusinessSlots = 5;
        public int handSize = 5;
        public int redrawsPerTurn = 1;
        public int shopCardsPerTurn = 3;
        public int eventInterval = 3;

        [Header("--- Ekonomi ---")]
        public float taxRate = 0.15f;
        public float reducedTaxRate = 0.075f;   // 1 muhasebeci
        public float minTaxRate = 0.03f;         // 2 muhasebeci
        public float sellRate = 0.4f;

        [Header("--- FBI ---")]
        public int fbiRaidPenalty = 300;
        public float fbiStartingRisk = 0f;

        [Header("--- Bölge ---")]
        public int totalTerritories = 10;
        public int winTerritories = 6;
        public int loseTerritories = 7;

        [Header("--- Market Havuzu (Balance Tablo) ---")]
        public int baseMarketCustomers = 60;
        public int earlyGrowthPerTurn = 5;
        public int midGrowthPerTurn = 6;
        public int lateGrowthPerTurn = 8;
        public int endGrowthPerTurn = 10;

        [Header("--- Skor (GDD 10.3) ---")]
        public int territoryScoreMultiplier = 500;
        public int moneyScoreMultiplier = 1;
        public int comboScoreMultiplier = 200;
        public int businessScoreMultiplier = 100;
        public int earlyFinishBonusPerTurn = 300;
        public int fbiEvasionBonus = 50;
        public int winBonus = 1000;

        [Header("--- İşletme Evrimi ---")]
        public int evolutionCustomerThreshold = 40;
        public int evolutionTurnRequirement = 15;

        [Header("--- Çalışan ---")]
        public int employeeLeaveTurnThreshold = 8;

        public int GetMarketPool(int currentTurn)
        {
            int pool = baseMarketCustomers;
            for (int t = 1; t < currentTurn; t++)
            {
                if (t <= 5) pool += earlyGrowthPerTurn;
                else if (t <= 10) pool += midGrowthPerTurn;
                else if (t <= 15) pool += lateGrowthPerTurn;
                else pool += endGrowthPerTurn;
            }
            return pool;
        }
    }
}
