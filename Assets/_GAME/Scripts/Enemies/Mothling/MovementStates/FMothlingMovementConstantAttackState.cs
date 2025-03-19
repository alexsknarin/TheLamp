using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Mothling.MovementStates
{
    public class FMothlingMovementConstantAttackState: EnemyMovementStateBase
    {
        // Dependencies
        private readonly Vector3 _cameraPosition;
        private readonly IPositionDirectionProvider _positionDirectionProvider;
        private ILampPositionProviderService _lampPositionProviderService;
        private readonly float _speed;

        // State specific attributes
        private readonly float _depthDecrement = 0.42f;
        private float _startDistance;
        private readonly float _speedMultiplier =  1.1f;

        public FMothlingMovementConstantAttackState(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            float speed
        )
        {
            _cameraPosition = cameraPosition;
            _positionDirectionProvider = positionDirectionProvider;
            _lampPositionProviderService = lampPositionProviderService;
            _speed = speed * _speedMultiplier;
        }
    
        public override void OnEnter()
        {
            _startDistance = _positionDirectionProvider.Position2D.magnitude - 0.65f;
            Position2D = _positionDirectionProvider.Position2D;
        }

        public override void Tick()
        {
            Vector2 direction = -(Position2D - _lampPositionProviderService.GetLampPosition()).normalized;
            Position2D += direction * (_speed * Time.deltaTime);
        
            Vector3 cameraDirection = (_cameraPosition - (Vector3)Position2D).normalized;
            float attackProximityGradient = Mathf.Clamp((Position2D.magnitude - 0.72f) / _startDistance, 0.0f, 1.0f);
            attackProximityGradient = Mathf.Pow(attackProximityGradient, 1.9f) + 0.1f;
            DepthDirection = cameraDirection * (1.1f * _depthDecrement * attackProximityGradient);
        }
    }
}
