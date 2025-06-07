using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug.MovementStates
{
    public class LadybugMovementAttackState : EnemyMovementStateBase
    {
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private readonly ILampPositionProviderService _lampPositionProviderService;
        private readonly float _speed;
    
        private readonly float _depth = 0.3f;
        private readonly float _localSpeedMultiplier = 0.86f;
        private readonly float _localSpeed;

        public LadybugMovementAttackState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            float speed
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _lampPositionProviderService = lampPositionProviderService;
            _speed = speed;
            _localSpeed = _speed * _localSpeedMultiplier;
        }

        public event Action Started;
        public event Action Ended;
    
        public override void Enter()
        {
            Started?.Invoke();
        }
    
        public override void Tick()
        {
            Vector2 newPosition = _positionDirectionProvider.Position2D;
            Vector2 direction = (_lampPositionProviderService.GetLampPosition() - Position2D).normalized;
            newPosition += direction * (_localSpeed * Time.deltaTime);
            Position2D = newPosition;
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            DepthDirection = cameraDirection * _depth;
        }
        
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
