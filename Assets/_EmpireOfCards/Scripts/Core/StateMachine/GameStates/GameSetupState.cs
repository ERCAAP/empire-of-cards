namespace EmpireOfCards.Core.StateMachine.GameStates
{
    public class GameSetupState : IState
    {
        readonly StateMachine stateMachine;

        public GameSetupState(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            // UI layer shows business name input + sector selection.
            // On confirm, calls GameManager.StartNewRun() and transitions to InGameState.
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}
