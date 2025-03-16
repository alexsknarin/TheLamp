using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Ladybug.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class LadybugMovementStateFactory
    {
        private Transform _cameraTransform;
        private IPositionDirectionProvider _positionDirectionProvider;
        private ILampPositionProviderService _lampPositionProviderService;
        private float _speed;
        private float _radius;
        private float _verticalAmplitude;
        private float _collisionRadius;
    
        public LadybugMovementStateFactory(
            Transform cameraTransform, 
            ILampPositionProviderService lampPositionProviderService
        )
        {
            _cameraTransform = cameraTransform;
            _lampPositionProviderService = lampPositionProviderService;
        }
    
        public void SetEnemyDependencies(
            IPositionDirectionProvider positionDirectionProvider,
            float speed,
            float radius,
            float verticalAmplitude,
            float collisionRadius
        )
        {
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
            _radius = radius;
            _verticalAmplitude = verticalAmplitude;
            _collisionRadius = collisionRadius;
        }

        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(LadybugMovementPatrolStateR))
            {
                return new LadybugMovementPatrolStateR(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(LadybugMovementPatrolStateL))
            {
                return new LadybugMovementPatrolStateL(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(LadybugMovementPreAttackStateR))
            {
                return new LadybugMovementPreAttackStateR(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed
                );
            }
            if (stateType == typeof(LadybugMovementPreAttackStateL))
            {
                return new LadybugMovementPreAttackStateL(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed
                );
            }
            if (stateType == typeof(LadybugMovementAttackState))
            {
                return new LadybugMovementAttackState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed
                );
            }
            if (stateType == typeof(LadybugMovementStickState))
            {
                return new LadybugMovementStickState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _collisionRadius
                );
            }
            if (stateType == typeof(LadybugMovementDeathFallState))
            {
                return new LadybugMovementDeathFallState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService
                );
            }
            if (stateType == typeof(FlyGenericMovementSpreadState))
            {
                return new FlyGenericMovementSpreadState(
                    _positionDirectionProvider,
                    _speed
                );
            }
            return null;
        }
    
    }
}
