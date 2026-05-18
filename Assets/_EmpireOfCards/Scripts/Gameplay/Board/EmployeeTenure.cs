using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay.Board
{
    /// <summary>
    /// Handles employee tenure tracking and the leaving mechanic (GDD Section 4.2).
    /// Employees leave after 8+ turns without salary raise. Leave chance:
    /// 20% base + 10% per extra turn over threshold.
    /// </summary>
    public class EmployeeTenure
    {
        /// <summary>
        /// Callback for when an employee needs to be removed from a business.
        /// The BoardManager provides its RemoveEmployee method here so this
        /// class does not need a back-reference to the MonoBehaviour.
        /// </summary>
        public delegate CardData RemoveEmployeeDelegate(int businessIndex, int employeeIndex);

        private readonly RemoveEmployeeDelegate _removeEmployee;

        public EmployeeTenure(RemoveEmployeeDelegate removeEmployee)
        {
            _removeEmployee = removeEmployee;
        }

        /// <summary>
        /// Increments tenure for every employee in the business and checks
        /// whether any of them leave. Iterates backwards for safe removal.
        /// </summary>
        public void TickTenure(int businessIndex, ActiveBusiness business)
        {
            for (int e = business.employeeTenure.Count - 1; e >= 0; e--)
            {
                business.employeeTenure[e]++;

                if (ShouldCheckLeaving(business.employeeTenure[e]))
                {
                    float leaveChance = CalculateLeaveChance(business.employeeTenure[e]);

                    if (UnityEngine.Random.value < leaveChance)
                    {
                        TryRemoveEmployee(businessIndex, business, e);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true when the employee's tenure has reached the leaving threshold.
        /// </summary>
        public bool ShouldCheckLeaving(int tenureTurns)
        {
            return tenureTurns >= Constants.EMPLOYEE_LEAVE_TURN_THRESHOLD;
        }

        /// <summary>
        /// Calculates leave probability: 20% base + 10% per turn over threshold.
        /// </summary>
        public float CalculateLeaveChance(int tenureTurns)
        {
            int overThreshold = tenureTurns - Constants.EMPLOYEE_LEAVE_TURN_THRESHOLD;
            return 0.20f + (overThreshold * 0.10f);
        }

        private void TryRemoveEmployee(int businessIndex, ActiveBusiness business, int employeeIndex)
        {
            CardData leavingEmployee = business.employees[employeeIndex];

            // Loyal Manager prevents transfer/leaving
            if (leavingEmployee != null && leavingEmployee.preventsTransfer)
                return;

            _removeEmployee(businessIndex, employeeIndex);
            Debug.Log($"[EmployeeTenure] Employee '{(leavingEmployee != null ? leavingEmployee.cardName : "?")}' left due to tenure ({business.employeeTenure.Count} turns).");
            EventBus.EmployeeLeft(leavingEmployee, businessIndex);
        }
    }
}
