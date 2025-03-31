using System;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth.MovementStates
{
    public class MothMovementNoiseDeathState: EnemyMovementStateBase
    {
        // Dependencies
        private IPositionDirectionProvider _positionDirectionProvider;
    
        // State specific attributes
        private readonly float _bounceForceMagnitude = 2.6f;
        private readonly float _gravityForceMagnitude = .17f;
        private readonly float _dragAmount = 0.94f;
        private readonly float _noiseFrequency = 7f;
        private readonly float _noiseAmplitude = 0.035f;
        private Vector2 _bounceForce;
        private Vector2 _gravityForce;

        public MothMovementNoiseDeathState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }

        public event Action Ended;
    
        public override void OnEnter()
        {
            Position2D = _positionDirectionProvider.Position2D;
            DepthDirection = Vector3.zero;
            _bounceForce = Position2D.normalized * _bounceForceMagnitude;
            _gravityForce = Vector3.zero;
        }

        public override void Tick()
        {
            Position2D += _bounceForce * Time.deltaTime + _gravityForce;
            // Add noise
            Vector2 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
            Position2D += trajectoryNoise * _noiseAmplitude;
            _bounceForce *= _dragAmount;
            _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
            if (Position2D.y < -4f)
            {
                Ended?.Invoke();
            }
        }
    }
}
