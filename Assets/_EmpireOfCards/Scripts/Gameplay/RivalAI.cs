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
    /// AI opponent coordinator (GDD Section 8).
    /// Owns RivalData and runtime state; delegates logic to sub-systems:
    ///   - RivalDecisionTree  (action selection)
    ///   - RivalEconomy       (income, customers, penalties)
    ///   - RivalGrowth        (businesses, employees, milestones)
    ///   - RivalDialogue      (taunts, mood)
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
        [SerializeField] private bool aggressionEnabled;

        // --- Sub-systems (created in Initialize) ---
        private RivalDecisionTree decisionTree;
        private RivalEconomy economy;
        private RivalGrowth growth;
        private RivalDialogue dialogue;

        // --- Properties ---
        public int RivalMoney => rivalMoney;
        public int RivalCustomers => rivalCustomers;
        public int RivalIncome => rivalIncome;
        public IReadOnlyList<RivalBusiness> RivalBusinesses => rivalBusinesses;
        public string CurrentMood => dialogue != null ? dialogue.CurrentMood : "neutral";
        public RivalData Data => data;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
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
        /// Creates sub-systems and the first business.
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
            rivalBusinesses.Clear();

            // Create sub-systems
            decisionTree = new RivalDecisionTree(data);
            economy = new RivalEconomy(data);
            growth = new RivalGrowth(data);
            dialogue = new RivalDialogue(data);

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
        /// Initializes the rival so its first business mirrors the player's venture choice.
        /// </summary>
        public void Initialize(VentureType playerVenture)
        {
            Initialize(); // Base init (creates default first business)

            if (data == null || rivalBusinesses == null || rivalBusinesses.Count == 0) return;

            var firstBiz = rivalBusinesses[0];
            switch (playerVenture)
            {
                case VentureType.Bufe:
                    firstBiz.name = "Rival Büfe";
                    firstBiz.income = 50;
                    firstBiz.customers = 3;
                    break;
                case VentureType.TechStartup:
                    firstBiz.name = "Rival Tech Startup";
                    firstBiz.income = 0;
                    firstBiz.customers = 0;
                    break;
                case VentureType.ReklamAjansi:
                    firstBiz.name = "Rival Reklam Ajansı";
                    firstBiz.income = 60;
                    firstBiz.customers = 3;
                    break;
                case VentureType.KaranlikPazar:
                    // Player chose no business, rival keeps default
                    break;
            }

            // Sync totals with updated business stats
            rivalIncome = economy.CalculateRivalIncome(rivalBusinesses);
            rivalCustomers = economy.CalculateRivalCustomers(rivalBusinesses);
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
        /// Main AI turn entry point. Called during the Rival Phase.
        /// </summary>
        public void TakeTurn(int playerTerritories, int rivalTerritories, int currentTurn)
        {
            if (data == null) return;

            // Step 1: Collect income
            rivalIncome = economy.CollectIncome(rivalBusinesses, ref rivalMoney);

            // Step 2: Milestone catch-up
            if (growth.ApplyGrowthMilestones(currentTurn, rivalBusinesses, ref totalRivalEmployees))
                aggressionEnabled = true;

            // Step 3: Update mood
            dialogue.UpdateMood(playerTerritories, rivalTerritories);

            // Step 4: Execute actions
            int actions = data.actionsPerTurn;
            for (int a = 0; a < actions; a++)
            {
                string decision = decisionTree.DecideAction(
                    playerTerritories,
                    rivalTerritories,
                    currentTurn,
                    rivalMoney,
                    rivalBusinesses.Count,
                    aggressionEnabled,
                    growth.HasEmptyEmployeeSlots(rivalBusinesses));

                ExecuteAction(decision);
            }

            // Step 5: Recalculate totals
            rivalCustomers = economy.CalculateRivalCustomers(rivalBusinesses);
            rivalIncome = economy.CalculateRivalIncome(rivalBusinesses);

            // Step 6: Deliver a taunt
            string taunt = dialogue.GetTaunt(playerTerritories, rivalTerritories);
            if (!string.IsNullOrEmpty(taunt))
            {
                EventBus.RivalTaunted(taunt);
            }
        }

        // ----------------------------------------------------------------
        // Action Dispatch
        // ----------------------------------------------------------------

        private void ExecuteAction(string action)
        {
            switch (action)
            {
                case "aggressive":
                    var result = growth.AggressiveAction(rivalBusinesses, ref rivalMoney);
                    // Apply player-side effects (RivalGrowth returns data, we apply it here)
                    var gm = GameManager.Instance;
                    if (gm != null)
                    {
                        if (result.stolenCustomers > 0)
                        {
                            gm.SetPlayerCustomers(
                                Mathf.Max(0, gm.PlayerCustomers - result.stolenCustomers));
                        }
                        if (result.isSabotage)
                        {
                            // Flag a 1-turn production loss on the player's board.
                            // BoardManager will skip BusinessProduce step next resolve.
                            gm.BoardManager.SetProductionDisabledNextTurn(true);
                        }
                    }
                    break;
                case "open_business":
                    growth.OpenBusiness(rivalBusinesses, ref rivalMoney);
                    break;
                case "hire_employee":
                    growth.HireEmployee(rivalBusinesses, ref rivalMoney, ref totalRivalEmployees);
                    break;
                case "event_bonus":
                    growth.EventBonusAction(rivalBusinesses);
                    break;
                default:
                    growth.NormalGrowth(rivalBusinesses, ref rivalMoney);
                    break;
            }
        }

        // ----------------------------------------------------------------
        // Public API (called by other systems)
        // ----------------------------------------------------------------

        public void CalculateRivalCustomers()
        {
            rivalCustomers = economy.CalculateRivalCustomers(rivalBusinesses);
        }

        public void CalculateRivalIncome()
        {
            rivalIncome = economy.CalculateRivalIncome(rivalBusinesses);
        }

        public void ApplyCustomerPenalty(int penalty)
        {
            economy.ApplyCustomerPenalty(rivalBusinesses, penalty);
            rivalCustomers = economy.CalculateRivalCustomers(rivalBusinesses);
        }

        public void CloseWeakestBusiness(int turns)
        {
            growth.CloseWeakestBusiness(rivalBusinesses, ref totalRivalEmployees);
        }

        public int DisableProductionOneTurn()
        {
            return economy.CalculateDisabledProductionLoss(rivalBusinesses);
        }

        public string GetTaunt(int playerTerritories, int rivalTerritories)
        {
            return dialogue.GetTaunt(playerTerritories, rivalTerritories);
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
            rivalBusinesses.Clear();
            dialogue?.Reset();
        }
    }
}
