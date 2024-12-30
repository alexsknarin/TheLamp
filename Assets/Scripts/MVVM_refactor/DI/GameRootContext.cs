using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameRootContext : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    [FormerlySerializedAs("_gameConfigProviderServiceService")] [FormerlySerializedAs("_gameConfigProvider")] [SerializeField] private SoGameConfigProviderService _gameConfigProviderService;
    [SerializeField] private DefaultGameStateData _defaultGameStateData;
    [SerializeField] private DefaultGameSettingsData _defaultGameSettingsData;
    [FormerlySerializedAs("_consentSettingsUIView")]
    [Header("Views")]
    [SerializeField] private ConsentSettingsViewUI _consentSettingsViewUI;
    [SerializeField] private GameStageView _gameStageView;
    [SerializeField] private GameStateView _gameStateView;
    [SerializeField] private PlayerAttackUIView _playerAttackUIView;
    [SerializeField] private LampAttackView _lampAttackView;
    [SerializeField] private LampCooldownView _lampCooldownView;
    [SerializeField] private LampHealthBarView _lampHealthBarView;
    [SerializeField] private LampBlockedModeView _lampBlockedModeView;
    [SerializeField] private LampDamageViewUI _lampDamageViewUI;
    [SerializeField] private LampDamageView _lampDamageView;
    [SerializeField] private PlayerUpgradeViewUI _playerUpgradeViewUI;
    [SerializeField] private GameOverViewUI _gameOverViewUI;
    [SerializeField] private PlayerGameplayViewUI _playerGameplayViewUI;
    [Header("Services")]
    [SerializeField] private UnityAnalyticsService _unityAnalyticsService;
    [SerializeField] private LampPositionProviderService _lampPositionProviderService;
    [Header("Controllers")]
    [SerializeField] private LampHealthBarController _lampHealthBarController;
    [SerializeField] private EnemyController _enemyController;
    [SerializeField] private PlayerCollidersPropertyController _playerCollidersPropertyController;
    [SerializeField] private PlayerEnemyInteractionHandler _playerEnemyInteractionHandler;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private LampMovementController _lampMovementController;
    [Header("Bosses")]
    [SerializeField] private Wasp _wasp;
    [SerializeField] private FWaspMovement _waspMovement;
    [SerializeField] private MegabeetleMovement _megabeetleMovement;
    
    private PlayerAttackHandler _playerAttackHandler;
    private GameSettingsService _gameSettingsService;
    private UGSAuthenticationService _ugsAuthenticationService;
    
    private IGameSettingsProviderService _gameSettingsProviderService;
    private GameSettingsModel _gameSettingsModel;
    private GameSettingsViewModel _gameSettingsViewModel;
    private IGameStateProviderService _gameStateProviderService;
    private GameStageViewModel _gameStageViewModel;
    private GameStateViewModel _gameStateViewModel;
    private PlayerGameplayViewModel _playerGameplayViewModel;
    private SoGameConfigProviderService _soGameConfigProviderService;
    private GameConfigService _gameConfigService;
    private PlayerAttackViewModel _playerAttackViewModel;
    private ScoresCollectionHandler _scoresCollectionHandler;
    private PlayerUpgradeViewModel _playerUpgradeViewModel;
    private GameOverViewModel _gameOverViewModel;
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
        _gameSettingsProviderService = new PlayerPrefsGameSettingsProviderService(_defaultGameSettingsData.GameSettings);
        _gameSettingsModel = new GameSettingsModel(_gameSettingsProviderService.Get());
        _gameSettingsService = new GameSettingsService(_gameSettingsProviderService, _gameSettingsModel);
        _gameSettingsService.Initialize();
        _gameSettingsViewModel = new GameSettingsViewModel(_gameSettingsModel);
        _disposables.Add(_gameSettingsViewModel);
        _gameSettingsViewModel.Initialize();
        
        Debug.Log("------ UI Initialization ------");
        _consentSettingsViewUI.Bind(_gameSettingsViewModel);
        _consentSettingsViewUI.Initialize();
        
        Debug.Log("------ Analytics Initialization ------");
        _ugsAuthenticationService = new UGSAuthenticationService();
        _ugsAuthenticationService.Initialize();
        _unityAnalyticsService.Construct(_gameSettingsService, _ugsAuthenticationService);
        _unityAnalyticsService.Initialize(); 
        
        
        Debug.Log("------ Game Initialization ------");
        // Game Config
        _gameConfigService = new GameConfigService(_gameConfigProviderService);
        _enemyController.Construct(_gameConfigService);
        _lampMovementController.Initialize();
        // Game State       
        _gameStateProviderService = new PlayerPrefsGameStateProviderService(_defaultGameStateData.GameState);
        _playerAttackHandler = new PlayerAttackHandler(coroutineHost);
        _scoresCollectionHandler = new ScoresCollectionHandler(_gameConfigService);
        _gameModel = new GameModel(
            _gameStateProviderService, 
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
        _gameStageView.Bind(_gameStageViewModel);
        _gameStageView.Initialize();
        _playerAttackViewModel = new PlayerAttackViewModel(_gameModel);
        _playerAttackUIView.Bind(_playerAttackViewModel);
        _gameStateViewModel = new GameStateViewModel(_gameModel);
        _gameStateView.Bind(_gameStateViewModel);
        _playerGameplayViewModel = new PlayerGameplayViewModel(_gameModel);
        _lampAttackView.Bind(_playerGameplayViewModel, _gameConfigService);
        _lampAttackView.Initialize();
        _lampCooldownView.Bind(_playerGameplayViewModel);
        _lampCooldownView.Initialize();
        _lampBlockedModeView.Bind(_playerGameplayViewModel);
        _disposables.Add(_playerGameplayViewModel);
        _lampHealthBarView.Bind(_playerGameplayViewModel);
        _playerEnemyInteractionHandler.Initialize();
        _lampDamageViewUI.Bind(_playerGameplayViewModel);
        _lampDamageView.Bind(_playerGameplayViewModel);
        _lampEmissionController.Initialize();
        _scoresCollectionHandler.Initialize();
        _disposables.Add(_scoresCollectionHandler);
        _playerGameplayViewUI.Bind(_playerGameplayViewModel);
        _playerGameplayViewUI.Initialize();
        _playerUpgradeViewModel = new PlayerUpgradeViewModel(_gameModel, _gameConfigService);
        _playerUpgradeViewUI.Bind(_playerUpgradeViewModel);
        _playerUpgradeViewUI.Initialize();
        _gameOverViewModel = new GameOverViewModel(_gameModel);
        _gameOverViewUI.Bind(_gameOverViewModel);
        _gameOverViewUI.Initialize();
        
        // Bosses
        _wasp.Construct(_gameModel);
        _waspMovement.Construct(_lampPositionProviderService);
        _megabeetleMovement.Construct(_gameModel);
        
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
