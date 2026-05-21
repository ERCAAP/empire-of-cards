namespace EmpireOfCards.Core.StateMachine.GameStates
{
    public class GameOverState : IState
    {
        readonly StateMachine stateMachine;
        readonly bool won;
        readonly string reason;

        public bool Won => won;
        public string Reason => reason;

        public GameOverState(StateMachine stateMachine, bool won, string reason)
        {
            this.stateMachine = stateMachine;
            this.won = won;
            this.reason = reason;
        }

        public void Enter()
        {
            // UI layer listens to OnGameOver and shows score screen.
            // Score calculation uses Constants.SCORE_* values.
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}
