using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.Staff;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Subscribes to EventBus.OnActionExecuted and resolves each action card's
    /// effect based on its ActionEffectType (GDD Section 3.3).
    /// Created by ManagerFactory and lives as a child of the GameManager object.
    /// Status: partial legacy surface. Some effects still reflect the older
    /// business-centric prototype and should be treated as compatibility logic
    /// until the venture-first action taxonomy fully replaces them.
    /// </summary>
    public class ActionCardResolver : MonoBehaviour
    {
        private void OnEnable()
        {
            EventBus.OnActionExecuted += ResolveAction;
        }

        private void OnDisable()
        {
            EventBus.OnActionExecuted -= ResolveAction;
        }

        private void ResolveAction(CardData card)
        {
            if (card == null || card.cardType != CardType.Action) return;

            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogError("[ActionCardResolver] GameManager.Instance is null.");
                return;
            }

            switch (card.actionEffectType)
            {
                // Active compatibility effect. Still used as a generic demand bump.
                case ActionEffectType.AddCustomersToRandom:
                    ResolveAddCustomersToRandom(gm, card);
                    break;

                // Active effect. Works cleanly in the v4 economy layer.
                case ActionEffectType.AddMoneyInstant:
                    gm.GainMoney(card.actionValue);
                    break;

                // Partial legacy effect. Scales older business output assumptions.
                case ActionEffectType.MultiplyAllCustomers:
                    ResolveMultiplyAllCustomers(gm, card);
                    break;

                // Partial legacy effect. Still routed through rival pressure hooks.
                case ActionEffectType.CloseRivalWeakestBusiness:
                    if (gm.RivalAI != null)
                        gm.RivalAI.CloseWeakestBusiness(1);
                    break;

                // Legacy FBI action. The FBI subsystem is no longer active in v4,
                // so this falls back to a plain customer swing.
                case ActionEffectType.AddCustomersWithFBI:
                    ResolveAddCustomersToRandom(gm, card);
                    break;

                // Partial legacy pressure effect. Keeps high-level risk/reward feel.
                case ActionEffectType.StealCustomersHalfIncome:
                    ResolveStealCustomersHalfIncome(gm, card);
                    break;

                // Active sabotage effect.
                case ActionEffectType.DisableRivalOneTurn:
                    if (gm.RivalAI != null)
                        gm.RivalAI.DisableProductionOneTurn();
                    break;

                // Active debt effect.
                case ActionEffectType.MoneyNowPayLater:
                    gm.GainMoney(card.actionValue);
                    if (gm.EconomyManager != null)
                        gm.EconomyManager.StartInvestorDebt(card.actionDebtDuration, card.actionDebtPercent);
                    break;

                // Legacy helper effect. Still useful until hiring loops become systemic.
                case ActionEffectType.DrawAndPlayEmployee:
                    ResolveDrawAndPlayEmployee(gm);
                    break;

                // Legacy business-centric effect retained for compatibility only.
                case ActionEffectType.SacrificeBusiness:
                    ResolveSacrificeBusiness(gm);
                    break;

                // Active staff pressure effect.
                case ActionEffectType.Overtime:
                    ResolveOvertime(gm);
                    break;

                default:
                    Debug.LogWarning($"[ActionCardResolver] Unknown ActionEffectType: {card.actionEffectType}");
                    break;
            }

            Debug.Log($"[ActionCardResolver] Resolved: {card.cardName} ({card.actionEffectType})");
        }

        // ----------------------------------------------------------------
        // Individual Effect Resolvers
        // ----------------------------------------------------------------

        /// <summary>
        /// Flyer: +actionValue customers to a random active business.
        /// </summary>
        private void ResolveAddCustomersToRandom(GameManager gm, CardData card)
        {
            if (gm.BoardManager == null) return;

            var businesses = gm.BoardManager.PlayerBusinesses;
            var activeIndices = new List<int>();

            for (int i = 0; i < businesses.Count; i++)
            {
                if (!businesses[i].isClosed)
                    activeIndices.Add(i);
            }

            if (activeIndices.Count == 0)
            {
                Debug.Log("[ActionCardResolver] No active businesses to add customers to.");
                return;
            }

            int randomIdx = activeIndices[Random.Range(0, activeIndices.Count)];
            gm.BoardManager.AddCustomersAttracted(randomIdx, card.actionValue);
            Debug.Log($"[ActionCardResolver] Added {card.actionValue} customers to business {randomIdx}.");
        }

        /// <summary>
        /// Viral Marketing: all businesses get customers multiplied this turn.
        /// Applies the multiplier as bonus customers to each active business.
        /// </summary>
        private void ResolveMultiplyAllCustomers(GameManager gm, CardData card)
        {
            if (gm.BoardManager == null) return;

            var businesses = gm.BoardManager.PlayerBusinesses;
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed) continue;
                var biz = businesses[i];
                if (biz.businessCard == null) continue;

                // Add bonus customers equal to (base customers * (multiplier - 1))
                // e.g., x2 means multiplier=2.0, bonus = base * 1.0 = doubling
                int baseCustomers = biz.businessCard.customersPerTurn;
                int bonusCustomers = Mathf.RoundToInt(baseCustomers * (card.actionMultiplier - 1f));
                if (bonusCustomers > 0)
                    gm.BoardManager.AddCustomersAttracted(i, bonusCustomers);
            }
        }

        /// <summary>
        /// Price Slashing: sacrifice actionIncomeSacrifice% of income,
        /// steal actionValue customers from rival.
        /// </summary>
        private void ResolveStealCustomersHalfIncome(GameManager gm, CardData card)
        {
            // Sacrifice income: deduct a percentage of current money as penalty
            if (gm.EconomyManager != null)
            {
                int incomeEstimate = gm.EconomyManager.GrossIncome;
                int sacrifice = Mathf.RoundToInt(incomeEstimate * card.actionIncomeSacrifice);
                if (sacrifice > 0)
                    gm.SpendMoney(sacrifice);
            }

            // Steal customers from rival
            if (gm.RivalAI != null)
                gm.RivalAI.ApplyCustomerPenalty(card.actionValue);
        }

        /// <summary>
        /// Emergency Hire: draw a random employee from the deck and add to hand.
        /// </summary>
        private void ResolveDrawAndPlayEmployee(GameManager gm)
        {
            if (gm.DeckManager == null) return;

            CardData employee = gm.DeckManager.DrawRandomEmployee();
            if (employee != null)
            {
                Debug.Log($"[ActionCardResolver] Drew random employee: {employee.cardName}");
            }
            else
            {
                Debug.Log("[ActionCardResolver] No employee cards available to draw.");
            }
        }

        /// <summary>
        /// Liquidation Sale: sacrifice the first active business for 2x its buy cost.
        /// </summary>
        private void ResolveSacrificeBusiness(GameManager gm)
        {
            if (gm.BoardManager == null) return;

            var businesses = gm.BoardManager.PlayerBusinesses;
            int targetIndex = -1;

            for (int i = 0; i < businesses.Count; i++)
            {
                if (!businesses[i].isClosed && businesses[i].businessCard != null)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex < 0)
            {
                Debug.Log("[ActionCardResolver] No active businesses to sacrifice.");
                return;
            }

            int value = businesses[targetIndex].businessCard.buyCost * 2;
            gm.GainMoney(value);
            gm.BoardManager.RemoveBusiness(targetIndex);
            EventBus.BusinessClosed(targetIndex);
            Debug.Log($"[ActionCardResolver] Sacrificed business {targetIndex} for {value} money.");
        }

        /// <summary>
        /// Overtime (GDD 6.3): +50% capacity this turn, +2 fatigue to all staff.
        /// 3 consecutive overtime turns triggers staff strike.
        /// </summary>
        private void ResolveOvertime(GameManager gm)
        {
            if (gm.StaffStateSystem == null)
            {
                Debug.LogWarning("[ActionCardResolver] StaffStateSystem is null, cannot apply overtime.");
                return;
            }

            gm.StaffStateSystem.ApplyOvertime();
            EventBus.OvertimeApplied();
            Debug.Log("[ActionCardResolver] Overtime applied: +50% capacity, +2 fatigue to all staff.");
        }
    }
}
