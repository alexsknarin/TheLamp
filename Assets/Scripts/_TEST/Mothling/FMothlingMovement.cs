using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FMothlingMovement : FEnemyMovementBase, IPositionDirectionProvider
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
    [SerializeField] private String _stateDebug;
    [SerializeField] private int _sideDirection = 1;
    [SerializeField] private int _depthSideDirection = 0;
    
    private Vector3 _position3D;
    // Debug only
    private Vector3 _prevPosition;
    private Vector3 _prevPosSmooth;
    private Vector3 _velocity = Vector3.zero;

    // State Machine
    private readonly FStateMachine _stateMachine = new();
    private MothlingMovementStateFactory _stateFactory;
    // States
    private FMothlingMovementStateBase _currentState;
    private FMothlingMovementEnterState _enterState;
    private FMothlingMovementPatrolState _patrolState;
    private FMothlingMovementPreAttackState _preAttackState;
    private FMothlingMovementAttackState _attackState;
    private FMothlingMovementFallState _fallState;
    private FMothlingMovementDeathState _deathState;
    private FMothlingMovementSpreadState _spreadState;
    
    // State parameters
    private bool _isAttacking = false;
    private bool _isCollided = false;
    private bool _isDead = false;
    private bool _isSpread = false;
    
    public void Construct(MothlingMovementStateFactory stateFactory)
    {
        _stateFactory = stateFactory;
    }
    
    public event Action PatrolStarted;
    public event Action PreAttackStarted;
    public event Action PreAttackEnded;
    public event Action DeathStateEnded;

    public Vector2 Position2D { get; private set; } 
    public Vector3 DepthDirection { get; private set; } 

    public override void Initialize()
    {
        Debug.Log("FMothlingMovement Initialize");
        _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude);
        _enterState = (FMothlingMovementEnterState)_stateFactory.Create(typeof(FMothlingMovementEnterState));
        _patrolState = (FMothlingMovementPatrolState)_stateFactory.Create(typeof(FMothlingMovementPatrolState));
        _preAttackState = (FMothlingMovementPreAttackState)_stateFactory.Create(typeof(FMothlingMovementPreAttackState));
        _attackState = (FMothlingMovementAttackState)_stateFactory.Create(typeof(FMothlingMovementAttackState));
        _fallState = (FMothlingMovementFallState)_stateFactory.Create(typeof(FMothlingMovementFallState));
        _deathState = (FMothlingMovementDeathState)_stateFactory.Create(typeof(FMothlingMovementDeathState));
        _spreadState = (FMothlingMovementSpreadState)_stateFactory.Create(typeof(FMothlingMovementSpreadState));
        
        // Subscribe to state events
        _patrolState.Started += OnPatrolStateStarted; 
        _preAttackState.Started += OnPreAttackStateStarted;
        _preAttackState.Ended += OnPreAttackStateEnded;
        _deathState.Ended += OnDeathStateEnded;
        
        // State transitions
        At(_enterState, _patrolState, () => _enterState.IsReadyToSwitch);
        At(_patrolState, _preAttackState, IsAttackStarted());
        At(_preAttackState, _attackState, () => _preAttackState.IsReadyToSwitch);
        At(_attackState, _fallState, IsCollided());
        At(_fallState, _enterState, IsFallEnded());
        Any(_deathState, () => _isDead);
        // Spread transitions
        At(_enterState, _spreadState, () => _isSpread);
        At(_patrolState, _spreadState, () => _isSpread);
        At(_preAttackState, _spreadState, () => _isSpread);
        At(_fallState, _spreadState, () => _isSpread);
        
        // TODO: events
        
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
        
        Func<bool> IsCollided() => () =>
        {
            if (_isCollided)
            {
                _isCollided = false;
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
        void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
    }

    private void OnDestroy()
    {
        _patrolState.Started -= OnPatrolStateStarted; 
        _preAttackState.Started -= OnPreAttackStateStarted;
        _preAttackState.Ended -= OnPreAttackStateEnded;
        _deathState.Ended -= OnDeathStateEnded;
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
        
        _isAttacking = false;
        _isCollided = false;
        _isDead = false;
    }

    public override void TriggerAttack()
    {
        if (_currentState.Equals(_patrolState))
        {
            _isAttacking = true;
        }
    }

    public override void TriggerFall(Vector2 newPosition)
    {
        if (_currentState.Equals(_attackState))
        {
            // Fix potential collision penetration
            Vector3 newPosition3D = transform.position;
            newPosition3D.x = newPosition.x;
            newPosition3D.y = newPosition.y;
            transform.position = newPosition3D;
            
            newPosition3D.x *= _sideDirection;
            Position2D = newPosition3D;
            
            // Refresh Smooth Damp
            _velocity = Vector3.zero;
            
            _isCollided = true;
        }
    }

    public override void TriggerDeath()
    {
        _isDead = true;
    }

    public override void TriggerSpread()
    {
        _isSpread = true;
    }

    private void Update()
    {
        // Debug only
        _prevPosition = _position3D;
        _prevPosSmooth = transform.position;

        
        _stateMachine.Tick();
        _currentState = (FMothlingMovementStateBase)_stateMachine.CurrentState;
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
        
        // Apply side direction
        _position3D.x *= _sideDirection;
        
        // Add SmoothDamp
        if (_isSmoothDampEnabled)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _position3D, ref _velocity, _smoothTime);
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

    private void OnPatrolStateStarted()
    {
        PatrolStarted?.Invoke();
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
}
