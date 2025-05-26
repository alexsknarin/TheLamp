using System;
using _GAME.Scripts.Enemies.Generic.States;
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
        [SerializeField] private int _depthSideDirection;
        private float _collisionRadius;
        // Debug
        private Vector3 _position3d;
        private Vector3 _prevPosition;
        private bool _isPatrolEnterChecked;
        private bool _isAfterSpreadAttackSkipFinished;

        private readonly StateMachine _stateMachine = new();
        private MothMovementStateFactory _stateFactory;
        // Movement States
        private EnemyMovementStateBase _currentState;
        private GenericIdleMovementState _idleState;
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
        public event Action HoverStateStarted;
        public event Action PatrolStateStarted;

        public Vector2 Position2D { get; private set; }
        public Vector3 DepthDirection { get; private set; }
        public override int SideDirection => _sideDirection;

        public override void Initialize()
        {
            _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude, _collisionRadius);
            _idleState = (GenericIdleMovementState)_stateFactory.Create(typeof(GenericIdleMovementState));
            _enterState = (MothMovementEnterState)_stateFactory.Create(typeof(MothMovementEnterState));
            _hoverState = (MothMovementHoverState)_stateFactory.Create(typeof(MothMovementHoverState));
            _patrolState = (MothMovementNoisePatrolState)_stateFactory.Create(typeof(MothMovementNoisePatrolState));
            _preAttackState = (MothMovementPreAttackState)_stateFactory.Create(typeof(MothMovementPreAttackState));
            _attackState = (MothMovementNoiseAttackState)_stateFactory.Create(typeof(MothMovementNoiseAttackState));
            _fallState = (MothMovementNoiseFallState)_stateFactory.Create(typeof(MothMovementNoiseFallState));
            _deathState = (MothMovementNoiseDeathState)_stateFactory.Create(typeof(MothMovementNoiseDeathState));
            _spreadState = (MothMovementNoiseSpreadState)_stateFactory.Create(typeof(MothMovementNoiseSpreadState));

            _patrolState.Started += OnPatrolStateStarted;
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
            _patrolState.Started -= OnPatrolStateStarted;
            _fallState.Ended -= OnFallStateEnded;
            _hoverState.Started -= OnHoverStateStarted;
            _hoverState.Ended -= OnHoverStateEnded;
            _preAttackState.Started -= OnPreAttackStateStarted;
            _preAttackState.Ended -= OnPreAttackStateEnded;
            _deathState.Ended -= OnDeathStateEnded;
            _spreadState.Ended -= OnSpreadStateEnded;
        }

        public void SetCollisionRadius(float radius)
        {
            _collisionRadius = radius;
        }

        public override void Play()
        {
            _stateMachine.SetState(_idleState);
            
            SetInitialDirections();
            _currentState = _enterState;
            _stateMachine.SetState(_currentState);
            _position3d = _enterState.Position2D;
            _position3d.x *= _sideDirection;

            transform.position = _position3d;
            _isPatrolEnterChecked = false;
            
            _isAttacking = false;
            enabled = true;
        }

        private void SetInitialDirections()
        {
            _sideDirection = RandomDirection.Generate();
            _depthSideDirection = -1;
        }

        public override void TriggerAttack()
        {
            if (_currentState.Equals(_hoverState))
            {
                ApplyTransformToPosition2D();
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
            ApplyTransformToPosition2D();
            _currentState = _deathState;
            _stateDebug = _currentState.GetType().Name; // Debug only
            _stateMachine.SetState(_currentState);
        }

        public void TriggerSpread()
        {
            if (!_currentState.Equals(_attackState))
            {
                ApplyTransformToPosition2D();
                _currentState = _spreadState;
                _stateDebug = _currentState.GetType().Name; // Debug only
                _stateMachine.SetState(_currentState);
            }
            
            ReadyToAttackStateEnded?.Invoke();
            _isAfterSpreadAttackSkipFinished = false;
        }

        private void Update()
        {
            _prevPosition = transform.position;
            UpdateStateMachine();

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
        
            Debug.DrawLine(_prevPosition, transform.position, Color.cyan, 2.5f);
            
            // Fix incorrect side switch
            if (_currentState.GetType() == typeof(MothMovementNoisePatrolState) && !_isPatrolEnterChecked)
            {
                _isPatrolEnterChecked = true;

                FixIncorrectSideSwitch();
            }
        }

        private void UpdateStateMachine()
        {
            _stateMachine.Tick();
            _currentState = (EnemyMovementStateBase)_stateMachine.CurrentState;
            _stateDebug = _currentState.GetType().Name;
            Position2D = _currentState.Position2D;
            DepthDirection = _currentState.DepthDirection;
        }

        private void FixIncorrectSideSwitch()
        {
            // Check if side is switched incorrectly and fix it:
            int prevPositionSign = (int)(Mathf.Sign(_prevPosition.x));
            int currentPositionSign = (int)(Mathf.Sign(transform.position.x));
            if (prevPositionSign != currentPositionSign)
            {
                Vector2 newPosition2D = Position2D;
                newPosition2D.x *= -1;
                Position2D = newPosition2D;
                    
                Vector3 newPosition3D = transform.position;
                newPosition3D.x *= -1;
                transform.position = newPosition3D;
            }
        }

        private void ApplyTransformToPosition2D()
        {
            Vector2 newPosition2D = Position2D;
            newPosition2D.x = Mathf.Abs(newPosition2D.x) * Mathf.Sign(transform.position.x);
            Position2D = newPosition2D;
        }

        private void OnPatrolStateStarted()
        {
            PatrolStateStarted?.Invoke();
        }

        private void OnHoverStateStarted()
        {
            if (_isAfterSpreadAttackSkipFinished)
            {
                ReadyToAttackStateStarted?.Invoke();    
            }
            _isPatrolEnterChecked = false;
            HoverStateStarted?.Invoke();
        }

        private void OnHoverStateEnded()
        {
            ReadyToAttackStateEnded?.Invoke();
            _isAfterSpreadAttackSkipFinished = true;
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
            _sideDirection = RandomDirection.Generate();

            Vector2 newPosition2D = Position2D;
            newPosition2D.x = Mathf.Abs(newPosition2D.x) * Mathf.Sign(transform.position.x) * _sideDirection;
            Position2D = newPosition2D;
            
            _currentState = _patrolState;
            _stateDebug = _currentState.GetType().Name; // Debug only
            _stateMachine.SetState(_currentState);
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
