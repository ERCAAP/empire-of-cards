using UnityEngine;

namespace EmpireOfCards.Data
{
    [CreateAssetMenu(fileName = "TechCategoryProfile", menuName = "EmpireOfCards/V4/Tech Category Profile")]
    public class TechCategoryProfile : ScriptableObject
    {
        public string categoryId;
        public string labelKey;
        public string summaryKey;
        public string displayName;
        [TextArea] public string summary;
        [TextArea] public string scenarioNote;
        public string rivalName;
        public string[] starterBonusCardIds;
        public string[] earlyBonusCardIds;
        public string[] midBonusCardIds;
        public string[] lateBonusCardIds;
        public string[] crisisCardIds;
        public float demandModifier;
        public float capacityModifier;
        public float qualityModifier;
        public float ratingModifier;
        public float legalRiskModifier;
    }

    public static class TechCategoryCatalog
    {
        private static TechCategoryProfile[] _defaults;

        public static TechCategoryProfile[] CreateDefaults()
        {
            _defaults = new[]
            {
                Create("graphic_design", "techcat.graphic_design.name", "techcat.graphic_design.summary",
                    "Graphic Design",
                    "Creative tooling, templates, export flow, and premium retention pressure.",
                    "Competes on export quality, collaboration flow, and subscription retention.",
                    "Rival Studio",
                    new[] { "TC10" }, new[] { "TC10", "TC11" }, new[] { "TC10", "TC11" }, new[] { "TC10", "TC11" }, new[] { "TC12" },
                    0.25f, 0f, 0.35f, 0.15f, 0f),
                Create("health_fitness", "techcat.health_fitness.name", "techcat.health_fitness.summary",
                    "Health & Fitness",
                    "Habit loops, streaks, coaching credibility, and trust-sensitive retention.",
                    "Competes on daily active habits, coaching trust, and churn control.",
                    "Rival Fitness",
                    new[] { "TC13" }, new[] { "TC13", "TC14" }, new[] { "TC13", "TC14" }, new[] { "TC13", "TC14" }, new[] { "TC15" },
                    0.2f, 0.1f, 0.2f, 0.1f, 0f),
                Create("lifestyle", "techcat.lifestyle.name", "techcat.lifestyle.summary",
                    "Lifestyle & Social",
                    "Messaging, dating-style discovery, moderation, privacy, and reputation swings.",
                    "Competes on growth loops, moderation quality, and privacy resilience.",
                    "Rival Social",
                    new[] { "TC16" }, new[] { "TC16", "TC17" }, new[] { "TC16", "TC17" }, new[] { "TC16", "TC17" }, new[] { "TC18" },
                    0.45f, -0.1f, 0f, 0f, 0.15f),
                Create("ai_tools", "techcat.ai_tools.name", "techcat.ai_tools.summary",
                    "AI Tools",
                    "Inference cost, prompt quality, model trust, and viral feature pressure.",
                    "Competes on reliability, unit economics, and feature novelty.",
                    "Rival AI",
                    new[] { "TC19" }, new[] { "TC19", "TC20" }, new[] { "TC19", "TC20" }, new[] { "TC19", "TC20" }, new[] { "TC21" },
                    0.3f, -0.15f, 0.3f, 0.1f, 0.1f),
                Create("casual_games", "techcat.casual_games.name", "techcat.casual_games.summary",
                    "Casual Mobile Game",
                    "Ad monetization, session retention, content cadence, and store reviews.",
                    "Competes on CPI, retention, live-ops pacing, and update cadence.",
                    "Rival Arcade",
                    new[] { "TC22" }, new[] { "TC22", "TC23" }, new[] { "TC22", "TC23" }, new[] { "TC22", "TC23" }, new[] { "TC24" },
                    0.5f, -0.05f, 0f, -0.05f, 0.05f),
                Create("hyper_casual", "techcat.hyper_casual.name", "techcat.hyper_casual.summary",
                    "Hyper Casual",
                    "Ultra-cheap acquisition, fragile retention, ad burnout, and clone pressure.",
                    "Competes on prototype speed, retention floor, and acquisition efficiency.",
                    "Clone Factory",
                    new[] { "TC25" }, new[] { "TC25", "TC26" }, new[] { "TC25", "TC26" }, new[] { "TC25", "TC26" }, new[] { "TC27" },
                    0.6f, -0.2f, -0.15f, -0.1f, 0.2f)
            };
            return _defaults;
        }

        public static TechCategoryProfile Find(string categoryId)
        {
            if (_defaults == null || _defaults.Length == 0)
                CreateDefaults();

            for (int i = 0; i < _defaults.Length; i++)
            {
                if (_defaults[i] != null && _defaults[i].categoryId == categoryId)
                    return _defaults[i];
            }

            return null;
        }

        private static TechCategoryProfile Create(
            string id,
            string labelKey,
            string summaryKey,
            string displayName,
            string summary,
            string scenarioNote,
            string rivalName,
            string[] starterBonusCardIds,
            string[] earlyBonusCardIds,
            string[] midBonusCardIds,
            string[] lateBonusCardIds,
            string[] crisisCardIds,
            float demandModifier,
            float capacityModifier,
            float qualityModifier,
            float ratingModifier,
            float legalRiskModifier)
        {
            var profile = ScriptableObject.CreateInstance<TechCategoryProfile>();
            profile.categoryId = id;
            profile.labelKey = labelKey;
            profile.summaryKey = summaryKey;
            profile.displayName = displayName;
            profile.summary = summary;
            profile.scenarioNote = scenarioNote;
            profile.rivalName = rivalName;
            profile.starterBonusCardIds = starterBonusCardIds;
            profile.earlyBonusCardIds = earlyBonusCardIds;
            profile.midBonusCardIds = midBonusCardIds;
            profile.lateBonusCardIds = lateBonusCardIds;
            profile.crisisCardIds = crisisCardIds;
            profile.demandModifier = demandModifier;
            profile.capacityModifier = capacityModifier;
            profile.qualityModifier = qualityModifier;
            profile.ratingModifier = ratingModifier;
            profile.legalRiskModifier = legalRiskModifier;
            profile.name = $"TechCategory_{id}";
            return profile;
        }
    }
}
