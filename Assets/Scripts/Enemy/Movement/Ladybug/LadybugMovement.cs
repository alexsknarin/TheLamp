using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LadybugMovement : EnemyMovement
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private bool _isDepthEnabled;

    // Movement States
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private LadybugMovementPatrolState _patrolState;
    private LadybugMovementPreAttackState _preAttackState;
    private LadybugMovementAttackState _attackState;
    private LadybugMovementStickState _stickState;
    private LadybugMovementFallState _fallState;
    private LadybugMovementDeathState _deathState;
    
    private int _sideDirection;
    private int _depthDirection;
    private Vector3 _position2d;
    private Vector3 _prevPosition2d;
    
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
        _patrolState  = new LadybugMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new LadybugMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new LadybugMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickState = new LadybugMovementStickState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new LadybugMovementFallState(this, _speed, _radius, _verticalAmplitude);
        _deathState = new LadybugMovementDeathState(this, _speed, _radius, _verticalAmplitude);

        Spawn();
       
        _currentState = _patrolState;
        
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }
    
    private void Spawn()
    {
        _position2d = GenerateSpawnPosition(_radius);
    }
    
    private Vector3 GenerateSpawnPosition(float distance)
    {
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = distance;
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        spawnPosition = rotation * spawnPosition;
        return spawnPosition;
    }
    
    public override void TriggerFall()
    {
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
    }
    
    public override void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState;
        switch (_currentState.State)
        {
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
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    newState = _stickState;
                    _isCollided = false;
                    break;    
                }
            case EnemyStates.Stick:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                }
                break;
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
        _currentState.ExecuteState(_position2d);
        _position2d = _currentState.Position;
        
        Vector3 position = _position2d;
        
        // Add Depth
        if (_isDepthEnabled)
        {
            position = _position2d + _currentState.Depth;
        }
        
        _movementStateMachine.CheckForStateChange();
        transform.position = position;
        

        Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d-_prevPosition2d).normalized*0.02f, Color.cyan, 5f);
        _stateDebug = _currentState.State;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_currentState.State != EnemyStates.Stick && other.CompareTag("StickTrigger"))
        {
            SwitchState();
        }
    }
}
