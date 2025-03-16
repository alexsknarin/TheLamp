using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Generic.States
{
    public class FlyGenericMovementSpreadState: EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly float _speed;
    
        // State specific attributes
        private readonly float _maxDistance = 6.4f;
        private readonly float _acceleration = 5.5f;
        private float _acceleratedSpeed;
        private Vector2 _direction;
        private float _extraDistance;
    
        public FlyGenericMovementSpreadState(IPositionDirectionProvider positionDirectionProvider, float speed)
        {
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
        }
    
        public event Action Ended; 
    
        public override void OnEnter()
        {
            Position2D = _positionDirectionProvider.Position2D;
            DepthDirection = _positionDirectionProvider.DepthDirection;
        
            _direction = Position2D.normalized;
            _acceleratedSpeed = 1;
            _extraDistance = Random.Range(50f, 70f);
        }

        public override void Tick()
        {
            Position2D += _direction * (_speed * _acceleratedSpeed * Time.deltaTime);
            _acceleratedSpeed += _acceleration * Time.deltaTime;
        
            if(Position2D.magnitude > _maxDistance + _extraDistance)
            {
                Ended?.Invoke();
            }
        }
    }
}
