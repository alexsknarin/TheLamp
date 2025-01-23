using System;
using UnityEngine;

public class GameModel : IDisposable, ILampDeadEventProviderService
{
    private GameState _currentGameState;
    private GameStageState _currentGameStageState = GameStageState.Loading;
    private float _currentPower;
    private bool _isLampBlocked;

    private bool _isAttacking = false;

    // Dependencies
    private IGameStateProviderService _gameStateProviderService;
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
        IGameStateProviderService gameStateProviderService, 
        EnemyController enemyController, 
        IGameConfigService gameConfigService,
        PlayerAttackHandler playerAttackHandler,
        PlayerCollidersPropertyController playerCollidersPropertyController,
        PlayerEnemyInteractionHandler playerEnemyInteractionHandler,
        LampMovementController lampMovementController,
        ScoresCollectionHandler scoresCollectionHandler)
    {
        Debug.Log(" +++ GameModel: Creating GameModel +++");
        _gameStateProviderService = gameStateProviderService;
        _currentGameState = gameStateProviderService.Get();
        _enemyController = enemyController;
        _gameConfigService = gameConfigService;
        _playerAttackHandler = playerAttackHandler;
        _playerCollidersPropertyController = playerCollidersPropertyController;
        _playerEnemyInteractionHandler = playerEnemyInteractionHandler;
        _lampMovementController = lampMovementController;
        _scoresCollectionHandler = scoresCollectionHandler;
        
        // Subscriptions
        _enemyController.WaveEnded += OnWaveEnded;
        _playerAttackHandler.PlayerAttackEnded += OnPlayerAttackEnded;
        _playerAttackHandler.PowerChanged += OnPowerChanged;
        _playerEnemyInteractionHandler.LampBlockedStarted += OnLampBlockedStarted;
        _playerEnemyInteractionHandler.LampBlockedEnded += OnLampBlockedEnded;
        _playerEnemyInteractionHandler.EnemyAttackBounced += OnEnemyAttackBounced;
        _scoresCollectionHandler.ScoreChanged += OnScoreChanged;
    }
    
    public void Dispose()
    {
        _enemyController.WaveEnded -= OnWaveEnded;
        _playerAttackHandler.PlayerAttackEnded -= OnPlayerAttackEnded;
        _playerAttackHandler.PowerChanged -= OnPowerChanged;
        _playerEnemyInteractionHandler.LampBlockedStarted -= OnLampBlockedStarted;
        _playerEnemyInteractionHandler.LampBlockedEnded -= OnLampBlockedEnded;
        _playerEnemyInteractionHandler.EnemyAttackBounced -= OnEnemyAttackBounced;
        _scoresCollectionHandler.ScoreChanged -= OnScoreChanged;
    }
    
    // Events
    public event Action GameStarted;
    public event Action<GameState> GameStateChanged;
    public event Action<GameStageState> GameStageStateChanged;
    public event Action<int> LampLevelChanged;
    public event Action<float> PowerChanged;
    public event Action UpgradeClicked;
    public event Action<int> LampHealthChanged;
    public event Action<int> LampMaxHealthChanged;
    public event Action<GlassDamageData> LampGlassDamageChanged;
    public event Action<bool> LampBlockedModeSet;
    public event Action<int> UpgradePointsChanged;
    public event Action<float> LampAttackDistanceChanged;
    public event Action<float> LampCooldownTimeChanged;
    public event Action<float> LampAttackStarted;
    public event Action<float, EnemyBase> LampDamageStarted;
    public event Action<Vector3> LampDeathHappened; // TODO: make single event for all lamp death events
    public event Action<EnemyBase> LampDied;
    public event Action<int> WaveStarted;
    public event Action<int> WaveEnded;
    public event Action HealthUpgraded;
    public event Action CoolDownUpgraded;
    public event Action AttackDistanceUpgraded;
    
    public int Wave => _currentGameState.Wave;
    public Vector3 LastEnemyPosition { get; private set; }
    
    public GameState CurrentGameState 
    {
        get => _currentGameState;
        private set
        {
            _currentGameState = value;
            GameStateChanged?.Invoke(value);
        }
    }
    public GameStageState CurrentGameStageState
    {
        get => _currentGameStageState;
        private set
        {
            var oldValue = _currentGameStageState;
            _currentGameStageState = value;
            if (!oldValue.Equals(value))
            {
                GameStageStateChanged?.Invoke(value);
            }
        }
    }
    public int LampLevel
    {
        get => _currentGameState.LampLevel;
        private set
        {
            _currentGameState.LampLevel = value;
            LampLevelChanged?.Invoke(value);
        }
    }
    public float CurrentPower
    {
        get => _currentPower;
        private set
        {
            _currentPower = value;
            PowerChanged?.Invoke(value);
        }
    }
    public int LampHealth
    {
        get => _currentGameState.LampHealth;
        private set
        {
            _currentGameState.LampHealth = value;
            LampHealthChanged?.Invoke(value);
        }
    }
    public int LampMaxHealth
    {
        get => _currentGameState.LampMaxHealth;
        private set
        {
            _currentGameState.LampMaxHealth = value;
            LampMaxHealthChanged?.Invoke(value);
        }
    }
    public GlassDamageData LampGlassDamage
    {
        get => _currentGameState.GlassDamageData;
        private set
        {
            _currentGameState.GlassDamageData = value;
            LampGlassDamageChanged?.Invoke(value);
        }
    }
    public bool IsLampBlocked
    {
        get => _isLampBlocked;
        private set
        {
            _isLampBlocked = value;
            LampBlockedModeSet?.Invoke(value);
        }
    }
    public int UpgradePoints
    {
        get => _currentGameState.LampUpgradePoints;
        private set
        {
            _currentGameState.LampUpgradePoints = value;
            UpgradePointsChanged?.Invoke(value);
        }
    }
    public float LampAttackDistance
    {
        get => _currentGameState.LampAttackDistance;
        private set
        {
            _currentGameState.LampAttackDistance = value;
            LampAttackDistanceChanged?.Invoke(value);
        }
    }
    public float LampCooldownTime
    {
        get => _currentGameState.LampCooldownTime;
        private set
        {
            _currentGameState.LampCooldownTime = value;
            LampCooldownTimeChanged?.Invoke(value);
        }
    }
    
    // Game State change Active methods
    public void StartGame()
    {
        Debug.Log("!!!!! The Game Is About to get Started !!!!!");
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
        _enemyController.StartGame();
        GameStarted?.Invoke();
    }
    
    private void StartPrepareIn()
    {
        CurrentGameStageState = GameStageState.PrepareIn;
    }

    private void StartPrepare()
    {
        CurrentGameStageState = GameStageState.Prepare;
    }

    private void StartPrepareOut()
    {
        CurrentGameStageState = GameStageState.PrepareOut;
    }

    private void StartGameOver()
    {
        CurrentGameStageState = GameStageState.GameOverIn;
        _playerAttackHandler.StopCooldown();
        _enemyController.HandleGameOver();
    }

    // Game State change Passive methods
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
    }

    public void HandleGameOverOutEnd()
    {
        Debug.Log("Game Over Out Ended");
        RestartGame();
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
                LampAttackStarted?.Invoke(CurrentPower); // Attack Power
            }
        }
        if (_currentGameStageState == GameStageState.Wave)
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _playerAttackHandler.PlayAttack();
                LampAttackStarted?.Invoke(CurrentPower); // Attack Power
                _playerEnemyInteractionHandler.LampAttack();
                _enemyController.HandleAttackButtonClicked(CurrentPower);
            }
        }
    }

    public void HandleDamageStateEnded()
    {
        _playerAttackHandler.PlayCooldown();
    }

    public void HandleHealthUpgrade()
    {
        UpgradeClicked?.Invoke();
        if (UpgradePoints <= 0)
        {
            Debug.LogWarning("GameModel: Health: Not enough upgrade points!");
            return;
        }
        
        UpgradePoints--;
        LampLevel++;
        LampHealth++;
        _gameStateProviderService.SaveCurrentState();
        
        if (LampHealth > _gameConfigService.PlayerConfig.HealthCap)
        {
            Debug.LogWarning("GameModel: Health: Lamp health got over the heal cap!");
        }
        
        if (LampHealth > LampMaxHealth)
        {
            LampMaxHealth = LampHealth;
            HealthUpgraded?.Invoke();
        }
    }

    public void HandleCooldownUpgrade()
    {
        UpgradeClicked?.Invoke();
        if (UpgradePoints <= 0)
        {
            Debug.LogWarning("Not enough upgrade points");
            return;
        }
        
        UpgradePoints--;
        LampLevel++;
        LampCooldownTime -= _gameConfigService.PlayerConfig.CooldownDecrement;
        _gameStateProviderService.SaveCurrentState();
        
        // Lamp Cooldown is decreasing with upgrade, therefore Cap is set to smaller number then current
        if (LampCooldownTime < _gameConfigService.PlayerConfig.CooldownTimeCap)
        {
            LampCooldownTime = _gameConfigService.PlayerConfig.CooldownTimeCap;
        }
        
        _playerAttackHandler.SetCooldownDuration(LampCooldownTime); // TODO: maybe combine it with PlayCooldown
        _playerAttackHandler.PlayCooldown();
        CoolDownUpgraded?.Invoke();
    }

    public void HandleAttackDistanceUpgrade()
    {
        UpgradeClicked?.Invoke();
        if (UpgradePoints <= 0)
        {
            Debug.LogWarning("Not enough upgrade points");
            return;
        }
        
        UpgradePoints--;
        LampLevel++;
        LampAttackDistance += _gameConfigService.PlayerConfig.AttackDistanceIncrement;
        _gameStateProviderService.SaveCurrentState();
        
        if (LampAttackDistance > _gameConfigService.PlayerConfig.AttackDistanceCap)
        {
            LampAttackDistance = _gameConfigService.PlayerConfig.AttackDistanceCap;
            return;
        }
        
        _playerCollidersPropertyController.SetAttackZoneRadius(LampAttackDistance);
        AttackDistanceUpgraded?.Invoke();
    }

    // Game End Handle Methods

    public void HandleRestartGameWitAdFromGameOver()
    {
        CurrentGameStageState = GameStageState.Advertisement;
    }

    public void HandleAdvertisementEnd()
    {
        Debug.Log("Advertisement Ended");
        _gameStateProviderService.SaveUpgradesOnly();
        RestartGame();
    }

    public void HandleRestartGameNoAdFromGameOver()
    {
        _gameStateProviderService.SaveDefaultState();
        CurrentGameStageState = GameStageState.GameOverOut;
    }

    public void HandleImmediateRestartGame()
    {
        Debug.Log(" +++++++++ Immediate Restart Game");
        _gameStateProviderService.SaveDefaultState();
        RestartGame();
    }

    public void ExitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
        
