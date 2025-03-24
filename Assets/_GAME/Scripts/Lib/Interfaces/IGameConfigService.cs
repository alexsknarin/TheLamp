using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IGameConfigService
    {
        public SpawnQueueData SpawnQueueConfig { get; }
        public ScoreConfig ScoreConfig { get; }
        public PlayerConfig PlayerConfig { get; }
        public GameConfig GameConfig { get; }
    }
}
