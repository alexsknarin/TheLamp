using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling.MovementStates
{
    public class FMegamothlingMovementAttackState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly float _speed;
    
        // State specific attributes
        private readonly float _acceleration = 3.1f;
        private readonly float _depthDecrement = 0.2f;
        private float _acceleratedSpeed = 1f;
        private float _startDistance;
    
        public FMegamothlingMovementAttackState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            float speed
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
        }
    
        public override void OnEnter()
        {
            _acceleratedSpeed = 1f;
            _startDistance = _positionDirectionProvider.Position2D.magnitude - 0.65f; // TODO: Magic number
        }

        public override void Tick()
        {
            Vector2 newPosition = _positionDirectionProvider.Position2D;
            Vector2 direction = -newPosition.normalized;
            newPosition += direction * (_speed * _acceleratedSpeed * Time.deltaTime);
            _acceleratedSpeed += _acceleration * Time.deltaTime;
            Position2D = newPosition;
        
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            float attackProximityGradient = Mathf.Clamp((Position2D.magnitude - 0.65f) / _startDistance, 0.5f, 1.0f);
            DepthDirection = cameraDirection * (2.5f * _depthDecrement * attackProximityGradient);
        }
    }
}
