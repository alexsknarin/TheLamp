using System;
using _GAME.Scripts.Enemies.Dragonfly;
using _GAME.Scripts.Enemies.Dragonfly.BehaviourStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class DragonflyBehaviourStateFactory: IInitializable
    {
        private readonly IGameConfigService _gameConfigService;
        private DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
        private Transform _visibleBodyTransform;
        private DragonflyMovement _movement;
        private float _swarmAttackDuration;
            
        public DragonflyBehaviourStateFactory(IGameConfigService gameConfigService)
        {
            _gameConfigService = gameConfigService;
        }
    
        public void Initialize()
        {
            _patrolAttackPositionProvider = new(
                _gameConfigService.GameConfig.PatrolAttackZonesL,
                _gameConfigService.GameConfig.PatrolAttackZonesR,
                _gameConfigService.GameConfig.DragonflyTailAttackPositionBase
            );
        }
        
        public void SetEnemyDependencies(
            Transform visibleBodyTransform,
            DragonflyMovement movement,
            float swarmAttackDuration
        )
        {
            _visibleBodyTransform = visibleBodyTransform;
            _movement = movement;
            _swarmAttackDuration = swarmAttackDuration;
        }


        public IState Create(Type stateType)
        {
            if (stateType == typeof(DragonflyInactiveState))
            {
                return new DragonflyInactiveState();
            }
            if (stateType == typeof(DragonflyPassiveState))
            {
                return new DragonflyPassiveState();
            }
            if (stateType == typeof(DragonflyPatrolState))
            {
                return new DragonflyPatrolState();
            }
            if (stateType == typeof(DragonflyHoverState))
            {
                return new DragonflyHoverState();
            }
            if (stateType == typeof(DragonflyPatrolHeadState))
            {
                return new DragonflyPatrolHeadState(
                    _gameConfigService.GameConfig.DragonflyPatrolHeadWaitMin,
                    _gameConfigService.GameConfig.DragonflyPatrolHeadWaitMax
                );
            }
            if (stateType == typeof(DragonflyPatrolTailState))
            {
                return new DragonflyPatrolTailState(
                    _gameConfigService.GameConfig.DragonflyPatrolTailWaitMin,
                    _gameConfigService.GameConfig.DragonflyPatrolTailWaitMax
                );
            }
            if (stateType == typeof(DragonflyWaitHeadAttackState))
            {
                return new DragonflyWaitHeadAttackState(
                    _visibleBodyTransform,
                    _patrolAttackPositionProvider,
                    _movement
                );
            }
            if (stateType == typeof(DragonflyWaitTailAttackState))
            {
                return new DragonflyWaitTailAttackState(
                    _visibleBodyTransform,
                    _patrolAttackPositionProvider,
                    _movement
                );
            }
            if (stateType == typeof(DragonflyWaitHoverAttackState))
            {
                return new DragonflyWaitHoverAttackState(
                    _gameConfigService.GameConfig.DragonflyHoverWaitMin,
                    _gameConfigService.GameConfig.DragonflyHoverWaitMax,
                    _movement
                );
            }
            if (stateType == typeof(DragonflyWaitSpiderAttackState))
            {
                return new DragonflyWaitSpiderAttackState(
                    _visibleBodyTransform,
                    _gameConfigService.GameConfig.DragonflySpiderAttackPositionBase
                );
            }
            if (stateType == typeof(DragonflySpiderEnterState))
            {
                return new DragonflySpiderEnterState();
            }
            if (stateType == typeof(DragonflyPatrolSpiderState))
            {
                return new DragonflyPatrolSpiderState(
                    _gameConfigService.GameConfig.DragonflySpiderPatrolWaitMin,
                    _gameConfigService.GameConfig.DragonflySpiderPatrolWaitMax
                );
            }
            if (stateType == typeof(DragonflySwarmAttackState))
            {
                return new DragonflySwarmAttackState(_swarmAttackDuration);
            }
            if (stateType == typeof(DragonflyWaitForBounceState))
            {
                return new DragonflyWaitForBounceState();
            }

            return null;
        }
    }
}
