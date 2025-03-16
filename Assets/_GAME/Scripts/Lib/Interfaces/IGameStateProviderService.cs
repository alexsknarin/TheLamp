using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IGameStateProviderService
    {
        public GameState Get();
        public void SaveCurrentState();
        public void SaveDefaultState();
        public void SaveUpgradesOnly();
    }
}
