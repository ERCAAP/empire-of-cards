using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "GameBalance", menuName = "EmpireOfCards/Game Balance")]
    public class GameBalanceData : ScriptableObject
    {
        [Header("General")]
        public int startingMoney = 500;
        public int maxTurns = 20;
        public int startingActions = 3;
        public int maxActions = 5;
        public int startingBusinessSlots = 3;
        public int maxBusinessSlots = 5;
        public int handSize = 5;
        public int redrawsPerTurn = 1;
        public int shopCardsPerTurn = 3;

        [Header("Economy")]
        [Tooltip("Standard tax rate applied to income")]
        public float taxRate = 0.15f;

        [Tooltip("Reduced tax rate (e.g. with Muhasebeci)")]
        public float reducedTaxRate = 0.075f;

        [Tooltip("Minimum possible tax rate")]
        public float minTaxRate = 0.03f;

        [Tooltip("Fraction of buy cost returned when selling a card")]
        public float sellRate = 0.4f;

        [Header("FBI")]
        [Tooltip("Money penalty when an FBI raid occurs")]
        public int fbiRaidPenalty = 300;

        [Tooltip("FBI risk percentage at the start of the game")]
        public float fbiStartingRisk = 0f;

        [Header("Territory")]
        public int totalTerritories = 10;

        [Tooltip("Territories needed for player to win")]
        public int winTerritories = 6;

        [Tooltip("Territories the rival needs for the player to lose")]
        public int loseTerritories = 7;

        [Header("Market Pool")]
        [Tooltip("Starting customer pool available in the market")]
        public int baseMarketCustomers = 60;

        [Tooltip("Customer growth per turn during turns 1-5")]
        public int earlyGrowthPerTurn = 5;

        [Tooltip("Customer growth per turn during turns 6-10")]
        public int midGrowthPerTurn = 6;

        [Tooltip("Customer growth per turn during turns 11-15")]
        public int lateGrowthPerTurn = 8;

        [Tooltip("Customer growth per turn during turns 16-20")]
        public int endGrowthPerTurn = 10;

        [Header("Scoring")]
        public int territoryScoreMultiplier = 500;
        public int moneyScoreMultiplier = 1;
        public int comboScoreMultiplier = 200;
        public int businessScoreMultiplier = 100;
        public int earlyFinishBonusPerTurn = 300;
        public int fbiEvasionBonus = 50;
        public int winBonus = 1000;

        [Header("Events")]
        [Tooltip("An event occurs every N turns")]
        public int eventInterval = 3;
    }
}
