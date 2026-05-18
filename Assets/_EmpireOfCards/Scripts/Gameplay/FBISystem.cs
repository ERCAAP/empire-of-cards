using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// FBI risk and raid mechanic matching GDD Section 9.3.
    ///
    /// - Risk accumulates from illegal cards (Hacker +10%, Dolandirici +12%, etc.)
    /// - Each turn end: random(1-100) less than risk% => RAID!
    /// - Raid: 300 ceza + en pahali illegal calisan kovulur + risk sifirlanir
    /// - Guvenlik Sistemi: risk artisi %50 azalir (50% reduction on incoming risk, NOT flat -%25)
    /// - Guvenli Suc combo: further 50% reduction stacks multiplicatively
    /// </summary>
    public class FBISystem : MonoBehaviour
    {
        // --- Configuration ---
        [Header("FBI Settings")]
        [SerializeField] private GameBalanceData balanceData;

        // --- Runtime State ---
        [Header("State (Read Only)")]
        [SerializeField] private float currentRisk;             // 0-100 percentage
        [SerializeField] private bool hasSecuritySystem;        // Guvenlik Sistemi upgrade
        [SerializeField] private bool hasGuvenliSucCombo;       // Guvenli Suc combo active

        // --- Manager References ---
        [Header("References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ComboSystem comboSystem;

        // --- Properties ---
        public float CurrentRisk => currentRisk;
        public bool HasSecuritySystem => hasSecuritySystem;

        // ----------------------------------------------------------------
        // Risk Management
        // ----------------------------------------------------------------

        /// <summary>
        /// Adds FBI risk from an illegal source. Amount is in percentage points (e.g. 10 = +10%).
        ///
        /// Reductions are applied multiplicatively:
        /// - Guvenlik Sistemi: 50% reduction on incoming risk
        /// - Guvenli Suc combo: additional 50% reduction
        ///
        /// Example: Hacker +10%, with security = +5%, with security+combo = +2.5%
        /// </summary>
        public void AddRisk(float amount)
        {
            if (amount <= 0f) return;

            float effectiveAmount = amount;

            // Guvenlik Sistemi: 50% reduction (NOT flat -%25, but 50% of incoming risk)
            if (hasSecuritySystem)
            {
                effectiveAmount *= 0.5f;
            }

            // Guvenli Suc combo: further 50% reduction (stacks multiplicatively)
            if (hasGuvenliSucCombo)
            {
                effectiveAmount *= 0.5f;
            }

            currentRisk = Mathf.Clamp(currentRisk + effectiveAmount, 0f, 100f);

            GameManager gm = GameManager.Instance;
            if (gm != null) gm.SetFBIRisk(currentRisk / 100f); // GameManager stores 0-1

            EventBus.FBIRiskUpdated(currentRisk);
        }

        /// <summary>
        /// Directly sets the risk level. Used for initialization/reset.
        /// </summary>
        public void SetRisk(float risk)
        {
            currentRisk = Mathf.Clamp(risk, 0f, 100f);

            GameManager gm = GameManager.Instance;
            if (gm != null) gm.SetFBIRisk(currentRisk / 100f);

            EventBus.FBIRiskUpdated(currentRisk);
        }

        // ----------------------------------------------------------------
        // Raid Check (GDD Section 9.3)
        // ----------------------------------------------------------------

        /// <summary>
        /// Rolls random(1-100). If result is less than currentRisk, raid occurs.
        /// Returns true if a raid was triggered.
        /// </summary>
        public bool CheckForRaid()
        {
            if (currentRisk <= 0f)
            {
                EventBus.FBIRaidWasAvoided();
                return false;
            }

            int roll = UnityEngine.Random.Range(1, 101); // 1-100 inclusive
            bool raidTriggered = roll < Mathf.RoundToInt(currentRisk);

            if (raidTriggered)
            {
                ExecuteRaid();
            }
            else
            {
                EventBus.FBIRaidWasAvoided();
            }

            return raidTriggered;
        }

        // ----------------------------------------------------------------
        // Raid Execution (GDD Section 9.3)
        // ----------------------------------------------------------------

        /// <summary>
        /// Executes an FBI raid:
        /// 1. Apply 300 money penalty
        /// 2. Remove the MOST EXPENSIVE illegal employee (by buyCost)
        /// 3. Reset risk to 0
        /// </summary>
        public void ExecuteRaid()
        {
            Debug.Log("[FBISystem] FBI RAID! Applying penalties.");

            GameManager gm = GameManager.Instance;
            if (gm == null) return;

            // Step 1: 300 penalty (from GDD, also in balanceData)
            int penalty = balanceData != null ? balanceData.fbiRaidPenalty : Constants.FBI_RAID_PENALTY;
            gm.SpendMoney(penalty);

            EventBus.FBIRaidOccurred(penalty);

            // Step 2: Remove most expensive illegal employee
            if (boardManager != null)
            {
                RemoveMostExpensiveIllegalEmployee();
            }

            // Step 3: Reset risk to 0
            currentRisk = 0f;
            gm.SetFBIRisk(0f);
            EventBus.FBIRiskUpdated(0f);
        }

        // ----------------------------------------------------------------
        // Per-Turn Risk Accumulation
        // ----------------------------------------------------------------

        /// <summary>
        /// Scans all illegal employees on the board and adds their per-turn risk.
        /// Called once per turn during the Resolve phase.
        ///
        /// Each illegal employee contributes fbiRiskPerTurn:
        /// - Hacker: +10%/turn
        /// - Dolandirici: +12%/turn
        /// etc.
        ///
        /// Also adds extra FBI risk from active combos (Yeralti: +8%).
        /// </summary>
        public void AccumulateRiskFromBoard()
        {
            if (boardManager == null) return;

            // Update security system status from board
            UpdateSecurityStatus();

            // Update combo status
            UpdateComboStatus();

            // Scan all illegal employees
            var illegalEmployees = boardManager.GetAllIllegalEmployees();

            foreach (var (employee, bizIndex, empIndex) in illegalEmployees)
            {
                if (employee.fbiRiskPerTurn > 0)
                {
                    AddRisk(employee.fbiRiskPerTurn);
                }
            }

            // Add combo-sourced FBI risk (Yeralti combo: +8%)
            if (comboSystem != null)
            {
                int comboRisk = comboSystem.GetExtraFBIRisk();
                if (comboRisk > 0)
                {
                    AddRisk(comboRisk);
                }
            }
        }

        // ----------------------------------------------------------------
        // Security & Combo Status
        // ----------------------------------------------------------------

        /// <summary>
        /// Checks if any Guvenlik Sistemi upgrade is on the board.
        /// Updates the hasSecuritySystem flag.
        /// </summary>
        private void UpdateSecurityStatus()
        {
            hasSecuritySystem = false;

            if (boardManager == null) return;

            // Check global upgrades for ReduceFBIRisk type
            foreach (var upgrade in boardManager.GlobalUpgrades)
            {
                if (upgrade != null && upgrade.upgradeEffectType == UpgradeEffectType.ReduceFBIRisk)
                {
                    hasSecuritySystem = true;
                    return;
                }
            }

            // Check per-business upgrades
            foreach (var business in boardManager.PlayerBusinesses)
            {
                if (business.isClosed) continue;
                foreach (var upgrade in business.upgrades)
                {
                    if (upgrade != null && upgrade.upgradeEffectType == UpgradeEffectType.ReduceFBIRisk)
                    {
                        hasSecuritySystem = true;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the Guvenli Suc combo is active.
        /// The combo ID convention is checked against comboSystem.
        /// </summary>
        private void UpdateComboStatus()
        {
            hasGuvenliSucCombo = false;

            if (comboSystem == null) return;

            // Check for "Guvenli Suc" or similar combo
            foreach (var combo in comboSystem.ActiveCombos)
            {
                if (combo == null) continue;

                // Check if combo has the tag/name pattern for the safe crime combo
                // ComboData doesn't have a type field, so we check by ID convention
                if (combo.comboId != null && combo.comboId.Contains("GuvenliSuc"))
                {
                    hasGuvenliSucCombo = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Enables or disables the security system manually.
        /// </summary>
        public void SetSecurityActive(bool active)
        {
            hasSecuritySystem = active;
        }

        // ----------------------------------------------------------------
        // Internal: Find and Remove Most Expensive Illegal Employee
        // ----------------------------------------------------------------

        /// <summary>
        /// Finds the most expensive illegal employee (by buyCost) and removes them.
        /// GDD says: "en pahali illegal calisan kovulur" on raid.
        /// </summary>
        private void RemoveMostExpensiveIllegalEmployee()
        {
            if (boardManager == null) return;

            var illegals = boardManager.GetAllIllegalEmployees();

            if (illegals.Count == 0)
            {
                Debug.Log("[FBISystem] No illegal employees to remove.");
                return;
            }

            // Find the most expensive one
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
                Debug.Log($"[FBISystem] Removing most expensive illegal employee: {targetEmployee.cardName} (cost: {mostExpensiveCost})");
                boardManager.RemoveEmployee(targetBizIndex, targetEmpIndex);
                EventBus.EmployeeLeft(targetEmployee, targetBizIndex);
            }
        }

        // ----------------------------------------------------------------
        // Queries
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the current risk as a percentage (0-100).
        /// </summary>
        public float GetRiskPercent()
        {
            return currentRisk;
        }

        /// <summary>
        /// Returns true if risk is above 0 (any illegal activity exists).
        /// </summary>
        public bool HasRisk()
        {
            return currentRisk > 0f;
        }

        /// <summary>
        /// Resets FBI state for a new run.
        /// </summary>
        public void Reset()
        {
            currentRisk = 0f;
            hasSecuritySystem = false;
            hasGuvenliSucCombo = false;

            GameManager gm = GameManager.Instance;
            if (gm != null) gm.SetFBIRisk(0f);
        }
    }
}
