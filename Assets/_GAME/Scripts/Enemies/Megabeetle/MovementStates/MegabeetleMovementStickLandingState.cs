using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class MegabeetleMovementStickLandingState : EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;

        private readonly float _startDistance = 0.64f;
        private readonly float _endDistance = 0.44f;
        private float _duration = .491f;
        private float _phase;
        private float _localTime;
        private Vector2 _startPosition;
        private Vector2 _endPosition;
    
        public MegabeetleMovementStickLandingState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
        
            Position2D = _positionDirectionProvider.Position2D;
            _localTime = 0;
            _phase = 0;
            _startPosition = Position2D.normalized * _startDistance;
            _endPosition = Position2D.normalized * _endDistance;
        
        }

        public override void Tick()
        {
            _phase  = _localTime / _duration;
            Position2D = Vector2.Lerp(_startPosition, _endPosition, _phase);
            _localTime += Time.deltaTime;
        
            if (_phase > 1)
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
