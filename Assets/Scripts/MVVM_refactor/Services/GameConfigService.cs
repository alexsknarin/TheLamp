public class GameConfigService : IGameConfigService
{
    private readonly IGameConfigProvider _gameConfigProvider;
    public SpawnQueueData SpawnQueueConfig => _gameConfigProvider.SpawnQueueData;
    public ScoreConfig ScoreConfig => _gameConfigProvider.ScoreConfig;

    public GameConfigService(IGameConfigProvider gameConfigProvider)
    {
        _gameConfigProvider = gameConfigProvider;
    }
}
