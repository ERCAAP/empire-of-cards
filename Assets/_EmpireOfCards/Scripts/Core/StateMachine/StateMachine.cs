namespace EmpireOfCards.Core.StateMachine
{
    public class StateMachine
    {
        IState currentState;

        public IState CurrentState => currentState;

        public void ChangeState(IState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        public void Tick(float deltaTime)
        {
            currentState?.Tick(deltaTime);
        }
    }
}
