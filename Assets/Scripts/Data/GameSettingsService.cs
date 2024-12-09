using UnityEngine;

public class GameSettingsService: IGameSettingsService
{
    private GameSettingsModel _gameSettingsModel;
    private IGameSettingsProvider _gameSettingsProvider;
    
    public GameSettingsService(IGameSettingsProvider gameSettingsProvider)
    {
        _gameSettingsProvider = gameSettingsProvider;
        Debug.Log("GameSettingsService created, dependencies injected");
    }
    
    public void Save()
    {
        _gameSettingsProvider.Save();
    }
}
