using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dragonfly : EnemyBase
{
    [SerializeField] private string _stateDebug;
    [SerializeField] private EnemyTypes _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    // [SerializeField] private DragonflyMovement _movement;
    [SerializeField] private FDragonflyMovement _movement;
    [SerializeField] private DragonflyPresentation _presentation;
    [SerializeField] private DragonflySwarm _swarm;
    [SerializeField] private DragonflyCollisionController _collisionController;
    [SerializeField] private Transform _visibleBodyTransform;
    [Header("Hover")]
    [SerializeField] private float _hoverWaitMin;
    [SerializeField] private float _hoverWaitMax;
    [Header("Patrol")]
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
    [SerializeField] private Vector3 _priderAttackPositionBase;
    [SerializeField] private float _spiderPatrolWaitMin;
    [SerializeField] private float _spiderPatrolWaitMax;
    [SerializeField] private DragonflyProjectileSpider _spider;
    public override EnemyTypes EnemyType => _enemyType;
    private bool _isDead = false;
    
    private float _hoverWait;
    private float _patrolHeadWait;
    private float _patrolTailWait;
    private float _patrolSpiderWait;
    [SerializeField] private float _localTime = 0;
    private bool _isWaitingForHoverAttack = false;
    private bool _isWaitingForHeadPatrolAttack = false;
    private bool _isWaitingForHeadPatrolAttackPoint = false;
    
    private bool _isWaitingForTailPatrolAttack = false;
    private bool _isWaitingForTailPatrolAttackPoint = false;
    
    private bool _isWaitingForSpiderPatrolAttack = false;
    private bool _isWaitingForSpiderPatrolAttackPoint = false;
    private int _lastPatrolDirection = 0;
    private bool _isLastPatrolDirectionSet = false;
    
    private DragonflyPatrolAttackZoneRangesData _patrolAttackZonesData = new DragonflyPatrolAttackZoneRangesData();

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
    private FStateMachine _stateMachine = new FStateMachine();
    private DragonflyInactiveState _inactiveState;
    private DragonflyPassiveState _passiveState;
    private DragonflyPatrolState _patrolState;
    private DragonflyHoverState _hoverState;
    private DragonflyPatrolHeadState _patrolHeadState;
    private DragonflyPatrolTailState _patrolTailState;
    private DragonflyWaitHeadAttackState _waitHeadAttackState;
    private DragonflyWaitTailAttackState _waitTailAttackState;
    private DragonflyWaitHoverAttackState _waitHoverAttackState;
    
    private bool _isActivated = false;
    private int _enterType = 0;
    private int _patrolAttackMode = 0;
    private bool _isReadyToPreAttackWait = false;
    private bool _isReadyToAttackWait = false;
    private bool _isAttacked = false;
    
    
    private void OnEnable()
    {
        
        _movement.OnReadyToAttackStateEntered += OnReadyToAttackEnterHandle;
        _patrolHeadState.OnEnded += GenerateAttackPosition;
        _patrolTailState.OnEnded += GenerateAttackPosition;
        _waitHeadAttackState.OnEnded += StartAttack;
        _waitTailAttackState.OnEnded += StartAttack;
        _waitHoverAttackState.OnEnded += StartAttack;
        /*
        _movement.OnReadyToSwarmAttackStateEntered += OnReadyToSwarmAttackEnterHandle;
        _movement.OnReadyToSpiderAttackStateEntered += OnReadyToSpiderAttackEnterHandle;
        _movement.OnAfterAttackExitEnded += OnAfterAttackExitEndHandle;
        _movement.OnPreattackStarted += OnPreAttackStartHandle;
        _movement.OnAttackStarted += OnAttackStartHandle;
        _movement.OnAttackEnded += OnAttackEndHandle;
        _movement.OnCatchSpiderStarted += OnCatchSpiderStartHandle;
        _movement.OnDeathAnimationEnded += OnDeathAnimationEndedHandle;
        _spider.OnEnterAnimationEnd += OnSpiderEnterAnimationEndHandle;
        */
        
        LampAttackModel.OnLampAttack += TMPHandleLampAttack;
    }

    

  

    private void OnDisable()
    {
        
        _movement.OnReadyToAttackStateEntered -= OnReadyToAttackEnterHandle;
        _patrolHeadState.OnEnded -= GenerateAttackPosition;
        _patrolTailState.OnEnded -= GenerateAttackPosition;
        _waitHeadAttackState.OnEnded -= StartAttack;
        _waitTailAttackState.OnEnded -= StartAttack;
        _waitHoverAttackState.OnEnded -= StartAttack;
        
        /*
        _movement.OnReadyToSwarmAttackStateEntered -= OnReadyToSwarmAttackEnterHandle;
        _movement.OnReadyToSpiderAttackStateEntered -= OnReadyToSpiderAttackEnterHandle;
        _movement.OnAfterAttackExitEnded -= OnAfterAttackExitEndHandle;
        _movement.OnAttackStarted -= OnAttackStartHandle;
        _movement.OnAttackEnded -= OnAttackEndHandle;
        _movement.OnPreattackStarted -= OnPreAttackStartHandle;
        _movement.OnCatchSpiderStarted -= OnCatchSpiderStartHandle;
        _movement.OnDeathAnimationEnded -= OnDeathAnimationEndedHandle;
        _spider.OnEnterAnimationEnd -= OnSpiderEnterAnimationEndHandle;
        */
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
        
        // Initialize the state machine
        _inactiveState = new DragonflyInactiveState();
        _passiveState = new DragonflyPassiveState();
        _patrolState = new DragonflyPatrolState();
        _hoverState = new DragonflyHoverState();
        _patrolHeadState = new DragonflyPatrolHeadState(_patrolWaitMin, _patrolWaitMax);
        _patrolTailState = new DragonflyPatrolTailState(_patrolTailWaitMin, _patrolTailWaitMax);
        _waitHeadAttackState = new DragonflyWaitHeadAttackState(_visibleBodyTransform, _patrolAttackPositionProvider, _movement);
        _waitTailAttackState = new DragonflyWaitTailAttackState(_visibleBodyTransform, _patrolAttackPositionProvider, _movement);
        _waitHoverAttackState = new DragonflyWaitHoverAttackState(_hoverWaitMin, _hoverWaitMax);
        
        At(_inactiveState, _patrolState, () => _isActivated && _enterType == 0);
        At(_inactiveState, _hoverState, () => _isActivated && _enterType == 1);
        
        At(_patrolState, _patrolHeadState, IsReadyToPatrolHead());
        At(_patrolState, _patrolTailState, IsReadyToPatrolTail());
        
        At(_patrolHeadState, _waitHeadAttackState, IsReadyToAttackWait());
        At(_patrolTailState, _waitTailAttackState, IsReadyToAttackWait());
        
        At(_hoverState, _waitHoverAttackState, IsReadyToPreAttackWait());
        
        At(_waitHeadAttackState, _passiveState, IsAttacked());
        At(_waitTailAttackState, _passiveState, IsAttacked());
        At(_waitHoverAttackState, _passiveState, IsAttacked());
        
        
        
        _stateMachine.SetState(_inactiveState);
        _isActivated = false;
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        
        Func<bool> IsReadyToPatrolHead() => () =>
        {
            if (_isReadyToPreAttackWait && _patrolAttackMode == 0)
            {
                _isReadyToPreAttackWait = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsReadyToPatrolTail() => () =>
        {
            if (_isReadyToPreAttackWait && _patrolAttackMode == 1)
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
        
    }

    private void Start()
    {

        Initialize();
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

    private void StartBossActivePhase()
    {
        _collisionController.DisableColliders();
        _enterType = Random.Range(0, 2); // 0 Patrol, 1 Hover TODO: enum???????????
        // _enterType = 1; // TMP
        int sideDirection = RandomDirection.Generate();
        _movement.Play(_enterType, sideDirection);
        _isActivated = true;
    }

    public override void SpreadStart()
    {
        throw new NotImplementedException();
    }

    public override void StartAttack()
    {
        throw new NotImplementedException();
    }
    
    private void StartAttack(DragonflyPatrolAttackMode mode)
    {
        _movement.StartAttack(mode);
        _isAttacked = true;
        
    }
    
    public override void HandleEnteringAttackZone(Collider2D collider)
    {
        ReadyToLampDamage = true;            
        _collisionController.SoloCollider(collider);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
        
        _stateMachine.Tick();
        _stateDebug = _stateMachine.CurrentState.ToString();
        
        // WaitForHoverAttack();
        // WaitForHeadAttack();
        // WaitForSpiderAttack();
        // WaitForTailAttack();
    }

    // Attack Handle Methods ---------------------------------------------------

    /*
    private void StartAttack(DragonflyPatrolAttackMode mode)
    {
        _movement.StartAttack(mode);
    }*/

    // Hover Attack -------------------------------------------------------------

    /*
    private void PrepareHoverAttack()
    {
        _localTime = 0;
        _isWaitingForHoverAttack = true;
        _hoverWait = Random.Range(_hoverWaitMin, _hoverWaitMax);
    }
*/
  /*
    private void WaitForHoverAttack()
    {
        if (_isWaitingForHoverAttack)
        {
            _localTime += Time.deltaTime;
            if (_localTime >= _hoverWait)
            {
                StartAttack(DragonflyPatrolAttackMode.Head);
                _isWaitingForHoverAttack = false;
            }
        }
    }*/

    // Head Attack ------------------------------------------------------------- 
/*
    private void PreparePatrolToHeadAttack(float minWaitTime)
    {
        _localTime = 0;
        _isWaitingForHeadPatrolAttack = true;
        _patrolHeadWait = Random.Range(minWaitTime, _patrolWaitMax);
    }

    private void WaitForHeadAttack()
    {
        if (_isWaitingForHeadPatrolAttack)
        {
            if(_localTime < _patrolHeadWait)
            {
                _localTime += Time.deltaTime;
            }
            else
            {
                _patrolAttackPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackHeadPosition(_movement.MovementState);
                _isWaitingForHeadPatrolAttack = false;
                _isWaitingForHeadPatrolAttackPoint = true;
                _isLastPatrolDirectionSet = false;
            }
        }

        if (_isWaitingForHeadPatrolAttackPoint)
        {
            Vector3 currentPosition = _visibleBodyTransform.position;
            currentPosition.y = 0;
            currentPosition.Normalize();
            float distance = Vector3.Distance(currentPosition, _patrolAttackPosition);
            if (distance < 0.25f)
            {
                if (!_isLastPatrolDirectionSet)
                {
                    _lastPatrolDirection = (int)Mathf.Sign((_patrolAttackPosition - currentPosition).normalized.x);
                    _isLastPatrolDirectionSet = true;
                }
                else
                {
                    float currentPatrolDirection = (int)Mathf.Sign((_patrolAttackPosition - currentPosition).normalized.x);
                    if (currentPatrolDirection + _lastPatrolDirection == 0)
                    {
                        _isWaitingForHeadPatrolAttackPoint = false;
                        _isLastPatrolDirectionSet = false;
                        StartAttack(DragonflyPatrolAttackMode.Head);
                    }
                }
            }   
        }
    }*/

    // Tail Attack -------------------------------------------------------------

/*
    private void PreparePatrolToTailAttack(float minWaitTime)
    {
        _localTime = 0;
        _isWaitingForTailPatrolAttack = true;
        _patrolTailWait = Random.Range(minWaitTime, _patrolTailWaitMax);
    }
    */

/*

    private void WaitForTailAttack()
    {
        if (_isWaitingForTailPatrolAttack)
        {
            if(_localTime < _patrolTailWait)
            {
                _localTime += Time.deltaTime;
            }
            else
            {
                _patrolAttackPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackTailPosition(_movement.MovementState);
                _isWaitingForTailPatrolAttack = false;
                _isWaitingForTailPatrolAttackPoint = true;
                _isLastPatrolDirectionSet = false;
            }
        }

        if (_isWaitingForTailPatrolAttackPoint)
        {
            Vector3 currentPosition = _visibleBodyTransform.position;
            currentPosition.y = 0;
            currentPosition.Normalize();
            float distance = Vector3.Distance(currentPosition, _patrolAttackPosition);
            if (distance < 0.25f)
            {
                if (!_isLastPatrolDirectionSet)
                {
                    _lastPatrolDirection = (int)Mathf.Sign((_patrolAttackPosition - currentPosition).normalized.x);
                    _isLastPatrolDirectionSet = true;
                }
                else
                {
                    float currentPatrolDirection = (int)Mathf.Sign((_patrolAttackPosition - currentPosition).normalized.x);
                    if (currentPatrolDirection + _lastPatrolDirection == 0)
                    {
                        _isWaitingForTailPatrolAttackPoint = false;
                        _isLastPatrolDirectionSet = false;
                        StartAttack(DragonflyPatrolAttackMode.Tail);
                    }
                }
            }   
        }
    }
*/

    // Spider Patrol Attack -----------------------------------------------------
/*
    private void PrepareSpiderAttack()
    {
        _localTime = 0;
        _isWaitingForSpiderPatrolAttack = true;
        _isWaitingForSpiderPatrolAttackPoint = false;
        _patrolSpiderWait = Random.Range(_spiderPatrolWaitMin, _spiderPatrolWaitMax);
    }
*/

/*
    private void WaitForSpiderAttack()
    {
        if (_isWaitingForSpiderPatrolAttack)
        {
            if(_localTime < _patrolSpiderWait)
            {
                _localTime += Time.deltaTime;
            }
            else
            {
                _patrolAttackPosition = _priderAttackPositionBase;
                _patrolAttackPosition.x *= RandomDirection.Generate();
                Debug.DrawRay(Vector3.zero, _patrolAttackPosition, Color.yellow, 5f);
                _patrolAttackPosition.y = 0;
                _patrolAttackPosition.Normalize();
                
                
                _isWaitingForHeadPatrolAttack = false;
                _isWaitingForHeadPatrolAttackPoint = false;
                _isLastPatrolDirectionSet = false;
                _isWaitingForSpiderPatrolAttack = false;
                
                _isWaitingForSpiderPatrolAttackPoint = true;
            }
        }
        
        if (_isWaitingForSpiderPatrolAttackPoint)
        {
            Vector3 currentPosition = _visibleBodyTransform.position;
            currentPosition.y = 0;
            currentPosition.Normalize();
            float distance = Vector3.Distance(currentPosition, _patrolAttackPosition);
            if (distance < 0.25f)
            {
                if (!_isLastPatrolDirectionSet)
                {
                    _lastPatrolDirection = (int)Mathf.Sign((_patrolAttackPosition - currentPosition).normalized.x);
                    _isLastPatrolDirectionSet = true;
                }
                else
                {
                    float currentPatrolDirection = (int)Mathf.Sign((_patrolAttackPosition - currentPosition).normalized.x);
                    if (currentPatrolDirection + _lastPatrolDirection == 0)
                    {
                        _isWaitingForSpiderPatrolAttackPoint = false;
                        _isLastPatrolDirectionSet = false;
                        SpiderAttack();
                    }
                }
            }   
        }
    }
*/

/*
    private void SpiderAttack()
    {
        _localTime = 0;
        _spider.gameObject.transform.SetParent(this.transform);
        _spider.StartAttack();
        _movement.StartAttack(DragonflyPatrolAttackMode.Spider);
    }
*/

    //--------------------------------------------------------------------------------
    // Event Handle Methods
    private void GenerateAttackPosition()
    {
        _isReadyToAttackWait = true;
    }
    
    private void OnReadyToAttackEnterHandle(IState movementState)
    {
        _patrolAttackMode = Random.Range(0, 2); // TODO: enum???????????
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

    /*
    private void OnReadyToSpiderAttackEnterHandle()
    {
        PrepareSpiderAttack();
    }*/

    /*
    private void OnReadyToAttackEnterHandle(IState movementState)
    {
        float minWaitTime = 0;
        if (movementState.GetType() == typeof(FDragonflyHoverState))
        {
            PrepareHoverAttack();
        }
        else if (movementState.GetType() == typeof(FDragonflyPatrolStateL) ||
                 movementState.GetType() == typeof(FDragonflyPatrolStateR))
        {
            minWaitTime = _patrolWaitMin;
            
            int mode = Random.Range(0, 2);
            if (mode == 0)
            {
                PreparePatrolToHeadAttack(minWaitTime);
            }
            else
            {
                PreparePatrolToTailAttack(minWaitTime);
            }
        }
        else if (movementState.GetType() == typeof(FDragonflySpiderPatrolStateL) || 
                 movementState.GetType() == typeof(FDragonflySpiderPatrolStateR))
        {
            PrepareSpiderAttack();
        }
    }
    */

    /*
    private void OnAttackStartHandle()
    {
        _collisionController.EnableColliders();
        _presentation.PreAttackEnd();
    }*/

    /*
    private void OnAttackEndHandle()
    {
        _collisionController.DisableColliders();
    }*/


    /*
    private void OnAfterAttackExitEndHandle(IState movementState)
    {
        
        DragonflyReturnMode mode = RETURN_MODES[Random.Range(0, 6)];
        _movement.ResolveReturnTransition(mode);
    }*/

    /*
    private void OnCatchSpiderStartHandle(int direction)
    {
        _spider.gameObject.SetActive(true);
        _spider.Initialize(direction);
    }*/

    /*
    private void OnSpiderEnterAnimationEndHandle()
    {
        _spider.gameObject.transform.SetParent(_visibleBodyTransform);
        Vector3 pos = Vector3.zero;
        pos.x = 0.012f;
        pos.y = -0.286f;
        pos.z = 0.082f;
        _spider.gameObject.transform.localPosition = pos;
    }*/

    // Enemy Base Methods
    public override void HandleCollisionWithLamp()
    {
        ReadyToCollide = false;
        ReadyToLampDamage = true;
        _movement.TriggerBounce();
    }

    public override void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
        if (!ReceivedLampAttack)
        {
            _movement.TriggerFall(false);
        }
    }

    public override void HandleCollisionWithStickZone()
    {
        Debug.LogWarning("Dragonfly penetrated collision zone");
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
    }
    
    private void OnDeathAnimationEndedHandle()
    {
        gameObject.SetActive(false);
    }

    public override void UpdateAttackAvailability()
    {
        throw new NotImplementedException();
    }

    public override void ReturnToPool()
    {
        throw new NotImplementedException();
    }
    
    public override Vector3 ProvideImpactPoint()
    {
        return _collisionController.GetFirstActiveColliderPosition();
    }
    
    private void OnPreAttackStartHandle()
    {
        ReceivedLampAttack = false;
        _presentation.PreAttackStart();
    }
    
    private void TMPHandleLampAttack(int arg1, float arg2, float arg3, float arg4)
    {
        if (ReadyToLampDamage)
        {
            ReceiveDamage(arg1);
        }
    }
}
