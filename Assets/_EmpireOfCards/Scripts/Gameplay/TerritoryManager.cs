using System;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Legacy: adapts customer share to visual territory blocks.
    /// GDD v3.0 replaced the 10-territory conquest system with a 100-customer
    /// market pool. This class now maps player/rival customer share into 10
    /// visual blocks for the Board3D market zone display. Win/lose conditions
    /// are checked via customer share in GameManager.EndCurrentTurn(), not here.
    /// </summary>
    public class TerritoryManager : MonoBehaviour
    {
        // --- Configuration ---
        [Header("Market Share Blocks")]
        [SerializeField] private int totalTerritories = 10; // Legacy: 10 visual blocks representing 100 customers
        [SerializeField] private int maxFlipPerTurn = 2; // Smooth transitions: max blocks that can change per turn

        // --- Runtime State ---
        [Header("Market Share Blocks (Read Only)")]
        [Tooltip("0 = unclaimed, 1 = player, 2 = rival")]
        [SerializeField] private int[] territoryOwnership; // Legacy name kept for serialization

        [Header("Cached Counts")]
        [SerializeField] private int playerTerritoryCount; // Number of market share blocks owned by player
        [SerializeField] private int rivalTerritoryCount;  // Number of market share blocks owned by rival

        // --- Properties ---
        public int TotalBlocks => totalTerritories;
        public int PlayerBlockCount => playerTerritoryCount;
        public int RivalBlockCount => rivalTerritoryCount;
        public int UnclaimedBlockCount => totalTerritories - playerTerritoryCount - rivalTerritoryCount;

        // Legacy: kept for backward-compat with external callers
        public int TotalTerritories => totalTerritories;
        public int PlayerTerritoryCount => playerTerritoryCount;
        public int RivalTerritoryCount => rivalTerritoryCount;

        // ----------------------------------------------------------------
        // Initialization
        // ----------------------------------------------------------------

        private void Awake()
        {
            territoryOwnership = new int[totalTerritories];
        }

        /// <summary>
        /// Resets all market share blocks to unclaimed.
        /// </summary>
        public void Reset()
        {
            territoryOwnership = new int[totalTerritories];
            playerTerritoryCount = 0;
            rivalTerritoryCount = 0;

            GameManager gm = GameManager.Instance;
            if (gm != null)
                gm.SetMarketBlocks(0, 0);
            else
                EventBus.MarketBlocksUpdated(0, 0);
        }

        // ----------------------------------------------------------------
        // Main Calculation (adapts customer share to visual blocks)
        // ----------------------------------------------------------------

        /// <summary>
        /// Recalculates market share block ownership based on customer ratios.
        /// 10 blocks proportional to: playerCustomers/totalMarket * 10.
        ///
        /// Smooth changes: at most `maxFlipPerTurn` blocks flip per turn.
        /// Win/lose is checked via customer share in GameManager, not here.
        /// </summary>
        public void CalculateMarketBlocks(int playerCustomers, int rivalCustomers, int totalMarket)
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
            if (gm != null)
                gm.SetMarketBlocks(playerTerritoryCount, rivalTerritoryCount);
            else
                EventBus.MarketBlocksUpdated(playerTerritoryCount, rivalTerritoryCount);

            // Win/lose is checked via customer share in GameManager.EndCurrentTurn().
            // These blocks are visual-only: market share display on Board3D.
        }

        // ----------------------------------------------------------------
        // Smooth Transition
        // ----------------------------------------------------------------

        /// <summary>
        /// Moves current block count towards target by at most maxFlipPerTurn.
        /// </summary>
        private int ApplySmoothTransition(int current, int target)
        {
            int diff = target - current;

            if (Mathf.Abs(diff) <= maxFlipPerTurn)
                return target;

            return current + (diff > 0 ? maxFlipPerTurn : -maxFlipPerTurn);
        }

        /// <summary>
        /// Rebuilds the block ownership array from the final counts.
        /// Player blocks fill from index 0, rival from the end, unclaimed in between.
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
        /// Returns the number of market share blocks owned by the player.
        /// </summary>
        public int GetPlayerBlocks()
        {
            return playerTerritoryCount;
        }

        /// <summary>
        /// Returns the number of market share blocks owned by the rival.
        /// </summary>
        public int GetRivalBlocks()
        {
            return rivalTerritoryCount;
        }

        /// <summary>
        /// Returns the number of unclaimed market share blocks.
        /// </summary>
        public int GetUnclaimedBlocks()
        {
            return totalTerritories - playerTerritoryCount - rivalTerritoryCount;
        }

        /// <summary>
        /// Returns the customer market share as a percentage (0-100).
        /// </summary>
        public float GetMarketSharePercent(int customers, int totalMarket)
        {
            if (totalMarket <= 0) return 0f;
            return ((float)customers / totalMarket) * 100f;
        }

        /// <summary>
        /// Returns the ownership value at a specific block index.
        /// 0 = unclaimed, 1 = player, 2 = rival.
        /// </summary>
        public int GetOwnerAtIndex(int index)
        {
            if (territoryOwnership == null || index < 0 || index >= territoryOwnership.Length)
                return 0;

            return territoryOwnership[index];
        }

        /// <summary>
        /// Returns true if the player owns 60%+ of visual market blocks.
        /// NOTE: Actual win condition uses customer share in GameManager.EndCurrentTurn().
        /// This is visual-only and should not drive game logic.
        /// </summary>
        public bool IsPlayerDominant()
        {
            return playerTerritoryCount >= (totalTerritories * 6 / 10);
        }

        /// <summary>
        /// Returns true if the rival owns 60%+ of visual market blocks.
        /// NOTE: Actual lose condition uses customer share in GameManager.EndCurrentTurn().
        /// This is visual-only and should not drive game logic.
        /// </summary>
        public bool IsRivalDominant()
        {
            return rivalTerritoryCount >= (totalTerritories * 6 / 10);
        }
    }
}
