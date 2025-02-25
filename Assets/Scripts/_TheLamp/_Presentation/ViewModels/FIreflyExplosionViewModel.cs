using System;
using UnityEngine;

public class FIreflyExplosionViewModel: IInitializable, IDisposable
{
    private WaveEnemyDirector _waveEnemyDirector;
    
    private bool _isExplosionLoadRequested = false;
    public event Action FireflyExplosionLoadRequested;
    public event Action<Vector2> FireflyExplosionStarted;
    
    public FIreflyExplosionViewModel(WaveEnemyDirector waveEnemyDirector)
    {
        _waveEnemyDirector = waveEnemyDirector;
    }

    public void Initialize()
    {
        _waveEnemyDirector.ExplodableEnemySpawned += OnExplodableEnemySpawned;
        _waveEnemyDirector.ExplodableEnemyDeactivated += OnExplodableEnemyDeactivated;
    }

    public void Dispose()
    {
        _waveEnemyDirector.ExplodableEnemySpawned -= OnExplodableEnemySpawned;
        _waveEnemyDirector.ExplodableEnemyDeactivated -= OnExplodableEnemyDeactivated;
    }

    private void OnExplodableEnemySpawned()
    {
        if (!_isExplosionLoadRequested)
        {
            _isExplosionLoadRequested = true;
            FireflyExplosionLoadRequested?.Invoke();
        }
    }

    private void OnExplodableEnemyDeactivated(FEnemy enemy)
    {
        FireflyExplosionStarted?.Invoke(enemy.transform.position);
    }
}
