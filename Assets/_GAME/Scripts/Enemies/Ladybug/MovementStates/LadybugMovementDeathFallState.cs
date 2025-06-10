using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug.MovementStates
{
    public class LadybugMovementDeathFallState: EnemyMovementStateBase
    {
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly ILampPositionProviderService _lampPositionProviderService;
        private readonly LadybugLampPositionsHolder _lampPositionsHolder;


        private readonly float _depth = 0.3f; // TODO: move to config

        private readonly float _bounceForceMagnitude = 3f;
        private readonly float _gravityForceMagnitude = .17f;
        private readonly float _dragAmount = 0.9f;
        private Vector2 _bounceForce;
        private Vector2 _gravityForce;

        public LadybugMovementDeathFallState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            LadybugLampPositionsHolder lampPositionsHolder,
            float depth
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _lampPositionProviderService = lampPositionProviderService;
            _lampPositionsHolder = lampPositionsHolder;
            _depth = depth;
        }
    
        public event Action Started;
        public event Action Ended;

        public override void Enter()
        {
            Position2D = _positionDirectionProvider.Position2D;
            _bounceForce 
                = (Position2D - _lampPositionProviderService.GetLampPosition()).normalized * _bounceForceMagnitude;
            _gravityForce = Vector2.zero;
            DepthDirection = _positionDirectionProvider.DepthDirection;
            
            _lampPositionsHolder.FreeLandingPosition(Position2D);
            
            Started?.Invoke();
        }

        public override void Tick()
        {
            Position2D += _bounceForce * Time.deltaTime + _gravityForce;
            _bounceForce *= _dragAmount;
            _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            DepthDirection = cameraDirection * _depth;
        
            if (Position2D.y < -4f)
            {
                Ended?.Invoke();
            }
        }
    }
}
