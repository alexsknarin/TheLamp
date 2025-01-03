using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MegamothlingMovement : EnemyMovement
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [Header("---- Spawn Settings ----")]
    [SerializeField] private float _spawnAreaSize = 0.5f;
    [SerializeField] private Vector3 _spawnAreaCenter;
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
    [SerializeField] private EnemyState _stateDebug;
    private Vector3 _velocity = Vector3.zero;
    private int _sideDirection;
    private int _depthDirection;
    // Movement States
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private MegamothlingMovementEnterState _enterState;
    private FlyMovementPatrolState _patrolState;
    private MegamothlingMovementAttackState _attackState;
    private MegamothlingMovementPreAttackState _preAttackState;    
    private MegamothlingMovementFallState _fallState;
    private MegamothlingMovementDeathState _deathState;
    private MothlingMovementSpreadState _spreadState;
    //Debug
    private Vector3 _prevPosition2d; 
    private Vector3 _prevPosSmooth; 
    private Vector3 _position2d;
    private Vector3 _position;
    private Vector3 _prevPosition;
    private readonly Vector3 IDLE_POSITION = new Vector3(-5f, 2f, 0); 
    // State parameters
    private bool _isDead = false;
    private bool _isCollided = false;
    private bool _isPlaying = false;
    
    public event Action OnBossAttackStartedEvent;
    public event Action DeathStateEnded;

    public override void Initialize()
    {
        _isDead = false;
        _isCollided = false;
        _movementStateMachine = new EnemyMovementStateMachine();
        _enterState = new MegamothlingMovementEnterState(this, _speed, _radius, _verticalAmplitude);
        _patrolState  = new FlyMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new MegamothlingMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new MegamothlingMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new MegamothlingMovementFallState(this, _speed, _radius, _verticalAmplitude);
        _deathState = new MegamothlingMovementDeathState(this, _speed, _radius, _verticalAmplitude);
        _spreadState = new MothlingMovementSpreadState(this, _speed, _radius, _verticalAmplitude);
    }
    
    public void Play()
    {
        MovementSetup();
    }

    public void MovementReset()
    {
        _isPlaying = false;
        transform.position = IDLE_POSITION;
    }

    public override void TriggerFall()
    {
        if(_currentState.State == EnemyState.Attack)
        {
            _isCollided = true;
            SwitchState();
        }
    }

    private void MovementSetup()
    {
        _sideDirection = RandomDirection.Generate();
        SideDirection = _sideDirection;
        _depthDirection = RandomDirection.Generate();
        _position2d = GenerateSpawnPosition(-_sideDirection);
        
        _currentState = _enterState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
        _position2d = _currentState.Position;
        transform.position = _position2d;
        _isPlaying = true;
    }

    public override void TriggerDeath()
    {
        if(_currentState.State != EnemyState.Death)
        {
            _isDead = true;
            SwitchState();
        }
    }

    public override void TriggerAttack()
    {
        SwitchState();
    }

    public override void TriggerSpread()
    {
        if(_currentState.State != EnemyState.Attack && 
           _currentState.State != EnemyState.PreAttack && 
           _currentState.State != EnemyState.Death)
        {
            _currentState = _spreadState;
            _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
        }
    }

    public override void TriggerStick()  { }

    public override void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState;
        switch (_currentState.State)
        {
            case EnemyState.Enter:
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
            case EnemyState.Patrol:
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
            case EnemyState.PreAttack:
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
                    OnBossAttackStartedEvent?.Invoke();
                    break;    
                }
            case EnemyState.Attack:
                if (_isCollided && !_isDead)
                {
                    newState = _fallState;
                    _isCollided = false;
                    OnAttackEndInvoke();
                    break;
                }
                else if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    OnAttackEndInvoke();
                    break;
                }
                else
                {
                    break;    
                }
            case EnemyState.Fall:
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
            case EnemyState.Spread:
                MovementReset();
                return;
            case EnemyState.Death:
                newState = _patrolState;
                DeathStateEnded?.Invoke();
                break;
        }

        _currentState = newState;
        State = _currentState.State;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
        SideDirection = _sideDirection;
    }

    private Vector3 GenerateSpawnPosition(int direction)
    {
        Vector3 spawnPosition = (Vector3)(Random.insideUnitCircle * _spawnAreaSize) + _spawnAreaCenter;
        spawnPosition.x *= direction;
        return spawnPosition;
    }

    private void Update()
    {
        if (!_isPlaying)
        {
            return;
        }
        
        _prevPosition2d = _position2d;
        
        // Debug Only
        _prevPosSmooth = transform.position;
        _prevPosition = _position;
       
        _movementStateMachine.Execute(_position2d);
        _position2d = _currentState.Position;
        _position = _position2d;
        
        // Add Noise
        if (_isNoiseEnabled)  
        {
            Vector3 trajectoryNoise1 = TrajectoryNoise.Generate(_noise1Frequency);
            Vector3 trajectoryNoise2 = TrajectoryNoise.Generate(_noise2Frequency);
            
            if (_currentState.State == EnemyState.Attack)
            {
                float noiseMultiplier = 0.5f;
                if (_position2d.magnitude < 0.8f)
                {
                    noiseMultiplier = 0.001f;
                }   
                trajectoryNoise1 *= noiseMultiplier;
                trajectoryNoise2 *= noiseMultiplier;
            }

            if (State == EnemyState.Death)
            {
                trajectoryNoise1 *= 0.1f;
                trajectoryNoise2 *= 0.25f;
            }
           
            _position = _position2d + trajectoryNoise1 * _noise1Amplitude + trajectoryNoise2 * _noise2Amplitude;
        }
        
        // Add Depth
        if (_isDepthEnabled)
        {
            _position += _currentState.Depth;
        }
        
        // Add SmoothDamp
        if (_isSmoothDampEnabled)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _position, ref _velocity, _smoothTime);
        }
        else
        {
            transform.position = _position;
        }
        
        _movementStateMachine.CheckForStateChange();
        Debug.DrawLine(_prevPosition, _prevPosition + (_position-_prevPosition).normalized*0.02f, Color.cyan, 5f);
        Debug.DrawLine(_prevPosSmooth, _prevPosSmooth + (transform.position-_prevPosSmooth).normalized*0.02f, Color.yellow, 5f);
        _stateDebug = _currentState.State;
    }
}
