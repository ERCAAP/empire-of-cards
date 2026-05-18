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
        public int maxEmployees = 3;
    }

    /// <summary>
    /// AI opponent logic matching GDD Section 8.
    ///
    /// Decision tree:
    /// 1. Player territories > 5 => AGGRESSIVE
    /// 2. Rival money > business cost AND less than 3 businesses => OPEN BUSINESS
    /// 3. Empty employee slots => HIRE
    /// 4. Event benefits rival => USE EVENT BONUS
    /// 5. Default => NORMAL GROWTH (+50 money, +2 customers)
    ///
    /// Growth milestones (GDD 8.3):
    /// Turn 1:  1 business (80/turn), 1 employee, 2 territories
    /// Turn 5:  2 businesses, 2-3 employees, 3 territories
    /// Turn 8:  2-3 businesses, 3-4 employees, 3-4 territories
    /// Turn 12: 3 businesses, 4-5 employees, aggressive, 4-5 territories
    /// Turn 15: 3-4 businesses, 5-6 employees, 4-6 territories
    /// </summary>
    public class RivalAI : MonoBehaviour
    {
        // --- Data ---
        [Header("Rival Configuration")]
        [SerializeField] private RivalData data;

        // --- Runtime State ---
        [Header("Rival State (Read Only)")]
        [SerializeField] private int rivalMoney;
        [SerializeField] private int rivalIncome;
        [SerializeField] private int rivalCustomers;
        [SerializeField] private int totalRivalEmployees;
        [SerializeField] private List<RivalBusiness> rivalBusinesses = new List<RivalBusiness>();
        [SerializeField] private string currentMood = "neutral";
        [SerializeField] private bool aggressionEnabled;

        // --- Properties ---
        public int RivalMoney => rivalMoney;
        public int RivalCustomers => rivalCustomers;
        public int RivalIncome => rivalIncome;
        public IReadOnlyList<RivalBusiness> RivalBusinesses => rivalBusinesses;
        public string CurrentMood => currentMood;
        public RivalData Data => data;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService instead of RuntimeWiring.SetField().
        /// </summary>
        public void Init(RivalData rivalData)
        {
            this.data = rivalData;
        }

        // ----------------------------------------------------------------
        // Initialization
        // ----------------------------------------------------------------

        /// <summary>
        /// Initializes the rival with starting values from RivalData.
        /// Creates the first business.
        /// </summary>
        public void Initialize()
        {
            if (data == null)
            {
                Debug.LogError("[RivalAI] RivalData is null. Cannot initialize.");
                return;
            }

            rivalMoney = data.startingMoney;
            rivalIncome = data.startingIncome;
            rivalCustomers = data.startingCustomers;
            totalRivalEmployees = 1;
            aggressionEnabled = false;
            currentMood = "neutral";
            rivalBusinesses.Clear();

            // Start with one default business (GDD: Turn 1, 1 business, 80/turn, 1 employee)
            rivalBusinesses.Add(new RivalBusiness
            {
                name = data.startingBusinessName,
                income = data.startingIncome,
                customers = data.startingCustomers,
                employeeCount = 1,
                maxEmployees = data.maxEmployeesPerBusiness
            });

            Debug.Log($"[RivalAI] Initialized: {data.rivalName}, money={rivalMoney}, businesses=1");
        }

        /// <summary>
        /// Initializes with a specific RivalData asset (for mid-game rival switch).
        /// </summary>
        public void Initialize(RivalData rivalData)
        {
            data = rivalData;
            Initialize();
        }

        // ----------------------------------------------------------------
        // Turn Execution
        // ----------------------------------------------------------------

        /// <summary>
        /// Main AI turn entry point. Executes actionsPerTurn actions using the decision tree.
        /// Called during the Rival Phase.
        /// </summary>
        public void TakeTurn(int playerTerritories, int currentTurn)
        {
            if (data == null) return;

            // Step 1: Collect income from all businesses
            CollectIncome();

            // Step 2: Check growth milestones and catch up if behind
            ApplyGrowthMilestones(currentTurn);

            // Step 3: Update mood based on territory comparison
            UpdateMood(playerTerritories);

            // Step 4: Execute actions (GDD: actionsPerTurn, default 2)
            int actions = data.actionsPerTurn;
            for (int a = 0; a < actions; a++)
            {
                string decision = DecideAction(playerTerritories, currentTurn);
                ExecuteAction(decision);
            }

            // Step 5: Recalculate totals
            CalculateRivalCustomers();
            CalculateRivalIncome();

            // Step 6: Deliver a taunt
            string taunt = GetTaunt();
            if (!string.IsNullOrEmpty(taunt))
            {
                EventBus.RivalTaunted(taunt);
            }
        }

        // ----------------------------------------------------------------
        // Decision Tree (GDD Section 8)
        // ----------------------------------------------------------------

        /// <summary>
        /// Decision tree exactly as GDD specifies:
        /// 1. Player territories > 5 => AGGRESSIVE
        /// 2. Rival money > business cost AND less than 3 businesses => OPEN_BUSINESS
        /// 3. Empty employee slots => HIRE_EMPLOYEE
        /// 4. Event benefits rival => EVENT_BONUS
        /// 5. Default => NORMAL_GROWTH
        /// </summary>
        public string DecideAction(int playerTerritories, int currentTurn)
        {
            if (data == null) return "normal_growth";

            // 1. Player territories > 5 => AGGRESSIVE
            if (playerTerritories > 5 && aggressionEnabled)
            {
                return "aggressive";
            }

            // 2. Rival money > business cost AND < 3 businesses => OPEN BUSINESS
            if (rivalMoney >= data.businessCostThreshold && rivalBusinesses.Count < 3)
            {
                return "open_business";
            }

            // 3. Empty employee slots => HIRE
            if (HasEmptyEmployeeSlots() && rivalMoney >= data.hireCostThreshold)
            {
                return "hire_employee";
            }

            // 4. Event benefits rival => EVENT BONUS
            // (Simplified: in late game, aggressive if behind)
            if (currentTurn >= 12 && aggressionEnabled)
            {
                int rivalTerritories = GameManager.Instance != null ? GameManager.Instance.RivalTerritories : 0;
                if (rivalTerritories < playerTerritories)
                    return "event_bonus";
            }

            // 5. Default => NORMAL GROWTH (+50 money, +2 customers)
            return "normal_growth";
        }

        /// <summary>
        /// Executes a single action based on the decision string.
        /// </summary>
        private void ExecuteAction(string action)
        {
            switch (action)
            {
                case "aggressive":
                    AggressiveAction();
                    break;
                case "open_business":
                    OpenBusiness();
                    break;
                case "hire_employee":
                    HireEmployee();
                    break;
                case "event_bonus":
                    EventBonusAction();
                    break;
                default:
                    NormalGrowth();
                    break;
            }
        }

        // ----------------------------------------------------------------
        // Actions
        // ----------------------------------------------------------------

        /// <summary>
        /// Opens a new rival business. Costs businessCostThreshold money.
        /// New business starts with baseBusinessIncome and baseBusinessCustomers.
        /// </summary>
        private void OpenBusiness()
        {
            if (data == null) return;
            if (rivalBusinesses.Count >= data.maxBusinesses) return;

            int cost = data.businessCostThreshold;
            if (rivalMoney < cost) return;

            rivalMoney -= cost;

            string businessName = "Rival Business";
            if (data.possibleBusinessNames != null && data.possibleBusinessNames.Length > 0)
            {
                businessName = data.possibleBusinessNames[
                    UnityEngine.Random.Range(0, data.possibleBusinessNames.Length)];
            }

            RivalBusiness newBiz = new RivalBusiness
            {
                name = businessName,
                income = data.baseBusinessIncome,
                customers = data.baseBusinessCustomers,
                employeeCount = 0,
                maxEmployees = data.maxEmployeesPerBusiness
            };

            rivalBusinesses.Add(newBiz);
            EventBus.RivalActed($"Rakip yeni isletme acti: {businessName}");
        }

        /// <summary>
        /// Hires an employee for the rival business with the fewest employees.
        /// Costs hireCostThreshold money.
        /// </summary>
        private void HireEmployee()
        {
            if (data == null) return;

            int cost = data.hireCostThreshold;
            if (rivalMoney < cost) return;

            // Find business with fewest employees that has room
            RivalBusiness target = null;
            int minEmployees = int.MaxValue;

            foreach (var biz in rivalBusinesses)
            {
                if (biz.employeeCount < biz.maxEmployees && biz.employeeCount < minEmployees)
                {
                    minEmployees = biz.employeeCount;
                    target = biz;
                }
            }

            if (target == null) return;

            rivalMoney -= cost;
            target.employeeCount++;
            target.income += data.employeeIncomeBoost;
            target.customers += data.employeeCustomerBoost;
            totalRivalEmployees++;

            EventBus.RivalActed($"Rakip {target.name} icin calisan ise aldi.");
        }

        /// <summary>
        /// Aggressive action: boosts the strongest business significantly.
        /// Invests 30% of current money.
        /// </summary>
        private void AggressiveAction()
        {
            if (data == null || rivalBusinesses.Count == 0) return;

            // Pick the strongest business
            RivalBusiness strongest = rivalBusinesses[0];
            foreach (var biz in rivalBusinesses)
            {
                if (biz.customers > strongest.customers)
                    strongest = biz;
            }

            int investmentCost = Mathf.RoundToInt(rivalMoney * 0.3f);
            if (investmentCost <= 0) return;

            rivalMoney -= investmentCost;
            strongest.customers += data.aggressiveCustomerBoost;
            strongest.income += data.aggressiveIncomeBoost;

            EventBus.RivalActed($"Rakip agresif hamle yapti: {strongest.name}!");
        }

        /// <summary>
        /// Normal growth: +50 money, +2 customers spread across businesses.
        /// GDD default action when nothing else applies.
        /// </summary>
        private void NormalGrowth()
        {
            rivalMoney += 50; // GDD: +50 money

            foreach (var biz in rivalBusinesses)
            {
                biz.customers += data != null ? data.passiveCustomerGrowth : 2; // GDD: +2 customers
            }

            EventBus.RivalActed("Rakip isletmeleri istikrarli buyudu.");
        }

        /// <summary>
        /// Event bonus action: rival leverages current events for a growth boost.
        /// Simulated as a moderate boost to all businesses.
        /// </summary>
        private void EventBonusAction()
        {
            foreach (var biz in rivalBusinesses)
            {
                biz.customers += 3;
                biz.income += 15;
            }

            EventBus.RivalActed("Rakip event bonusunu kullandi.");
        }

        // ----------------------------------------------------------------
        // Growth Milestones (GDD Section 8.3)
        // ----------------------------------------------------------------

        /// <summary>
        /// Checks rival growth milestones and ensures the rival catches up
        /// if it has fallen behind the expected curve.
        ///
        /// Milestone targets come from RivalData.growthMilestones.
        /// </summary>
        private void ApplyGrowthMilestones(int currentTurn)
        {
            if (data == null || data.growthMilestones == null) return;

            RivalMilestone currentMilestone = null;

            // Find the latest applicable milestone
            foreach (var milestone in data.growthMilestones)
            {
                if (currentTurn >= milestone.turn)
                    currentMilestone = milestone;
            }

            if (currentMilestone == null) return;

            // Enable aggression if milestone says so
            if (currentMilestone.enableAggression)
                aggressionEnabled = true;

            // Catch up businesses if behind target
            while (rivalBusinesses.Count < currentMilestone.targetBusinesses &&
                   rivalBusinesses.Count < data.maxBusinesses)
            {
                string bizName = "Milestone Business";
                if (data.possibleBusinessNames != null && data.possibleBusinessNames.Length > 0)
                {
                    bizName = data.possibleBusinessNames[
                        UnityEngine.Random.Range(0, data.possibleBusinessNames.Length)];
                }

                rivalBusinesses.Add(new RivalBusiness
                {
                    name = bizName,
                    income = data.baseBusinessIncome,
                    customers = data.baseBusinessCustomers,
                    employeeCount = 1,
                    maxEmployees = data.maxEmployeesPerBusiness
                });

                totalRivalEmployees++;
            }

            // Catch up employees if behind target
            int employeeDeficit = currentMilestone.targetEmployees - totalRivalEmployees;
            for (int i = 0; i < employeeDeficit; i++)
            {
                // Add employee to business with fewest
                RivalBusiness target = null;
                int minEmp = int.MaxValue;
                foreach (var biz in rivalBusinesses)
                {
                    if (biz.employeeCount < biz.maxEmployees && biz.employeeCount < minEmp)
                    {
                        minEmp = biz.employeeCount;
                        target = biz;
                    }
                }

                if (target == null) break;

                target.employeeCount++;
                target.income += data.employeeIncomeBoost;
                target.customers += data.employeeCustomerBoost;
                totalRivalEmployees++;
            }
        }

        // ----------------------------------------------------------------
        // Income Collection
        // ----------------------------------------------------------------

        /// <summary>
        /// Collects income from all rival businesses.
        /// Called at the start of each rival turn.
        /// </summary>
        private void CollectIncome()
        {
            int totalIncome = 0;
            foreach (var biz in rivalBusinesses)
            {
                totalIncome += biz.income;
            }
            rivalIncome = totalIncome;
            rivalMoney += totalIncome;
        }

        // ----------------------------------------------------------------
        // Customer & Income Calculation
        // ----------------------------------------------------------------

        /// <summary>
        /// Recalculates the total number of customers across all rival businesses.
        /// Applies combo penalty from player (Monopol: -3/turn).
        /// </summary>
        public void CalculateRivalCustomers()
        {
            int total = 0;
            foreach (var biz in rivalBusinesses)
            {
                total += biz.customers;
            }
            rivalCustomers = Mathf.Max(0, total);
        }

        /// <summary>
        /// Applies a customer penalty to the rival (from player combos like Monopol).
        /// </summary>
        public void ApplyCustomerPenalty(int penalty)
        {
            if (penalty <= 0 || rivalBusinesses.Count == 0) return;

            // Distribute penalty across businesses evenly
            int perBiz = Mathf.Max(1, penalty / rivalBusinesses.Count);
            int remaining = penalty;

            foreach (var biz in rivalBusinesses)
            {
                int loss = Mathf.Min(perBiz, remaining);
                biz.customers = Mathf.Max(0, biz.customers - loss);
                remaining -= loss;
                if (remaining <= 0) break;
            }

            CalculateRivalCustomers();
        }

        /// <summary>
        /// Recalculates total rival income.
        /// </summary>
        public void CalculateRivalIncome()
        {
            int total = 0;
            foreach (var biz in rivalBusinesses)
            {
                total += biz.income;
            }
            rivalIncome = total;
        }

        // ----------------------------------------------------------------
        // Taunts (GDD Section 8)
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns a taunt based on the current mood and game state.
        /// Taunt categories from RivalData:
        /// - growingTaunts: rival is growing
        /// - playerGrowingTaunts: player is growing
        /// - aggressiveTaunts: rival is aggressive
        /// - losingTaunts: rival is losing
        /// - winningTaunts: rival is winning
        /// </summary>
        public string GetTaunt()
        {
            if (data == null) return null;

            int rivalTerritories = GameManager.Instance != null ? GameManager.Instance.RivalTerritories : 0;
            int playerTerritories = GameManager.Instance != null ? GameManager.Instance.PlayerTerritories : 0;

            // Aggressive mood
            if (currentMood == "aggressive" && data.aggressiveTaunts != null && data.aggressiveTaunts.Length > 0)
                return PickRandom(data.aggressiveTaunts);

            // Winning
            if (rivalTerritories > playerTerritories + 2 && data.winningTaunts != null && data.winningTaunts.Length > 0)
                return PickRandom(data.winningTaunts);

            // Losing
            if (playerTerritories > rivalTerritories + 2 && data.losingTaunts != null && data.losingTaunts.Length > 0)
                return PickRandom(data.losingTaunts);

            // Player growing (player gaining territory)
            if (playerTerritories > rivalTerritories && data.playerGrowingTaunts != null && data.playerGrowingTaunts.Length > 0)
                return PickRandom(data.playerGrowingTaunts);

            // Default: rival growing
            if (data.growingTaunts != null && data.growingTaunts.Length > 0)
                return PickRandom(data.growingTaunts);

            return data.tagline;
        }

        // ----------------------------------------------------------------
        // Mood
        // ----------------------------------------------------------------

        /// <summary>
        /// Updates rival mood based on territory comparison.
        /// </summary>
        private void UpdateMood(int playerTerritories)
        {
            int rivalTerritories = GameManager.Instance != null ? GameManager.Instance.RivalTerritories : 0;

            if (playerTerritories > 5)
                currentMood = "aggressive";
            else if (rivalTerritories > playerTerritories + 2)
                currentMood = "confident";
            else if (playerTerritories > rivalTerritories + 2)
                currentMood = "threatened";
            else
                currentMood = "neutral";
        }

        // ----------------------------------------------------------------
        // Helpers
        // ----------------------------------------------------------------

        /// <summary>
        /// Checks if any rival business has room for more employees.
        /// </summary>
        private bool HasEmptyEmployeeSlots()
        {
            foreach (var biz in rivalBusinesses)
            {
                if (biz.employeeCount < biz.maxEmployees)
                    return true;
            }
            return false;
        }

        private string PickRandom(string[] array)
        {
            if (array == null || array.Length == 0) return null;
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Closes the weakest rival business (used by player action cards like Dusmanca Devralma).
        /// </summary>
        public void CloseWeakestBusiness(int turns)
        {
            if (rivalBusinesses.Count <= 1) return; // Keep at least 1

            int weakestIdx = 0;
            int weakestCustomers = int.MaxValue;

            for (int i = 0; i < rivalBusinesses.Count; i++)
            {
                if (rivalBusinesses[i].customers < weakestCustomers)
                {
                    weakestCustomers = rivalBusinesses[i].customers;
                    weakestIdx = i;
                }
            }

            // Remove weakest
            RivalBusiness removed = rivalBusinesses[weakestIdx];
            totalRivalEmployees -= removed.employeeCount;
            rivalBusinesses.RemoveAt(weakestIdx);

            EventBus.RivalActed($"Rakibin en zayif isletmesi kapatildi: {removed.name}");
        }

        /// <summary>
        /// Disables all rival production for one turn (Sabotaj action card).
        /// Returns the income that was lost.
        /// </summary>
        public int DisableProductionOneTurn()
        {
            int lostIncome = 0;
            foreach (var biz in rivalBusinesses)
            {
                lostIncome += biz.income;
            }
            // Production is skipped this turn; rival doesn't collect income
            // The caller should have called this INSTEAD of CollectIncome
            return lostIncome;
        }

        /// <summary>
        /// Resets the rival for a new run.
        /// </summary>
        public void Reset()
        {
            rivalMoney = 0;
            rivalIncome = 0;
            rivalCustomers = 0;
            totalRivalEmployees = 0;
            aggressionEnabled = false;
            currentMood = "neutral";
            rivalBusinesses.Clear();
        }
    }
}
