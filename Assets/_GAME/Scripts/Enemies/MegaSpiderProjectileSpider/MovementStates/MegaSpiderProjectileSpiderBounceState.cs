using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaspiderProjectileSpider.MovementStates
{
    public class MegaspiderProjectileSpiderBounceState : EnemyMovementStateBase
    {
        // Dependencies
        private readonly Transform _bodyTransform;
        private readonly Transform _lampTransform;
        // Config
        private readonly float _speed;
        
        private Vector3 _direction;

        public MegaspiderProjectileSpiderBounceState(
            Transform bodyTransform, 
            Transform lampTransform,
            IGameConfigService configService
            )
        {
            _bodyTransform = bodyTransform;
            _lampTransform = lampTransform;
            _speed = configService.GameConfig.MegaspiderProjectileSpiderBounceSpeed;
        }
        
        public event Action Ended;
        
        public override void Enter()
        {
            _direction = (_bodyTransform.position - _lampTransform.position).normalized;
        }

        public override void Tick()
        {
            Vector3 newPosition = _bodyTransform.position;
            newPosition += _direction * (_speed * Time.deltaTime);
            
            _bodyTransform.position = newPosition;
        }
        
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
