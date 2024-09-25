using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dragonfly : MonoBehaviour
{
    [SerializeField] private DragonflyMovement _dragonflyMovement;
    [SerializeField] private DragonflyCollisionCatcher _collisionCatcher;
    [SerializeField] private Transform _visibleBodyTransform;
    [Header("Hover")]
    [SerializeField] private float _hoverWaitMin;
    [SerializeField] private float _hoverWaitMax;
    [Header("Patrol")]
    [SerializeField] private float _patrolWaitMin;
    [SerializeField] private float _patrolWaitMax;
    [SerializeField] private DragonflyPatrolAttackZoneRanges _patrolAttackZonesL;
    [SerializeField] private DragonflyPatrolAttackZoneRanges _patrolAttackZonesR;
    [Header("Spider Patrol")]
    [SerializeField] private DragonflySpiderPatrolAttackZoneRanges _spiderPatrolAttackZones;
    [SerializeField] private float _spiderPatrolWaitMin;
    [SerializeField] private float _spiderPatrolWaitMax;
    
    private float _hoverWait;
    private float _patrolHeadWait;
    private float _patrolSpiderWait;
    private float _localTime = 0;
    private bool _isWaitingForHoverAttack = false;
    private bool _isWaitingForHeadPatrolAttack = false;
    private bool _isWaitingForHeadPatrolAttackPoint = false;
    private bool _isWaitingForSpiderPatrolAttack = false;
    private bool _isWaitingForSpiderPatrolAttackPoint = false;
    private int _lastPatrolDirection = 0;
    private bool _isLastPatrolDirectionSet = false;
    
    private DragonflyPatrolAttackZoneRangesData _patrolAttackZonesData = new DragonflyPatrolAttackZoneRangesData();
    private DragonflySpiderPatrolAttackZoneRangesData _spiderPatrolAttackZonesData = new DragonflySpiderPatrolAttackZoneRangesData();

    private DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
        
    private Vector3 _patrolAttackPosition;
    private Vector3 _patrolSpiderAttackPosition;
    
    private void OnEnable()
    {
        _dragonflyMovement.OnReadyToAttackStateEntered += OnReadyToAttackStateEntered;
        _dragonflyMovement.OnAfterAttackExitEnded += OnAfterAttackExitEnded;
        _dragonflyMovement.OnAttackStarted += OnAttackStarted;
        _dragonflyMovement.OnAttackEnded += OnAttackEnded;
    }
    
    private void OnDisable()
    {
        _dragonflyMovement.OnReadyToAttackStateEntered -= OnReadyToAttackStateEntered;
        _dragonflyMovement.OnAfterAttackExitEnded -= OnAfterAttackExitEnded;
        _dragonflyMovement.OnAttackStarted -= OnAttackStarted;
        _dragonflyMovement.OnAttackEnded -= OnAttackEnded;
    }

    private void Start()
    {
        Initialize();
        _patrolAttackPositionProvider = new DragonflyPatrolAttackPositionProvider(
            _patrolAttackZonesL, 
            _patrolAttackZonesR, 
            _spiderPatrolAttackZones);
    }

    public void Initialize()
    {
        _isWaitingForHoverAttack = false;
        _isWaitingForHeadPatrolAttack = false;
        _isWaitingForHeadPatrolAttackPoint = false;
        _isWaitingForSpiderPatrolAttack = false;
        _isWaitingForSpiderPatrolAttackPoint = false;
    }

    public void Play()
    {
        _isWaitingForHoverAttack = false;
        _isWaitingForHeadPatrolAttack = false;
        _isWaitingForHeadPatrolAttackPoint = false;
        _isWaitingForSpiderPatrolAttack = false;
        _isWaitingForSpiderPatrolAttackPoint = false;
        StartBossActivePhase();
    }

    private void StartBossActivePhase()
    {
        _collisionCatcher.DisableColliders();
        // There are only two enter types at the moment
        int enterType = Random.Range(0, 2);
        // Randomly decide the first attack
        int sideDirection = RandomDirection.Generate();
        _dragonflyMovement.Play(enterType, sideDirection);
    }
    
    // Attack Handle Methods ---------------------------------------------------
    private void StartAttack()
    {
        _dragonflyMovement.StartAttack();
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
                StartAttack();
                _isWaitingForHoverAttack = false;
            }
        }
    }
    
    // Head Attack ------------------------------------------------------------- 
    private void PreparePatrolToHeadAttack()
    {
        _isWaitingForHeadPatrolAttack = true;
        _localTime = 0;
        _patrolHeadWait = Random.Range(_patrolWaitMin, _patrolWaitMax);
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
                _patrolAttackPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackHeadPosition(_dragonflyMovement.State);
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
                        StartAttack();
                    }
                }
            }   
        }
    }
    
    // Spider Patrol Attack -----------------------------------------------------
    private void PrepareSpiderAttack()
    {
        _isWaitingForSpiderPatrolAttack = true;
        _isWaitingForSpiderPatrolAttackPoint = false;
        _localTime = 0;
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
                _patrolAttackPosition = _patrolAttackPositionProvider.GenerateSpiderRandomPreAttackHeadPosition();
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
                        StartAttack();
                    }
                }
            }   
        }
    }



    //--------------------------------------------------------------------------------
    // Event Handle Methods
    private void OnReadyToAttackStateEntered(DragonflyStates state)
    {
        if (state == DragonflyStates.Hover)
        {
            PrepareHoverAttack();
        }
        if (state == DragonflyStates.PatrolL || state == DragonflyStates.PatrolR)
        {
            PreparePatrolToHeadAttack();
        }
        if (state == DragonflyStates.SpiderPatrolL || state == DragonflyStates.SpiderPatrolR)
        {
            PrepareSpiderAttack();
        }
    }

    private void OnAttackStarted()
    {
        _collisionCatcher.EnableColliders();
    }
    
    private void OnAttackEnded()
    {
        _collisionCatcher.DisableColliders();
    }
    
    
    private void OnAfterAttackExitEnded(DragonflyStates state)
    {
        int mode = Random.Range(0, 3);
        int direction = RandomDirection.Generate();
        _dragonflyMovement.Return(mode, direction);
    }
    
    // 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Play();
        }
        
        WaitForHoverAttack();
        WaitForHeadAttack();
        WaitForSpiderAttack();
    }
}
