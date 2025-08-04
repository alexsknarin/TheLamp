using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflySpiderPushStateR", menuName = "FDragonflyMovementStates/FDragonflySpiderPushStateR")]
    public class FDragonflySpiderPushStateR : ScriptableObject, IState, IRight
    {
        [SerializeField] private float _distance = 0.5f;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private AnimationCurve _animCurve;
        private readonly int _sideDirection = -1;
        private float _localTime = 0f;
        private float _phase = 0f;
        private bool _readyToSwitch = false;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _spiderPatrolTransform;
        private DragonflyPatrolRotator _spiderPatrolRotator;
            
        public bool ReadyToSwitch => _readyToSwitch;

        public void SetDependencies(Transform visibleBodyTransform, Transform spiderPatrolTransform, 
            DragonflyPatrolRotator spiderPatrolRotator)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _spiderPatrolTransform = spiderPatrolTransform;
            _spiderPatrolRotator = spiderPatrolRotator;
        }
        
        public event Action Started;
        public event Action Ended;

    
        public void Enter()
        {
            Vector3 currentPosition = _visibleBodyTransform.position;
            _spiderPatrolRotator.SetRotationPhase(currentPosition);
            _spiderPatrolRotator.Play(_sideDirection);
        
            _visibleBodyTransform.SetParent(_spiderPatrolTransform, false);
            _visibleBodyTransform.localPosition = Vector3.zero;
            _visibleBodyTransform.localRotation = Quaternion.identity;
        
            _localTime = 0f;
            _phase = 0f;
            _readyToSwitch = false;
            
            Started?.Invoke();
        }
    
        public void Tick()
        {
            Vector3 position = Vector3.zero;
            position.y = _animCurve.Evaluate(_phase) * _distance;
            _visibleBodyTransform.localPosition = position;
        
            _localTime += Time.deltaTime;
            CheckForStateChange();
        }

        public void Exit()
        {
            _spiderPatrolRotator.Stop();
            Ended?.Invoke();
        }

        private void CheckForStateChange()
        {
            _phase = _localTime / _duration;
            if (_phase > 1)
            {   
                _readyToSwitch = true;
            }
        }
    }
}
