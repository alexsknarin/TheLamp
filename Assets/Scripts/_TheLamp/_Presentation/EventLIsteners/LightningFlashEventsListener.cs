public class LightningFlashEventsListener: IDisposable
{
    // private EnemyController _enemyController;
    private LightningFlashController _lightningFlashController;

    public LightningFlashEventsListener(
        // EnemyController enemyController,
        LightningFlashController lightningFlashController
        )
    {
        // _enemyController = enemyController;
        _lightningFlashController = lightningFlashController;
        
        // _enemyController.BossSpawned += OnBossSpawned;
        // _enemyController.BossDied += OnBossDied;
    }

    public void Dispose()
    {
        // _enemyController.BossSpawned -= OnBossSpawned;
        // _enemyController.BossDied -= OnBossDied;
    }

    private void OnBossSpawned(BossBase obj)
    {
        _lightningFlashController.Play();
    }

    private void OnBossDied(EnemyBase obj)
    {
        _lightningFlashController.Play();
    }
}
