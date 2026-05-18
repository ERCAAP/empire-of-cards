using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Represents one of the rival's active businesses.
    /// </summary>
    [Serializable]
    public class RivalBusiness
    {
        public string name;
        public int income;
        public int customers;
        public int employeeCount;
    }

    /// <summary>
    /// AI opponent logic. Decides actions each turn based on personality and game state.
    /// </summary>
    public class RivalAI : MonoBehaviour
    {
        // --- Data ---
        [Header("Rival Configuration")]
        [SerializeField] private RivalData data;

        // --- Runtime State ---
        [Header("Rival State (Read Only)")]
        [SerializeField] private int rivalMoney;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private List<RivalBusiness> rivalBusinesses = new List<RivalBusiness>();
        [SerializeField] private string currentMood = "neutral";

        // --- Events ---
        public event Action<string> OnRivalAction;

        // --- Properties ---
        public int RivalMoney => rivalMoney;
        public int RivalCustomers => rivalCustomers;
        public IReadOnlyList<RivalBusiness> RivalBusinesses => rivalBusinesses;
        public string CurrentMood => currentMood;

        /// <summary>
        /// Initializes the rival with starting values from RivalData.
        /// </summary>
        public void Initialize()
        {
            if (data == null)
            {
                Debug.LogError("[RivalAI] RivalData is null. Cannot initialize.");
                return;
            }

            rivalMoney = data.startingMoney;
            rivalCustomers = 0;
            rivalBusinesses.Clear();
            currentMood = "neutral";

            // Start with one default business
            rivalBusinesses.Add(new RivalBusiness
            {
                name = data.startingBusinessName,
                income = data.startingIncome,
                customers = data.startingCustomers,
                employeeCount = 1
            });
        }

        /// <summary>
        /// Main AI turn entry point. Evaluates the game state and takes one or more actions.
        /// </summary>
        public void TakeTurn(int playerTerritories, int currentTurn)
        {
            if (data == null)
                return;

            UpdateMood(playerTerritories);
            CalculateRivalIncome();

            string action = DecideAction(playerTerritories, currentTurn);

            switch (action)
            {
                case "open_business":
                    OpenBusiness();
                    break;
                case "hire_employee":
                    HireEmployee();
                    break;
                case "aggressive":
                    AggressiveAction();
                    break;
                default:
                    NormalGrowth();
                    break;
            }

            CalculateRivalCustomers();
        }

        /// <summary>
        /// Decision tree: determines the best action based on personality, mood, and game state.
        /// </summary>
        public string DecideAction(int playerTerritories, int currentTurn)
        {
            if (data == null)
                return "normal_growth";

            int rivalTerritories = GameManager.Instance.RivalTerritories;

            // If player is dominating, get aggressive
            if (playerTerritories >= rivalTerritories + 3)
            {
                if (data.personality == RivalPersonality.Aggressive || currentMood == "angry")
                    return "aggressive";
            }

            // If rival has money and few businesses, expand
            if (rivalBusinesses.Count < 3 && rivalMoney >= data.businessCostThreshold)
            {
                return "open_business";
            }

            // If businesses need employees
            foreach (var business in rivalBusinesses)
            {
                if (business.employeeCount < data.maxEmployeesPerBusiness && rivalMoney >= data.hireCostThreshold)
                {
                    return "hire_employee";
                }
            }

            // Late-game aggression
            if (currentTurn >= 15 && rivalTerritories < playerTerritories)
            {
                return "aggressive";
            }

            return "normal_growth";
        }

        /// <summary>
        /// Opens a new rival business.
        /// </summary>
        public void OpenBusiness()
        {
            if (data == null)
                return;

            int cost = data.businessCostThreshold;
            if (rivalMoney < cost)
                return;

            rivalMoney -= cost;

            string businessName = data.possibleBusinessNames != null && data.possibleBusinessNames.Length > 0
                ? data.possibleBusinessNames[UnityEngine.Random.Range(0, data.possibleBusinessNames.Length)]
                : $"Rival Business {rivalBusinesses.Count + 1}";

            RivalBusiness newBusiness = new RivalBusiness
            {
                name = businessName,
                income = data.baseBusinessIncome,
                customers = data.baseBusinessCustomers,
                employeeCount = 1
            };

            rivalBusinesses.Add(newBusiness);
            OnRivalAction?.Invoke($"Rival opened a new business: {businessName}");
        }

        /// <summary>
        /// Hires an employee for the rival business that needs one most.
        /// </summary>
        public void HireEmployee()
        {
            if (data == null)
                return;

            int cost = data.hireCostThreshold;
            if (rivalMoney < cost)
                return;

            // Find the business with the fewest employees
            RivalBusiness target = null;
            int minEmployees = int.MaxValue;

            foreach (var business in rivalBusinesses)
            {
                if (business.employeeCount < data.maxEmployeesPerBusiness && business.employeeCount < minEmployees)
                {
                    minEmployees = business.employeeCount;
                    target = business;
                }
            }

            if (target == null)
                return;

            rivalMoney -= cost;
            target.employeeCount++;
            target.income += data.employeeIncomeBoost;
            target.customers += data.employeeCustomerBoost;

            OnRivalAction?.Invoke($"Rival hired an employee for {target.name}.");
        }

        /// <summary>
        /// Takes an aggressive action: boosts an existing business significantly.
        /// </summary>
        public void AggressiveAction()
        {
            if (data == null || rivalBusinesses.Count == 0)
                return;

            // Pick the strongest business and boost it
            RivalBusiness strongest = rivalBusinesses[0];
            foreach (var business in rivalBusinesses)
            {
                if (business.customers > strongest.customers)
                    strongest = business;
            }

            int investmentCost = Mathf.RoundToInt(rivalMoney * 0.3f);
            if (investmentCost <= 0)
                return;

            rivalMoney -= investmentCost;
            strongest.customers += data.aggressiveCustomerBoost;
            strongest.income += data.aggressiveIncomeBoost;

            OnRivalAction?.Invoke($"Rival made an aggressive move with {strongest.name}!");
        }

        /// <summary>
        /// Normal growth: all businesses gain a small passive bonus.
        /// </summary>
        public void NormalGrowth()
        {
            foreach (var business in rivalBusinesses)
            {
                business.customers += data != null ? data.passiveCustomerGrowth : 2;
                business.income += data != null ? data.passiveIncomeGrowth : 5;
            }

            OnRivalAction?.Invoke("Rival businesses grew steadily.");
        }

        /// <summary>
        /// Returns a taunt or dialogue line based on the current mood and game state.
        /// </summary>
        public string GetTaunt()
        {
            if (data == null || data.taunts == null || data.taunts.Length == 0)
                return "...";

            int rivalTerritories = GameManager.Instance.RivalTerritories;
            int playerTerritories = GameManager.Instance.PlayerTerritories;

            // Pick taunt based on mood
            if (currentMood == "angry" && data.angryTaunts != null && data.angryTaunts.Length > 0)
                return data.angryTaunts[UnityEngine.Random.Range(0, data.angryTaunts.Length)];

            if (rivalTerritories > playerTerritories && data.winningTaunts != null && data.winningTaunts.Length > 0)
                return data.winningTaunts[UnityEngine.Random.Range(0, data.winningTaunts.Length)];

            if (rivalTerritories < playerTerritories && data.losingTaunts != null && data.losingTaunts.Length > 0)
                return data.losingTaunts[UnityEngine.Random.Range(0, data.losingTaunts.Length)];

            return data.taunts[UnityEngine.Random.Range(0, data.taunts.Length)];
        }

        /// <summary>
        /// Recalculates the total number of customers across all rival businesses.
        /// </summary>
        public void CalculateRivalCustomers()
        {
            int total = 0;
            foreach (var business in rivalBusinesses)
            {
                total += business.customers;
            }
            rivalCustomers = total;
        }

        private void CalculateRivalIncome()
        {
            int totalIncome = 0;
            foreach (var business in rivalBusinesses)
            {
                totalIncome += business.income;
            }
            rivalMoney += totalIncome;
        }

        private void UpdateMood(int playerTerritories)
        {
            int rivalTerritories = GameManager.Instance.RivalTerritories;

            if (playerTerritories >= rivalTerritories + 3)
                currentMood = "angry";
            else if (rivalTerritories >= playerTerritories + 3)
                currentMood = "confident";
            else
                currentMood = "neutral";
        }
    }
}
