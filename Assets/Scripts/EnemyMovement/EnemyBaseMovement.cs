using System;
using UnityEngine;
using Random = UnityEngine.Random;

enum NoiseType
{
    WorldSpace,
    AlongTrajectory
}

public class EnemyBaseMovement : MonoBehaviour
{
    private int _sideDirection;
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private float _spawnAreaSize = 0.5f;
    [SerializeField] private Vector3 _spawnAreaCenter;
    
    [SerializeField] private bool _isNoiseEnabled;
    [SerializeField] private float _noiseFrequency;
    [SerializeField] private float _noiseAmplitude;
    [SerializeField] private NoiseType _noiseType;
    
    [SerializeField] private float _smoothTime = .3f;
    private Vector3 _velocity = Vector3.zero;
    
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private EnemyMovementEnterState _enterState;
    private EnemyMovementPatrolState _patrolState;
    private EnemyMovementAttackState _attackState;
    private EnemyMovementPreAttackState _preAttackState;
    private EnemyMovementFallState _fallState;
    
    //Debug
    private Vector3 _prevPos;
    private Vector3 _prevPosSmooth;
    private Vector3 _position;
    


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
        _movementStateMachine = new EnemyMovementStateMachine();
        
        _enterState = new EnemyMovementEnterState(this, _speed, _radius, _verticalAmplitude);
        _patrolState  = new EnemyMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new EnemyMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new EnemyMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new EnemyMovementFallState(this, _speed, _radius, _verticalAmplitude);
        
        Spawn();
        
        _currentState = _enterState;
        
        _movementStateMachine.SetState(_currentState, transform.position, _sideDirection);
        _position = _currentState.Position;
        transform.position = _position;
    }
    
    private void Spawn()
    {
        _position = GenerateSpawnPosition(-_sideDirection);
    }
    
    public void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState.State switch
        {
            EnemyStates.Enter => _patrolState,
            EnemyStates.Patrol => _preAttackState,
            EnemyStates.PreAttack => _attackState,
            EnemyStates.Attack => _fallState,
            EnemyStates.Fall => ReturnToPatrol(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _currentState = newState;
        _movementStateMachine.SetState(_currentState, _position, _sideDirection);
    }

    private EnemyMovementEnterState ReturnToPatrol()
    {
        _sideDirection = -(int)Mathf.Sign(_position.x);
        return _enterState;
    }

    private void Update()
    {   
        _prevPos = _position;
        _prevPosSmooth = transform.position;
        
        _movementStateMachine.Execute(_position);
        _position = _currentState.Position;

        
        // Add Noise
        if (_isNoiseEnabled && _currentState.State == EnemyStates.Patrol)  
        {
            Vector3 noiseValue = Vector3.zero;
            noiseValue.x = Mathf.PerlinNoise(_noiseFrequency * Time.time, 0) * 2 - 1;
            noiseValue.y = Mathf.PerlinNoise(0, _noiseFrequency * Time.time) * 2 - 1;

            if (_noiseType == NoiseType.WorldSpace)
            {
                _position += noiseValue * _noiseAmplitude;
            }
            else if (_noiseType == NoiseType.AlongTrajectory)
            {
                _position += transform.position.normalized * (noiseValue.x * _noiseAmplitude);
            }
        }
        
        // Add SmoothDamp
        if (_currentState.State == EnemyStates.Attack)
        {
            transform.position = _position;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, _position, ref _velocity, _smoothTime);;    
        }
        
        _movementStateMachine.CheckForStateChange();
        
        Debug.DrawLine(_prevPos, _prevPos + (_position-_prevPos).normalized*0.02f, Color.cyan, 5f);
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
