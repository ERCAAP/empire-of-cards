namespace EmpireOfCards.Core.TurnPhases
{
    public interface ITurnPhase
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
