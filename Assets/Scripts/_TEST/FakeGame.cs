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
    private FEnemyFactory _enemyFactory;
    private FEnemySpawner _enemySpawner;

    private void Awake()
    {
        _gameConfigService = new GameConfigService(_gameConfigProvider);
        _mothlingMovementStateFactory = new MothlingMovementStateFactory(_cameraTransform, _lampPositionProviderService);
        _enemyFactory = new FEnemyFactory(_mothlingMovementStateFactory);
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
            _waveEnemyDirector.PrepareWave(1);
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            _waveEnemyDirector.StartWave(1);
        }
        
    }
}
