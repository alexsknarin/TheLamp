using UnityEngine;

public class PlayerPrefsGameStateProviderService : IGameStateProviderService
{
    private GameState _gameState = null;
    private GameState _defaultGameState;
    public PlayerPrefsGameStateProviderService(GameState defaultGameState)
    {
        _defaultGameState = defaultGameState;
    }
    
    public GameState Get()
    {
        if (PlayerPrefs.HasKey("GameState"))
        {
            Debug.Log("GameState found in PlayerPrefs");
            string gameStateJson = PlayerPrefs.GetString("GameState");
            GameState gameState = JsonUtility.FromJson<GameState>(gameStateJson);
            _gameState = gameState;
            return _gameState;
        }
        else
        {
            Debug.Log("GameState not found in PlayerPrefs - Generating a new one");
            
            // TODO: use a spreadsheet to generate the default values
            // TODO: make in updatable after deployment !!!
            // Use ScriptableObject to store default values for now
            
            _gameState = new GameState(_defaultGameState);
            
            return _gameState;
        }
    }

    public void SaveCurrentState()
    {
        string gameStateJson = JsonUtility.ToJson(_gameState);
        PlayerPrefs.SetString("GameState", gameStateJson);
        PlayerPrefs.Save();
        Debug.Log("GameState saved to PlayerPrefs");
        Debug.Log(gameStateJson);
    }
    
    public void SaveDefaultState()
    {
        string gameStateJson = JsonUtility.ToJson(_defaultGameState);
        PlayerPrefs.SetString("GameState", gameStateJson);
        PlayerPrefs.Save();
        Debug.Log("Default GameState saved to PlayerPrefs");
        Debug.Log(gameStateJson);
    }

    public void SaveUpgradesOnly()
    {
        var gameState = new GameState(_defaultGameState);
        gameState.LampMaxHealth = _gameState.LampMaxHealth;
        gameState.LampHealth = _gameState.LampMaxHealth;
        gameState.LampAttackDistance = _gameState.LampAttackDistance;
        gameState.LampCooldownTime = _gameState.LampCooldownTime;
        gameState.LampLevel = _gameState.LampLevel;
        gameState.LampUpgradePoints = _gameState.LampUpgradePoints;
        
        string gameStateJson = JsonUtility.ToJson(gameState);
        PlayerPrefs.SetString("GameState", gameStateJson);
        PlayerPrefs.Save();
        Debug.Log("upgrades Only GameState saved to PlayerPrefs");
        Debug.Log(gameStateJson);
    }
}
