using System;
using UnityEngine;
using Random = UnityEngine.Random;

enum NoiseType
{
    WorldSpace,
    AlongTrajectory
}

public class FlyMovement : MonoBehaviour, IStateMachineOwner
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
    [SerializeField] private NoiseType _noiseType;
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
    
    
    
    private Vector3 _prevPosition2d; //Debug
    private Vector3 _prevPosSmooth; //Debug
    private Vector3 _position2d;
    private Vector3 _position;
    
    // Events
    public event Action OnPreAttackStart;
    public event Action OnPreAttackEnd;

    private void Start()
    {
        Init();
    }
 
    private Vector3 GenerateSpawnPosition(int direction)
    {
        Vector3 spawnPosition = (Vector3)(Random.insideUnitCircle * _spawnAreaSize) + _spawnAreaCenter;
        spawnPosition.x *= direction;
        return spawnPosition;
    }

    private void Init()
    {
        _sideDirection = RandomDirection.Generate();
        Debug.Log("Side Direction: " + _sideDirection.ToString());
        _depthDirection = RandomDirection.Generate();
        Debug.Log("Depth Direction: " + _depthDirection.ToString());
        
        _movementStateMachine = new EnemyMovementStateMachine();
        
        _enterState = new FlyMovementEnterState(this, _speed, _radius, _verticalAmplitude);
        _patrolState  = new FlyMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new FlyMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new FlyMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new FlyMovementFallState(this, _speed, _radius, _verticalAmplitude);
        
        Spawn();
        
        _currentState = _enterState;
        
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }
    
    private void Spawn()
    {
        _position2d = GenerateSpawnPosition(-_sideDirection);
    }
    
    // Extract to the Interface
    public void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState.State switch
        {
            EnemyStates.Enter => _patrolState,
            EnemyStates.Patrol => startPreAttackState(),
            EnemyStates.PreAttack => startAttackState(),
            EnemyStates.Attack => _fallState,
            EnemyStates.Fall => ReturnToPatrol(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _currentState = newState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
    }

    // Transition Handlers
    private FlyMovementEnterState ReturnToPatrol()
    {
        _sideDirection = -(int)Mathf.Sign(_position2d.x);
        return _enterState;
    }
    
    private FlyMovementPreAttackState startPreAttackState()
    {
        OnPreAttackStart?.Invoke();
        return _preAttackState;
    }
    
    private FlyMovementAttackState startAttackState()
    {
        OnPreAttackEnd?.Invoke();
        return _attackState;
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
        
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentState.State == EnemyStates.Patrol)
            {
                SwitchState();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_currentState.State == EnemyStates.Attack)
        {
            SwitchState();
        }
    }
}
