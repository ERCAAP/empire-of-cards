using System;
using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Manages the 10 territory zones and market share distribution between player and rival.
    /// </summary>
    public class TerritoryManager : MonoBehaviour
    {
        // --- Configuration ---
        [Header("Territory Settings")]
        [SerializeField] private int totalTerritories = 10;

        // --- Runtime State ---
        [Header("Territory Ownership (Read Only)")]
        [Tooltip("0 = empty, 1 = player, 2 = rival")]
        [SerializeField] private int[] territoryOwnership;

        // --- Events ---
        public event Action<int, int> OnTerritoryChanged;

        // --- Properties ---
        public int TotalTerritories => totalTerritories;

        private void Awake()
        {
            territoryOwnership = new int[totalTerritories];
        }

        /// <summary>
        /// Recalculates territory ownership based on customer counts and total market size.
        /// Territories are proportional: more customers = more territories.
        /// </summary>
        public void CalculateTerritories(int playerCustomers, int rivalCustomers, int totalMarket)
        {
            if (totalMarket <= 0)
            {
                ResetTerritories();
                return;
            }

            float playerShare = GetMarketSharePercent(playerCustomers, totalMarket);
            float rivalShare = GetMarketSharePercent(rivalCustomers, totalMarket);

            int playerCount = Mathf.RoundToInt((playerShare / 100f) * totalTerritories);
            int rivalCount = Mathf.RoundToInt((rivalShare / 100f) * totalTerritories);

            // Clamp so totals never exceed available territories
            int combined = playerCount + rivalCount;
            if (combined > totalTerritories)
            {
                // Whoever has more customers gets priority
                if (playerCustomers >= rivalCustomers)
                    rivalCount = totalTerritories - playerCount;
                else
                    playerCount = totalTerritories - rivalCount;
            }

            playerCount = Mathf.Clamp(playerCount, 0, totalTerritories);
            rivalCount = Mathf.Clamp(rivalCount, 0, totalTerritories - playerCount);

            // Assign ownership to territory array
            for (int i = 0; i < totalTerritories; i++)
            {
                if (i < playerCount)
                    territoryOwnership[i] = 1; // Player
                else if (i < playerCount + rivalCount)
                    territoryOwnership[i] = 2; // Rival
                else
                    territoryOwnership[i] = 0; // Empty
            }

            // Sync with GameManager
            GameManager.Instance.SetTerritories(playerCount, rivalCount);

            OnTerritoryChanged?.Invoke(playerCount, rivalCount);
        }

        /// <summary>
        /// Updates visual territory display. Override or extend for your UI.
        /// </summary>
        public void UpdateTerritoryDisplay()
        {
            // UI binding will be handled by TerritoryUI component.
            // This method is called after CalculateTerritories for any additional refresh logic.
        }

        /// <summary>
        /// Returns the number of territories owned by the player.
        /// </summary>
        public int GetPlayerTerritories()
        {
            int count = 0;
            for (int i = 0; i < territoryOwnership.Length; i++)
            {
                if (territoryOwnership[i] == 1)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Returns the number of territories owned by the rival.
        /// </summary>
        public int GetRivalTerritories()
        {
            int count = 0;
            for (int i = 0; i < territoryOwnership.Length; i++)
            {
                if (territoryOwnership[i] == 2)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Returns the number of unclaimed territories.
        /// </summary>
        public int GetEmptyTerritories()
        {
            int count = 0;
            for (int i = 0; i < territoryOwnership.Length; i++)
            {
                if (territoryOwnership[i] == 0)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Returns the market share as a percentage (0-100).
        /// </summary>
        public float GetMarketSharePercent(int customers, int totalMarket)
        {
            if (totalMarket <= 0)
                return 0f;

            return ((float)customers / totalMarket) * 100f;
        }

        private void ResetTerritories()
        {
            for (int i = 0; i < territoryOwnership.Length; i++)
            {
                territoryOwnership[i] = 0;
            }

            GameManager.Instance.SetTerritories(0, 0);
            OnTerritoryChanged?.Invoke(0, 0);
        }
    }
}
