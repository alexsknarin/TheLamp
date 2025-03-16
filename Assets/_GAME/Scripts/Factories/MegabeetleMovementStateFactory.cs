using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Ladybug.MovementStates;
using _GAME.Scripts.Enemies.Megabeetle.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MegabeetleMovementStateFactory
    {
        private Transform _cameraTransform;
        private ILampPositionProviderService _lampPositionProviderService;
        private IPositionDirectionProvider _positionDirectionProvider;
        private float _speed;
        private float _radius;
        private float _verticalAmplitude;
        private float _collisionRadius;
    
        public MegabeetleMovementStateFactory(
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
            if (stateType == typeof(FMegabeetleMovementEnterStateR))
            {
                return new FMegabeetleMovementEnterStateR(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(FMegabeetleMovementEnterStateL))
            {
                return new FMegabeetleMovementEnterStateL(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(FMegabeetleMovementPatrolStateR))
            {
                return new FMegabeetleMovementPatrolStateR(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(FMegabeetleMovementPatrolStateL))
            {
                return new FMegabeetleMovementPatrolStateL(
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
            if (stateType == typeof(FMegabeetleMovementStickState))
            {
                return new FMegabeetleMovementStickState(_positionDirectionProvider);
            }
            if (stateType == typeof(FMegabeetleMovementStickPreAttackState))
            {
                return new FMegabeetleMovementStickPreAttackState(_positionDirectionProvider);
            }
            if (stateType == typeof(FMegabeetleMovementStickPreAttackPauseState))
            {
                return new FMegabeetleMovementStickPreAttackPauseState(_positionDirectionProvider);
            }
            if (stateType == typeof(FMegabeetleMovementStickAttackState))
            {
                return new FMegabeetleMovementStickAttackState(_positionDirectionProvider);
            }
            if (stateType == typeof(FMegabeetleMovementStickLandingState))
            {
                return new FMegabeetleMovementStickLandingState(_positionDirectionProvider);
            }
            if (stateType == typeof(FMegabeetleMovementFallState))
            {
                return new FMegabeetleMovementFallState(_positionDirectionProvider);
            }
            if (stateType == typeof(FMegabeetleMovementFallState))
            {
                return new FMegabeetleMovementFallState(_positionDirectionProvider);
            }
            if (stateType == typeof(FMegabeetleMovementDeathState))
            {
                return new FMegabeetleMovementDeathState(_positionDirectionProvider);
            }
            return null;
        }
    }
}
