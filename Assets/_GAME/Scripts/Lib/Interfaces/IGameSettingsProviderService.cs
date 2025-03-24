using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IGameSettingsProviderService
    {
        public GameSettings Get();
        public void Save();
    }
}
