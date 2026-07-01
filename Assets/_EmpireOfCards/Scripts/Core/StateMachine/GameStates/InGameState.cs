namespace EmpireOfCards.Core.StateMachine.GameStates
{
    public class InGameState : IState
    {
        readonly StateMachine stateMachine;

        public InGameState(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            EventBus.OnGameOver += HandleGameOver;
        }

        public void Tick(float deltaTime)
        {
            // TurnManager.Update() handles phase ticking via MonoBehaviour.
            // This state exists to represent the "in-game" meta-state
            // and listen for game-ending conditions.
        }

        public void Exit()
        {
            EventBus.OnGameOver -= HandleGameOver;
        }

        void HandleGameOver(bool won, string reason)
        {
            stateMachine.ChangeState(new GameOverState(stateMachine, won, reason));
        }
    }
}
