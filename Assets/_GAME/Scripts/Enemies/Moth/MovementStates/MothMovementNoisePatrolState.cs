using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth.MovementStates
{
    public class MothMovementNoisePatrolState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly float _speed;
        private readonly float _radius;
        private readonly float _verticalAmplitude;
    
        // State specific attributes
        private float _patrolStartOffsetAngle;
        private float _enterTimeOffset; // TMP
        private float _phase;
        private float _mainTrajectoryAdaptTime = 0.45f;
        private float _patrolDuration;
        private float _patrolDurationMin = 0.5f;
        private float _patrolDurationMax = 1.2f;
        private float _noiseFrequency = 9f;
        private float _noiseAmplitude = 0.05f;
        private float _localTime;
    
        public MothMovementNoisePatrolState(
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
            _phase = 0;
            _patrolDuration = Random.Range(_patrolDurationMin, _patrolDurationMax);
            Position2D = _positionDirectionProvider.Position2D;
            Vector3 horizontalVector = Vector2.right;
            _localTime = 0;
        
            _patrolStartOffsetAngle = Mathf.Acos(Vector3.Dot(horizontalVector.normalized, Position2D.normalized));
            _patrolStartOffsetAngle *= Mathf.Sign(Position2D.y);
        }

        public override void Tick()
        {
            float trajectoryAdaptPhase = _localTime / _mainTrajectoryAdaptTime;
            _phase += Time.deltaTime * _speed;

            // Circle motion
            Vector2 circlePosition = EnemyMovementPatterns.CircleMotion(_patrolStartOffsetAngle, _radius, _radius, _verticalAmplitude, _phase);
            if (trajectoryAdaptPhase < 1)
            {
                circlePosition = Vector2.Lerp(Position2D, circlePosition, Mathf.SmoothStep(0, 1, trajectoryAdaptPhase));
            }
        
            // Add noise
            Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
            Position2D = circlePosition + trajectoryNoise * _noiseAmplitude;
        
            _localTime += Time.deltaTime;
        
            if(_localTime > _patrolDuration)
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
