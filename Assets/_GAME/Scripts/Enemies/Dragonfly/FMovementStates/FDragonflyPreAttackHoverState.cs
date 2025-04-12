using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyPreAttackHoverState", menuName = "FDragonflyMovementStates/FDragonflyPreAttackHoverState")]
    public class FDragonflyPreAttackHoverState : ScriptableObject, IState
    {
        [SerializeField] private float _speed = 4f;
        [SerializeField] private float _duration = 0.4f;
        [SerializeField] private float _deccelerationPower = 2f;
        private Vector3 _attackDirection;
        private float _localTime = 0f;
        private float _phase = 0f;
        private bool _readyToSwitch = false;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _baseTransform;

        public event Action Started;
    
        public bool ReadyToSwitch => _readyToSwitch;

        public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _baseTransform = baseTransform;
        }
    
        public void Enter()
        {
            _visibleBodyTransform.SetParent(_baseTransform);
            _attackDirection = -_visibleBodyTransform.position.normalized;
            _localTime = 0f;
            _phase = 0f;
            _readyToSwitch = false;
            Started?.Invoke();
        }

        public void Tick()
        {
            float decceleration = Mathf.Pow(1-_phase, _deccelerationPower);
            _visibleBodyTransform.position += -_attackDirection * (_speed * decceleration * Time.deltaTime);

            _localTime += Time.deltaTime;
            CheckForStateChange();
        }

        public void Exit() { }

        private void CheckForStateChange()
        {
            _phase = _localTime / _duration;
            if (_phase > 1f)
            {
                _readyToSwitch = true;
            }
        }
    }
}
