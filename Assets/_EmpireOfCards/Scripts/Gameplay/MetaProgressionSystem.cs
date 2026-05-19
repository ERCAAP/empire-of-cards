using UnityEngine;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using EmpireOfCards.Save;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Bridges MetaProgressionData and SaveManager at runtime.
    /// - Awards XP at end of run
    /// - Applies ascension modifiers at game start
    /// - Gates card unlocks based on accumulated XP
    /// </summary>
    public class MetaProgressionSystem : MonoBehaviour
    {
        [SerializeField] private MetaProgressionData progressionData;

        public void Init(MetaProgressionData data)
        {
            progressionData = data;
        }

        // ------------------------------------------------------------------
        // XP Calculation
        // ------------------------------------------------------------------

        /// <summary>
        /// Called at end of run to calculate XP earned. Returns 10% of score
        /// with a minimum of 10 XP per run.
        /// </summary>
        public int CalculateRunXP(int score)
        {
            return Mathf.Max(Mathf.RoundToInt(score * 0.1f), 10);
        }

        // ------------------------------------------------------------------
        // Ascension Modifiers
        // ------------------------------------------------------------------

        /// <summary>
        /// Called at game start to apply all ascension-level modifiers
        /// up to the player's current ascension level.
        /// Modifies balance data in-place for the current run.
        /// </summary>
        public void ApplyAscension(int ascensionLevel, GameBalanceData balance)
        {
            if (progressionData == null || progressionData.ascensionLevels == null) return;
            if (balance == null) return;

            foreach (var asc in progressionData.ascensionLevels)
            {
                if (asc.level <= ascensionLevel)
                {
                    balance.startingMoney += asc.startingMoneyModifier;

                    // Apply crisis frequency and rival aggression via the
                    // RivalAI or event systems that read balance data each turn.
                    // Those multipliers are stored on the AscensionLevel itself
                    // and can be queried via GetAscensionMultiplier().
                }
            }

            Debug.Log($"[MetaProgressionSystem] Ascension {ascensionLevel} applied. " +
                      $"Starting money now {balance.startingMoney}.");
        }

        /// <summary>
        /// Returns the rival aggression multiplier for the given ascension level.
        /// Systems like RivalAI can query this each turn.
        /// </summary>
        public float GetRivalAggressionMultiplier(int ascensionLevel)
        {
            if (progressionData == null || progressionData.ascensionLevels == null)
                return 1f;

            float multiplier = 1f;
            foreach (var asc in progressionData.ascensionLevels)
            {
                if (asc.level <= ascensionLevel)
                    multiplier = asc.rivalAggressionMultiplier;
            }
            return multiplier;
        }

        /// <summary>
        /// Returns the crisis frequency multiplier for the given ascension level.
        /// </summary>
        public float GetCrisisFrequencyMultiplier(int ascensionLevel)
        {
            if (progressionData == null || progressionData.ascensionLevels == null)
                return 1f;

            float multiplier = 1f;
            foreach (var asc in progressionData.ascensionLevels)
            {
                if (asc.level <= ascensionLevel)
                    multiplier = asc.crisisFrequencyMultiplier;
            }
            return multiplier;
        }

        // ------------------------------------------------------------------
        // Card Unlock Gating
        // ------------------------------------------------------------------

        /// <summary>
        /// Checks whether a card is unlocked based on accumulated XP
        /// and the unlock tiers defined in MetaProgressionData.
        /// Cards that don't appear in any tier are always available.
        /// </summary>
        public bool IsCardUnlocked(string cardId, int totalXP)
        {
            if (progressionData == null || progressionData.unlockTiers == null)
                return true;

            // Check if the card appears in ANY tier
            bool cardIsGated = false;

            foreach (var tier in progressionData.unlockTiers)
            {
                if (tier.unlockedCards == null) continue;

                foreach (var card in tier.unlockedCards)
                {
                    if (card != null && card.cardId == cardId)
                    {
                        cardIsGated = true;

                        // Player has enough XP for this tier -> unlocked
                        if (totalXP >= tier.xpRequired)
                            return true;
                    }
                }
            }

            // Card is not listed in any tier -> always available
            if (!cardIsGated)
                return true;

            // Card is gated and player hasn't reached the required tier
            return false;
        }

        /// <summary>
        /// Convenience: checks using the current save data.
        /// </summary>
        public bool IsCardUnlocked(string cardId)
        {
            var save = SaveManager.Instance;
            int totalXP = save != null ? save.Load().totalXP : 0;
            return IsCardUnlocked(cardId, totalXP);
        }

        /// <summary>
        /// Filters a card pool to only include unlocked cards.
        /// Used by ShopManager / CreateShopPool to gate the shop.
        /// </summary>
        public CardData[] FilterUnlockedCards(CardData[] pool, int totalXP)
        {
            if (progressionData == null || progressionData.unlockTiers == null)
                return pool;

            var unlocked = new System.Collections.Generic.List<CardData>(pool.Length);
            foreach (var card in pool)
            {
                if (card != null && IsCardUnlocked(card.cardId, totalXP))
                    unlocked.Add(card);
            }
            return unlocked.ToArray();
        }
    }
}
