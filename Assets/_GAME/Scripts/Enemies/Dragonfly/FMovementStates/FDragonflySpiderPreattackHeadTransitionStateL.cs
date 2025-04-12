using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflySpiderPreattackHeadTransitionStateL", menuName = "FDragonflyMovementStates/FDragonflySpiderPreattackHeadTransitionStateL")]
    public class FDragonflySpiderPreattackHeadTransitionStateL : ScriptableObject, IState
    {
        [SerializeField] private float _duration = 0.65f;
        private Transform _patrolTransformParent;
        private float _phase;
        private float _localTime;
        private readonly int _sideDirection = 1;
        private bool _readyToSwitch = false;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _patrolTransform;
        private Transform _spiderPatrolTransform;
        private DragonflyPatrolRotator _patrolRotator;
        private DragonflyPatrolRotator _spiderPatrolRotator;
    
        public bool ReadyToSwitch => _readyToSwitch;

        public void SetDependencies(Transform visibleBodyTransform, Transform patrolTransform, Transform spiderPatrolTransform, 
            DragonflyPatrolRotator patrolRotator, DragonflyPatrolRotator spiderPatrolRotator)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _patrolTransform = patrolTransform;
            _spiderPatrolTransform = spiderPatrolTransform;
            _patrolRotator = patrolRotator;
            _spiderPatrolRotator = spiderPatrolRotator;
        }
    
        public void Enter()
        {
            Vector3 currentPosition = _visibleBodyTransform.position;
        
            _patrolTransformParent = _patrolTransform.parent;
            _spiderPatrolRotator.SetRotationPhase(currentPosition);
            _spiderPatrolRotator.Play(_sideDirection);
            _patrolRotator.SetRotationPhase(currentPosition);
            _patrolRotator.Stop();
        
            _visibleBodyTransform.SetParent(_spiderPatrolTransform);
            _visibleBodyTransform.position = _spiderPatrolTransform.position;
            _visibleBodyTransform.rotation = _spiderPatrolTransform.rotation;
        
            _localTime = 0;
            _phase = 0;
            _readyToSwitch = false;
        }
    
        public void Tick()
        {
            _patrolRotator.SetRotationPhase(_spiderPatrolTransform.position);
            _patrolTransform.SetParent(_spiderPatrolTransform);
        
            Vector3 position = Vector3.Lerp(Vector3.zero, _patrolTransform.localPosition, _phase);
            Quaternion rotation = Quaternion.Slerp(Quaternion.identity, _patrolTransform.localRotation, _phase);
        
            _patrolTransform.SetParent(_patrolTransformParent);
            _patrolTransform.localPosition = Vector3.zero;
            _patrolTransform.localRotation = Quaternion.identity;
            _patrolTransform.localScale = Vector3.one;
        
            _visibleBodyTransform.localPosition = position;
            _visibleBodyTransform.localRotation = rotation;
        
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
