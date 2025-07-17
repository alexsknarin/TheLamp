using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public abstract class FMegabeetleMovementPatrolState : EnemyMovementStateBase
    {
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly float _speed;
        private readonly float _radius;
        private readonly float _verticalAmplitude;
    
        private readonly float _attackRange = 1.5f;
        private readonly float _spiralSpeedStart = 0.2f;
        private readonly float _spiralSpeedEnd = 0.015f;
        private readonly float _preAttackTriggerDistance = 1.0f;
        private readonly float _preAttackTriggerYThreshold = 0.3f;
        private readonly float _depthMultiplierMax = 1.8f;
        private readonly float _depthMultiplierMin = 0f;
        private bool _outsideAttackRange;
        private float _patrolStartOffsetAngle;
        private float _enterTimeOffset;
        private float _phase;
        private float _spiralPhase = 1f;

        public FMegabeetleMovementPatrolState(
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
    
        public event Action EnteredAttackRange;
        public event Action Started;
         
        protected void HandleEnter(int sideDirection)
        {
            IsReadyToSwitch = false;
            _outsideAttackRange = true;

            Position2D = _positionDirectionProvider.Position2D;
            _phase = 0;
            _spiralPhase = 1f;
        
            Vector2 horizontalVector = Vector2.right;
            horizontalVector.x *= sideDirection;
            _patrolStartOffsetAngle = Mathf.Acos(Vector2.Dot(horizontalVector.normalized, Position2D.normalized));
            _patrolStartOffsetAngle *= Mathf.Sign(Position2D.y);

            Started?.Invoke();
        }
    
        protected void HandleTick(int sideDirection)
        {
            float speedCompenstation = (1 - Position2D.magnitude/_radius) + 1;
       
            _phase += Time.deltaTime * _speed * speedCompenstation * sideDirection;
            Vector3 ellipsePosition = EnemyMovementPatterns.CircleMotion(_patrolStartOffsetAngle, _radius, _radius, _verticalAmplitude, _phase);
            ellipsePosition *= _spiralPhase;

            Vector3 circlePosition = ellipsePosition;
            circlePosition.y /= _verticalAmplitude;
        
            if (ellipsePosition.magnitude > _preAttackTriggerDistance)
            {
                _spiralPhase -= Mathf.Lerp(_spiralSpeedStart, _spiralSpeedEnd, 1 - (circlePosition.magnitude/_radius)) * Time.deltaTime;
            }

            Position2D = ellipsePosition;
        
            // Depth To Camera
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            float depthPhase = Mathf.Clamp(circlePosition.magnitude - _preAttackTriggerDistance, 0.0001f, _radius) / (_radius - _preAttackTriggerDistance);
            depthPhase = Mathf.Pow(depthPhase, 0.85f);
            depthPhase = Mathf.Clamp(depthPhase, 0.0001f, 1f);
            float depthValue = Mathf.Lerp(_depthMultiplierMin, _depthMultiplierMax, depthPhase);
            DepthDirection = cameraDirection * depthValue;
        
            float distanceToLamp = Position2D.magnitude;
        
            if (_outsideAttackRange && distanceToLamp < _attackRange)
            {
                _outsideAttackRange = false;
                EnteredAttackRange?.Invoke();
            }
        
            if(distanceToLamp < _preAttackTriggerDistance && Position2D.y < _preAttackTriggerYThreshold)
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
