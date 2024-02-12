using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseMovement : MonoBehaviour
{
    private int _sideDirection;
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    
    private EnemyMovementStateMachine _movementStateMachine;
    
    private EnemyMovementBaseState _currentState;
    
    private EnemyMovementEnterState _enterState;
    private EnemyMovementPatrolState _patrolState;
    private EnemyMovementAttackState _attackState;
    


    private void Start()
    {
        Init();
    }    

    private void Init()
    {
        _sideDirection = RandomDirection.Generate();
        _movementStateMachine = new EnemyMovementStateMachine();
        
        _enterState = new EnemyMovementEnterState(this, _sideDirection, _speed, _radius, _verticalAmplitude);
        _patrolState  = new EnemyMovementPatrolState(this, _sideDirection, _speed, _radius, _verticalAmplitude);
        _attackState = new EnemyMovementAttackState(this, _sideDirection, _speed, _radius, _verticalAmplitude);
        
        _currentState = _enterState;
        
        _movementStateMachine.SetState(_currentState);
        transform.position = _currentState.Position;
    }
    
    public void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState.State switch
        {
            EnemyStates.Enter => _patrolState,
            EnemyStates.Patrol => _attackState,
            EnemyStates.Attack => _enterState,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _currentState = newState;
        _movementStateMachine.SetState(_currentState);
        Debug.Log("State Changed -------------------------");
    }

    private void Update()
    {   
        Debug.Log(_currentState.State);
        _movementStateMachine.Execute(transform.position);
        transform.position = _currentState.Position;
        _movementStateMachine.CheckForStateChange();
    }
}
