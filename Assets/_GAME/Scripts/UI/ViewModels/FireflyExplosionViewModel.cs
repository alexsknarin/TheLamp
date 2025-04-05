using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.GameCoreSystems.EnemyManagement;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using IDisposable = _GAME.Scripts.Lib.Interfaces.IDisposable;

namespace _GAME.Scripts.UI.ViewModels
{
    public class FireflyExplosionViewModel: IInitializable, IDisposable
    {
        private WaveEnemyDirector _waveEnemyDirector;
    
        private bool _isExplosionLoadRequested = false;
        public event Action FireflyExplosionLoadRequested;
        public event Action<Vector2> FireflyExplosionStarted;
    
        public FireflyExplosionViewModel(WaveEnemyDirector waveEnemyDirector)
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

        private void OnExplodableEnemyDeactivated(Enemy enemy)
        {
            FireflyExplosionStarted?.Invoke(enemy.transform.position);
        }
    }
}
