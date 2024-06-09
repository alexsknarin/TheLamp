using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MegabeetleMovement : EnemyMovement
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
    private MegabeetleMovementEnterState _enterState;
    private LadybugMovementPatrolState _patrolState;
    private LadybugMovementPreAttackState _preAttackState;
    private LadybugMovementAttackState _attackState;
    private LadybugMovementStickState _stickState;
    private MegabeetleMovementStickLandingState _stickLandingState;
    private LadybugMovementDeathState _deathState;
    private LadybugMovementSpreadState _spreadState;
    
    
    [SerializeField] private int _sideDirection;
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
        _enterState = new MegabeetleMovementEnterState(this, _speed, _radius, _verticalAmplitude);
        _patrolState  = new LadybugMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new LadybugMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new LadybugMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickLandingState = new MegabeetleMovementStickLandingState(this, _speed, _radius, _verticalAmplitude);
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
        _position2d = GenerateSpawnPosition(_radius, _sideDirection);
        _currentState = _enterState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }
    
    private void MovementReset()
    {
        OnMovementResetInvoke();
        MovementSetup();    
    }
    
    private Vector3 GenerateSpawnPosition(float distance, int direction)
    {
        Debug.Log("Generating spawn position: " + distance.ToString() + " " + direction.ToString());
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = distance;
        Quaternion rotation = Quaternion.Euler(0, 0, -Random.Range(22, 49));
        spawnPosition = rotation * spawnPosition;
        spawnPosition.x *= -direction;
        return spawnPosition;
    }
    
    public override void TriggerFall()
    {
    }

    public void FallOnLampDestroyed(EnemyBase enemy)
    {
        Debug.Log("Trying to unparent");
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
        Debug.Log("Boss: Stick Triggered");
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
            case EnemyStates.Enter:
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
                    Debug.Log("Switching to stick landing");
                    newState = _stickLandingState;
                    _position2d = transform.localPosition;
                    _isCollided = false;
                    OnStickStartInvoke();
                    break;    
                }
            case EnemyStates.StickLanding:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    Debug.Log("Switching to stick");
                    newState = _stickState;
                    _position2d = transform.localPosition;
                    _isCollided = false;
                    OnStickStartInvoke();
                    break;    
                }
            
            //--------------
            
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
        _stateDebug = _currentState.State;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
    }
    
    private void Update()
    {
        if (_currentState.State != EnemyStates.Stick &&
            _currentState.State != EnemyStates.StickLanding)
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
        }
        
        if (_currentState.State == EnemyStates.StickLanding)
        {
            _prevPosition2d = _position2d;
            _currentState.ExecuteState(_position2d);
            _position2d = _currentState.Position;
            
            _movementStateMachine.CheckForStateChange();
            transform.localPosition = _position2d;
            
            Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d - _prevPosition2d).normalized * 0.02f, Color.cyan, 5f);
        }
    }
}
