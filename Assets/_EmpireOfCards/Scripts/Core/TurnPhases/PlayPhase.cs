namespace EmpireOfCards.Core.TurnPhases
{
    public class PlayPhase : ITurnPhase
    {
        readonly TurnManager turnManager;
        bool endTurnRequested;

        public PlayPhase(TurnManager turnManager)
        {
            this.turnManager = turnManager;
        }

        public void Enter()
        {
            endTurnRequested = false;
            EventBus.OnActionsChanged += HandleActionsChanged;
        }

        public void Tick(float deltaTime)
        {
            if (endTurnRequested)
            {
                turnManager.CompleteCurrentPhase();
                return;
            }

            var gm = GameManager.Instance;
            if (gm != null && !gm.Resources.HasActions())
                turnManager.CompleteCurrentPhase();
        }

        public void Exit()
        {
            EventBus.OnActionsChanged -= HandleActionsChanged;
        }

        public void RequestEndTurn()
        {
            endTurnRequested = true;
        }

        void HandleActionsChanged(int remaining)
        {
            if (remaining <= 0)
                endTurnRequested = true;
        }
    }
}
