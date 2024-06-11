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
    private MegabeetleMovementPatrolState _patrolState;
    private LadybugMovementPreAttackState _preAttackState;
    private LadybugMovementAttackState _attackState;
    private MegabeetleMovementStickState _stickState;
    private MegabeetleMovementStickPreAttackState _stickPreAttackState;
    private MegabeetleMovementStickAttackState _stickAttackState;
    private MegabeetleMovementStickLandingState _stickLandingState;
    private MegabeetleMovementFallState _fallState;
    private MegabeetleMovementDeathState _deathState;
    private LadybugMovementSpreadState _spreadState;
    
    
    [SerializeField] private int _sideDirection;
    private int _depthDirection;
    private Vector3 _position2d;
    private Vector3 _prevPosition2d;
    
    [SerializeField] private Vector3 IDLE_POSITION; 
    
    // State parameters
    private bool _isDead = false;
    private bool _isCollided = false;
    private bool _isFalling = false;
    private bool _isPlaying = false;
    
    public event Action OnDeathStateEnded;
    
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
        _isFalling = false;
        
        _movementStateMachine = new EnemyMovementStateMachine();
        _enterState = new MegabeetleMovementEnterState(this, _speed, _radius, _verticalAmplitude);
        _patrolState  = new MegabeetleMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new LadybugMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new LadybugMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickLandingState = new MegabeetleMovementStickLandingState(this, _speed, _radius, _verticalAmplitude);
        _stickState = new MegabeetleMovementStickState(this, _speed, _radius, _verticalAmplitude);
        _stickPreAttackState = new MegabeetleMovementStickPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickAttackState = new MegabeetleMovementStickAttackState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new MegabeetleMovementFallState(this, _speed, _radius, _verticalAmplitude);
        _deathState = new MegabeetleMovementDeathState(this, _speed, _radius, _verticalAmplitude);
        _spreadState = new LadybugMovementSpreadState(this, _speed, _radius, _verticalAmplitude);
    }
    
    public void Play()
    {
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
        _isPlaying = true;
    }
    
    public void MovementReset()
    {
       _isPlaying = false;
       transform.position = IDLE_POSITION;
       OnMovementResetInvoke();
    }
    
    private Vector3 GenerateSpawnPosition(float distance, int direction)
    {
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = distance;
        Quaternion rotation = Quaternion.Euler(0, 0, -Random.Range(22, 49));
        spawnPosition = rotation * spawnPosition;
        spawnPosition.x *= -direction;
        return spawnPosition;
    }
    
    public override void TriggerFall()
    {
        if(_currentState.State != EnemyStates.Fall)
        {
            _isFalling = true;
            SwitchState();
        }
    }

    public void FallOnLampDestroyed(EnemyBase enemy)
    {
        transform.parent = null;
        if(_currentState.State == EnemyStates.StickLanding ||
           _currentState.State == EnemyStates.Stick || 
           _currentState.State == EnemyStates.StickAttack || 
           _currentState.State == EnemyStates.StickPreAttack)
        {
            _isDead = true;
            SwitchState();
        }
    }

    public override void TriggerDeath()
    {
        if(_currentState.State != EnemyStates.Death)
        {
            _isFalling = true;
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
                }
                else if (_isFalling)
                {
                    newState = _fallState;
                    transform.parent = null;
                    _position2d = transform.position;
                    _isCollided = false;
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    newState = _stickState;
                    _position2d = transform.localPosition;
                    _isCollided = false;
                    OnStickStartInvoke();
                        
                }
                break;
            case EnemyStates.Stick:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                }
                else if (_isFalling)
                {
                    newState = _fallState;
                    transform.parent = null;
                    _position2d = transform.position;
                    _isCollided = false;
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    newState = _stickPreAttackState;
                    _position2d = transform.localPosition;
                    _isCollided = false;
                }
                break;
            case EnemyStates.StickPreAttack:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                }
                else if (_isFalling)
                {
                    newState = _fallState;
                    transform.parent = null;
                    _position2d = transform.position;
                    _isCollided = false;
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    newState = _stickAttackState;
                    _position2d = transform.localPosition;
                    _isCollided = false;
                }
                break;
            case EnemyStates.StickAttack:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                }
                else if (_isFalling)
                {
                    newState = _fallState;
                    transform.parent = null;
                    _position2d = transform.position;
                    _isCollided = false;
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    newState = _stickState;
                    _position2d = transform.localPosition;
                    _isCollided = false;
                }
                break;
            case EnemyStates.Fall:
                newState = _patrolState;
                break;
            case EnemyStates.Spread:
                MovementReset();
                return;
            case EnemyStates.Death:
                OnDeathStateEnded?.Invoke();
                return;
        }
        
        _currentState = newState;
        State = _currentState.State;
        _stateDebug = _currentState.State;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
    }
    
    private void Update()
    {
        if(!_isPlaying)
        {
            return;
        }
        
        if ((_currentState.State != EnemyStates.Stick) &&
            (_currentState.State != EnemyStates.StickLanding))
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
        
        if ((_currentState.State == EnemyStates.StickLanding) ||
            (_currentState.State == EnemyStates.Stick) ||
            (_currentState.State == EnemyStates.StickPreAttack) ||
            (_currentState.State == EnemyStates.StickAttack))
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
