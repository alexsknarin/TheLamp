public interface IGameStateProviderService
{
    public GameState Get();
    public void SaveCurrentState();
    public void SaveDefaultState();
    public void SaveUpgradesOnly();
}
