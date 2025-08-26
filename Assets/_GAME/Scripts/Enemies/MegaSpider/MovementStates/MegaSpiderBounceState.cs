using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderBounceState : EnemyMovementStateBase
    {
        private Vector3 _direction;
        
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private Transform _lampTransform;
        private float _speed;
        
        public MegaSpiderBounceState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,  
            Transform lampTransform,
            float speed)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _lampTransform = lampTransform;
            _speed = speed;
        }
        
        public override void Enter()
        {
            _visibleBodyTransform.SetParent(null);
            
            // TODO: TEMP for tests - remove later
            Vector3 position = Vector3.up * (0.51f+0.325f);
            float angle = 145f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            position = rotation * position + _lampTransform.position;
            _visibleBodyTransform.position = position;
            
            
            _calculatedTransform.position = _visibleBodyTransform.position;
            _visibleBodyTransform.SetParent(_calculatedTransform);
            _visibleBodyTransform.localPosition = Vector3.zero;

            _direction = (position - _lampTransform.position).normalized;
        }

        public override void Tick()
        {
            Vector3 newPosition = _calculatedTransform.position;
            newPosition += _direction * _speed * Time.deltaTime;
            
            _calculatedTransform.position = newPosition;
        }
    }
}
