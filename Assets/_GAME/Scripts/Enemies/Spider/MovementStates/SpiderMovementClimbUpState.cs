using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Spider.MovementStates
{
    public class SpiderMovementClimbUpState: EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly Vector2 _hangingPoint;
    
        private readonly float _duration = 3f;
        private readonly float _tau = Mathf.PI * 2;
        private float _localTime;
        private float _startY;
        private float _startX;
        // TODO: lower height
    
        public SpiderMovementClimbUpState(IPositionDirectionProvider positionDirectionProvider, float xCenter, float height)
        {
            _positionDirectionProvider = positionDirectionProvider;
            _hangingPoint.x = xCenter;
            _hangingPoint.y = height;
        }
        
        public event Action Ended;
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
            Position2D = _positionDirectionProvider.Position2D;
            _startY = Position2D.y;
            _startX = (Position2D.x - _hangingPoint.x) * 0.75f;
            _localTime = 0;
        }

        public override void Tick()
        {
            float phase = _localTime / _duration;
        
            Vector2 newPosition = Position2D;
            newPosition.y = _startY + Mathf.Sin(phase * _tau) + phase * 3.6f;
            newPosition.x = Mathf.Cos(_localTime * 8) * _startX * Mathf.Pow(1-phase, 2)  + _hangingPoint.x;
        
            Position2D = newPosition;
            _localTime += Time.deltaTime;
        
            if (phase > 1)
            {
                Ended?.Invoke();
                IsReadyToSwitch = true;
            }
        }
    }
}
