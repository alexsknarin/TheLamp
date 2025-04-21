using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Spider.MovementStates
{
    public class SpiderMovementAttackState: EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly Vector2 _hangingPoint;
        private readonly float _speed;
    
        private readonly float _acceleration = 8.7f;
        private float _acceleratedSpeed;
    
        public SpiderMovementAttackState(
            IPositionDirectionProvider positionDirectionProvider, float speed, float xCenter, float height)
        {
            _positionDirectionProvider = positionDirectionProvider;
            _hangingPoint.x = xCenter;
            _hangingPoint.y = height;
            _speed = speed;
        }
    
        public override void Enter()
        {
            Position2D = _positionDirectionProvider.Position2D;
            _acceleratedSpeed = 1f;
        }

        public override void Tick()
        {
            Vector2 newPosition = Position2D;
            Vector2 direction = -Vector2.right;
            newPosition += direction * (_speed * _acceleratedSpeed * Time.deltaTime);
        
            // Arc swing
            newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint; 
        
            _acceleratedSpeed += _acceleration*Time.deltaTime;
            Position2D = newPosition;
        }
    }
}
