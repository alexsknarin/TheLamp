using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MegabeetleMovement : EnemyMovement
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private bool _isDepthEnabled;
    [SerializeField] private Vector3 IDLE_POSITION; // For Debug
    [SerializeField] private int _sideDirection; // For Debug
    [SerializeField] private EnemyState _stateDebug; // For Debug
    private ILampDeadEventProviderService _lampDeadEventProvider;
    // Movement States
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private MegabeetleMovementEnterState _enterState;
    private MegabeetleMovementPatrolState _patrolState;
    private LadybugMovementPreAttackState _preAttackState;
    private LadybugMovementAttackState _attackState;
    private MegabeetleMovementStickState _stickState;
    private MegabeetleMovementStickPreAttackState _stickPreAttackState;
    private MegabeetleMovementStickPreAttackPauseState _stickPreAttackPauseState;
    private MegabeetleMovementStickAttackState _stickAttackState;
    private MegabeetleMovementStickLandingState _stickLandingState;
    private MegabeetleMovementFallState _fallState;
    private MegabeetleMovementDeathState _deathState;
    private LadybugMovementSpreadState _spreadState;
    private int _depthDirection;
    private Vector3 _position2d;
    private Vector3 _prevPosition2d;
    // State parameters
    private bool _isDead = false;
    private bool _isFalling = false;
    private bool _isSpreading = false;
    
    public void Construct(ILampDeadEventProviderService lampDeadEventProviderService)
    {
        _lampDeadEventProvider = lampDeadEventProviderService;
    }
    
    public event Action DeathStateEnded;
    public event Action StickAttackStateEnded;
    public event Action SpreadTriggered;

    public override void Initialize()
    {
        _isDead = false;
        _isFalling = false;
        _isSpreading = false;
        enabled = false;
        
        _movementStateMachine = new EnemyMovementStateMachine();
        _enterState = new MegabeetleMovementEnterState(this, _speed, _radius, _verticalAmplitude);
        _patrolState  = new MegabeetleMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new LadybugMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new LadybugMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickLandingState = new MegabeetleMovementStickLandingState(this, _speed, _radius, _verticalAmplitude);
        _stickState = new MegabeetleMovementStickState(this, _speed, _radius, _verticalAmplitude);
        _stickPreAttackState = new MegabeetleMovementStickPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickPreAttackPauseState = new MegabeetleMovementStickPreAttackPauseState(this, _speed, _radius, _verticalAmplitude);
        _stickAttackState = new MegabeetleMovementStickAttackState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new MegabeetleMovementFallState(this, _speed, _radius, _verticalAmplitude);
        _deathState = new MegabeetleMovementDeathState(this, _speed, _radius, _verticalAmplitude);
        _spreadState = new LadybugMovementSpreadState(this, _speed, _radius, _verticalAmplitude);
        
        _lampDeadEventProvider.LampDestroyed += OnLampDied;
    }

    private void OnDestroy()
    {
        _lampDeadEventProvider.LampDestroyed -= OnLampDied;
    }

    public void Play()
    {
        MovementSetup();
    }

    public void MovementReset()
    {
        enabled = false;
        transform.position = IDLE_POSITION;
        OnMovementResetInvoke();
    }

    public override void TriggerFall()
    {
        if(_currentState.State != EnemyState.Fall)
        {
            _isFalling = true;
            SwitchState();
        }
    }

    public override void TriggerDeath()
    {
        if(_currentState.State != EnemyState.Death)
        {
            _isDead = true;
            SwitchState();
        }
    }

    public override void TriggerAttack(){}

    public override void TriggerSpread(){}

    public override void TriggerStick()
    {
        if (_currentState.State != EnemyState.Stick)
        {
            SwitchState();
        }
    }

    public override void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState;
        switch (_currentState.State)
        {
            case EnemyState.Enter:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else if (_isSpreading)
                {
                    _isSpreading = false;
                    newState = _spreadState;
                }
                else
                {
                    OnPreAttackStartInvoke();
                    newState = _preAttackState;
                }
                break;
            case EnemyState.Patrol:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                }
                else if (_isSpreading)
                {
                    _isSpreading = false;
                    newState = _spreadState;
                }
                else
                {
                    OnPreAttackStartInvoke();
                    newState = _preAttackState;
                    SpreadTriggered?.Invoke();
                }
                break;
            case EnemyState.PreAttack:
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                    break;
                }
                else if (_isSpreading)
                {
                    _isSpreading = false;
                    newState = _spreadState;
                }
                else
                {
                    OnPreAttackEndInvoke();
                    newState = _attackState;
                       
                }
                break; 
            case EnemyState.Attack:
                OnAttackEndInvoke();
                if (_isDead)
                {
                    newState = _deathState;
                    _isDead = false;
                }
                else if (_isSpreading)
                {
                    _isSpreading = false;
                    newState = _spreadState;
                }
                else
                {
                    newState = _stickLandingState;
                    _position2d = transform.localPosition;
                    OnStickStartInvoke();
                    SpreadTriggered?.Invoke();
                }
                break;
            case EnemyState.StickLanding:
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
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    newState = _stickState;
                    _position2d = transform.localPosition;
                    OnStickStartInvoke();
                        
                }
                break;
            case EnemyState.Stick:
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
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    newState = _stickPreAttackState;
                    _position2d = transform.localPosition;
                }
                break;
            case EnemyState.StickPreAttack:
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
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    OnPreAttackStartInvoke();
                    SpreadTriggered?.Invoke();
                    newState = _stickPreAttackPauseState;
                    _position2d = transform.localPosition;
                }
                break;
            case EnemyState.StickPreAttackPause:
                if (_isDead)
                {
                    OnPreAttackEndInvoke();
                    newState = _deathState;
                    _isDead = false;
                }
                else if (_isFalling)
                {
                    OnPreAttackEndInvoke();
                    newState = _fallState;
                    transform.parent = null;
                    _position2d = transform.position;
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    OnPreAttackEndInvoke();
                    newState = _stickAttackState;
                    _position2d = transform.localPosition;
                }
                break;
            case EnemyState.StickAttack:
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
                    _isFalling = false;
                    _sideDirection = RandomDirection.Generate();
                }
                else
                {
                    StickAttackStateEnded?.Invoke();
                    SpreadTriggered?.Invoke();
                    newState = _stickState;
                    _position2d = transform.localPosition;
                }
                break;
            case EnemyState.Fall:
                newState = _patrolState;
                break;
            case EnemyState.Spread:
                MovementReset();
                return;
            case EnemyState.Death:
                DeathStateEnded?.Invoke();
                return;
        }
        
       
        _currentState = newState;
        State = _currentState.State;
        _stateDebug = _currentState.State;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
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
        enabled = true;
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

    private IEnumerator FallDelayedStart()
    {
        yield return null;
        _isFalling = true;
        SwitchState();
    }


    private IEnumerator SpreadDelayedStart()
    {
        yield return null;
        _isSpreading = true;
        SwitchState();
    }

    private void Update()
    {
        _prevPosition2d = _position2d;
        _currentState.ExecuteState(_position2d);
        _position2d = _currentState.Position;
        
        if ((_currentState.State == EnemyState.StickLanding) ||
            (_currentState.State == EnemyState.Stick) ||
            (_currentState.State == EnemyState.StickPreAttack) ||
            (_currentState.State == EnemyState.StickAttack) ||
            (_currentState.State == EnemyState.StickPreAttackPause))
        {
            transform.localPosition = _position2d;
            _movementStateMachine.CheckForStateChange();
           
            Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d - _prevPosition2d).normalized * 0.02f, Color.cyan, 5f);
            return;
        }
        
        Vector3 position = _position2d;
        // Add Depth
        if (_isDepthEnabled)
        {
            position = _position2d + _currentState.Depth;
        }

        transform.position = position;
        _movementStateMachine.CheckForStateChange();
        
        Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d - _prevPosition2d).normalized * 0.02f, Color.cyan, 5f);
    }

    // Event Handler Methods
    private void OnLampDied()
    {
        if (enabled)
        {
            transform.parent = null;
            if (_currentState.State == EnemyState.StickLanding ||
                _currentState.State == EnemyState.Stick ||
                _currentState.State == EnemyState.StickAttack ||
                _currentState.State == EnemyState.StickPreAttack ||
                _currentState.State == EnemyState.StickPreAttackPause)
            {
                StartCoroutine(FallDelayedStart());
            }
            else
            {
                StartCoroutine(SpreadDelayedStart());
            }
        }
    }
}
