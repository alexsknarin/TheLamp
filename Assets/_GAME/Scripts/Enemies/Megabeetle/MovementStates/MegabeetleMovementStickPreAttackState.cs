using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class MegabeetleMovementStickPreAttackState : EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
    
        private readonly float _duration = 3f;
        private float _localTime = 0f;
        private float _phase = 0f;
        private Vector2 _startPosition;
        private Vector2 _endPosition;
    
        public MegabeetleMovementStickPreAttackState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }
    
        public override void OnEnter()
        {
            IsReadyToSwitch = false;
        
            Position2D = _positionDirectionProvider.Position2D;
        
            _startPosition = Position2D;
            _endPosition = Position2D.normalized * 0.66f;
            _localTime = 0;
            _phase = 0;
        }

        public override void Tick()
        {
            _phase  = _localTime / _duration;
            Position2D = Vector2.Lerp(_startPosition, _endPosition, Mathf.Pow(_phase, 0.45f));
            _localTime += Time.deltaTime;
        
            if (_phase > 1)
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
