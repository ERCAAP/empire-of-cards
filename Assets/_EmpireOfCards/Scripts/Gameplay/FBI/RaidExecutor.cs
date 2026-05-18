using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay;

namespace EmpireOfCards.Gameplay.FBI
{
    /// <summary>
    /// Raid consequence logic extracted from FBISystem.
    /// Handles penalty application, employee removal, and risk reset on raid.
    /// Uses a callback to apply money penalties instead of reaching into GameManager.
    /// </summary>
    public class RaidExecutor
    {
        private readonly GameBalanceData balanceData;
        private readonly BoardManager boardManager;
        private readonly RiskCalculator riskCalculator;
        private readonly Action<int> _applyPenalty;

        public RaidExecutor(GameBalanceData balanceData, BoardManager boardManager,
            RiskCalculator riskCalculator, Action<int> applyPenalty = null)
        {
            this.balanceData = balanceData;
            this.boardManager = boardManager;
            this.riskCalculator = riskCalculator;
            this._applyPenalty = applyPenalty;
        }

        /// <summary>
        /// Executes an FBI raid:
        /// 1. Apply 300 money penalty
        /// 2. Remove the MOST EXPENSIVE illegal employee (by buyCost)
        /// 3. Reset risk to 0
        /// </summary>
        public void ExecuteRaid()
        {
            Debug.Log("[FBISystem] FBI RAID! Applying penalties.");

            // Step 1: 300 penalty (from GDD, also in balanceData)
            int penalty = balanceData != null ? balanceData.fbiRaidPenalty : Constants.FBI_RAID_PENALTY;
            _applyPenalty?.Invoke(penalty);

            EventBus.FBIRaidOccurred(penalty);

            // Step 2: Remove most expensive illegal employee
            if (boardManager != null)
            {
                RemoveMostExpensiveIllegalEmployee();
            }

            // Step 3: Reset risk to 0
            riskCalculator.ResetRisk();
        }

        /// <summary>
        /// Finds the most expensive illegal employee (by buyCost) on the board.
        /// Returns null if no illegal employees exist.
        /// </summary>
        public (CardData employee, int bizIndex, int empIndex)? FindMostExpensiveIllegalEmployee()
        {
            if (boardManager == null) return null;

            var illegals = boardManager.GetAllIllegalEmployees();
            if (illegals.Count == 0) return null;

            int mostExpensiveCost = -1;
            int targetBizIndex = -1;
            int targetEmpIndex = -1;
            CardData targetEmployee = null;

            foreach (var (employee, bizIndex, empIndex) in illegals)
            {
                if (employee.buyCost > mostExpensiveCost)
                {
                    mostExpensiveCost = employee.buyCost;
                    targetBizIndex = bizIndex;
                    targetEmpIndex = empIndex;
                    targetEmployee = employee;
                }
            }

            if (targetEmployee != null && targetBizIndex >= 0)
            {
                return (targetEmployee, targetBizIndex, targetEmpIndex);
            }

            return null;
        }

        /// <summary>
        /// Removes the specified employee from the board via BoardManager.
        /// </summary>
        public void RemoveEmployee(int bizIndex, int empIndex)
        {
            if (boardManager == null) return;
            boardManager.RemoveEmployee(bizIndex, empIndex);
        }

        /// <summary>
        /// Finds and removes the most expensive illegal employee.
        /// GDD says: "most expensive illegal employee is fired" on raid.
        /// </summary>
        private void RemoveMostExpensiveIllegalEmployee()
        {
            var target = FindMostExpensiveIllegalEmployee();

            if (target == null)
            {
                Debug.Log("[FBISystem] No illegal employees to remove.");
                return;
            }

            var (employee, bizIndex, empIndex) = target.Value;
            Debug.Log($"[FBISystem] Removing most expensive illegal employee: {employee.cardName} (cost: {employee.buyCost})");
            RemoveEmployee(bizIndex, empIndex);
            EventBus.EmployeeLeft(employee, bizIndex);
        }
    }
}
