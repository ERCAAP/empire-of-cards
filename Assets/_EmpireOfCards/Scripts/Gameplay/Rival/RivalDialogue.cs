using System.Collections.Generic;
using EmpireOfCards.Core;
using EmpireOfCards.Data;
using UnityEngine;

namespace EmpireOfCards.Gameplay
{
    /// <summary>
    /// Rival mood categories. Determines the mood "tell" icon shown
    /// before the rival acts each turn (GDD Section 8).
    /// </summary>
    public enum RivalMood
    {
        Calm,       // Rival is ahead or even
        Aggressive, // Rival is behind by 2+ territories
        Greedy,     // Rival has high money, planning a big purchase
        Scheming    // Rival is about to use a special action
    }

    /// <summary>
    /// Game-state categories for context-aware dialogue selection.
    /// Ordered by priority (highest first) for the picker.
    /// </summary>
    public enum DialogueCategory
    {
        FinalTurns,
        FBIRiskHigh,
        PlayerCombo,
        RivalDesperate,
        CloseGame,
        RivalLeading,
        PlayerLeading,
        EarlyGame
    }

    /// <summary>
    /// Handles rival mood tracking, context-aware dialogue selection,
    /// and player strategy reaction (GDD Section 8).
    ///
    /// Key behaviors:
    /// - Mood "tell" icon shown before rival actions each turn
    /// - State-based dialogue lines that never repeat in a run
    /// - One-time strategy reaction when player concentrates in a tag
    /// </summary>
    public class RivalDialogue
    {
        private readonly RivalData data;

        // --- Mood Tell ---
        private RivalMood currentMood = RivalMood.Calm;
        private string currentMoodIcon = "";

        // --- No-repeat tracking ---
        private readonly HashSet<string> usedLines = new HashSet<string>();

        // --- Strategy reaction tracking ---
        private readonly HashSet<CardTag> acknowledgedStrategies = new HashSet<CardTag>();

        // --- State-based dialogue pools ---
        private static readonly Dictionary<DialogueCategory, string[]> DialoguePool =
            new Dictionary<DialogueCategory, string[]>
        {
            {
                DialogueCategory.EarlyGame, new[]
                {
                    "Let's see what you've got.",
                    "Small beginnings... just like mine.",
                    "The market is wide open.",
                    "May the best entrepreneur win.",
                    "Show me what you're made of."
                }
            },
            {
                DialogueCategory.RivalLeading, new[]
                {
                    "This industry isn't big enough for both of us.",
                    "I can do this all day.",
                    "You're falling behind.",
                    "My empire grows stronger.",
                    "Better luck next turn."
                }
            },
            {
                DialogueCategory.PlayerLeading, new[]
                {
                    "Don't get comfortable.",
                    "It's not over yet.",
                    "Impressive... but temporary.",
                    "Enjoy it while it lasts.",
                    "I've seen comebacks before."
                }
            },
            {
                DialogueCategory.CloseGame, new[]
                {
                    "This is going to be close.",
                    "One territory away...",
                    "May the best business win.",
                    "Neck and neck.",
                    "Every decision matters now."
                }
            },
            {
                DialogueCategory.RivalDesperate, new[]
                {
                    "I'm not done yet!",
                    "Watch your back.",
                    "Desperate times call for desperate measures.",
                    "You haven't won yet.",
                    "I still have a few tricks left."
                }
            },
            {
                DialogueCategory.PlayerCombo, new[]
                {
                    "Clever combination.",
                    "I need to step up my game.",
                    "That synergy won't save you.",
                    "Interesting strategy."
                }
            },
            {
                DialogueCategory.FBIRiskHigh, new[]
                {
                    "Playing with fire, I see.",
                    "The authorities are watching.",
                    "One wrong move and it's over for you.",
                    "That risk will catch up to you."
                }
            },
            {
                DialogueCategory.FinalTurns, new[]
                {
                    "Last chance.",
                    "Everything comes down to this.",
                    "The end is near.",
                    "No more room for mistakes.",
                    "This is it."
                }
            }
        };

