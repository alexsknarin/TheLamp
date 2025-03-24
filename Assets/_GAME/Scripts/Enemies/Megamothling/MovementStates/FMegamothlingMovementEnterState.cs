using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling.MovementStates
{
    public class FMegamothlingMovementEnterState : EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly float _speed;
        private readonly float _radius;
        private readonly float _verticalAmplitude;
    
        private readonly Vector2 _invertX = new Vector2(-1, 1);
        private readonly float _depthMultiplier = 2f;
        private readonly float _depthAdjustDuration = 0.6f;
        private Vector2 _endPos = Vector2.zero;
        private Vector2 _enterDirection;
        private float _initialDistance;
        private float _startDepth;
        private float _depthAdjustLocalTime;
    
        public FMegamothlingMovementEnterState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            float speed,
            float radius,
            float verticalAmplitude
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
            _radius = radius;
            _verticalAmplitude = verticalAmplitude;
        }
    
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
        
            Position2D = _positionDirectionProvider.Position2D;
            if (Position2D.x > 0)
            {
                Position2D *= _invertX;
            }
        
            float xProjectionLength = Mathf.Abs(Position2D.x);
            float enterDirectionLength = Position2D.magnitude;
            float r = _radius * _verticalAmplitude;
        
            float patrolStartOffsetAngle = Mathf.PI - Mathf.Acos(r / enterDirectionLength) - Mathf.Acos(xProjectionLength / enterDirectionLength);
        
            _endPos.x = Mathf.Cos(-patrolStartOffsetAngle);
            _endPos.y = Mathf.Sin(-patrolStartOffsetAngle);
            _endPos = _endPos.normalized * r;

            _enterDirection = (_endPos - Position2D);
            _initialDistance = _enterDirection.magnitude;
            _enterDirection = _enterDirection.normalized;

            _startDepth = 0;
            _depthAdjustLocalTime = 0;
        }

        public override void Tick()
        {
            Position2D += _enterDirection * (_speed * Time.deltaTime * (Mathf.PI/2));
            // Depth To Camera
            float distancePhase = 1 - (_endPos - Position2D).magnitude / _initialDistance;
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        
            float depthAdjustPhase = _depthAdjustLocalTime / _depthAdjustDuration;
            if (depthAdjustPhase > 1)
            {
                depthAdjustPhase = 1;
            }
            else
            {
                _depthAdjustLocalTime += Time.deltaTime;    
            }
            float depth = Mathf.Lerp(Position2D.y * _depthMultiplier, Position2D.y, distancePhase);
            depth = Mathf.Lerp(_startDepth, depth, depthAdjustPhase);
       
            DepthDirection = cameraDirection * depth;
        
            if(Position2D.x > Mathf.Abs(_endPos.x))
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
