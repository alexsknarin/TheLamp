using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    [CreateAssetMenu(fileName = "FDragonflyBounceHeadState", menuName = "FDragonflyMovementStates/FDragonflyBounceHeadState")]
    public class FDragonflyBounceHeadState : ScriptableObject, IState
    {
        [SerializeField] private float _speed = 4.1f;
        private Vector3 _attackDirection;
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
            _visibleBodyTransform.SetParent(_baseTransform);
            _attackDirection = -_visibleBodyTransform.position.normalized;
        }

        public void Tick()
        {
            _visibleBodyTransform.position += -_attackDirection * (_speed * Time.deltaTime);
        }

        public void Exit() { }
    }
}
