using UnityEngine;

namespace EmpireOfCards.Data
{
    using EmpireOfCards.Core;

    [CreateAssetMenu(fileName = "NewCard", menuName = "EmpireOfCards/Card Data")]
    public class CardData : ScriptableObject
    {
        [Header("Base Info")]
        [Tooltip("Unique identifier, e.g. B02_Kahveci")]
        public string cardId;

        [Tooltip("Display name, e.g. Kahveci")]
        public string cardName;

        public CardType cardType;
        public Rarity rarity;
        public int buyCost;

        [TextArea(2, 4)]
        [Tooltip("Turkish flavor text")]
        public string description;

        public Sprite icon;
        public Sprite cardFrame;
        public CardTag[] tags;

        [Header("Business Stats")]
        [Tooltip("Base income generated each turn")]
        public int incomePerTurn;

        [Tooltip("Base customers attracted each turn")]
        public int customersPerTurn;

        [Tooltip("How many employee slots this business provides")]
        public int employeeSlots;

        [Tooltip("Whether this card benefits from trend mechanics")]
        public bool hasTrendBonus;

        [Tooltip("Trend multiplier (e.g. 1.5 for Kahveci)")]
        public float trendMultiplier;

        [Tooltip("Turns before this card activates (e.g. Tech Startup: 3)")]
        public int activationDelay;

        [Header("Employee Stats")]
        public int salaryPerTurn;

        [Tooltip("Base customer bonus when assigned to any business")]
        public int customerBonus;

        [Tooltip("Bonus when assigned to a matching business (e.g. Barista in coffee = +6 vs +3)")]
        public int synergyCustomerBonus;

        [Tooltip("Which business tag triggers the synergy bonus")]
        public string synergyTag;

        [Tooltip("Income multiplier (e.g. Marketing Guru: 1.25)")]
        public float incomeMultiplier;

        [Tooltip("FBI risk added per turn (e.g. Hacker: 10, Dolandirici: 12)")]
        public int fbiRiskPerTurn;

        [Tooltip("Illegal income per turn (e.g. Dolandirici: 120)")]
        public int illegalIncomePerTurn;

        [Tooltip("Prevents employee transfers (Sadik Mudur)")]
        public bool preventsTransfer;

        [Tooltip("Tax reduction factor (e.g. Muhasebeci: 0.5)")]
        public float taxReduction;

        [Header("Active Ability")]
        [Tooltip("Ability display name, e.g. Latte Festivali")]
        public string activeAbilityName;

        [TextArea(2, 3)]
        public string activeAbilityDesc;

        public ActiveAbilityType activeAbilityType;

        [Tooltip("Primary ability parameter (multiplier or flat bonus)")]
        public float abilityParam1;

        [Tooltip("Secondary ability parameter")]
        public int abilityParam2;

        [Header("Action Card")]
        public ActionEffectType actionEffectType;

        [Tooltip("Flat value (+150 money, +3 customers, etc.)")]
        public int actionValue;

        [Tooltip("Multiplier (e.g. Viral Pazarlama: x2)")]
        public float actionMultiplier;

        [Tooltip("FBI risk from this action (e.g. Sahte Yorumlar: 12)")]
        public int actionFBIRisk;

        [Tooltip("Duration in turns (e.g. Yatirimci Sunumu: 3)")]
        public int actionDuration;

        [Header("Upgrade")]
        public UpgradeEffectType upgradeEffectType;
        public float upgradeValue;

        [Tooltip("Whether this upgrade affects all businesses")]
        public bool isGlobalUpgrade;

        [Tooltip("Employee slots closed by this upgrade (e.g. Otomasyon: 1)")]
        public int closedEmployeeSlots;

        [Header("Event")]
        [Tooltip("Duration in turns (1-2)")]
        public int eventDuration;

        public EventEffectType eventEffectType;
        public float eventMultiplier;

        [Tooltip("Which card tags are affected by this event")]
        public string[] affectedTags;
    }

    public enum ActiveAbilityType
    {
        None,
        MultiplyCustomers,
        AddCustomers,
        MultiplyIncome,
        StealCustomers,
        AddCustomersAll,
        NullifyTax,
        BonusIncomeWithPenalty,
        AddCustomersToAll
    }

    public enum ActionEffectType
    {
        None,
        AddCustomers,
        AddMoney,
        MultiplyAllCustomers,
        CloseRivalBusiness,
        AddCustomersWithFBI,
        StealCustomers,
        DisableRival,
        MoneyWithDebt,
        DrawAndPlayEmployee,
        SacrificeForMoney
    }

    public enum UpgradeEffectType
    {
        None,
        IncomePercent,
        IncomePercentWithSlotLoss,
        GlobalCustomerBonus,
        GlobalCustomerFlat,
        ReduceFBIRisk,
        ExtraAction
    }

    public enum EventEffectType
    {
        None,
        TagCustomerBoost,
        AllIncomeReduction,
        TagDoubleEffect,
        TagCustomerPenalty,
        TagDoubleEffectFinance,
        HighFBIPenalty
    }
}
