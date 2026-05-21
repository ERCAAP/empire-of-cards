namespace EmpireOfCards.Core.TurnPhases
{
    public class CrisisReactionPhase : ITurnPhase
    {
        readonly TurnManager turnManager;
        float elapsed;
        bool crisisActive;
        bool crisisHandled;

        public CrisisReactionPhase(TurnManager turnManager)
        {
            this.turnManager = turnManager;
        }

        public void Enter()
        {
            elapsed = 0f;
            crisisActive = false;
            crisisHandled = false;

            EventBus.OnCrisisTriggered += HandleCrisisTriggered;
            EventBus.OnCrisisResolved += HandleCrisisResolved;

            // Gameplay layer evaluates crisis conditions on PhaseStarted
            // and fires CrisisTriggered if one occurs.
        }

        public void Tick(float deltaTime)
        {
            elapsed += deltaTime;

            if (crisisActive)
            {
                if (crisisHandled || elapsed >= Constants.CRISIS_DURATION)
                    turnManager.CompleteCurrentPhase();
            }
            else
            {
                if (elapsed >= Constants.CRISIS_NO_CRISIS_DURATION)
                    turnManager.CompleteCurrentPhase();
            }
        }

        public void Exit()
        {
            EventBus.OnCrisisTriggered -= HandleCrisisTriggered;
            EventBus.OnCrisisResolved -= HandleCrisisResolved;
        }

        void HandleCrisisTriggered(CrisisType crisis)
        {
            if (crisis != CrisisType.None)
                crisisActive = true;
        }

        void HandleCrisisResolved(CrisisType crisis, string choiceId)
        {
            crisisHandled = true;
        }
    }
}
