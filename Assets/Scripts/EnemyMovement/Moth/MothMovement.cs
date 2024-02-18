using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MothMovement : MonoBehaviour
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
    
    private int _depthDirection;

    // Movement Stats
    private EnemyMovementStateMachine _movementStateMachine;
    private MothMovementBaseState _currentState;
    private MothMovementPatrolState _patrolState;
    private MothMovementEnterState _enterState;
    private MothMovementHoverState _hoverState;
    
    private Vector3 _prevPosition2d; //Debug
    private Vector3 _position2d;
    // private Vector3 _position;

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
            // EnemyStates.PreAttack => startAttackState(),
            // EnemyStates.Attack => _fallState,
            // EnemyStates.Fall => ReturnToPatrol(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _currentState = newState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
    }

    private void Update()
    {
        _prevPosition2d = _position2d;
        
        _movementStateMachine.Execute(_position2d);
        _position2d = _currentState.Position;
        
        transform.position = _position2d;
        
        _movementStateMachine.CheckForStateChange();
        
        Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d-_prevPosition2d).normalized*0.02f, Color.cyan, 5f);
    }
}
