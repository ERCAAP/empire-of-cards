#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Editor
{
    /// <summary>
    /// Generates ALL ScriptableObject assets for the v2 Venture & Slot system.
    /// Menu: EmpireOfCards > Generate All Assets
    /// 5 ventures x 15 cards + 20 general = 95 card target.
    /// </summary>
    public static class AssetGenerator
    {
        // -------------------------------------------------------------------
        // Root paths
        // -------------------------------------------------------------------
        private const string DataRoot   = "Assets/_EmpireOfCards/Data";
        private const string CardsRoot  = "Assets/_EmpireOfCards/Data/Cards";
        private const string DecksRoot  = "Assets/_EmpireOfCards/Data/Decks";
        private const string RivalsRoot = "Assets/_EmpireOfCards/Data/Rivals";
        private const string CombosRoot = "Assets/_EmpireOfCards/Data/Combos";

        // Per-venture card folders
        private const string FastFoodCards    = "Assets/_EmpireOfCards/Data/Cards/FastFood";
        private const string CafeCards        = "Assets/_EmpireOfCards/Data/Cards/Cafe";
        private const string TechAppCards     = "Assets/_EmpireOfCards/Data/Cards/TechApp";
        private const string ClothingCards    = "Assets/_EmpireOfCards/Data/Cards/ClothingStore";
        private const string GroceryCards     = "Assets/_EmpireOfCards/Data/Cards/GroceryStore";
        private const string GeneralCards     = "Assets/_EmpireOfCards/Data/Cards/General";

        // Legacy folders kept for backward compat
        private const string BusinessCards = "Assets/_EmpireOfCards/Data/Cards/Business";
        private const string EmployeeCards = "Assets/_EmpireOfCards/Data/Cards/Employee";
        private const string ActionCards   = "Assets/_EmpireOfCards/Data/Cards/Action";
        private const string UpgradeCards  = "Assets/_EmpireOfCards/Data/Cards/Upgrade";
        private const string EventCards    = "Assets/_EmpireOfCards/Data/Cards/Event";

        // -------------------------------------------------------------------
        // MENU ITEM
        // -------------------------------------------------------------------
        [MenuItem("EmpireOfCards/Generate All Assets")]
        public static void GenerateAll()
        {
            EnsureFolders();

            GenerateGameBalance();
            var cards = GenerateAllCards();
            GenerateStartingDeck(cards);
            GenerateMegaCorpRival();
            GenerateCombos(cards);
            GenerateMetaProgression();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[AssetGenerator] All assets generated successfully!");
        }

        // -------------------------------------------------------------------
        // FOLDER CREATION
        // -------------------------------------------------------------------
        private static void EnsureFolders()
        {
            EnsureFolder("Assets/_EmpireOfCards", "Data");
            EnsureFolder(DataRoot, "Cards");
            EnsureFolder(CardsRoot, "FastFood");
            EnsureFolder(CardsRoot, "Cafe");
            EnsureFolder(CardsRoot, "TechApp");
            EnsureFolder(CardsRoot, "ClothingStore");
            EnsureFolder(CardsRoot, "GroceryStore");
            EnsureFolder(CardsRoot, "General");
            // Legacy folders
            EnsureFolder(CardsRoot, "Business");
            EnsureFolder(CardsRoot, "Employee");
            EnsureFolder(CardsRoot, "Action");
            EnsureFolder(CardsRoot, "Upgrade");
            EnsureFolder(CardsRoot, "Event");
            EnsureFolder(DataRoot, "Decks");
            EnsureFolder(DataRoot, "Rivals");
            EnsureFolder(DataRoot, "Combos");
        }

        private static void EnsureFolder(string parent, string folderName)
        {
            string full = parent + "/" + folderName;
            if (!AssetDatabase.IsValidFolder(full))
            {
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        // -------------------------------------------------------------------
        // HELPER: create or overwrite an asset
        // -------------------------------------------------------------------
        private static T CreateOrReplace<T>(string path) where T : ScriptableObject
        {
            T existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                return existing;
            }
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static void MarkDirty(Object obj)
        {
            EditorUtility.SetDirty(obj);
        }

        private static string GetVentureFolder(VentureType venture)
        {
            switch (venture)
            {
                case VentureType.FastFood:      return FastFoodCards;
                case VentureType.Cafe:           return CafeCards;
                case VentureType.TechApp:        return TechAppCards;
                case VentureType.ClothingStore:  return ClothingCards;
                case VentureType.GroceryStore:   return GroceryCards;
                default:                         return GeneralCards;
            }
        }

        // ===================================================================
        // CARD CREATION HELPERS (v2 Venture & Slot system)
        // ===================================================================

        private static CardData CreateOperation(string id, string name, VentureType venture,
            int buyCost, int income, int customers, float quality, float price, float speed)
        {
            string folder = GetVentureFolder(venture);
            string path = folder + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Business;
            c.rarity = Rarity.Common;
            c.buyCost = buyCost;
            c.incomePerTurn = income;
            c.customersPerTurn = customers;
            c.tags = new CardTag[0];

            // v2 fields
            c.ventureType = venture;
            c.targetSlotType = SlotType.Operation;
            c.isGeneralCard = false;
            c.qualityScore = quality;
            c.priceScore = price;
            c.serviceSpeedScore = speed;

            // Reset legacy fields
            c.hasTrendBonus = false;
            c.trendIncomeMultiplier = 1f;
            c.activationDelay = 0;
            c.requiresTrendToOperate = false;
            c.hasRandomIncome = false;
            c.randomIncomeMin = 0;
            c.randomIncomeMax = 0;
            c.foodBonusTag = "";
            c.foodBonusAmount = 0;
            c.globalCustomerBonus = 0;
            c.canEvolve = false;
            c.employeeSlots = 0;

            // Reset v2 non-operation fields
            c.platformRatingGain = 0f;
            c.platformRatingOnPlay = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.costReductionPercent = 0f;
            c.qualityBoostAmount = 0f;

            MarkDirty(c);
            return c;
        }

        private static CardData CreateStaff(string id, string name, VentureType venture,
            int buyCost, int salary, int customerBonus, int synergyBonus)
        {
            string folder = GetVentureFolder(venture);
            string path = folder + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Employee;
            c.rarity = Rarity.Common;
            c.buyCost = buyCost;
            c.salaryPerTurn = salary;
            c.customerBonus = customerBonus;
            c.synergyCustomerBonus = synergyBonus;
            c.tags = new CardTag[0];

            // v2 fields
            c.ventureType = venture;
            c.targetSlotType = SlotType.Staff;
            c.isGeneralCard = false;

            // Reset other fields
            c.synergyTag = CardTag.Food;
            c.incomeMultiplier = 0f;
            c.incomeFlatBonus = 0f;
            c.incomeBonusTag = CardTag.Food;
            c.fbiRiskPerTurn = 0;
            c.illegalIncomePerTurn = 0;
            c.preventsTransfer = false;
            c.taxReduction = 0f;
            c.activeAbilityType = ActiveAbilityType.None;
            c.activeAbilityName = "";
            c.activeAbilityDesc = "";
            c.abilityValue1 = 0f;
            c.abilityValue2 = 0;
            c.qualityScore = 0f;
            c.priceScore = 0f;
            c.serviceSpeedScore = 0f;
            c.platformRatingGain = 0f;
            c.platformRatingOnPlay = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.costReductionPercent = 0f;
            c.qualityBoostAmount = 0f;

            MarkDirty(c);
            return c;
        }

        private static CardData CreateMarketing(string id, string name, VentureType venture,
            int buyCost, float ratingGain, float ratingOnPlay)
        {
            string folder = GetVentureFolder(venture);
            string path = folder + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Action;
            c.rarity = Rarity.Common;
            c.buyCost = buyCost;
            c.tags = new[] { CardTag.Marketing };

            // v2 fields
            c.ventureType = venture;
            c.targetSlotType = SlotType.Marketing;
            c.isGeneralCard = false;
            c.platformRatingGain = ratingGain;
            c.platformRatingOnPlay = ratingOnPlay;

            // Reset unrelated
            c.qualityScore = 0f;
            c.priceScore = 0f;
            c.serviceSpeedScore = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.costReductionPercent = 0f;
            c.qualityBoostAmount = 0f;
            c.actionEffectType = ActionEffectType.None;
            c.actionValue = 0;
            c.actionMultiplier = 0f;
            c.actionFBIRisk = 0;
            c.actionDebtDuration = 0;
            c.actionDebtPercent = 0f;
            c.actionIncomeSacrifice = 0f;

            MarkDirty(c);
            return c;
        }

        private static CardData CreateSupplier(string id, string name, VentureType venture,
            int buyCost, float costReduction, float qualityBoost)
        {
            string folder = GetVentureFolder(venture);
            string path = folder + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Upgrade;
            c.rarity = Rarity.Common;
            c.buyCost = buyCost;
            c.tags = new[] { CardTag.Support };

            // v2 fields
            c.ventureType = venture;
            c.targetSlotType = SlotType.Supplier;
            c.isGeneralCard = false;
            c.costReductionPercent = costReduction;
            c.qualityBoostAmount = qualityBoost;

            // Reset unrelated
            c.qualityScore = 0f;
            c.priceScore = 0f;
            c.serviceSpeedScore = 0f;
            c.platformRatingGain = 0f;
            c.platformRatingOnPlay = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.upgradeEffectType = UpgradeEffectType.None;
            c.upgradeValue = 0f;
            c.isGlobalUpgrade = false;
            c.closedEmployeeSlots = 0;
            c.extraActions = 0;

            MarkDirty(c);
            return c;
        }

        private static CardData CreateEvent(string id, string name, VentureType venture,
            EventEffectType effectType, int duration, float multiplier)
        {
            string folder = GetVentureFolder(venture);
            string path = folder + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Event;
            c.rarity = Rarity.Common;
            c.buyCost = 0;
            c.tags = new CardTag[0];

            c.eventEffectType = effectType;
            c.eventDuration = duration;
            c.eventMultiplier = multiplier;
            c.affectedTags = new CardTag[0];
            c.eventCustomerPenalty = 0;
            c.eventFBIThreshold = 0f;

            // v2 fields
            c.ventureType = venture;
            c.targetSlotType = SlotType.TempEffect;
            c.isGeneralCard = false;

            // Reset unrelated
            c.qualityScore = 0f;
            c.priceScore = 0f;
            c.serviceSpeedScore = 0f;
            c.platformRatingGain = 0f;
            c.platformRatingOnPlay = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.costReductionPercent = 0f;
            c.qualityBoostAmount = 0f;

            MarkDirty(c);
            return c;
        }

        // General card variant (isGeneralCard = true, stored in General folder)
        private static CardData CreateGeneralOperation(string id, string name,
            int buyCost, int income, int customers, float quality, float price, float speed)
        {
            string path = GeneralCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Business;
            c.rarity = Rarity.Common;
            c.buyCost = buyCost;
            c.incomePerTurn = income;
            c.customersPerTurn = customers;
            c.tags = new CardTag[0];

            c.ventureType = VentureType.FastFood; // default, unused for general
            c.targetSlotType = SlotType.Operation;
            c.isGeneralCard = true;
            c.qualityScore = quality;
            c.priceScore = price;
            c.serviceSpeedScore = speed;

            c.hasTrendBonus = false;
            c.trendIncomeMultiplier = 1f;
            c.activationDelay = 0;
            c.requiresTrendToOperate = false;
            c.hasRandomIncome = false;
            c.randomIncomeMin = 0;
            c.randomIncomeMax = 0;
            c.foodBonusTag = "";
            c.foodBonusAmount = 0;
            c.globalCustomerBonus = 0;
            c.canEvolve = false;
            c.employeeSlots = 0;
            c.platformRatingGain = 0f;
            c.platformRatingOnPlay = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.costReductionPercent = 0f;
            c.qualityBoostAmount = 0f;

            MarkDirty(c);
            return c;
        }

        private static CardData CreateGeneralStaff(string id, string name,
            int buyCost, int salary, int customerBonus)
        {
            string path = GeneralCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Employee;
            c.rarity = Rarity.Common;
            c.buyCost = buyCost;
            c.salaryPerTurn = salary;
            c.customerBonus = customerBonus;
            c.synergyCustomerBonus = 0;
            c.tags = new CardTag[0];

            c.ventureType = VentureType.FastFood;
            c.targetSlotType = SlotType.Staff;
            c.isGeneralCard = true;

            c.synergyTag = CardTag.Food;
            c.incomeMultiplier = 0f;
            c.incomeFlatBonus = 0f;
            c.incomeBonusTag = CardTag.Food;
            c.fbiRiskPerTurn = 0;
            c.illegalIncomePerTurn = 0;
            c.preventsTransfer = false;
            c.taxReduction = 0f;
            c.activeAbilityType = ActiveAbilityType.None;
            c.activeAbilityName = "";
            c.activeAbilityDesc = "";
            c.abilityValue1 = 0f;
            c.abilityValue2 = 0;
            c.qualityScore = 0f;
            c.priceScore = 0f;
            c.serviceSpeedScore = 0f;
            c.platformRatingGain = 0f;
            c.platformRatingOnPlay = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.costReductionPercent = 0f;
            c.qualityBoostAmount = 0f;

            MarkDirty(c);
            return c;
        }

        private static CardData CreateGeneralMarketing(string id, string name,
            int buyCost, float ratingGain, float ratingOnPlay)
        {
            string path = GeneralCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Action;
            c.rarity = Rarity.Common;
            c.buyCost = buyCost;
            c.tags = new[] { CardTag.Marketing };

            c.ventureType = VentureType.FastFood;
            c.targetSlotType = SlotType.Marketing;
            c.isGeneralCard = true;
            c.platformRatingGain = ratingGain;
            c.platformRatingOnPlay = ratingOnPlay;

            c.qualityScore = 0f;
            c.priceScore = 0f;
            c.serviceSpeedScore = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.costReductionPercent = 0f;
            c.qualityBoostAmount = 0f;
            c.actionEffectType = ActionEffectType.None;
            c.actionValue = 0;
            c.actionMultiplier = 0f;
            c.actionFBIRisk = 0;
            c.actionDebtDuration = 0;
            c.actionDebtPercent = 0f;
            c.actionIncomeSacrifice = 0f;

            MarkDirty(c);
            return c;
        }

        private static CardData CreateGeneralEvent(string id, string name,
            EventEffectType effectType, int duration, float multiplier)
        {
            string path = GeneralCards + "/" + id + ".asset";
            var c = CreateOrReplace<CardData>(path);

            c.cardId = id;
            c.cardName = name;
            c.cardType = CardType.Event;
            c.rarity = Rarity.Uncommon;
            c.buyCost = 0;
            c.tags = new CardTag[0];

            c.eventEffectType = effectType;
            c.eventDuration = duration;
            c.eventMultiplier = multiplier;
            c.affectedTags = new CardTag[0];
            c.eventCustomerPenalty = 0;
            c.eventFBIThreshold = 0f;

            c.ventureType = VentureType.FastFood;
            c.targetSlotType = SlotType.TempEffect;
            c.isGeneralCard = true;

            c.qualityScore = 0f;
            c.priceScore = 0f;
            c.serviceSpeedScore = 0f;
            c.platformRatingGain = 0f;
            c.platformRatingOnPlay = 0f;
            c.legalRiskPerTurn = 0;
            c.legalRiskOnPlay = 0;
            c.costReductionPercent = 0f;
            c.qualityBoostAmount = 0f;

            MarkDirty(c);
            return c;
        }

        // ===================================================================
        // 1. GAME BALANCE
        // ===================================================================
        private static void GenerateGameBalance()
        {
            string path = DataRoot + "/GameBalance.asset";
            var gb = CreateOrReplace<GameBalanceData>(path);

            // General
            gb.startingMoney          = 500;
            gb.maxTurns               = 25;
            gb.startingActions        = 3;
            gb.maxActions             = 5;
            gb.startingBusinessSlots  = 4;
            gb.maxBusinessSlots       = 8;
            gb.handSize               = 5;
            gb.redrawsPerTurn         = 1;
            gb.shopCardsPerTurn       = 3;
            gb.eventInterval          = 3;

            // Economy
            gb.taxRate                = 0.15f;
            gb.reducedTaxRate         = 0.075f;
            gb.minTaxRate             = 0.03f;
            gb.sellRate               = 0.4f;

            // FBI
            gb.fbiRaidPenalty         = 300;
            gb.fbiStartingRisk        = 0f;

            // Territory / Customer Share
            gb.totalMarketCustomers   = 100;
            gb.winCustomerShare       = 60;
            gb.loseCustomerShare      = 60;

            // Market Pool
            gb.baseMarketCustomers    = 60;
            gb.earlyGrowthPerTurn     = 5;
            gb.midGrowthPerTurn       = 6;
            gb.lateGrowthPerTurn      = 8;
            gb.endGrowthPerTurn       = 10;

            // Score
            gb.customerShareScoreMultiplier = 500;
            gb.moneyScoreMultiplier       = 1;
            gb.comboScoreMultiplier       = 200;
            gb.businessScoreMultiplier    = 100;
            gb.earlyFinishBonusPerTurn    = 300;
            gb.fbiEvasionBonus            = 50;
            gb.winBonus                   = 1000;

            // Business Evolution
            gb.evolutionCustomerThreshold = 40;
            gb.evolutionTurnRequirement   = 15;

            // Employee
            gb.employeeLeaveTurnThreshold = 8;

            MarkDirty(gb);
            Debug.Log("[AssetGenerator] GameBalance.asset created.");
        }

        // ===================================================================
        // 2. ALL CARDS (5 ventures + general)
        // ===================================================================
        private static CardData[] GenerateAllCards()
        {
            var all = new List<CardData>();

            // --- FastFood venture cards ---
            all.AddRange(GenerateFastFoodCards());

            // --- Cafe venture cards ---
            all.AddRange(GenerateCafeCards());

            // --- TechApp venture cards ---
            all.AddRange(GenerateTechAppCards());

            // --- ClothingStore venture cards ---
            all.AddRange(GenerateClothingStoreCards());

            // --- GroceryStore venture cards ---
            all.AddRange(GenerateGroceryStoreCards());

            // --- General cards (available to all ventures) ---
            all.AddRange(GenerateGeneralCards());

            Debug.Log($"[AssetGenerator] {all.Count} card assets created.");
            return all.ToArray();
        }

        // -------------------------------------------------------------------
        // FastFood (FFC prefix)
        // -------------------------------------------------------------------
        private static CardData[] GenerateFastFoodCards()
        {
            var cards = new List<CardData>();

            // Operation cards
            var c = CreateOperation("FFC01_Grill", "Grill Station", VentureType.FastFood,
                100, 40, 3, 5f, 7f, 8f);
            c.description = "Fast and cheap. The core of any fast food joint.";
            c.rarity = Rarity.Common;
            cards.Add(c);

            c = CreateOperation("FFC02_FryerUnit", "Fryer Unit", VentureType.FastFood,
                120, 50, 4, 4f, 8f, 9f);
            c.description = "Golden fries keep customers coming back.";
            c.rarity = Rarity.Common;
            cards.Add(c);

            c = CreateOperation("FFC03_DriveThru", "Drive-Through", VentureType.FastFood,
                200, 70, 6, 6f, 6f, 10f);
            c.description = "Speed is everything. Serve without stopping.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            // Staff cards
            c = CreateStaff("FFC04_LineCook", "Line Cook", VentureType.FastFood,
                60, 15, 2, 4);
            c.description = "Fast hands, fast food.";
            cards.Add(c);

            c = CreateStaff("FFC05_Cashier", "Cashier", VentureType.FastFood,
                40, 10, 1, 2);
            c.description = "Keeps the line moving.";
            cards.Add(c);

            // Marketing cards
            c = CreateMarketing("FFC06_ValueMeal", "Value Meal Promo", VentureType.FastFood,
                80, 0.5f, 1.0f);
            c.description = "Combo deals bring the crowds.";
            cards.Add(c);

            // Supplier cards
            c = CreateSupplier("FFC07_BulkMeat", "Bulk Meat Supplier", VentureType.FastFood,
                90, 15f, 1f);
            c.description = "Cheaper ingredients, same taste.";
            cards.Add(c);

            // Event card
            c = CreateEvent("FFC08_HealthScare", "Health Scare", VentureType.FastFood,
                EventEffectType.TagCustomerPenalty, 2, -0.2f);
            c.description = "Bad press hits fast food hard.";
            c.rarity = Rarity.Uncommon;
            c.affectedTags = new[] { CardTag.Food };
            c.eventCustomerPenalty = -3;
            MarkDirty(c);
            cards.Add(c);

            return cards.ToArray();
        }

        // -------------------------------------------------------------------
        // Cafe (CAF prefix)
        // -------------------------------------------------------------------
        private static CardData[] GenerateCafeCards()
        {
            var cards = new List<CardData>();

            // Operation cards
            var c = CreateOperation("CAF01_EspressoBar", "Espresso Bar", VentureType.Cafe,
                120, 55, 3, 7f, 5f, 6f);
            c.description = "Premium coffee at premium prices.";
            c.rarity = Rarity.Common;
            cards.Add(c);

            c = CreateOperation("CAF02_PastryCounter", "Pastry Counter", VentureType.Cafe,
                90, 35, 2, 8f, 6f, 5f);
            c.description = "Fresh pastries pair perfectly with coffee.";
            c.rarity = Rarity.Common;
            cards.Add(c);

            c = CreateOperation("CAF03_CozyLounge", "Cozy Lounge", VentureType.Cafe,
                180, 60, 5, 9f, 4f, 3f);
            c.description = "People linger longer. And spend more.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            // Staff cards
            c = CreateStaff("CAF04_Barista", "Barista", VentureType.Cafe,
                70, 20, 3, 6);
            c.description = "Latte art and loyal customers.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            c = CreateStaff("CAF05_BaristaTrainee", "Barista Trainee", VentureType.Cafe,
                30, 8, 1, 2);
            c.description = "Learning the craft. Cheap but improving.";
            cards.Add(c);

            // Marketing cards
            c = CreateMarketing("CAF06_LoyaltyCard", "Loyalty Card Program", VentureType.Cafe,
                100, 0.8f, 0.5f);
            c.description = "Buy 9, get the 10th free. They always come back.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            // Supplier cards
            c = CreateSupplier("CAF07_BeanImporter", "Premium Bean Importer", VentureType.Cafe,
                110, 10f, 2f);
            c.description = "Single-origin beans elevate the brand.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            // Event card
            c = CreateEvent("CAF08_CoffeeTrend", "Coffee Trend", VentureType.Cafe,
                EventEffectType.TagCustomerBoost, 2, 0.4f);
            c.description = "Everyone wants artisan coffee this season.";
            c.rarity = Rarity.Common;
            c.affectedTags = new[] { CardTag.Coffee };
            MarkDirty(c);
            cards.Add(c);

            return cards.ToArray();
        }

        // -------------------------------------------------------------------
        // TechApp (TEC prefix)
        // -------------------------------------------------------------------
        private static CardData[] GenerateTechAppCards()
        {
            var cards = new List<CardData>();

            // Operation cards
            var c = CreateOperation("TEC01_AppLaunch", "App Launch", VentureType.TechApp,
                150, 0, 0, 3f, 3f, 8f);
            c.description = "No revenue yet. But the user base is growing.";
            c.rarity = Rarity.Common;
            c.activationDelay = 3;
            MarkDirty(c);
            cards.Add(c);

            c = CreateOperation("TEC02_ServerFarm", "Server Farm", VentureType.TechApp,
                250, 80, 2, 5f, 5f, 10f);
            c.description = "Scale up. Handle the traffic.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            c = CreateOperation("TEC03_PremiumTier", "Premium Tier", VentureType.TechApp,
                200, 100, 1, 7f, 2f, 7f);
            c.description = "Freemium to premium. The real money starts here.";
            c.rarity = Rarity.Rare;
            cards.Add(c);

            // Staff cards
            c = CreateStaff("TEC04_Developer", "Developer", VentureType.TechApp,
                80, 25, 1, 3);
            c.description = "Writes the code that runs the business.";
            c.rarity = Rarity.Common;
            cards.Add(c);

            c = CreateStaff("TEC05_SysAdmin", "Sys Admin", VentureType.TechApp,
                60, 18, 0, 2);
            c.description = "Keeps the servers running. No glamour, all necessity.";
            cards.Add(c);

            // Marketing cards
            c = CreateMarketing("TEC06_AppStoreAd", "App Store Ad", VentureType.TechApp,
                120, 1.0f, 2.0f);
            c.description = "Featured placement drives massive downloads.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            // Supplier cards
            c = CreateSupplier("TEC07_CloudProvider", "Cloud Provider", VentureType.TechApp,
                130, 20f, 1.5f);
            c.description = "Cheaper hosting, better uptime.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            // Event card
            c = CreateEvent("TEC08_DataBreach", "Data Breach", VentureType.TechApp,
                EventEffectType.TagCustomerPenalty, 1, -0.3f);
            c.description = "Users flee. Trust takes time to rebuild.";
            c.rarity = Rarity.Uncommon;
            c.affectedTags = new[] { CardTag.Tech };
            c.eventCustomerPenalty = -5;
            MarkDirty(c);
            cards.Add(c);

            return cards.ToArray();
        }

        // -------------------------------------------------------------------
        // ClothingStore (CLO prefix)
        // -------------------------------------------------------------------
        private static CardData[] GenerateClothingStoreCards()
        {
            var cards = new List<CardData>();

            // Operation cards
            var c = CreateOperation("CLO01_Storefront", "Storefront", VentureType.ClothingStore,
                130, 50, 3, 6f, 6f, 5f);
            c.description = "A well-placed shop draws foot traffic.";
            c.rarity = Rarity.Common;
            cards.Add(c);

            c = CreateOperation("CLO02_OnlineShop", "Online Shop", VentureType.ClothingStore,
                180, 65, 4, 5f, 7f, 8f);
            c.description = "Reach customers everywhere. No rent required.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            c = CreateOperation("CLO03_BoutiqueSection", "Boutique Section", VentureType.ClothingStore,
                220, 80, 2, 9f, 3f, 4f);
            c.description = "Exclusive pieces at exclusive prices.";
            c.rarity = Rarity.Rare;
            cards.Add(c);

            // Staff cards
            c = CreateStaff("CLO04_Tailor", "Tailor", VentureType.ClothingStore,
                70, 20, 2, 4);
            c.description = "Custom fits build reputation.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            c = CreateStaff("CLO05_SalesAssociate", "Sales Associate", VentureType.ClothingStore,
                40, 12, 2, 3);
            c.description = "Friendly face at the register.";
            cards.Add(c);

            // Marketing cards
            c = CreateMarketing("CLO06_FashionShow", "Fashion Show", VentureType.ClothingStore,
                150, 0.3f, 3.0f);
            c.description = "One big splash. Everyone talks about it.";
            c.rarity = Rarity.Rare;
            cards.Add(c);

            // Supplier cards
            c = CreateSupplier("CLO07_FabricWholesaler", "Fabric Wholesaler", VentureType.ClothingStore,
                100, 18f, 1f);
            c.description = "Bulk fabric at bulk prices.";
            cards.Add(c);

            // Event card
            c = CreateEvent("CLO08_FashionWeek", "Fashion Week", VentureType.ClothingStore,
                EventEffectType.TagCustomerBoost, 1, 0.5f);
            c.description = "The spotlight is on fashion. Cash in.";
            c.rarity = Rarity.Uncommon;
            c.affectedTags = new[] { CardTag.Trendy };
            MarkDirty(c);
            cards.Add(c);

            return cards.ToArray();
        }

        // -------------------------------------------------------------------
        // GroceryStore (GRO prefix)
        // -------------------------------------------------------------------
        private static CardData[] GenerateGroceryStoreCards()
        {
            var cards = new List<CardData>();

            // Operation cards
            var c = CreateOperation("GRO01_FreshProduce", "Fresh Produce Aisle", VentureType.GroceryStore,
                80, 35, 5, 7f, 8f, 6f);
            c.description = "Everyday essentials. Steady demand.";
            c.rarity = Rarity.Common;
            cards.Add(c);

            c = CreateOperation("GRO02_DeliCounter", "Deli Counter", VentureType.GroceryStore,
                110, 45, 4, 8f, 6f, 5f);
            c.description = "Fresh cuts and prepared meals.";
            c.rarity = Rarity.Common;
            cards.Add(c);

            c = CreateOperation("GRO03_BulkWarehouse", "Bulk Warehouse", VentureType.GroceryStore,
                200, 60, 7, 5f, 9f, 7f);
            c.description = "Buy in bulk. Sell in volume.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            // Staff cards
            c = CreateStaff("GRO04_Stocker", "Stocker", VentureType.GroceryStore,
                35, 10, 2, 3);
            c.description = "Shelves stay full. Customers stay happy.";
            cards.Add(c);

            c = CreateStaff("GRO05_StoreManager", "Store Manager", VentureType.GroceryStore,
                80, 22, 3, 5);
            c.description = "Keeps everything running smoothly.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            // Marketing cards
            c = CreateMarketing("GRO06_WeeklyFlyer", "Weekly Flyer", VentureType.GroceryStore,
                50, 0.4f, 0.8f);
            c.description = "Deals of the week. In every mailbox.";
            cards.Add(c);

            // Supplier cards
            c = CreateSupplier("GRO07_LocalFarm", "Local Farm Partnership", VentureType.GroceryStore,
                70, 12f, 2f);
            c.description = "Farm to shelf. Fresher and cheaper.";
            cards.Add(c);

            // Event card
            c = CreateEvent("GRO08_SupplyShortage", "Supply Shortage", VentureType.GroceryStore,
                EventEffectType.AllIncomeReduction, 2, -0.15f);
            c.description = "Supply chain disruption hits grocery hardest.";
            c.rarity = Rarity.Uncommon;
            cards.Add(c);

            return cards.ToArray();
        }

        // -------------------------------------------------------------------
        // General cards (GEN prefix, isGeneralCard = true)
        // -------------------------------------------------------------------
        private static CardData[] GenerateGeneralCards()
        {
            var cards = new List<CardData>();

            // General Operation cards
            var c = CreateGeneralOperation("GEN01_PopUpShop", "Pop-Up Shop",
                60, 25, 2, 4f, 7f, 7f);
            c.description = "Temporary but effective. Test the waters.";
            cards.Add(c);

            c = CreateGeneralOperation("GEN02_FranchiseDesk", "Franchise Desk",
                180, 30, 1, 5f, 5f, 5f);
            c.description = "Expand your brand through partnerships.";
            c.rarity = Rarity.Uncommon;
            MarkDirty(c);
            cards.Add(c);

            // General Staff cards
            c = CreateGeneralStaff("GEN03_Intern", "Intern", 20, 5, 1);
            c.description = "Eager to learn. Cheap to keep.";
            cards.Add(c);

            c = CreateGeneralStaff("GEN04_Accountant", "Accountant", 80, 20, 0);
            c.description = "Saves money on taxes.";
            c.rarity = Rarity.Uncommon;
            c.taxReduction = 0.5f;
            c.activeAbilityType = ActiveAbilityType.NullifyTaxThisTurn;
            c.activeAbilityName = "Tax Plan";
            c.activeAbilityDesc = "Tax is nullified this turn.";
            MarkDirty(c);
            cards.Add(c);

            c = CreateGeneralStaff("GEN05_SecurityGuard", "Security Guard", 50, 15, 0);
            c.description = "Reduces legal risk across the board.";
            c.rarity = Rarity.Common;
            MarkDirty(c);
            cards.Add(c);

            c = CreateGeneralStaff("GEN06_HRManager", "HR Manager", 90, 22, 1);
            c.description = "Better staff management means lower turnover.";
            c.rarity = Rarity.Uncommon;
            c.activeAbilityType = ActiveAbilityType.MotivateAllEmployees;
            c.activeAbilityName = "Team Building";
            c.activeAbilityDesc = "All employees gain +1 customer this turn.";
            MarkDirty(c);
            cards.Add(c);

            // General Marketing cards
            c = CreateGeneralMarketing("GEN07_SocialMediaAd", "Social Media Ad",
                60, 0.3f, 1.0f);
            c.description = "Cheap reach. Everyone scrolls.";
            cards.Add(c);

            c = CreateGeneralMarketing("GEN08_BillboardAd", "Billboard Ad",
                120, 0.6f, 0.5f);
            c.description = "Old school but effective. High visibility.";
            c.rarity = Rarity.Uncommon;
            MarkDirty(c);
            cards.Add(c);

            c = CreateGeneralMarketing("GEN09_InfluencerDeal", "Influencer Deal",
                150, 0.2f, 2.5f);
            c.description = "One post, massive exposure.";
            c.rarity = Rarity.Rare;
            MarkDirty(c);
            cards.Add(c);

            // General Event cards
            c = CreateGeneralEvent("GEN10_TaxAudit", "Tax Audit",
                EventEffectType.AllIncomeReduction, 1, -0.2f);
            c.description = "The taxman cometh. Everyone pays.";
            c.rarity = Rarity.Common;
            MarkDirty(c);
            cards.Add(c);

            c = CreateGeneralEvent("GEN11_EconomicBoom", "Economic Boom",
                EventEffectType.TagCustomerBoost, 2, 0.3f);
            c.description = "Good times for everyone. Spend freely.";
            c.rarity = Rarity.Common;
            c.affectedTags = new CardTag[0];
            MarkDirty(c);
            cards.Add(c);

            c = CreateGeneralEvent("GEN12_Recession", "Recession",
                EventEffectType.AllIncomeReduction, 2, -0.25f);
            c.description = "Belts tighten. Only the efficient survive.";
            c.rarity = Rarity.Uncommon;
            MarkDirty(c);
            cards.Add(c);

            c = CreateGeneralEvent("GEN13_ViralTrend", "Viral Trend",
                EventEffectType.TagDoubleEffect, 1, 1.0f);
            c.description = "Social media explodes. Marketing pays double.";
            c.rarity = Rarity.Uncommon;
            c.affectedTags = new[] { CardTag.Marketing };
            MarkDirty(c);
            cards.Add(c);

            c = CreateGeneralEvent("GEN14_InvestorSeason", "Investor Season",
                EventEffectType.TagDoubleEffectFinance, 1, 1.0f);
            c.description = "Money flows. Finance strategies pay off big.";
            c.rarity = Rarity.Uncommon;
            c.affectedTags = new[] { CardTag.Finance };
            MarkDirty(c);
            cards.Add(c);

            c = CreateGeneralEvent("GEN15_CancelCulture", "Cancel Culture",
                EventEffectType.HighFBICustomerPenalty, 1, -0.4f);
            c.description = "Public backlash hits those with skeletons.";
            c.rarity = Rarity.Rare;
            c.eventFBIThreshold = 0.3f;
            MarkDirty(c);
            cards.Add(c);

            // General Supplier cards
            var sup = CreateOrReplace<CardData>(GeneralCards + "/GEN16_BulkDealer.asset");
            sup.cardId = "GEN16_BulkDealer";
            sup.cardName = "Bulk Dealer";
            sup.cardType = CardType.Upgrade;
            sup.rarity = Rarity.Common;
            sup.buyCost = 60;
            sup.description = "Generic cost savings for any venture.";
            sup.tags = new[] { CardTag.Support };
            sup.ventureType = VentureType.FastFood;
            sup.targetSlotType = SlotType.Supplier;
            sup.isGeneralCard = true;
            sup.costReductionPercent = 8f;
            sup.qualityBoostAmount = 0.5f;
            sup.upgradeEffectType = UpgradeEffectType.None;
            sup.upgradeValue = 0f;
            sup.isGlobalUpgrade = false;
            sup.closedEmployeeSlots = 0;
            sup.extraActions = 0;
            sup.qualityScore = 0f;
            sup.priceScore = 0f;
            sup.serviceSpeedScore = 0f;
            sup.platformRatingGain = 0f;
            sup.platformRatingOnPlay = 0f;
            sup.legalRiskPerTurn = 0;
            sup.legalRiskOnPlay = 0;
            MarkDirty(sup);
            cards.Add(sup);

            // General Upgrade cards
            var upg = CreateOrReplace<CardData>(GeneralCards + "/GEN17_Automation.asset");
            upg.cardId = "GEN17_Automation";
            upg.cardName = "Automation";
            upg.cardType = CardType.Upgrade;
            upg.rarity = Rarity.Uncommon;
            upg.buyCost = 300;
            upg.description = "Machines replace hands. Efficient but soulless.";
            upg.tags = new[] { CardTag.Tech, CardTag.Automation };
            upg.ventureType = VentureType.FastFood;
            upg.targetSlotType = SlotType.Operation;
            upg.isGeneralCard = true;
            upg.upgradeEffectType = UpgradeEffectType.IncomePercentWithSlotLoss;
            upg.upgradeValue = 30f;
            upg.isGlobalUpgrade = false;
            upg.closedEmployeeSlots = 1;
            upg.extraActions = 0;
            upg.qualityScore = 0f;
            upg.priceScore = 0f;
            upg.serviceSpeedScore = 0f;
            upg.platformRatingGain = 0f;
            upg.platformRatingOnPlay = 0f;
            upg.legalRiskPerTurn = 0;
            upg.legalRiskOnPlay = 0;
            upg.costReductionPercent = 0f;
            upg.qualityBoostAmount = 0f;
            MarkDirty(upg);
            cards.Add(upg);

            var ai = CreateOrReplace<CardData>(GeneralCards + "/GEN18_AIAssistant.asset");
            ai.cardId = "GEN18_AIAssistant";
            ai.cardName = "AI Assistant";
            ai.cardType = CardType.Upgrade;
            ai.rarity = Rarity.Rare;
            ai.buyCost = 400;
            ai.description = "Extra action each turn. The strongest upgrade.";
            ai.tags = new[] { CardTag.Tech, CardTag.AI };
            ai.ventureType = VentureType.FastFood;
            ai.targetSlotType = SlotType.Operation;
            ai.isGeneralCard = true;
            ai.upgradeEffectType = UpgradeEffectType.ExtraAction;
            ai.upgradeValue = 0f;
            ai.isGlobalUpgrade = true;
            ai.closedEmployeeSlots = 0;
            ai.extraActions = 1;
            ai.qualityScore = 0f;
            ai.priceScore = 0f;
            ai.serviceSpeedScore = 0f;
            ai.platformRatingGain = 0f;
            ai.platformRatingOnPlay = 0f;
            ai.legalRiskPerTurn = 0;
            ai.legalRiskOnPlay = 0;
            ai.costReductionPercent = 0f;
            ai.qualityBoostAmount = 0f;
            MarkDirty(ai);
            cards.Add(ai);

            var sec = CreateOrReplace<CardData>(GeneralCards + "/GEN19_SecuritySystem.asset");
            sec.cardId = "GEN19_SecuritySystem";
            sec.cardName = "Security System";
            sec.cardType = CardType.Upgrade;
            sec.rarity = Rarity.Uncommon;
            sec.buyCost = 280;
            sec.description = "Reduces legal risk across all ventures.";
            sec.tags = new[] { CardTag.Security };
            sec.ventureType = VentureType.FastFood;
            sec.targetSlotType = SlotType.Operation;
            sec.isGeneralCard = true;
            sec.upgradeEffectType = UpgradeEffectType.ReduceFBIRisk;
            sec.upgradeValue = 25f;
            sec.isGlobalUpgrade = true;
            sec.closedEmployeeSlots = 0;
            sec.extraActions = 0;
            sec.qualityScore = 0f;
            sec.priceScore = 0f;
            sec.serviceSpeedScore = 0f;
            sec.platformRatingGain = 0f;
            sec.platformRatingOnPlay = 0f;
            sec.legalRiskPerTurn = 0;
            sec.legalRiskOnPlay = 0;
            sec.costReductionPercent = 0f;
            sec.qualityBoostAmount = 0f;
            MarkDirty(sec);
            cards.Add(sec);

            var delivery = CreateOrReplace<CardData>(GeneralCards + "/GEN20_DeliveryNetwork.asset");
            delivery.cardId = "GEN20_DeliveryNetwork";
            delivery.cardName = "Delivery Network";
            delivery.cardType = CardType.Upgrade;
            delivery.rarity = Rarity.Uncommon;
            delivery.buyCost = 250;
            delivery.description = "Reach more customers across all ventures.";
            delivery.tags = new[] { CardTag.Logistics };
            delivery.ventureType = VentureType.FastFood;
            delivery.targetSlotType = SlotType.Supplier;
            delivery.isGeneralCard = true;
            delivery.upgradeEffectType = UpgradeEffectType.GlobalCustomerPerTurn;
            delivery.upgradeValue = 2f;
            delivery.isGlobalUpgrade = true;
            delivery.closedEmployeeSlots = 0;
            delivery.extraActions = 0;
            delivery.qualityScore = 0f;
            delivery.priceScore = 0f;
            delivery.serviceSpeedScore = 0f;
            delivery.platformRatingGain = 0f;
            delivery.platformRatingOnPlay = 0f;
            delivery.legalRiskPerTurn = 0;
            delivery.legalRiskOnPlay = 0;
            delivery.costReductionPercent = 0f;
            delivery.qualityBoostAmount = 0f;
            MarkDirty(delivery);
            cards.Add(delivery);

            return cards.ToArray();
        }

        // ===================================================================
        // 3. STARTING DECK PRESET
        // ===================================================================
        private static void GenerateStartingDeck(CardData[] allCards)
        {
            string path = DecksRoot + "/StarterDeck.asset";
            var deck = CreateOrReplace<DeckPresetData>(path);

            deck.presetName    = "Starter Deck";
            deck.startingMoney = 500;

            CardData gen01 = FindCard(allCards, "GEN01_PopUpShop");
            CardData gen03 = FindCard(allCards, "GEN03_Intern");
            CardData gen07 = FindCard(allCards, "GEN07_SocialMediaAd");
            CardData gen16 = FindCard(allCards, "GEN16_BulkDealer");

            // 14 cards: general starter cards that work with any venture
            deck.cards = new DeckEntry[]
            {
                new DeckEntry { card = gen01, count = 2 },  // 2x Pop-Up Shop (basic operation)
                new DeckEntry { card = gen03, count = 4 },  // 4x Intern (cheap staff)
                new DeckEntry { card = gen07, count = 4 },  // 4x Social Media Ad (basic marketing)
                new DeckEntry { card = gen16, count = 4 },  // 4x Bulk Dealer (basic supplier)
            };

            MarkDirty(deck);
            Debug.Log("[AssetGenerator] StarterDeck.asset created (14 cards).");
        }

        private static CardData FindCard(CardData[] cards, string id)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null && cards[i].cardId == id) return cards[i];
            }
            Debug.LogError($"[AssetGenerator] Card not found: {id}");
            return null;
        }

        // ===================================================================
        // 4. MEGACORP RIVAL
        // ===================================================================
        private static void GenerateMegaCorpRival()
        {
            string path = RivalsRoot + "/MegaCorp.asset";
            var rival = CreateOrReplace<RivalData>(path);

            rival.rivalId                  = "RIVAL_MegaCorp";
            rival.rivalName                = "MegaCorp";
            rival.personality              = RivalPersonality.Balanced;
            rival.tagline                  = "This industry isn't big enough for both of us.";

            // Starting Stats
            rival.startingMoney            = 400;
            rival.startingIncome           = 80;
            rival.startingCustomers        = 5;
            rival.startingBusinessName     = "MegaCorp HQ";

            // Behavior
            rival.actionsPerTurn           = 2;
            rival.aggressionThreshold      = 0.5f;
            rival.maxBusinesses            = 4;
            rival.maxEmployeesPerBusiness  = 3;

            // Growth Parameters
            rival.businessCostThreshold    = 200;
            rival.hireCostThreshold        = 80;
            rival.baseBusinessIncome       = 80;
            rival.baseBusinessCustomers    = 5;
            rival.employeeIncomeBoost      = 30;
            rival.employeeCustomerBoost    = 3;
            rival.aggressiveCustomerBoost  = 8;
            rival.aggressiveIncomeBoost    = 50;
            rival.passiveCustomerGrowth    = 2;
            rival.passiveIncomeGrowth      = 10;

            rival.possibleBusinessNames = new[]
            {
                "Rival Fast Food",
                "Rival Cafe",
                "Rival Tech App",
                "Rival Clothing Store",
                "Rival Grocery Store"
            };

            // Venture Mirror — indexed by VentureType ordinal
            // [0] FastFood, [1] Cafe, [2] TechApp, [3] ClothingStore, [4] GroceryStore
            rival.ventureMatchedNames     = new[] { "Rival Fast Food", "Rival Cafe", "Rival Tech App", "Rival Clothing Store", "Rival Grocery Store" };
            rival.ventureMatchedIncome    = new[] { 45, 55, 0, 50, 40 };
            rival.ventureMatchedCustomers = new[] { 4, 3, 0, 3, 5 };

            // Growth Schedule (GDD Section 8.3)
            rival.growthMilestones = new RivalMilestone[]
            {
                new RivalMilestone
                {
                    turn = 5,
                    targetBusinesses = 2,
                    targetEmployees = 2,
                    targetTerritories = 15,
                    enableAggression = false
                },
                new RivalMilestone
                {
                    turn = 8,
                    targetBusinesses = 3,
                    targetEmployees = 4,
                    targetTerritories = 25,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 12,
                    targetBusinesses = 3,
                    targetEmployees = 6,
                    targetTerritories = 35,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 15,
                    targetBusinesses = 4,
                    targetEmployees = 8,
                    targetTerritories = 45,
                    enableAggression = true
                },
                new RivalMilestone
                {
                    turn = 20,
                    targetBusinesses = 5,
                    targetEmployees = 10,
                    targetTerritories = 55,
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

            MarkDirty(rival);
            Debug.Log("[AssetGenerator] MegaCorp.asset created.");
        }

        // ===================================================================
        // 5. COMBOS (10 combos from GDD Combo Matrix)
        // ===================================================================
        private static void GenerateCombos(CardData[] allCards)
        {
            // COMBO 01 - Latte Art (Cafe: Espresso Bar + Barista)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_01_LatteArt.asset");
                combo.comboId       = "COMBO_01_LatteArt";
                combo.comboName     = "Latte Art";
                combo.displayText   = "LATTE ART!";
                combo.tier          = ComboTier.Easy;
                combo.description   = "Espresso Bar + Barista = customers x2, income +50%.";
                combo.requiredCardIds = new[] { "CAF01_EspressoBar", "CAF04_Barista" };
                combo.requiredTags    = new[] { CardTag.Coffee };
                combo.requiresSpecificPlacement = true;
                combo.employeeCardId  = "CAF04_Barista";
                combo.businessCardId  = "CAF01_EspressoBar";
                combo.customerMultiplier = 2f;
                combo.incomeMultiplier   = 1.5f;
                combo.glowColor          = new Color(0.6f, 0.4f, 0.2f, 1f);
                combo.comboSoundId       = "combo_trigger";
                combo.screenShakeIntensity = 0.3f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 02 - Drive-Thru Rush (FastFood: Drive-Through + Line Cook)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_02_DriveThruRush.asset");
                combo.comboId       = "COMBO_02_DriveThruRush";
                combo.comboName     = "Drive-Thru Rush";
                combo.displayText   = "DRIVE-THRU RUSH!";
                combo.tier          = ComboTier.Easy;
                combo.description   = "Drive-Through + Line Cook = customers +50%, speed bonus.";
                combo.requiredCardIds = new[] { "FFC03_DriveThru", "FFC04_LineCook" };
                combo.requiredTags    = new[] { CardTag.Food };
                combo.requiresSpecificPlacement = true;
                combo.employeeCardId  = "FFC04_LineCook";
                combo.businessCardId  = "FFC03_DriveThru";
                combo.customerMultiplier = 1.5f;
                combo.incomeMultiplier   = 1.2f;
                combo.glowColor          = new Color(1f, 0.5f, 0f, 1f);
                combo.comboSoundId       = "combo_trigger";
                combo.screenShakeIntensity = 0.3f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 03 - Tech Scale (TechApp: Server Farm + Developer)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_03_TechScale.asset");
                combo.comboId       = "COMBO_03_TechScale";
                combo.comboName     = "Tech Scale";
                combo.displayText   = "TECH SCALE!";
                combo.tier          = ComboTier.Easy;
                combo.description   = "Server Farm + Developer = income x2.";
                combo.requiredCardIds = new[] { "TEC02_ServerFarm", "TEC04_Developer" };
                combo.requiredTags    = new[] { CardTag.Tech };
                combo.requiresSpecificPlacement = true;
                combo.employeeCardId  = "TEC04_Developer";
                combo.businessCardId  = "TEC02_ServerFarm";
                combo.incomeMultiplier = 2f;
                combo.glowColor        = new Color(0f, 0.8f, 1f, 1f);
                combo.comboSoundId     = "combo_trigger";
                combo.screenShakeIntensity = 0.3f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 04 - Fashion Empire (Clothing: Boutique + Tailor + Fashion Show)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_04_FashionEmpire.asset");
                combo.comboId       = "COMBO_04_FashionEmpire";
                combo.comboName     = "Fashion Empire";
                combo.displayText   = "FASHION EMPIRE!";
                combo.tier          = ComboTier.Medium;
                combo.description   = "Boutique + Tailor + Fashion Show = customers x3.";
                combo.requiredCardIds = new[] { "CLO03_BoutiqueSection", "CLO04_Tailor", "CLO06_FashionShow" };
                combo.requiredTags    = new[] { CardTag.Trendy };
                combo.requiresSpecificPlacement = false;
                combo.customerMultiplier  = 3f;
                combo.glowColor           = new Color(0.8f, 0f, 1f, 1f);
                combo.comboSoundId        = "combo_trigger";
                combo.screenShakeIntensity = 0.5f;
                combo.screenShakeDuration  = 0.4f;
                MarkDirty(combo);
            }

            // COMBO 05 - Farm to Table (Grocery: Fresh Produce + Local Farm)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_05_FarmToTable.asset");
                combo.comboId       = "COMBO_05_FarmToTable";
                combo.comboName     = "Farm to Table";
                combo.displayText   = "FARM TO TABLE!";
                combo.tier          = ComboTier.Easy;
                combo.description   = "Fresh Produce + Local Farm = quality boost, cost reduction.";
                combo.requiredCardIds = new[] { "GRO01_FreshProduce", "GRO07_LocalFarm" };
                combo.requiredTags    = new[] { CardTag.Food, CardTag.Organic };
                combo.requiresSpecificPlacement = false;
                combo.bonusIncome     = 30;
                combo.customerMultiplier = 1.5f;
                combo.glowColor       = new Color(0.2f, 0.8f, 0.2f, 1f);
                combo.comboSoundId    = "combo_trigger";
                combo.screenShakeIntensity = 0.3f;
                combo.screenShakeDuration  = 0.3f;
                MarkDirty(combo);
            }

            // COMBO 06 - AI Revolution (General: Automation + AI Assistant)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_06_AIRevolution.asset");
                combo.comboId       = "COMBO_06_AIRevolution";
                combo.comboName     = "AI Revolution";
                combo.displayText   = "AI REVOLUTION!";
                combo.tier          = ComboTier.Hard;
                combo.description   = "Automation + AI Assistant = +1 action, income x2.";
                combo.requiredCardIds = new[] { "GEN17_Automation", "GEN18_AIAssistant" };
                combo.requiredTags    = new[] { CardTag.Tech, CardTag.AI };
                combo.requiresSpecificPlacement = false;
                combo.extraActions     = 1;
                combo.incomeMultiplier = 2f;
                combo.glowColor        = new Color(0f, 1f, 1f, 1f);
                combo.comboSoundId     = "combo_trigger";
                combo.screenShakeIntensity = 0.6f;
                combo.screenShakeDuration  = 0.5f;
                MarkDirty(combo);
            }

            // COMBO 07 - Crisis Hunter (General: during Recession with 1000+ money)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_07_CrisisHunter.asset");
                combo.comboId       = "COMBO_07_CrisisHunter";
                combo.comboName     = "Crisis Hunter";
                combo.displayText   = "CRISIS HUNTER!";
                combo.tier          = ComboTier.Hard;
                combo.description   = "During Recession with 1000+ money: shop 50% off, steal rival employee.";
                combo.requiredCardIds = new string[0];
                combo.requiredTags    = new[] { CardTag.Finance };
                combo.requiresActiveEvent  = true;
                combo.requiredEventId      = "GEN12_Recession";
                combo.requiresMinMoney     = true;
                combo.minMoneyRequired     = 1000;
                combo.shopDiscount         = 0.5f;
                combo.transferRivalEmployee = true;
                combo.glowColor            = new Color(1f, 0.84f, 0f, 1f);
                combo.comboSoundId         = "combo_trigger";
                combo.screenShakeIntensity = 0.5f;
                combo.screenShakeDuration  = 0.4f;
                MarkDirty(combo);
            }

            // COMBO 08 - Value Meal Blitz (FastFood: Grill + Value Meal + Bulk Meat)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_08_ValueMealBlitz.asset");
                combo.comboId       = "COMBO_08_ValueMealBlitz";
                combo.comboName     = "Value Meal Blitz";
                combo.displayText   = "VALUE MEAL BLITZ!";
                combo.tier          = ComboTier.Medium;
                combo.description   = "Grill + Value Meal Promo + Bulk Meat = income +50, customers x2.";
                combo.requiredCardIds = new[] { "FFC01_Grill", "FFC06_ValueMeal", "FFC07_BulkMeat" };
                combo.requiredTags    = new[] { CardTag.Food };
                combo.requiresSpecificPlacement = false;
                combo.bonusIncome     = 50;
                combo.customerMultiplier = 2f;
                combo.glowColor       = new Color(1f, 0.3f, 0.1f, 1f);
                combo.comboSoundId    = "combo_trigger";
                combo.screenShakeIntensity = 0.4f;
                combo.screenShakeDuration  = 0.4f;
                MarkDirty(combo);
            }

            // COMBO 09 - Premium Experience (Cafe: Cozy Lounge + Premium Bean Importer + Loyalty Card)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_09_PremiumExperience.asset");
                combo.comboId       = "COMBO_09_PremiumExperience";
                combo.comboName     = "Premium Experience";
                combo.displayText   = "PREMIUM EXPERIENCE!";
                combo.tier          = ComboTier.Hard;
                combo.description   = "Cozy Lounge + Bean Importer + Loyalty Card = income x2, customers x2.";
                combo.requiredCardIds = new[] { "CAF03_CozyLounge", "CAF07_BeanImporter", "CAF06_LoyaltyCard" };
                combo.requiredTags    = new[] { CardTag.Coffee };
                combo.requiresSpecificPlacement = false;
                combo.incomeMultiplier    = 2f;
                combo.customerMultiplier  = 2f;
                combo.glowColor           = new Color(0.4f, 0.2f, 0.1f, 1f);
                combo.comboSoundId        = "combo_trigger";
                combo.screenShakeIntensity = 0.6f;
                combo.screenShakeDuration  = 0.5f;
                MarkDirty(combo);
            }

            // COMBO 10 - Monopoly (Automatic: 4+ businesses, 55%+ market share)
            {
                var combo = CreateOrReplace<ComboData>(CombosRoot + "/COMBO_10_Monopoly.asset");
                combo.comboId       = "COMBO_10_Monopoly";
                combo.comboName     = "Monopoly";
                combo.displayText   = "MONOPOLY!";
                combo.tier          = ComboTier.Automatic;
                combo.description   = "4+ businesses, 55%+ market share = rival -3 customers/turn, income +20%.";
                combo.requiredCardIds  = new string[0];
                combo.requiredTags     = new CardTag[0];
                combo.requiresSpecificPlacement = false;
                combo.minActiveBusinesses = 4;
                combo.minMarketShare      = 0.55f;
                combo.rivalCustomerPenalty = 3;
                combo.incomeMultiplier     = 1.2f;
                combo.glowColor            = new Color(1f, 0f, 0f, 1f);
                combo.comboSoundId         = "combo_trigger";
                combo.screenShakeIntensity = 0.7f;
                combo.screenShakeDuration  = 0.6f;
                MarkDirty(combo);
            }

            Debug.Log("[AssetGenerator] 10 combo assets created.");
        }

        // ===================================================================
        // 6. META PROGRESSION
        // ===================================================================
        private static void GenerateMetaProgression()
        {
            string path = DataRoot + "/MetaProgression.asset";
            var meta = CreateOrReplace<MetaProgressionData>(path);

            meta.unlockTiers = new UnlockTier[]
            {
                new UnlockTier
                {
                    xpRequired = 50,
                    unlockDescription = "Shop pool unlocked. Common cards available.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
                new UnlockTier
                {
                    xpRequired = 200,
                    unlockDescription = "Uncommon cards enter the shop pool.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
                new UnlockTier
                {
                    xpRequired = 500,
                    unlockDescription = "Rare cards unlocked. Shadow Inc. rival available.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
                new UnlockTier
                {
                    xpRequired = 1000,
                    unlockDescription = "All cards unlocked. Ascension 1 mode active.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
                new UnlockTier
                {
                    xpRequired = 2000,
                    unlockDescription = "The Cartel rival unlocked. Ascension 2 mode active.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
                new UnlockTier
                {
                    xpRequired = 5000,
                    unlockDescription = "Ascension 3 mode. All content unlocked.",
                    unlockedCards = new CardData[0],
                    unlockedRival = null
                },
            };

            meta.ascensionLevels = new AscensionLevel[]
            {
                new AscensionLevel
                {
                    level = 1,
                    description = "Rival more aggressive, events more frequent.",
                    rivalAggressionMultiplier = 1.2f,
                    startingMoneyModifier = 0,
                    crisisFrequencyMultiplier = 1.2f
                },
                new AscensionLevel
                {
                    level = 2,
                    description = "Starting money -100, rival very aggressive.",
                    rivalAggressionMultiplier = 1.5f,
                    startingMoneyModifier = -100,
                    crisisFrequencyMultiplier = 1.3f
                },
                new AscensionLevel
                {
                    level = 3,
                    description = "Hardest mode. Crisis frequency x1.5, rival merciless.",
                    rivalAggressionMultiplier = 2.0f,
                    startingMoneyModifier = -200,
                    crisisFrequencyMultiplier = 1.5f
                },
            };

            MarkDirty(meta);
            Debug.Log("[AssetGenerator] MetaProgression.asset created.");
        }
    }
}
#endif
