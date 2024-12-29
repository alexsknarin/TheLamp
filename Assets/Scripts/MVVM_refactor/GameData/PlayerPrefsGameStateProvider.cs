using UnityEngine;

public class PlayerPrefsGameStateProvider : IGameStateProvider
{
    private GameState _gameState = null;
    private GameState _defaultGameState;
    public PlayerPrefsGameStateProvider(GameState defaultGameState)
    {
        _defaultGameState = defaultGameState;
    }
    
    public GameState Get()
    {
        if (_gameState != null)
        {
            return _gameState;
        }
        
        if (PlayerPrefs.HasKey("GameState"))
        {
            Debug.Log("GameState found in PlayerPrefs");
            string gameStateJson = PlayerPrefs.GetString("GameState");
            _gameState = JsonUtility.FromJson<GameState>(gameStateJson);
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

    public void Save()
    {
        string gameStateJson = JsonUtility.ToJson(_gameState);
        PlayerPrefs.SetString("GameState", gameStateJson);
        PlayerPrefs.Save();
        Debug.Log("GameState saved to PlayerPrefs");
        Debug.Log(gameStateJson);
    }
}
