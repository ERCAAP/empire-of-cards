using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "EmpireOfCards/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("--- Identity ---")]
        public string cardId;               // "B02_Kahveci", "C03_Barista" etc (internal IDs)
        public string cardName;             // Display name: "Coffee Shop"
        public CardType cardType;
        public Rarity rarity;
        [TextArea(2, 4)]
        public string description;          // Flavor text
        public Sprite icon;
        public Sprite cardFrame;
        public CardTag[] tags;              // food, coffee, trendy etc.

        [Header("--- Cost ---")]
        public int buyCost;                 // Shop/play cost

        // ====== BUSINESS (Blue) ======
        [Header("--- Business Properties ---")]
        public int incomePerTurn;           // Base income/turn
        public int customersPerTurn;        // Base customers/turn
        public int employeeSlots;           // Employee slot count (1-3)
        public bool hasTrendBonus;          // Coffee Shop: bonus when trend active
        [Range(1f, 3f)]
        public float trendIncomeMultiplier = 1f;  // Coffee Shop: 1.5
        public int activationDelay;         // Tech Startup: 3 turn delay
        public bool requiresTrendToOperate; // Nightclub: 0 income without trend
        public bool hasRandomIncome;        // Crypto Exchange
        public int randomIncomeMin;         // 0
        public int randomIncomeMax;         // 250
        public int[] randomIncomeThresholds;// [0, 0, 100, 100, 250, 250] (dice 1-6)
        public string foodBonusTag;         // Organic Farm: bonus to all "food" tagged
        public int foodBonusAmount;         // +20 per food business
        public int globalCustomerBonus;     // Ad Agency: +2 to all businesses

        // Business Evolution (GDD Section 3.1)
        public bool canEvolve;
        public CardData evolvedForm;        // Diner -> Restaurant -> Chain
        public int evolutionCustomerReq;    // 40 total customers
        public int evolutionTurnReq;        // 15 turns

        // ====== EMPLOYEE (Green) ======
        [Header("--- Employee Properties ---")]
        public int salaryPerTurn;           // Salary/turn
        public int customerBonus;           // Base customer contribution
        public int synergyCustomerBonus;    // Extra bonus in matching business (Barista coffee: +6 vs +3)
        public CardTag synergyTag;          // Which tag synergizes (Coffee for Barista)
        public float incomeMultiplier;      // Marketing Guru: 0.25 (+25%)
        public float incomeFlatBonus;       // Chef in food: +30
        public CardTag incomeBonusTag;      // Which tag activates flat bonus
        public int fbiRiskPerTurn;          // Hacker: 10, Fraudster: 12
        public int illegalIncomePerTurn;    // Fraudster: 120
        public bool preventsTransfer;       // Loyal Manager: transfer protection
        public float taxReduction;          // Accountant: 0.5 (50% reduction)

        // Active Ability (GDD Section 3.2)
        public ActiveAbilityType activeAbilityType;
        public string activeAbilityName;    // "Latte Festival"
        [TextArea(1, 3)]
        public string activeAbilityDesc;    // Tooltip description
        public float abilityValue1;         // Primary param (multiplier or flat)
        public int abilityValue2;           // Secondary param (customers, money etc)

        // ====== ACTION (Red) ======
        [Header("--- Action Properties ---")]
        public ActionEffectType actionEffectType;
        public int actionValue;             // Flat value (+150 money, +3 customers)
        public float actionMultiplier;      // Viral Marketing: 2.0 (x2)
        public int actionFBIRisk;           // Fake Reviews: 12, Sabotage: 15
        public int actionDebtDuration;      // Investor Pitch: 3 turns
        public float actionDebtPercent;     // 0.15 (15% income to investor)
        public float actionIncomeSacrifice; // Price Slashing: 0.5 (50% income sacrifice)

        // ====== UPGRADE (Purple) ======
        [Header("--- Upgrade Properties ---")]
        public UpgradeEffectType upgradeEffectType;
        public float upgradeValue;          // +10%, +30%, +2 customers etc
        public bool isGlobalUpgrade;        // Affects entire board
        public int closedEmployeeSlots;     // Automation: 1 slot closed
        public int extraActions;            // AI Assistant: +1

        // ====== EVENT (Yellow) ======
        [Header("--- Event Properties ---")]
        public EventEffectType eventEffectType;
        public int eventDuration;           // 1-2 turns
        public float eventMultiplier;       // +50% = 0.5, -30% = -0.3
        public CardTag[] affectedTags;      // Which tags are affected
        public int eventCustomerPenalty;    // Data Breach: -5
        public float eventFBIThreshold;     // Cancel Culture: 0.30 (30%)
    }
}
