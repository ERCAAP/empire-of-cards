namespace EmpireOfCards.Core
{
    // ── Card placement and type ──────────────────────────────────────

    public enum CardType
    {
        Operation,
        Staff,
        Marketing,
        Supplier,
        Risk,
        Reaction,
        Crisis
    }

    public enum CardFamily
    {
        Setup,
        Growth,
        Risk,
        Reaction,
        Crisis,
        Neutral
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public enum SlotType
    {
        Kitchen,
        Salon,
        Storage,
        Marketing,
        TempEffect
    }

    public enum DropZoneType
    {
        KitchenSlot,
        SalonSlot,
        StorageSlot,
        MarketingSlot,
        TempEffectSlot,
        DiscardZone
    }

    // ── Game flow ────────────────────────────────────────────────────

    public enum TurnPhase
    {
        DrawPhase,
        PlanningPhase,
        PlayPhase,
        ResolvePhase,
        CrisisReactionPhase,
        RivalPhase,
        MarketUpdatePhase
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

    public enum Era
    {
        Garage,
        Growth,
        Scale,
        Dominance
    }

    public enum SeasonType
    {
        Spring,
        Summer,
        Autumn,
        Winter,
        Ramadan
    }

    // ── Resolve ──────────────────────────────────────────────────────

    public enum ResolveStep
    {
        CalculateMetrics,
        UpdateHygiene,
        CalculateDemand,
        UpdateRating,
        CustomerFlow,
        CalculateIncome,
        UpdateStaff,
        UpdateMarketShare
    }

    // ── Sector (unlock system - MVP only Restaurant) ─────────────────

    public enum SectorType
    {
        Restaurant,
        TechApp,
        Fashion,
        Grocery,
        Fintech
    }

    // ── Customer ─────────────────────────────────────────────────────

    public enum CustomerType
    {
        BargainHunter,
        Gourmet,
        Loyal,
        Influencer,
        Family,
        Random
    }

    // ── Staff ────────────────────────────────────────────────────────

    public enum StaffTier
    {
        Intern,
        Junior,
        Experienced,
        Senior,
        Master
    }

    // ── Rival ────────────────────────────────────────────────────────

    public enum RivalStrategy
    {
        AggressiveMarketing,
        PremiumQuality,
        CheapExpansion,
        Defensive,
        DirtyPlay
    }

    public enum RivalMove
    {
        PriceWar,
        MarketingBlitz,
        QualityPush,
        StaffPoach,
        Sabotage,
        Expand,
        Stabilize
    }

    // ── Economy ──────────────────────────────────────────────────────

    public enum SalaryChoice
    {
        PayOnTime,
        Delay,
        PartialPay,
        Advance
    }

    public enum CreditType
    {
        Micro,
        Business,
        BigInvestment,
        Emergency
    }

    public enum MenuPricing
    {
        Economy,
        Standard,
        Premium,
        Seasonal
    }

    // ── Crisis ───────────────────────────────────────────────────────

    public enum CrisisType
    {
        None,
        ReviewBomb,
        HygieneInspection,
        StaffQuit,
        SupplyShortage,
        RentIncrease,
        ViralBadVideo,
        FoodPoisoning,
        RivalPriceCut,
        StaffTheft,
        StreetConstruction,
        Pandemic
    }

    // ── Board pressure ───────────────────────────────────────────────

    public enum BoardPressureType
    {
        None,
        LowDemand,
        CapacityShortfall,
        LowRating,
        HighLegalRisk,
        LowCash,
        WeakQuality,
        StaffInstability,
        LowHygiene
    }

    // ── Legal ────────────────────────────────────────────────────────

    public enum LegalRiskLevel
    {
        Safe,
        Caution,
        Danger,
        Critical
    }

    // ── Score ────────────────────────────────────────────────────────

    public enum ScoreGrade
    {
        S,
        A,
        B,
        C,
        D,
        F
    }
}
