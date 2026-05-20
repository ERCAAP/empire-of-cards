using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewRival", menuName = "EmpireOfCards/Rival Data")]
    public class RivalData : ScriptableObject
    {
        [Header("--- Identity ---")]
        public string rivalId;              // "RIVAL_MegaCorp"
        public string rivalName;            // "MegaCorp"
        public RivalPersonality personality;
        public Sprite portrait;
        [TextArea(1, 2)]
        public string tagline;              // "This industry isn't big enough for both of us."

        [Header("--- Starting Stats ---")]
        public int startingMoney = 400;
        public int startingIncome = 80;     // First business income
        public int startingCustomers = 5;   // First business customers
        public string startingBusinessName = "MegaCorp HQ";

        [Header("--- Behavior ---")]
        public int actionsPerTurn = 2;      // Normal: 2 actions/turn
        [Range(0f, 1f)]
        public float aggressionThreshold = 0.5f;  // Player territories > threshold -> aggressive
        public int maxBusinesses = 4;
        public int maxEmployeesPerBusiness = 3;

        [Header("--- Venture Mirror (GDD Section 1.7) ---")]
        public string[] ventureMatchedNames;        // Business name per VentureType ordinal
        public int[] ventureMatchedIncome;           // Starting income per VentureType ordinal
        public int[] ventureMatchedCustomers;        // Starting customers per VentureType ordinal

        [Header("--- Growth Parameters ---")]
        public int businessCostThreshold = 200;
        public int hireCostThreshold = 80;
        public int baseBusinessIncome = 80;
        public int baseBusinessCustomers = 5;
        public int employeeIncomeBoost = 30;
        public int employeeCustomerBoost = 3;
        public int aggressiveCustomerBoost = 8;
        public int aggressiveIncomeBoost = 50;
        public int passiveCustomerGrowth = 2;
        public int passiveIncomeGrowth = 10;
        public int passiveMoneyGrowth = 50;         // Flat money per normal-growth action
        public int eventBonusCustomers = 3;          // Customers per business from event bonus
        public int eventBonusIncome = 15;            // Income per business from event bonus
        public string[] possibleBusinessNames;  // ["Tech Store", "Supermarket", ...]

        [Header("--- Growth Schedule (GDD Section 8.3) ---")]
        public RivalMilestone[] growthMilestones;

        [Header("--- Dialogue ---")]
        [TextArea(1, 2)]
        public string[] growingTaunts;      // Rival growing: "Our market share is growing."
        [TextArea(1, 2)]
        public string[] playerGrowingTaunts;// Player growing: "Interesting move..."
        [TextArea(1, 2)]
        public string[] aggressiveTaunts;   // Aggressive: "This industry isn't big enough for both of us."
        [TextArea(1, 2)]
        public string[] losingTaunts;       // Losing: "This isn't over."
        [TextArea(1, 2)]
        public string[] winningTaunts;      // Winning: "It was inevitable."
    }

    [System.Serializable]
    public class RivalMilestone
    {
        public int turn;                    // Turn 5, 8, 12, 15 etc
        public int targetBusinesses;        // Target business count
        public int targetEmployees;         // Target employee count
        public int targetMarketBlocks;      // Target market share blocks
        public bool enableAggression;       // Can be aggressive after this turn
    }
}
