using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyHoverState", menuName = "FDragonflyMovementStates/FDragonflyHoverState")]
    public class FDragonflyHoverState : ScriptableObject, IState, ILeft
    {
        [SerializeField] private float _amplitude = 0.5f;
        [SerializeField] private float _frequency = 0.33f;
        private float _localTime = 0f;
        private Vector3 _hoverPos = Vector3.zero;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _baseTransform;
    
        public event Action Started;

        public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _baseTransform = baseTransform;
        }
    
        public void Enter()
        {
            _visibleBodyTransform.SetParent(_baseTransform);
            _hoverPos = _visibleBodyTransform.localPosition;
            _localTime = 0f;
            Started?.Invoke();
        }

        public void Tick()
        {
            Vector3 hoverPos = _visibleBodyTransform.localPosition;
            hoverPos = _hoverPos + _visibleBodyTransform.right * ((Mathf.PerlinNoise1D(_localTime * _frequency + 2) - 0.5f) * _amplitude);
            hoverPos.y = _hoverPos.y + (Mathf.PerlinNoise1D(_localTime * _frequency) - 0.5f) * _amplitude;
            _visibleBodyTransform.localPosition = hoverPos;
        
            _localTime += Time.deltaTime;
        }

        public void Exit() { }
    }
}
