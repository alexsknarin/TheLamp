using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LadybugMovement : MonoBehaviour, IStateMachineOwner, IPreAttackStateProvider
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    private int _sideDirection;

    private Vector3 _position2d;
    
    // Movement States
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private LadybugMovementPatrolState _patrolState;
    private LadybugMovementPreAttackState _preAttackState;
    private LadybugMovementAttackState _attackState;
    private LadybugMovementStickState _stickState;
    
    
    private int _depthDirection;
    private Vector3 _prevPosition2d;

    public event Action OnPreAttackStart;
    public event Action OnPreAttackEnd;
    
    public void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState.State switch
        {
            EnemyStates.Patrol => _preAttackState,
            EnemyStates.PreAttack => _attackState,
            EnemyStates.Attack => _stickState,
            // EnemyStates.Fall => _hoverState,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _currentState = newState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
    }

    private void Start()
    {
        Init();
    }
    
    private Vector3 GenerateSpawnPosition(float distance)
    {
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = distance;
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        spawnPosition = rotation * spawnPosition;
        return spawnPosition;
    }
    
    private void Spawn()
    {
        _position2d = GenerateSpawnPosition(_radius);
    }

    private void Init()
    {
        _sideDirection = RandomDirection.Generate();
        _depthDirection = RandomDirection.Generate();
        
        _movementStateMachine = new EnemyMovementStateMachine();
        _patrolState  = new LadybugMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new LadybugMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new LadybugMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickState = new LadybugMovementStickState(this, _speed, _radius, _verticalAmplitude);

        Spawn();
       
        _currentState = _patrolState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }

    private void Update()
    {
        _prevPosition2d = _position2d;
        _currentState.ExecuteState(_position2d);
        _position2d = _currentState.Position;

        _movementStateMachine.CheckForStateChange();
        transform.position = _position2d;
        
        Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d-_prevPosition2d).normalized*0.02f, Color.cyan, 5f);
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_currentState.State == EnemyStates.Attack && other.CompareTag("LadybugStickTrigger"))
        {
            SwitchState();
        }
    }
}
