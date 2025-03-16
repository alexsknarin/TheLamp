using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies
{
    public class FireflyExplosionEnemyDamager: ITickable, IInitializable
    {
        private IGameConfigService _gameConfigService;
    
        public FireflyExplosionEnemyDamager(IGameConfigService gameConfigService)
        {
            _gameConfigService = gameConfigService;
        }
    
        private List<FEnemy> _enemies;
        private Vector2 _explosionPosition;
        private float _explosionRadius;   // TODO: read from config
        private float _explosionDuration; // TODO: read from config
    
        private bool _isExploding = false;
        private float _localTime;

        public void Initialize()
        {
            _explosionRadius = _gameConfigService.GameConfig.FireflyExplosionRadius;
            _explosionDuration = _gameConfigService.GameConfig.FireflyExplosionDuration;
        }

        public void StartExplosion(Vector2 explosionPosition, List<FEnemy> enemies)
        {
            _enemies = enemies;
            _explosionPosition = explosionPosition;
            _localTime = 0;
            _isExploding = true;
        }


        public void Tick(float deltaTime)
        {
            if (_isExploding)
            {
                if (_localTime < _explosionDuration)
                {
                    PerformExplosion();
                    _localTime += deltaTime;
                }
                else
                {
                    _isExploding = false;
                }
            }
        }

        private void PerformExplosion()
        {
            foreach (var enemy in _enemies)
            {
                // TODO: take camera projection into account
                Vector2 enemyPosition2d = enemy.transform.position;
                if((_explosionPosition - enemyPosition2d).magnitude < _explosionRadius)
                {
                    enemy.ReceiveDamage(100);
                }
            }
        }
    }
}
