using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public partial class ActionCardResolver
    {
        private bool TryResolveLegacyEffect(GameManager gm, CardData card)
        {
            switch (card.actionEffectType)
            {
                case ActionEffectType.MultiplyAllCustomers:
                    ResolveMultiplyAllCustomers(gm, card);
                    return true;

                case ActionEffectType.AddCustomersWithFBI:
                    ResolveAddCustomersToRandom(gm, card);
                    return true;

                case ActionEffectType.StealCustomersHalfIncome:
                    ResolveStealCustomersHalfIncome(gm, card);
                    return true;

                case ActionEffectType.DrawAndPlayEmployee:
                    ResolveDrawAndPlayEmployee(gm);
                    return true;

                case ActionEffectType.SacrificeBusiness:
                    ResolveSacrificeBusiness(gm);
                    return true;
            }

            return false;
        }

        private void ResolveMultiplyAllCustomers(GameManager gm, CardData card)
        {
            if (gm.BoardManager == null)
                return;

            var businesses = gm.BoardManager.PlayerBusinesses;
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed || businesses[i].businessCard == null)
                    continue;

                int baseCustomers = businesses[i].businessCard.customersPerTurn;
                int bonusCustomers = Mathf.RoundToInt(baseCustomers * (card.actionMultiplier - 1f));
                if (bonusCustomers > 0)
                    gm.BoardManager.AddCustomersAttracted(i, bonusCustomers);
            }
        }

        private void ResolveStealCustomersHalfIncome(GameManager gm, CardData card)
        {
            if (gm.EconomyManager != null)
            {
                int incomeEstimate = gm.EconomyManager.GrossIncome;
                int sacrifice = Mathf.RoundToInt(incomeEstimate * card.actionIncomeSacrifice);
                if (sacrifice > 0)
                    gm.SpendMoney(sacrifice);
            }

            gm.RivalAI?.ApplyCustomerPenalty(card.actionValue);
        }

        private void ResolveDrawAndPlayEmployee(GameManager gm)
        {
            if (gm.DeckManager == null)
                return;

            CardData employee = gm.DeckManager.DrawRandomEmployee();
            if (employee != null)
                Debug.Log($"[ActionCardResolver] Drew random employee: {employee.cardName}");
        }

        private void ResolveSacrificeBusiness(GameManager gm)
        {
            if (gm.BoardManager == null)
                return;

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
        }
    }
}
