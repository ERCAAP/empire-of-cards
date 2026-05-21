using UnityEngine;
using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Game balance, starting deck, rival data, and shop pool definitions.
    /// </summary>
    public static class BalanceDefs
    {
        // ----------------------------------------------------------------
        // Game Balance
        // ----------------------------------------------------------------

        public static GameBalanceData CreateBalance()
        {
            var gb = ScriptableObject.CreateInstance<GameBalanceData>();
            gb.name = "GameBalance_Runtime";

            // General
            gb.startingMoney = 500;
            gb.maxTurns = Constants.MAX_TURNS;            // 25
            gb.startingActions = Constants.STARTING_ACTIONS;
            gb.maxActions = Constants.MAX_ACTIONS;
            gb.startingBusinessSlots = Constants.STARTING_OPERATION_SLOTS; // 4
            gb.maxBusinessSlots = Constants.MAX_OPERATION_SLOTS;           // 8
            gb.handSize = Constants.HAND_SIZE;
            gb.redrawsPerTurn = Constants.REDRAWS_PER_TURN;
            gb.shopCardsPerTurn = Constants.SHOP_CARDS_PER_TURN;
            gb.eventInterval = Constants.EVENT_INTERVAL;

            // Economy
            gb.taxRate = Constants.TAX_RATE;
            gb.reducedTaxRate = Constants.REDUCED_TAX_RATE;
            gb.minTaxRate = Constants.MIN_TAX_RATE;
            gb.sellRate = Constants.SELL_RATE;

            // FBI
            gb.fbiRaidPenalty = Constants.FBI_RAID_PENALTY;
            gb.fbiStartingRisk = 0f;

            // Customer Market
            gb.totalMarketCustomers = Constants.TOTAL_MARKET_CUSTOMERS; // 100
            gb.winCustomerShare = Constants.WIN_CUSTOMER_SHARE;         // 60
            gb.loseCustomerShare = Constants.WIN_CUSTOMER_SHARE;        // 60

            // Market Pool
            gb.baseMarketCustomers = Constants.BASE_MARKET_CUSTOMERS;
            gb.earlyGrowthPerTurn = Constants.EARLY_GROWTH_PER_TURN;
            gb.midGrowthPerTurn = Constants.MID_GROWTH_PER_TURN;
            gb.lateGrowthPerTurn = Constants.LATE_GROWTH_PER_TURN;
            gb.endGrowthPerTurn = Constants.END_GROWTH_PER_TURN;

            // Score
            gb.customerShareScoreMultiplier = Constants.SCORE_CUSTOMER_SHARE;
            gb.moneyScoreMultiplier = Constants.SCORE_MONEY;
            gb.comboScoreMultiplier = Constants.SCORE_COMBO;
            gb.businessScoreMultiplier = Constants.SCORE_BUSINESS;
            gb.earlyFinishBonusPerTurn = Constants.SCORE_EARLY_FINISH;
            gb.fbiEvasionBonus = Constants.SCORE_FBI_EVASION;
            gb.winBonus = Constants.SCORE_WIN_BONUS;

            // Business Evolution
            gb.evolutionCustomerThreshold = Constants.EVOLUTION_CUSTOMER_THRESHOLD;
            gb.evolutionTurnRequirement = Constants.EVOLUTION_TURN_REQUIREMENT;

            // Employee
            gb.employeeLeaveTurnThreshold = Constants.EMPLOYEE_LEAVE_TURN_THRESHOLD;

            return gb;
        }

        // ----------------------------------------------------------------
        // MegaCorp Rival
        // ----------------------------------------------------------------

        public static RivalData CreateRival()
        {
            var rival = ScriptableObject.CreateInstance<RivalData>();
            rival.name = "MegaCorp_Runtime";

            rival.rivalId = "RIVAL_MegaCorp";
            rival.rivalName = "MegaCorp";
            rival.personality = RivalPersonality.Balanced;
            rival.tagline = "This industry isn't big enough for both of us.";

            // Starting Stats
            rival.startingMoney = 400;
            rival.startingIncome = 80;
            rival.startingCustomers = 5;
            rival.startingBusinessName = "MegaCorp HQ";

            // Behavior
            rival.actionsPerTurn = 2;
            rival.aggressionThreshold = 0.5f;
            rival.maxBusinesses = 4;
            rival.maxEmployeesPerBusiness = 3;

            // Growth Parameters
            rival.businessCostThreshold = 200;
            rival.hireCostThreshold = 80;
            rival.baseBusinessIncome = 80;
            rival.baseBusinessCustomers = 5;
            rival.employeeIncomeBoost = 30;
            rival.employeeCustomerBoost = 3;
            rival.aggressiveCustomerBoost = 8;
            rival.aggressiveIncomeBoost = 50;
            rival.passiveCustomerGrowth = 2;
            rival.passiveIncomeGrowth = 10;
            rival.passiveMoneyGrowth = 50;
            rival.eventBonusCustomers = 3;
            rival.eventBonusIncome = 15;

            rival.possibleBusinessNames = new[]
            {
                "Corner Grill",
                "Cafe Chain",
                "Neighborhood Market",
                "Late Night Counter"
            };

            // Venture Mirror (GDD Section 1.7) — indexed by VentureType ordinal
            // [0] Diner, [1] TechStartup, [2] AdAgency, [3] BlackMarket (legacy)
            // [4] FastFood, [5] Cafe, [6] TechApp, [7] ClothingStore, [8] GroceryStore
            rival.ventureMatchedNames = new[]
            {
                "Rival Diner", "Rival Tech Startup", "Rival Ad Agency", "MegaCorp HQ",
                "Rival Restaurant", "Rival Cafe", "Rival Tech App", "Rival Clothing Store", "Rival Market"
            };
            rival.ventureMatchedIncome = new[]
            {
                50, 0, 60, 80,
                45, 55, 0, 50, 40
            };
            rival.ventureMatchedCustomers = new[]
            {
                3, 0, 3, 5,
                4, 3, 0, 3, 5
            };

            // Growth Schedule (GDD Section 8.3)
            // Rival pacing slowed: market blocks reduced at each milestone
            // to give players a comeback window.
            rival.growthMilestones = new RivalMilestone[]
            {
                new RivalMilestone
                {
                    turn = 5,
                    targetBusinesses = 2,
                    targetEmployees = 2,
                    targetMarketBlocks = 2,
                    enableAggression = false
                },
                new RivalMilestone
                {
                    turn = 8,
                    targetBusinesses = 2,
                    targetEmployees = 3,
                    targetMarketBlocks = 3,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 12,
                    targetBusinesses = 3,
                    targetEmployees = 5,
                    targetMarketBlocks = 4,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 15,
                    targetBusinesses = 3,
                    targetEmployees = 6,
                    targetMarketBlocks = 5,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 20,
                    targetBusinesses = 4,
                    targetEmployees = 7,
                    targetMarketBlocks = 6,
                    enableAggression = true
                },
            };

            // Dialogue
            rival.growingTaunts = new[]
            {
                "Our market share is growing.",
                "This is just the beginning."
            };
            rival.playerGrowingTaunts = new[]
            {
                "Interesting move...",
                "Didn't see that coming."
            };
            rival.aggressiveTaunts = new[]
            {
                "This industry isn't big enough for both of us.",
                "Competition is heating up."
            };
            rival.losingTaunts = new[]
            {
                "This isn't over.",
                "We'll be back."
            };
            rival.winningTaunts = new[]
            {
                "It was inevitable.",
                "The market is ours."
            };

            return rival;
        }

    }
}
