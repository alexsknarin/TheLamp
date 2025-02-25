using UnityEngine;

public class CameraShakeEventListener: IDisposable
{
    private GameModel _gameModel;
    private WaveEnemyDirector _waveEnemyDirector;
    private CameraShakeService _cameraShakeService;
    private BossCameraShakeFactory _bossCameraShakeFactory;


    public CameraShakeEventListener(
        GameModel gameModel,
        WaveEnemyDirector waveEnemyDirector,
        CameraShakeService cameraShakeService,
        BossCameraShakeFactory bossCameraShakeFactory
        )
    {
        _gameModel = gameModel;
        _waveEnemyDirector = waveEnemyDirector;
        _cameraShakeService = cameraShakeService;
        _bossCameraShakeFactory = bossCameraShakeFactory;
        
        _gameModel.LampDamageStarted += OnLampDamageStarted;
        _gameModel.LampDestroyed += OnLampDestroyed;
        _waveEnemyDirector.FireflyExplosionStarted += OnFireflyExplosionStarted;
        
        // _enemyController.BossSpawned += OnBossSpawned;
        // _enemyController.BossDied += OnBossDied;
    }

    public void Dispose()
    {
        _gameModel.LampDamageStarted -= OnLampDamageStarted;
        _gameModel.LampDestroyed -= OnLampDestroyed;
        _waveEnemyDirector.FireflyExplosionStarted -= OnFireflyExplosionStarted;
        
        // _enemyController.BossSpawned -= OnBossSpawned;
        // _enemyController.BossDied -= OnBossDied;
    }

    private void OnFireflyExplosionStarted()
    {
        _cameraShakeService.StartExplosionShake();
    }

    private void OnLampDestroyed()
    {
        _cameraShakeService.StartDamageShake();
        _cameraShakeService.DisableBossShake();
    }

    private void OnLampDamageStarted(float obj, string enemy)
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
