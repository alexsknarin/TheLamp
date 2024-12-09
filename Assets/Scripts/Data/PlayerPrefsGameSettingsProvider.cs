using UnityEngine;

public class PlayerPrefsGameSettingsProvider : IGameSettingsProvider
{
    private GameSettings _gameSettings;
    public GameSettings Get()
    {
        if (PlayerPrefs.HasKey("GameSettings"))
        {
            string settingsJson = PlayerPrefs.GetString("GameSettings");
            var s = JsonUtility.FromJson<GameSettings>(settingsJson);
            _gameSettings = JsonUtility.FromJson<GameSettings>(settingsJson);
            return _gameSettings;
        }
        else
        {
            // Generate default settings TODO: Move to a separate storage
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
