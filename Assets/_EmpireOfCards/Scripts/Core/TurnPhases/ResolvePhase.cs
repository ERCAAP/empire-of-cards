namespace EmpireOfCards.Core.TurnPhases
{
    public class ResolvePhase : ITurnPhase
    {
        readonly TurnManager turnManager;

        static readonly ResolveStep[] StepOrder =
        {
            ResolveStep.CalculateMetrics,
            ResolveStep.UpdateHygiene,
            ResolveStep.CalculateDemand,
            ResolveStep.UpdateRating,
            ResolveStep.CustomerFlow,
            ResolveStep.CalculateIncome,
            ResolveStep.UpdateStaff,
            ResolveStep.UpdateMarketShare
        };

        int stepIndex;
        float stepElapsed;

        public ResolvePhase(TurnManager turnManager)
        {
            this.turnManager = turnManager;
        }

        public void Enter()
        {
            stepIndex = 0;
            stepElapsed = 0f;
            ExecuteCurrentStep();
        }

        public void Tick(float deltaTime)
        {
            stepElapsed += deltaTime;
            if (stepElapsed >= Constants.RESOLVE_STEP_DURATION)
            {
                stepIndex++;
                stepElapsed = 0f;

                if (stepIndex < StepOrder.Length)
                {
                    ExecuteCurrentStep();
                }
                else
                {
                    turnManager.CompleteCurrentPhase();
                }
            }
        }

        public void Exit() { }

        void ExecuteCurrentStep()
        {
            ResolveStep step = StepOrder[stepIndex];
            EventBus.ResolveStepFired(step);
        }
    }
}
