using System;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderFallState : EnemyMovementStateBase, IPositionProvider
    {
        private Vector3 _initialDirection;
        private float _outForceMagnitude;
        private float _fallForceMagnitude;
        private bool _isFreeFall;
        private bool _outForceCancelled;
        
        // Dependencies
        private readonly Transform _visibleBodyTransform;
        private readonly Transform _calculatedTransform;
        private readonly Transform _lampTransform;
        private readonly Transform _rootTransform;
        private readonly Transform _cameraTransform;
        private readonly float _exitYCoordinate;
        // Config
        private readonly float _initialOutForceMagnitude;
        private readonly float _outForceIncrement;
        private readonly float _fallForceIncrement;

        public MegaspiderFallState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,  
            Transform lampTransform,
            Transform rootTransform,
            Transform cameraTransform,
            IGameConfigService configService,
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
            
            _initialOutForceMagnitude = configService.GameConfig.MegaspiderFallInitialOutForceMagnitude;
            _outForceIncrement = configService.GameConfig.MegaspiderFallOutForceIncrement;
            _fallForceIncrement = configService.GameConfig.MegaspiderFallFallForceIncrement;
        }


        public event Action Started;
        public event Action OutForceCancelled;
        public event Action Ended;

        public Vector3 Position3D => _calculatedTransform.position;

        public override void Enter()
        {
            IsReadyToSwitch = false;
            _visibleBodyTransform.SetParent(_rootTransform);
            _calculatedTransform.position = _visibleBodyTransform.position;
            HierarchyUtilities.ParentWithoutOffset(_visibleBodyTransform, _calculatedTransform);
            
            _initialDirection = (_calculatedTransform.position - _lampTransform.position).normalized;
            _outForceMagnitude = _initialOutForceMagnitude;
            _fallForceMagnitude = 0f;
            _outForceCancelled = false;
            Started?.Invoke();
        }

        public override void Tick()
        {
            _calculatedTransform.position += SimplePhysics.Fall(
                _initialDirection, 
                ref _outForceMagnitude, 
                ref _fallForceMagnitude, 
                _isFreeFall,
                _initialOutForceMagnitude,
                _outForceIncrement,
                _fallForceIncrement
                );
            
            Vector2 projectedPosition = CameraProjection.ProjectPointOnXYPlane(
                _cameraTransform.position,
                _calculatedTransform.position
                );
            
            if (projectedPosition.y < _exitYCoordinate)
            {
                IsReadyToSwitch = true;
            }

            if (!_outForceCancelled && _outForceMagnitude < 0.01f)
            {
                _outForceCancelled = true;
                OutForceCancelled?.Invoke();
            }
        }
        
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
