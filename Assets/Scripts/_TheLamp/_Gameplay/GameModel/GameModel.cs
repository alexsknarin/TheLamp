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
    private readonly IGameStateProviderService _gameStateProviderService;
    private readonly IGameConfigService _gameConfigService;
    private readonly WaveEnemyDirector _waveEnemyDirector;
    // private EnemyController _enemyController;
    private readonly PlayerAttackCooldownHandler _playerAttackCooldownHandler;
    private readonly PlayerEnemyInteractionMediator _playerEnemyInteractionMediator;
    // private PlayerCollidersPropertyController _playerCollidersPropertyController;
    private readonly LampMovementController _lampMovementController;
    private readonly ScoresCollectionController _scoresCollectionController;
    private readonly LampDamageDataHandler _lampDamageDataHandler = new LampDamageDataHandler();
    private readonly UpgradeHandler _upgradeHandler = new UpgradeHandler();

    public GameModel(
        IGameStateProviderService gameStateProviderService, 
        IGameConfigService gameConfigService,
        WaveEnemyDirector waveEnemyDirector,
        // EnemyController enemyController, 
        PlayerAttackCooldownHandler playerAttackCooldownHandler,
        PlayerEnemyInteractionMediator playerEnemyInteractionMediator,
        LampMovementController lampMovementController,
        ScoresCollectionController scoresCollectionController)
    {
        Debug.Log(" +++ GameModel: Creating GameModel +++");
        _gameStateProviderService = gameStateProviderService;
        _currentGameState = gameStateProviderService.Get();
        _waveEnemyDirector = waveEnemyDirector;
        _gameConfigService = gameConfigService;
        _playerAttackCooldownHandler = playerAttackCooldownHandler;
        _playerEnemyInteractionMediator = playerEnemyInteractionMediator;
        // _playerEnemyInteractionHandler = playerEnemyInteractionHandler;
        _lampMovementController = lampMovementController;
        _scoresCollectionController = scoresCollectionController;
        
        // Subscriptions
        _waveEnemyDirector.WaveEnded += OnWaveEnded;
        _playerAttackCooldownHandler.PlayerAttackEnded += OnPlayerAttackCooldownEnded;
        _playerAttackCooldownHandler.PowerChanged += OnPowerChanged;
        
        _playerEnemyInteractionMediator.EnemyAttackEnded += OnEnemyAttackEnded;
        _playerEnemyInteractionMediator.EnemySticked += OnEnemySticked;
        _playerEnemyInteractionMediator.EnemyUnSticked += OnEnemyUnSticked;
        _waveEnemyDirector.LampBlocked += OnLampBlocked;
        _waveEnemyDirector.LampUnblocked += OnLampUnblocked;
        
        _scoresCollectionController.ScoreChanged += OnScoreChanged;
    }

    public void Dispose()
    {
        _waveEnemyDirector.WaveEnded -= OnWaveEnded;
        _playerAttackCooldownHandler.PlayerAttackEnded -= OnPlayerAttackCooldownEnded;
        _playerAttackCooldownHandler.PowerChanged -= OnPowerChanged;
        
        _playerEnemyInteractionMediator.EnemyAttackEnded -= OnEnemyAttackEnded;
        _playerEnemyInteractionMediator.EnemySticked -= OnEnemySticked;
        _playerEnemyInteractionMediator.EnemyUnSticked -= OnEnemyUnSticked;
        _waveEnemyDirector.LampBlocked -= OnLampBlocked;
        _waveEnemyDirector.LampUnblocked -= OnLampUnblocked;

        _scoresCollectionController.ScoreChanged -= OnScoreChanged;
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
    public event Action<bool> IsLampBlockedChanged;
    public event Action<int> UpgradePointsChanged;
    public event Action<float> LampAttackDistanceChanged;
    public event Action<float> LampCooldownTimeChanged;
    public event Action<float> LampAttackStarted;
    public event Action<float, string> LampDamageStarted;
    public event Action LampDestroyed;
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
            IsLampBlockedChanged?.Invoke(value);
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
        _playerAttackCooldownHandler.SetAttackDuration(_gameConfigService.PlayerConfig.AttackDuration);
        _playerAttackCooldownHandler.SetCooldownDuration(_currentGameState.LampCooldownTime); 
        _playerEnemyInteractionMediator.SetAttackZoneRadius(_currentGameState.LampAttackDistance);
        LampAttackDistanceChanged?.Invoke(_currentGameState.LampAttackDistance);
        _currentPower = 1.0f;
        LampGlassDamage = _currentGameState.GlassDamageData;
        _lampDamageDataHandler.MaxHealth = _currentGameState.LampMaxHealth;
        _scoresCollectionController.StartCollecting();
        GameStarted?.Invoke();
    }
    
    private void StartPrepareIn()
    {
        CurrentGameStageState = GameStageState.PrepareIn;
    }

    private void StartPrepare()
    {
        CurrentGameStageState = GameStageState.Prepare;
        Debug.Log("Prepare Started for wave " + _currentGameState.Wave);
        _waveEnemyDirector.PrepareWave(_currentGameState.Wave);
    }

    private void StartPrepareOut()
    {
        CurrentGameStageState = GameStageState.PrepareOut;
    }

    private void StartGameOver()
    {
        _waveEnemyDirector.HandleLampDestroyed();
        _scoresCollectionController.StopCollecting();
        CurrentGameStageState = GameStageState.GameOverIn;
        _playerAttackCooldownHandler.StopCooldown();
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
                _playerAttackCooldownHandler.PlayAttack();
                LampAttackStarted?.Invoke(CurrentPower);
            }
        }
        if (_currentGameStageState == GameStageState.Wave)
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _playerAttackCooldownHandler.PlayAttack();
                LampAttackStarted?.Invoke(CurrentPower);
                _waveEnemyDirector.HandleAttackButtonClicked(CurrentPower);
            }
        }
    }

    public void HandleDamageStateEnded()
    {
        _playerAttackCooldownHandler.PlayCooldown();
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
        
        _playerAttackCooldownHandler.SetCooldownDuration(LampCooldownTime);
        _playerAttackCooldownHandler.PlayCooldown();
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
        _scoresCollectionController.StopCollecting();
        _playerEnemyInteractionMediator.Reset();
        _waveEnemyDirector.Reset();
        _lampMovementController.Reset();
        StartGame();
    }

    private void StartWave()
    {
        CurrentGameStageState = GameStageState.Wave;
        Debug.Log("Starting Wave: " +_currentGameState.Wave);
        _waveEnemyDirector.StartWave();
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

    private void OnPlayerAttackCooldownEnded()
    {
        _isAttacking = false;
    }

    private void OnPowerChanged(float power)
    {
        CurrentPower = power;
    }
    
    private void OnEnemySticked(IStickableWithLamp stickable)
    {
        _lampMovementController.AddForce(-stickable.ProvideImpactPoint().normalized.x * 2);
    }
    
    private void OnEnemyUnSticked(IStickableWithLamp stickable)
    {
        _lampMovementController.AddForce(-stickable.ProvideImpactPoint().normalized.x * 2);
    }
    
    private void OnLampBlocked()
    {
        IsLampBlocked = true;
    }
    
    private void OnLampUnblocked()
    {
        IsLampBlocked = false;
    }

    private void OnEnemyAttackEnded(Vector3 impactPoint, bool isEnemyDamaged, string enemyTypeName)
    {
        if (!isEnemyDamaged)
        {
            if(_gameConfigService.PlayerConfig.IsDamageable)
                LampHealth -= 1;
            
            if (LampHealth <= 0)
            {
                LastEnemyPosition = impactPoint;
                _lampMovementController.AddForce(-LastEnemyPosition.normalized.x * 2);
                LampDestroyed?.Invoke();
                StartGameOver();
                Debug.Log("++++++++++ Game Over ++++++++++");
                return;
            }
            
            LampGlassDamage = _lampDamageDataHandler.UpdateGlassDamageDataDamage(LampGlassDamage, impactPoint.normalized);
            
            _lampMovementController.AddForce(-impactPoint.normalized.x * 2); 
            LampDamageStarted?.Invoke(_gameConfigService.PlayerConfig.DamageDuration, enemyTypeName);
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
