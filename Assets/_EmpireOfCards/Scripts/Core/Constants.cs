namespace EmpireOfCards.Core
{
    public static class Constants
    {
        // GDD Section 9: Economy
        public const int STARTING_MONEY = 500;
        public const float SELL_RATE = 0.4f;        // Sell card = 40%

        // GDD Section 10: Run structure
        public const int MAX_TURNS = 20;             // Premortem revision: 25->20

        // GDD Section 4: Turn mechanics
        public const int STARTING_ACTIONS = 3;
        public const int MAX_ACTIONS = 5;            // Max with AI Assistant
        public const int HAND_SIZE = 5;              // Draw 5 cards each turn
        public const int REDRAWS_PER_TURN = 1;

        // GDD Section 5: Slot system
        public const int STARTING_SLOTS = 3;
        public const int MAX_SLOTS = 5;

        // GDD Section 9.2: Tax
        public const float TAX_RATE = 0.15f;
        public const float REDUCED_TAX_RATE = 0.075f;  // 1 accountant
        public const float MIN_TAX_RATE = 0.03f;        // 2 accountants

        // GDD Section 9.3: FBI
        public const int FBI_RAID_PENALTY = 300;

        // GDD Section 6: Territory map
        public const int TERRITORY_COUNT = 10;
        public const int WIN_TERRITORIES = 6;        // 60% market share
        public const int LOSE_TERRITORIES = 7;       // Rival takes 7 territories

        // GDD Section 2.2: Shop
        public const int SHOP_CARDS_PER_TURN = 3;

        // GDD Section 4.1: Event frequency
        public const int EVENT_INTERVAL = 3;         // 1 event every 3 turns

        // GDD Section 2.2: Starter deck
        public const int STARTING_DECK_SIZE = 14;

        // Market Pool - GDD Balance table
        public const int BASE_MARKET_CUSTOMERS = 60;
        public const int EARLY_GROWTH_PER_TURN = 5;   // Turn 1-5
        public const int MID_GROWTH_PER_TURN = 6;     // Turn 6-10
        public const int LATE_GROWTH_PER_TURN = 8;    // Turn 11-15
        public const int END_GROWTH_PER_TURN = 10;    // Turn 16-20

        // Scoring - GDD Section 10.3
        public const int SCORE_TERRITORY = 500;
        public const int SCORE_MONEY = 1;
        public const int SCORE_COMBO = 200;
        public const int SCORE_BUSINESS = 100;
        public const int SCORE_EARLY_FINISH = 300;    // Remaining turns x 300
        public const int SCORE_FBI_EVASION = 50;
        public const int SCORE_WIN_BONUS = 1000;

        // Business Evolution - GDD Section 3.1
        public const int EVOLUTION_CUSTOMER_THRESHOLD = 40; // Level up after 40 customers
        public const int EVOLUTION_TURN_REQUIREMENT = 15;   // Over 15 turns

        // Employee leaving - GDD Section 4.2
        public const int EMPLOYEE_LEAVE_TURN_THRESHOLD = 8; // Employee working 8+ turns
    }
}
