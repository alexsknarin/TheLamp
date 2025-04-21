using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Mothling.MovementStates
{
    public class FMothlingMovementPreAttackState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;

        // State specific attributes
        private readonly float _duration = .35f;
        private Vector2 _direction;
        private float _localTime;

        public FMothlingMovementPreAttackState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
        }

        public event Action Started;
        public event Action Ended;

        public override void Enter()
        {
            IsReadyToSwitch = false;
        
            Position2D = _positionDirectionProvider.Position2D;
            DepthDirection = (_cameraPosition - (Vector3)Position2D).normalized;
        
            _localTime = 0;
            Started?.Invoke();
        }

        public override void Tick()
        {
            DepthDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            _localTime += Time.deltaTime;
        
            if (_localTime > _duration)
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
