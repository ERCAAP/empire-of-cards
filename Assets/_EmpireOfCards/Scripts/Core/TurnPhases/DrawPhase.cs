namespace EmpireOfCards.Core.TurnPhases
{
    public class DrawPhase : ITurnPhase
    {
        readonly TurnManager turnManager;
        float elapsed;
        bool drawTriggered;

        public DrawPhase(TurnManager turnManager)
        {
            this.turnManager = turnManager;
        }

        public void Enter()
        {
            elapsed = 0f;
            drawTriggered = false;

            var gm = GameManager.Instance;
            if (gm != null)
                gm.Resources.ResetActions();
        }

        public void Tick(float deltaTime)
        {
            if (!drawTriggered)
            {
                drawTriggered = true;
                // DeckManager draws HAND_SIZE cards via EventBus listener
                // or WiringService-connected reference. Drawing is handled
                // by the Gameplay layer listening to OnPhaseStarted.
            }

            elapsed += deltaTime;
            if (elapsed >= Constants.DRAW_DURATION)
                turnManager.CompleteCurrentPhase();
        }

        public void Exit() { }
    }
}
