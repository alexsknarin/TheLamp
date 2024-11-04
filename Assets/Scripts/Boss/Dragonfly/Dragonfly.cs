using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Dragonfly : EnemyBase
{
    [SerializeField] private string _stateDebug;
    [SerializeField] private EnemyTypes _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private FDragonflyMovement _movement;
    [SerializeField] private DragonflyPresentation _presentation;
    [SerializeField] private DragonflySwarm _swarm;
    [SerializeField] private DragonflyCollisionController _collisionController;
    [SerializeField] private Transform _visibleBodyTransform;
    [Header("Hover")]
    [SerializeField] private float _hoverWaitMin;
    [SerializeField] private float _hoverWaitMax;

    [Header("Patrol")]
    [SerializeField] private float _swarmAttackDuration; // TODO: control swarm duration itself from here as well
    [SerializeField] private float _patrolWaitMin;
    [SerializeField] private float _patrolWaitMax;
    [SerializeField] private DragonflyPatrolAttackZoneRanges _patrolAttackZonesL;
    [SerializeField] private DragonflyPatrolAttackZoneRanges _patrolAttackZonesR;
    [SerializeField] private float _patrolTailWaitMin;
    [SerializeField] private float _patrolTailWaitMax;
    [SerializeField] private Vector3 _tailAttackZoneLMin;
    [SerializeField] private Vector3 _tailAttackZoneLMax;
    [SerializeField] private Vector3 _tailAttackZoneRMin;
    [SerializeField] private Vector3 _tailAttackZoneRMax;

    [Header("Spider")]
    [SerializeField] private Vector3 _spiderAttackPositionBase;
    [SerializeField] private float _spiderPatrolWaitMin;
    [SerializeField] private float _spiderPatrolWaitMax;
    [SerializeField] private DragonflyProjectileSpider _spider;
    public override EnemyTypes EnemyType => _enemyType;
    private DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
        
    private Vector3 _patrolAttackPosition;
    private Vector3 _patrolSpiderAttackPosition;
    
    private DragonflyReturnMode[] RETURN_MODES = new DragonflyReturnMode[]
    {
        DragonflyReturnMode.PatrolL,
        DragonflyReturnMode.PatrolR,
        DragonflyReturnMode.SpiderL,
        DragonflyReturnMode.SpiderR,
        DragonflyReturnMode.Hover,
        DragonflyReturnMode.Hover
    };

    // STATE MACHINE 
    private readonly FStateMachine _stateMachine = new FStateMachine();
    private DragonflyInactiveState _inactiveState;
    private DragonflyPassiveState _passiveState;
    private DragonflyPatrolState _patrolState;
    private DragonflyHoverState _hoverState;
    private DragonflyPatrolHeadState _patrolHeadState;
    private DragonflyPatrolTailState _patrolTailState;
    private DragonflyWaitHeadAttackState _waitHeadAttackState;
    private DragonflyWaitTailAttackState _waitTailAttackState;
    private DragonflyWaitHoverAttackState _waitHoverAttackState;
    private DragonflySpiderEnterState _spiderEnterState;
    private DragonflyPatrolSpiderState _patrolSpiderState;
    private DragonflyWaitSpiderAttackState _waitSpiderAttackState;
    private DragonflySwarmAttackState _swarmAttackState;
    private DragonflyWaitForBounceState _waitForBounceState;

    private bool _isActivated = false;
    private DragonflyEnterType _enterType = 0;
    private DragonflyPatrolAttackMode _patrolAttackMode = DragonflyPatrolAttackMode.Head;
    private bool _isReadyToPreAttackWait = false;
    private bool _isReadyToAttackWait = false;
    private bool _isAttacked = false;
    private bool _isDead = false;
    private DragonflyReturnMode _returnMode;
    
    [SerializeField] private bool _readyToLampDamage;
    
    // TODO: for refactor
    [SerializeField] private bool _isInAttackExitZone = false;
    [SerializeField] private bool _isCollidedWithLamp = false;
    
    private void OnEnable()
    {
        _movement.OnReadyToAttackStateEntered += OnReadyToAttackEnterHandle;
        _movement.OnReadyToSwarmAttackStateEntered += OnReadyToSwarmAttackEnterHandle;
        _movement.OnPreattackStarted += OnPreAttackStartHandle;
        _movement.OnAttackStarted += OnAttackStartedHandle;
        _movement.OnAttackEnded += OnAttackEndedHandle;
        _patrolHeadState.OnEnded += GenerateAttackPosition;
        _patrolTailState.OnEnded += GenerateAttackPosition;
        _patrolSpiderState.OnEnded += GenerateAttackPosition;
        _waitHeadAttackState.OnEnded += StartAttack;
        _waitTailAttackState.OnEnded += StartAttack;
        _waitHoverAttackState.OnEnded += StartAttack;
        _waitSpiderAttackState.OnReadyToPreAttack += StartSpiderPreAttack;
        _waitSpiderAttackState.OnEnded += StartSpiderAttack;
        
        _waitForBounceState.OnEnded += HandleBounce;
        
        _movement.OnAfterAttackExitEnded += OnAfterAttackExitEndHandle;
        _movement.OnReadyToSpiderAttackStateEntered += OnReadyToSpiderAttackEnterHandle;
        _movement.OnCatchSpiderStarted += OnCatchSpiderStartHandle;
        _spider.OnEnterAnimationEnd += OnSpiderEnterAnimationEndHandle;
        _movement.OnDeathAnimationEnded += OnDeathAnimationEndedHandle;
        
        LampAttackModel.OnLampAttack += TMPHandleLampAttack;
    }

    private void OnDisable()
    {
        
        _movement.OnReadyToAttackStateEntered -= OnReadyToAttackEnterHandle;
        _movement.OnReadyToSwarmAttackStateEntered -= OnReadyToSwarmAttackEnterHandle;
        _movement.OnPreattackStarted -= OnPreAttackStartHandle;
        _movement.OnAttackStarted -= OnAttackStartedHandle;
        _movement.OnAttackEnded -= OnAttackEndedHandle;
        
        _patrolHeadState.OnEnded -= GenerateAttackPosition;
        _patrolTailState.OnEnded -= GenerateAttackPosition;
        _patrolSpiderState.OnEnded -= GenerateAttackPosition;
        _waitHeadAttackState.OnEnded -= StartAttack;
        _waitTailAttackState.OnEnded -= StartAttack;
        _waitHoverAttackState.OnEnded -= StartAttack;
        _waitSpiderAttackState.OnReadyToPreAttack -= StartSpiderPreAttack;
        _waitSpiderAttackState.OnEnded -= StartSpiderAttack;
        
        _waitForBounceState.OnEnded -= HandleBounce;
        
        _movement.OnAfterAttackExitEnded -= OnAfterAttackExitEndHandle;
        _movement.OnReadyToSpiderAttackStateEntered -= OnReadyToSpiderAttackEnterHandle;
        _movement.OnCatchSpiderStarted -= OnCatchSpiderStartHandle;
        _spider.OnEnterAnimationEnd -= OnSpiderEnterAnimationEndHandle;
        _movement.OnDeathAnimationEnded -= OnDeathAnimationEndedHandle;
        LampAttackModel.OnLampAttack -= TMPHandleLampAttack;
    }

    private void Awake()
    {
        _patrolAttackPositionProvider = new DragonflyPatrolAttackPositionProvider(
            _patrolAttackZonesL, 
            _patrolAttackZonesR, 
            _tailAttackZoneLMin,
            _tailAttackZoneLMax,
            _tailAttackZoneRMin,
            _tailAttackZoneRMax
        );
        
        // Initialize the states
        _inactiveState = new DragonflyInactiveState();
        _passiveState = new DragonflyPassiveState();
        _patrolState = new DragonflyPatrolState();
        _hoverState = new DragonflyHoverState();
        _patrolHeadState = new DragonflyPatrolHeadState(_patrolWaitMin, _patrolWaitMax);
        _patrolTailState = new DragonflyPatrolTailState(_patrolTailWaitMin, _patrolTailWaitMax);
        _waitHeadAttackState = new DragonflyWaitHeadAttackState(_visibleBodyTransform, _patrolAttackPositionProvider, _movement);
        _waitTailAttackState = new DragonflyWaitTailAttackState(_visibleBodyTransform, _patrolAttackPositionProvider, _movement);
        _waitHoverAttackState = new DragonflyWaitHoverAttackState(_hoverWaitMin, _hoverWaitMax);
        _spiderEnterState = new DragonflySpiderEnterState();
        _patrolSpiderState = new DragonflyPatrolSpiderState(_spiderPatrolWaitMin, _spiderPatrolWaitMax);
        _waitSpiderAttackState = new DragonflyWaitSpiderAttackState(_visibleBodyTransform, _spiderAttackPositionBase);
        _swarmAttackState = new DragonflySwarmAttackState(_swarmAttackDuration);
        _waitForBounceState = new DragonflyWaitForBounceState();
        
        // Set up State Machine Transitions
        // Enter
        At(_inactiveState, _patrolState, () => _isActivated && _enterType == DragonflyEnterType.Patrol);
        At(_inactiveState, _hoverState, () => _isActivated && _enterType == DragonflyEnterType.Hover);
        // Patrol to Head/Tail attack through the swarm attack state
        At(_patrolState, _swarmAttackState, IsReadyToPatrolHead());
        At(_patrolState, _swarmAttackState, IsReadyToPatrolTail());
        At(_swarmAttackState, _patrolHeadState, () => _swarmAttackState.ReadyToSwitch 
                                                      && _patrolAttackMode == DragonflyPatrolAttackMode.Head);
        At(_swarmAttackState, _patrolTailState, () => _swarmAttackState.ReadyToSwitch 
                                                      && _patrolAttackMode == DragonflyPatrolAttackMode.Tail);
        At(_patrolHeadState, _waitHeadAttackState, IsReadyToAttackWait());
        At(_patrolTailState, _waitTailAttackState, IsReadyToAttackWait());
        // Hover to Attack        
        At(_hoverState, _waitHoverAttackState, IsReadyToPreAttackWait());
        // Exit from attacks to passive state
        At(_waitHeadAttackState, _waitForBounceState, IsAttacked());
        At(_waitTailAttackState, _waitForBounceState, IsAttacked());
        At(_waitHoverAttackState, _waitForBounceState, IsAttacked());
        
        At(_waitForBounceState, _passiveState, () => _isInAttackExitZone && _isCollidedWithLamp);
        
        // Return to patrol/hover
        At(_passiveState, _patrolState, IsReturnToPatrol());
        At(_passiveState, _hoverState, IsReturnToHover());
        At(_passiveState, _spiderEnterState, IsReturnToSpider());
        // Spider Attack
        At(_spiderEnterState, _patrolSpiderState, IsReadyToAttackWait());
        At(_patrolSpiderState, _waitSpiderAttackState, IsReadyToAttackWait());
        At(_waitSpiderAttackState, _patrolState, IsAttacked());

        _stateMachine.SetState(_inactiveState);
        _isActivated = false;
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        
        #region Transition Predicate Delegates
        Func<bool> IsReadyToPatrolHead() => () =>
        {
            if (_isReadyToPreAttackWait && _patrolAttackMode == DragonflyPatrolAttackMode.Head)
            {
                _isReadyToPreAttackWait = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsReadyToPatrolTail() => () =>
        {
            if (_isReadyToPreAttackWait && _patrolAttackMode == DragonflyPatrolAttackMode.Tail)
            {
                _isReadyToPreAttackWait = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsReadyToPreAttackWait() => () =>
        {
            if (_isReadyToPreAttackWait)
            {
                _isReadyToPreAttackWait = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsReadyToAttackWait() => () =>
        {
            if(_isReadyToAttackWait)
            {
                _isReadyToAttackWait = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAttacked() => () =>
        {
            if(_isAttacked)
            {
                _isAttacked = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsReturnToPatrol() => () =>
        {
            if (_isReadyToPreAttackWait && (_returnMode == DragonflyReturnMode.PatrolL || _returnMode == DragonflyReturnMode.PatrolR))
            {
                _isReadyToPreAttackWait = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsReturnToHover() => () =>
        {
            if (_isReadyToPreAttackWait && _returnMode == DragonflyReturnMode.Hover)
            {
                _isReadyToPreAttackWait = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsReturnToSpider() => () =>
        {
            if (_isReadyToPreAttackWait && (_returnMode == DragonflyReturnMode.SpiderR || _returnMode == DragonflyReturnMode.SpiderL))
            {
                _isReadyToPreAttackWait = false;
                return true;
            }
            return false;
        };
        #endregion
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
        _stateMachine.Tick();
        _stateDebug = _stateMachine.CurrentState.ToString();
    }

    public void Initialize()
    {
        _isDead = false;
        _isActivated = false;
        _isReadyToPreAttackWait = false;
        _isReadyToAttackWait = false;
        _isAttacked = false;
        _presentation.Initialize();
    }

    public void Play()
    {
        StartBossActivePhase();
        _presentation.Initialize();
    }

    // State change methods

    private void StartBossActivePhase()
    {
        _collisionController.DisableColliders();
        _enterType = (DragonflyEnterType)Random.Range(0, 2); // 0 Patrol, 1 Hover TODO: enum???????????
        int sideDirection = RandomDirection.Generate();
        _movement.Play(_enterType, sideDirection);
        _isActivated = true;
    }

    private void StartAttack(DragonflyPatrolAttackMode mode)
    {
        _movement.StartAttack(mode);
        _isAttacked = true;
        _isCollidedWithLamp = false;
    }

    private void GenerateAttackPosition()
    {
        _isReadyToAttackWait = true;
    }

    private void StartSpiderPreAttack()
    {
        _spider.StartPreAttack();
    }
    
    private void StartSpiderAttack()
    {
        _spider.gameObject.transform.SetParent(this.transform);
        _spider.StartAttack();
        _movement.StartAttack(DragonflyPatrolAttackMode.Spider);
        _isAttacked = true;
    }

    // Event Handle Methods
    private void OnReadyToAttackEnterHandle(IState movementState)
    {
        _patrolAttackMode = (DragonflyPatrolAttackMode)Random.Range(0, 2);
        _isReadyToPreAttackWait = true;
    }

    private void OnReadyToSwarmAttackEnterHandle(IState movementState)
    {
        if (movementState.GetType() == typeof(FDragonflyPatrolStateL))
        {
            _swarm.PlayAttack(1);
        }
        else if (movementState.GetType() == typeof(FDragonflyPatrolStateR))
        {
            _swarm.PlayAttack(-1);
        }
    }

    private void OnAttackStartedHandle()
    {
        _collisionController.EnableColliders();
        _presentation.PreAttackEnd();
    }

    private void OnAttackEndedHandle()
    {
        _collisionController.DisableColliders();
    }

    private void OnAfterAttackExitEndHandle(IState movementState)
    {
        _returnMode = RETURN_MODES[Random.Range(0, 6)];
        _movement.ResolveReturnTransition(_returnMode);
        _isReadyToPreAttackWait = true;
    }

    private void OnPreAttackStartHandle()
    {
        ReceivedLampAttack = false;
        _presentation.PreAttackStart();
    }
    
    private void OnSpiderEnterAnimationEndHandle()
    {
        _spider.gameObject.transform.SetParent(_visibleBodyTransform);
        Vector3 pos = Vector3.zero;
        pos.x = 0.012f;
        pos.y = -0.286f;
        pos.z = 0.082f;
        _spider.gameObject.transform.localPosition = pos;
    }

    private void OnCatchSpiderStartHandle(int direction)
    {
        _spider.gameObject.SetActive(true);
        _spider.Play(direction);
    }

    private void OnReadyToSpiderAttackEnterHandle()
    {
        _isReadyToAttackWait = true;
    }

    private void OnDeathAnimationEndedHandle()
    {
        gameObject.SetActive(false);
    }

    // Lamp Interaction Methods
    private void TMPHandleLampAttack(int arg1, float arg2, float arg3, float arg4)
    {
        if (ReadyToLampDamage)
        {
            ReceiveDamage(arg1);
        }
    }

    public void CatchFirstCollider()
    {
        _collisionController.SoloCollider();
    }

    public override void HandleEnteringAttackZone()
    {
        ReadyToLampDamage = true;
        _readyToLampDamage = true;
    }

    public void HandleEnteringAttackExitZone()
    {
        _isInAttackExitZone = true;
    }

    public override void HandleCollisionWithLamp()
    {
        ReadyToCollide = false;
        ReadyToLampDamage = true;
        _readyToLampDamage = true;
        _isCollidedWithLamp = true;
    }

    public void HandleExitingLampCollisionZone()
    {
        _isCollidedWithLamp = false;
    }

    public override void HandleExitingAttackExitZone()
    {
        _isInAttackExitZone = false;
        ReadyToLampDamage = false;
        _readyToLampDamage = false;
        if (!ReceivedLampAttack)
        {
            _movement.TriggerFall(false);
        }
    }

    public override void HandleCollisionWithStickZone()
    {
        Debug.LogWarning("Dragonfly penetrated collision zone");
    }

    private void HandleBounce()
    {
        _movement.TriggerBounce();
    }

    public override Vector3 ProvideImpactPoint()
    {
        return _collisionController.GetFirstActiveColliderPosition();
    }

    public override void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            ReceivedLampAttack = true;
            
            _presentation.HealthUpdate(_currentHealth, _maxHealth);
            _presentation.SetActiveColliderTransform(_collisionController.GetFirstActiveColliderTransform());
            _presentation.DamageFlash();
            _movement.TriggerFall(true);
        }
        else
        {
            if (!_isDead)
            {
                ReceivedLampAttack = true;
                _currentHealth = 0; 
                _movement.TriggerDeath(); 
                _presentation.DeathFlash();
                OnEnemyDeathInvoke(this);
                _isDead = true;
            }
        }
        
        ReadyToLampDamage = false;
        _readyToLampDamage = false;
    }


    #region Unused Enemy Base Methods
    public override void ReturnToPool()
    {
        throw new NotImplementedException();
    }

    public override void UpdateAttackAvailability()
    {
        throw new NotImplementedException();
    }

    public override void SpreadStart()
    {
        throw new NotImplementedException();
    }

    public override void StartAttack()
    {
        throw new NotImplementedException();
    }
    #endregion


}
