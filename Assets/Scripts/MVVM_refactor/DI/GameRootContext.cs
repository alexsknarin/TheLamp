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
    [Header("Services")]
    [SerializeField] private UnityAnalyticsService _unityAnalyticsService;
    [SerializeField] private AdsManager _adsManager;
    [SerializeField] private Game _game;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private Lamp _lamp;
    [SerializeField] private LampHealthBar _lampHealthBar;
    [Header("Controllers")]
    [SerializeField] private EnemyController _enemyController;
    
    // [SerializeField] private GameConfig _gameConfig;
    
    
    private GameSettingsService _gameSettingsService;
    private UGSAuthenticationService _ugsAuthenticationService;
    
    private List<IDisposable> _disposables = new List<IDisposable>();
    private IGameSettingsProvider _gameSettingsProvider;
    private GameSettingsModel _gameSettingsModel;
    private GameSettingsViewModel _gameSettingsViewModel;
    private IGameStateProvider _gameStateProvider;
    private GameModel _gameModel;
    private GameStageViewModel _gameStageViewModel;
    private GameStateViewModel _gameStateViewModel;
    private SOGameConfigProvider _soGameConfigProvider;
    private GameConfigService _gameConfigService;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        Debug.Log("------------------------------------------");
        Debug.Log("------ Starting Game Initialization ------");
        Debug.Log("------ Game Settings Initialization ------");
        // TODO: Make game services as fields ????
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
        // Game State       
        _gameStateProvider = new PlayerPrefsGameStateProvider();
        _gameModel = new GameModel(_gameStateProvider.Get());
        _gameStageViewModel = new GameStageViewModel(_gameModel);
        _disposables.Add(_gameStageViewModel);
        _gameStageView.Construct(_gameStageViewModel);
        _gameStageView.Initialize();
        
        _gameStateViewModel = new GameStateViewModel(_gameModel);
        _gameStateView.Construct(_gameStateViewModel);
        _lampHealthBar.Initialize();
        
        
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
        _gameModel.Start();
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
