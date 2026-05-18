namespace EmpireOfCards.Core
{
    public enum CardType
    {
        Business,
        Employee,
        Action,
        Upgrade,
        Event
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum CardTag
    {
        Food,
        Coffee,
        Tech,
        Marketing,
        Finance,
        Illegal,
        Trendy,
        Basic,
        Chain,
        Startup,
        Nightlife,
        Entertainment,
        Organic,
        Support,
        Crypto,
        Risky,
        Aggressive,
        Pricing,
        Investor,
        Hiring,
        Desperate,
        Office,
        Automation,
        Logistics,
        Security,
        AI,
        Guru,
        Influencer,
        Management,
        Viral
    }

    public enum TurnPhase
    {
        EventPhase,
        DrawPhase,
        PlayPhase,
        ResolvePhase,
        RivalPhase,
        TurnEnd
    }

    public enum GameState
    {
        MainMenu,
        GameSetup,
        InGame,
        Paused,
        GameOver,
        ScoreScreen
    }

    public enum RivalPersonality
    {
        Balanced,
        Aggressive,
        Defensive,
        Economic
    }

    public enum ComboTier
    {
        Easy,
        Medium,
        Hard,
        Automatic
    }
}
