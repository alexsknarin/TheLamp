public class CameraShakeEventListener: IDisposable
{
    private GameModel _gameModel;
    private EnemyController _enemyController;
    private CameraShakeService _cameraShakeService;
    private BossCameraShakeFactory _bossCameraShakeFactory;


    public CameraShakeEventListener(
        GameModel gameModel,
        EnemyController enemyController,
        CameraShakeService cameraShakeService,
        BossCameraShakeFactory bossCameraShakeFactory
        )
    {
        _gameModel = gameModel;
        _enemyController = enemyController;
        _cameraShakeService = cameraShakeService;
        _bossCameraShakeFactory = bossCameraShakeFactory;
        
        _gameModel.LampDamageStarted += OnLampDamageStarted;
        _gameModel.LampDied += OnLampDied;
        _enemyController.FireflyExplosionStarted += OnFireflyExplosionStarted;
        _enemyController.BossSpawned += OnBossSpawned;
        _enemyController.BossDied += OnBossDied;
    }

    public void Dispose()
    {
        _gameModel.LampDamageStarted -= OnLampDamageStarted;
        _gameModel.LampDied -= OnLampDied;
        _enemyController.FireflyExplosionStarted -= OnFireflyExplosionStarted;
        _enemyController.BossSpawned -= OnBossSpawned;
    }

    private void OnFireflyExplosionStarted()
    {
        _cameraShakeService.StartExplosionShake();
    }

    private void OnLampDied(EnemyBase obj)
    {
        _cameraShakeService.StartDamageShake();
    }

    private void OnLampDamageStarted(float obj)
    {
        _cameraShakeService.StartDamageShake();
    }

    private void OnBossSpawned(BossBase boss)
    {
        _cameraShakeService.EnableBossShake(_bossCameraShakeFactory.Create(boss)); 
    }

    private void OnBossDied(EnemyBase obj)
    {
        _cameraShakeService.DisableBossShake();
    }
}
