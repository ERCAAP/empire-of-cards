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
        public VentureType ventureType;
        public float qualityScore = 5f;
        public float platformRating = 2.5f;
        public int legalRisk;
        public float priceScore = 5f;
    }

    /// <summary>
    /// AI opponent coordinator (GDD Section 8).
    /// Owns RivalData and runtime state; delegates logic to sub-systems:
    ///   - RivalDecisionTree  (action selection)
    ///   - RivalEconomy       (income, customers, penalties)
    ///   - RivalGrowth        (businesses, employees, milestones)
    ///   - RivalDialogue      (taunts, mood, strategy reactions)
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

        // --- Cost Sabotage (Lobbyist ability) ---
        [SerializeField] private float nextPurchaseCostMultiplier = 0f;

        // --- Properties ---
        public int RivalMoney => rivalMoney;
        public int RivalCustomers => rivalCustomers;
        public int RivalIncome => rivalIncome;
        public IReadOnlyList<RivalBusiness> RivalBusinesses => rivalBusinesses;
        public string CurrentMood => dialogue != null ? dialogue.CurrentMood : "neutral";
        public string MoodIcon => dialogue != null ? dialogue.MoodIcon : "";
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
            firstBiz.ventureType = playerVenture;

            switch (playerVenture)
            {
                // --- New ventures ---
                case VentureType.FastFood:
                    firstBiz.name = "Rival Fast Food";
                    firstBiz.income = 45;
                    firstBiz.customers = 4;
                    firstBiz.qualityScore = 4f;
                    firstBiz.priceScore = 7f;
                    firstBiz.platformRating = 2.5f;
                    break;
                case VentureType.Cafe:
                    firstBiz.name = "Rival Cafe";
                    firstBiz.income = 55;
                    firstBiz.customers = 3;
                    firstBiz.qualityScore = 6f;
                    firstBiz.priceScore = 5f;
                    firstBiz.platformRating = 3.0f;
                    break;
                case VentureType.TechApp:
                    firstBiz.name = "Rival Tech App";
                    firstBiz.income = 0;
                    firstBiz.customers = 0;
                    firstBiz.qualityScore = 3f;
                    firstBiz.priceScore = 5f;
                    firstBiz.platformRating = 2.0f;
                    break;
                case VentureType.ClothingStore:
                    firstBiz.name = "Rival Clothing Store";
                    firstBiz.income = 50;
                    firstBiz.customers = 3;
                    firstBiz.qualityScore = 5f;
                    firstBiz.priceScore = 6f;
                    firstBiz.platformRating = 2.5f;
                    break;
                case VentureType.GroceryStore:
                    firstBiz.name = "Rival Grocery Store";
                    firstBiz.income = 40;
                    firstBiz.customers = 5;
                    firstBiz.qualityScore = 5f;
                    firstBiz.priceScore = 8f;
                    firstBiz.platformRating = 2.5f;
                    break;

                // --- Legacy ventures (save compatibility) ---
                case VentureType.Diner:
                    firstBiz.name = "Rival Diner";
                    firstBiz.income = 50;
                    firstBiz.customers = 3;
                    firstBiz.qualityScore = 5f;
                    firstBiz.priceScore = 6f;
                    firstBiz.platformRating = 2.5f;
                    break;
                case VentureType.TechStartup:
                    firstBiz.name = "Rival Tech Startup";
                    firstBiz.income = 0;
                    firstBiz.customers = 0;
                    firstBiz.qualityScore = 3f;
                    firstBiz.priceScore = 5f;
                    firstBiz.platformRating = 2.0f;
                    break;
                case VentureType.AdAgency:
                    firstBiz.name = "Rival Ad Agency";
                    firstBiz.income = 60;
                    firstBiz.customers = 3;
                    firstBiz.qualityScore = 6f;
                    firstBiz.priceScore = 5f;
                    firstBiz.platformRating = 3.0f;
                    break;
                case VentureType.BlackMarket:
                    // Player chose no business, rival keeps default
                    firstBiz.qualityScore = 5f;
                    firstBiz.priceScore = 5f;
                    firstBiz.platformRating = 2.5f;
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

            // Step 3: Determine action count from Company Tier (GDD Section 1.7)
            int actions = GetActionsForCurrentTier();

            // Step 4: Pre-determine first action to inform mood tell
            RivalMove previewMove = decisionTree.DecideMove(
                playerTerritories,
                rivalTerritories,
                currentTurn,
                rivalMoney,
                rivalBusinesses,
                aggressionEnabled);

            // Step 5: Mood tell -- determine and broadcast mood BEFORE actions
            dialogue.DetermineMood(
                playerTerritories,
                rivalTerritories,
                rivalMoney,
                data.businessCostThreshold,
                previewMove.ToString());

            // Step 6: Check player strategy and react (fires once per shift)
            dialogue.CheckPlayerStrategy();

            // Step 7: Execute actions
            for (int a = 0; a < actions; a++)
            {
                RivalMove move;
                if (a == 0)
                {
                    move = previewMove;
                }
                else
                {
                    move = decisionTree.DecideMove(
                        playerTerritories,
                        rivalTerritories,
                        currentTurn,
                        rivalMoney,
                        rivalBusinesses,
                        aggressionEnabled);
                }

                ExecuteRivalMove(move);
            }

            // Step 8: Recalculate totals
            rivalCustomers = economy.CalculateRivalCustomers(rivalBusinesses);
            rivalIncome = economy.CalculateRivalIncome(rivalBusinesses);

            // Step 9: Deliver a context-aware taunt
            string taunt = dialogue.GetTaunt(playerTerritories, rivalTerritories);
            if (!string.IsNullOrEmpty(taunt))
            {
                EventBus.RivalTaunted(taunt);
            }
        }

        private int GetActionsForCurrentTier()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.CompanyTierSystem == null) return data.actionsPerTurn;

            return gm.CompanyTierSystem.CurrentTier switch
            {
                CompanyTier.Trader => 2,        // Tier 1
                CompanyTier.Entrepreneur => 3,   // Tier 2
                CompanyTier.Corporation => 4,    // Tier 3
                CompanyTier.Conglomerate => 4,   // Tier 3+ (cap at 4)
                _ => data.actionsPerTurn
            };
        }

        // ----------------------------------------------------------------
        // Action Dispatch
        // ----------------------------------------------------------------

        private void ExecuteRivalMove(RivalMove move)
        {
            var gm = GameManager.Instance;

            switch (move)
            {
                case RivalMove.PriceWar:
                    foreach (var biz in rivalBusinesses)
                        biz.priceScore = Mathf.Min(10f, biz.priceScore + 2f);
                    EventBus.RivalActed("Rival launched a price war!");
                    break;

                case RivalMove.MarketingBlitz:
                    foreach (var biz in rivalBusinesses)
                        biz.platformRating = Mathf.Min(5f, biz.platformRating + 0.4f);
                    EventBus.RivalActed("Rival ran a marketing blitz!");
                    break;

                case RivalMove.QualityImprove:
                    foreach (var biz in rivalBusinesses)
                        biz.qualityScore = Mathf.Min(10f, biz.qualityScore + 1.5f);
                    EventBus.RivalActed("Rival improved product quality.");
                    break;

                case RivalMove.StaffPoach:
                    if (gm != null && gm.BoardManager != null)
                    {
                        var playerBusinesses = gm.BoardManager.PlayerBusinesses;
                        if (playerBusinesses != null)
                        {
                            int bestBonus = 0;
                            foreach (var pb in playerBusinesses)
                            {
                                if (pb.isClosed) continue;
                                foreach (var emp in pb.employees)
                                {
                                    if (emp != null && emp.customerBonus > bestBonus)
                                        bestBonus = emp.customerBonus;
                                }
                            }
                            if (bestBonus > 0 && rivalBusinesses.Count > 0)
                            {
                                rivalBusinesses[0].customers += bestBonus;
                                EventBus.RivalActed($"Rival poached your staff! Stole {bestBonus} customer bonus.");
                            }
                        }
                    }
                    break;

                case RivalMove.SeekInvestment:
                    int investmentGain = Mathf.RoundToInt(rivalIncome * 0.3f);
                    rivalMoney += investmentGain;
                    EventBus.RivalActed($"Rival secured investment: +{investmentGain} money.");
                    break;

                case RivalMove.OpenBranch:
                    if (rivalBusinesses.Count > 0)
                    {
                        var target = rivalBusinesses[0];
                        foreach (var biz in rivalBusinesses)
                        {
                            if (biz.income > target.income) target = biz;
                        }
                        if (target.maxEmployees < 6)
                        {
                            target.maxEmployees++;
                            // Apply Lobbyist cost-increase sabotage if active
                            if (nextPurchaseCostMultiplier > 0f)
                            {
                                int baseCost = data.businessCostThreshold;
                                int surcharge = Mathf.RoundToInt(baseCost * nextPurchaseCostMultiplier);
                                rivalMoney -= surcharge;
                                nextPurchaseCostMultiplier = 0f;
                            }
                            EventBus.RivalActed($"Rival opened a new branch for {target.name}.");
                        }
                    }
                    break;

                case RivalMove.Sabotage:
                    if (gm != null && gm.BoardManager != null)
                    {
                        gm.BoardManager.SetProductionDisabledNextTurn(true);
                        foreach (var biz in rivalBusinesses)
                            biz.legalRisk = Mathf.Min(100, biz.legalRisk + 15);
                        EventBus.RivalActed("Rival sabotaged your operations! Production disabled next turn.");
                    }
                    break;
            }
        }

        // Legacy action dispatch -- kept for backward compatibility with old save/replay data
        private void ExecuteAction(string action)
        {
            switch (action)
            {
                case "aggressive":
                    var result = growth.AggressiveAction(rivalBusinesses, ref rivalMoney);
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
                            gm.BoardManager.SetProductionDisabledNextTurn(true);
                        }
                    }
                    break;
                case "open_business":
                    if (nextPurchaseCostMultiplier > 0f)
                    {
                        int baseCost = data.businessCostThreshold;
                        int surcharge = Mathf.RoundToInt(baseCost * nextPurchaseCostMultiplier);
                        rivalMoney -= surcharge;
                        nextPurchaseCostMultiplier = 0f;
                    }
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

        /// <summary>
        /// Marks the rival's next business purchase as more expensive.
        /// Called by AbilitySystem when the Lobbyist "Red Tape" ability fires.
        /// The multiplier stacks additively (e.g., 0.25 = +25% cost).
        /// </summary>
        public void ApplyNextPurchaseCostIncrease(float multiplier)
        {
            nextPurchaseCostMultiplier += multiplier;
            Debug.Log($"[RivalAI] Next business purchase cost increased by +{multiplier * 100}% (total +{nextPurchaseCostMultiplier * 100}%)");
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
        /// Called by ComboSystem (via EventBus) when the player triggers a combo.
        /// Delivers a one-time combo reaction taunt from the rival.
        /// </summary>
        public void OnPlayerComboTriggered()
        {
            if (dialogue == null) return;
            string reaction = dialogue.GetComboReactionTaunt();
            if (!string.IsNullOrEmpty(reaction))
            {
                EventBus.RivalTaunted(reaction);
            }
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
            nextPurchaseCostMultiplier = 0f;
            rivalBusinesses.Clear();
            dialogue?.Reset();
        }
    }
}
