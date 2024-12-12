using System.Collections.Generic;
using UnityEngine;

public class GameRootContext : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private ConsentSettingsUIView _consentSettingsUIView;
    [SerializeField] private GameStageView _gameStageView;
    [Header("Services")]
    [SerializeField] private UnityAnalyticsService _unityAnalyticsService;
    [SerializeField] private AdsManager _adsManager;
    [SerializeField] private Game _game;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private Lamp _lamp;
    [SerializeField] private GameConfig _gameConfig;
    
    private GameSettingsService _gameSettingsService;
    private UGSAuthenticationService _ugsAuthenticationService;
    
    private List<IDisposable> _disposables = new List<IDisposable>();
    private IGameSettingsProvider _gameSettingsProvider;
    private GameSettingsModel _gameSettingsModel;
    private GameSettingsViewModel _gameSettingsViewModel;
    private IGameStateProvider _gameStateProvider;
    private GameModel _gameModel;
    private GameStageViewModel _gameStageViewModel;


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
        _consentSettingsUIView.Bind(_gameSettingsViewModel);
        _consentSettingsUIView.Initialize();
        
        Debug.Log("------ Analytics Initialization ------");
        _ugsAuthenticationService = new UGSAuthenticationService();
        _ugsAuthenticationService.Initialize();
        _unityAnalyticsService.Construct(_gameSettingsService, _ugsAuthenticationService);
        _unityAnalyticsService.Initialize(); 
        
        
        Debug.Log("------ Game Initialization ------");
        _gameStateProvider = new PlayerPrefsGameStateProvider();
        _gameModel = new GameModel(_gameStateProvider.Get());
        _gameStageViewModel = new GameStageViewModel(_gameModel);
        _disposables.Add(_gameStageViewModel);
        _gameStageView.Bind(_gameStageViewModel);
        
        
        
        
        // Start Game
        _gameModel.Start();
        
        
        
        
        
        
        
        // Configure legcy systems
        _enemyManager.Construct(_unityAnalyticsService);
        _lamp.Construct(_unityAnalyticsService);
        _game.Construct(_unityAnalyticsService, _ugsAuthenticationService, _adsManager);
        
        
        
    }
    
    
    private void OnDestroy()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }
}
