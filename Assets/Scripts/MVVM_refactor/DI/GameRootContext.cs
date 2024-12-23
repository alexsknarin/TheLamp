using System.Collections.Generic;
using UnityEngine;

public class GameRootContext : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    [SerializeField] private SOGameConfigProvider _gameConfigProvider;
    [Header("Views")]
    [SerializeField] private ConsentSettingsUIView _consentSettingsUIView;
    [SerializeField] private GameStageView _gameStageView;
    [SerializeField] private GameStateView _gameStateView;
    [SerializeField] private PlayerAttackUIView _playerAttackUIView;
    [SerializeField] private LampAttackView _lampAttackView;
    [SerializeField] private LampCooldownView _lampCooldownView;
    [SerializeField] private LampHealthBarView _lampHealthBarView;
    [SerializeField] private LampBlockedModeView _lampBlockedModeView;
    [SerializeField] private LampDamageViewUI _lampDamageViewUI;
    [SerializeField] private LampDamageView _lampDamageView;
    [Header("Services")]
    [SerializeField] private UnityAnalyticsService _unityAnalyticsService;
    [SerializeField] private AdsManager _adsManager;
    [SerializeField] private Game _game;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private Lamp _lamp;
    [Header("Controllers")]
    [SerializeField] private LampHealthBarController _lampHealthBarController;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private PlayerCollidersPropertyController _playerCollidersPropertyController;
    [SerializeField] private PlayerEnemyInteractionHandler _playerEnemyInteractionHandler;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private LampMovementController _lampMovementController;
    private PlayerAttackHandler _playerAttackHandler;

    // [SerializeField] private GameConfig _gameConfig;
    private GameSettingsService _gameSettingsService;
    private UGSAuthenticationService _ugsAuthenticationService;
    
    private IGameSettingsProvider _gameSettingsProvider;
    private GameSettingsModel _gameSettingsModel;
    private GameSettingsViewModel _gameSettingsViewModel;
    private IGameStateProvider _gameStateProvider;
    private GameStageViewModel _gameStageViewModel;
    private GameStateViewModel _gameStateViewModel;
    private PlayerGameplayViewModel _playerGameplayViewModel;
    private SOGameConfigProvider _soGameConfigProvider;
    private GameConfigService _gameConfigService;
    private PlayerAttackViewModel _playerAttackViewModel;
    private ScoresCollectionHandler _scoresCollectionHandler;
    private GameModel _gameModel;

    private List<IDisposable> _disposables = new List<IDisposable>();
    private List<ITickable> _tickables = new List<ITickable>();
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        CoroutineHost coroutineHost = GetComponent<CoroutineHost>();
        
        Debug.Log("------------------------------------------");
        Debug.Log("------ Starting Game Initialization ------");
        Debug.Log("------ Game Settings Initialization ------");
        _gameSettingsProvider = new PlayerPrefsGameSettingsProvider();
        _gameSettingsModel = new GameSettingsModel(_gameSettingsProvider.Get());
        _gameSettingsService = new GameSettingsService(_gameSettingsProvider, _gameSettingsModel);
        _gameSettingsService.Initialize();
        _gameSettingsViewModel = new GameSettingsViewModel(_gameSettingsModel);
        _disposables.Add(_gameSettingsViewModel);
        _gameSettingsViewModel.Initialize();
        
        Debug.Log("------ UI Initialization ------");
        _consentSettingsUIView.Construct(_gameSettingsViewModel);
        _consentSettingsUIView.Initialize();
        
        Debug.Log("------ Analytics Initialization ------");
        _ugsAuthenticationService = new UGSAuthenticationService();
        _ugsAuthenticationService.Initialize();
        _unityAnalyticsService.Construct(_gameSettingsService, _ugsAuthenticationService);
        _unityAnalyticsService.Initialize(); 
        
        
        Debug.Log("------ Game Initialization ------");
        // Game Config
        _gameConfigService = new GameConfigService(_gameConfigProvider);
        _enemyController.Construct(_gameConfigService);
        _lampMovementController.Initialize();
        // Game State       
        _gameStateProvider = new PlayerPrefsGameStateProvider();
        _playerAttackHandler = new PlayerAttackHandler(coroutineHost);
        _scoresCollectionHandler = new ScoresCollectionHandler(_gameConfigService);
        _gameModel = new GameModel(
            _gameStateProvider.Get(), 
            _enemyController, 
            _gameConfigService, 
            _playerAttackHandler, 
            _playerCollidersPropertyController,
            _playerEnemyInteractionHandler,
            _lampMovementController,
            _scoresCollectionHandler);
        _tickables.Add(_playerAttackHandler);
        _lampHealthBarController.Initialize();
        _gameStageViewModel = new GameStageViewModel(_gameModel);
        _disposables.Add(_gameStageViewModel);
        _gameStageView.Construct(_gameStageViewModel);
        _gameStageView.Initialize();
        _playerAttackViewModel = new PlayerAttackViewModel(_gameModel);
        _playerAttackUIView.Construct(_playerAttackViewModel);
        _gameStateViewModel = new GameStateViewModel(_gameModel);
        _gameStateView.Construct(_gameStateViewModel);
        _playerGameplayViewModel = new PlayerGameplayViewModel(_gameModel);
        _lampAttackView.Construct(_playerGameplayViewModel, _gameConfigService);
        _lampAttackView.Initialize();
        _lampCooldownView.Construct(_playerGameplayViewModel);
        _lampCooldownView.Initialize();
        _lampBlockedModeView.Construct(_playerGameplayViewModel);
        _disposables.Add(_playerGameplayViewModel);
        _lampHealthBarView.Construct(_playerGameplayViewModel);
        _playerEnemyInteractionHandler.Initialize();
        _lampDamageViewUI.Construct(_playerGameplayViewModel);
        _lampDamageView.Construct(_playerGameplayViewModel);
        _lampEmissionController.Initialize();
        _scoresCollectionHandler.Initialize();
        _disposables.Add(_scoresCollectionHandler);
        
        
        
        // Configure legacy systems - TEMPORARY
                
        
        _enemyManager.Construct(_unityAnalyticsService);
        _lamp.Construct(_unityAnalyticsService);
        _game.Construct(_unityAnalyticsService, _ugsAuthenticationService, _adsManager);
        
        
        
        // Load Game Config
        _googleSheetsDataReader.OnDataLoadedEvent += OnGameConfigLoaded;
        _googleSheetsDataReader.Initialize();
    }

    private void OnGameConfigLoaded()
    {
        // Start Game
        Debug.Log("------ Game Config Loaded ------");
        _enemyController.Initialize();
        _gameModel.StartGame();
    }
    
    private void Update()
    {
        foreach (var tickable in _tickables)
        {
            tickable.Tick(Time.deltaTime);
        }
    }


    private void OnDestroy()
    {
        _googleSheetsDataReader.OnDataLoadedEvent -= OnGameConfigLoaded;
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }
}
