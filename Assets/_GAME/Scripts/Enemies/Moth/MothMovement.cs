using System;
using _GAME.Scripts.Enemies.Moth.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth
{
    public class MothMovement : EnemyMovementBase, IPositionDirectionProvider, ISpreadableMovement
    {
        [Header("-- Movement Settings --")]
        [SerializeField] private float _speed;
        [SerializeField] private float _radius;
        [SerializeField] private float _verticalAmplitude;
        [Header("---- Spawn Settings ----")]
        [SerializeField] private float _spawnXPos;
        [SerializeField] private float _spawnYPosMin;
        [SerializeField] private float _spawnYPosMax;
        [Header("---- Depth Settings ----")]
        [SerializeField] bool _isDepthEnabled;
        // Debug
        [SerializeField] private string _stateDebug;
        [SerializeField] private int _sideDirection = 1;
        [SerializeField] private int _depthSideDirection = 0;
        // Debug
        private Vector3 _position3d;
        private Vector3 _prevPosition;

        private readonly StateMachine _stateMachine = new();
        private MothMovementStateFactory _stateFactory;
        // Movement States
        private EnemyMovementStateBase _currentState;
        private MothMovementEnterState _enterState;
        private MothMovementHoverState _hoverState;
        private MothMovementNoisePatrolState _patrolState;
        private MothMovementPreAttackState _preAttackState;
        private MothMovementNoiseAttackState _attackState;
        private MothMovementNoiseFallState _fallState;
        private MothMovementNoiseDeathState _deathState;
        private MothMovementNoiseSpreadState _spreadState;


        public void Construct(MothMovementStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
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
            _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude);
            _enterState = (MothMovementEnterState)_stateFactory.Create(typeof(MothMovementEnterState));
            _hoverState = (MothMovementHoverState)_stateFactory.Create(typeof(MothMovementHoverState));
            _patrolState = (MothMovementNoisePatrolState)_stateFactory.Create(typeof(MothMovementNoisePatrolState));
            _preAttackState = (MothMovementPreAttackState)_stateFactory.Create(typeof(MothMovementPreAttackState));
            _attackState = (MothMovementNoiseAttackState)_stateFactory.Create(typeof(MothMovementNoiseAttackState));
            _fallState = (MothMovementNoiseFallState)_stateFactory.Create(typeof(MothMovementNoiseFallState));
            _deathState = (MothMovementNoiseDeathState)_stateFactory.Create(typeof(MothMovementNoiseDeathState));
            _spreadState = (MothMovementNoiseSpreadState)_stateFactory.Create(typeof(MothMovementNoiseSpreadState));
        
            _hoverState.Started += OnHoverStateStarted;
            _hoverState.Ended += OnHoverStateEnded;
            _preAttackState.Started += OnPreAttackStateStarted;
            _preAttackState.Ended += OnPreAttackStateEnded;
            _fallState.Ended += OnFallStateEnded;
            _deathState.Ended += OnDeathStateEnded;
            _spreadState.Ended += OnSpreadStateEnded;
        
        
            At(_enterState, _hoverState, () => _enterState.IsReadyToSwitch);
            At(_hoverState, _patrolState, () => _hoverState.IsReadyToSwitch);
            At(_hoverState, _preAttackState, IsAttackStarted());
            At(_patrolState, _hoverState, () => _patrolState.IsReadyToSwitch && Position2D.y < 0.9f);
            At(_preAttackState, _attackState, () => _preAttackState.IsReadyToSwitch);
        
            At(_fallState, _hoverState, () => _fallState.IsReadyToSwitch);
        
        
        
            // Predicates
            Func<bool> IsAttackStarted() => () =>
            {
                if (_isAttacking)
                {
                    Debug.Log("Attack started.");
                    _isAttacking = false;
                    return true;
                }
                return false;
            };
        
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        }

        private void OnDestroy()
        {
            _fallState.Ended -= OnFallStateEnded;
            _hoverState.Started -= OnHoverStateStarted;
            _hoverState.Ended -= OnHoverStateEnded;
            _preAttackState.Started -= OnPreAttackStateStarted;
            _preAttackState.Ended -= OnPreAttackStateEnded;
            _deathState.Ended -= OnDeathStateEnded;
            _spreadState.Ended -= OnSpreadStateEnded;
        }

        public override void Play()
        {
            _sideDirection = RandomDirection.Generate();
            _depthSideDirection = RandomDirection.Generate();
            _currentState = _enterState;
            _stateMachine.SetState(_currentState);
            _position3d = _enterState.Position2D;
            _position3d.x *= _sideDirection;
        
            transform.position = _position3d;
        
            _isAttacking = false;
            enabled = true;
        }

        public override void TriggerAttack()
        {
            Debug.Log("Attack triggered.");
            if (_currentState.Equals(_hoverState))
            {
                ApplyTransformToPosition2D(1);
                _isAttacking = true;    
            }
        }

        public override void TriggerFall()
        {
            if (_currentState.Equals(_attackState))
            {
                _currentState = _fallState;
                _stateDebug = _currentState.GetType().Name; // Debug only
                _stateMachine.SetState(_currentState);

                // Immediately Apply Position2D and SideDirection to transform to avoid visible collision penetration.
                transform.position = _fallState.Position2D;
                if (_isDepthEnabled)
                {
                    transform.position += DepthDirection;
                }
            }
        }

        public override void TriggerDeath()
        {
            ApplyTransformToPosition2D(1);
            _currentState = _deathState;
            _stateDebug = _currentState.GetType().Name; // Debug only
            _stateMachine.SetState(_currentState);
        }

        public void TriggerSpread()
        {
            if (!_currentState.Equals(_attackState))
            {
                ApplyTransformToPosition2D(1);
                _currentState = _spreadState;
                _stateDebug = _currentState.GetType().Name; // Debug only
                _stateMachine.SetState(_currentState);
            }
        }

        private void Update()
        {
            _prevPosition = transform.position;
        
            _stateMachine.Tick();
            _currentState = (EnemyMovementStateBase)_stateMachine.CurrentState;
            _stateDebug = _currentState.GetType().Name;
            Position2D = _currentState.Position2D;
            DepthDirection = _currentState.DepthDirection;
        
            _position3d = Position2D;
        
            if (_currentState.Equals(_patrolState) || _currentState.Equals(_hoverState) || _currentState.Equals(_enterState))
            {
                _position3d.x *= _sideDirection;
            }
        
            if (_isDepthEnabled)
            {
                transform.position = _position3d + DepthDirection;
            }
            else
            {
                transform.position = _position3d;
            }
        
            Debug.DrawLine(_prevPosition, transform.position, Color.cyan, 5f);
        }

        private void ApplyTransformToPosition2D(int direction)
        {
            Vector2 newPosition2D = Position2D;
            newPosition2D.x = Mathf.Abs(newPosition2D.x) * Mathf.Sign(transform.position.x) * direction;
            Position2D = newPosition2D;
        }

        private void OnHoverStateEnded()
        {
            ReadyToAttackStateEnded?.Invoke();
        }

        private void OnHoverStateStarted()
        {
            ReadyToAttackStateStarted?.Invoke();
        }

        private void OnPreAttackStateStarted()
        {
            PreAttackStarted?.Invoke();
        }

        private void OnPreAttackStateEnded()
        {
            PreAttackEnded?.Invoke();
        }

        private void OnFallStateEnded()
        {
            Debug.Log("Fall state ended.");
            ApplyTransformToPosition2D(_sideDirection);
        }

        private void OnDeathStateEnded()
        {
            DeathStateEnded?.Invoke();
        }

        private void OnSpreadStateEnded()
        {
            // TODO: 
            // if lamp is not dead or gameover (need to DI this information or let EnemyManager decide)
            SpreadStateEnded?.Invoke();
            Play();
        }
    }
}
