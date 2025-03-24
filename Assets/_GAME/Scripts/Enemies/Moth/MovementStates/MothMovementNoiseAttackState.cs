using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth.MovementStates
{
    public class MothMovementNoiseAttackState : EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly ILampPositionProviderService _lampPositionProvider;
        private readonly float _speed;
    
        // State specific attributes
        private readonly float _acceleration = 0.02f;
        private readonly float _depthDecrement = 0.2f;
        private readonly float _noiseFrequency = 13f;
        private readonly float _noiseAmplitude = 0.08f;
        private float _acceleratedSpeed = 1f;
        private float _maxDistance = 0.5f;
    
        public MothMovementNoiseAttackState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProvider,
            float speed
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _lampPositionProvider = lampPositionProvider;
            _speed = speed;
        }
    
        public override void OnEnter()
        {
            Position2D = _positionDirectionProvider.Position2D;
            _maxDistance = Position2D.magnitude;
            _acceleratedSpeed = 1f;
        }

        public override void Tick()
        {
            Vector2 newPosition = Position2D;
            Vector2 lampVector = _lampPositionProvider.GetLampPosition() - Position2D;
            Vector2 direction = lampVector.normalized;
            newPosition += direction * (_speed * _acceleratedSpeed * Time.deltaTime);
            _acceleratedSpeed += _acceleration;

            // Add noise
            Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
            float noiseAttenuation = Mathf.Clamp((lampVector.magnitude - 0.65f) / (_maxDistance - 0.65f) * 1.5f - 0.5f , 0, 1); 
            Position2D = newPosition + trajectoryNoise * (_noiseAmplitude * noiseAttenuation);
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            DepthDirection = cameraDirection * (0.2f * _depthDecrement);
        }
    }
}
