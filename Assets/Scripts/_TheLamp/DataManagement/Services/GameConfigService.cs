public class GameConfigService : IGameConfigService
{
    private readonly IGameConfigProvider _gameConfigProvider;

    public GameConfigService(IGameConfigProvider gameConfigProvider)
    {
        _gameConfigProvider = gameConfigProvider;
    }

    public SpawnQueueData SpawnQueueConfig => _gameConfigProvider.SpawnQueueData;
    public ScoreConfig ScoreConfig => _gameConfigProvider.ScoreConfig;
    public PlayerConfig PlayerConfig => _gameConfigProvider.PlayerConfig;
    public GameConfig GameConfig => _gameConfigProvider.GameConfig;
}
