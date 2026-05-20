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
        public const int DOMINATION_CHECK_START_TURN = 6;
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

        #region Market Share Visual Blocks (GDD v3.0)
        public const int MARKET_VISUAL_BLOCKS = 10;
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
        public const int SCORE_CUSTOMER_SHARE = 500;
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

        #region Staff State System (GDD Section 6.1)
        public const int STAFF_DEFAULT_MORAL = 7;
        public const int STAFF_DEFAULT_FATIGUE = 0;
        public const int STAFF_DEFAULT_LOYALTY = 5;
        public const int STAFF_MORAL_MAX = 10;
        public const int STAFF_FATIGUE_MAX = 10;
        public const int STAFF_LOYALTY_MAX = 10;

        // Moral effects (GDD 6.1)
        public const float STAFF_LOW_MORAL_ERROR_PENALTY = 0.30f;     // moral < 3 -> +30% error
        public const float STAFF_MID_MORAL_EFFICIENCY_PENALTY = 0.20f; // moral < 5 -> -20% efficiency
        public const float STAFF_HIGH_MORAL_BONUS = 0.10f;            // moral > 8 -> +10% bonus

        // Fatigue effects (GDD 6.1)
        public const float STAFF_HIGH_FATIGUE_PENALTY = 0.30f;        // fatigue > 7 -> -30% performance
        public const float STAFF_STRIKE_FATIGUE_CHANCE = 0.50f;       // fatigue > 9 -> 50% strike risk

        // Loyalty effects (GDD 6.5)
        public const float STAFF_RIVAL_STEAL_CHANCE = 0.25f;          // loyalty < 3 -> 25% steal/turn
        public const int STAFF_TRANSFER_PROTECTION_LOYALTY = 8;       // loyalty > 8 -> transfer protection
        #endregion

        #region Staff XP/Level System (GDD Section 6.4)
        public const int STAFF_XP_PER_TURN = 5;
        public const int STAFF_XP_PER_LEVEL = 25;
        public const int STAFF_MAX_LEVEL = 4;
        public const float STAFF_LEVEL2_ERROR_REDUCTION = 0.20f;       // Level 2: -20% error rate
        public const float STAFF_LEVEL3_EFFICIENCY_BONUS = 0.15f;      // Level 3: +15% efficiency
        public const int STAFF_LEVEL3_CUSTOMER_SATISFACTION = 1;       // Level 3: +1 customer satisfaction
        public const int STAFF_ABILITY_UNLOCK_LEVEL = 4;               // Level 4: active ability unlocked
        #endregion

        #region Overtime (GDD Section 6.3)
        public const float STAFF_OVERTIME_CAPACITY_BONUS = 0.50f;     // +50% capacity this turn
        public const int STAFF_OVERTIME_FATIGUE = 2;                   // +2 fatigue
        public const int STAFF_OVERTIME_STRIKE_THRESHOLD = 3;         // 3 consecutive -> strike
        #endregion

        #region Chain Reaction System (GDD Section 11, 12.2)
        public const int CHAIN_CHEAP_SUPPLIER_THRESHOLD = 4;
        public const int CHAIN_SALARY_DELAY_THRESHOLD = 3;
        public const int CHAIN_TAX_UNPAID_THRESHOLD = 2;
        public const int CHAIN_UNINSURED_STAFF_THRESHOLD = 3;
        public const float CHAIN_PLATFORM_RATING_CRISIS = 3.0f;
        public const float CHAIN_GROWTH_TRAP_OPERATION_RATIO = 0.50f;
        #endregion

        #region Headhunting (GDD Section 6.5)
        public const float HEADHUNT_COUNTER_OFFER_MULTIPLIER = 1.5f;
        public const float HEADHUNT_REJECT_BONUS_MULTIPLIER = 2.0f;
        public const int HEADHUNT_REJECT_LOYALTY_BONUS = 2;
        #endregion

        #region Customer Loyalty (GDD Section 7.3)
        public const float LOYALTY_PER_GOOD_TURN = 5f;
        public const float LOYALTY_DECAY_PER_BAD_TURN = 10f;
        public const int LOYALTY_CUSTOMERS_PER_GROUP = 5;
        public const float LOYALTY_MAX = 100f;
        public const float LOYALTY_MIN = 0f;
        #endregion

        #region Location (GDD Section 10.1)
        public const int LOCATION_REMOTE_CUSTOMERS = 0;
        public const int LOCATION_REMOTE_RENT = 20;
        public const int LOCATION_SIDE_CUSTOMERS = 2;
        public const int LOCATION_SIDE_RENT = 50;
        public const int LOCATION_MAIN_CUSTOMERS = 5;
        public const int LOCATION_MAIN_RENT = 100;
        public const int LOCATION_MALL_CUSTOMERS = 8;
        public const int LOCATION_MALL_RENT = 180;
        #endregion

        #region Rating Recovery (GDD Section 8.4)
        public const float PLATFORM_RATING_RECOVERY_PER_GOOD_TURN = 0.1f;
        #endregion

        #region Tax Period (GDD Section 5.8)
        public const int TAX_PERIOD_INTERVAL = 5;
        public const float TAX_PERIOD_RATE = 0.20f;
        public const float TAX_DEBT_INTEREST_RATE = 0.10f;
        public const int TAX_DEBT_AUDIT_THRESHOLD = 2;
        #endregion

        #region Salary System (GDD Section 5.5)
        public const int SALARY_DELAY_MORALE = -3;
        public const float SALARY_DELAY_RESIGN_RISK = 0.15f;
        public const float SALARY_PARTIAL_PAY_RATE = 0.50f;
        public const int SALARY_PARTIAL_MORALE = -1;
        public const int SALARY_PARTIAL_LOYALTY = -1;
        public const float SALARY_ADVANCE_PAY_RATE = 1.25f;
        public const int SALARY_ADVANCE_MORALE = 2;
        public const int SALARY_ADVANCE_LOYALTY = 1;
        #endregion

        #region Insurance System (GDD Section 5.6)
        public const float INSURANCE_SGK_MULTIPLIER = 0.37f;
        public const int INSURANCE_UNINSURED_RISK = 5;
        public const int INSURANCE_DAILY_RISK = 2;
        #endregion

        #region Credit System (GDD Section 5.7)
        public const int CREDIT_SMALL_AMOUNT = 300;
        public const float CREDIT_SMALL_INTEREST = 0.05f;
        public const int CREDIT_SMALL_DURATION = 5;
        public const int CREDIT_MEDIUM_AMOUNT = 600;
        public const float CREDIT_MEDIUM_INTEREST = 0.08f;
        public const int CREDIT_MEDIUM_DURATION = 8;
        public const int CREDIT_LARGE_AMOUNT = 1200;
        public const float CREDIT_LARGE_INTEREST = 0.12f;
        public const int CREDIT_LARGE_DURATION = 12;
        public const int CREDIT_EMERGENCY_AMOUNT = 200;
        public const float CREDIT_EMERGENCY_INTEREST = 0.20f;
        public const int CREDIT_EMERGENCY_DURATION = 3;
        #endregion

        #region Inflation System (GDD Section 5.9)
        public const int INFLATION_INTERVAL = 5;
        public const float INFLATION_COST_INCREASE_MIN = 0.05f;
        public const float INFLATION_COST_INCREASE_MAX = 0.15f;
        public const float INFLATION_SALARY_INCREASE = 0.03f;
        #endregion

        #region Stock System (GDD Section 9.1-9.4)
        public const int STOCK_SPOILAGE_INTERVAL = 3;
        public const float STOCK_SPOILAGE_MIN = 0.10f;
        public const float STOCK_SPOILAGE_MAX = 0.20f;
        public const float STOCK_CLOTHING_SEASON_LOSS = 0.25f;
        #endregion

        #region Location System (GDD Section 10.1-10.3)
        public const int LOCATION_TRAFFIC_REMOTE = 0;
        public const int LOCATION_RENT_REMOTE = 20;
        public const int LOCATION_TRAFFIC_SIDE = 2;
        public const int LOCATION_RENT_SIDE = 50;
        public const int LOCATION_TRAFFIC_MAIN = 5;
        public const int LOCATION_RENT_MAIN = 100;
        public const int LOCATION_TRAFFIC_MALL = 8;
        public const int LOCATION_RENT_MALL = 180;
        #endregion
    }
}
