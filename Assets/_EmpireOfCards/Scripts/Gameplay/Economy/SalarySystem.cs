using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay.Economy
{
    public class SalaryResult
    {
        public int amountPaid;
        public int moraleChange;
        public int loyaltyChange;
        public float resignRiskIncrease;
    }

    public class SalarySystem
    {
        private SalaryChoice _lastChoice = SalaryChoice.PayOnTime;
        private int _consecutiveDelayTurns;

        public SalaryChoice LastChoice => _lastChoice;
        public int ConsecutiveDelayTurns => _consecutiveDelayTurns;

        public void TrackChoice(SalaryChoice choice)
        {
            _lastChoice = choice;
            if (choice == SalaryChoice.Delay)
                _consecutiveDelayTurns++;
            else
                _consecutiveDelayTurns = 0;
        }

        public void Reset()
        {
            _lastChoice = SalaryChoice.PayOnTime;
            _consecutiveDelayTurns = 0;
        }

        public void RestoreState(SalaryChoice choice, int consecutiveDelayTurns)
        {
            _lastChoice = choice;
            _consecutiveDelayTurns = Mathf.Max(0, consecutiveDelayTurns);
        }

        public SalaryResult ProcessSalary(SalaryChoice choice, int totalSalaries, int playerMoney)
        {
            var result = new SalaryResult();

            switch (choice)
            {
                case SalaryChoice.PayOnTime:
                    result.amountPaid = totalSalaries;
                    result.moraleChange = 0;
                    result.loyaltyChange = 0;
                    result.resignRiskIncrease = 0f;
                    break;

                case SalaryChoice.Delay:
                    result.amountPaid = 0;
                    result.moraleChange = Constants.SALARY_DELAY_MORALE;
                    result.loyaltyChange = 0;
                    result.resignRiskIncrease = Constants.SALARY_DELAY_RESIGN_RISK;
                    break;

                case SalaryChoice.PartialPay:
                    result.amountPaid = Mathf.RoundToInt(totalSalaries * Constants.SALARY_PARTIAL_PAY_RATE);
                    result.moraleChange = Constants.SALARY_PARTIAL_MORALE;
                    result.loyaltyChange = Constants.SALARY_PARTIAL_LOYALTY;
                    result.resignRiskIncrease = 0f;
                    break;

                case SalaryChoice.Advance:
                    result.amountPaid = Mathf.RoundToInt(totalSalaries * Constants.SALARY_ADVANCE_PAY_RATE);
                    result.moraleChange = Constants.SALARY_ADVANCE_MORALE;
                    result.loyaltyChange = Constants.SALARY_ADVANCE_LOYALTY;
                    result.resignRiskIncrease = 0f;
                    break;
            }

            // Clamp to available money if player cannot afford full amount
            if (result.amountPaid > playerMoney && choice != SalaryChoice.Delay)
            {
                result.amountPaid = playerMoney;
            }

            TrackChoice(choice);
            EventBus.SalaryPaid(choice, result.amountPaid);

            return result;
        }
    }
}
