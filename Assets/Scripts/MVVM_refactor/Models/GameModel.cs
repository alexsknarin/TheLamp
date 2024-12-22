using System;
using UnityEngine;

public class GameModel : IDisposable
{
    private GameState _currentGameState;
    public GameState CurrentGameState => _currentGameState; // Debug Only
    public int Wave => _currentGameState.Wave;
    
    #region CurrentGameStageState Reactive Property
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
    #endregion

    #region CurrentPower Reactive Property
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
    #endregion

    #region LampHealth Reactive Property 
    public int LampHealth
    {
        get => _currentGameState.LampHealth;
        private set
        {
            _currentGameState.LampHealth = value;
            OnLampHealthChangedEvent?.Invoke(value);
        }
    }
    public event Action<int> OnLampHealthChangedEvent;
    #endregion
    
    #region LampMaxHealth Reactive Property 
    public int LampMaxHealth
    {
        get => _currentGameState.LampMaxHealth;
        private set
        {
            _currentGameState.LampMaxHealth = value;
            OnLampMaxHealthChangedEvent?.Invoke(value);
        }
    }
    public event Action<int> OnLampMaxHealthChangedEvent;
    #endregion
    
    #region LampBlocked Reactive Property
    private bool _isLampBlocked;
    public bool IsLampBlocked
    {
        get => _isLampBlocked;
        private set
        {
            _isLampBlocked = value;
            OnLampBlockedModeSetEvent?.Invoke(value);
        }
    }
    public event Action<bool> OnLampBlockedModeSetEvent;
    #endregion
    
    
    public event Action<float> OnLampAttackStartedEvent;
    
    private bool _isAttacking = false;
    
    
    // Dependencies
    private IGameConfigService _gameConfigService;
    private EnemyController _enemyController;
    private PlayerAttackController _playerAttackController;
    PlayerCollidersPropertyController _playerCollidersPropertyController;
    PlayerEnemyInteractionHandler _playerEnemyInteractionHandler;
   
    public GameModel(
        GameState _gameState, 
        EnemyController enemyController, 
        IGameConfigService gameConfigService,
        PlayerAttackController playerAttackController,
        PlayerCollidersPropertyController playerCollidersPropertyController,
        PlayerEnemyInteractionHandler playerEnemyInteractionHandler)
    {
        _currentGameState = _gameState;
        _enemyController = enemyController;
        _gameConfigService = gameConfigService;
        _playerAttackController = playerAttackController;
        _playerCollidersPropertyController = playerCollidersPropertyController;
        _playerEnemyInteractionHandler = playerEnemyInteractionHandler;
        
        // Subscriptions
        _enemyController.OnWaveEndEvent += HandleWaveEnd;
        _playerAttackController.OnAttackEndedEvent += HandleLampAttackEnded;
        _playerAttackController.OnPowerChangedEvent += HandlePowerChanged;
        _playerEnemyInteractionHandler.OnLampBlockedSetEvent += SetLampBlockedState;
        
        Debug.Log("GameModel created");
        Debug.Log("GameState: " + _gameState.LampCooldownTime);
    }

    public void Dispose()
    {
        _enemyController.OnWaveEndEvent -= HandleWaveEnd;
        _playerAttackController.OnAttackEndedEvent -= HandleLampAttackEnded;
        _playerAttackController.OnPowerChangedEvent -= HandlePowerChanged;
    }

    public void StartGame()
    {
        Debug.Log("!!!!! The Game Has Been Started !!!!!");
        // Start the game
        if (_gameConfigService.GameConfig.IsTestStartWave)
        {
            _currentGameState.Wave = _gameConfigService.GameConfig.TestStartWave;
        }
        CurrentGameStageState = GameStageState.Intro;
        _playerAttackController.SetAttackDuration(_gameConfigService.PlayerConfig.AttackDuration); // Attack Duration
        _playerAttackController.SetCooldownDuration(_currentGameState.LampCooldownTime); // Cooldown Duration
        _playerCollidersPropertyController.SetAttackZoneRadius(_currentGameState.LampAttackDistance); // Attack Distance
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
        Debug.Log("Starting Wave: " +_currentGameState.Wave);
        _enemyController.StartWave(_currentGameState.Wave);
    }

    private void HandleCurrentStageStateFinished()
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
                CurrentGameStageState = GameStageState.PrepareIn;
                StartPrepareIn();
                break;
            case GameStageState.GameOverOut:
                Debug.Log("<<<<<<<   Game Finished.  >>>>>>>");
                break;
        }
    }

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
        Debug.Log($"Wave {_currentGameState.Wave} Ended");
        _currentGameState.Wave++;
        HandleCurrentStageStateFinished();
    }

    public void HandleAttackButtonClicked()
    {
        if (_currentGameStageState == GameStageState.Prepare)
        {
            HandlePrepareEnd();
            if (!_isAttacking)
            {
                _isAttacking = true;
                _playerAttackController.PlayAttack();
                OnLampAttackStartedEvent?.Invoke(CurrentPower); // Attack Power
            }
        }
        if (_currentGameStageState == GameStageState.Wave)
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _playerAttackController.PlayAttack();
                OnLampAttackStartedEvent?.Invoke(CurrentPower); // Attack Power
                _playerEnemyInteractionHandler.LampAttack();
                _enemyController.HandleAttackButtonClicked(CurrentPower);
            }
        }
    }

    private void HandleLampAttackEnded()
    {
        _isAttacking = false;
    }

    private void HandlePowerChanged(float power)
    {
        CurrentPower = power;
    }

    private void SetLampBlockedState(bool isBlocked)
    {
        IsLampBlocked = isBlocked;
        _enemyController.SetBlockedMode(isBlocked);
        if (isBlocked)
        {
            Debug.Log("Lamp is blocked");
        }
        else
        {
            Debug.Log("Lamp is not blocked");
        }
    }
}
