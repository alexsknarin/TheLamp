using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflySpiderPatrolStateL", menuName = "FDragonflyMovementStates/FDragonflySpiderPatrolStateL")]
    public class FDragonflySpiderPatrolStateL : ScriptableObject, IState
    {
        private readonly int _sideDirection = 1;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _spiderPatrolTransform;
        private DragonflyPatrolRotator _spiderPatrolRotator;
        public event Action Started;

        public void SetDependencies(Transform visibleBodyTransform, Transform spiderPatrolTransform, 
            DragonflyPatrolRotator spiderPatrolRotator)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _spiderPatrolTransform = spiderPatrolTransform;
            _spiderPatrolRotator = spiderPatrolRotator;
        }
    
        public void Enter()
        {
            Vector3 currentPosition = _visibleBodyTransform.position;
        
            _spiderPatrolRotator.SetRotationPhase(currentPosition);
            _spiderPatrolRotator.Play(_sideDirection);
        
            _visibleBodyTransform.SetParent(_spiderPatrolTransform, false);
            _visibleBodyTransform.localPosition = Vector3.zero;
            _visibleBodyTransform.localRotation = Quaternion.identity;
            Started?.Invoke();
        }

        public void Tick() { }

        public void Exit()
        {
            _spiderPatrolRotator.Stop();
        }
    }
}
