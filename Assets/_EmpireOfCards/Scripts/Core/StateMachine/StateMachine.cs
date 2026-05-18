namespace EmpireOfCards.Core.StateMachine
{
    /// <summary>
    /// Generic state machine that manages IState transitions.
    /// Call Update() each frame to execute the current state's logic.
    /// </summary>
    public class StateMachine
    {
        private IState currentState;
        private IState previousState;

        /// <summary>
        /// Transitions from the current state to the given new state.
        /// Calls Exit on the old state and Enter on the new state.
        /// </summary>
        public void ChangeState(IState newState)
        {
            if (newState == null)
                return;

            if (currentState != null)
            {
                currentState.Exit();
                previousState = currentState;
            }

            currentState = newState;
            currentState.Enter();
        }

        /// <summary>
        /// Executes the current state's per-frame logic.
        /// Should be called from MonoBehaviour.Update().
        /// </summary>
        public void Update()
        {
            if (currentState != null)
            {
                currentState.Execute();
            }
        }

        /// <summary>
        /// Returns the currently active state.
        /// </summary>
        public IState GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Returns the state that was active before the current one.
        /// </summary>
        public IState GetPreviousState()
        {
            return previousState;
        }
    }
}
