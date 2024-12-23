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
    
    #region LampGlassDamage Reactive Property 
    public GlassDamageData LampGlassDamage
    
    {
        get => _currentGameState.GlassDamageData;
        private set
        {
            _currentGameState.GlassDamageData = value;
            OnLampGlassDamageChangedEvent?.Invoke(value);
        }
    }
    public event Action<GlassDamageData> OnLampGlassDamageChangedEvent;
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
    public event Action<float> OnLampDamageStartedEvent;
    public event Action OnLampDeathEvent;
    
    private bool _isAttacking = false;
    
    
    // Dependencies
    private IGameConfigService _gameConfigService;
    private EnemyController _enemyController;
    private PlayerAttackHandler _playerAttackHandler;
    private PlayerCollidersPropertyController _playerCollidersPropertyController;
    private PlayerEnemyInteractionHandler _playerEnemyInteractionHandler;
    private LampDamageDataHandler _lampDamageDataHandler = new LampDamageDataHandler();
    private LampMovementController _lampMovementController;
   
    public GameModel(
        GameState _gameState, 
        EnemyController enemyController, 
        IGameConfigService gameConfigService,
        PlayerAttackHandler playerAttackHandler,
        PlayerCollidersPropertyController playerCollidersPropertyController,
        PlayerEnemyInteractionHandler playerEnemyInteractionHandler,
        LampMovementController lampMovementController)
    {
        _currentGameState = _gameState;
        _enemyController = enemyController;
        _gameConfigService = gameConfigService;
        _playerAttackHandler = playerAttackHandler;
        _playerCollidersPropertyController = playerCollidersPropertyController;
        _playerEnemyInteractionHandler = playerEnemyInteractionHandler;
        _lampMovementController = lampMovementController;
        
        // Subscriptions
        _enemyController.OnWaveEndEvent += HandleWaveEnd;
        _playerAttackHandler.OnAttackEndedEvent += HandleLampAttackEnded;
        _playerAttackHandler.OnPowerChangedEvent += HandlePowerChanged;
        _playerEnemyInteractionHandler.OnLampBlockedSetEvent += SetLampBlockedState;
        _playerEnemyInteractionHandler.OnEnemyAttackDeflectedEvent += HandleEnemyAttackDeflected;
        
        Debug.Log("GameModel created");
        Debug.Log("GameState: " + _gameState.LampCooldownTime);
    }

    public void Dispose()
    {
        _enemyController.OnWaveEndEvent -= HandleWaveEnd;
        _playerAttackHandler.OnAttackEndedEvent -= HandleLampAttackEnded;
        _playerAttackHandler.OnPowerChangedEvent -= HandlePowerChanged;
        _playerEnemyInteractionHandler.OnLampBlockedSetEvent -= SetLampBlockedState;
        _playerEnemyInteractionHandler.OnEnemyAttackDeflectedEvent -= HandleEnemyAttackDeflected;
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
        _playerAttackHandler.SetAttackDuration(_gameConfigService.PlayerConfig.AttackDuration); // Attack Duration
        _playerAttackHandler.SetCooldownDuration(_currentGameState.LampCooldownTime); // Cooldown Duration
        _playerCollidersPropertyController.SetAttackZoneRadius(_currentGameState.LampAttackDistance); // Attack Distance
        _currentPower = 1.0f;
        LampGlassDamage = _currentGameState.GlassDamageData;
        _lampDamageDataHandler.MaxHealth = _currentGameState.LampMaxHealth;
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
                _playerAttackHandler.PlayAttack();
                OnLampAttackStartedEvent?.Invoke(CurrentPower); // Attack Power
            }
        }
        if (_currentGameStageState == GameStageState.Wave)
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _playerAttackHandler.PlayAttack();
                OnLampAttackStartedEvent?.Invoke(CurrentPower); // Attack Power
                _playerEnemyInteractionHandler.LampAttack();
                _enemyController.HandleAttackButtonClicked(CurrentPower);
            }
        }
    }
    
    public void HandleDamageStateEnded()
    {
        _playerAttackHandler.PlayCooldown();
    }

    private void HandleLampAttackEnded()
    {
        _isAttacking = false;
    }

    private void HandlePowerChanged(float power)
    {
        CurrentPower = power;
    }

    private void SetLampBlockedState(bool isBlocked, EnemyBase enemy)
    {
        IsLampBlocked = isBlocked;
        _enemyController.SetBlockedMode(isBlocked);
        // TODO: take lamp position into consideration
        _lampMovementController.AddForce(-enemy.ProvideImpactPoint().normalized.x * 2);
    }

    private void HandleEnemyAttackDeflected(bool isDeflected, EnemyBase enemy)
    {
        if (!isDeflected || IsLampBlocked)
        {
            LampHealth -= 1;

            if (LampHealth <= 0)
            {
                OnLampDeathEvent?.Invoke();
                Debug.Log("++++++++++ Game Over ++++++++++");
                return;
                // Play Game Over In state
            }
            
            LampGlassDamage = _lampDamageDataHandler.UpdateGlassDamageDataDamage(LampGlassDamage, enemy.ProvideImpactPoint().normalized);
            // TODO: take lamp position into consideration
            // Or calculate it in the LampMovementController because it knows about lamp position
            _lampMovementController.AddForce(-enemy.ProvideImpactPoint().normalized.x * 2); 
            OnLampDamageStartedEvent?.Invoke(_gameConfigService.PlayerConfig.DamageDuration);
        }
    }
}
