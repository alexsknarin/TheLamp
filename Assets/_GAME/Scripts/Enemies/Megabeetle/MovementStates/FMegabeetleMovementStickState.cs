using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class FMegabeetleMovementStickState : EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
    
        private float _duration = 1.25f;
        private float _localTime = 0f;
        private float _phase = 0f;
    
        public FMegabeetleMovementStickState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }
    
        public override void OnEnter()
        {
        
            IsReadyToSwitch = false;
            Position2D = _positionDirectionProvider.Position2D.normalized * 0.44f; // TODO: set normalized local position, magic number
            // TODO: read depth as well
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
