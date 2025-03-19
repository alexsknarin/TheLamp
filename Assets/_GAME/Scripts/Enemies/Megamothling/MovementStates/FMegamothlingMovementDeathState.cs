using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling.MovementStates
{
    public class FMegamothlingMovementDeathState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly IPositionDirectionProvider _positionDirectionProvider;
    
        // State specific attributes
        private readonly float _bounceForceMagnitude = 2f;
        private readonly float _gravityForceMagnitude = .2f;
        private readonly float _duration = 1.7f;
        private float _dragAmount = 0.94f;
        private Vector2 _bounceForce;
        private Vector2 _gravityForce;
        private float _localTime;

        public FMegamothlingMovementDeathState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }
    
        public event Action Ended;
    
        public override void OnEnter()
        {        
            Position2D = _positionDirectionProvider.Position2D;
            DepthDirection = Vector3.zero;
        
            _bounceForce = Position2D.normalized * _bounceForceMagnitude;
            _gravityForce = Vector2.zero;
            _dragAmount = 0.94f;
            _localTime = 0;
        }

        public override void Tick()
        {
            Position2D += _bounceForce * Time.deltaTime + _gravityForce;
            _bounceForce *=_dragAmount;
        
            _dragAmount -= 0.12f * Time.deltaTime;
            if (_dragAmount < 0.0f)
            {
                _dragAmount = 0.0001f;
            }
        
            _localTime += Time.deltaTime;
            _gravityForce = _gravityForce * _dragAmount + Vector2.down * (_gravityForceMagnitude * _dragAmount * Time.deltaTime);
        
            if (_localTime > _duration)
            {
                Ended?.Invoke();
            }
        }
    }
}
