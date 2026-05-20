using UnityEngine;
using System.Collections.Generic;
using EmpireOfCards.Data;
using EmpireOfCards.Core;

namespace EmpireOfCards.Bootstrap.Data
{
    /// <summary>
    /// Shared helper methods used by all card definition files.
    /// Manages a static card lookup that is built during creation and
    /// handed back to the orchestrator.
    /// </summary>
    public static class CardHelper
    {
        private static Dictionary<string, CardData> _lookup;

        public static void BeginSession()
        {
            _lookup = new Dictionary<string, CardData>();
        }

        public static Dictionary<string, CardData> EndSession()
        {
            var result = _lookup;
            _lookup = null;
            return result;
        }

        public static CardData FindCard(string id)
        {
            if (_lookup != null && _lookup.TryGetValue(id, out var card)) return card;
            Debug.LogError($"[CardHelper] Card not found: {id}");
            return null;
        }

        // ----------------------------------------------------------------
        // Base card
        // ----------------------------------------------------------------

        public static CardData CreateCard(string id, string name, CardType type, Rarity rarity,
            int cost, string desc, CardTag[] tags)
        {
            var card = ScriptableObject.CreateInstance<CardData>();
            card.cardId = id;
            card.cardName = name;
            card.cardType = type;
            card.rarity = rarity;
            card.buyCost = cost;
            card.playCost = 0;
            card.description = desc;
            card.tags = tags;
            card.name = id; // For debug display
            _lookup[id] = card;
            return card;
        }

        public static CardData CreateV4Card(
            string id,
            string name,
            CardType type,
            CardFamily family,
            VentureType venture,
            SlotType slotType,
            string subSlotId,
            Rarity rarity,
            int cost,
            string desc,
            CardTag[] tags)
        {
            var card = CreateCard(id, name, type, rarity, cost, desc, tags);
            card.cardFamily = family;
            card.ventureType = venture;
            card.targetSlotType = slotType;
            card.targetSubSlotId = subSlotId;
            card.isGeneralCard = venture == VentureType.Diner;
            card.tempEffectDuration = 0;
            card.crisisTags = new string[0];
            card.solutionTags = new string[0];
            card.preferredPressures = new BoardPressureType[0];
            return card;
        }

        // ----------------------------------------------------------------
        // Business
        // ----------------------------------------------------------------

        public static CardData CreateBusiness(string id, string name, Rarity rarity,
            string desc, int cost, int income, int customers, int slots, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Business, rarity, cost, desc, tags);
            c.incomePerTurn = income;
            c.customersPerTurn = customers;
            c.employeeSlots = slots;
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
            return c;
        }

        // ----------------------------------------------------------------
        // Employee
        // ----------------------------------------------------------------

        public static CardData CreateEmployee(string id, string name, Rarity rarity,
            string desc, int salary, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Employee, rarity, 0, desc, tags);
            c.salaryPerTurn = salary;
            c.customerBonus = 0;
            c.synergyCustomerBonus = 0;
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
            return c;
        }

        // ----------------------------------------------------------------
        // Action
        // ----------------------------------------------------------------

        public static CardData CreateAction(string id, string name, Rarity rarity,
            string desc, int cost, ActionEffectType effect, int value, float multiplier,
            int fbiRisk, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Action, rarity, cost, desc, tags);
            c.actionEffectType = effect;
            c.actionValue = value;
            c.actionMultiplier = multiplier;
            c.actionFBIRisk = fbiRisk;
            c.actionDebtDuration = 0;
            c.actionDebtPercent = 0f;
            c.actionIncomeSacrifice = 0f;
            return c;
        }

        // ----------------------------------------------------------------
        // Upgrade
        // ----------------------------------------------------------------

        public static CardData CreateUpgrade(string id, string name, Rarity rarity,
            string desc, int cost, UpgradeEffectType effect, float value, bool isGlobal,
            int closedSlots, int extraActs, CardTag[] tags)
        {
            var c = CreateCard(id, name, CardType.Upgrade, rarity, cost, desc, tags);
            c.upgradeEffectType = effect;
            c.upgradeValue = value;
            c.isGlobalUpgrade = isGlobal;
            c.closedEmployeeSlots = closedSlots;
            c.extraActions = extraActs;
            return c;
        }

        // ----------------------------------------------------------------
        // Event
        // ----------------------------------------------------------------

        public static CardData CreateEvent(string id, string name, Rarity rarity,
            string desc, int duration, EventEffectType effect, float multiplier,
            CardTag[] affected, int customerPenalty, float fbiThreshold)
        {
            var c = CreateCard(id, name, CardType.Event, rarity, 0, desc, new CardTag[0]);
            c.eventEffectType = effect;
            c.eventDuration = duration;
            c.eventMultiplier = multiplier;
            c.affectedTags = affected;
            c.eventCustomerPenalty = customerPenalty;
            c.eventFBIThreshold = fbiThreshold;
            return c;
        }
    }
}
