using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameRootContext : MonoBehaviour
{
    [SerializeField] private ConsentSettingsUIView _consentSettingsUIView;
    [SerializeField] private UnityAnalyticsService _unityAnalyticsService;
    [SerializeField] private AdsManager _adsManager;
    [SerializeField] private Game _game;
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private Lamp _lamp;
    
    private GameSettingsService _gameSettingsService;
    private UGSAuthenticationService _ugsAuthenticationService;
    
    private List<IDisposable> _disposables = new List<IDisposable>();
    
    
    
    private void Awake()
    {
        Debug.Log("------------------------------------------");
        Debug.Log("------ Starting Game Initialization ------");
        Debug.Log("------ Game Settings Initialization ------");
        // TODO: Make game services as fields ????
        IGameSettingsProvider gameSettingsProvider = new PlayerPrefsGameSettingsProvider();
        GameSettingsModel gameSettingsModel = new GameSettingsModel(gameSettingsProvider.Get());
        _gameSettingsService = new GameSettingsService(gameSettingsProvider, gameSettingsModel);
        _gameSettingsService.Initialize();
        GameSettingsViewModel gameSettingsViewModel = new GameSettingsViewModel(gameSettingsModel);
        _disposables.Add(gameSettingsViewModel);
        gameSettingsViewModel.Initialize();
        
        Debug.Log("------ UI Initialization ------");
        _consentSettingsUIView.Bind(gameSettingsViewModel);
        _consentSettingsUIView.Initialize();
        
        Debug.Log("------ Analytics Initialization ------");
        _ugsAuthenticationService = new UGSAuthenticationService();
        _ugsAuthenticationService.Initialize();
        _unityAnalyticsService.Construct(_gameSettingsService, _ugsAuthenticationService);
        _unityAnalyticsService.Initialize(); 
        
        
        Debug.Log("------ Game Initialization ------");
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
