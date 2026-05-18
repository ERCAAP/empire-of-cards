using EmpireOfCards.Core.StateMachine;

namespace EmpireOfCards.Core.GameStates
{
    /// <summary>
    /// State responsible for setting up a new game run.
    /// Enter initializes all systems via GameManager.StartNewRun().
    /// Tick polls _setupComplete and transitions to InGameState.
    /// </summary>
    public class GameSetupState : IState
    {
        private StateMachine.StateMachine _stateMachine;
        private IState _nextState;
        private bool _setupComplete;

        public GameSetupState(StateMachine.StateMachine sm, IState next)
        {
            _stateMachine = sm;
            _nextState = next;
        }

        public void Enter()
        {
            _setupComplete = false;
            var gm = GameManager.Instance;
            gm.SetGameState(GameState.GameSetup);
            gm.StartNewRun();
            _setupComplete = true;
        }

        public void Tick()
        {
            if (_setupComplete)
                _stateMachine.ChangeState(_nextState);
        }

        public void Exit() { }
    }
}
