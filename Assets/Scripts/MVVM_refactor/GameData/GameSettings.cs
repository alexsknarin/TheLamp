[System.Serializable]
public class GameSettings
{
    public GameSettings()
    {
    }
    
    public GameSettings(GameSettings gameSettings)
    {
        IsConsentSet = gameSettings.IsConsentSet;
        IsDataCollectionEnabled = gameSettings.IsDataCollectionEnabled;
    }
    
    public bool IsConsentSet;
    public bool IsDataCollectionEnabled;
}
