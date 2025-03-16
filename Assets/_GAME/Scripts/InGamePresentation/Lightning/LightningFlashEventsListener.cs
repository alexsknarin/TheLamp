public class LightningFlashEventsListener: IDisposable
{
    private WaveEnemyDirector _waveEnemyDirector;
    private LightningFlashController _lightningFlashController;

    public LightningFlashEventsListener(
        WaveEnemyDirector waveEnemyDirector,
        LightningFlashController lightningFlashController
        )
    {
        _waveEnemyDirector = waveEnemyDirector;
        _lightningFlashController = lightningFlashController;
        
        _waveEnemyDirector.BossSpawned += OnBossSpawned;
        _waveEnemyDirector.BossDied += OnBossDied;
    }

    public void Dispose()
    {
        _waveEnemyDirector.BossSpawned -= OnBossSpawned;
        _waveEnemyDirector.BossDied -= OnBossDied;
    }

    private void OnBossSpawned(FEnemy boss)
    {
        _lightningFlashController.Play();
    }

    private void OnBossDied()
    {
        _lightningFlashController.Play();
    }
}