        // --- Strategy reaction lines (keyed by CardTag) ---
        private static readonly Dictionary<CardTag, string> StrategyReactionLines =
            new Dictionary<CardTag, string>
        {
            { CardTag.Food, "Going all-in on food? Risky." },
            { CardTag.Coffee, "A coffee empire? Bold choice." },
            { CardTag.Tech, "Tech is a gamble. Let's see if it pays off." },
            { CardTag.Marketing, "All marketing, no substance? We'll see." },
            { CardTag.Finance, "Following the money, are you?" },
            { CardTag.Illegal, "I see you've chosen the dark path." },
            { CardTag.Crypto, "Crypto? You're braver than I thought." },
            { CardTag.Nightlife, "The nightlife business? Interesting angle." },
            { CardTag.Entertainment, "Entertainment mogul in the making." },
            { CardTag.Franchise, "Building a franchise empire, I see." }
        };

        // --- Mood icon map ---
        private static readonly Dictionary<RivalMood, string> MoodIcons =
            new Dictionary<RivalMood, string>
        {
            { RivalMood.Calm, "\U0001F610" },       // neutral face
            { RivalMood.Aggressive, "\U0001F624" },  // angry face
            { RivalMood.Greedy, "\U0001F4B0" },      // money bag
            { RivalMood.Scheming, "\U0001F914" }     // thinking face
        };

        // --- Properties ---
        public string CurrentMood => currentMood.ToString().ToLower();
        public RivalMood Mood => currentMood;
        public string MoodIcon => currentMoodIcon;

        // ----------------------------------------------------------------
        // Construction
        // ----------------------------------------------------------------

        public RivalDialogue(RivalData data)
        {
            this.data = data;
            this.currentMood = RivalMood.Calm;
            this.currentMoodIcon = MoodIcons[RivalMood.Calm];
        }

        // ----------------------------------------------------------------
        // Feature 1: Mood Tell System
        // ----------------------------------------------------------------

        /// <summary>
        /// Determines the rival mood based on game state. Called BEFORE
        /// actions execute so the player sees a "tell" before the rival acts.
        /// Fires EventBus.RivalMoodChanged with the mood icon.
        /// </summary>
        public void DetermineMood(
            int playerBlocks,
            int rivalBlocks,
            int rivalMoney,
            int businessCostThreshold,
            string nextAction)
        {
            RivalMood previousMood = currentMood;

            // Priority: Scheming > Greedy > Aggressive > Calm
            if (nextAction == "aggressive" || nextAction == "event_bonus")
            {
                currentMood = RivalMood.Scheming;
            }
            else if (rivalMoney >= businessCostThreshold * 2)
            {
                currentMood = RivalMood.Greedy;
            }
            else if (rivalBlocks < playerBlocks - 1)
            {
                currentMood = RivalMood.Aggressive;
            }
            else
            {
                currentMood = RivalMood.Calm;
            }

            currentMoodIcon = MoodIcons[currentMood];
            EventBus.RivalMoodChanged(currentMoodIcon);
        }

        /// <summary>
        /// Legacy mood update for backward compatibility.
        /// Now delegates to DetermineMood with defaults.
        /// </summary>
        public void UpdateMood(int playerBlocks, int rivalBlocks)
        {
            // Simplified path when we don't have full context
            if (playerBlocks > rivalBlocks + 1)
                currentMood = RivalMood.Aggressive;
            else if (rivalBlocks > playerBlocks + 1)
                currentMood = RivalMood.Calm;
            else
                currentMood = RivalMood.Calm;

            currentMoodIcon = MoodIcons[currentMood];
        }

        // ----------------------------------------------------------------
        // Feature 2: Context-Aware Dialogue Escalation
        // ----------------------------------------------------------------

        /// <summary>
        /// Returns a context-aware taunt based on the current game state.
        /// Picks the most relevant category, then an UNUSED line from it.
        /// Never repeats a line within the same run.
        /// Returns null if all lines in the best category have been used
        /// (silence is also powerful).
        /// </summary>
        public string GetTaunt(int playerBlocks, int rivalBlocks)
        {
            if (data == null) return null;

            var gm = GameManager.Instance;
            int currentTurn = gm != null ? gm.CurrentTurn : 1;
            float fbiRisk = gm != null ? gm.FBIRisk : 0f;

            DialogueCategory bestCategory = PickBestCategory(
                playerBlocks, rivalBlocks, currentTurn, fbiRisk);

            return PickUnusedLine(bestCategory);
        }

        /// <summary>
        /// Extended taunt getter with combo notification.
        /// Called after a player combo triggers to deliver a reaction line.
        /// </summary>
        public string GetComboReactionTaunt()
        {
            return PickUnusedLine(DialogueCategory.PlayerCombo);
        }

