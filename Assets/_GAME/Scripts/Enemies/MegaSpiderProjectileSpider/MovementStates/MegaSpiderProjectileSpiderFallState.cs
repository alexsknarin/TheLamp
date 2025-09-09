using System;
using _GAME.Scripts.Lib;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaspiderProjectileSpider.MovementStates
{
    public class MegaspiderProjectileSpiderFallState : EnemyMovementStateBase
    {
        // TODO: Extract fall logic into a static class Methods - move to library
        
        private const float InitialOutForceMagnitude = 1.8f;
        private const float OutForceIncrement = 5f;
        private const float FallForceIncrement = 6f;
        
        private Vector3 _initialDirection;
        private float _outForceMagnitude;
        private float _fallForceMagnitude;
        private bool _isFreeFall;
        
        // Dependencies
        private Transform _bodyTransform;
        private Transform _lampTransform;
        private float _exitYCoordinate;

        public MegaspiderProjectileSpiderFallState(
            Transform bodyTransform, 
            Transform lampTransform,
            float exitYCoordinate,
            bool isFreeFall
            )
        {
            _bodyTransform = bodyTransform;
            _lampTransform = lampTransform;
            _exitYCoordinate = exitYCoordinate;
            _isFreeFall = isFreeFall;
        }
        
        public event Action Ended;
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
            
            _initialDirection = (_bodyTransform.position - _lampTransform.position).normalized;
            _outForceMagnitude = InitialOutForceMagnitude;
            _fallForceMagnitude = 0f;

        }

        public override void Tick()
        {
            _bodyTransform.position += SimplePhysics.Fall(
                _initialDirection, 
                ref _outForceMagnitude, 
                ref _fallForceMagnitude, 
                _isFreeFall,
                InitialOutForceMagnitude,
                OutForceIncrement,
                FallForceIncrement
                );
            
            if (_bodyTransform.position.y < _exitYCoordinate)
            {
                IsReadyToSwitch = true;
            }
        }

        public override void Exit()
        {
            base.Exit();
            Ended?.Invoke();
        }
    }
}
