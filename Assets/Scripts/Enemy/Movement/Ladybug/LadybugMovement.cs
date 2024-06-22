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
    private LadybugMovementDeathState _deathState;
    private LadybugMovementSpreadState _spreadState;
    private int _sideDirection;
    private int _depthDirection;
    private Vector3 _position2d;
    private Vector3 _prevPosition2d;
    // State parameters
    private bool _isDead = false;
    private bool _isCollided = false;
    
    // Debug
    [SerializeField] private EnemyStates _stateDebug;

    private void OnEnable()
    {
        Lamp.OnLampDead += FallOnLampDestroyed;
    }
    
    private void OnDisable()
    {
        Lamp.OnLampDead -= FallOnLampDestroyed;
    }

    public override void Initialize()
    {
        _isDead = false;
        _isCollided = false;
        _movementStateMachine = new EnemyMovementStateMachine();
        _patrolState  = new LadybugMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new LadybugMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new LadybugMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickState = new LadybugMovementStickState(this, _speed, _radius, _verticalAmplitude);
        _deathState = new LadybugMovementDeathState(this, _speed, _radius, _verticalAmplitude);
        _spreadState = new LadybugMovementSpreadState(this, _speed, _radius, _verticalAmplitude);
        MovementSetup();
    }

    private void MovementSetup()
    {
        _sideDirection = RandomDirection.Generate();
        SideDirection = _sideDirection;
        _depthDirection = RandomDirection.Generate();
        _position2d = GenerateSpawnPosition(_radius);
        _currentState = _patrolState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }
    
    private void MovementReset()
    {
        OnMovementResetInvoke();
        MovementSetup();    
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

    public void FallOnLampDestroyed(EnemyBase enemy)
    {
        transform.parent = null;
        if(_currentState.State == EnemyStates.Stick)
        {
            _isDead = true;
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
    }
    
    public override void TriggerSpread()
    {
        if(_currentState.State != EnemyStates.Attack && 
           _currentState.State != EnemyStates.PreAttack && 
           _currentState.State != EnemyStates.Death &&
           _currentState.State != EnemyStates.Stick)
        {
            _currentState = _spreadState;
            _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
        }
    }
    
    public override void TriggerStick()
    {
        if (_currentState.State != EnemyStates.Stick)
        {
            SwitchState();
        }
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
                OnAttackEndInvoke();
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
                    OnStickStartInvoke();
                    break;    
                }
            case EnemyStates.Stick:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                }
                break;
            case EnemyStates.Spread:
                MovementReset();
                return;
            case EnemyStates.Death:
                OnEnemyDeactivatedInvoke();
                break;
        }

        _currentState = newState;
        State = _currentState.State;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
    }
    
    private void Update()
    {
        if (_currentState.State != EnemyStates.Stick)
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

            Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d - _prevPosition2d).normalized * 0.02f, Color.cyan, 5f);
            _stateDebug = _currentState.State;
        }
    }
}
