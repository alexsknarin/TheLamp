using UnityEngine;

public class PlayerPrefsGameStateProvider : IGameStateProvider
{
    private GameState _gameState = null;
    
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
            // TODO: make in updateble after deployment !!! 
            
            _gameState = new GameState();
            
            _gameState.Wave = 0;
            _gameState.Scores = 0;
            _gameState.LampHealth = 8;
            _gameState.LampMaxHealth = 8;
            _gameState.LampCooldownTime = 3.0f;
            _gameState.LampAttackDistance = 0.62f;
            _gameState.LampLevel = 0;
            _gameState.LampXp = 0;
            _gameState.LampXpThreshold = 5;
            _gameState.LampXpIncrement = 5;
            _gameState.ImpactLastPointNumber = 0;
            _gameState.LampDamageWeightRight = 0;
            _gameState.LampDamageWeightLeft = 0;
            _gameState.LampDamageWeightBottom = 0;
            _gameState.LampDamagePoint01 = new ();
            _gameState.LampDamagePoint02 = new ();
            _gameState.LampDamagePoint03 = new();
            
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
