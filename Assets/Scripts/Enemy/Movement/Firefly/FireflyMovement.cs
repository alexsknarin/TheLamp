using UnityEngine;
using Random = UnityEngine.Random;

public class FireflyMovement : EnemyMovement
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    private int _sideDirection;
    [Header("---- Spawn Settings ----")]
    [SerializeField] private float _spawnAreaSize = 0.5f;
    [SerializeField] private Vector3 _spawnAreaCenter;
    [Header("---- Noise Settings ----")]
    [SerializeField] private bool _isNoiseEnabled;
    [SerializeField] private float _noiseFrequency;
    [SerializeField] private float _noiseAmplitude;
    [Header("-- Smooth Damp Settings --")]
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private float _smoothTime = .3f;
    private Vector3 _velocity = Vector3.zero;
    [Header("---- Depth Settings ----")]
    [SerializeField] bool _isDepthEnabled;
    private int _depthDirection;
    
    // Movement Stats
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private FlyMovementEnterState _enterState;
    private FlyMovementPatrolState _patrolState;
    private FlyMovementAttackState _attackState;
    private FlyMovementPreAttackState _preAttackState;    
    private FlyMovementFallState _fallState;
    private FireflyMovementDeathState _deathState;
    
    private Vector3 _prevPosition2d; //Debug
    private Vector3 _prevPosSmooth; //Debug
    private Vector3 _position2d;
    private Vector3 _position;
    
    // State parameters
    private bool _isDead = false;
    private bool _isCollided = false;
    
    // Debug
    [SerializeField] private EnemyStates _stateDebug;
    
    public override void Initialize()
    {
        _sideDirection = RandomDirection.Generate();
        _depthDirection = RandomDirection.Generate();
        
        _movementStateMachine = new EnemyMovementStateMachine();
        
        _enterState = new FlyMovementEnterState(this, _speed, _radius, _verticalAmplitude);
        _patrolState  = new FlyMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new FlyMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new FlyMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new FlyMovementFallState(this, _speed, _radius, _verticalAmplitude);
        _deathState = new FireflyMovementDeathState(this, _speed, _radius, _verticalAmplitude);
        
        _position2d = GenerateSpawnPosition(-_sideDirection);
        
        _currentState = _enterState;
        
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }
    
    private Vector3 GenerateSpawnPosition(int direction)
    {
        Vector3 spawnPosition = (Vector3)(Random.insideUnitCircle * _spawnAreaSize) + _spawnAreaCenter;
        spawnPosition.x *= direction;
        return spawnPosition;
    }
    
    public override void TriggerFall()
    {
        if(_currentState.State == EnemyStates.Attack)
        {
            _isCollided = true;
            SwitchState();
        }
    }

    public override void TriggerDeath()
    {
        if(_currentState.State != EnemyStates.Death)
        {
            _isDead = true;
            SwitchState();
        }
    }

    public override void TriggerAttack()
    {
        SwitchState();
    }

    public override void TriggerStick()
    {
    }

    public override void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState;
        switch (_currentState.State)
        {
            case EnemyStates.Enter:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    newState = _patrolState;
                    break;    
                }
            case EnemyStates.Patrol:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    OnPreAttackStartInvoke();
                    newState = _preAttackState;
                    break;    
                }
            case EnemyStates.PreAttack:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    OnPreAttackEndInvoke();
                    newState = _attackState;
                    break;    
                }
            case EnemyStates.Attack:
                if (_isCollided && !_isDead)
                {
                    newState = _fallState;
                    _isCollided = false;
                    break;
                }
                else if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    break;    
                }
            case EnemyStates.Fall:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    _sideDirection = -(int)Mathf.Sign(_position2d.x);
                    newState = _enterState;
                    break;
                }
            case EnemyStates.Death:
                OnEnemyDeactivatedInvoke();
                break;
        }

        _currentState = newState;
        State = _currentState.State;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
        OnStateChangeInvoke();
    }

    private void Update()
    {   
        _prevPosition2d = _position2d;
        
        // Debug Only
        _prevPosSmooth = transform.position;
       
        _movementStateMachine.Execute(_position2d);
        _position2d = _currentState.Position;
        _position = _position2d;
        
        // Add Noise
        if (_isNoiseEnabled && _currentState.State == EnemyStates.Patrol)  
        {
            Vector3 trajectoryNoise = TrajectoryNoise.Generate(_noiseFrequency); 
            _position2d += trajectoryNoise * _noiseAmplitude;
            _position = _position2d;
        }
        
        // Add Depth
        if (_isDepthEnabled)
        {
            _position = _position2d + _currentState.Depth;
        }
        
        // Add SmoothDamp
        if (_isSmoothDampEnabled)
        {
            if (_currentState.State == EnemyStates.Attack)
            {
                transform.position = _position;
            }
            else
            {
                transform.position = Vector3.SmoothDamp(transform.position, _position, ref _velocity, _smoothTime);
            }
        }
        else
        {
            transform.position = _position2d;
        }
        
        _movementStateMachine.CheckForStateChange();
        
        Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d-_prevPosition2d).normalized*0.02f, Color.cyan, 5f);
        Debug.DrawLine(_prevPosSmooth, _prevPosSmooth + (transform.position-_prevPosSmooth).normalized*0.02f, Color.yellow, 5f);
        
        _stateDebug = _currentState.State;

    }
}
