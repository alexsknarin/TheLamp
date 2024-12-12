using UnityEngine;

public class PlayerPrefsGameSettingsProvider : IGameSettingsProvider
{
    private GameSettings _gameSettings = null;
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
            // Generate default settings TODO: Move to a separate storage, Use factory
            _gameSettings = new GameSettings();
            _gameSettings.IsConsentSet = false;
            _gameSettings.IsDataCollectionEnabled = false;
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
