namespace EmpireOfCards.Core.TurnPhases
{
    public class PlanningPhase : ITurnPhase
    {
        readonly TurnManager turnManager;
        float elapsed;

        public PlanningPhase(TurnManager turnManager)
        {
            this.turnManager = turnManager;
        }

        public void Enter()
        {
            elapsed = 0f;
            // UI layer listens to OnPhaseStarted(PlanningPhase) and
            // shows dashboard summary with bottleneck highlights.
        }

        public void Tick(float deltaTime)
        {
            elapsed += deltaTime;
            if (elapsed >= Constants.PLANNING_DURATION)
                turnManager.CompleteCurrentPhase();
        }

        public void Exit() { }
    }
}
