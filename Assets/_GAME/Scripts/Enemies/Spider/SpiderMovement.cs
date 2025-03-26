using System;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Spider.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Spider
{
    public class SpiderMovement : EnemyMovementBase, IPositionDirectionProvider, ISpreadableMovement
    {
        [Header("-- Movement Settings --")]
        [SerializeField] private float _speed;
        // Debug
        [SerializeField] private string _stateDebug;
        [SerializeField] private int _sideDirection = 1;
        [SerializeField] private float _height = 5f;
        [SerializeField] private float _xCenter = 1.12f;
    
        private ILampPositionProviderService _lampPositionProviderService;
        private Vector3 _position3D;

        private float _lampCollisionRadius;
        private float _collisionThreshold;
        private float _collisionRadius;

        // State Machine
        private readonly StateMachine _stateMachine = new();
        private SpiderMovementStateFactory _stateFactory;
        private EnemyMovementStateBase _currentState;
    
        private SpiderMovementEnterState _enterState;
        private SpiderMovementPatrolState _patrolState;
        private SpiderMovementPreAttackState _preAttackState;
        private SpiderMovementAttackState _attackState;
        private SpiderMovementReturnState _returnState;
        private FlyGenericMovementDeathState _deathState;
        private SpiderMovementClimbUpState _climbUpState;
    
        public void Construct(
            SpiderMovementStateFactory stateFactory, 
            ILampPositionProviderService lampPositionProviderService,
            float lampCollisionRadius,
            float collisionThreshold
            )
        {
            _stateFactory = stateFactory;
            _lampPositionProviderService = lampPositionProviderService;
            _lampCollisionRadius = lampCollisionRadius;
            _collisionThreshold = collisionThreshold;
        }
    
        public event Action ReadyToAttackStateStarted;
        public event Action ReadyToAttackStateEnded;
        public event Action PreAttackStarted;
        public event Action PreAttackEnded;
        public event Action DeathStateEnded;
        public event Action SpreadStateEnded;

        public Vector2 Position2D { get; private set; } 
        public Vector3 DepthDirection { get; private set; }
    
        public override int SideDirection => _sideDirection;
    
        public override void Initialize()
        {
            // Create Movement States
            _stateFactory.SetEnemyDependencies(this, _speed, _xCenter, _height, _collisionRadius);
            _enterState = (SpiderMovementEnterState)_stateFactory.Create(typeof(SpiderMovementEnterState));
            _patrolState = (SpiderMovementPatrolState)_stateFactory.Create(typeof(SpiderMovementPatrolState));
            _preAttackState = (SpiderMovementPreAttackState)_stateFactory.Create(typeof(SpiderMovementPreAttackState));
            _attackState = (SpiderMovementAttackState)_stateFactory.Create(typeof(SpiderMovementAttackState));
            _returnState = (SpiderMovementReturnState)_stateFactory.Create(typeof(SpiderMovementReturnState));
            _deathState = (FlyGenericMovementDeathState)_stateFactory.Create(typeof(FlyGenericMovementDeathState));
            _climbUpState = (SpiderMovementClimbUpState)_stateFactory.Create(typeof(SpiderMovementClimbUpState));
       
            // Subscribe to state events
            _patrolState.Started += OnPatrolStateStarted; 
            _patrolState.Ended += OnPatrolStateEnded;
            _preAttackState.Started += OnPreAttackStateStarted;
            _preAttackState.Ended += OnPreAttackStateEnded;
            _deathState.Ended += OnDeathStateEnded;
            _climbUpState.Ended += OnSpreadStateEnded;
        
            At(_enterState, _patrolState, () => _enterState.IsReadyToSwitch);
            At(_patrolState, _preAttackState, IsAttackStarted());
            At(_preAttackState, _attackState, () => _preAttackState.IsReadyToSwitch);
            At(_returnState, _patrolState, () => _returnState.IsReadyToSwitch);
            At(_climbUpState, _enterState, () => _climbUpState.IsReadyToSwitch);
        
            // Predicates
            Func<bool> IsAttackStarted() => () =>
            {
                if (_isAttacking)
                {
                    _isAttacking = false;
                    return true;
                }
                return false;
            };
        
        
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        }

        private void OnDestroy()
        {
            _patrolState.Started += OnPatrolStateStarted; 
            _patrolState.Ended += OnPatrolStateEnded;
            _preAttackState.Started -= OnPreAttackStateStarted;
            _preAttackState.Ended -= OnPreAttackStateEnded;
            _deathState.Ended -= OnDeathStateEnded;
            _climbUpState.Ended += OnSpreadStateEnded;
        }

        public void SetCollisionRadius(float radius)
        {
            _collisionRadius = radius;
        }

        public override void Play()
        {
            _sideDirection = RandomDirection.Generate();;
        
            SwitchToStateAndApply(_enterState);
        
            _isAttacking = false;
            enabled = true;
        }
        
        public void Restart()
        {
            SwitchToStateAndApply(_enterState);
            _isAttacking = false;
            enabled = true;
        }

        public override void TriggerAttack()
        {
            _isAttacking = true;
        }

        public override void TriggerFall()
        {
            HandleLampCollision();
            SwitchToStateAndApply(_returnState);
        }

        public override void TriggerDeath()
        {
            HandleLampCollision();
            SwitchToStateAndApply(_deathState);
        }

        public void TriggerSpread()
        {
            SwitchToStateAndApply(_climbUpState);
        }

        private void Update()
        {
            _stateMachine.Tick();
            _currentState = (EnemyMovementStateBase)_stateMachine.CurrentState;
            _stateDebug = _currentState.GetType().Name;
            Position2D = _currentState.Position2D;
        
            Vector2 newPosition = Position2D;
            newPosition.x *= _sideDirection;
        
            transform.position = newPosition;
        }

        private void SwitchToStateAndApply(EnemyMovementStateBase state)
        {
            _currentState = state;
            _stateMachine.SetState(_currentState);
            Position2D = _currentState.Position2D;
            Vector2 newPosition = Position2D;
        
            // Apply side direction
            newPosition.x *= _sideDirection;
        
            transform.position = newPosition;
            _stateDebug = _currentState.GetType().Name;
        }

        private void HandleLampCollision()
        {
            Vector2 lampPosition = _lampPositionProviderService.GetLampPosition();
            lampPosition.x *= _sideDirection;
            Vector2 collisionDirection = (Position2D - lampPosition).normalized;
            Position2D = lampPosition + collisionDirection * (
                _lampCollisionRadius + _collisionRadius + _collisionThreshold);
        }

        private void OnPatrolStateStarted()
        {
            ReadyToAttackStateStarted?.Invoke();
        }

        private void OnPatrolStateEnded()
        {
            ReadyToAttackStateEnded?.Invoke();
        }

        private void OnPreAttackStateStarted()
        {
            PreAttackStarted?.Invoke();        
        }

        private void OnPreAttackStateEnded()
        {
            PreAttackEnded?.Invoke();
        }

        private void OnDeathStateEnded()
        {
            DeathStateEnded?.Invoke();
        }

        private void OnSpreadStateEnded()
        {
            SpreadStateEnded?.Invoke();
        }
    }
}
