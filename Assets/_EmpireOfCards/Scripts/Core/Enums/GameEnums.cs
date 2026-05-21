namespace EmpireOfCards.Core
{
    public enum CardType
    {
        Business,   // Blue - placed in slots, permanent, generates income
        Employee,   // Green - placed in businesses, passive + active ability
        Action,     // Red - single use, powerful effect
        Upgrade,    // Purple - permanent improvement
        Event       // Yellow - automatic, changes the world
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
        AI, Guru, Influencer, Management, Viral,
        Franchise, Luxury, Consulting, Defensive, Scaling
    }

    // 7-phase turn system from GDD v4 Section 9.1
    public enum TurnPhase
    {
        DrawPhase,              // Step 1: Draw cards
        PlanningPhase,          // Step 2: Player reviews board state
        PlayPhase,              // Step 3: Player spends actions
        ResolvePhase,           // Step 4: Board resolves - income, customers, staff, chains
        CrisisReactionPhase,    // Step 5: Crises fire after resolve
        RivalPhase,             // Step 6: Rival AI plays
        MarketUpdatePhase       // Step 7: Final market recalculation, season update
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

    // GDD Section 8: Rival personalities
    public enum RivalPersonality
    {
        Balanced,       // MegaCorp - Normal difficulty
        Aggressive,     // Shadow Inc. (post-MVP)
        Economic        // The Cartel (post-MVP)
    }

    // Resolve phase sub-steps from GDD Section 4.2 Step 4
    public enum ResolveStep
    {
        BusinessProduce,        // 4a: Businesses produce
        CustomerFlow,           // 4b: Customers flow / territory
        SeasonCheck,            // 4b.5: Season transition check
        MarketShareCalculation, // 4b.6: Market share calculation
        IncomeCalculation,      // 4d: Income calculated
        StaffTick,              // 4e: Staff moral/fatigue/loyalty/XP tick (GDD 6.1)
        ChainReactionCheck      // 4f: Chain reaction evaluation (GDD 11)
    }

    // Employee active ability types from GDD Section 3.2
    public enum ActiveAbilityType
    {
        None,
        MultiplyCustomersThisTurn,  // Barista: customers x2 this turn
        AddCustomersThisTurn,       // Intern: +3 customers this turn
        MultiplyIncomeThisTurn,     // Chef: income x1.5 this turn
        StealCustomersFromRival,    // Influencer: steal 5 customers from rival
        AddCustomersToAll,          // Marketing Guru: +3 to all businesses
        NullifyTaxThisTurn,         // Accountant: 0% tax this turn
        BonusIncomeWithPenalty,     // Fraudster: +300 but -150 next turn
        MotivateAllEmployees,       // Loyal Manager: all employees +1 customer
        ScaleIncomePerTurn,         // Consultant: income grows each turn employee is active
        ReduceRivalCustomers,       // Bouncer: remove customers from rival each turn
        CopyRandomEmployeeAbility,  // Headhunter: activates a random ally employee ability
        SabotageCostIncrease        // Lobbyist: rival pays more for businesses
    }

    // Action card effect types from GDD Section 3.3
    public enum ActionEffectType
    {
        None,
        AddCustomersToRandom,       // Flyer: +3 customers to random business
        AddMoneyInstant,            // Small Investment: +150 instant
        MultiplyAllCustomers,       // Viral Marketing: all customers x2
        CloseRivalWeakestBusiness,  // Hostile Takeover: close rival's weakest
        AddCustomersWithFBI,        // Fake Reviews: +8 customers, FBI +12%
        StealCustomersHalfIncome,   // Price Slashing: sacrifice 50% income, steal 8 customers
        DisableRivalOneTurn,        // Sabotage: rival can't produce for 1 turn, FBI +15%
        MoneyNowPayLater,           // Investor Pitch: +600 instant, 3 turns 15%
        DrawAndPlayEmployee,        // Emergency Hire: draw random employee and play
        SacrificeBusiness,          // Liquidation: sell business, get 2x price
        SwapBusinessWithRival,      // Hostile Merger: swap your weakest business with rival's strongest
        GambleDoubleOrNothing,      // All In: double your money or lose half
        ProtectBusinessOneTurn,     // Insurance Claim: one business immune to events/rival this turn
        Overtime                    // Overtime: +50% capacity, +2 fatigue, 3 consecutive = strike (GDD 6.3)
    }

    // Upgrade effect types from GDD Section 3.4
    public enum UpgradeEffectType
    {
        None,
        IncomePercentSingle,        // Office Supplies: 1 business +10%
        IncomePercentWithSlotLoss,  // Automation: +30% but 1 employee slot closed
        GlobalCustomerPerTurn,      // Delivery Network: +2 customers/turn to all businesses
        GlobalCustomerFlat,         // Billboard: +3 customers/turn global
        ReduceFBIRisk,              // Security System: FBI risk -25%
        ExtraAction,                // AI Assistant: +1 action
        IncomePerEmployeeSingle,    // Break Room: +15 income per employee in this business
        RivalCostIncrease           // Patent Wall: rival businesses cost 25% more
    }

    // Event effect types from GDD Section 3.5
    public enum EventEffectType
    {
        None,
        TagCustomerBoost,           // Coffee Craze: food/coffee +50% customers
        AllIncomeReduction,         // Economic Crisis: all income -30%
        TagDoubleEffect,            // Viral Trend: marketing cards 2x
        TagCustomerPenalty,         // Data Breach: tech -5 customers
        TagDoubleEffectFinance,     // Investor Season: finance cards 2x
        HighFBICustomerPenalty,     // Cancel Culture: FBI>30% -> customers -40%
        TerritoryScramble,          // Gold Rush: unclaimed territories generate bonus income
        ShopFloodRare               // Black Friday: shop has 5 cards, all rare+, 30% off
    }

    // Business evolution levels from GDD Section 3.1
    public enum BusinessLevel
    {
        Level1, // Diner
        Level2, // Shop
        Level3  // Store/Chain
    }

    // 5 selectable venture types at game start (GDD v3.0 Section 1.5)
    public enum VentureType
    {
        FastFood,       // Fast food restaurant — high volume, low margin
        Cafe,           // Coffee shop — loyalty-driven, barista-dependent
        TechApp,        // Mobile/web app — platform fees, viral potential
        ClothingStore,  // Retail fashion — seasonal cycles, trend-sensitive
        GroceryStore    // Neighborhood market — spoilage risk, veresiye system
    }

    // 5 slot types replacing the old single-business-slot system (GDD v3.0 Section 4)
    public enum SlotType
    {
        Operation,      // Physical infrastructure (table, kitchen, server)
        Staff,          // Employees (cook, barista, developer)
        Marketing,      // Ad campaigns (flyer, influencer, google ads)
        Supplier,       // Supply deals (butcher, firebase, organic)
        TempEffect      // Temporary events/crises
    }

    // World-space drop targets used by board presentation and drag/drop routing.
    public enum DropZoneType
    {
        BusinessSlot,
        EmployeeSlot,
        UpgradeSlot,
        ActionZone,
        SellZone,
        BurnZone,
        OperationSlot,
        StaffSlot,
        MarketingSlot,
        SupplierSlot,
        TempEffectSlot
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

    public enum BoardPressureType
    {
        None,
        LowDemand,
        CapacityShortfall,
        LowRating,
        HighLegalRisk,
        LowCash,
        WeakQuality,
        StaffInstability
    }

    // 5-season cycle, 5 turns each (GDD v3.0 Section 14)
    public enum SeasonType
    {
        Spring,         // Balanced demand
        Summer,         // High foot traffic, heat events
        Autumn,         // Back-to-school, fashion transition
        Winter,         // Holiday surge, cold chain costs
        RamadanSeason   // Demand shift (food/cafe affected)
    }

    // Legal risk level brackets (GDD v3.0 Section 13)
    public enum LegalRiskLevel
    {
        Safe,           // 0-25: No risk
        Caution,        // 26-50: Minor penalties
        Danger,         // 51-75: FBI interest, income penalty
        Certain         // 76-100: Raid imminent
    }

    // Customer segments for market pool (GDD v3.0 Section 7)
    public enum CustomerSegment
    {
        PriceSensitive,     // Chooses cheapest option
        QualitySeeking,     // Chooses highest quality score
        TrendFollower,      // Chooses highest platform rating
        LoyalRegular,       // Sticks with established business
        NewCustomer         // First-time, influenced by marketing
    }

    // Rival AI moves (GDD v3.0 Section 15)
    public enum RivalMove
    {
        PriceWar,           // Lower prices, steal price-sensitive customers
        MarketingBlitz,     // Boost platform rating, steal trend followers
        QualityImprove,     // Raise quality score
        StaffPoach,         // Steal a staff card effect
        SeekInvestment,     // Gain extra income this turn
        OpenBranch,         // Add a slot (increases rival capacity)
        Sabotage            // Target player's highest-income slot
    }

    // Salary payment choice (GDD Section 5.5)
    public enum SalaryChoice
    {
        PayOnTime,      // Full payment, no penalty
        Delay,          // Skip payment, moral penalty + quit chance
        PartialPay,     // Partial payment, minor moral penalty
        Advance         // Pay extra, moral bonus
    }

    // Insurance type for employees (GDD Section 5.6)
    public enum InsuranceType
    {
        FullSGK,        // Legal, costs 37% extra
        Uninsured,      // High risk, no cost
        DailyWage       // Medium risk, no benefits
    }

    // Credit types available from bank (GDD Section 5.7)
    public enum CreditType
    {
        SmallBusiness,      // Low amount, low interest
        Medium,             // Medium amount, medium interest
        LargeInvestment,    // High amount, high interest
        Emergency           // Quick cash, highest interest
    }

    // Location tiers affecting passive customers and rent (GDD Section 10.1)
    public enum LocationType
    {
        RemoteCorner,       // Cheap rent, no passive customers
        SideStreet,         // Low rent, few passive customers
        MainStreet,         // Medium rent, good foot traffic
        ShoppingMall        // High rent, best foot traffic
    }
}
