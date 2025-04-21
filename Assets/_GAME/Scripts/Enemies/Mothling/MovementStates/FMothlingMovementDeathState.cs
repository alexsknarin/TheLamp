using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Mothling.MovementStates
{
    public class FMothlingMovementDeathState: EnemyMovementStateBase
    {
        private IPositionDirectionProvider _positionDirectionProvider;
    
        // State specific attributes
        private Vector2 _bounceForce;
        private Vector2 _gravityForce;
        private readonly float _bounceForceMagnitude = 3f;
        private readonly float _gravityForceMagnitude = .2f;
        private readonly float _dragAmount = 0.94f;
        private readonly float _speedMultiplier = 0.9f;
        private readonly float _fallBottomYCoordinate = -6f;
    
        public FMothlingMovementDeathState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }

        public event Action Ended;
    
        public override void Enter()
        {
            Position2D = _positionDirectionProvider.Position2D;
            DepthDirection = Vector3.zero;
        
            _bounceForce = Position2D.normalized * _bounceForceMagnitude;
            _gravityForce = Vector2.zero;
            IsReadyToSwitch = false;
        }

        public override void Tick()
        {
            Position2D += _bounceForce * (Time.deltaTime * _speedMultiplier) + _gravityForce;
            _bounceForce *= _dragAmount;
            _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
            if (Position2D.y < _fallBottomYCoordinate)
            {
                Ended?.Invoke();
            }
        }
    }
}
