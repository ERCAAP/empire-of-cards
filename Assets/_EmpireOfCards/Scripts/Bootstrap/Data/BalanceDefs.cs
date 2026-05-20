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
            gb.maxTurns = 30;
            gb.startingActions = 3;
            gb.maxActions = 5;
            gb.startingBusinessSlots = 3;
            gb.maxBusinessSlots = 5;
            gb.handSize = 5;
            gb.redrawsPerTurn = 1;
            gb.shopCardsPerTurn = 3;
            gb.eventInterval = 3;

            // Economy
            gb.taxRate = 0.15f;
            gb.reducedTaxRate = 0.075f;
            gb.minTaxRate = 0.03f;
            gb.sellRate = 0.4f;

            // FBI
            gb.fbiRaidPenalty = 300;
            gb.fbiStartingRisk = 0f;

            // Territory
            gb.totalTerritories = 10;
            gb.winTerritories = 6;
            gb.loseTerritories = 7;

            // Market Pool
            gb.baseMarketCustomers = 60;
            gb.earlyGrowthPerTurn = 5;
            gb.midGrowthPerTurn = 6;
            gb.lateGrowthPerTurn = 8;
            gb.endGrowthPerTurn = 10;

            // Score
            gb.territoryScoreMultiplier = 500;
            gb.moneyScoreMultiplier = 1;
            gb.comboScoreMultiplier = 200;
            gb.businessScoreMultiplier = 100;
            gb.earlyFinishBonusPerTurn = 300;
            gb.fbiEvasionBonus = 50;
            gb.winBonus = 1000;

            // Business Evolution
            gb.evolutionCustomerThreshold = 40;
            gb.evolutionTurnRequirement = 15;

            // Employee
            gb.employeeLeaveTurnThreshold = 8;

            return gb;
        }

        // ----------------------------------------------------------------
        // Starting Deck (14 cards)
        // ----------------------------------------------------------------

        public static DeckPresetData CreateDeck(CardData[] allCards)
        {
            var deck = ScriptableObject.CreateInstance<DeckPresetData>();
            deck.name = "StarterDeck_Runtime";

            deck.presetName = "Starter Deck";
            deck.startingMoney = 500;

            CardData b01 = CardHelper.FindCard("B01_Diner");
            CardData b02 = CardHelper.FindCard("B02_CoffeeShop");
            CardData c01 = CardHelper.FindCard("C01_Intern");
            CardData c02 = CardHelper.FindCard("C02_JuniorMarketer");
            CardData c03 = CardHelper.FindCard("C03_Barista");
            CardData a01 = CardHelper.FindCard("A01_Flyer");
            CardData a02 = CardHelper.FindCard("A02_SmallInvestment");
            CardData u01 = CardHelper.FindCard("U01_OfficeSupplies");

            // 14 cards: diverse starter with combo potential
            // 2x Business (Diner + Coffee Shop = early combo potential with Barista)
            // 3x Employee (1 Intern + 1 Jr Marketer + 1 Barista = variety + Latte Art combo piece)
            // 4x Action (2 Flyer + 2 Small Investment = choices between customers vs money)
            // 2x Upgrade (Office Supplies = immediate value)
            deck.cards = new DeckEntry[]
            {
                new DeckEntry { card = b01, count = 1 },   // 1x Diner (venture gives another)
                new DeckEntry { card = b02, count = 1 },   // 1x Coffee Shop (combo potential!)
                new DeckEntry { card = c01, count = 1 },   // 1x Intern (cheap starter)
                new DeckEntry { card = c02, count = 1 },   // 1x Jr Marketer (income boost)
                new DeckEntry { card = c03, count = 1 },   // 1x Barista (Latte Art combo!)
                new DeckEntry { card = a01, count = 2 },   // 2x Flyer (customer burst)
                new DeckEntry { card = a02, count = 2 },   // 2x Small Investment (money burst)
                new DeckEntry { card = u01, count = 2 },   // 2x Office Supplies (immediate value)
            };

            return deck;
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
                "Tech Store",
                "Supermarket",
                "Cafe Chain",
                "Fitness Center"
            };

            // Venture Mirror (GDD Section 1.7) — indexed by VentureType ordinal
            // [0] Diner, [1] TechStartup, [2] AdAgency, [3] BlackMarket (legacy)
            // [4] FastFood, [5] Cafe, [6] TechApp, [7] ClothingStore, [8] GroceryStore
            rival.ventureMatchedNames = new[]
            {
                "Rival Diner", "Rival Tech Startup", "Rival Ad Agency", "MegaCorp HQ",
                "Rival Fast Food", "Rival Cafe", "Rival Tech App", "Rival Clothing Store", "Rival Grocery Store"
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
            // Rival pacing slowed: territories reduced at each milestone
            // to give players a comeback window (old values reached 6 territories by turn 15).
            rival.growthMilestones = new RivalMilestone[]
            {
                new RivalMilestone
                {
                    turn = 5,
                    targetBusinesses = 2,
                    targetEmployees = 2,
                    targetTerritories = 2,
                    enableAggression = false
                },
                new RivalMilestone
                {
                    turn = 8,
                    targetBusinesses = 2,
                    targetEmployees = 3,
                    targetTerritories = 3,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 12,
                    targetBusinesses = 3,
                    targetEmployees = 5,
                    targetTerritories = 4,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 15,
                    targetBusinesses = 3,
                    targetEmployees = 6,
                    targetTerritories = 5,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 20,
                    targetBusinesses = 4,
                    targetEmployees = 7,
                    targetTerritories = 6,
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

        // ----------------------------------------------------------------
        // Meta Progression (GDD Section 10)
        // ----------------------------------------------------------------

        public static MetaProgressionData CreateMetaProgression()
        {
            var meta = ScriptableObject.CreateInstance<MetaProgressionData>();
            meta.name = "MetaProgression_Runtime";

            // Unlock tiers: XP thresholds that gate content
            meta.unlockTiers = new UnlockTier[]
            {
                new UnlockTier
                {
                    xpRequired = 50,
                    unlockDescription = "Uncommon cards enter the shop pool"
                },
                new UnlockTier
                {
                    xpRequired = 200,
                    unlockDescription = "Rare cards enter the shop pool"
                },
                new UnlockTier
                {
                    xpRequired = 500,
                    unlockDescription = "Shadow Inc. rival unlocked"
                },
                new UnlockTier
                {
                    xpRequired = 1000,
                    unlockDescription = "Epic cards enter the shop pool"
                },
                new UnlockTier
                {
                    xpRequired = 2000,
                    unlockDescription = "The Cartel rival unlocked"
                },
                new UnlockTier
                {
                    xpRequired = 5000,
                    unlockDescription = "Legendary cards enter the shop pool"
                }
            };

            // Ascension levels: harder modifiers for repeat players
            meta.ascensionLevels = new AscensionLevel[]
            {
                new AscensionLevel
                {
                    level = 1,
                    description = "Rival more aggressive",
                    rivalAggressionMultiplier = 1.2f,
                    startingMoneyModifier = 0,
                    crisisFrequencyMultiplier = 1f
                },
                new AscensionLevel
                {
                    level = 2,
                    description = "Less starting money",
                    rivalAggressionMultiplier = 1.5f,
                    startingMoneyModifier = -100,
                    crisisFrequencyMultiplier = 1f
                },
                new AscensionLevel
                {
                    level = 3,
                    description = "Crises strike more often",
                    rivalAggressionMultiplier = 1.5f,
                    startingMoneyModifier = -100,
                    crisisFrequencyMultiplier = 1.5f
                }
            };

            return meta;
        }

        // ----------------------------------------------------------------
        // Shop Pool (all non-starter, non-event cards)
        // ----------------------------------------------------------------

        public static CardData[] CreateShopPool(CardData[] allCards, DeckPresetData deck)
        {
            var starterIds = new HashSet<string>();
            for (int i = 0; i < deck.cards.Length; i++)
                starterIds.Add(deck.cards[i].card.cardId);

            var pool = new List<CardData>();
            for (int i = 0; i < allCards.Length; i++)
            {
                var card = allCards[i];
                if (card.cardType == CardType.Event) continue;
                if (starterIds.Contains(card.cardId)) continue;
                pool.Add(card);
            }

            return pool.ToArray();
        }
    }
}
