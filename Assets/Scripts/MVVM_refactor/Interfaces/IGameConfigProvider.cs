public interface IGameConfigProvider
{
    public SpawnQueueData SpawnQueueData { get; }
    public ScoreConfig ScoreConfig { get; }
    public PlayerConfig PlayerConfig { get; }
}
