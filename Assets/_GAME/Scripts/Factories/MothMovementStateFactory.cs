using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Moth.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MothMovementStateFactory
    {
        private Transform _cameraTransform;
        private ILampPositionProviderService _lampPositionProviderService;
        private IPositionDirectionProvider _positionDirectionProvider;
        private IGameConfigService _gameConfigService;
        private float _speed;
        private float _radius;
        private float _verticalAmplitude;
        private float _collisionRadius;
    
        public MothMovementStateFactory(
            Transform cameraTransform, 
            ILampPositionProviderService lampPositionProviderService,
            IGameConfigService gameConfigService
        )
        {
            _cameraTransform = cameraTransform;
            _lampPositionProviderService = lampPositionProviderService;
            _gameConfigService = gameConfigService;
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
            if (stateType == typeof(MothMovementEnterState))
            {
                return new MothMovementEnterState(
                    _cameraTransform.position,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(MothMovementHoverState))
            {
                return new MothMovementHoverState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _speed,
                    _radius
                );
            }
            if (stateType == typeof(MothMovementNoisePatrolState))
            {
                return new MothMovementNoisePatrolState(
                    _positionDirectionProvider,
                    _speed,
                    _radius,
                    _verticalAmplitude
                );
            }
            if (stateType == typeof(MothMovementPreAttackState))
            {
                return new MothMovementPreAttackState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _speed
                );
            }
            if (stateType == typeof(MothMovementNoiseAttackState))
            {
                return new MothMovementNoiseAttackState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed
                );
            }
            if (stateType == typeof(MothMovementNoiseFallState))
            {
                return new MothMovementNoiseFallState(
                    _cameraTransform.position,
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _radius,
                    _verticalAmplitude,
                    _gameConfigService.PlayerConfig.LampCollisionRadius,
                    _gameConfigService.PlayerConfig.CollisionThreshold,
                    _collisionRadius
                );
            }
            if (stateType == typeof(MothMovementNoiseDeathState))
            {
                return new MothMovementNoiseDeathState(
                    _positionDirectionProvider
                );
            }
            if (stateType == typeof(MothMovementNoiseSpreadState))
            {
                return new MothMovementNoiseSpreadState(
                    _positionDirectionProvider,
                    _speed
                );
            }
            return null;
        }


    }
}
