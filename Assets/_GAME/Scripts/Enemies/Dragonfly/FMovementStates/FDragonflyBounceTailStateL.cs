using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyBounceTailStateL",
        menuName = "FDragonflyMovementStates/FDragonflyBounceTailStateL")]
    public class FDragonflyBounceTailStateL : ScriptableObject, IState
    {
        [SerializeField] private float _rotationSpeed = 125f;
        private readonly int _sideDirection = 1;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _patrolTransform;
        private DragonflyPatrolRotator _patrolRotator;

        public void SetDependencies(Transform visibleBodyTransform, Transform patrolTransform,
            DragonflyPatrolRotator patrolRotator)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _patrolTransform = patrolTransform;
            _patrolRotator = patrolRotator;
        }

        public void Enter()
        {
            Vector3 currentPosition = _visibleBodyTransform.position;
            _patrolRotator.SetRotationPhase(currentPosition);
            _patrolRotator.Play(_sideDirection);

            _visibleBodyTransform.SetParent(_patrolTransform, false);
        }

        public void Tick()
        {
            Vector3 eulers = _visibleBodyTransform.localRotation.eulerAngles;
            eulers.y += _rotationSpeed * Time.deltaTime * _sideDirection;
            _visibleBodyTransform.localRotation = Quaternion.Euler(eulers);
        }

        public void Exit() { }
    }
}
