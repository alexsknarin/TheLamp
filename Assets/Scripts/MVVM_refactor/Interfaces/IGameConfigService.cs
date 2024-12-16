public interface IGameConfigService
{
    public SpawnQueueData SpawnQueueConfig { get; } // TODO: segregate into separate interfaces
    public ScoreConfig ScoreConfig { get; }
}
