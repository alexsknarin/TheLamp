using UnityEngine;

public class PlayerPrefsGameSettingsProvider : IGameSettingsProvider
{
    private GameSettings _gameSettings = null;
    private GameSettings _defaultGameSettings;
    
    public PlayerPrefsGameSettingsProvider(GameSettings defaultGameSettings)
    {
        _defaultGameSettings = defaultGameSettings;
    }
    
    public GameSettings Get()
    {
        if (_gameSettings != null)
        {
            return _gameSettings;
        }
        
        if (PlayerPrefs.HasKey("GameSettings"))
        {
            Debug.Log("GameSettings found in PlayerPrefs");
            string settingsJson = PlayerPrefs.GetString("GameSettings");
            _gameSettings = JsonUtility.FromJson<GameSettings>(settingsJson);
            return _gameSettings;
        }
        else
        {
            Debug.Log("GameSettings not found in PlayerPrefs - Generating a new one");
            _gameSettings = new GameSettings(_defaultGameSettings);
            return _gameSettings;
        }
    }

    public void Save()
    {
        string gameSettingsJson = JsonUtility.ToJson(_gameSettings);
        PlayerPrefs.SetString("GameSettings", gameSettingsJson);
        PlayerPrefs.Save();
    }
}
