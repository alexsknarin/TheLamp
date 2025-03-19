using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class MegabeetleMovementStickAttackState : EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
    
        private float _duration = .27f;
        private float _localTime = 0f;
        private float _phase = 0f;

        private Vector2 _startPosition;
        private Vector2 _endPosition;
    
        public MegabeetleMovementStickAttackState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }
    
        public event Action Ended;
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
            Position2D = _positionDirectionProvider.Position2D;
            _startPosition = Position2D;
            _endPosition = Position2D.normalized * 0.38f; 
            _localTime = 0;
            _phase = 0;
        }

        public override void Tick()
        {
        
            _phase  = _localTime / _duration;
            Position2D = Vector3.Lerp(_startPosition, _endPosition, Mathf.Pow(_phase, 2.6f));
            _localTime += Time.deltaTime;
        
            if (_phase > 1.0f)
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
