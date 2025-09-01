using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaspiderProjectileSpider.MovementStates
{
    public class MegaspiderProjectileSpiderBounceState : EnemyMovementStateBase
    {
        private const float Speed = 2.5f; // TODO: To Config

        private Vector3 _direction;

        // Dependencies
        private Transform _bodyTransform;
        private Transform _lampTransform;

        public MegaspiderProjectileSpiderBounceState(
            Transform bodyTransform, 
            Transform lampTransform
            )
        {
            _bodyTransform = bodyTransform;
            _lampTransform = lampTransform;
        }
        
        public event Action Ended;
        
        public override void Enter()
        {
            _direction = (_bodyTransform.position - _lampTransform.position).normalized;
        }

        public override void Tick()
        {
            Vector3 newPosition = _bodyTransform.position;
            newPosition += _direction * (Speed * Time.deltaTime);
            
            _bodyTransform.position = newPosition;
        }
        
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
