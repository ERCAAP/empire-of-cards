using System;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Manages the 10 territory zones (GDD Section 6).
    /// Territories are proportional to customer share.
    /// Player 6+ = WIN, Rival 7+ = LOSE.
    /// Supports smooth ownership changes (territories don't flip all at once).
    /// </summary>
    public class TerritoryManager : MonoBehaviour
    {
        // --- Configuration ---
        [Header("Territory Settings")]
        [SerializeField] private int totalTerritories = 10;
        [SerializeField] private int maxFlipPerTurn = 2; // Smooth transitions: max territories that can change per turn

        // --- Runtime State ---
        [Header("Territory Ownership (Read Only)")]
        [Tooltip("0 = neutral, 1 = player, 2 = rival")]
        [SerializeField] private int[] territoryOwnership;

        [Header("Cached Counts")]
        [SerializeField] private int playerTerritoryCount;
        [SerializeField] private int rivalTerritoryCount;

        // --- Properties ---
        public int TotalTerritories => totalTerritories;
        public int PlayerTerritoryCount => playerTerritoryCount;
        public int RivalTerritoryCount => rivalTerritoryCount;
        public int NeutralTerritoryCount => totalTerritories - playerTerritoryCount - rivalTerritoryCount;

        // ----------------------------------------------------------------
        // Initialization
        // ----------------------------------------------------------------

        private void Awake()
        {
            territoryOwnership = new int[totalTerritories];
        }

        /// <summary>
        /// Resets all territories to neutral.
        /// </summary>
        public void Reset()
        {
            territoryOwnership = new int[totalTerritories];
            playerTerritoryCount = 0;
            rivalTerritoryCount = 0;

            GameManager gm = GameManager.Instance;
            if (gm != null) gm.SetTerritories(0, 0);

            EventBus.TerritoryUpdated(0, 0);
        }

        // ----------------------------------------------------------------
        // Main Calculation (GDD Section 6)
        // ----------------------------------------------------------------

        /// <summary>
        /// Recalculates territory ownership based on customer ratios.
        /// Territories are proportional: playerCustomers/totalMarket * 10 territories.
        ///
        /// Smooth ownership changes: old territories don't flip instantly.
        /// Each turn, at most `maxFlipPerTurn` territories can change hands.
        /// This prevents jarring swings and adds strategic tension.
        /// </summary>
        public void CalculateTerritories(int playerCustomers, int rivalCustomers, int totalMarket)
        {
            if (totalMarket <= 0)
            {
                // No market yet, keep current state
                return;
            }

            // Calculate target territory counts from customer share
            float playerShare = (float)playerCustomers / totalMarket;
            float rivalShare = (float)rivalCustomers / totalMarket;

            int targetPlayerCount = Mathf.RoundToInt(playerShare * totalTerritories);
            int targetRivalCount = Mathf.RoundToInt(rivalShare * totalTerritories);

            // Clamp so totals never exceed available territories
            targetPlayerCount = Mathf.Clamp(targetPlayerCount, 0, totalTerritories);
            targetRivalCount = Mathf.Clamp(targetRivalCount, 0, totalTerritories);

            int combined = targetPlayerCount + targetRivalCount;
            if (combined > totalTerritories)
            {
                // Whoever has more customers gets priority
                if (playerCustomers >= rivalCustomers)
                    targetRivalCount = totalTerritories - targetPlayerCount;
                else
                    targetPlayerCount = totalTerritories - targetRivalCount;
            }

            // Apply smooth transition: move towards targets but limited per turn
            int newPlayerCount = ApplySmoothTransition(playerTerritoryCount, targetPlayerCount);
            int newRivalCount = ApplySmoothTransition(rivalTerritoryCount, targetRivalCount);

            // Final safety clamp
            newPlayerCount = Mathf.Clamp(newPlayerCount, 0, totalTerritories);
            newRivalCount = Mathf.Clamp(newRivalCount, 0, totalTerritories - newPlayerCount);

            // Update the ownership array
            UpdateOwnershipArray(newPlayerCount, newRivalCount);

            // Cache counts
            playerTerritoryCount = newPlayerCount;
            rivalTerritoryCount = newRivalCount;

            // Sync with GameManager
            GameManager gm = GameManager.Instance;
            if (gm != null) gm.SetTerritories(playerTerritoryCount, rivalTerritoryCount);

            EventBus.TerritoryUpdated(playerTerritoryCount, rivalTerritoryCount);

            // Win/lose is now checked via customer share in GameManager.EndCurrentTurn().
            // Territory visuals remain for market share display purposes.
        }

        // ----------------------------------------------------------------
        // Smooth Transition
        // ----------------------------------------------------------------

        /// <summary>
        /// Moves current count towards target by at most maxFlipPerTurn.
        /// </summary>
        private int ApplySmoothTransition(int current, int target)
        {
            int diff = target - current;

            if (Mathf.Abs(diff) <= maxFlipPerTurn)
                return target;

            return current + (diff > 0 ? maxFlipPerTurn : -maxFlipPerTurn);
        }

        /// <summary>
        /// Rebuilds the territory ownership array from the final counts.
        /// Player territories fill from index 0, rival from the end, neutral in between.
        /// </summary>
        private void UpdateOwnershipArray(int playerCount, int rivalCount)
        {
            for (int i = 0; i < totalTerritories; i++)
            {
                if (i < playerCount)
                    territoryOwnership[i] = 1;  // Player
                else if (i >= totalTerritories - rivalCount)
                    territoryOwnership[i] = 2;  // Rival
                else
                    territoryOwnership[i] = 0;  // Neutral
            }
        }

        // ----------------------------------------------------------------
        // Queries
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns the number of territories owned by the player.
        /// </summary>
        public int GetPlayerTerritories()
        {
            return playerTerritoryCount;
        }

        /// <summary>
        /// Returns the number of territories owned by the rival.
        /// </summary>
        public int GetRivalTerritories()
        {
            return rivalTerritoryCount;
        }

        /// <summary>
        /// Returns the number of unclaimed territories.
        /// </summary>
        public int GetNeutralTerritories()
        {
            return totalTerritories - playerTerritoryCount - rivalTerritoryCount;
        }

        /// <summary>
        /// Returns the market share as a percentage (0-100).
        /// </summary>
        public float GetMarketSharePercent(int customers, int totalMarket)
        {
            if (totalMarket <= 0) return 0f;
            return ((float)customers / totalMarket) * 100f;
        }

        /// <summary>
        /// Returns the ownership value at a specific territory index.
        /// 0 = neutral, 1 = player, 2 = rival.
        /// </summary>
        public int GetOwnerAtIndex(int index)
        {
            if (territoryOwnership == null || index < 0 || index >= territoryOwnership.Length)
                return 0;

            return territoryOwnership[index];
        }

        /// <summary>
        /// Checks if the player meets the win condition (6+ territories).
        /// </summary>
        public bool CheckPlayerWin()
        {
            return playerTerritoryCount >= Constants.WIN_TERRITORIES;
        }

        /// <summary>
        /// Checks if the rival meets the win condition (7+ territories = player loses).
        /// </summary>
        public bool CheckRivalWin()
        {
            return rivalTerritoryCount >= Constants.LOSE_TERRITORIES;
        }
    }
}
