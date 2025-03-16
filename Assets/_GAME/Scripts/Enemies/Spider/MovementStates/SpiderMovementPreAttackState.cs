using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Spider.MovementStates
{
    public class SpiderMovementPreAttackState: EnemyMovementStateBase
    {
        private IPositionDirectionProvider _positionDirectionProvider;
        private Vector2 _hangingPoint;
    
        private float _localTime;
        private readonly float _acceleration = 9.7f;
        private float _acceleratedSpeed;
        private float _lastXPosition;
        private float _swingPhase;
        private float _prevSwingPhase;
    
        public SpiderMovementPreAttackState(
            IPositionDirectionProvider positionDirectionProvider, float xCenter, float height)
        {
            _positionDirectionProvider = positionDirectionProvider;
            _hangingPoint.x = xCenter;
            _hangingPoint.y = height;
        }
        public event Action Started;
        public event Action Ended;
    
        public override void OnEnter()
        {
            Vector2 newPosition = _positionDirectionProvider.Position2D;
            _lastXPosition = newPosition.x;
            newPosition.x = _hangingPoint.x;
            Position2D = newPosition;
            _localTime = 0;
            _acceleratedSpeed = 1f;
            _swingPhase = 0;
            IsReadyToSwitch = false;
            Started?.Invoke();
        }

        public override void Tick()
        {
            _prevSwingPhase = _swingPhase;
            _swingPhase = Mathf.Sin(_localTime * 4.2f);
            float swing =  -_swingPhase * 0.036f * _acceleratedSpeed + _lastXPosition;
        
            Vector2 newPosition = _positionDirectionProvider.Position2D;
            newPosition.x = swing;
        
            // Arc swing
            newPosition = (newPosition - _hangingPoint).normalized * 5f + _hangingPoint; 
        
            Position2D = newPosition;
        
            _localTime += Time.deltaTime;
            _acceleratedSpeed += _acceleration * Time.deltaTime;
        
            if(_swingPhase < 0 && _swingPhase > _prevSwingPhase)
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
