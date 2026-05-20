namespace EmpireOfCards.Core
{
    /// <summary>
    /// Game-wide balance constants. Every value references the GDD section it
    /// comes from so designers can trace changes back to the spec.
    /// </summary>
    public static class Constants
    {
        #region Economy (GDD Section 9)
        public const int STARTING_MONEY = 500;
        public const float SELL_RATE = 0.4f;        // Sell card = 40%
        #endregion

        #region Run Structure (GDD v3.0 Section 16)
        public const int MAX_TURNS = 25;             // 25 turns, 5 seasons x 5 turns
        public const int TURNS_PER_SEASON = 5;
        public const int SEASON_COUNT = 5;
        #endregion

        #region Dynamic Game Length (GDD v3.0 Section 16)
        public const int SOFT_CAP_TURN = 20;                    // Income penalty starts at turn 20
        public const float SOFT_CAP_PENALTY = 0.05f;            // -5% per turn after soft cap
        public const int HARD_CAP_TURN = 25;                    // Forced end at turn 25
        #endregion

        #region Business Maintenance (GDD Section 3.1)
        public const int NEGLECT_THRESHOLD_MINOR = 4;           // 20% income loss
        public const int NEGLECT_THRESHOLD_MAJOR = 6;           // 40% income loss
        public const float NEGLECT_PENALTY_MINOR = 0.20f;
        public const float NEGLECT_PENALTY_MAJOR = 0.40f;
        #endregion

        #region Shop Bias (GDD Section 1.5)
        public const int SHOP_BIAS_TURNS = 5;                   // Bias active for first 5 turns
        #endregion

        #region Turn Mechanics (GDD Section 4)
        public const int STARTING_ACTIONS = 3;
        public const int MAX_ACTIONS = 5;            // Max with AI Assistant
        public const int HAND_SIZE = 5;              // Draw 5 cards each turn
        public const int REDRAWS_PER_TURN = 1;
        #endregion

        #region Slot System v2 (GDD v3.0 Section 4)
        // Starting slot counts per type
        public const int STARTING_OPERATION_SLOTS = 4;
        public const int STARTING_STAFF_SLOTS = 5;
        public const int STARTING_MARKETING_SLOTS = 3;
        public const int STARTING_SUPPLIER_SLOTS = 2;
        public const int STARTING_TEMP_EFFECT_SLOTS = 3; // Fixed, never expands

        // Maximum slot counts per type
        public const int MAX_OPERATION_SLOTS = 8;
        public const int MAX_STAFF_SLOTS = 10;
        public const int MAX_MARKETING_SLOTS = 5;
        public const int MAX_SUPPLIER_SLOTS = 4;
        public const int MAX_TEMP_EFFECT_SLOTS = 3; // Fixed
        #endregion

        #region Customer Market System (GDD v3.0 Section 7)
        public const int TOTAL_MARKET_CUSTOMERS = 100;
        public const int WIN_CUSTOMER_SHARE = 60;            // 60/100 = 60% win condition
        public const float CUSTOMER_WEIGHT_QUALITY = 0.30f;
        public const float CUSTOMER_WEIGHT_PRICE = 0.20f;
        public const float CUSTOMER_WEIGHT_PLATFORM_RATING = 0.20f;
        public const float CUSTOMER_WEIGHT_MARKETING = 0.15f;
        public const float CUSTOMER_WEIGHT_SPEED = 0.10f;
        public const float CUSTOMER_WEIGHT_LOYALTY = 0.05f;
        #endregion

        #region Platform Rating (GDD v3.0 Section 8)
        public const float PLATFORM_RATING_MIN = 1.0f;
        public const float PLATFORM_RATING_MAX = 5.0f;
        public const float PLATFORM_RATING_DEFAULT = 3.0f;
        public const float PLATFORM_RATING_DECAY_PER_TURN = 0.1f; // Without marketing
        public const float PLATFORM_RATING_GAIN_GREAT_REVIEW = 0.3f;
        public const float PLATFORM_RATING_GAIN_MARKETING = 0.2f;
        public const float PLATFORM_RATING_LOSS_BAD_EVENT = 0.5f;
        #endregion

        #region Legal Risk System (GDD v3.0 Section 13)
        public const int LEGAL_RISK_MIN = 0;
        public const int LEGAL_RISK_MAX = 100;
        public const int LEGAL_RISK_CAUTION_THRESHOLD = 26;
        public const int LEGAL_RISK_DANGER_THRESHOLD = 51;
        public const int LEGAL_RISK_CERTAIN_THRESHOLD = 76;
        public const float LEGAL_RISK_INCOME_PENALTY = 0.15f; // -15% income when in Danger
        public const int LEGAL_RISK_DECAY_PER_TURN = 3;       // Risk drops 3 per turn naturally
        #endregion

        #region Cash Flow (GDD v3.0)
        public const int CASH_CRISIS_THRESHOLD = 3;              // Turns with negative balance before crisis triggers
        #endregion

        #region Season System (GDD v3.0 Section 14)
        public const float SEASON_PEAK_INCOME_MULTIPLIER = 1.25f;
        public const float SEASON_TRANSITION_INCOME_PENALTY = 0.70f; // -30% during transitions
        public const float SEASON_OFFPEAK_INCOME_MULTIPLIER = 0.85f;

        // Per-season multipliers used by ResolvePhase.GetSeasonMultiplier()
        public const float SEASON_MULTIPLIER_SPRING = 1.0f;          // Balanced
        public const float SEASON_MULTIPLIER_SUMMER = 1.25f;         // Peak foot traffic
        public const float SEASON_MULTIPLIER_AUTUMN = 0.70f;         // Transition penalty
        public const float SEASON_MULTIPLIER_WINTER = 1.15f;         // Holiday surge
        public const float SEASON_MULTIPLIER_RAMADAN = 0.85f;        // Off-peak shift
        #endregion

        #region Tax (GDD Section 9.2)
        public const float TAX_RATE = 0.15f;
        public const float REDUCED_TAX_RATE = 0.075f;  // 1 accountant
        public const float MIN_TAX_RATE = 0.03f;        // 2 accountants
        #endregion

        #region FBI (GDD Section 9.3)
        public const int FBI_RAID_PENALTY = 300;
        #endregion

        #region Territory Map (GDD Section 6)
        public const int TERRITORY_COUNT = 10;
        public const int WIN_TERRITORIES = 6;        // 60% market share
        public const int LOSE_TERRITORIES = 7;       // Rival takes 7 territories
        #endregion

        #region Shop & Events (GDD Section 2.2 / 4.1)
        public const int SHOP_CARDS_PER_TURN = 3;
        public const int EVENT_INTERVAL = 3;         // 1 event every 3 turns
        public const int STARTING_DECK_SIZE = 14;
        #endregion

        #region Market Pool (GDD Balance Table)
        public const int BASE_MARKET_CUSTOMERS = 60;
        public const int EARLY_GROWTH_PER_TURN = 5;   // Turn 1-5
        public const int MID_GROWTH_PER_TURN = 6;     // Turn 6-10
        public const int LATE_GROWTH_PER_TURN = 8;    // Turn 11-15
        public const int END_GROWTH_PER_TURN = 10;    // Turn 16-20
        #endregion

        #region Scoring (GDD Section 10.3)
        public const int SCORE_TERRITORY = 500;
        public const int SCORE_MONEY = 1;
        public const int SCORE_COMBO = 200;
        public const int SCORE_BUSINESS = 100;
        public const int SCORE_EARLY_FINISH = 300;    // Remaining turns x 300
        public const int SCORE_FBI_EVASION = 50;
        public const int SCORE_WIN_BONUS = 1000;
        #endregion

        #region Business Evolution (GDD Section 3.1)
        public const int EVOLUTION_CUSTOMER_THRESHOLD = 40; // Level up after 40 customers
        public const int EVOLUTION_TURN_REQUIREMENT = 15;   // Over 15 turns
        #endregion

        #region Employee Leaving (GDD Section 4.2)
        public const int EMPLOYEE_LEAVE_TURN_THRESHOLD = 8; // Employee working 8+ turns
        #endregion

        #region Company Tier (GDD Section 1.6 — customer-based v3.0)
        public const int TIER_ENTREPRENEUR_CUSTOMERS = 20;
        public const int TIER_ENTREPRENEUR_COMBOS = 1;
        public const int TIER_CORPORATION_CUSTOMERS = 45;
        public const int TIER_CORPORATION_COMBOS = 2;
        public const float TIER_CORPORATION_OPERATION_OCCUPANCY = 0.75f;
        public const int TIER_CONGLOMERATE_CUSTOMERS = 60;
        public const int TIER_CONGLOMERATE_COMBOS = 3;
        public const int TIER_SCORE_ENTREPRENEUR = 200;
        public const int TIER_SCORE_CORPORATION = 500;
        public const int TIER_SCORE_CONGLOMERATE = 1000;
        #endregion

        #region First Venture (GDD Section 1.5)
        public const int BLACK_MARKET_BONUS_MONEY = 200;  // Extra $200 for black market
        #endregion

        #region Special Business Mechanics
        public const int FRANCHISE_HUB_INCOME_PER_BUSINESS = 40;  // B09: +40 income per active business
        public const float CONSULTING_FIRM_MULTIPLIER = 1.4f;     // B11: best business earns 40% more
        public const int POPUP_SHOP_LIFETIME_TURNS = 4;           // B12: self-destructs after 4 turns
        #endregion
    }
}
