using System;
using System.Collections;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Mothling.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Mothling
{
    public class MothlingMovement : EnemyMovementBase, IPositionDirectionProvider, ISpreadableMovement
    {
        [Header("-- Movement States Base Settings --")]
        [SerializeField] private float _speed;
        [SerializeField] private float _radius;
        [SerializeField] private float _verticalAmplitude;
        [Header("---- Spawn Settings ----")]
        [SerializeField] private float _spawnAreaSize = 0.5f;
        [SerializeField] private Vector2 _spawnAreaCenter;
        [Header("---- Noise Settings ----")]
        [SerializeField] private bool _isNoiseEnabled;
        [SerializeField] private float _noise1Frequency;
        [SerializeField] private float _noise1Amplitude;
        [SerializeField] private float _noise2Frequency;
        [SerializeField] private float _noise2Amplitude;
        [Header("-- Smooth Damp Settings --")]
        [SerializeField] private bool _isSmoothDampEnabled;
        [SerializeField] private float _smoothTime = .3f;
        private float _smoothTimeAllowed = 0;
        [Header("---- Depth Settings ----")]
        [SerializeField] bool _isDepthEnabled;
        [Header("---- States Settings ----")]
        [SerializeField] private float _fallBounceForce = 4f;
        [SerializeField] private float _fallGravityForce = .2f;
        // Debug
        [SerializeField] private string _stateDebug;
        [SerializeField] private int _sideDirection = 1;
        [SerializeField] private int _depthSideDirection = 0;
        
        private float _collisionRadius = 0.075f;
        private Vector3 _position3D;
        // Debug only
        private Vector3 _prevPosition;
        private Vector3 _prevPosSmooth;
        private Vector3 _velocity = Vector3.zero;

        // State Machine
        private readonly StateMachine _stateMachine = new();
        private MothlingMovementStateFactory _stateFactory;
        // States
        private EnemyMovementStateBase _currentState;
        private FlyGenericMovementEnterState _enterState;
        private FlyGenericMovementPatrolState _patrolState;
        private FMothlingMovementPreAttackState _preAttackState;
        private FMothlingMovementConstantAttackState _attackState;
        private FlyGenericMovementFallState _fallState;
        private FMothlingMovementDeathState _deathState;
        private FlyGenericMovementSpreadState _spreadState;
    
        private WaitForSeconds _waitSmoothDamp = new(0.5f);
    
        public void Construct(MothlingMovementStateFactory stateFactory)
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

        public override void Initialize()
        {
            // Create Movement States
            _stateFactory.SetEnemyDependencies(
                this,
                _speed,
                _radius,
                _verticalAmplitude,
                _collisionRadius,
                _fallBounceForce,
                _fallGravityForce
            );
            _enterState = (FlyGenericMovementEnterState)_stateFactory.Create(typeof(FlyGenericMovementEnterState));
            _patrolState = (FlyGenericMovementPatrolState)_stateFactory.Create(typeof(FlyGenericMovementPatrolState));
            _preAttackState = (FMothlingMovementPreAttackState)_stateFactory.Create(typeof(FMothlingMovementPreAttackState));
            _attackState = (FMothlingMovementConstantAttackState)_stateFactory.Create(typeof(FMothlingMovementConstantAttackState));
            _fallState = (FlyGenericMovementFallState)_stateFactory.Create(typeof(FlyGenericMovementFallState));
            _deathState = (FMothlingMovementDeathState)_stateFactory.Create(typeof(FMothlingMovementDeathState));
            _spreadState = (FlyGenericMovementSpreadState)_stateFactory.Create(typeof(FlyGenericMovementSpreadState));
        
            // Subscribe to state events
            _patrolState.Started += OnPatrolStateStarted;
            _patrolState.Ended += OnPatrolStateEnded;
            _preAttackState.Started += OnPreAttackStateStarted;
            _preAttackState.Ended += OnPreAttackStateEnded;
            _deathState.Ended += OnDeathStateEnded;
            _spreadState.Ended += OnSpreadStateEnded;
        
            // Automatic State transitions
            At(_enterState, _patrolState, () => _enterState.IsReadyToSwitch);
            At(_patrolState, _preAttackState, IsAttackStarted());
            At(_preAttackState, _attackState, () => _preAttackState.IsReadyToSwitch);
            At(_fallState, _enterState, IsFallEnded());
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
        
            Func<bool> IsFallEnded() => () =>
            {
                if (_fallState.IsReadyToSwitch)
                {
                    _sideDirection = -(int)Mathf.Sign(transform.position.x);
                    return true;
                }
                return false;
            };
        
            void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        }

        private void OnDestroy()
        {
            _patrolState.Started -= OnPatrolStateStarted; 
            _patrolState.Ended -= OnPatrolStateEnded;
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
            _sideDirection = RandomDirection.Generate();
            _depthSideDirection = RandomDirection.Generate();
            Position2D = GenerateSpawnPosition(-1);
            _position3D = Position2D;
            transform.position = _position3D;
        
            _currentState = _enterState;
            _stateMachine.SetState(_currentState);
        
            // Disable Smooth Damp at the beginning
            _smoothTimeAllowed = 0;
            if (_isSmoothDampEnabled)
            {
                StartCoroutine(SmoothDampDelay());
            }
        
            _isAttacking = false;
            enabled = true;
        }

        public override void TriggerAttack()
        {
            if (_currentState.Equals(_patrolState))
            {
                ApplyTransformToPosition2D();
                _isAttacking = true;
            }
        }

        public override void TriggerFall()
        {
            if (_currentState.Equals(_attackState))
            {
                ApplyTransformToPosition2D();
                _currentState = _fallState;
                _stateDebug = _currentState.GetType().Name; // Debug only
                _stateMachine.SetState(_currentState);

                // Immediately Apply Position2D and SideDirection to transform to avoid visible collision penetration.
                Vector3 newPosition = transform.position;
                newPosition.x = _currentState.Position2D.x;
                newPosition.y = _currentState.Position2D.y;
                transform.position = newPosition;
            
                // Refresh Smooth Damp velocity (for the sharp bounce).
                _velocity = Vector3.zero;
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
            if (_currentState.Equals(_enterState)||
                _currentState.Equals(_patrolState)||
                _currentState.Equals(_preAttackState)||
                _currentState.Equals(_fallState))
            {
                ApplyTransformToPosition2D();
                _currentState = _spreadState;
                _stateDebug = _currentState.GetType().Name; // Debug only
                _stateMachine.SetState(_currentState);
            }
        }

        private void Update()
        {
            // Debug only
            _prevPosition = _position3D;
            _prevPosSmooth = transform.position;

        
            _stateMachine.Tick();
            _currentState = (EnemyMovementStateBase)_stateMachine.CurrentState;
            _stateDebug = _currentState.GetType().Name; // Debug only
            Position2D = _currentState.Position2D;
            DepthDirection = _currentState.DepthDirection;
        
            // Add Noise
            if (_isNoiseEnabled)
            {
                AddMotionNoise();
            }
            else
            {
                _position3D = Position2D;
            }
        
            // Add Depth
            if (_isDepthEnabled)
            {
                int depthDirection = _depthSideDirection;
                // Always Jump forward in depth for Attack
                if (_currentState.Equals(_preAttackState) || _currentState.Equals(_attackState))
                {
                    depthDirection = 1;
                }
                _position3D += _currentState.DepthDirection * depthDirection;
            }
        
            // Apply side direction Only for States that require Left/Right mirroring
            if (_currentState.Equals(_enterState)||
                _currentState.Equals(_patrolState))
            {
                _position3D.x *= _sideDirection;            
            }
        
            // Add SmoothDamp
            if (_isSmoothDampEnabled)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _position3D, ref _velocity, _smoothTimeAllowed);
            }
            else
            {
                transform.position = _position3D;
            }
        
            Debug.DrawLine(_prevPosition, _prevPosition + (_position3D-_prevPosition).normalized*0.02f, Color.cyan, 5f);
            Debug.DrawLine(_prevPosSmooth, _prevPosSmooth + (transform.position-_prevPosSmooth).normalized*0.02f, Color.yellow, 5f);
        }

        private void AddMotionNoise()
        {
            Vector3 trajectoryNoise1 = TrajectoryNoise.Generate(_noise1Frequency);
            Vector3 trajectoryNoise2 = TrajectoryNoise.Generate(_noise2Frequency);

            if (_currentState.Equals(_attackState))
            {
                float noiseMultiplier = 0.5f;
                if (Position2D.magnitude < 0.86f)
                {
                    noiseMultiplier = 0.001f;
                }   
                trajectoryNoise1 *= noiseMultiplier;
                trajectoryNoise2 *= noiseMultiplier;
            }
            _position3D = (Vector3)Position2D + trajectoryNoise1 * _noise1Amplitude + trajectoryNoise2 * _noise2Amplitude;
        }

        private Vector2 GenerateSpawnPosition(int direction)
        {
            Vector2 spawnPosition = Random.insideUnitCircle * _spawnAreaSize + _spawnAreaCenter;
            spawnPosition.x *= direction;
            return spawnPosition;
        }

        private void ApplyTransformToPosition2D()
        {
            Vector2 newPosition2D = Position2D;
            newPosition2D.x = Mathf.Abs(newPosition2D.x) * Mathf.Sign(transform.position.x);
            Position2D = newPosition2D;
        }

        private IEnumerator SmoothDampDelay()
        {
            yield return _waitSmoothDamp;
            _smoothTimeAllowed = _smoothTime;
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
            enabled = false;
        }

        private void OnSpreadStateEnded()
        {
            // TODO: should be decided based on gameover state
            SpreadStateEnded?.Invoke();
            Play();
        }
    }
}
