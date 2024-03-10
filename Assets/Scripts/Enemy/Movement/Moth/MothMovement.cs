using UnityEngine;
using Random = UnityEngine.Random;

public class MothMovement : EnemyMovement
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

    // Movement States
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private MothMovementPatrolState _patrolState;
    private MothMovementEnterState _enterState;
    private MothMovementHoverState _hoverState;
    private MothMovementPreAttackState _preAttackState;
    private MothMovementAttackState _attackState;
    private MothMovementFallState _fallState;
    private MothMovementDeathState _deathState;
    
    private Vector3 _prevPosition2d; //Debug
    private Vector3 _position2d;
    private Vector3 _position;
    
    // State parameters
    private bool _isDead = false;
    private bool _isCollided = false;
    private bool _isAttacking = false;
    
    // Debug
    [SerializeField] private EnemyStates _stateDebug;

    
    public override void Initialize()
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
        _deathState = new MothMovementDeathState(this, _speed, _radius, _verticalAmplitude);
        
        Spawn();
       
        _currentState = _enterState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }
    
    private void Spawn()
    {
        _position2d = GenerateSpawnPosition(_sideDirection, _spawnXPos, _spawnYPosMin, _spawnYPosMax);
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
   
    public override void TriggerFall()
    {
        if(_currentState.State == EnemyStates.Attack)
        {
            _isCollided = true;
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
        _isAttacking = true;
        SwitchState();
    }

    public override void TriggerStick()
    {
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
                    newState = _hoverState;
                    break;
                }
                
            case EnemyStates.Hover:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else if (_isAttacking)
                {
                    OnPreAttackStartInvoke();
                    newState = _preAttackState;
                    _isAttacking = false;
                    break;
                }
                else
                {
                    newState = _patrolState;
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
            case EnemyStates.Patrol:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    newState = _hoverState;
                    break;    
                }
            case EnemyStates.Attack:
                if (_isCollided && !_isDead)
                {
                    newState = _fallState;
                    _isCollided = false;
                    break;
                }
                else if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    break;    
                }
            case EnemyStates.Fall:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else
                {
                    _sideDirection = -(int)Mathf.Sign(_position2d.x);
                    newState = _hoverState;
                    break;
                }
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
        _stateDebug = _currentState.State;
    }
}
