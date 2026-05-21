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

            // Each step is handled by the Gameplay layer (EconomyManager,
            // BoardManager, etc.) listening to OnBoardTicked or running
            // their own resolve logic triggered by OnPhaseStarted.
            //
            // The Core layer drives timing; the Gameplay layer owns the math.
            //
            // Resolve order per GDD section 16.2:
            // 1. Kitchen+Staff+Supplier -> Quality+Capacity
            // 2. Hygiene update
            // 3. Marketing -> Demand
            // 4. Rating update
            // 5. Customer flow (served / waited / left)
            // 6. Income / expense / tax / credit interest
            // 7. Staff morale, experience, loyalty
            // 8. Market share shift

            EventBus.BoardTicked();
        }
    }
}
