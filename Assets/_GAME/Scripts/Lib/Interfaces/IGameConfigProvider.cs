using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IGameConfigProvider
    {
        public SpawnQueueData SpawnQueueData { get; }
        public ScoreConfig ScoreConfig { get; }
        public PlayerConfig PlayerConfig { get; }
        public GameConfig GameConfig { get; }
    }
}
