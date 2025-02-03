using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class FFlyMovement : FEnemyMovementBase, IPositionDirectionProvider
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
    [SerializeField] private float _noiseFrequency;
    [SerializeField] private float _noiseAmplitude;
    [Header("-- Smooth Damp Settings --")]
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private float _smoothTime = .3f;
    private float _smoothTimeAllowed = 0;
    [Header("---- Depth Settings ----")]
    [SerializeField] bool _isDepthEnabled;
    // Debug
    [SerializeField] private string _stateDebug;
    [SerializeField] private int _sideDirection = 1;
    [SerializeField] private int _depthSideDirection = 0;
    
    private Vector3 _position3D;
    // Debug only
    private Vector3 _prevPosition;
    private Vector3 _prevPosSmooth;
    private Vector3 _velocity = Vector3.zero;

    // State Machine
    private readonly FStateMachine _stateMachine = new();
    private FlyMovementStateFactory _stateFactory;
    // States
    private FFlyMovementStateBase _currentState;
    private FFlyMovementEnterState _enterState;
    private FFlyMovementPatrolState _patrolState;
    private FFlyMovementPreAttackState _preAttackState;
    private FFlyMovementAttackState _attackState;
    private FFlyMovementFallState _fallState;
    private FFlyMovementDeathState _deathState;
    private FFlyMovementSpreadState _spreadState;
    
    
    // State parameters
    private bool _isAttacking = false;
    private WaitForSeconds _waitSmoothDamp = new(0.5f);
    
    public void Construct(FlyMovementStateFactory stateFactory)
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
        Debug.Log("FFlyMovement Initializing");
        // Create Movement States
        _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude);
        _enterState = (FFlyMovementEnterState)_stateFactory.Create(typeof(FFlyMovementEnterState));
        _patrolState = (FFlyMovementPatrolState)_stateFactory.Create(typeof(FFlyMovementPatrolState));
        _preAttackState = (FFlyMovementPreAttackState)_stateFactory.Create(typeof(FFlyMovementPreAttackState));
        _attackState = (FFlyMovementAttackState)_stateFactory.Create(typeof(FFlyMovementAttackState));
        _fallState = (FFlyMovementFallState)_stateFactory.Create(typeof(FFlyMovementFallState));
        _deathState = (FFlyMovementDeathState)_stateFactory.Create(typeof(FFlyMovementDeathState));
        _spreadState = (FFlyMovementSpreadState)_stateFactory.Create(typeof(FFlyMovementSpreadState));
        
        // Subscribe to state events
        _patrolState.Started += OnPatrolStateStarted; 
        _preAttackState.Started += OnPreAttackStateStarted;
        _preAttackState.Ended += OnPreAttackStateEnded;
        _deathState.Ended += OnDeathStateEnded;
        
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

    public override void TriggerSpread()
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
        _currentState = (FFlyMovementStateBase)_stateMachine.CurrentState;
        _stateDebug = _currentState.GetType().Name; // Debug only
        Position2D = _currentState.Position2D;
        DepthDirection = _currentState.DepthDirection;
        
        // Add Noise
        if (_isNoiseEnabled && _currentState.Equals(_patrolState))
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
        Vector3 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency);
        _position3D = (Vector3)Position2D + trajectoryNoise * _noiseAmplitude;
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
        enabled = false;
    }
}
