namespace EmpireOfCards.Core
{
    public static class Constants
    {
        #region Turn & Era

        public const int MAX_TURNS = 25;
        public const int ERA_1_END = 6;   // Garage  (turns 1-6)
        public const int ERA_2_END = 13;  // Growth  (turns 7-13)
        public const int ERA_3_END = 19;  // Scale   (turns 14-19)
        // ERA_4 = turns 20-25 (Dominance)
        public const int TURNS_PER_SEASON = 5;

        #endregion

        #region Actions Per Era

        public const int ACTIONS_ERA_1 = 2;
        public const int ACTIONS_ERA_2 = 3;
        public const int ACTIONS_ERA_3 = 4;
        public const int ACTIONS_ERA_4 = 5;

        #endregion

        #region Slot Counts Per Era [Operation, Staff, Marketing, Supplier, Temp]

        public static readonly int[] SLOTS_ERA_1 = { 2, 3, 1, 1, 2 };  // total  9
        public static readonly int[] SLOTS_ERA_2 = { 3, 4, 2, 2, 3 };  // total 14
        public static readonly int[] SLOTS_ERA_3 = { 4, 6, 3, 2, 3 };  // total 18
        public static readonly int[] SLOTS_ERA_4 = { 5, 7, 3, 3, 3 };  // total 21

        #endregion

        #region Starting Values (Restaurant)

        public const int STARTING_CASH = 300;
        public const float STARTING_DEMAND = 2f;
        public const float STARTING_CAPACITY = 3f;
        public const float STARTING_QUALITY = 3f;
        public const float STARTING_RATING = 3.0f;
        public const float STARTING_STABILITY = 5f;
        public const float STARTING_LEGAL_RISK = 0f;
        public const float STARTING_MARKET_SHARE = 15f;
        public const float STARTING_HYGIENE = 5f;

        #endregion

        #region Stat Clamps

        public const float STAT_MIN = 0f;
        public const float STAT_MAX = 10f;
        public const float MARKET_SHARE_MIN = 0f;
        public const float MARKET_SHARE_MAX = 100f;
        public const float LEGAL_RISK_MAX = 100f;

        #endregion

        #region Win / Lose Conditions

        public const float WIN_MARKET_SHARE = 60f;
        public const float LOSE_RATING = 1.5f;
        public const int LOSE_RATING_TURNS = 3;
        public const float LOSE_LEGAL_RISK = 90f;
        public const float LOSE_RIVAL_SHARE = 70f;
        public const float LOSE_HYGIENE = 1f;
        public const int LOSE_BANKRUPT_TURNS = 3;

        #endregion

        #region Economy

        public const float TAX_RATE = 0.10f;
        public const float BASE_RENT = 20f;
        public const float COMMISSION_DELIVERY_APP = 0.15f;

        #endregion

        #region Hygiene

        public const float HYGIENE_NATURAL_DECAY = 0.1f;
        public const float HYGIENE_OVERLOAD_PENALTY = 0.3f;
        public const float HYGIENE_CLEAN_THRESHOLD = 8f;
        public const float HYGIENE_DIRTY_THRESHOLD = 5f;
        public const float HYGIENE_DANGER_THRESHOLD = 3f;
        public const float HYGIENE_SHUTDOWN_THRESHOLD = 1f;

        #endregion

        #region Rating

        public const float RATING_MIN = 1f;
        public const float RATING_MAX = 5f;
        public const float ORGANIC_DEMAND_THRESHOLD = 3.5f;

        #endregion

        #region Season Multipliers (Restaurant)

        public const float SEASON_SPRING = 1.0f;
        public const float SEASON_SUMMER = 1.2f;
        public const float SEASON_AUTUMN = 1.0f;
        public const float SEASON_WINTER = 0.9f;
        public const float SEASON_RAMADAN = 1.3f;

        #endregion

        #region Phase Durations (seconds)

        public const float DRAW_DURATION = 1.0f;
        public const float PLANNING_DURATION = 1.5f;
        public const float RESOLVE_STEP_DURATION = 0.5f;
        public const float CRISIS_DURATION = 2.0f;
        public const float CRISIS_NO_CRISIS_DURATION = 0.5f;
        public const float RIVAL_DURATION = 1.5f;
        public const float MARKET_UPDATE_DURATION = 1.0f;

        #endregion

        #region Staff

        public const float BURNOUT_THRESHOLD = 0.6f;
        public const float RESIGNATION_MORALE_THRESHOLD = 3f;
        public const float RESIGNATION_MORALE_CRITICAL = 2f;
        public const int PROMOTION_EXPERIENCE_REQUIRED = 5;

        #endregion

        #region Customer Satisfaction Weights

        public const float SATISFACTION_QUALITY_WEIGHT = 0.30f;
        public const float SATISFACTION_SPEED_WEIGHT = 0.25f;
        public const float SATISFACTION_HYGIENE_WEIGHT = 0.20f;
        public const float SATISFACTION_PRICE_WEIGHT = 0.15f;
        public const float SATISFACTION_AMBIANCE_WEIGHT = 0.10f;

        #endregion

        #region Market

        public const int TOTAL_MARKET_CUSTOMERS = 100;

        #endregion

        #region Credit

        public const int CREDIT_MICRO_AMOUNT = 100;
        public const float CREDIT_MICRO_INTEREST = 0.05f;
        public const int CREDIT_MICRO_DURATION = 5;

        public const int CREDIT_BUSINESS_AMOUNT = 300;
        public const float CREDIT_BUSINESS_INTEREST = 0.08f;
        public const int CREDIT_BUSINESS_DURATION = 8;

        public const int CREDIT_BIG_AMOUNT = 800;
        public const float CREDIT_BIG_INTEREST = 0.12f;
        public const int CREDIT_BIG_DURATION = 12;

        public const int CREDIT_EMERGENCY_AMOUNT = 50;
        public const float CREDIT_EMERGENCY_INTEREST = 0.15f;
        public const int CREDIT_EMERGENCY_DURATION = 3;

        #endregion

        #region Meta-progression

        public const float EXIT_CARRY_OVER_RATE = 0.30f;

        #endregion

        #region Scoring

        public const int SCORE_MARKET_SHARE = 10;
        public const int SCORE_CASH = 5;
        public const int SCORE_RATING = 100;
        public const int SCORE_CRISIS_SOLVED = 50;
        public const int SCORE_ERA_REACHED = 200;
        public const int SCORE_CLEAN_PLAY = 500;
        public const int SCORE_FAST_FINISH = 300;

        #endregion

        #region Hand

        public const int HAND_SIZE = 5;
        public const int REDRAW_PER_TURN = 1;

        #endregion
    }
}
