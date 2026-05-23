using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    public partial class ActionCardResolver
    {
        private bool TryResolveActiveEffect(GameManager gm, CardData card)
        {
            switch (card.actionEffectType)
            {
                case ActionEffectType.AddCustomersToRandom:
                    ResolveAddCustomersToRandom(gm, card);
                    return true;

                case ActionEffectType.AddMoneyInstant:
                    gm.GainMoney(card.actionValue);
                    return true;

                case ActionEffectType.CloseRivalWeakestBusiness:
                    gm.RivalAI?.CloseWeakestBusiness(1);
                    return true;

                case ActionEffectType.DisableRivalOneTurn:
                    gm.RivalAI?.DisableProductionOneTurn();
                    return true;

                case ActionEffectType.MoneyNowPayLater:
                    gm.GainMoney(card.actionValue);
                    gm.EconomyManager?.StartInvestorDebt(card.actionDebtDuration, card.actionDebtPercent);
                    return true;

                case ActionEffectType.Overtime:
                    ResolveOvertime(gm);
                    return true;
            }

            return false;
        }

        private void ResolveAddCustomersToRandom(GameManager gm, CardData card)
        {
            if (gm.BoardManager == null)
                return;

            int chosenIndex = -1;
            int activeCount = 0;
            var businesses = gm.BoardManager.PlayerBusinesses;
            for (int i = 0; i < businesses.Count; i++)
            {
                if (businesses[i].isClosed || businesses[i].businessCard == null)
                    continue;

                activeCount++;
                if (Random.Range(0, activeCount) == 0)
                    chosenIndex = i;
            }

            if (chosenIndex < 0)
            {
                Debug.Log("[ActionCardResolver] No active businesses to add customers to.");
                return;
            }

            gm.BoardManager.AddCustomersAttracted(chosenIndex, card.actionValue);
        }

        private void ResolveOvertime(GameManager gm)
        {
            if (gm.StaffStateSystem == null)
            {
                Debug.LogWarning("[ActionCardResolver] StaffStateSystem is null, cannot apply overtime.");
                return;
            }

            gm.StaffStateSystem.ApplyOvertime();
            EventBus.OvertimeApplied();
        }
    }
}
