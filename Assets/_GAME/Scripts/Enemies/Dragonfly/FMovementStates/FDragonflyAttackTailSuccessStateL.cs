using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyAttackTailSuccessStateL", menuName = "FDragonflyMovementStates/FDragonflyAttackTailSuccessStateL")]
    public class FDragonflyAttackTailSuccessStateL : ScriptableObject, IState, ILeft
    {
        [SerializeField] private float _duration = 1.1f;
        [SerializeField] private AnimationCurve _txCurve;
        [SerializeField] private AnimationCurve _tyCurve;
        [SerializeField] private AnimationCurve _tzCurve;
        [SerializeField] private AnimationCurve _rxCurve;
        [SerializeField] private AnimationCurve _ryCurve;
        [SerializeField] private AnimationCurve _rzCurve;
        private Vector3 _startPosition;
        private Vector3 _startEuelerRotation;
        private float _localTime = 0f;
        private float _phase = 0f;
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
            _startPosition = _visibleBodyTransform.position;
            _startEuelerRotation = _visibleBodyTransform.eulerAngles;
            _localTime = 0f;
            _phase = 0f;
            Started?.Invoke();
        }
    
        public void Tick()
        {
            Vector3 pos = _startPosition;
            pos.x += _txCurve.Evaluate(_phase);
            pos.y += _tyCurve.Evaluate(_phase);
            pos.z += _tzCurve.Evaluate(_phase);
        
            Vector3 rot = _startEuelerRotation;
            rot.x += _rxCurve.Evaluate(_phase);
            rot.y += _ryCurve.Evaluate(_phase);
            rot.z += _rzCurve.Evaluate(_phase);
        
            _visibleBodyTransform.position = pos;
            _visibleBodyTransform.eulerAngles = rot;
        
            _localTime += Time.deltaTime;
            CheckForStateChange();
        }

        public void Exit() { }

        private void CheckForStateChange()
        {
            _phase = _localTime / _duration;
            if (_phase > 1)
            {
                Ended?.Invoke();
            }
        }
    }
}
