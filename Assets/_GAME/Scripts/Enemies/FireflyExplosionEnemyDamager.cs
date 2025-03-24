using System.Collections.Generic;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies
{
    public class FireflyExplosionEnemyDamager: ITickable, IInitializable
    {
        private IGameConfigService _gameConfigService;
        private Transform _cameraTransform;
    
        public FireflyExplosionEnemyDamager(
            IGameConfigService gameConfigService,
            Transform cameraTransform)
        {
            _gameConfigService = gameConfigService;
            _cameraTransform = cameraTransform;
        }
    
        private List<FEnemy> _enemies;
        private Vector2 _explosionPosition;
        private float _explosionRadius;
        private float _explosionDuration;
    
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
                Vector2 enemyPosition2d = CameraProjection.ProjectPointOnXYPlane(
                    _cameraTransform.position,
                    enemy.transform.position
                    );
                if((_explosionPosition - enemyPosition2d).magnitude < _explosionRadius)
                {
                    enemy.ReceiveDamage(100);
                }
            }
        }
    }
}
