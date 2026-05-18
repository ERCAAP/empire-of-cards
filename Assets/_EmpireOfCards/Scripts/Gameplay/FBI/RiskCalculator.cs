using UnityEngine;
using EmpireOfCards.Core;

namespace EmpireOfCards.Gameplay.FBI
{
    /// <summary>
    /// Pure risk calculation logic extracted from FBISystem.
    /// Handles risk addition with security/combo reductions and queries.
    /// </summary>
    public class RiskCalculator
    {
        private float currentRisk;
        private bool hasSecuritySystem;
        private bool hasGuvenliSucCombo;

        public float CurrentRisk => currentRisk;
        public bool HasSecuritySystem => hasSecuritySystem;

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

            float effectiveAmount = ApplySecurityReduction(amount);
            currentRisk = Mathf.Clamp(currentRisk + effectiveAmount, 0f, 100f);

            SyncToGameManager();
            EventBus.FBIRiskUpdated(currentRisk);
        }

        /// <summary>
        /// Applies security system and combo reductions to incoming risk.
        /// Returns the effective risk amount after all reductions.
        /// </summary>
        public float ApplySecurityReduction(float amount)
        {
            float effective = amount;

            // Guvenlik Sistemi: 50% reduction (NOT flat -%25, but 50% of incoming risk)
            if (hasSecuritySystem)
            {
                effective *= 0.5f;
            }

            // Guvenli Suc combo: further 50% reduction (stacks multiplicatively)
            if (hasGuvenliSucCombo)
            {
                effective *= 0.5f;
            }

            return effective;
        }

        /// <summary>
        /// Returns the current risk as a percentage (0-100).
        /// </summary>
        public float GetEffectiveRisk()
        {
            return currentRisk;
        }

        /// <summary>
        /// Directly sets the risk level. Used for initialization/reset.
        /// </summary>
        public void SetRisk(float risk)
        {
            currentRisk = Mathf.Clamp(risk, 0f, 100f);
            SyncToGameManager();
            EventBus.FBIRiskUpdated(currentRisk);
        }

        /// <summary>
        /// Resets risk to zero.
        /// </summary>
        public void ResetRisk()
        {
            currentRisk = 0f;
            SyncToGameManager();
            EventBus.FBIRiskUpdated(0f);
        }

        public void SetSecurityActive(bool active)
        {
            hasSecuritySystem = active;
        }

        public void SetGuvenliSucCombo(bool active)
        {
            hasGuvenliSucCombo = active;
        }

        /// <summary>
        /// Returns true if risk is above 0 (any illegal activity exists).
        /// </summary>
        public bool HasRisk()
        {
            return currentRisk > 0f;
        }

        /// <summary>
        /// Full reset for a new run.
        /// </summary>
        public void Reset()
        {
            currentRisk = 0f;
            hasSecuritySystem = false;
            hasGuvenliSucCombo = false;

            GameManager gm = GameManager.Instance;
            if (gm != null) gm.SetFBIRisk(0f);
        }

        private void SyncToGameManager()
        {
            GameManager gm = GameManager.Instance;
            if (gm != null) gm.SetFBIRisk(currentRisk / 100f); // GameManager stores 0-1
        }
    }
}
