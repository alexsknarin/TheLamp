using System;
using _GAME.Scripts.Lib;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderClimbState : EnemyMovementStateBase
    {
        private const float ClimbDuration = 2.25f;
        
        private float _originalHeight;
        private float _localTime;
        
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private Transform _rootTransform;
        private AnimationCurve _climbCurve;

        public MegaspiderClimbState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,
            Transform rootTransform,
            AnimationCurve climbCurve
            )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _rootTransform = rootTransform;
            _climbCurve = climbCurve;
        }
        
        public event Action Ended;
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
            
            _visibleBodyTransform.SetParent(_rootTransform);
            _calculatedTransform.position = _visibleBodyTransform.position;
            HierarchyUtilities.ParentWithoutOffset(_visibleBodyTransform, _calculatedTransform);
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
        
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
