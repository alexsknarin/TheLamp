using System;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderDeathFallState: EnemyMovementStateBase
    {
        private Vector3 _initialDirection;
        private float _outForceMagnitude;
        private float _fallForceMagnitude;
        private float _currentSpeed;
        private float _localTime;

        // Dependencies
        private readonly Transform _visibleBodyTransform;
        private readonly Transform _calculatedTransform;
        private readonly Transform _lampTransform;
        private readonly Transform _rootTransform;
        private readonly Transform _cameraTransform;
        // Config
        private readonly float _initialOutForceMagnitude;
        private readonly float _outForceIncrement;
        private readonly float _fallForceIncrement;
        private readonly float _exitYCoordinate;
        private readonly bool _isFreeFall;
        private readonly float _deathFallAccelerationDecrement;
        private readonly float _deathFallDuration;

        public MegaspiderDeathFallState(
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
            _deathFallAccelerationDecrement = configService.GameConfig.MegaspiderDeathAccelerationDecrement;
            _deathFallDuration = configService.GameConfig.MegaspiderDeathDuration;
        }

        public event Action Ended;
        
        public override void Enter()
        {
            IsReadyToSwitch = false;
            _visibleBodyTransform.SetParent(_rootTransform);
            _calculatedTransform.position = _visibleBodyTransform.position;
            HierarchyUtilities.ParentWithoutOffset(_visibleBodyTransform, _calculatedTransform);
            
            _initialDirection = (_calculatedTransform.position - _lampTransform.position).normalized;
            _outForceMagnitude = _initialOutForceMagnitude;
            _fallForceMagnitude = 0f;
            _currentSpeed = 1;
            _localTime = 0;
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
                _fallForceIncrement,
                _currentSpeed
                );
            
            Vector2 projectedPosition = CameraProjection.ProjectPointOnXYPlane(
                _cameraTransform.position,
                _calculatedTransform.position
                );
            
            _currentSpeed -= _deathFallAccelerationDecrement * Time.deltaTime;
            if (_currentSpeed < 0)
            {
                _currentSpeed = 0;
            }
            
            _localTime += Time.deltaTime;
            
            if (_localTime > _deathFallDuration)
            {

                IsReadyToSwitch = true;
            }
            
        }

        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