#if UNITY_ANDROID
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void RestartGame()
    {
        Debug.Log(" --- --- Restarting Game");
        CurrentGameState = _gameStateProviderService.Get();
        Debug.Log("New GameState Generated");
        Debug.Log($"Wave: {CurrentGameState.Wave}");
        _playerEnemyInteractionHandler.Reset();
        _enemyController.Restart();
        _lampMovementController.Reset();
        StartGame();
    }

    private void StartWave()
    {
        CurrentGameStageState = GameStageState.Wave;
        Debug.Log("Starting Wave: " +_currentGameState.Wave);
        _enemyController.StartWave(_currentGameState.Wave);
        WaveStarted?.Invoke(_currentGameState.Wave);
    }

    // Event Handle Methods
    private void OnWaveEnded()
    {
        Debug.Log($"Wave {_currentGameState.Wave} Ended");
        _currentGameState.Wave++;
        _gameStateProviderService.SaveCurrentState();
        StartPrepareIn();
        WaveEnded?.Invoke(_currentGameState.Wave);
    }

    private void OnPlayerAttackEnded()
    {
        _isAttacking = false;
    }

    private void OnPowerChanged(float power)
    {
        CurrentPower = power;
    }

    private void OnLampBlockedStarted(Vector3 impactPoint)
    {
        IsLampBlocked = true;
        _enemyController.SetBlockedMode(IsLampBlocked);
        _lampMovementController.AddForce(-impactPoint.normalized.x * 2);
    }
    
    private void OnLampBlockedEnded(Vector3 impactPoint)
    {
        IsLampBlocked = false;
        _enemyController.SetBlockedMode(IsLampBlocked);
        _lampMovementController.AddForce(-impactPoint.normalized.x * 2);
    }
    
    private void OnEnemyAttackBounced(bool isDeflected, EnemyBase enemy)
    {
        if (!isDeflected || IsLampBlocked)
        {
            if(_gameConfigService.PlayerConfig.IsDamageable)
                LampHealth -= 1;

            if (LampHealth <= 0)
            {
                LastEnemyPosition = enemy.ProvideImpactPoint();
                _lampMovementController.AddForce(-LastEnemyPosition.normalized.x * 2);
                _enemyController.HandleLampDestroyed();
                LampDeathHappened?.Invoke(enemy.ProvideImpactPoint());
                LampDied?.Invoke(enemy);
                StartGameOver();
                Debug.Log("++++++++++ Game Over ++++++++++");
                return;
            }
            
            LampGlassDamage = _lampDamageDataHandler.UpdateGlassDamageDataDamage(LampGlassDamage, enemy.ProvideImpactPoint().normalized);
            
            // TODO: take lamp position into consideration
            // Or calculate it in the LampMovementController because it knows about lamp position
            
            _lampMovementController.AddForce(-enemy.ProvideImpactPoint().normalized.x * 2); 
            LampDamageStarted?.Invoke(_gameConfigService.PlayerConfig.DamageDuration, enemy);
        }
    }

    private void OnScoreChanged(int newScore)
    {
        _currentGameState.UpgradeData.Score += newScore;
        int newUpgradePoints = _upgradeHandler.GetUpgradePointsAndUpdateScoreData(ref _currentGameState.UpgradeData);
        if (newUpgradePoints > 0)
        {
            _currentGameState.LampUpgradePoints += newUpgradePoints;
            // TODO: maybe add event to indicate it somehow
        }
    }
}
