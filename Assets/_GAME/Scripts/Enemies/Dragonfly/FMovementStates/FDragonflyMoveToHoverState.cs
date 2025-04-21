using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyMoveToHoverState", menuName = "FDragonflyMovementStates/FDragonflyMoveToHoverState")]
    public class FDragonflyMoveToHoverState : ScriptableObject, IState, ILeft
    {
        [SerializeField] private Vector3[] _hoverPoints;
        [SerializeField] private float _zMaxDistance = -1.90932f;
        [SerializeField] private float _zMinDistance = 2f;
        [SerializeField] private float _farDuration = 1.1f;
        [SerializeField] private float _closeDuration = 1.7f;
        [SerializeField] private float _distance = 9.5f;
        [SerializeField] private AnimationCurve _moveCurve;
        private float _localTime = 0f;
        private int _currentPointIndex = 0;
        private float _phase = 0f;
        private float _normalizedDuration = 0f;
        private Vector3 _startPos = Vector3.zero;
        private Vector3 _endPos = Vector3.zero;
        private Vector3 _startDirection = Vector3.zero;
        private Vector3 _endDirection = Vector3.zero;
        private Quaternion _startRotation = Quaternion.identity;
        private Quaternion _endRotation = Quaternion.identity;
        private bool _readyToSwitch = false;
        public bool ReadyToSwitch => _readyToSwitch;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _baseTransform;
    
        public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _baseTransform = baseTransform;
        }
    
        public void Enter()
        {
            _currentPointIndex = Random.Range(0, _hoverPoints.Length);
            _localTime = 0f;
            _phase = 0f;
        
            _endPos = _hoverPoints[_currentPointIndex];
            _startPos = _endPos;
            _startPos.y += -_distance;
        
            _endDirection = -(_endPos).normalized;
            _startDirection = _endDirection;
            _startDirection.y = 0;
            _startDirection.Normalize();
            _startRotation = Quaternion.LookRotation(_startDirection, Vector3.up);
            _endRotation = Quaternion.LookRotation(_endDirection, Vector3.up);

            _visibleBodyTransform.SetParent(_baseTransform);
            _visibleBodyTransform.position = _startPos;
            _visibleBodyTransform.rotation = Quaternion.LookRotation(_startDirection, Vector3.up);
        
            // Normalize duration by Z distance from camera
            float zPhase = Mathf.InverseLerp(_zMinDistance, _zMaxDistance, _endPos.z);
            _normalizedDuration = Mathf.Lerp(_farDuration, _closeDuration, zPhase);
        
            _readyToSwitch = false;
        }
    
        public void Tick()
        {
            Vector3 position = Vector3.Lerp(_startPos, _endPos, _moveCurve.Evaluate(_phase));
            _visibleBodyTransform.position = position;
        
            Quaternion rotation = Quaternion.Slerp(_startRotation, _endRotation, _phase);
            _visibleBodyTransform.rotation = rotation;
        
            _localTime += Time.deltaTime;
            CheckForStateChange();
        }

        public void Exit() { }

        private void CheckForStateChange()
        {
            _phase = _localTime / _normalizedDuration;
            if (_phase > 1)
            {
                _readyToSwitch = true;
            }
        }
    }
}
