using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Lamp _lamp;
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    [SerializeField] private UGSSetup _ugsSetup;
    [SerializeField] private GameStates _currentGameState;
    

    private void OnEnable()
    {
        _googleSheetsDataReader.OnDataLoaded += InitializeEnemyManager;
        _uiManager.OnIntroFinished += OnIntroEnded;
        _ugsSetup.OnConsentAddressed += HandleDataConsentAddressed;
        PlayerInputHandler.OnPlayerAttack += HandlePlayerAttackButtonPressed;
        EnemyManager.OnWaveEnded += HandleWaveEnded;
    }

    private void OnDisable()
    {
        _googleSheetsDataReader.OnDataLoaded -= InitializeEnemyManager;
        _uiManager.OnIntroFinished -= OnIntroEnded;
        _ugsSetup.OnConsentAddressed -= HandleDataConsentAddressed;
        PlayerInputHandler.OnPlayerAttack -= HandlePlayerAttackButtonPressed;
        EnemyManager.OnWaveEnded -= HandleWaveEnded;
    }
    
    void Start()
    {
        Debug.Log("Game Started");
        Application.targetFrameRate = 60;
        _currentGameState = GameStates.Loading;

        // Init all systems
        _lamp.Initialize(); 
        _uiManager.Initialize();
        _playerInputHandler.Initialize();
        Debug.Log("Loaging Google Sheet Data");
        _googleSheetsDataReader.Initialize();
    }
    
    private void InitializeEnemyManager()
    {
        Debug.Log("GoogleSheet Data Loaded");
        SwitchGameState();

        // Initialize enemyManager only after we 100% sure that data was initialized
        _enemyManager.Initialize();
        if (PlayerPrefs.HasKey("dataConsent"))
        {
            _ugsSetup.Setup();
            SwitchGameState();
        }
    }
    
    // Player Agrees or Disagrees with Data Collection
    private void HandleDataConsentAddressed()
    {
        SwitchGameState();
    }
    private void OnIntroEnded()
    {
        SwitchGameState();
    }
    private void HandlePlayerAttackButtonPressed()
    {
        if (_currentGameState == GameStates.Prepare)
        {
            SwitchGameState();
        }
    }
    
    private void HandleWaveEnded(int waveNumber)
    {
        SwitchGameState();
    }

    private void SwitchGameState()
    {
        switch (_currentGameState)
        {
            case GameStates.Loading:
                _currentGameState = GameStates.ConsentScreen;
                break;
            case GameStates.ConsentScreen:
                _currentGameState = GameStates.Intro;
                _uiManager.PlayIntro();
                break;
            case GameStates.Intro:
                _playerInputHandler.AllowAttackInput();
                _uiManager.StartPrepare(_enemyManager.CurrentWave);
                _currentGameState = GameStates.Prepare;
                break;
            case GameStates.Prepare:
                _currentGameState = GameStates.Fight;
                _uiManager.StartFight();
                _enemyManager.StartWave();
                break;
            case GameStates.Fight:
                _uiManager.StartPrepare(_enemyManager.CurrentWave);
                _currentGameState = GameStates.Prepare;
                break;
        }
    }   

}
