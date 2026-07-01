namespace EmpireOfCards.Core.TurnPhases
{
    public class MarketUpdatePhase : ITurnPhase
    {
        readonly TurnManager turnManager;
        float elapsed;

        public MarketUpdatePhase(TurnManager turnManager)
        {
            this.turnManager = turnManager;
        }

        public void Enter()
        {
            elapsed = 0f;

            // Final market share recalculation and season check.
            // EconomyManager listens to OnPhaseStarted(MarketUpdatePhase)
            // and fires OnMarketShareChanged after recalc.
        }

        public void Tick(float deltaTime)
        {
            elapsed += deltaTime;
            if (elapsed >= Constants.MARKET_UPDATE_DURATION)
                turnManager.CompleteCurrentPhase();
        }

        public void Exit() { }
    }
}
