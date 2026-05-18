using EmpireOfCards.Data;
using UnityEngine;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Handles rival mood tracking and taunt/dialogue selection (GDD Section 8).
    /// </summary>
    public class RivalDialogue
    {
        private readonly RivalData data;
        private string currentMood = "neutral";

        public string CurrentMood => currentMood;

        public RivalDialogue(RivalData data)
        {
            this.data = data;
            this.currentMood = "neutral";
        }

        /// <summary>
        /// Updates rival mood based on territory comparison.
        /// </summary>
        public void UpdateMood(int playerTerritories, int rivalTerritories)
        {
            if (playerTerritories > 5)
                currentMood = "aggressive";
            else if (rivalTerritories > playerTerritories + 2)
                currentMood = "confident";
            else if (playerTerritories > rivalTerritories + 2)
                currentMood = "threatened";
            else
                currentMood = "neutral";
        }

        /// <summary>
        /// Returns a taunt based on the current mood and game state.
        /// Categories: growing, playerGrowing, aggressive, losing, winning.
        /// </summary>
        public string GetTaunt(int playerTerritories, int rivalTerritories)
        {
            if (data == null) return null;

            // Aggressive mood
            if (currentMood == "aggressive"
                && data.aggressiveTaunts != null
                && data.aggressiveTaunts.Length > 0)
                return PickRandom(data.aggressiveTaunts);

            // Winning
            if (rivalTerritories > playerTerritories + 2
                && data.winningTaunts != null
                && data.winningTaunts.Length > 0)
                return PickRandom(data.winningTaunts);

            // Losing
            if (playerTerritories > rivalTerritories + 2
                && data.losingTaunts != null
                && data.losingTaunts.Length > 0)
                return PickRandom(data.losingTaunts);

            // Player growing
            if (playerTerritories > rivalTerritories
                && data.playerGrowingTaunts != null
                && data.playerGrowingTaunts.Length > 0)
                return PickRandom(data.playerGrowingTaunts);

            // Default: rival growing
            if (data.growingTaunts != null && data.growingTaunts.Length > 0)
                return PickRandom(data.growingTaunts);

            return data.tagline;
        }

        /// <summary>
        /// Resets mood to neutral.
        /// </summary>
        public void Reset()
        {
            currentMood = "neutral";
        }

        private string PickRandom(string[] array)
        {
            if (array == null || array.Length == 0) return null;
            return array[Random.Range(0, array.Length)];
        }
    }
}
