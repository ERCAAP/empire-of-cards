using System;
using System.Collections.Generic;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Gameplay.FBI;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// FBI risk and raid mechanic matching GDD Section 9.3.
    /// Coordinator that delegates to RiskCalculator and RaidExecutor.
    ///
    /// - Risk accumulates from illegal cards (Hacker +10%, Fraudster +12%, etc.)
    /// - Each turn end: random(1-100) less than risk% => RAID!
    /// - Raid: 300 penalty + most expensive illegal employee fired + risk reset
    /// - Security System: incoming risk reduced by 50% (NOT flat -25%)
    /// - Safe Crime combo: further 50% reduction stacks multiplicatively
    /// </summary>
    public class FBISystem : MonoBehaviour
    {
        // --- Configuration ---
        [Header("FBI Settings")]
        [SerializeField] private GameBalanceData balanceData;

        // --- Manager References ---
        [Header("References")]
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private ComboSystem comboSystem;

        // --- Extracted Helpers ---
        private RiskCalculator riskCalculator;
        private RaidExecutor raidExecutor;

        // --- Properties ---
        public float CurrentRisk => riskCalculator?.CurrentRisk ?? 0f;
        public bool HasSecuritySystem => riskCalculator?.HasSecuritySystem ?? false;

        /// <summary>
        /// Assigns all dependencies without reflection.
        /// Called by WiringService during bootstrap.
        /// </summary>
        public void Init(GameBalanceData balance, BoardManager board, ComboSystem combo)
        {
            this.balanceData = balance;
            this.boardManager = board;
            this.comboSystem = combo;

            riskCalculator = new RiskCalculator(risk =>
            {
                GameManager gm = GameManager.Instance;
                if (gm != null) gm.SetFBIRisk(risk);
            });
            raidExecutor = new RaidExecutor(balanceData, boardManager, riskCalculator,
                penalty =>
                {
                    GameManager gm = GameManager.Instance;
                    if (gm != null) gm.SpendMoney(penalty);
                });
        }

        // ----------------------------------------------------------------
        // Delegated Risk Methods
        // ----------------------------------------------------------------

        public void AddRisk(float amount) => riskCalculator.AddRisk(amount);
        public void SetRisk(float risk) => riskCalculator.SetRisk(risk);
        public float GetRiskPercent() => riskCalculator.GetEffectiveRisk();
        public bool HasRisk() => riskCalculator.HasRisk();
        public void SetSecurityActive(bool active) => riskCalculator.SetSecurityActive(active);

        // ----------------------------------------------------------------
        // Raid Check (GDD Section 9.3)
        // ----------------------------------------------------------------

        /// <summary>
        /// Rolls random(1-100). If result is less than currentRisk, raid occurs.
        /// Returns true if a raid was triggered.
        /// </summary>
        public bool CheckForRaid()
        {
            float risk = riskCalculator.GetEffectiveRisk();

            if (risk <= 0f)
            {
                EventBus.FBIRaidWasAvoided();
                return false;
            }

            int roll = UnityEngine.Random.Range(1, 101); // 1-100 inclusive
            bool raidTriggered = roll < Mathf.RoundToInt(risk);

            if (raidTriggered)
            {
                raidExecutor.ExecuteRaid();
            }
            else
            {
                EventBus.FBIRaidWasAvoided();
            }

            return raidTriggered;
        }

        // ----------------------------------------------------------------
        // Per-Turn Risk Accumulation
        // ----------------------------------------------------------------

        /// <summary>
        /// Scans all illegal employees on the board and adds their per-turn risk.
        /// Called once per turn during the Resolve phase.
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
                    riskCalculator.AddRisk(employee.fbiRiskPerTurn);
                }
            }

            // Add combo-sourced FBI risk (Yeralti combo: +8%)
            if (comboSystem != null)
            {
                int comboRisk = comboSystem.GetExtraFBIRisk();
                if (comboRisk > 0)
                {
                    riskCalculator.AddRisk(comboRisk);
                }
            }
        }

        // ----------------------------------------------------------------
        // Security & Combo Status (private helpers)
        // ----------------------------------------------------------------

        private void UpdateSecurityStatus()
        {
            bool found = false;

            foreach (var upgrade in boardManager.GlobalUpgrades)
            {
                if (upgrade != null && upgrade.upgradeEffectType == UpgradeEffectType.ReduceFBIRisk)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                foreach (var business in boardManager.PlayerBusinesses)
                {
                    if (business.isClosed) continue;
                    foreach (var upgrade in business.upgrades)
                    {
                        if (upgrade != null && upgrade.upgradeEffectType == UpgradeEffectType.ReduceFBIRisk)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
            }

            riskCalculator.SetSecurityActive(found);
        }

        private void UpdateComboStatus()
        {
            bool active = false;

            if (comboSystem != null)
            {
                foreach (var combo in comboSystem.ActiveCombos)
                {
                    if (combo != null && combo.comboId != null && combo.comboId.Contains("SafeCrime"))
                    {
                        active = true;
                        break;
                    }
                }
            }

            riskCalculator.SetGuvenliSucCombo(active);
        }

        /// <summary>
        /// Resets FBI state for a new run.
        /// </summary>
        public void Reset()
        {
            riskCalculator.Reset();
        }
    }
}
