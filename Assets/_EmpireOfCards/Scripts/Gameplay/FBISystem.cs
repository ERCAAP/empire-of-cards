using System;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// FBI risk and raid mechanic. Illegal activities increase risk; raids penalize the player.
    /// </summary>
    public class FBISystem : MonoBehaviour
    {
        // --- Configuration ---
        [Header("FBI Settings")]
        [SerializeField] private float securityReduction = 0.25f;

        // --- Runtime State ---
        [Header("State (Read Only)")]
        [SerializeField] private float currentRisk;
        [SerializeField] private bool hasSecuritySystem;

        // --- Manager References ---
        [Header("References")]
        [SerializeField] private BoardManager boardManager;

        // --- Events ---
        public event Action<float> OnRiskChanged;
        public event Action OnRaidOccurred;
        public event Action OnRaidAvoided;

        // --- Properties ---
        public float CurrentRisk => currentRisk;
        public bool HasSecuritySystem => hasSecuritySystem;

        /// <summary>
        /// Adds FBI risk. If a security system is active, the amount is reduced.
        /// </summary>
        public void AddRisk(float amount)
        {
            if (amount <= 0f)
                return;

            float effectiveAmount = amount;

            if (hasSecuritySystem)
            {
                effectiveAmount *= (1f - securityReduction);
            }

            currentRisk = Mathf.Clamp01(currentRisk + effectiveAmount);
            GameManager.Instance.SetFBIRisk(currentRisk);
            OnRiskChanged?.Invoke(currentRisk);
        }

        /// <summary>
        /// Rolls a random check against the current risk level.
        /// Returns true if a raid occurs.
        /// </summary>
        public bool CheckForRaid()
        {
            if (currentRisk <= 0f)
                return false;

            float roll = UnityEngine.Random.Range(0f, 1f);
            bool raidTriggered = roll < currentRisk;

            if (raidTriggered)
            {
                ExecuteRaid();
            }
            else if (currentRisk > 0f)
            {
                OnRaidAvoided?.Invoke();
            }

            return raidTriggered;
        }

        /// <summary>
        /// Executes a raid: applies a money penalty, fires one illegal employee, and resets risk.
        /// </summary>
        public void ExecuteRaid()
        {
            Debug.Log("[FBISystem] FBI Raid! Applying penalties.");

            // Money penalty: lose 20% of current money
            int penalty = Mathf.RoundToInt(GameManager.Instance.PlayerMoney * 0.2f);
            if (penalty > 0)
            {
                GameManager.Instance.SpendMoney(penalty);
            }

            // Remove one illegal employee if the board manager is available
            if (boardManager != null)
            {
                RemoveIllegalEmployee();
            }

            // Reset risk after a raid
            currentRisk = 0f;
            GameManager.Instance.SetFBIRisk(currentRisk);
            OnRiskChanged?.Invoke(currentRisk);
            OnRaidOccurred?.Invoke();
        }

        /// <summary>
        /// Returns the current risk as a percentage (0-100).
        /// </summary>
        public float GetRiskPercent()
        {
            return currentRisk * 100f;
        }

        /// <summary>
        /// Enables or disables the security system that reduces incoming risk.
        /// </summary>
        public void SetSecurityActive(bool active)
        {
            hasSecuritySystem = active;
        }

        private void RemoveIllegalEmployee()
        {
            // Search through all businesses for an employee with the Illegal tag
            var businesses = boardManager.PlayerBusinesses;

            for (int b = 0; b < businesses.Count; b++)
            {
                var business = businesses[b];
                if (business.isClosed)
                    continue;

                for (int e = 0; e < business.employees.Count; e++)
                {
                    var employee = business.employees[e];
                    if (employee != null && HasIllegalTag(employee))
                    {
                        Debug.Log($"[FBISystem] Removing illegal employee: {employee.cardName}");
                        boardManager.RemoveEmployee(b, e);
                        return;
                    }
                }
            }
        }

        private bool HasIllegalTag(CardData card)
        {
            if (card.tags == null)
                return false;

            foreach (string tag in card.tags)
            {
                if (tag == CardTag.Illegal.ToString())
                    return true;
            }

            return false;
        }
    }
}
