using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug.MovementStates
{
    public abstract class FLadybugMovementPatrolState: EnemyMovementStateBase
    {
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly ILampPositionProviderService _lampPositionProviderService;
        private readonly LadybugLampPositionsHolder _lampPositionsHolder;
        private readonly float _speed;
        private readonly float _radius;
        private readonly float _verticalAmplitude;
    
        private readonly float _attackRange = 1.5f;
        private bool _outsideAttackRange;
        private float _patrolStartOffsetAngle;
        private float _enterTimeOffset;
        private float _phase;
        private readonly float _spiralSpeedStart = 0.2f;
        private readonly float _spiralSpeedEnd = 0.015f;
        private float _spiralPhase = 1f;
        private readonly float _preAttackTriggerDistance = 0.71f;
        private readonly float _preAttackTriggerYThreshold = 0.3f;
        private readonly float _depthMultiplierMax = 3f;
        private readonly float _depthMultiplierMin = 0f;
        private Vector2 _prevPosition2D;
        private Vector2 _velocity;
        private bool _isEnteredPreAttackRange;
        private Vector2 _closestLandingWorldPosition;  
        

        public event Action EnteredAttackRange;
    
        public FLadybugMovementPatrolState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            LadybugLampPositionsHolder lampPositionsHolder,
            float speed,
            float radius,
            float verticalAmplitude
        )
        { 
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _lampPositionProviderService = lampPositionProviderService;
            _lampPositionsHolder = lampPositionsHolder;
            _speed = speed;
            _radius = radius;
            _verticalAmplitude = verticalAmplitude;
        }
    
        protected void HandleEnter(int sideDirection)
        {
            SetDefaultValues();
            FindPatrolStartOffsetAngle(sideDirection);
            
            if (sideDirection < 0)
            {
                _patrolStartOffsetAngle = Mathf.PI - _patrolStartOffsetAngle;
            }
            _outsideAttackRange = true;
            IsReadyToSwitch = false;
        }

        protected void HandleTick(int sideDirection)
        {
            _prevPosition2D = Position2D;
            float speedCompenstation = (1 - Position2D.magnitude/_radius) + 1;
            Vector2 lampPos = _lampPositionProviderService.GetLampPosition();

            _phase += Time.deltaTime * _speed * speedCompenstation * sideDirection;

            Vector2 ellipsePosition = FindEllipsePosition();
            Vector2 circlePosition = FindCirclePosition(ellipsePosition);
            UpdateSpiralPhase(ellipsePosition, circlePosition);

            Position2D = ellipsePosition + lampPos;
            Vector2 localPositionVector = (Position2D - lampPos);
            Vector2 directionFromLamp = localPositionVector.normalized;
            float distanceToLamp = localPositionVector.magnitude;
            
            if(!_isEnteredPreAttackRange && distanceToLamp < _preAttackTriggerDistance)
            {
                _isEnteredPreAttackRange = true;
            }
            
            if (_isEnteredPreAttackRange)
            {
                Position2D = lampPos + directionFromLamp * _preAttackTriggerDistance;
                
                var proximityToLandingPoint = GetProximityToLandingPoint();

                if (proximityToLandingPoint < 0.19f && _lampPositionsHolder.CheckIfClosestLandingPositionIsFree())
                {
                    IsReadyToSwitch = true;
                    _lampPositionsHolder.OccupyClosestLandingPosition();
                }
            }
            
            if (_outsideAttackRange && distanceToLamp < _attackRange)
            {
                _outsideAttackRange = false;
                EnteredAttackRange?.Invoke();
            }
            
            DepthDirection = CalculateCameraDirection(circlePosition);
        }

        private float GetProximityToLandingPoint()
        {
            _velocity = (Position2D - _prevPosition2D).normalized;
            _closestLandingWorldPosition = _lampPositionsHolder.GetClosestLandingPosition(Position2D);
            Vector2 landingPointDirection = (_closestLandingWorldPosition - Position2D).normalized;
            float proximityToLandingPoint = Vector2.Dot(_velocity, landingPointDirection);
            return proximityToLandingPoint;
        }

        private void SetDefaultValues()
        {
            _phase = 0;
            _spiralPhase = 1f;
            _isEnteredPreAttackRange = false;
            Position2D = _positionDirectionProvider.Position2D;
            _prevPosition2D = Position2D;
        }

        private void FindPatrolStartOffsetAngle(int sideDirection)
        {
            Vector2 horizontalVector = Vector2.right;
            horizontalVector.x *= sideDirection;
            _patrolStartOffsetAngle 
                = Mathf.Acos(Vector2.Dot(horizontalVector.normalized, Position2D.normalized));
            _patrolStartOffsetAngle *= Mathf.Sign(Position2D.y);
        }

        private void UpdateSpiralPhase(Vector2 ellipsePosition, Vector2 circlePosition)
        {
            if (ellipsePosition.magnitude > _preAttackTriggerDistance)
            {
                _spiralPhase -= Mathf.Lerp(_spiralSpeedStart, _spiralSpeedEnd, 1 - (circlePosition.magnitude/_radius)) 
                                * Time.deltaTime;
            }
        }

        private Vector2 FindCirclePosition(Vector2 ellipsePosition)
        {
            Vector2 circlePosition = ellipsePosition;
            circlePosition.y /= _verticalAmplitude;
            return circlePosition;
        }

        private Vector2 FindEllipsePosition()
        {
            Vector2 ellipsePosition = EnemyMovementPatterns.CircleMotion(
                _patrolStartOffsetAngle, 
                _radius,
                _radius,
                _verticalAmplitude, 
                _phase
                );

            ellipsePosition *= _spiralPhase;
            return ellipsePosition;
        }

        private Vector3 CalculateCameraDirection(Vector2 circlePosition)
        {
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            float depthPhase = Mathf.Clamp(circlePosition.magnitude - _preAttackTriggerDistance, 0.0001f, _radius) 
                               / (_radius - _preAttackTriggerDistance);
            depthPhase = Mathf.Pow(depthPhase, 0.85f);
            depthPhase = Mathf.Clamp(depthPhase, 0.0001f, 1f);
            float depthValue = Mathf.Lerp(_depthMultiplierMin, _depthMultiplierMax, depthPhase);
            return cameraDirection * depthValue;
        }
    }
}
