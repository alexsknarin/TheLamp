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
        private Transform _cameraTransform;
        private float _exitYCoordinate;

        public MegaspiderFallState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,  
            Transform lampTransform,
            Transform rootTransform,
            Transform cameraTransform,
            float exitYCoordinate,
            bool isFreeFall
            )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _calculatedTransform = calculatedTransform;
            _lampTransform = lampTransform;
            _rootTransform = rootTransform;
            _cameraTransform = cameraTransform;
            _exitYCoordinate = exitYCoordinate;
            _isFreeFall = isFreeFall;
        }
        
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
            _visibleBodyTransform.SetParent(_rootTransform);
            _calculatedTransform.position = _visibleBodyTransform.position;
            HierarchyUtilities.ParentWithoutOffset(_visibleBodyTransform, _calculatedTransform);
            
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
            
            Vector2 projectedPosition = CameraProjection.ProjectPointOnXYPlane(
                _cameraTransform.position,
                _calculatedTransform.position
                );
            
            if (projectedPosition.y < _exitYCoordinate)
            {

                IsReadyToSwitch = true;
            }
        }
    }
}
