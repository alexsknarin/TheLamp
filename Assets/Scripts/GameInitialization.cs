using System;
using UnityEngine;

public class GameInitialization : MonoBehaviour
{
    [SerializeField] private Lamp _lamp;
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    [SerializeField] private GameStates _currentGameState;
    [SerializeField] private UGSSetup _ugsSetup;

    private void OnEnable()
    {
        _googleSheetsDataReader.OnDataLoaded += InitializeEnemyManager;
        _uiManager.OnIntroFinished += OnIntroEnded;
        _ugsSetup.OnConsentAddressed += HandleConsentAddressed;
    }

    private void OnDisable()
    {
        _googleSheetsDataReader.OnDataLoaded -= InitializeEnemyManager;
        _uiManager.OnIntroFinished -= OnIntroEnded;
        _ugsSetup.OnConsentAddressed -= HandleConsentAddressed;
    }
    
    private void SwitchGameState()
    {
        switch (_currentGameState)
        {
            case GameStates.ConsentScreen:
                _currentGameState = GameStates.Loading;
                break;
            case GameStates.Loading:
                _currentGameState = GameStates.Intro;
                _uiManager.PlayIntro();
                break;
            case GameStates.Intro:
               _currentGameState = GameStates.MainGameLoop;
                _playerInputHandler.AllowAttackInput();
               break;
        }
    }   

    void Start()
    {
        _currentGameState = GameStates.ConsentScreen;
        Application.targetFrameRate = 60;
        _lamp.Initialize(); 
        _enemyPool.Initialize();
        _googleSheetsDataReader.Initialize();
        _uiManager.Initialize();
        
        if (PlayerPrefs.HasKey("dataConsent"))
        {
            HandleConsentAddressed();
        }
    }
    
    private void HandleConsentAddressed()
    {
        SwitchGameState();
        _ugsSetup.Setup();
        SwitchGameState();
    }

    
    private void InitializeEnemyManager()
    {
        _enemyManager.Initialize();
    }
    
    private void OnIntroEnded()
    {
        SwitchGameState();
    }

}
