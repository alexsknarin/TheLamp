public class GameConfigService : IGameConfigService
{
    private readonly IGameConfigProvider _gameConfigProvider;
    public SpawnQueueData SpawnQueueConfig => _gameConfigProvider.SpawnQueueData;
    public ScoreConfig ScoreConfig => _gameConfigProvider.ScoreConfig;
    public PlayerConfig PlayerConfig => _gameConfigProvider.PlayerConfig;
    public GameConfig GameConfig => _gameConfigProvider.GameConfig;

    public GameConfigService(IGameConfigProvider gameConfigProvider)
    {
        _gameConfigProvider = gameConfigProvider;
    }
}
