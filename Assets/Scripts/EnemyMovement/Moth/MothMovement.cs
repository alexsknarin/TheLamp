using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MothMovement : MonoBehaviour, IStateMachineOwner
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    private int _sideDirection;
    [Header("---- Spawn Settings ----")]
    [SerializeField] private float _spawnXPos;
    [SerializeField] private float _spawnYPosMin;
    [SerializeField] private float _spawnYPosMax;
    [Header("---- Depth Settings ----")]
    [SerializeField] bool _isDepthEnabled;
    private int _depthDirection;

    // Movement Stats
    private EnemyMovementStateMachine _movementStateMachine;
    private MothMovementBaseState _currentState;
    private MothMovementPatrolState _patrolState;
    private MothMovementEnterState _enterState;
    private MothMovementHoverState _hoverState;
    private MothMovementPreAttackState _preAttackState;
    private MothMovementAttackState _attackState;
    private MothMovementFallState _fallState;
    
    private Vector3 _prevPosition2d; //Debug
    private Vector3 _position2d;
    private Vector3 _position;
    
    // Events
    public event Action OnPreAttackStart;
    public event Action OnPreAttackEnd;

    private void Start()
    {
        Init();
    }
    
    private Vector3 GenerateSpawnPosition(int direction, float xPos, float yPosMin, float yPosMax)
    {
        Vector3 spawnPositionSide = Vector3.zero;
        spawnPositionSide.x = xPos * direction;
        spawnPositionSide.y = Random.Range(yPosMin, yPosMax) * RandomDirection.Generate();
        
        Vector3 spawnPositionTopBottom = Vector3.zero;
        spawnPositionTopBottom.x = Random.Range(-xPos, xPos);
        spawnPositionTopBottom.y = yPosMax * RandomDirection.Generate();
        
        if (Random.Range(0, 2) == 0)
        {
            return spawnPositionSide;
        }
        else
        {
            return spawnPositionTopBottom;
        }
    }
    
    private void Spawn()
    {
        _position2d = GenerateSpawnPosition(_sideDirection, _spawnXPos, _spawnYPosMin, _spawnYPosMax);
    }

    private void Init()
    {
        _sideDirection = RandomDirection.Generate();
        _depthDirection = RandomDirection.Generate();
        
        _movementStateMachine = new EnemyMovementStateMachine();
        _patrolState  = new MothMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _enterState = new MothMovementEnterState(this, _speed, _radius, _verticalAmplitude);
        _hoverState = new MothMovementHoverState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new MothMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new MothMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new MothMovementFallState(this, _speed, _radius, _verticalAmplitude);
        
        Spawn();
       
        _currentState = _enterState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }
    
    // Extract to the Interface
    public void SwitchState()
    {
        MothMovementBaseState newState = _currentState.State switch
        {
            EnemyStates.Enter => _hoverState,
            EnemyStates.Hover => _patrolState,
            EnemyStates.Patrol => _hoverState,
            EnemyStates.PreAttack => startAttackState(),
            EnemyStates.Attack => _fallState,
            EnemyStates.Fall => _hoverState,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _currentState = newState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
    }
    
    private MothMovementAttackState startAttackState()
    {
        OnPreAttackEnd?.Invoke();
        return _attackState;
    }
    
    private void Update()
    {
        _prevPosition2d = _position2d;
        
        _movementStateMachine.Execute(_position2d);
        _position2d = _currentState.Position;
        
        
        _position = _position2d;

        // Add Depth
        if (_isDepthEnabled)
        {
            _position = _position2d + _currentState.Depth;
        }
        
        
        
        _movementStateMachine.CheckForStateChange();
        transform.position = _position;
        
        Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d-_prevPosition2d).normalized*0.02f, Color.cyan, 5f);
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (_currentState.State == EnemyStates.Hover)
            {
                _currentState = _preAttackState;
                _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
                OnPreAttackStart?.Invoke();
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
