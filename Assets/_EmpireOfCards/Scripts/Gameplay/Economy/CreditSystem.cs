using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay.Economy
{
    public class ActiveCredit
    {
        public CreditType type;
        public int principal;
        public float interestRate;
        public int turnsRemaining;
        public int accumulatedInterest;
    }

    public class CreditSystem
    {
        private readonly List<ActiveCredit> activeCredits = new List<ActiveCredit>();

        public IReadOnlyList<ActiveCredit> ActiveCredits => activeCredits;
        public int ActiveCreditCount => activeCredits.Count;

        public int TotalDebt
        {
            get
            {
                int total = 0;
                for (int i = 0; i < activeCredits.Count; i++)
                {
                    total += activeCredits[i].principal + activeCredits[i].accumulatedInterest;
                }
                return total;
            }
        }

        public bool CanTakeCredit(CreditType type, CompanyTier currentTier)
        {
            switch (type)
            {
                case CreditType.SmallBusiness:
                    return currentTier >= CompanyTier.Trader;
                case CreditType.Medium:
                    return currentTier >= CompanyTier.Entrepreneur;
                case CreditType.LargeInvestment:
                    return currentTier >= CompanyTier.Corporation;
                case CreditType.Emergency:
                    return true;
                default:
                    return false;
            }
        }

        public int GetCreditAmount(CreditType type)
        {
            switch (type)
            {
                case CreditType.SmallBusiness:   return Constants.CREDIT_SMALL_AMOUNT;
                case CreditType.Medium:          return Constants.CREDIT_MEDIUM_AMOUNT;
                case CreditType.LargeInvestment: return Constants.CREDIT_LARGE_AMOUNT;
                case CreditType.Emergency:       return Constants.CREDIT_EMERGENCY_AMOUNT;
                default: return 0;
            }
        }

        public float GetInterestRate(CreditType type)
        {
            switch (type)
            {
                case CreditType.SmallBusiness:   return Constants.CREDIT_SMALL_INTEREST;
                case CreditType.Medium:          return Constants.CREDIT_MEDIUM_INTEREST;
                case CreditType.LargeInvestment: return Constants.CREDIT_LARGE_INTEREST;
                case CreditType.Emergency:       return Constants.CREDIT_EMERGENCY_INTEREST;
                default: return 0f;
            }
        }

        public int GetCreditDuration(CreditType type)
        {
            switch (type)
            {
                case CreditType.SmallBusiness:   return Constants.CREDIT_SMALL_DURATION;
                case CreditType.Medium:          return Constants.CREDIT_MEDIUM_DURATION;
                case CreditType.LargeInvestment: return Constants.CREDIT_LARGE_DURATION;
                case CreditType.Emergency:       return Constants.CREDIT_EMERGENCY_DURATION;
                default: return 0;
            }
        }

        public void TakeCredit(CreditType type)
        {
            int amount = GetCreditAmount(type);
            float rate = GetInterestRate(type);
            int duration = GetCreditDuration(type);

            var credit = new ActiveCredit
            {
                type = type,
                principal = amount,
                interestRate = rate,
                turnsRemaining = duration,
                accumulatedInterest = 0
            };

            activeCredits.Add(credit);
            EventBus.CreditTaken(type, amount);
            Debug.Log($"[CreditSystem] Credit taken: {type}, amount={amount}, rate={rate}, duration={duration}");
        }

        public int TickCredits()
        {
            int totalInterestThisTurn = 0;

            for (int i = activeCredits.Count - 1; i >= 0; i--)
            {
                var credit = activeCredits[i];
                int interest = Mathf.RoundToInt(credit.principal * credit.interestRate);
                credit.accumulatedInterest += interest;
                totalInterestThisTurn += interest;
                credit.turnsRemaining--;

                if (credit.turnsRemaining <= 0)
                {
                    EventBus.CreditRepaid(credit.type);
                    Debug.Log($"[CreditSystem] Credit expired: {credit.type}, total interest paid={credit.accumulatedInterest}");
                    activeCredits.RemoveAt(i);
                }
            }

            return totalInterestThisTurn;
        }

        public void Reset()
        {
            activeCredits.Clear();
        }
    }
}
