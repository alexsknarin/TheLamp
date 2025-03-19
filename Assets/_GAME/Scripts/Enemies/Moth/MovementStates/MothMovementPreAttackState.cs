using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth.MovementStates
{
    public class MothMovementPreAttackState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly float _speed;

        private readonly float _duration = .35f;
        private readonly float _acceleration = 0.93f;
        private float _acceleratedSpeed;
        private Vector2 _direction;
        private float _localTime;
    
        public MothMovementPreAttackState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            float speed
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
        }
    
        public event Action Started;
        public event Action Ended;
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
            _acceleratedSpeed = 1f;
            Position2D = _positionDirectionProvider.Position2D;
            _direction = Position2D.normalized;
            _localTime = 0;
            Started?.Invoke();
        }

        public override void Tick()
        {
            Position2D += _direction * (_speed * Time.deltaTime * (Mathf.PI/2) * _acceleratedSpeed);
        
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            DepthDirection = cameraDirection * 2.5f;
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
