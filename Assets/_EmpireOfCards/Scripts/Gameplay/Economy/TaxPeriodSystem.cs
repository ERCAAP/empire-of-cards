using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay.Economy
{
    public class TaxPeriodSystem
    {
        private int accumulatedNetProfit;
        private int unpaidTaxDebt;
        private int unpaidTaxTurns;

        public int AccumulatedNetProfit => accumulatedNetProfit;
        public int UnpaidTaxDebt => unpaidTaxDebt;
        public int UnpaidTaxTurns => unpaidTaxTurns;
        public bool HasTaxDebt => unpaidTaxDebt > 0;

        public void TrackProfit(int netIncome)
        {
            accumulatedNetProfit += netIncome;
        }

        public bool IsTaxPeriod(int currentTurn)
        {
            return currentTurn > 0 && (currentTurn % Constants.TAX_PERIOD_INTERVAL) == 0;
        }

        public int CalculatePeriodTax()
        {
            if (accumulatedNetProfit <= 0) return 0;
            return Mathf.RoundToInt(accumulatedNetProfit * Constants.TAX_PERIOD_RATE);
        }

        public TaxPeriodResult ProcessTaxPeriod(int currentTurn, int playerMoney)
        {
            var result = new TaxPeriodResult();
            result.taxOwed = CalculatePeriodTax();

            if (result.taxOwed <= 0)
            {
                accumulatedNetProfit = 0;
                result.paid = true;
                return result;
            }

            if (playerMoney >= result.taxOwed)
            {
                result.amountPaid = result.taxOwed;
                result.paid = true;
                unpaidTaxDebt = 0;
                unpaidTaxTurns = 0;
            }
            else
            {
                result.amountPaid = playerMoney;
                int remaining = result.taxOwed - playerMoney;
                unpaidTaxDebt += remaining;
                result.paid = false;
                result.newDebtAmount = remaining;
            }

            accumulatedNetProfit = 0;

            EventBus.TaxPeriodProcessed(result.taxOwed, result.amountPaid);
            Debug.Log($"[TaxPeriodSystem] Tax period at turn {currentTurn}: owed={result.taxOwed}, paid={result.amountPaid}, debt={unpaidTaxDebt}");

            return result;
        }

        public int TickTaxDebt()
        {
            if (unpaidTaxDebt <= 0) return 0;

            unpaidTaxTurns++;

            // Apply interest on unpaid tax debt
            int interest = Mathf.RoundToInt(unpaidTaxDebt * Constants.TAX_DEBT_INTEREST_RATE);
            unpaidTaxDebt += interest;

            // Check for audit trigger
            if (unpaidTaxTurns >= Constants.TAX_DEBT_AUDIT_THRESHOLD)
            {
                EventBus.TaxAuditTriggered(unpaidTaxDebt);
                Debug.Log($"[TaxPeriodSystem] Tax audit triggered! Unpaid debt: {unpaidTaxDebt}");
            }

            return interest;
        }

        public void PayTaxDebt(int amount)
        {
            int payment = Mathf.Min(amount, unpaidTaxDebt);
            unpaidTaxDebt -= payment;
            if (unpaidTaxDebt <= 0)
            {
                unpaidTaxDebt = 0;
                unpaidTaxTurns = 0;
            }
        }

        public void Reset()
        {
            accumulatedNetProfit = 0;
            unpaidTaxDebt = 0;
            unpaidTaxTurns = 0;
        }
    }

    public class TaxPeriodResult
    {
        public int taxOwed;
        public int amountPaid;
        public bool paid;
        public int newDebtAmount;
    }
}
