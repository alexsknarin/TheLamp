using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class MegabeetleMovementStickPreAttackPauseState : EnemyMovementStateBase
    {
        private readonly IPositionDirectionProvider _positionDirectionProvider;
    
        private readonly float _duration = 0.12f;
        private float _localTime = 0f;
        private float _phase = 0f;
    
        public MegabeetleMovementStickPreAttackPauseState(IPositionDirectionProvider positionDirectionProvider)
        {
            _positionDirectionProvider = positionDirectionProvider;
        }
    
        public event Action Started;
        public event Action Ended;
    
        public override void Enter()
        {
            IsReadyToSwitch = false;
            Position2D = _positionDirectionProvider.Position2D;
            _localTime = 0;
            _phase = 0;
            Started?.Invoke();
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
    
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
