namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IState
    {
        public void OnEnter();
        public void Tick();
        public void OnExit();
    }
}