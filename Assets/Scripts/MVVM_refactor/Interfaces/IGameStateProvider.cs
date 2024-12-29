public interface IGameStateProvider
{
    public GameState Get();
    public void SaveCurrentState();
}
