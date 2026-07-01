namespace EmpireOfCards.Core.StateMachine.GameStates
{
    public class MainMenuState : IState
    {
        readonly StateMachine stateMachine;

        public MainMenuState(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public void Enter()
        {
            // UI layer shows main menu panel.
            // Player can start new run or quit.
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}
