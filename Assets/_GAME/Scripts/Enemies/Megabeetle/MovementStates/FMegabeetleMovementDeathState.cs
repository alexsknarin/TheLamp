using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class FMegabeetleMovementDeathState : EnemyMovementStateBase
    {
        // Dependencies
        private readonly IPositionDirectionProvider _positionDirectionProvider;
    
        private readonly Vector2 IDLE_POSITION = new Vector2(0f, -4.5f);
        private Vector2 _bounceForce;
        private Vector2 _gravityForce;
        private float _duration = 1.7f;
        private float _localTime = 0f;
        private float _phase = 0f;
        private float _bounceForceMagnitude = 3f;
        private float _gravityForceMagnitude = .17f;
        private float _dragAmount = 0.9f;
    
        public FMegabeetleMovementDeathState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }

        public event Action Ended;
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
            Position2D = _positionDirectionProvider.Position2D;
            _bounceForce = Position2D.normalized * _bounceForceMagnitude;
            _gravityForce = Vector2.zero;
            _localTime = 0f;
            _phase = 0f;
        }

        public override void Tick()
        {
            _phase = _localTime / _duration;
            Position2D += (_bounceForce * Time.deltaTime + _gravityForce) * Mathf.Pow(Mathf.Clamp01(1 - _phase), 2f);
            _bounceForce *= _dragAmount;
            _gravityForce += Vector2.down * (_gravityForceMagnitude * Time.deltaTime);
        
            _localTime += Time.deltaTime;
        
            if (_phase > 1f)
            {
                Position2D = IDLE_POSITION;
                IsReadyToSwitch = true;
                Ended?.Invoke();
            }
        }
    }
}
