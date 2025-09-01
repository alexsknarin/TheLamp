using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaspiderProjectileSpider.MovementStates
{
    public class MegaspiderProjectileSpiderPreAttackState : EnemyMovementStateBase
    {
        private const float Duration = 0.25f;
        private const float Distance = 0.02f;
        
        private Vector3 _startLocalPosition;
        private Vector3 _endLocalPosition;
        private float _localTime;
        
        // Dependencies
        private Transform _bodyTransform;

        public MegaspiderProjectileSpiderPreAttackState(Transform bodyTransform)
        {
            _bodyTransform = bodyTransform;
        }
        
        public override void Enter()
        {
            _startLocalPosition = _bodyTransform.localPosition;
            _endLocalPosition = _startLocalPosition + Vector3.down * Distance;
            IsReadyToSwitch = false;
            _localTime = 0;
        }

        public override void Tick()
        {
            float phase = _localTime / Duration;
            
            if (phase > 1)
            {
                IsReadyToSwitch = true;
                return;
            }

            _bodyTransform.localPosition = Vector3.Lerp(_startLocalPosition, _endLocalPosition, phase);
            _localTime += Time.deltaTime;
        }
    }
}
