using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Mothling.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MothlingMovementStateFactory
    {
        private Transform _cameraTransform;
        private ILampPositionProviderService _lampPositionProviderService;
        private IPositionDirectionProvider _positionDirectionProvider;
        private float _speed;
        private float _radius;
        private float _verticalAmplitude;
        private float _collisionRadius;
        private float _fallBounceForce;
        private float _fallGravityForce;

        public MothlingMovementStateFactory(
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
            float collisionRadius,
            float fallBounceForce,
            float fallGravityForce
        )
        {
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
            _radius = radius;
            _verticalAmplitude = verticalAmplitude;
            _collisionRadius = collisionRadius;
            _fallBounceForce = fallBounceForce;
            _fallGravityForce = fallGravityForce;
        }
    
    
        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(GenericIdleMovementState))
            {
                return new GenericIdleMovementState();
            }
            if (stateType == typeof(FlyGenericMovementEnterState))
            {
                return new FlyGenericMovementEnterState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(FlyGenericMovementPatrolState))
            {
                return new FlyGenericMovementPatrolState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(FMothlingMovementPreAttackState))
            {
                return new FMothlingMovementPreAttackState(
                    _cameraTransform.position,
                    _positionDirectionProvider
                );
            }
            if (stateType == typeof(FMothlingMovementConstantAttackState))
            {
                return new FMothlingMovementConstantAttackState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed
                );
            }
            if (stateType == typeof(FlyGenericMovementFallState))
            {
                return new FlyGenericMovementFallState(
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _radius,
                    _verticalAmplitude,
                    _collisionRadius,
                    _fallBounceForce,
                    _fallGravityForce
                );
            }
            if (stateType == typeof(FMothlingMovementDeathState))
            {
                return new FMothlingMovementDeathState(
                    _positionDirectionProvider
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
