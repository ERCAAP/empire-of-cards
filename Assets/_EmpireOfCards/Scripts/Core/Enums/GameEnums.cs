namespace EmpireOfCards.Core
{
    public enum CardType
    {
        Business,   // Mavi - slota konur, kalıcı, gelir üretir
        Employee,   // Yeşil - işletmeye konur, passif + aktif yetenek
        Action,     // Kırmızı - tek kullanım, güçlü etki
        Upgrade,    // Mor - kalıcı iyileştirme
        Event       // Sarı - otomatik, dünyayı değiştirir
    }

    public enum Rarity
    {
        Common,     // ★
        Uncommon,   // ★★
        Rare,       // ★★★
        Epic,       // ★★★★ (post-MVP)
        Legendary   // ★★★★★ (post-MVP)
    }

    // Tags directly from GDD card list - used for combo matching, synergy checks
    public enum CardTag
    {
        Food, Coffee, Tech, Marketing, Finance,
        Illegal, Trendy, Basic, Chain, Startup,
        Nightlife, Entertainment, Organic, Support, Crypto,
        Risky, Aggressive, Pricing, Investor, Hiring,
        Desperate, Office, Automation, Logistics, Security,
        AI, Guru, Influencer, Management, Viral
    }

    // 5-phase turn system from GDD Section 4
    public enum TurnPhase
    {
        EventPhase,     // Adım 1: Event gelir (her 3 turda 1)
        DrawPhase,      // Adım 2: 5 kart çek, 1 redraw
        PlayPhase,      // Adım 3: 3 aksiyon - kart koy veya yetenek kullan
        ResolvePhase,   // Adım 4: Masa çalışır - gelir, müşteri, combo, FBI
        RivalPhase      // Adım 5: Rakip AI oynar
    }

    public enum GameState
    {
        Boot,
        MainMenu,
        GameSetup,
        Playing,
        Paused,
        GameOver,
        ScoreScreen
    }

    // GDD Section 8: Rakip kişilikleri
    public enum RivalPersonality
    {
        Balanced,       // MegaCorp - Normal zorluk
        Aggressive,     // Shadow Inc. (post-MVP)
        Economic        // The Cartel (post-MVP)
    }

    public enum ComboTier
    {
        Easy,       // 2 kart, erken keşfedilir
        Medium,     // 2-3 kart veya event bağımlı
        Hard,       // 3+ kart
        Automatic   // Koşul sağlanınca otomatik (Monopol)
    }

    // Resolve phase sub-steps from GDD Section 4.2 Adım 4
    public enum ResolveStep
    {
        BusinessProduce,    // 4a: İşletmeler üretir
        CustomerFlow,       // 4b: Müşteriler kayar
        ComboCheck,         // 4c: Combo kontrolü
        IncomeCalculation,  // 4d: Gelir hesaplanır
        DeteriorationCheck  // 4e: Bozulma kontrolü (FBI, kapanma, ayrılma)
    }

    // Employee active ability types from GDD Section 3.2
    public enum ActiveAbilityType
    {
        None,
        MultiplyCustomersThisTurn,  // Barista: müşteri x2 bu tur
        AddCustomersThisTurn,       // Stajyer: +3 müşteri bu tur
        MultiplyIncomeThisTurn,     // Şef: 1 tur gelir x1.5
        StealCustomersFromRival,    // Influencer: rakipten 5 müşteri çal
        AddCustomersToAll,          // Marketing Guru: tüm işletmelere +3
        NullifyTaxThisTurn,         // Muhasebeci: bu tur vergi %0
        BonusIncomeWithPenalty,     // Dolandırıcı: +300 ama sonraki tur -150
        MotivateAllEmployees        // Sadık Müdür: tüm çalışanlar +1 müşteri
    }

    // Action card effect types from GDD Section 3.3
    public enum ActionEffectType
    {
        None,
        AddCustomersToRandom,       // El İlanı: +3 müşteri rastgele işletmeye
        AddMoneyInstant,            // Küçük Yatırım: +150 anında
        MultiplyAllCustomers,       // Viral Pazarlama: tüm müşteri x2
        CloseRivalWeakestBusiness,  // Düşmanca Devralma: rakibin en zayıfını kapat
        AddCustomersWithFBI,        // Sahte Yorumlar: +8 müşteri, FBI +%12
        StealCustomersHalfIncome,   // Fiyat Kırma: gelir %50 karşılığı 8 müşteri çal
        DisableRivalOneTurn,        // Sabotaj: rakip 1 tur üretim yapamaz, FBI +%15
        MoneyNowPayLater,           // Yatırımcı Sunumu: +600 anında, 3 tur %15
        DrawAndPlayEmployee,        // Acil İşe Alım: rastgele çalışan çek ve oyna
        SacrificeBusiness           // Tasfiye: işletmeyi sat, 2x fiyat al
    }

    // Upgrade effect types from GDD Section 3.4
    public enum UpgradeEffectType
    {
        None,
        IncomePercentSingle,        // Ofis Malzemeleri: 1 işletme +%10
        IncomePercentWithSlotLoss,  // Otomasyon: +%30 ama 1 çalışan slotu kapanır
        GlobalCustomerPerTurn,      // Teslimat Ağı: tüm işletmelere +2 müşteri/tur
        GlobalCustomerFlat,         // Reklam Panosu: +3 müşteri/tur genel
        ReduceFBIRisk,              // Güvenlik Sistemi: FBI riski -%25
        ExtraAction                 // Yapay Zeka Asistanı: +1 aksiyon hakkı
    }

    // Event effect types from GDD Section 3.5
    public enum EventEffectType
    {
        None,
        TagCustomerBoost,           // Kahve Çılgınlığı: food/coffee +%50 müşteri
        AllIncomeReduction,         // Ekonomik Kriz: tüm gelirler -%30
        TagDoubleEffect,            // Viral Trend: marketing kartları 2x
        TagCustomerPenalty,         // Veri Sızıntısı: tech -5 müşteri
        TagDoubleEffectFinance,     // Yatırımcı Sezonu: finance kartlar 2x
        HighFBICustomerPenalty      // İptal Kültürü: FBI>%30 → müşteri -%40
    }

    // Business evolution levels from GDD Section 3.1
    public enum BusinessLevel
    {
        Level1, // Büfe
        Level2, // Dükkan
        Level3  // Mağaza/Zincir
    }
}
