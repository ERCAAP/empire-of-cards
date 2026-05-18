namespace EmpireOfCards.Core
{
    public static class Constants
    {
        // GDD Bölüm 9: Ekonomi
        public const int STARTING_MONEY = 500;
        public const float SELL_RATE = 0.4f;        // Kart satma = %40

        // GDD Bölüm 10: Run yapısı
        public const int MAX_TURNS = 20;             // Premortem revizyonu: 25→20

        // GDD Bölüm 4: Tur mekaniği
        public const int STARTING_ACTIONS = 3;
        public const int MAX_ACTIONS = 5;            // Yapay Zeka Asistanı ile max
        public const int HAND_SIZE = 5;              // Her tur 5 kart çek
        public const int REDRAWS_PER_TURN = 1;

        // GDD Bölüm 5: Slot sistemi
        public const int STARTING_SLOTS = 3;
        public const int MAX_SLOTS = 5;

        // GDD Bölüm 9.2: Vergi
        public const float TAX_RATE = 0.15f;
        public const float REDUCED_TAX_RATE = 0.075f;  // 1 muhasebeci
        public const float MIN_TAX_RATE = 0.03f;        // 2 muhasebeci

        // GDD Bölüm 9.3: FBI
        public const int FBI_RAID_PENALTY = 300;

        // GDD Bölüm 6: Bölge haritası
        public const int TERRITORY_COUNT = 10;
        public const int WIN_TERRITORIES = 6;        // %60 market share
        public const int LOSE_TERRITORIES = 7;       // Rakip 7 bölge alırsa

        // GDD Bölüm 2.2: Dükkan
        public const int SHOP_CARDS_PER_TURN = 3;

        // GDD Bölüm 4.1: Event sıklığı
        public const int EVENT_INTERVAL = 3;         // Her 3 turda 1 event

        // GDD Bölüm 2.2: Başlangıç destesi
        public const int STARTING_DECK_SIZE = 14;

        // Market Pool - GDD Balance tablo
        public const int BASE_MARKET_CUSTOMERS = 60;
        public const int EARLY_GROWTH_PER_TURN = 5;   // Tur 1-5
        public const int MID_GROWTH_PER_TURN = 6;     // Tur 6-10
        public const int LATE_GROWTH_PER_TURN = 8;    // Tur 11-15
        public const int END_GROWTH_PER_TURN = 10;    // Tur 16-20

        // Scoring - GDD Bölüm 10.3
        public const int SCORE_TERRITORY = 500;
        public const int SCORE_MONEY = 1;
        public const int SCORE_COMBO = 200;
        public const int SCORE_BUSINESS = 100;
        public const int SCORE_EARLY_FINISH = 300;    // Kalan tur × 300
        public const int SCORE_FBI_EVASION = 50;
        public const int SCORE_WIN_BONUS = 1000;

        // İşletme Evrimi - GDD Bölüm 3.1
        public const int EVOLUTION_CUSTOMER_THRESHOLD = 40; // 40 müşteri çekince seviye atla
        public const int EVOLUTION_TURN_REQUIREMENT = 15;   // 15 tur boyunca

        // Çalışan ayrılma - GDD Bölüm 4.2
        public const int EMPLOYEE_LEAVE_TURN_THRESHOLD = 8; // 8 turdan fazla çalışan
    }
}
