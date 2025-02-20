using System.Collections.Generic;
using UnityEngine;

public class GameRootContext : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GoogleSheetsDataReader _googleSheetsDataReader;
    [SerializeField] private SoGameConfigProvider _gameConfigProvider;
    [SerializeField] private DefaultGameStateData _defaultGameStateData;
    [SerializeField] private DefaultGameSettingsData _defaultGameSettingsData;
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
    [SerializeField] private CameraShakeService _cameraShakeService;
    [SerializeField] private LampPositionProviderService _lampPositionProviderService;
    [Header("Controllers")]
    [SerializeField] private LampHealthBarController _lampHealthBarController;
    [SerializeField] private EnemyController _enemyController;                          // TODO: remove
    [SerializeField] private WaveEnemyDirector _waveEnemyDirector;
    // [SerializeField] private PlayerCollidersPropertyController _playerCollidersPropertyController;
    // [SerializeField] private PlayerEnemyInteractionHandler _playerEnemyInteractionHandler;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private LampMovementController _lampMovementController;
    [SerializeField] private LightningFlashController _lightningFlashController;
    [SerializeField] private FakeAd _fakeAd;
    [Header("Bosses")]
    [SerializeField] private Megamothling _megamothling;
    [SerializeField] private Wasp _wasp;
    [SerializeField] private FWaspMovement _waspMovement;
    [SerializeField] private Megabeetle _megabeetle;
    [SerializeField] private MegabeetleMovement _megabeetleMovement;
    [SerializeField] private Dragonfly _dragonfly;
    [Header("Scene References")]
    [SerializeField] private Transform _cameraTransform;

    private CoroutineHost _coroutineHost;
    
    private IGameSettingsProviderService _gameSettingsProviderService;
    private AdvertisementBaseService _advertisementService;
    private GameSettingsService _gameSettingsService;
    private UGSAuthenticationService _ugsAuthenticationService;
    private IGameStateProviderService _gameStateProviderService;
    private GameConfigService _gameConfigService;
    private HapticFeedbackService _hapticFeedbackService;
    private UnityAnalyticsService _unityAnalyticsService;
    
    private CameraShakeEventListener _cameraShakeEventListener;
    private PlayerAttackHandler _playerAttackHandler;
    private ScoresCollectionHandler _scoresCollectionHandler;
    private GameModel _gameModel;
    private GameSettingsModel _gameSettingsModel;
    private GameSettingsViewModel _gameSettingsViewModel;
    private GameStageViewModel _gameStageViewModel;
    private GameStateViewModel _gameStateViewModel;
    private PlayerGameplayViewModel _playerGameplayViewModel;
    private PlayerAttackViewModel _playerAttackViewModel;
    private PlayerUpgradeViewModel _playerUpgradeViewModel;
    private GameOverViewModel _gameOverViewModel;
    private HapticFeedbackEventListener _hapticFeedbackEventListener;
    private LightningFlashEventsListener _lightningFlashEventsListener;
    private AnalyticsEventListener _analyticsEventListener;
    private AdvertisementEventListener _advertisementEventListener;
    // +++ Factories
    private BossCameraShakeFactory _bossCameraShakeFactory;
    // Enemy Factories
    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    private FlyMovementStateFactory _flyMovementStateFactory;
    private MothMovementStateFactory _mothMovementStateFactory;
    private SpiderMovementStateFactory _spiderMovementStateFactory;
    private LadybugMovementStateFactory _ladybugMovementStateFactory;
    private FEnemyFactory _enemyFactory;
    
    private FEnemyPool _enemyPool;
    private FEnemySpawner _enemySpawner;
    
    private List<IDisposable> _disposables = new();
    private List<ITickable> _tickables = new();
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        _coroutineHost = GetComponent<CoroutineHost>();
        
        Debug.Log("------------------------------------------");
        Debug.Log("------ Game Initialization ------");
        Debug.Log("------ Services ------");
        ServicesSetup();
        Debug.Log("------ Handlers ------");
        HandlersSetup();
        Debug.Log("------ Factories ------");
        FactoriesSetup();
        Debug.Log("------ Controllers ------");
        ControllersSetup();
        Debug.Log("------ Game Model ------");
        GameModelSetup();
        Debug.Log("------ View Models ------");
        ViewModelsSetup();
        Debug.Log("------ Binding Views ------");
        BindViews();
        Debug.Log("------ Event Listeners ------");
        EventListenersSetup();


        // Bosses TMP
        Debug.Log("------ Bosses Listeners ------");
        _megamothling.Initialize();                                          // TODO: factory should do initialization AND construct
        _wasp.Construct(_gameModel);
        _wasp.Initialize();                                                // TODO: need to spawn bosses - load them later - this is TMP
        _waspMovement.Construct(_lampPositionProviderService);
        _megabeetleMovement.Construct(_gameModel);
        _megabeetle.Initialize();
        _dragonfly.Initialize();

        Debug.Log("------ Loading Gameconfig ------");
        // Load Game Config
        _googleSheetsDataReader.OnDataLoadedEvent += OnGameConfigLoaded;
        _googleSheetsDataReader.Construct(_coroutineHost);
        _googleSheetsDataReader.Initialize();

    }

    private void ServicesSetup()
    {
        // Game Config
        _gameConfigService = new GameConfigService(_gameConfigProvider);
        
        // Game Settings
        _gameSettingsProviderService = new PlayerPrefsGameSettingsProviderService(_defaultGameSettingsData.GameSettings);
        _gameSettingsModel = new GameSettingsModel(_gameSettingsProviderService.Get());
        _gameSettingsService = new GameSettingsService(_gameSettingsProviderService, _gameSettingsModel);
        _gameSettingsService.Initialize();
        
        // Game Services - Analytics
        _ugsAuthenticationService = new UGSAuthenticationService();
        _ugsAuthenticationService.Initialize();
        _unityAnalyticsService = new UnityAnalyticsService(
            _gameSettingsService,
            _ugsAuthenticationService,
            _coroutineHost,
            _gameConfigService
            );
        _disposables.Add(_unityAnalyticsService);
        _unityAnalyticsService.Initialize();

        // Game State
        _gameStateProviderService = new PlayerPrefsGameStateProviderService(_defaultGameStateData.GameState);
        
        // Haptic
        _hapticFeedbackService = new HapticFeedbackService();
        
        // Camera Shake
        _cameraShakeService.Initialize();
        
        // Advertisement
        _advertisementService = new FakeAdService(_fakeAd);
        _advertisementService.Initialize();
    }

    private void HandlersSetup()
    {
        _playerAttackHandler = new PlayerAttackHandler(_coroutineHost);
        _tickables.Add(_playerAttackHandler);
        _scoresCollectionHandler = new ScoresCollectionHandler(_gameConfigService);
        _scoresCollectionHandler.Initialize();
        _disposables.Add(_scoresCollectionHandler);
        // _playerEnemyInteractionHandler.Initialize();
    }

    private void FactoriesSetup()
    {
        // Camera Shake Factories
        _bossCameraShakeFactory = new BossCameraShakeFactory();
        
        // Enemy Factories
        _mothlingMovementStateFactory = new MothlingMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        _flyMovementStateFactory = new FlyMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        _mothMovementStateFactory = new MothMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        _spiderMovementStateFactory = new SpiderMovementStateFactory();
        _ladybugMovementStateFactory = new LadybugMovementStateFactory(
            _cameraTransform,
            _lampPositionProviderService
        );
        
        _enemyFactory = new FEnemyFactory(
            _mothlingMovementStateFactory, 
            _flyMovementStateFactory, 
            _mothMovementStateFactory, 
            _spiderMovementStateFactory,
            _ladybugMovementStateFactory,
            _lampPositionProviderService
        );
        _enemyPool = new FEnemyPool(_enemyFactory);
        _enemyPool.Initialize();
    }

    private void ControllersSetup()
    {
        // _enemyController.Construct(_gameConfigService, _lampPositionProviderService);
        _enemySpawner = new FEnemySpawner(_enemyPool);
        _enemySpawner.Initialize();
        _disposables.Add(_enemySpawner);
        _waveEnemyDirector.Construct(_gameConfigService, _enemySpawner);
        _lampMovementController.Initialize();
        _lampHealthBarController.Initialize();
        _lampEmissionController.Initialize();
        _lightningFlashController.Initialize();
    }

    private void GameModelSetup()
    {
        _gameModel = new GameModel(
            _gameStateProviderService, 
            _gameConfigService, 
            _waveEnemyDirector,
            _playerAttackHandler,
            _lampMovementController,
            _scoresCollectionHandler);
    }

    private void ViewModelsSetup()
    {
        _gameSettingsViewModel = new GameSettingsViewModel(_gameSettingsModel);
        _disposables.Add(_gameSettingsViewModel);
        _gameSettingsViewModel.Initialize();
        _gameStageViewModel = new GameStageViewModel(_gameModel, _gameConfigService);
        _disposables.Add(_gameStageViewModel);
        _playerAttackViewModel = new PlayerAttackViewModel(_gameModel);
        _gameStateViewModel = new GameStateViewModel(_gameModel);
        _playerGameplayViewModel = new PlayerGameplayViewModel(_gameModel);
        _disposables.Add(_playerGameplayViewModel);
        _playerUpgradeViewModel = new PlayerUpgradeViewModel(_gameModel, _gameConfigService);
        _gameOverViewModel = new GameOverViewModel(_gameModel);
    }

    private void BindViews()
    {
        _consentSettingsViewUI.Bind(_gameSettingsViewModel);
        _consentSettingsViewUI.Initialize();
        
        _gameStageView.Bind(_gameStageViewModel);
        _gameStageView.Initialize();
        
        _playerAttackUIView.Bind(_playerAttackViewModel);
        _gameStateView.Bind(_gameStateViewModel);
        
        _lampAttackView.Bind(_playerGameplayViewModel, _gameConfigService);
        _lampAttackView.Initialize();
        
        _lampCooldownView.Bind(_playerGameplayViewModel);
        _lampCooldownView.Initialize();
        
        _lampBlockedModeView.Bind(_playerGameplayViewModel);
        
        _lampHealthBarView.Bind(_playerGameplayViewModel);
        
        _lampDamageViewUI.Bind(_playerGameplayViewModel);
        _lampDamageView.Bind(_playerGameplayViewModel);
        _lampDamageView.Initialize();

        _playerGameplayViewUI.Bind(_playerGameplayViewModel);
        _playerGameplayViewUI.Initialize();
        
        _playerUpgradeViewUI.Bind(_playerUpgradeViewModel);
        _playerUpgradeViewUI.Initialize();
        
        _gameOverViewUI.Bind(_gameOverViewModel);
        _gameOverViewUI.Initialize();
    }

    private void EventListenersSetup()
    {
        _hapticFeedbackEventListener = new HapticFeedbackEventListener(
            _hapticFeedbackService,
            _enemyController,
            _gameModel
        );
        _disposables.Add(_hapticFeedbackEventListener);
        
        // Camera Shake Test
        _cameraShakeEventListener = new CameraShakeEventListener(_gameModel, _enemyController, _cameraShakeService, _bossCameraShakeFactory);
        _disposables.Add(_cameraShakeEventListener);
        
        _lightningFlashEventsListener = new LightningFlashEventsListener(_enemyController, _lightningFlashController);
        _disposables.Add(_lightningFlashEventsListener);
        
        _analyticsEventListener = new AnalyticsEventListener(_unityAnalyticsService, _gameModel);
        
        _advertisementEventListener = new AdvertisementEventListener(_advertisementService, _gameModel);
        _disposables.Add(_advertisementEventListener);
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
    
    // Event Handle Methods 
    private void OnGameConfigLoaded()
    {
        // Start Game
        Debug.Log("-+---- Game Config Loaded ----+-");
        // _enemyController.Initialize();
        _waveEnemyDirector.Initialize();
        _gameModel.StartGame();
    }
}
