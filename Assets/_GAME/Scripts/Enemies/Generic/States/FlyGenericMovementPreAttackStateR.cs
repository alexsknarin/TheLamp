using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.States
{
    public class FlyGenericMovementPreAttackStateR: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private float _speed;

        // State specific attributes
        private readonly float _duration;
        private readonly float _acceleration = 0.93f;
        private float _acceleratedSpeed;
        private Vector2 _direction;
        private float _localTime;

        public FlyGenericMovementPreAttackStateR(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            float speed,
            float duration
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
            _duration = duration;
        }

        public event Action Started;
        public event Action Ended;

        public override void OnEnter()
        {
            IsReadyToSwitch = false;
            _acceleratedSpeed = 1f;
            _direction = _positionDirectionProvider.Position2D.normalized;
            Quaternion rotation = Quaternion.Euler(0, 0, 60);
            _direction = rotation * _direction;
        
            Position2D = _positionDirectionProvider.Position2D;
            DepthDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        
            _localTime = 0;
            Started?.Invoke();
        }

        public override void Tick()
        {
            Position2D += _direction * (_speed * Time.deltaTime * (Mathf.PI/2) * _acceleratedSpeed);
            
            DepthDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            
            _acceleratedSpeed *= _acceleration;
            _localTime += Time.deltaTime;
        
            if (_localTime > _duration)
            {
                IsReadyToSwitch = true;
            }
        }
    
        public override void OnExit()
        {
            Ended?.Invoke();
        }
    }
}
