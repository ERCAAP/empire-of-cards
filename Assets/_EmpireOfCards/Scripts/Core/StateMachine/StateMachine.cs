using System;

namespace EmpireOfCards.Core.StateMachine
{
    public class StateMachine
    {
        public IState CurrentState { get; private set; }
        public IState PreviousState { get; private set; }

        public event Action<IState, IState> OnStateChanged;  // old, new

        public void Initialize(IState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }

        public void ChangeState(IState newState)
        {
            if (newState == null || newState == CurrentState)
                return;

            IState old = CurrentState;
            CurrentState?.Exit();
            PreviousState = old;
            CurrentState = newState;
            CurrentState.Enter();

            OnStateChanged?.Invoke(old, newState);
        }

        // Call from MonoBehaviour.Update() - this is the polling
        public void Tick()
        {
            CurrentState?.Tick();
        }

        public bool IsInState<T>() where T : IState
        {
            return CurrentState is T;
        }
    }
}
