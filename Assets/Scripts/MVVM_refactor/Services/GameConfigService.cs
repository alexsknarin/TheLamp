public class GameConfigService : IGameConfigService
{
    private readonly IGameConfigProviderService _gameConfigProviderService;
    public SpawnQueueData SpawnQueueConfig => _gameConfigProviderService.SpawnQueueData;
    public ScoreConfig ScoreConfig => _gameConfigProviderService.ScoreConfig;
    public PlayerConfig PlayerConfig => _gameConfigProviderService.PlayerConfig;
    public GameConfig GameConfig => _gameConfigProviderService.GameConfig;

    public GameConfigService(IGameConfigProviderService gameConfigProviderService)
    {
        _gameConfigProviderService = gameConfigProviderService;
    }
}
