using System;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth.MovementStates
{
    public class MothMovementNoiseFallState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private ILampPositionProviderService _lampPositionProviderService;
        private readonly float _radius;
        private readonly float _verticalAmplitude;
        
        private float _lampCollisionRadius;
        private float _collisionThreshold;
        private float _collisionRadius;
        
        // State specific attributes
        private readonly float _bounceForceMagnitude = 2.6f;
        private readonly float _gravityForceMagnitude = .17f;
        private readonly float _dragAmount = 0.94f;
        private readonly float _noiseFrequency = 7f;
        private readonly float _noiseAmplitude = 0.015f;
        private readonly float _duration = 1.5f;
        
        private Vector2 _bounceForce;
        private Vector2 _gravityForce;
        private float _localTime;
        private readonly float _yPositionToSwitch;
    
        public MothMovementNoiseFallState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            float radius,
            float verticalAmplitude,
            float lampCollisionRadius,
            float collisionThreshold,
            float collisionRadius
        )
        {
            _lampCollisionRadius = lampCollisionRadius;
            _collisionThreshold = collisionThreshold;
            _collisionRadius = collisionRadius;
            
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _lampPositionProviderService = lampPositionProviderService;
            _radius = radius;
            _verticalAmplitude = verticalAmplitude;
        
            _yPositionToSwitch = -_radius * _verticalAmplitude * 1.1f;
        }

        public event Action Started;
        public event Action Ended;
    
        public override void Enter()
        {
            IsReadyToSwitch = false;
            Position2D = _positionDirectionProvider.Position2D;
        
            Vector2 position2DNormalized = (Position2D - _lampPositionProviderService.GetLampPosition()).normalized;
            Position2D = position2DNormalized 
                * (_lampCollisionRadius + _collisionRadius + _collisionThreshold) 
                + _lampPositionProviderService.GetLampPosition();
        
            _bounceForce = position2DNormalized * _bounceForceMagnitude;
        
            Debug.DrawRay(
                _lampPositionProviderService.GetLampPosition(),
                position2DNormalized, 
                Color.red,
                5f
                );
        
            _gravityForce = Vector3.zero;
            _localTime = 0;
            
            Started?.Invoke();
        }

        public override void Tick()
        {
            Position2D += _bounceForce * Time.deltaTime + _gravityForce;
        
            // Add noise
            float noisePhase = _localTime / _duration;
            Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency) * noisePhase;
            Position2D += trajectoryNoise * _noiseAmplitude;
        
            _bounceForce *= _dragAmount;
            _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
            _localTime += Time.deltaTime;
        
            if (Position2D.y < _yPositionToSwitch)
            {
                IsReadyToSwitch = true;
                Ended?.Invoke();
            }
        }
    }
}
