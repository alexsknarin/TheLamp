using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dragonfly : EnemyBase
{
    [SerializeField] private EnemyTypes _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private DragonflyMovement _movement;
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
    private DragonflySpiderPatrolAttackZoneRangesData _spiderPatrolAttackZonesData = new DragonflySpiderPatrolAttackZoneRangesData();

    private DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
        
    private Vector3 _patrolAttackPosition;
    private Vector3 _patrolSpiderAttackPosition;
    
    private DragonflyState[] ATTACK_STATES = new DragonflyState[]
    {
        DragonflyState.AttackHead,
        DragonflyState.AttackHover,
        DragonflyState.AttackTailL,
        DragonflyState.AttackTailR
    };
    
    
    private void OnEnable()
    {
        _movement.OnReadyToAttackStateEntered += OnReadyToAttackStateEntered;
        _movement.OnAfterAttackExitEnded += OnAfterAttackExitEnded;
        _movement.OnAttackStarted += OnAttackStarted;
        _movement.OnAttackEnded += OnAttackEnded;
        _movement.OnPreattackStarted += OnPreAttackStart;
        _spider.OnEnterAnimationEnd += OnSpiderEnterAnimationEnd;
        
        LampAttackModel.OnLampAttack += TMPHandleLampAttack;
    }

    private void OnDisable()
    {
        _movement.OnReadyToAttackStateEntered -= OnReadyToAttackStateEntered;
        _movement.OnAfterAttackExitEnded -= OnAfterAttackExitEnded;
        _movement.OnAttackStarted -= OnAttackStarted;
        _movement.OnAttackEnded -= OnAttackEnded;
        _movement.OnPreattackStarted -= OnPreAttackStart;
        _spider.OnEnterAnimationEnd -= OnSpiderEnterAnimationEnd;
        
        LampAttackModel.OnLampAttack -= TMPHandleLampAttack;
    }

    private void Start()
    {
        Initialize();
        _patrolAttackPositionProvider = new DragonflyPatrolAttackPositionProvider(
            _patrolAttackZonesL, 
            _patrolAttackZonesR, 
            _tailAttackZoneLMin,
            _tailAttackZoneLMax,
            _tailAttackZoneRMin,
            _tailAttackZoneRMax);
    }

    public void Initialize()
    {
        _isWaitingForHoverAttack = false;
        _isWaitingForHeadPatrolAttack = false;
        _isWaitingForHeadPatrolAttackPoint = false;
        _isWaitingForTailPatrolAttack = false;
        _isWaitingForTailPatrolAttackPoint = false;
        _isWaitingForSpiderPatrolAttack = false;
        _isWaitingForSpiderPatrolAttackPoint = false;
        
        _presentation.Initialize();
    }

    public override void SpreadStart()
    {
        throw new NotImplementedException();
    }

    public override void AttackStart()
    {
        throw new NotImplementedException();
    }

    public override void HandleEnteringAttackZone(Collider2D collider)
    {
        ReadyToLampDamage = true;            
        _collisionController.SoloCollider(collider);
    }

    public void Play()
    {
        _isWaitingForHoverAttack = false;
        _isWaitingForHeadPatrolAttack = false;
        _isWaitingForHeadPatrolAttackPoint = false;
        _isWaitingForTailPatrolAttack = false;
        _isWaitingForTailPatrolAttackPoint = false;
        _isWaitingForSpiderPatrolAttack = false;
        _isWaitingForSpiderPatrolAttackPoint = false;
        StartBossActivePhase();
        _presentation.Initialize();
    }

    private void StartBossActivePhase()
    {
        _collisionController.DisableColliders();
        // There are only two enter types at the moment
        int enterType = Random.Range(0, 2);
        // Randomly decide the first attack
        int sideDirection = RandomDirection.Generate();
        _movement.Play(enterType, sideDirection);
    }
    
    // Attack Handle Methods ---------------------------------------------------
    private void StartAttack(DragonflyPatrolAttackMode mode)
    {
        _movement.StartAttack(mode);
    }
    
    // Hover Attack -------------------------------------------------------------
    private void PrepareHoverAttack()
    {
        _localTime = 0;
        _isWaitingForHoverAttack = true;
        _hoverWait = Random.Range(_hoverWaitMin, _hoverWaitMax);
    }

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
    }
    
    // Head Attack ------------------------------------------------------------- 
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
                _patrolAttackPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackHeadPosition(_movement.State);
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
    }
    
    // Tail Attack -------------------------------------------------------------
    private void PreparePatrolToTailAttack(float minWaitTime)
    {
        _localTime = 0;
        _isWaitingForTailPatrolAttack = true;
        _patrolTailWait = Random.Range(minWaitTime, _patrolTailWaitMax);
    }
    
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
                _patrolAttackPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackTailPosition(_movement.State);
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
    
    
    // Spider Patrol Attack -----------------------------------------------------
    private void PrepareSpiderAttack()
    {
        _localTime = 0;
        _isWaitingForSpiderPatrolAttack = true;
        _isWaitingForSpiderPatrolAttackPoint = false;
        _patrolSpiderWait = Random.Range(_spiderPatrolWaitMin, _spiderPatrolWaitMax);
    }

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
                        // StartAttack(DragonflyPatrolAttackMode.Tail);
                    }
                }
            }   
        }
    }
    
    private void SpiderAttack()
    {
        _localTime = 0;
        _spider.gameObject.transform.SetParent(this.transform);
        _spider.AttackStart();
        _movement.SwitchState();
    }

    //--------------------------------------------------------------------------------
    // Event Handle Methods
    private void OnReadyToAttackStateEntered(DragonflyState state, DragonflyState prevState)
    {
        if (state == DragonflyState.Hover)
        {
            PrepareHoverAttack();
        }
        if (state == DragonflyState.PatrolL || state == DragonflyState.PatrolR)
        {
            float minWaitTime = 0;
            if (prevState == DragonflyState.EnterToPatrolL || prevState == DragonflyState.MoveToPatrolL)
            {
                _swarm.PlayAttack(1);
                minWaitTime = _patrolWaitMin;
            }
            if (prevState == DragonflyState.EnterToPatrolR || prevState == DragonflyState.MoveToPatrolR)
            {
                _swarm.PlayAttack(-1);
                minWaitTime = _patrolWaitMin;
            
            }
            
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
        if (state == DragonflyState.SpiderPatrolL || state == DragonflyState.SpiderPatrolR)
        {
            PrepareSpiderAttack();
        }
    }

    private void OnAttackStarted()
    {
        _collisionController.EnableColliders();
        _presentation.PreAttackEnd();
    }
    
    private void OnAttackEnded()
    {
        _collisionController.DisableColliders();
    }
    
    
    private void OnAfterAttackExitEnded(DragonflyState state)
    {
        
        DragonflyReturnMode mode = (DragonflyReturnMode)Random.Range(0, 3);
        int direction = RandomDirection.Generate();
        if (mode == DragonflyReturnMode.Spider)
        {
            _spider.gameObject.SetActive(true);
            _spider.Initialize(direction);
        }
        _movement.ResolveReturnTransition(mode, direction);
    }

    private void OnSpiderEnterAnimationEnd()
    {
        _spider.gameObject.transform.SetParent(_visibleBodyTransform);
        Vector3 pos = Vector3.zero;
        pos.x = 0.012f;
        pos.y = -0.286f;
        pos.z = 0.082f;
        _spider.gameObject.transform.localPosition = pos;
    }
    
    // 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            int direction = RandomDirection.Generate();
            _spider.Initialize(direction);
            _movement.ResolveReturnTransition(DragonflyReturnMode.Spider, direction);
        }
        
        WaitForHoverAttack();
        WaitForHeadAttack();
        WaitForSpiderAttack();
        WaitForTailAttack();
    }
    
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
    }

    public override void HandleCollisionWithStickZone()
    {
        Debug.LogWarning("Dragonfly penetrated collision zone");
    }

    public override void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
        _presentation.DamageFlash();
        if (_currentHealth > 0)
        {
            ReceivedLampAttack = true;
            // _enemyPresentation.DamageFlash();
            // _enemyPresentation.HealthUpdate(_currentHealth, _maxHealth);
            // OnEnemyDamaged?.Invoke(this);
            _movement.TriggerFall(true);
            // _enemyMovement.TriggerFall();
        }
        else
        {
            if (!_isDead)
            {
                ReceivedLampAttack = true;
                _currentHealth = 0; 
                // _enemyMovement.TriggerDeath();
                _movement.TriggerFall(true);
                OnEnemyDeathInvoke(this);
                // _enemyPresentation.DeathFlash();
                _isDead = true;
            }
        }   
    }

    public override void UpdateAttackAvailability()
    {
        throw new NotImplementedException();
    }

    public override void ReturnToPool()
    {
        throw new NotImplementedException();
    }
    
    private void OnPreAttackStart()
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
