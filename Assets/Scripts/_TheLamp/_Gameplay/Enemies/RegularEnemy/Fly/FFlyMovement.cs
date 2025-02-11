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
    [SerializeField] private float _proximityOffset = 0.11f;
    [SerializeField] private bool _isDeathByTimer;
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
    private RegularEnemyMovementStateBase _currentState;
    private FFlyGenericMovementEnterState _enterState;
    private FFlyGenericMovementPatrolState _patrolState;
    private FFlyMovementPreAttackStateR _preAttackStateR;
    private FFlyMovementPreAttackStateL _preAttackStateL;
    private FFlyGenericMovementAcceleratedAttackState _attackState;
    private FFlyGenericMovementFallState _fallState;
    private FFlyMovementDeathState _deathState;
    private FFlyGenericMovementSpreadState _spreadState;
    
    
    // State parameters
    private bool _isAttacking = false;
    private WaitForSeconds _waitSmoothDamp = new(0.5f);
    
    public void Construct(FlyMovementStateFactory stateFactory)
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
        Debug.Log("FFlyMovement Initializing");
        // Create Movement States
        // TODO: get collision radius from configs
        _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude, _proximityOffset, _isDeathByTimer, 0.1f); // TODO: magic numbers
        _enterState = (FFlyGenericMovementEnterState)_stateFactory.Create(typeof(FFlyGenericMovementEnterState));
        _patrolState = (FFlyGenericMovementPatrolState)_stateFactory.Create(typeof(FFlyGenericMovementPatrolState));
        _preAttackStateR = (FFlyMovementPreAttackStateR)_stateFactory.Create(typeof(FFlyMovementPreAttackStateR));
        _preAttackStateL = (FFlyMovementPreAttackStateL)_stateFactory.Create(typeof(FFlyMovementPreAttackStateL));
        _attackState = (FFlyGenericMovementAcceleratedAttackState)_stateFactory.Create(typeof(FFlyGenericMovementAcceleratedAttackState));
        _fallState = (FFlyGenericMovementFallState)_stateFactory.Create(typeof(FFlyGenericMovementFallState));
        _deathState = (FFlyMovementDeathState)_stateFactory.Create(typeof(FFlyMovementDeathState));
        _spreadState = (FFlyGenericMovementSpreadState)_stateFactory.Create(typeof(FFlyGenericMovementSpreadState));
        
        // Subscribe to state events
        _patrolState.Started += OnPatrolStateStarted; 
        _patrolState.Ended += OnPatrolStateEnded;
        _preAttackStateR.Started += OnPreAttackStateStarted;
        _preAttackStateR.Ended += OnPreAttackStateEnded;
        _preAttackStateL.Started += OnPreAttackStateStarted;
        _preAttackStateL.Ended += OnPreAttackStateEnded;
        _deathState.Ended += OnDeathStateEnded;
        _spreadState.Ended += OnSpreadStateEnded;
        
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
        _preAttackStateR.Started -= OnPreAttackStateStarted;
        _preAttackStateR.Ended -= OnPreAttackStateEnded;
        _preAttackStateL.Started -= OnPreAttackStateStarted;
        _preAttackStateL.Ended -= OnPreAttackStateEnded;
        _deathState.Ended -= OnDeathStateEnded;
        _spreadState.Ended -= OnSpreadStateEnded;
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
            // TODO: doublecheck this
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
            _currentState.Equals(_preAttackStateR)||
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
        _currentState = (RegularEnemyMovementStateBase)_stateMachine.CurrentState;
        _stateDebug = _currentState.GetType().Name; // Debug only
        Position2D = _currentState.Position2D;
        DepthDirection = _currentState.DepthDirection;
        
        // Add Noise
        if (_isNoiseEnabled && 
            (_currentState.Equals(_patrolState) ||
             _currentState.Equals(_enterState)))
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
            if (_currentState.Equals(_preAttackStateR) || _currentState.Equals(_attackState))
            {
                depthDirection = 1;
            }
            _position3D += _currentState.DepthDirection * depthDirection;
        }
        
        // Apply side direction Only for States that require Left/Right mirroring
        if (_currentState.Equals(_enterState)||
            _currentState.Equals(_patrolState) 
            )
        {
            _position3D.x *= _sideDirection;            
        }
        
        // Add SmoothDamp
        if (_isSmoothDampEnabled && !_currentState.Equals(_attackState))
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


    // TODO: use the same in Mothling

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
        // TODO: external control over it
        SpreadStateEnded?.Invoke();
        Play();
    }
}
