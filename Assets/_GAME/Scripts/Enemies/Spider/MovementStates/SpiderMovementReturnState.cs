using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Spider.MovementStates
{
    public class SpiderMovementReturnState: EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly ILampPositionProviderService _lampPositionProviderService;
        private readonly Vector2 _hangingPoint;
        private readonly float _speed;
        private readonly float _lampCollisionRadius;
        private readonly float _collisionThreshold;
        private readonly float _collisionRadius;

        private readonly float _decceleration = 0.08f;
        private float _localTime;
        private float _initialAmplitude;
        private float _swingAmplitude;
        private float _initialXpos;
        private int _returnPhase;
        private Vector2 _initialDirection;

        public SpiderMovementReturnState(
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            float speed,
            float xCenter,
            float height,
            float lampCollisionRadius,
            float collisionThreshold,
            float collisionRadius
            )
        {
            _positionDirectionProvider = positionDirectionProvider;
            _lampPositionProviderService = lampPositionProviderService;
            _hangingPoint.x = xCenter;
            _hangingPoint.y = height;
            _speed = speed;
            _lampCollisionRadius = lampCollisionRadius;
            _collisionThreshold = collisionThreshold;
            _collisionRadius = collisionRadius;
        }
    
        public override void Enter()
        {
            Position2D 
                = (_positionDirectionProvider.Position2D - _lampPositionProviderService.GetLampPosition()).normalized
                  * (_lampCollisionRadius + _collisionThreshold + _collisionRadius);
            
            _initialAmplitude = Mathf.Abs(Mathf.Abs(Position2D.x) - Mathf.Abs(_hangingPoint.x));
            _swingAmplitude = _initialAmplitude;
            _initialDirection = Position2D.normalized;
            _initialDirection.y = 0;
            _initialXpos = Position2D.x;
            _returnPhase = 0;
            _localTime = 0;
        
            IsReadyToSwitch = false;
        }

        public override void Tick()
        {
            if (_returnPhase == 0)
            {
                Vector2 newPosition = Position2D;
                float phase = 1 - Mathf.Abs(Position2D.x - _initialXpos) / (_initialAmplitude * 2);
                phase = Mathf.Pow(phase, 0.5f);
            
                newPosition += _initialDirection * (_speed * 2 * phase * Time.deltaTime);
                newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint;
                Position2D = newPosition;
            
                if(Mathf.Abs(Position2D.x - _initialXpos) > (_initialAmplitude * 2 - 0.05f))
                {
                    _returnPhase = 1;
                    _localTime = 0;
                    _swingAmplitude = _initialAmplitude - 0.05f;
                }
            }
            if (_returnPhase == 1)
            {
                float swing = Mathf.Cos(_localTime * 1.5f) * _swingAmplitude + _hangingPoint.x;
                Vector2 newPosition = Position2D;
            
                newPosition.x = swing;
                newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint;
                Position2D = newPosition;
            
                _localTime += Time.deltaTime;
                _swingAmplitude -= _decceleration * Time.deltaTime;
            }
        
            if(_swingAmplitude < 0)
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
