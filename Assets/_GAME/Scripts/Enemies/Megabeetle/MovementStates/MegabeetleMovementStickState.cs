using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class MegabeetleMovementStickState : EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
    
        private readonly float _duration = 1.25f;
        private readonly float _stickDistance = 0.44f;
        private float _localTime = 0f;
        private float _phase = 0f;
    
        public MegabeetleMovementStickState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }
    
        public override void Enter()
        {
        
            IsReadyToSwitch = false;
            Position2D = _positionDirectionProvider.Position2D.normalized * _stickDistance;
            DepthDirection = _positionDirectionProvider.DepthDirection;
            _localTime = 0;
            _phase = 0;
        }

        public override void Tick()
        {
            _phase  = _localTime / _duration;
            _localTime += Time.deltaTime;
        
            if (_phase > 1)
            {
                IsReadyToSwitch = true;
            }
        }
    }
}
