using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Megamothling.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MegamothlingMovementStateFactory
    {
        private Transform _cameraTransform;
        private ILampPositionProviderService _lampPositionProviderService;
        private IPositionDirectionProvider _positionDirectionProvider;
        private float _speed;
        private float _radius;
        private float _verticalAmplitude;
        private float _collisionRadius;
        private float _preAttackDuration;
        private float _fallBounceForce;
        private float _fallGravityForce;
    
        public MegamothlingMovementStateFactory(
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
            float preAttackDuration,
            float fallBounceForce,
            float fallGravityForce
        )
        {
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
            _radius = radius;
            _verticalAmplitude = verticalAmplitude;
            _collisionRadius = collisionRadius;
            _preAttackDuration = preAttackDuration;
            _fallBounceForce = fallBounceForce;
            _fallGravityForce = fallGravityForce;
        }

        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(FMegamothlingMovementEnterState))
            {
                return new FMegamothlingMovementEnterState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
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
            if (stateType == typeof(FlyGenericMovementPreAttackStateL))
            {
                return new FlyGenericMovementPreAttackStateL(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _speed,
                    _preAttackDuration
                );
            }
            if (stateType == typeof(FlyGenericMovementPreAttackStateR))
            {
                return new FlyGenericMovementPreAttackStateR(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _speed,
                    _preAttackDuration
                );
            }
            if (stateType == typeof(MegamothlingMovementAttackState))
            {
                return new MegamothlingMovementAttackState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
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
            if (stateType == typeof(MegamothlingMovementDeathState))
            {
                return new MegamothlingMovementDeathState(
                    _positionDirectionProvider
                );
            }
            return null;
        }
    }
}
