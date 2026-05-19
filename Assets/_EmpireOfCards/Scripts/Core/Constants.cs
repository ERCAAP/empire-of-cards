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

        #region Run Structure (GDD Section 10)
        public const int MAX_TURNS = 30;             // Hard cap (GDD dynamic game length)
        #endregion

        #region Dynamic Game Length (GDD Section 1.7)
        public const int SOFT_CAP_TURN = 25;                    // Income penalty starts
        public const float SOFT_CAP_PENALTY = 0.05f;            // -5% per turn after soft cap
        public const int HARD_CAP_TURN = 30;                    // Forced end
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

        #region Slot System (GDD Section 5)
        public const int STARTING_SLOTS = 3;
        public const int MAX_SLOTS = 5;
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

        #region Company Tier (GDD Section 1.6)
        public const int TIER_GIRISIMCI_BUSINESSES = 2;
        public const int TIER_GIRISIMCI_COMBOS = 1;
        public const int TIER_SIRKET_BUSINESSES = 3;
        public const int TIER_SIRKET_COMBOS = 2;
        public const int TIER_SIRKET_TERRITORIES = 4;
        public const int TIER_HOLDING_BUSINESSES = 3;
        public const int TIER_HOLDING_COMBOS = 3;
        public const int TIER_HOLDING_TERRITORIES = 5;
        public const int TIER_SCORE_GIRISIMCI = 200;
        public const int TIER_SCORE_SIRKET = 500;
        public const int TIER_SCORE_HOLDING = 1000;
        #endregion

        #region First Venture (GDD Section 1.5)
        public const int KARANLIK_PAZAR_BONUS_MONEY = 200;  // Extra $200 for black market
        #endregion
    }
}
