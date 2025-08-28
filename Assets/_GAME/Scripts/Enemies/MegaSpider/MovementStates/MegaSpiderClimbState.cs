using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderClimbState : EnemyMovementStateBase
    {
        private const float ClimbDuration = 2.25f;
        
        private float _originalHeight;
        private float _localTime;
        
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private AnimationCurve _climbCurve;

        public MegaSpiderClimbState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,
            AnimationCurve climbCurve
            )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _climbCurve = climbCurve;
        }
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
            
            _visibleBodyTransform.SetParent(null);
            
            _calculatedTransform.position = _visibleBodyTransform.position;
            _visibleBodyTransform.SetParent(_calculatedTransform);
            _visibleBodyTransform.localPosition = Vector3.zero;
            
            _originalHeight = _calculatedTransform.position.y;
            _localTime = 0;
        }

        public override void Tick()
        {
            float phase = _localTime / ClimbDuration;
            if (phase > 1)
            {
                IsReadyToSwitch = true;
            }
            
            Vector3 newPosition = _calculatedTransform.position;
            newPosition.y = _originalHeight + _climbCurve.Evaluate(phase);
            _calculatedTransform.position = newPosition;
            
            _localTime += Time.deltaTime;
        }
    }
}
