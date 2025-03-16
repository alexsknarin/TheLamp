public interface IGameConfigService
{
    public SpawnQueueData SpawnQueueConfig { get; }
    public ScoreConfig ScoreConfig { get; }
    public PlayerConfig PlayerConfig { get; }
    public GameConfig GameConfig { get; }
}
