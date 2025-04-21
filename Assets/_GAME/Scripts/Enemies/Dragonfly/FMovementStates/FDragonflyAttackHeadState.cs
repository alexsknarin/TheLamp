using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyAttackHeadState", menuName = "FDragonflyMovementStates/FDragonflyAttackHeadState")]
    public class FDragonflyAttackHeadState : ScriptableObject, IState, ILeft
    {
        [SerializeField] private float _speed = 4f;
        [SerializeField] private float _acceleration = 0.75f;
        [SerializeField] private float _collisionReadyTime = 0.3f;
        private bool _isCollisionPhaseReached = false;
        private float _attackAccelerationValue = 0;
        private Vector3 _attackDirection;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _baseTransform;
        private float _localTime = 0f;
    
        public event Action Started;
        public event Action CollisionPhaseReached;

        public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _baseTransform = baseTransform;
        }

        public void Enter()
        {
            _localTime = 0;
            _isCollisionPhaseReached = false;
            Vector3 currentPosition = _visibleBodyTransform.position;
            _visibleBodyTransform.SetParent(_baseTransform);
            _attackAccelerationValue = 0;
        
            Vector3 sideGoal = currentPosition;
            sideGoal.z = 0;
            sideGoal.Normalize();
            sideGoal *= 0.85f;
        
            if (_visibleBodyTransform.position.z > 0)
            {
                sideGoal *= 0.55f;
                sideGoal.y *= 0.75f;
            }
            else
            {
                sideGoal *= 0.85f;
            }
            _attackDirection = (sideGoal - currentPosition).normalized;
            Started?.Invoke();
        }

        public void Tick()
        {
            _visibleBodyTransform.position += _attackDirection * (_speed * Time.deltaTime + _attackAccelerationValue);
            _attackAccelerationValue += _acceleration * Time.deltaTime;
            _localTime += Time.deltaTime; 
            if (!_isCollisionPhaseReached &&  _localTime >= _collisionReadyTime)
            {
                _isCollisionPhaseReached = true;
                CollisionPhaseReached?.Invoke();
            }
        }

        public void Exit() { }
    }
}
