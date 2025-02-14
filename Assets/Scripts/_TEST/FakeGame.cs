using System;
using UnityEngine;

public class FakeGame : MonoBehaviour
{
    [SerializeField] private WaveEnemyDirector _waveEnemyDirector;
    [SerializeField] private SoGameConfigProvider _gameConfigProvider;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private LampPositionProviderService _lampPositionProviderService;
    [SerializeField] private LampCollisionDetectionService _lampCollisionDetectionService;
    private IGameConfigService _gameConfigService;
    private MothlingMovementStateFactory _mothlingMovementStateFactory;
    private FlyMovementStateFactory _flyMovementStateFactory;
    private MothMovementStateFactory _mothMovementStateFactory;
    private SpiderMovementStateFactory _spiderMovementStateFactory;
    private FEnemyFactory _enemyFactory;
    private FEnemySpawner _enemySpawner;

    private void Awake()
    {
        _gameConfigService = new GameConfigService(_gameConfigProvider);
        _mothlingMovementStateFactory = new MothlingMovementStateFactory(_cameraTransform, _lampPositionProviderService);
        _flyMovementStateFactory = new FlyMovementStateFactory(_cameraTransform, _lampPositionProviderService);
        _mothMovementStateFactory = new MothMovementStateFactory(_cameraTransform, _lampPositionProviderService);
        _spiderMovementStateFactory = new SpiderMovementStateFactory();
        
        _enemyFactory = new FEnemyFactory(
            _mothlingMovementStateFactory,
            _flyMovementStateFactory,
            _mothMovementStateFactory,
            _spiderMovementStateFactory,
            _lampPositionProviderService
            );
        _enemySpawner = new FEnemySpawner(_enemyFactory);
        _waveEnemyDirector.Construct(_gameConfigService, _lampCollisionDetectionService, _enemySpawner);
        _waveEnemyDirector.Initialize();
    }

    private void Start()
    {
        _waveEnemyDirector.HandleStartGame();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _waveEnemyDirector.PrepareWave(6);
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Starting Fake Wave");
            _waveEnemyDirector.StartWave();
        }
        
    }
}
