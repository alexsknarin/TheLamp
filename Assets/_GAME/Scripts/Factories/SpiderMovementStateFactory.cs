using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Spider.MovementStates;
using _GAME.Scripts.Lib.Interfaces;

namespace _GAME.Scripts.Factories
{
    public class SpiderMovementStateFactory
    {
        private ILampPositionProviderService _lampPositionProviderService;
        private IGameConfigService _gameConfigService;
        
        private IPositionDirectionProvider _positionDirectionProvider;
        private float _speed;
        private float _xCenter;
        private float _height;
        private float _collisionRadius;
    
        public SpiderMovementStateFactory(
            ILampPositionProviderService lampPositionProviderService,
            IGameConfigService gameConfigService
        )
        {
            _lampPositionProviderService = lampPositionProviderService;
            _gameConfigService = gameConfigService;
        }
        
        public void SetEnemyDependencies(
            IPositionDirectionProvider positionDirectionProvider,
            float speed,
            float xCenter,
            float height,
            float collisionRadius
        )
        {
            _positionDirectionProvider = positionDirectionProvider;
            _speed = speed;
            _xCenter = xCenter;
            _height = height;
            _collisionRadius = collisionRadius;
        }

        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(SpiderMovementEnterState))
            {
                return new SpiderMovementEnterState(
                    _speed,
                    _xCenter,
                    _height
                );
            }
            if (stateType == typeof(SpiderMovementPatrolState))
            {
                return new SpiderMovementPatrolState(
                    _positionDirectionProvider,
                    _xCenter,
                    _height
                );
            }
            if (stateType == typeof(SpiderMovementPreAttackState))
            {
                return new SpiderMovementPreAttackState(
                    _positionDirectionProvider,
                    _xCenter,
                    _height
                );
            }
            if (stateType == typeof(SpiderMovementAttackState))
            {
                return new SpiderMovementAttackState(
                    _positionDirectionProvider,
                    _speed,
                    _xCenter,
                    _height
                );
            }
            if (stateType == typeof(SpiderMovementReturnState))
            {
                return new SpiderMovementReturnState(
                    _positionDirectionProvider,
                    _lampPositionProviderService,
                    _speed,
                    _xCenter,
                    _height,
                    _gameConfigService.PlayerConfig.LampCollisionRadius,
                    _gameConfigService.PlayerConfig.CollisionThreshold,
                    _collisionRadius
                );
            }
            if (stateType == typeof(FlyGenericMovementDeathState))
            {
                return new FlyGenericMovementDeathState(
                    _positionDirectionProvider,
                    false
                );
            }
            if (stateType == typeof(SpiderMovementClimbUpState))
            {
                return new SpiderMovementClimbUpState(
                    _positionDirectionProvider,
                    _xCenter,
                    _height
                );
            }
        
            return null;
        }
    }
}
