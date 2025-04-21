using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyDeathTailStateL", menuName = "FDragonflyMovementStates/FDragonflyDeathTailStateL")]
    public class FDragonflyDeathTailStateL : ScriptableObject, IState, ILeft
    {
        private const int SideDirection = 1;
        [SerializeField] private float _duration = 2f;
        [SerializeField] private float _afterDelay = 1f;
        [SerializeField] private float _fallSpeed = 100f;
        [SerializeField] private float _rotationSpeed = 380f;
        [SerializeField] private AnimationCurve _rzMixCurve;
        [SerializeField] private float _rzMaxValue;
        private float _localTime;
        private float _phase;
        private bool _isAfterDelay;
        private float _startRz;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _baseTransform;
    
        public event Action Started;
        public event Action Ended;

        public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _baseTransform = baseTransform;
        }
    
        public void Enter()
        {
            _visibleBodyTransform.SetParent(_baseTransform);
            _startRz = _visibleBodyTransform.localEulerAngles.z;
            _localTime = 0f;
            _phase = 0f;
            _isAfterDelay = false;
            Started?.Invoke();
        }
    
        public void Tick()
        {
            if (!_isAfterDelay)
            {
                Vector3 position = _visibleBodyTransform.localPosition;
                position += Vector3.down * (_fallSpeed * Time.deltaTime);
                _visibleBodyTransform.localPosition = position;
        
        
                Vector3 rotation = _visibleBodyTransform.localEulerAngles;
                rotation.y += _rotationSpeed * Time.deltaTime * SideDirection;
                rotation.z = _startRz - _rzMixCurve.Evaluate(_phase) * _rzMaxValue;
                _visibleBodyTransform.localEulerAngles = rotation;     
            }
        
            _localTime += Time.deltaTime;
            CheckForStateChange();
        }

        public void Exit() { }

        private void CheckForStateChange()
        {
            if (!_isAfterDelay)
            {
                _phase = _localTime / _duration;
                if (_phase > 1)
                {
                    _isAfterDelay = true;
                    _localTime = 0f;
                }    
            }
            else if (_isAfterDelay && _localTime > _afterDelay)
            {
                _isAfterDelay = false;
                Ended?.Invoke();
            }
        }
    }
}
