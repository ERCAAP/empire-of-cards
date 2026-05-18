namespace EmpireOfCards.Core.StateMachine
{
    public interface IState
    {
        void Enter();
        void Tick();    // Called every frame (polling)
        void Exit();
    }
}
