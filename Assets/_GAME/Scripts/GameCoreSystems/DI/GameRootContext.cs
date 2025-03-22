using System.Collections.Generic;
using _GAME.Scripts.Factories;
using _GAME.Scripts.GameCoreSystems.DataManagement;
using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;
using _GAME.Scripts.GameCoreSystems.DataManagement.GameDataHandlers;
using _GAME.Scripts.GameCoreSystems.EnemyManagement;
using _GAME.Scripts.InGamePresentation.CameraShake;
using _GAME.Scripts.InGamePresentation.FX.Views;
using _GAME.Scripts.InGamePresentation.GameStageTransitions;
using _GAME.Scripts.InGamePresentation.Haptic;
using _GAME.Scripts.InGamePresentation.Lightning;
using _GAME.Scripts.Lamp;
using _GAME.Scripts.Lamp.Views;
using _GAME.Scripts.Lib.Interfaces;
using _GAME.Scripts.ServicesGlobal;
using _GAME.Scripts.ServicesGlobal.Advertisement;
using _GAME.Scripts.UI.ViewModels;
using _GAME.Scripts.UI.Views;
using UnityEngine;

namespace _GAME.Scripts.GameCoreSystems.DI
{
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
        [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
        [SerializeField] private LampStickyDetectionService _lampStickyDetectionService;
        [Header("Controllers")]
        [SerializeField] private LampHealthBarController _lampHealthBarController;
        [SerializeField] private WaveEnemyDirector _waveEnemyDirector;
        [SerializeField] private LampEmissionController _lampEmissionController;
        [SerializeField] private LampMovementController _lampMovementController;
        [SerializeField] private LightningFlashController _lightningFlashController;
        [SerializeField] private FakeAd _fakeAd;
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
        private PlayerAttackCooldownHandler _playerAttackCooldownHandler;
        private ScoresCollectionController _scoresCollectionController;
        private PlayerEnemyInteractionMediator _playerEnemyInteractionMediator;
    
        private GameModel _gameModel;
        private GameSettingsModel _gameSettingsModel;
    
        private GameSettingsViewModel _gameSettingsViewModel;
        private GameStageViewModel _gameStageViewModel;
        private GameStateViewModel _gameStateViewModel;
        private PlayerGameplayViewModel _playerGameplayViewModel;
        private PlayerAttackViewModel _playerAttackViewModel;
        private PlayerUpgradeViewModel _playerUpgradeViewModel;
        private GameOverViewModel _gameOverViewModel;
        private FireflyExplosionViewModel _fireflyExplosionViewModel;
    
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
        private MegamothlingMovementStateFactory _megamothlingMovementStateFactory;
        private MegabeetleMovementStateFactory _megabeetleMovementStateFactory;
        private EnemyFactory _enemyFactory;
        private FXFactory _fxFactory;   
    
        private EnemyPool _enemyPool;
        private EnemySpawner _enemySpawner;
    
        private FireflyExplosionView _fireflyExplosionView;
    
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
        
            // Lamp 
            _lampCollisionDetectionService.Initialize();
        }

        private void HandlersSetup()
        {
            _playerAttackCooldownHandler = new PlayerAttackCooldownHandler(_coroutineHost);
            _tickables.Add(_playerAttackCooldownHandler);
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
                _lampPositionProviderService,
                _gameConfigService
            );
            _megamothlingMovementStateFactory = new MegamothlingMovementStateFactory(
                _cameraTransform,
                _lampPositionProviderService
            );
        
            _megabeetleMovementStateFactory = new MegabeetleMovementStateFactory(
                _cameraTransform,
                _lampPositionProviderService
            );
        
            _enemyFactory = new EnemyFactory(
                _mothlingMovementStateFactory, 
                _flyMovementStateFactory, 
                _mothMovementStateFactory, 
                _spiderMovementStateFactory,
                _ladybugMovementStateFactory,
                _megamothlingMovementStateFactory,
                _megabeetleMovementStateFactory,
                _lampPositionProviderService,
                _gameConfigService
            );
            _enemyPool = new EnemyPool(_enemyFactory);
            _enemyPool.Initialize();
        
            _fxFactory = new FXFactory(_gameConfigService);
        }

        private void ControllersSetup()
        {
            _enemySpawner = new EnemySpawner(_enemyPool);
            _enemySpawner.Initialize();
            _tickables.Add(_enemySpawner);
            _disposables.Add(_enemySpawner);
        
            _lampStickyDetectionService.Initialize();
        
            _waveEnemyDirector.Construct(_gameConfigService, _enemySpawner);
        
            _scoresCollectionController = new ScoresCollectionController(_gameConfigService, _enemyPool, _waveEnemyDirector);
            _scoresCollectionController.Initialize();
            _disposables.Add(_scoresCollectionController);


            _lampMovementController.Initialize();
            _lampHealthBarController.Initialize();
            _lampEmissionController.Initialize();
            _lightningFlashController.Initialize();
        
            _playerEnemyInteractionMediator = new PlayerEnemyInteractionMediator(
                _waveEnemyDirector, 
                _lampCollisionDetectionService, 
                _lampStickyDetectionService
            );
            _playerEnemyInteractionMediator.Initialize();
            _disposables.Add(_playerEnemyInteractionMediator);
        }

        private void GameModelSetup()
        {
            _gameModel = new GameModel(
                _gameStateProviderService, 
                _gameConfigService, 
                _waveEnemyDirector,
                _playerAttackCooldownHandler,
                _playerEnemyInteractionMediator,
                _lampMovementController,
                _scoresCollectionController);
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
       
            _fireflyExplosionViewModel = new FireflyExplosionViewModel(_waveEnemyDirector);
            _fireflyExplosionViewModel.Initialize();
            _disposables.Add(_fireflyExplosionViewModel);
        
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
        
            _fireflyExplosionView = new FireflyExplosionView(_fxFactory, _fireflyExplosionViewModel);
            _fireflyExplosionView.Initialize();
            _disposables.Add(_fireflyExplosionView);
        }

        private void EventListenersSetup()
        {
            _hapticFeedbackEventListener = new HapticFeedbackEventListener(
                _hapticFeedbackService,
                _waveEnemyDirector,
                _gameModel
            );
            _disposables.Add(_hapticFeedbackEventListener);
        
            // Camera Shake Test
            _cameraShakeEventListener = new CameraShakeEventListener(
                _gameModel, 
                _waveEnemyDirector, 
                _cameraShakeService, 
                _bossCameraShakeFactory);
            _disposables.Add(_cameraShakeEventListener);
        
            _lightningFlashEventsListener = new LightningFlashEventsListener(
                _waveEnemyDirector,
                _lightningFlashController);
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
            _waveEnemyDirector.Initialize();
            _gameModel.StartGame();
        }
    }
}
