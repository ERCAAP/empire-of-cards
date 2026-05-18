namespace EmpireOfCards.Core
{
    /// <summary>
    /// Central repository for all game-balance constants.
    /// </summary>
    public static class Constants
    {
        // --- Economy ---
        public const int STARTING_MONEY = 500;
        public const float SELL_RATE = 0.4f;

        // --- Turns ---
        public const int MAX_TURNS = 20;

        // --- Actions ---
        public const int STARTING_ACTIONS = 3;
        public const int MAX_ACTIONS = 5;

        // --- Business Slots ---
        public const int STARTING_SLOTS = 3;
        public const int MAX_SLOTS = 5;

        // --- Hand ---
        public const int HAND_SIZE = 5;
        public const int REDRAWS_PER_TURN = 1;

        // --- Tax ---
        public const float TAX_RATE = 0.15f;
        public const float REDUCED_TAX_RATE = 0.075f;
        public const float MIN_TAX_RATE = 0.03f;

        // --- FBI ---
        public const int FBI_RAID_PENALTY = 300;

        // --- Territory ---
        public const int TERRITORY_COUNT = 10;
        public const int WIN_TERRITORIES = 6;
        public const int LOSE_TERRITORIES = 7;

        // --- Shop ---
        public const int SHOP_CARDS_PER_TURN = 3;
    }
}
