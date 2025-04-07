using System;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Moth.MovementStates
{
    public class MothMovementHoverState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly float _speed;
        private readonly float _radius;
    
        // State specific attributes
        private readonly float _depthMultiplier = 0.6f;
        private readonly float _hoverRadius = .35f;
        private readonly float _moveFromCenterDuration = 0.2f;
        private readonly float _speedNoiseCompensation = 0.4f;
        private readonly float _hoverDurationMin = .5f;
        private readonly float _hoverDurationMax = 2f;
        private readonly float _noiseFrequency = 8f;
        private readonly float _noiseAmplitude = 0.29f;
        private float _patrolStartOffsetAngle;
        private float _enterTimeOffset; // TMP
        private float _phase;
        private Vector2 _hoverCenter;
        private float _speedFactor;
        private float _hoverDuration;
        private float _hoverPhase;
        private float _localTime;
    
        public MothMovementHoverState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            float speed,
            float radius
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
            _radius = radius;
        }
    
        public event Action Started;
        public event Action Ended;
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
            _hoverDuration = Random.Range(_hoverDurationMin, _hoverDurationMax);
            _phase = 0;
            Position2D = _positionDirectionProvider.Position2D;
            _hoverCenter = Position2D;
            _speedFactor = _radius / _hoverRadius;
            _localTime = 0;
            Started?.Invoke();
        }

        public override void Tick()
        {
            float radiusAdaptPhase = _localTime / _moveFromCenterDuration;
            float speedNoiseCompensation = Mathf.Lerp(1, _speedNoiseCompensation, radiusAdaptPhase);
            _phase += Time.deltaTime * _speed * _speedFactor * speedNoiseCompensation;
            _hoverPhase = _localTime / _hoverDuration;

            Vector2 circlePosition 
                = _hoverCenter 
                  + EnemyMovementPatterns.CircleMotion(0, _hoverRadius, _hoverRadius, 1, _phase);
        
            if (radiusAdaptPhase < 1f)
            {
                circlePosition 
                    = Vector3.Lerp(_hoverCenter, circlePosition, Mathf.SmoothStep(0, 1, radiusAdaptPhase));
            }
        
            // Add noise
            Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
            Position2D 
                = circlePosition + trajectoryNoise * (Mathf.Clamp(radiusAdaptPhase, 0, 1) * _noiseAmplitude);
        
            // Depth To Camera
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            DepthDirection = cameraDirection * _depthMultiplier;
        
            _localTime += Time.deltaTime;
        
            if(_hoverPhase > 1f)
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
