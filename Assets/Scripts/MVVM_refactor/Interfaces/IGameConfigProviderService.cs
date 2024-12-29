public interface IGameConfigProviderService
{
    public SpawnQueueData SpawnQueueData { get; }
    public ScoreConfig ScoreConfig { get; }
    public PlayerConfig PlayerConfig { get; }
    public GameConfig GameConfig { get; }
}
