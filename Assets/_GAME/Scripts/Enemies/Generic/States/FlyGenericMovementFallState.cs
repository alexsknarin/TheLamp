using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Generic.States
{
    public class FlyGenericMovementFallState: EnemyMovementStateBase
    {
        private const float LampCollisionRadius = 0.49f;
        private const float CollisionThreshold = 0.0001f;
    
        private IPositionDirectionProvider _positionDirectionProvider;
        private ILampPositionProviderService _lampPositionProviderService;
        private readonly float _radius;
        private readonly float _verticalAmplitude;
        private readonly float _collisionRadius;
    
        // State specific attributes
        private readonly float _fullLampCollisionRadius = LampCollisionRadius + CollisionThreshold;
        private Vector2 _bounceForce;
        private Vector2 _gravityForce;
        private readonly float _bounceForceMagnitude;
        private readonly float _gravityForceMagnitude;
        private readonly float _dragAmount = 0.94f;
        private readonly float _ySwitchDistance;
    
        public FlyGenericMovementFallState(
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            float radius,
            float verticalAmplitude,
            float collisionRadius,
            float bounceForceMagnitude,
            float gravityForceMagnitude
        )
        {
            _positionDirectionProvider = positionDirectionProvider;
            _lampPositionProviderService = lampPositionProviderService;
            _radius = radius;
            _verticalAmplitude = verticalAmplitude;
            _collisionRadius = collisionRadius;
        
            _bounceForceMagnitude = bounceForceMagnitude;
            _gravityForceMagnitude = gravityForceMagnitude;
        
            _ySwitchDistance = -_radius * _verticalAmplitude * 1.1f;
        }
    
        public override void OnEnter()
        {
            Position2D = _positionDirectionProvider.Position2D;
        
            Vector2 position2DNormalized = (Position2D - _lampPositionProviderService.GetLampPosition()).normalized;
            Position2D = position2DNormalized 
                         * (_fullLampCollisionRadius + _collisionRadius) 
                         + _lampPositionProviderService.GetLampPosition();
                     
            DepthDirection = Vector3.zero;
        
            _bounceForce = position2DNormalized * _bounceForceMagnitude;
        
            Debug.DrawRay(
                _lampPositionProviderService.GetLampPosition(), 
                position2DNormalized, 
                Color.red,
                5f
                );
        
            _gravityForce = Vector2.zero;
            IsReadyToSwitch = false;
        }

        public override void Tick()
        {
            Position2D += _bounceForce * Time.deltaTime + _gravityForce;
            _bounceForce *= _dragAmount;
            _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
   
            if (Position2D.y < _ySwitchDistance)
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
