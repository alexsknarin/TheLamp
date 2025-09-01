using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderBounceState : EnemyMovementStateBase
    {
        private Vector3 _direction;
        
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private Transform _lampTransform;
        private Transform _rootTransform;
        private float _speed;
        
        public MegaspiderBounceState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,  
            Transform lampTransform,
            Transform rootTransform,
            float speed)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _lampTransform = lampTransform;
            _rootTransform = rootTransform;
            _speed = speed;
        }
        
        public event Action Ended;
        
        public override void Enter()
        {
            _visibleBodyTransform.SetParent(_rootTransform);
           
            _calculatedTransform.position = _visibleBodyTransform.position;
            _visibleBodyTransform.SetParent(_calculatedTransform);
            _visibleBodyTransform.localPosition = Vector3.zero;

            _direction = (_calculatedTransform.position - _lampTransform.position).normalized;
        }

        public override void Tick()
        {
            Vector3 newPosition = _calculatedTransform.position;
            newPosition += _direction * _speed * Time.deltaTime;
            
            _calculatedTransform.position = newPosition;
        }
        
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
