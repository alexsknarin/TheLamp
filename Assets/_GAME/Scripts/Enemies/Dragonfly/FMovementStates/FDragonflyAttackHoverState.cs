using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyAttackHoverState", menuName = "FDragonflyMovementStates/FDragonflyAttackHoverState")]
    public class FDragonflyAttackHoverState : ScriptableObject, IState
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

        public void OnEnter()
        {
            _isCollisionPhaseReached = false;
            _localTime = 0f;
            Vector3 currentPosition = _visibleBodyTransform.position;
            _visibleBodyTransform.SetParent(_baseTransform);
            _attackAccelerationValue = 0;
        
            Vector3 sideGoal = currentPosition;
            sideGoal.z = 0;
            sideGoal.Normalize();
            sideGoal *= 0.85f;
        
            if (currentPosition.z > 0)
            {
                sideGoal *= 0.95f;
                sideGoal.y *= 0.75f;
            }
            else
            {
                sideGoal *= 0.85f;
            }
            _attackDirection = (sideGoal - currentPosition).normalized;
#if UNITY_EDITOR  
            Debug.DrawLine(currentPosition, sideGoal, Color.yellow, 5f);
#endif
            Started?.Invoke();
        }

        public void Tick()
        {
            if (_visibleBodyTransform.position.magnitude > 0.2f)
            {
                _visibleBodyTransform.position += _attackDirection * (_speed * Time.deltaTime + _attackAccelerationValue);
                _attackAccelerationValue += _acceleration * Time.deltaTime;    
            }
        
            _localTime += Time.deltaTime;

            if (!_isCollisionPhaseReached && _localTime >= _collisionReadyTime)
            {
                Debug.Log(" -- Collision phase reached - state: " + this);
                _isCollisionPhaseReached = true;
                CollisionPhaseReached?.Invoke();
            }
        }

        public void OnExit() { }
    }
}
