using System;
using UnityEngine;

public class GameModel : IDisposable
{
    private GameState _currentGameState;
    public GameState CurrentGameState
    {
        get => _currentGameState;
        private set
        {
            _currentGameState = value;
            OnGameStateChangedEvent?.Invoke(value);
        }
    }
    public event Action<GameState> OnGameStateChangedEvent; // TODO: do we need this event? Expose separate properties instead of the whole GameState
    
    
    private GameStageState _currentGameStageState = GameStageState.Loading;
    public GameStageState CurrentGameStageState
    {
        get => _currentGameStageState;
        private set
        {
            var oldValue = _currentGameStageState;
            _currentGameStageState = value;
            if (!oldValue.Equals(value))
            {
                OnGameStageStateChangedEvent?.Invoke(value);
            }
        }
    }
    public event Action<GameStageState> OnGameStageStateChangedEvent;
    
    
    private float _currentPower;
    public float CurrentPower
    {
        get => _currentPower;
        private set
        {
            _currentPower = value;
            OnPowerChangedEvent?.Invoke(value);
        }
    }
    public event Action<float> OnPowerChangedEvent;
    
    
    
    
    
    public event Action<float> OnLampAttackStartedEvent;
    
    
    
    

    
    // Dependencies
    private IGameConfigService _gameConfigService;
    private EnemyController _enemyController;
    private PlayerAttackController _playerAttackController;
    private PlayerCooldownController _playerCooldownController;
    
    
    public GameModel(
        GameState _gameState, 
        EnemyController enemyController, 
        IGameConfigService gameConfigService,
        PlayerAttackController playerAttackController,
        PlayerCooldownController playerCooldownController)
    {
        _currentGameState = _gameState;
        _enemyController = enemyController;
        _gameConfigService = gameConfigService;
        _playerAttackController = playerAttackController;
        _playerCooldownController = playerCooldownController;
        _enemyController.OnWaveEndEvent += HandleWaveEnd;
        _playerAttackController.OnAttackEndedEvent += HandleLampAttackEnded;
        _playerCooldownController.OnPowerChangedEvent += HandlePowerChanged;
        Debug.Log("GameModel created");
        Debug.Log("GameState: " + _gameState.LampCooldownTime);
    }

    public void Dispose()
    {
        _enemyController.OnWaveEndEvent -= HandleWaveEnd;
        _playerAttackController.OnAttackEndedEvent -= HandleLampAttackEnded;
    }

    public void StartGame()
    {
        Debug.Log("!!!!! The Game Has Been Started !!!!!");
        // Start the game
        CurrentGameStageState = GameStageState.Intro;
        _playerAttackController.SetDuration(_gameConfigService.PlayerConfig.AttackDuration); // Attack Duration
        _playerCooldownController.SetDuration(_currentGameState.LampCooldownTime); // Cooldown Duration
        _currentPower = 1.0f;
    }

    private void StartPrepareIn()
    {
        CurrentGameStageState = GameStageState.PrepareIn;
        Debug.Log("Starting PrepareIn");
    }

    private void StartPrepare()
    {
        CurrentGameStageState = GameStageState.Prepare;
        Debug.Log("Starting Prepare");
    }

    private void StartPrepareOut()
    {
        CurrentGameStageState = GameStageState.PrepareOut;
        Debug.Log("Starting PrepareOut");
    }

    private void StartWave()
    {
        CurrentGameStageState = GameStageState.Wave;
        _enemyController.StartWave();
    }

    public void HandleCurrentStageStateFinished() // TODO: viewModels Should not call this method directly
    {
        switch (_currentGameStageState)
        {
            case GameStageState.Intro:
                StartPrepareIn();
                break;
            case GameStageState.PrepareIn:
                StartPrepare();
                break;
            case GameStageState.Prepare:
                StartPrepareOut();
                break;
            case GameStageState.PrepareOut:
                StartWave();
                break;
            case GameStageState.Wave:
                CurrentGameStageState = GameStageState.GameOverOut;
                break;
            case GameStageState.GameOverOut:
                Debug.Log("<<<<<<<   Game Finished.  >>>>>>>");
                break;
        }
    }


    // TODO: rename public methods from Handle... to something else 

    public void HandleIntroEnd()
    {
        StartPrepareIn();    
    }

    public void HandlePrepareInEnd()
    {
        StartPrepare();
    }

    private void HandlePrepareEnd()
    {
        StartPrepareOut();
    }

    public void HandlePrepareOutEnd()
    {
        StartWave();
    }

    private void HandleWaveEnd()
    {
        HandleCurrentStageStateFinished();
    }

    public void HandleAttackButtonClicked()
    {
        if (_currentGameStageState == GameStageState.Prepare)
        {
            HandlePrepareEnd();
            OnLampAttackStartedEvent?.Invoke(1.0f); // Attack Power
            _playerAttackController.Play();
        }
        if (_currentGameStageState == GameStageState.Wave)
        {
            Debug.Log("Attack button clicked");
            OnLampAttackStartedEvent?.Invoke(CurrentPower); // Attack Power
            _playerAttackController.Play();
            // _enemyController.HandleAttackButtonClicked();
        }
        
    }

    private void HandleLampAttackEnded()
    {
        Debug.Log("Lamp Attack Ended");
        _playerCooldownController.StartCooldown();
    }

    private void HandlePowerChanged(float power)
    {
        CurrentPower = power;
    }
}
