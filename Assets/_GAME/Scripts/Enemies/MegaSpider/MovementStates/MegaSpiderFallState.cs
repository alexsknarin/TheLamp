using _GAME.Scripts.Lib;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderFallState : EnemyMovementStateBase
    {
        private const float InitialOutForceMagnitude = 1.8f;
        private const float OutForceIncrement = 5f;
        private const float FallForceIncrement = 6f;
        
        private Vector3 _initialDirection;
        private float _outForceMagnitude;
        private float _fallForceMagnitude;
        private bool _isFreeFall;
        
        
        
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _calculatedTransform;
        private Transform _lampTransform;
        private Transform _rootTransform;
        private float _exitYCoordinate;

        public MegaspiderFallState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,  
            Transform lampTransform,
            Transform rootTransform,
            float exitYCoordinate,
            bool isFreeFall
            )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _lampTransform = lampTransform;
            _rootTransform = rootTransform;
            _exitYCoordinate = exitYCoordinate;
            _isFreeFall = isFreeFall;
        }
        
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
                
            _visibleBodyTransform.SetParent(_rootTransform);
            
            _calculatedTransform.position = _visibleBodyTransform.position;
            _visibleBodyTransform.SetParent(_calculatedTransform);
            _visibleBodyTransform.localPosition = Vector3.zero;
            
            
            _initialDirection = (_calculatedTransform.position - _lampTransform.position).normalized;
            _outForceMagnitude = InitialOutForceMagnitude;
            _fallForceMagnitude = 0f;

        }

        public override void Tick()
        {
            _calculatedTransform.position += SimplePhysics.Fall(
                _initialDirection, 
                ref _outForceMagnitude, 
                ref _fallForceMagnitude, 
                _isFreeFall,
                InitialOutForceMagnitude,
                OutForceIncrement,
                FallForceIncrement
                );
            
            if (_calculatedTransform.position.y < _exitYCoordinate)
            {
                Debug.Log("Megaspider FallState: Exiting");
                IsReadyToSwitch = true;
            }
        }
    }
}
