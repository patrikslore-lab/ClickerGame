namespace GameStateMachine
{
    public interface IGameState
    {
        void Enter();
        void Update();
        void Exit();
    }
}