        /// <summary>
        /// Selects the most relevant dialogue category based on current game state.
        /// Categories are checked in priority order.
        /// </summary>
        private DialogueCategory PickBestCategory(
            int playerBlocks,
            int rivalBlocks,
            int currentTurn,
            float fbiRisk)
        {
            // Final turns (turn 20+) -- highest narrative weight
            if (currentTurn >= 20)
                return DialogueCategory.FinalTurns;

            // FBI risk high (player FBI > 50%)
            if (fbiRisk > 0.5f)
                return DialogueCategory.FBIRiskHigh;

            // Rival desperate (rival has 2+ fewer territories)
            int gap = playerBlocks - rivalBlocks;
            if (gap >= 2)
                return DialogueCategory.RivalDesperate;

            // Close game (within 1 territory)
            if (Mathf.Abs(playerBlocks - rivalBlocks) <= 1 &&
                (playerBlocks + rivalBlocks) > 0)
                return DialogueCategory.CloseGame;

            // Rival leading
            if (rivalBlocks > playerBlocks)
                return DialogueCategory.RivalLeading;

            // Player leading
            if (playerBlocks > rivalBlocks)
                return DialogueCategory.PlayerLeading;

            // Early game (turn 1-5)
            if (currentTurn <= 5)
                return DialogueCategory.EarlyGame;

            // Default fallback: early game lines for neutral mid-game
            return DialogueCategory.EarlyGame;
        }

        /// <summary>
        /// Picks an unused line from the given category.
        /// Returns null if all lines have been used (intentional silence).
        /// </summary>
        private string PickUnusedLine(DialogueCategory category)
        {
            if (!DialoguePool.TryGetValue(category, out string[] lines))
                return null;

            // Collect available (unused) lines
            List<int> availableIndices = new List<int>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (!usedLines.Contains(lines[i]))
                    availableIndices.Add(i);
            }

            if (availableIndices.Count == 0)
                return null; // All used -- silence

            int chosen = availableIndices[Random.Range(0, availableIndices.Count)];
            string line = lines[chosen];
            usedLines.Add(line);
            return line;
        }

        // ----------------------------------------------------------------
        // Feature 3: Rival Reacts to Player's Strategy
        // ----------------------------------------------------------------

        /// <summary>
        /// Checks the player's board for a dominant tag (2+ businesses of same type).
        /// If detected and not yet acknowledged, fires a one-time strategy comment.
        /// Called once per turn by RivalAI.
        /// </summary>
        public void CheckPlayerStrategy()
        {
            var gm = GameManager.Instance;
            if (gm == null || gm.BoardManager == null) return;

            var playerBusinesses = gm.BoardManager.PlayerBusinesses;
            if (playerBusinesses == null || playerBusinesses.Count < 2) return;

            // Count business cards by tag
            Dictionary<CardTag, int> tagCounts = new Dictionary<CardTag, int>();

            foreach (var biz in playerBusinesses)
            {
                if (biz.isClosed || biz.businessCard == null) continue;
                if (biz.businessCard.tags == null) continue;

                foreach (var tag in biz.businessCard.tags)
                {
                    if (tagCounts.ContainsKey(tag))
                        tagCounts[tag]++;
                    else
                        tagCounts[tag] = 1;
                }
            }

            // Find the dominant tag (2+ businesses)
            CardTag? dominantTag = null;
            int highestCount = 1; // Need at least 2 to trigger

            foreach (var kvp in tagCounts)
            {
                if (kvp.Value > highestCount)
                {
                    highestCount = kvp.Value;
                    dominantTag = kvp.Key;
                }
            }

            if (dominantTag == null) return;

            // Only fire ONCE per detected strategy
            if (acknowledgedStrategies.Contains(dominantTag.Value)) return;

            if (StrategyReactionLines.TryGetValue(dominantTag.Value, out string reaction))
            {
                // Mark as acknowledged BEFORE firing to prevent double-fire
                acknowledgedStrategies.Add(dominantTag.Value);

                // Check that this exact line hasn't been used
                if (!usedLines.Contains(reaction))
                {
                    usedLines.Add(reaction);
                    EventBus.RivalStrategyCommented(reaction);
                }
            }
        }

        // ----------------------------------------------------------------
        // Reset
        // ----------------------------------------------------------------

        /// <summary>
        /// Resets all dialogue state for a new run.
        /// </summary>
        public void Reset()
        {
            currentMood = RivalMood.Calm;
            currentMoodIcon = MoodIcons[RivalMood.Calm];
            usedLines.Clear();
            acknowledgedStrategies.Clear();
        }
    }
}
