using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay.Economy
{
    public class InsuranceSystem
    {
        public int CalculateInsuranceCost(int baseSalary, InsuranceType type)
        {
            switch (type)
            {
                case InsuranceType.FullSGK:
                    return Mathf.RoundToInt(baseSalary * Constants.INSURANCE_SGK_MULTIPLIER);
                case InsuranceType.Uninsured:
                    return 0;
                case InsuranceType.DailyWage:
                    return 0;
                default:
                    return 0;
            }
        }

        public int GetLegalRiskPerTurn(InsuranceType type)
        {
            switch (type)
            {
                case InsuranceType.FullSGK:
                    return 0;
                case InsuranceType.Uninsured:
                    return Constants.INSURANCE_UNINSURED_RISK;
                case InsuranceType.DailyWage:
                    return Constants.INSURANCE_DAILY_RISK;
                default:
                    return 0;
            }
        }

        public int CalculateTotalInsuranceCost(
            System.Collections.Generic.IReadOnlyList<ActiveBusiness> businesses,
            System.Func<int, int, InsuranceType> getInsuranceType)
        {
            int totalCost = 0;
            for (int bizIdx = 0; bizIdx < businesses.Count; bizIdx++)
            {
                var biz = businesses[bizIdx];
                if (biz.isClosed) continue;
                for (int empIdx = 0; empIdx < biz.employees.Count; empIdx++)
                {
                    var emp = biz.employees[empIdx];
                    if (emp == null) continue;
                    InsuranceType insType = getInsuranceType(bizIdx, empIdx);
                    totalCost += CalculateInsuranceCost(emp.salaryPerTurn, insType);
                }
            }
            return totalCost;
        }

        public int CalculateTotalLegalRisk(
            System.Collections.Generic.IReadOnlyList<ActiveBusiness> businesses,
            System.Func<int, int, InsuranceType> getInsuranceType)
        {
            int totalRisk = 0;
            for (int bizIdx = 0; bizIdx < businesses.Count; bizIdx++)
            {
                var biz = businesses[bizIdx];
                if (biz.isClosed) continue;
                for (int empIdx = 0; empIdx < biz.employees.Count; empIdx++)
                {
                    var emp = biz.employees[empIdx];
                    if (emp == null) continue;
                    InsuranceType insType = getInsuranceType(bizIdx, empIdx);
                    totalRisk += GetLegalRiskPerTurn(insType);
                }
            }
            return totalRisk;
        }
    }
}
