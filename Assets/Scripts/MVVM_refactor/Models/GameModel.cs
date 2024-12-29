using System;
using UnityEngine;

public class GameModel : IDisposable
{
    private GameState _currentGameState;
    public GameState CurrentGameState => _currentGameState; // Debug Only
    public int Wave => _currentGameState.Wave;
    public Vector3 LastEnemyPosition { get; private set; }
    
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
    
    #region UpgradePoints Reactive Property
    public int UpgradePoints
    {
        get => _currentGameState.LampUpgradePoints;
        private set
        {
            _currentGameState.LampUpgradePoints = value;
            OnUpgradePointsChangedEvent?.Invoke(value);
        }
    }
    public event Action<int> OnUpgradePointsChangedEvent;
    #endregion
    
    #region LampAttackDistance Reactive Property
    public float LampAttackDistance
    {
        get => _currentGameState.LampAttackDistance;
        private set
        {
            _currentGameState.LampAttackDistance = value;
            OnLampAttackDistanceChangedEvent?.Invoke(value);
        }
    }
    public event Action<float> OnLampAttackDistanceChangedEvent;
    #endregion
    
    #region LampCooldownTime Reactive Property
    public float LampCooldownTime
    {
        get => _currentGameState.LampCooldownTime;
        private set
        {
            _currentGameState.LampCooldownTime = value;
            OnLampCooldownTimeChangedEvent?.Invoke(value);
        }
    }
    public event Action<float> OnLampCooldownTimeChangedEvent;
    #endregion
    
    
    public event Action<float> OnLampAttackStartedEvent;
    public event Action<float> OnLampDamageStartedEvent;
    public event Action<Vector3> OnLampDeathEvent;
    
    private bool _isAttacking = false;
    
    
    // Dependencies
    private IGameConfigService _gameConfigService;
    private EnemyController _enemyController;
    private PlayerAttackHandler _playerAttackHandler;
    private PlayerCollidersPropertyController _playerCollidersPropertyController;
    private PlayerEnemyInteractionHandler _playerEnemyInteractionHandler;
    private LampDamageDataHandler _lampDamageDataHandler = new LampDamageDataHandler();
    private LampMovementController _lampMovementController;
    private ScoresCollectionHandler _scoresCollectionHandler;
    private UpgradeHandler _upgradeHandler = new UpgradeHandler();
   
    public GameModel(
        GameState _gameState, 
        EnemyController enemyController, 
        IGameConfigService gameConfigService,
        PlayerAttackHandler playerAttackHandler,
        PlayerCollidersPropertyController playerCollidersPropertyController,
        PlayerEnemyInteractionHandler playerEnemyInteractionHandler,
        LampMovementController lampMovementController,
        ScoresCollectionHandler scoresCollectionHandler)
    {
        _currentGameState = _gameState;
        _enemyController = enemyController;
        _gameConfigService = gameConfigService;
        _playerAttackHandler = playerAttackHandler;
        _playerCollidersPropertyController = playerCollidersPropertyController;
        _playerEnemyInteractionHandler = playerEnemyInteractionHandler;
        _lampMovementController = lampMovementController;
        _scoresCollectionHandler = scoresCollectionHandler;
        
        // Subscriptions
        _enemyController.OnWaveEndEvent += HandleWaveEnd;
        _playerAttackHandler.OnAttackEndedEvent += HandleLampAttackEnded;
        _playerAttackHandler.OnPowerChangedEvent += HandlePowerChanged;
        _playerEnemyInteractionHandler.OnLampBlockedSetEvent += SetLampBlockedState;
        _playerEnemyInteractionHandler.OnEnemyAttackDeflectedEvent += HandleEnemyAttackDeflected;
        _scoresCollectionHandler.OnScoreChangeEvent += HandleScoreChange;
        
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
        _scoresCollectionHandler.OnScoreChangeEvent -= HandleScoreChange;
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
    
    private void StartGameOver()
    {
        CurrentGameStageState = GameStageState.GameOverIn;
        _playerAttackHandler.StopCooldown();
        _enemyController.HandleGameOver();
        Debug.Log("Starting Game Over");
    }
    
    // --- Event Handlers ---

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
    
    public void HandleGameOverInEnd()
    {
        CurrentGameStageState = GameStageState.GameOver;
        Debug.Log("Finally Game is Over");
    }

    private void HandleWaveEnd()
    {
        Debug.Log($"Wave {_currentGameState.Wave} Ended");
        _currentGameState.Wave++;
        StartPrepareIn();
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
            if(_gameConfigService.PlayerConfig.IsDamageable)
                LampHealth -= 1;

            if (LampHealth <= 0)
            {
                LastEnemyPosition = enemy.ProvideImpactPoint();
                OnLampDeathEvent?.Invoke(enemy.ProvideImpactPoint());
                StartGameOver();
                Debug.Log("++++++++++ Game Over ++++++++++");
                return;
            }
            
            LampGlassDamage = _lampDamageDataHandler.UpdateGlassDamageDataDamage(LampGlassDamage, enemy.ProvideImpactPoint().normalized);
            
            // TODO: take lamp position into consideration
            // Or calculate it in the LampMovementController because it knows about lamp position
            
            _lampMovementController.AddForce(-enemy.ProvideImpactPoint().normalized.x * 2); 
            OnLampDamageStartedEvent?.Invoke(_gameConfigService.PlayerConfig.DamageDuration);
        }
    }

