using System;
using System.Collections;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Megamothling.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Megamothling
{
    public class MegamothlingMovement : EnemyMovementBase, IPositionDirectionProvider
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
        [Header("---- Depth Settings ----")]
        [SerializeField] bool _isDepthEnabled;
        // Debug
        [SerializeField] private string _stateDebug;
        [SerializeField] private int _sideDirection = 1;
        [SerializeField] private int _depthSideDirection = 0;
        [Header("---- States Settings ----")]
        [SerializeField] private float _preAttackDuration = .45f;
        [SerializeField] private float _fallBounceForce = 2f;
        [SerializeField] private float _fallGravityForce = .1f;
        private float _collisionRadius;
        private float _smoothTimeAllowed = 0;
    
        private Vector3 _position3D;
        // Debug only
        private Vector3 _prevPosition;
        private Vector3 _prevPosSmooth;
        private Vector3 _velocity = Vector3.zero;
    
        // State Machine
        private readonly StateMachine _stateMachine = new();
        private MegamothlingMovementStateFactory _stateFactory;
        // States
        private EnemyMovementStateBase _currentState;
        private GenericIdleMovementState _idleState;
        private MegamothlingMovementEnterState _enterState;
        private FlyGenericMovementPatrolState  _patrolState;
        private FlyGenericMovementPreAttackStateL _preAttackStateL;
        private FlyGenericMovementPreAttackStateR _preAttackStateR;
        private MegamothlingMovementAttackState _attackState;
        private FlyGenericMovementFallState _fallState;
        private MegamothlingMovementDeathState _deathState;
    
        private WaitForSeconds _waitSmoothDamp = new(0.5f);
    
    
        public void Construct(MegamothlingMovementStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }
    
        public event Action ReadyToAttackStateStarted;
        public event Action ReadyToAttackStateEnded;
        public event Action PreAttackStarted;
        public event Action PreAttackEnded;
        public event Action DeathStateEnded;
    
        public Vector2 Position2D { get; private set; } 
        public Vector3 DepthDirection { get; private set; }

        public void SetCollisionRadius(float radius)
        {
            _collisionRadius = radius;
        }

        public override void Initialize()
        {
            // Create Movement States
            _stateFactory.SetEnemyDependencies(
                this,
                _speed,
                _radius,
                _verticalAmplitude,
                _collisionRadius,
                _preAttackDuration,
                _fallBounceForce,
                _fallGravityForce
            );
            _idleState = (GenericIdleMovementState)_stateFactory.Create(typeof(GenericIdleMovementState));
            _enterState = (MegamothlingMovementEnterState)_stateFactory.Create(typeof(MegamothlingMovementEnterState));
            _patrolState = (FlyGenericMovementPatrolState)_stateFactory.Create(typeof(FlyGenericMovementPatrolState));
            _preAttackStateL = (FlyGenericMovementPreAttackStateL)_stateFactory.Create(typeof(FlyGenericMovementPreAttackStateL));
            _preAttackStateR = (FlyGenericMovementPreAttackStateR)_stateFactory.Create(typeof(FlyGenericMovementPreAttackStateR));
            _attackState = (MegamothlingMovementAttackState)_stateFactory.Create(typeof(MegamothlingMovementAttackState));
            _fallState = (FlyGenericMovementFallState)_stateFactory.Create(typeof(FlyGenericMovementFallState));
            _deathState = (MegamothlingMovementDeathState)_stateFactory.Create(typeof(MegamothlingMovementDeathState));
        
            // Subscribe to state events
            _patrolState.Started += OnPatrolStateStarted;
            _patrolState.Ended += OnPatrolStateEnded;
            _preAttackStateL.Started += OnPreAttackStateStarted;
            _preAttackStateR.Started += OnPreAttackStateStarted;
            _preAttackStateL.Ended += OnPreAttackStateEnded;
            _preAttackStateR.Ended += OnPreAttackStateEnded;
            _deathState.Ended += OnDeathStateEnded;
        
            // Automatic State transitions
            At(_enterState, _patrolState, () => _enterState.IsReadyToSwitch);
            At(_patrolState, _preAttackStateR, IsAttackStartedR());
            At(_patrolState, _preAttackStateL, IsAttackStartedL());
            At(_preAttackStateR, _attackState, () => _preAttackStateR.IsReadyToSwitch);
            At(_preAttackStateL, _attackState, () => _preAttackStateL.IsReadyToSwitch);
            At(_fallState, _enterState, IsFallEnded());
        
            // Predicates
            Func<bool> IsAttackStartedR() => () =>
            {
                if (_isAttacking && _sideDirection == 1)
                {
                    _isAttacking = false;
                    return true;
                }
                return false;
            };
        
            Func<bool> IsAttackStartedL() => () =>
            {
                if (_isAttacking && _sideDirection == -1)
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
            _preAttackStateL.Started -= OnPreAttackStateStarted;
            _preAttackStateR.Started -= OnPreAttackStateStarted;
            _preAttackStateL.Ended -= OnPreAttackStateEnded;
            _preAttackStateR.Ended -= OnPreAttackStateEnded;
            _deathState.Ended -= OnDeathStateEnded;
        }

        public override void Play()
        {
            _stateMachine.SetState(_idleState);
            
            SetInitialDirections();
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

        private void SetInitialDirections()
        {
            _sideDirection = RandomDirection.Generate();
            _depthSideDirection = RandomDirection.Generate();
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
                _stateDebug = _currentState.GetType().Name;
                _stateMachine.SetState(_currentState);
                
                ApplyPosition2DToTransform();

                // Refresh Smooth Damp velocity (for the sharp bounce).
                _velocity = Vector3.zero;
            }
        }

        private void ApplyPosition2DToTransform()
        {
            Vector3 newPosition = transform.position;
            newPosition.x = _currentState.Position2D.x;
            newPosition.y = _currentState.Position2D.y;
            transform.position = newPosition;
        }

        public override void TriggerDeath()
        {
            ApplyTransformToPosition2D();
        
            _currentState = _deathState;
            _stateDebug = _currentState.GetType().Name; // Debug only
            _stateMachine.SetState(_currentState);
        }
    
        private void Update()
        {
            StashPreviousPositions();
            UpdateStateMachine();

            if (_isNoiseEnabled)
            {
                AddMotionNoise();
            }
            else
            {
                _position3D = Position2D;
            }
        
            if (_isDepthEnabled)
            {
                AddDepth();
            }
        
            // Apply side direction Only for States that require Left/Right mirroring
            if (_currentState.Equals(_enterState)||
                _currentState.Equals(_patrolState))
            {
                _position3D.x *= _sideDirection;            
            }
        
            ApplySmoothDampIfEnabled();
            DrawMotionDebugLines();
        }

        private void StashPreviousPositions()
        {
            _prevPosition = _position3D;
            _prevPosSmooth = transform.position;
        }

        private void UpdateStateMachine()
        {
            _stateMachine.Tick();
            _currentState = (EnemyMovementStateBase)_stateMachine.CurrentState;
            _stateDebug = _currentState.GetType().Name; // Debug only
            Position2D = _currentState.Position2D;
            DepthDirection = _currentState.DepthDirection;
        }

        private void AddMotionNoise()
        {
            Vector3 trajectoryNoise1 = TrajectoryNoise.Generate(_noise1Frequency);
            Vector3 trajectoryNoise2 = TrajectoryNoise.Generate(_noise2Frequency);

            if (_currentState.Equals(_attackState))
            {
                float noiseMultiplier = 0.5f;
                if (Position2D.magnitude < 0.8f)
                {
                    noiseMultiplier = 0.001f;
                }   
                trajectoryNoise1 *= noiseMultiplier;
                trajectoryNoise2 *= noiseMultiplier;
            }
        
            if (_currentState.Equals(_deathState))
            {
                trajectoryNoise1 *= 0.1f;
                trajectoryNoise2 *= 0.25f;
            }
            _position3D = (Vector3)Position2D + trajectoryNoise1 * _noise1Amplitude + trajectoryNoise2 * _noise2Amplitude;
        }

        private void AddDepth()
        {
            int depthDirection = _depthSideDirection;
            // Always Jump forward in depth for Attack
            if (_currentState.Equals(_preAttackStateL) 
                || _currentState.Equals(_preAttackStateR) 
                || _currentState.Equals(_attackState))
            {
                depthDirection = 1;
            }
            _position3D += _currentState.DepthDirection * depthDirection;
        }

        private void ApplySmoothDampIfEnabled()
        {
            if (_isSmoothDampEnabled)
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    _position3D,
                    ref _velocity,
                    _smoothTimeAllowed);
            }
            else
            {
                transform.position = _position3D;
            }
        }

        private void DrawMotionDebugLines()
        {
            Debug.DrawLine(
                _prevPosition, 
                _prevPosition + (_position3D-_prevPosition).normalized*0.02f, 
                Color.cyan,
                5f);
            Debug.DrawLine(
                _prevPosSmooth,
                _prevPosSmooth + (transform.position-_prevPosSmooth).normalized*0.02f, 
                Color.yellow,
                5f);
        }


        private Vector2 GenerateSpawnPosition(int direction)
        {
            Vector2 spawnPosition = (Random.insideUnitCircle * _spawnAreaSize) + _spawnAreaCenter;
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
    }
}
