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

    private void OnEnable()
    {
        _googleSheetsDataReader.OnDataLoaded += InitializeEnemyManager;
        _uiManager.OnIntroFinished += OnIntoEnded;
    }

    private void OnDisable()
    {
        _googleSheetsDataReader.OnDataLoaded -= InitializeEnemyManager;
        _uiManager.OnIntroFinished -= OnIntoEnded;
    }
    
    private void SwitchGameState()
    {
        Debug.Log("Switching Game State");
        switch (_currentGameState)
        {
            case GameStates.Loading:
                _currentGameState = GameStates.Intro;
                _uiManager.PlayIntro();
                break;
            case GameStates.Intro:
               _currentGameState = GameStates.MainGameLoop;
                _playerInputHandler.AllowAttackInput();
               break;
        }
        Debug.Log(_currentGameState);
    }

    void Start()
    {
        _currentGameState = GameStates.Loading;
        Application.targetFrameRate = 60;
        _lamp.Initialize(); 
        _enemyPool.Initialize();
        _uiManager.Initialize();
        _googleSheetsDataReader.Initialize();
    }
    
    private void InitializeEnemyManager()
    {
        _enemyManager.Initialize();
        SwitchGameState();
    }
    
    private void OnIntoEnded()
    {
        SwitchGameState();
    }

}
