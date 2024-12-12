using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    [SerializeField] private Lamp _lamp;
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private LampStatsManager _lampStatsManager;
    [SerializeField] private PlayerInputHandler _playerInputHandler;
    [SerializeField] private ScoresManager _scoresManager;
    [SerializeField] private SaveLoadManager _saveLoadManager;
    [FormerlySerializedAs("_currentGameState")] [SerializeField] private GameStage _currentGameStage;
    [SerializeField] private bool _skipIntro;
    [SerializeField] private float _introDuration;
    [SerializeField] private float _deathDuration;
    
    // Dependencies
    private IAnalyticsService _analyticsService;
    private IUGSAuthenticationService _ugsAuthenticationService;
    private IAdvertisementService _advertisementService;


    // State paremeters  
    private bool _isLampDead = false;
    
    private readonly bool GAME_RESET = true;
    private readonly bool GAME_RUNNING = false;
    private readonly bool SAVE_UPGRADES = true;
    private readonly bool DONT_SAVE_UPGRADES = false;

    public void Construct(IAnalyticsService analyticsService, 
        IUGSAuthenticationService ugsAuthenticationService,
        IAdvertisementService advertisementService)
    {
        _analyticsService = analyticsService;
        _analyticsService.OnConsentAddressedEvent += HandleDataConsentAddressed;
        
        _ugsAuthenticationService = ugsAuthenticationService;
        
        _advertisementService = advertisementService;
        _advertisementService.OnAdFinishedEvent += SaveRewards;
    }

    private void OnEnable()
    {
        _googleSheetsDataReader.OnDataLoadedEvent += InitializeEnemyManager; // TODO: use coroutine to wait?
        _uiManager.OnIntroFinishedEvent += OnIntroEnded;
        PlayerInputHandler.OnPlayerAttackEvent += HandlePlayerAttackButtonPressed;
        EnemyManager.OnWaveEndedEvent += HandleWaveEnded;
        Lamp.OnLampDeadEvent += HandleLampDead;
        _lampStatsManager.OnHealthChangeEvent += HandleStatsUpgrade;
        _lampStatsManager.OnCooldownUpgradedEvent += HandleStatsUpgrade;
        _lampStatsManager.OnAttackDistanceUpgradedEvent += HandleStatsUpgrade;
        _uiManager.OnGameoverFinishedEvent += HandleGameoverUiAnimationFinished;
    }

    private void OnDisable()
    {
        _googleSheetsDataReader.OnDataLoadedEvent -= InitializeEnemyManager;
        _uiManager.OnIntroFinishedEvent -= OnIntroEnded;
        _analyticsService.OnConsentAddressedEvent -= HandleDataConsentAddressed;
        PlayerInputHandler.OnPlayerAttackEvent -= HandlePlayerAttackButtonPressed;
        EnemyManager.OnWaveEndedEvent -= HandleWaveEnded;
        Lamp.OnLampDeadEvent -= HandleLampDead;
        _lampStatsManager.OnHealthChangeEvent -= HandleStatsUpgrade;
        _lampStatsManager.OnCooldownUpgradedEvent -= HandleStatsUpgrade;
        _lampStatsManager.OnAttackDistanceUpgradedEvent -= HandleStatsUpgrade;
        _advertisementService.OnAdFinishedEvent -= SaveRewards;
        _uiManager.OnGameoverFinishedEvent -= HandleGameoverUiAnimationFinished;
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Start()
    {
        if (_skipIntro)
        {
            _introDuration = 0.001f;
        }
        _isLampDead = false;
        _currentGameStage = GameStage.Loading;

        // Init all systems
        _saveLoadManager.Initialize();
        // Load game state
        _saveLoadManager.LoadGame();
        _scoresManager.Initialize();
        _lamp.Initialize();
        // _advertisementService.Initialize();

        _uiManager.SetIntroDuration(_introDuration);
        _uiManager.Initialize();
        _playerInputHandler.Initialize();
        _googleSheetsDataReader.Initialize(); // This Method will trigger InitializeEnemyManager and  Switch State
    }

    public void RestartGame(int mode)
    {
        if (mode == 0)
        {
            // Save Upgrades AFTER add is finished
            _advertisementService.ShowAd();
        }
        else
        {
            // Don't Save Upgrades
            _saveLoadManager.SaveGame(GAME_RESET, DONT_SAVE_UPGRADES);
            
            _isLampDead = false;
            _currentGameStage = GameStage.Loading;
            _enemyManager.Restart();
            _uiManager.Initialize();
            _playerInputHandler.Initialize();
            _scoresManager.Initialize();
            _lamp.Initialize();
            _googleSheetsDataReader.Initialize(); // This Method will trigger InitializeEnemyManager and  Switch State
        }
    }
    
    private void SaveRewards()
    {
        _saveLoadManager.SaveGame(GAME_RESET, SAVE_UPGRADES);
        
        _isLampDead = false;
        _currentGameStage = GameStage.Loading;
        _enemyManager.Restart();
        _uiManager.Initialize();
        _playerInputHandler.Initialize();
        _scoresManager.Initialize();
        _lamp.Initialize();
        _googleSheetsDataReader.Initialize(); // This Method will trigger InitializeEnemyManager and  Switch State
    }
    
    private void InitializeEnemyManager()
    {
        SwitchGameState();

        // Initialize enemyManager only after we 100% sure that data was initialized
        _enemyManager.Initialize();
        if (PlayerPrefs.HasKey("dataConsent"))
        {
            // _ugsAuthenticationService.Initialize(); // TODO: move to init area, or find out why it should be there
            // _analyticsService.Initialize();    // The same
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
        if (_currentGameStage == GameStage.Prepare)
        {
            SwitchGameState();
        }
    }
    
    private void HandleStatsUpgrade()
    {
        _saveLoadManager.SaveGame(GAME_RUNNING, SAVE_UPGRADES);
    }
    
    private void HandleWaveEnded(int waveNumber)
    {
        SwitchGameState();
    }
    
    private void HandleLampDead(EnemyBase enemy)
    {
        _isLampDead = true;
        SwitchGameState();
    }
    
    private void HandleGameoverUiAnimationFinished()
    {
        _enemyManager.DeactivateAllEnemies();
    }
   
    private void SwitchGameState()
    {
        switch (_currentGameStage)
        {
            case GameStage.Loading:
                _currentGameStage = GameStage.ConsentScreen;
                break;
            case GameStage.ConsentScreen:
                _currentGameStage = GameStage.Intro;
                _uiManager.PlayIntro();
                _lamp.PlayIntro(_introDuration);
                break;
            case GameStage.Intro:
                _playerInputHandler.EnableAttackInput();
                _uiManager.StartPrepare(_enemyManager.CurrentWave);
                _currentGameStage = GameStage.Prepare;
                break;
            case GameStage.Prepare:
                _currentGameStage = GameStage.Fight;
                _uiManager.StartFight();
                _enemyManager.StartWave();
                break;
            case GameStage.Fight:
                if (_isLampDead)
                {
                    _saveLoadManager.SaveTempData();
                    _playerInputHandler.DisableAttackInput();
                    _lamp.PlayDeath(_deathDuration);
                    _enemyManager.HandleGameOver();
                    _uiManager.StartGameOver();
                    _currentGameStage = GameStage.GameOver;   
                }
                else
                {
                    _saveLoadManager.SaveGame(GAME_RUNNING, SAVE_UPGRADES);
                    _uiManager.StartPrepare(_enemyManager.CurrentWave);
                    _currentGameStage = GameStage.Prepare;    
                }
                break;
        }
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
    
    
}
