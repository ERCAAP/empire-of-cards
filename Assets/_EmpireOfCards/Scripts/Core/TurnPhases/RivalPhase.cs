namespace EmpireOfCards.Core.TurnPhases
{
    public class RivalPhase : ITurnPhase
    {
        readonly TurnManager turnManager;
        float elapsed;

        public RivalPhase(TurnManager turnManager)
        {
            this.turnManager = turnManager;
        }

        public void Enter()
        {
            elapsed = 0f;
            // RivalAI listens to OnPhaseStarted(RivalPhase) and
            // executes its decision, then fires OnRivalAction.
        }

        public void Tick(float deltaTime)
        {
            elapsed += deltaTime;
            if (elapsed >= Constants.RIVAL_DURATION)
                turnManager.CompleteCurrentPhase();
        }

        public void Exit() { }
    }
}
