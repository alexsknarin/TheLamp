using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyAttackTailStateR", menuName = "FDragonflyMovementStates/FDragonflyAttackTailStateR")]
    public class FDragonflyAttackTailStateR : ScriptableObject, IState, IRight
    {
        [SerializeField] private float _duration = 0.6f;
        [SerializeField] private float _distance = 0.52f;
        [SerializeField] private AnimationCurve _tzCurve;
        [SerializeField] private AnimationCurve _rxCurve;
        [SerializeField] private AnimationCurve _ryCurve;
        [SerializeField] private AnimationCurve _rzCurve;
        [SerializeField] private float _collisionReadyTime = 0.33f;
        private bool _isCollisionPhaseReached = false;
        private float _localTime = 0f;
        private float _phase = 0f;
        private float _startZPos = 0f;
        private readonly int _sideDirection = -1;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _patrolTransform;
        private DragonflyPatrolRotator _patrolRotator;

        public event Action Started;
        public event Action CollisionPhaseReached;
    
        public void SetDependencies(Transform visibleBodyTransform, Transform patrolTransform, 
            DragonflyPatrolRotator patrolRotator)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _patrolTransform = patrolTransform;
            _patrolRotator = patrolRotator;
        }
    
        public void Enter()
        {
            _isCollisionPhaseReached = false;
            _patrolRotator.SetRotationPhase(_visibleBodyTransform.position + _visibleBodyTransform.right * 0.2f); // Smooth transition
            _patrolRotator.Play(_sideDirection);
        
            _visibleBodyTransform.SetParent(_patrolTransform);
            _visibleBodyTransform.localRotation = Quaternion.identity;
        
            _startZPos = _visibleBodyTransform.localPosition.z;
        
            _localTime = 0f;
            _phase = 0f;
            Started?.Invoke();
        }
    
        public void Tick()
        {
            float zPos = Mathf.Lerp(_startZPos, _distance, _tzCurve.Evaluate(_phase));
            float ry = _ryCurve.Evaluate(_phase) * _sideDirection;
            float rz = _rzCurve.Evaluate(_phase) * _sideDirection;
            Vector3 pos = Vector3.zero;
            pos.z = zPos;
            _visibleBodyTransform.localPosition = pos;
            _visibleBodyTransform.localRotation = Quaternion.Euler(0f, ry, rz);
        
            _localTime += Time.deltaTime;
        
            _phase = _localTime / _duration;
            if (_phase > 1f)
            {
                _phase = 1f;
            }
            if (!_isCollisionPhaseReached && _localTime >= _collisionReadyTime)
            {
                _isCollisionPhaseReached = true;
                CollisionPhaseReached?.Invoke();
            }
        }

        public void Exit() { }
    }
}