    private void HandleScoreChange(int newScore)
    {
        _currentGameState.UpgradeData.Score += newScore;
        int newUpgradePoints = _upgradeHandler.GetUpgradePointsAndUpdateScoreData(ref _currentGameState.UpgradeData);
        if (newUpgradePoints > 0)
        {
            _currentGameState.LampUpgradePoints += newUpgradePoints;
            // TODO: maybe add event to indicate it somehow
        }
    }
    
    
    // --- Upgrades ---
    public void HandleHealthUpgrade()
    {
        if (UpgradePoints <= 0)
        {
            Debug.LogWarning("GameModel: Health: Not enough upgrade points!");
            return;
        }
        
        UpgradePoints--;
        LampHealth++;
        
        if (LampHealth > _gameConfigService.PlayerConfig.HealthCap)
        {
            Debug.LogWarning("GameModel: Health: Lamp health got over the heal cap!");
        }
        
        if (LampHealth > LampMaxHealth)
        {
            LampMaxHealth = LampHealth;
        }
    }

    public void HandleCooldownUpgrade()
    {
        if (UpgradePoints <= 0)
        {
            Debug.LogWarning("Not enough upgrade points");
            return;
        }
        
        UpgradePoints--;
        LampCooldownTime -= _gameConfigService.PlayerConfig.CooldownDecrement;
        
        // Lamp Cooldown is decreasing with upgrade, therefore Cap is set to smaller number then current
        if (LampCooldownTime < _gameConfigService.PlayerConfig.CooldownTimeCap)
        {
            LampCooldownTime = _gameConfigService.PlayerConfig.CooldownTimeCap;
        }
        
        _playerAttackHandler.SetCooldownDuration(LampCooldownTime); // TODO: maybe combine it with PlayCooldown
        _playerAttackHandler.PlayCooldown();
    }

    public void HandleAttackDistanceUpgrade()
    {
        if (UpgradePoints <= 0)
        {
            Debug.LogWarning("Not enough upgrade points");
            return;
        }
        
        UpgradePoints--;
        LampAttackDistance += _gameConfigService.PlayerConfig.AttackDistanceIncrement;
        
        if (LampAttackDistance > _gameConfigService.PlayerConfig.AttackDistanceCap)
        {
            LampAttackDistance = _gameConfigService.PlayerConfig.AttackDistanceCap;
            return;
        }
        
        _playerCollidersPropertyController.SetAttackZoneRadius(LampAttackDistance);
    }
}
