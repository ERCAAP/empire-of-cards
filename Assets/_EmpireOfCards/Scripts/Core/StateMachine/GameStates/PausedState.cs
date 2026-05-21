using UnityEngine;

namespace EmpireOfCards.Core.StateMachine.GameStates
{
    public class PausedState : IState
    {
        readonly StateMachine stateMachine;
        readonly IState returnState;

        public PausedState(StateMachine stateMachine, IState returnState)
        {
            this.stateMachine = stateMachine;
            this.returnState = returnState;
        }

        public void Enter()
        {
            Time.timeScale = 0f;
        }

        public void Tick(float deltaTime) { }

        public void Exit()
        {
            Time.timeScale = 1f;
        }

        public void Unpause()
        {
            stateMachine.ChangeState(returnState);
        }
    }
}
