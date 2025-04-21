using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class MegabeetleMovementFallState : EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;

        private readonly float _bounceForceMagnitude = 3f;
        private readonly float _gravityForceMagnitude = .17f;
        private readonly float _dragAmount = 0.9f;
        private Vector2 _bounceForce;
        private Vector2 _gravityForce;

        public MegabeetleMovementFallState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }

        public event Action Ended;
    
        public override void Enter()
        {
            IsReadyToSwitch = false;
            Position2D = _positionDirectionProvider.Position2D;
            _bounceForce = Position2D.normalized * _bounceForceMagnitude;
            _gravityForce = Vector3.zero;
        }

        public override void Tick()
        {
            Position2D += _bounceForce * Time.deltaTime + _gravityForce;
            _bounceForce *= _dragAmount;
            _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
            if (Position2D.y < -7.3f)
            {
                Ended?.Invoke();
                IsReadyToSwitch = true;
            }
        }
    }
}
