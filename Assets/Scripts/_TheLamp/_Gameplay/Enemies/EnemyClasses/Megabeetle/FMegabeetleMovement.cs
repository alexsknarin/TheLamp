using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FMegabeetleMovement : FEnemyMovementBase, IPositionDirectionProvider
{
    [SerializeField] private float _collisionRadius = 0.125f; // TODO: DI?
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private bool _isDepthEnabled;
    [SerializeField] private Vector3 IDLE_POSITION; // For Debug
    [SerializeField] private int _sideDirection; // For Debug
    [SerializeField] private string _stateDebug; // For Debug
    
    // State Machine
    private readonly FStateMachine _stateMachine = new();
    private MegabeetleMovementStateFactory _stateFactory;
    private RegularEnemyMovementStateBase _currentState;
    
    private FMegabeetleMovementEnterState _enterState;
    private FMegabeetleMovementPatrolState _patrolState;
    private FLadybugMovementPreAttackState _preAttackStateR;
    private FLadybugMovementAttackState _attackState;
    private FMegabeetleMovementStickState _stickState;
    private FMegabeetleMovementStickPreAttackState _stickPreAttackState;
    private FMegabeetleMovementStickPreAttackPauseState _stickPreAttackPauseState;
    private FMegabeetleMovementStickAttackState _stickAttackState;
    private FMegabeetleMovementStickLandingState _stickLandingState;
    private FMegabeetleMovementFallState _fallState;
    private FMegabeetleMovementDeathState _deathState;
    
    
    // Debug only
    private Vector3 _prevPosition;
    private Vector3 _prevPosSmooth;
    private Vector3 _velocity = Vector3.zero;
    
    public void Construct(
        MegabeetleMovementStateFactory stateFactory) 
    {
        _stateFactory = stateFactory;
    }
    
    public event Action EnteredAttackRange;
    public event Action PreAttackStarted;
    public event Action PreAttackEnded;
    public event Action DeathStateEnded;
    public event Action AttackStarted;
    public event Action StickyAttackEnded;
    public Vector2 Position2D { get; private set; }
    public Vector3 DepthDirection { get; private set; }
    
    
    public override void Initialize()
    {
        _stateFactory.SetEnemyDependencies(
            this,
            _speed,
            _radius, 
            _verticalAmplitude,
            _collisionRadius);
        
        _enterState = (FMegabeetleMovementEnterState)_stateFactory.Create(typeof(FMegabeetleMovementEnterState));
        _patrolState = (FMegabeetleMovementPatrolState)_stateFactory.Create(typeof(FMegabeetleMovementPatrolState));
        _preAttackStateR = (FLadybugMovementPreAttackState)_stateFactory.Create(typeof(FLadybugMovementPreAttackState));
        _attackState = (FLadybugMovementAttackState)_stateFactory.Create(typeof(FLadybugMovementAttackState));
        _stickState = (FMegabeetleMovementStickState)_stateFactory.Create(typeof(FMegabeetleMovementStickState));
        _stickPreAttackState = (FMegabeetleMovementStickPreAttackState)_stateFactory.Create(typeof(FMegabeetleMovementStickPreAttackState));
        _stickPreAttackPauseState = (FMegabeetleMovementStickPreAttackPauseState)_stateFactory.Create(typeof(FMegabeetleMovementStickPreAttackPauseState));
        _stickAttackState = (FMegabeetleMovementStickAttackState)_stateFactory.Create(typeof(FMegabeetleMovementStickAttackState));
        _stickLandingState = (FMegabeetleMovementStickLandingState)_stateFactory.Create(typeof(FMegabeetleMovementStickLandingState));
        _fallState = (FMegabeetleMovementFallState)_stateFactory.Create(typeof(FMegabeetleMovementFallState));
        _deathState = (FMegabeetleMovementDeathState)_stateFactory.Create(typeof(FMegabeetleMovementDeathState));
        
        _enterState.EnteredAttackRange += OnEnteredAttackRange;
        _patrolState.EnteredAttackRange += OnEnteredAttackRange;
        _preAttackStateR.Started += OnPreAttackStarted;
        _preAttackStateR.Ended += OnPreAttackEnded;
        _stickPreAttackPauseState.Started += OnPreAttackStarted;
        _stickPreAttackPauseState.Ended += OnPreAttackEnded;
        _attackState.Started += OnAttackStarted;
        _deathState.Ended += OnDeathStateEnded;
        _stickAttackState.Ended += OnStickAttackEnded;
            
        At(_enterState, _preAttackStateR, () => _enterState.IsReadyToSwitch);
        At(_patrolState, _preAttackStateR, () => _patrolState.IsReadyToSwitch);
        At(_preAttackStateR, _attackState, () => _preAttackStateR.IsReadyToSwitch);
        
        At(_stickLandingState, _stickState, () => _stickLandingState.IsReadyToSwitch);
        At(_stickState, _stickPreAttackState, () => _stickState.IsReadyToSwitch);
        At(_stickPreAttackState, _stickPreAttackPauseState, () => _stickPreAttackState.IsReadyToSwitch);
        At(_stickPreAttackPauseState, _stickAttackState, () => _stickPreAttackPauseState.IsReadyToSwitch);
        At(_stickAttackState, _stickState, () => _stickAttackState.IsReadyToSwitch);
        
        At(_fallState, _patrolState, () => _fallState.IsReadyToSwitch);
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }



    private void OnDisable()
    {
        _enterState.EnteredAttackRange -= OnEnteredAttackRange;
        _patrolState.EnteredAttackRange -= OnEnteredAttackRange;
        _preAttackStateR.Started -= OnPreAttackStarted;
        _preAttackStateR.Ended -= OnPreAttackEnded;
        _stickPreAttackPauseState.Started -= OnPreAttackStarted;
        _stickPreAttackPauseState.Ended -= OnPreAttackEnded;
        _attackState.Started -= OnAttackStarted;
        _deathState.Ended -= OnDeathStateEnded;
        _stickAttackState.Ended += OnStickAttackEnded;
    }

    public override void Play()
    {
        _sideDirection = 1; //RandomDirection.Generate();
        SideDirection = _sideDirection;
        // _depthDirection = 1; //RandomDirection.Generate();
        Position2D = GenerateSpawnPosition(_radius, _sideDirection);
        
        // TODO: probably we will need L R enter as Ladybug has
        _currentState = _enterState;
        _stateMachine.SetState(_currentState);
        
        Position2D = _currentState.Position2D;
        transform.position = Position2D; // TODO: include distance to camera
        
        enabled = true;
    }

    public override void TriggerAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerFall()
    {
        transform.parent = null;
        SwitchToStateAndApply(_fallState);
    }

    public override void TriggerDeath()
    {
        transform.parent = null;
        SwitchToStateAndApply(_deathState);
    }

    public void TriggerStick(Transform target)
    {
        transform.parent = target;
        
        _currentState = _stickLandingState;
        _stateMachine.SetState(_currentState); // Correct sticky position on enter
    
        // TODO: Later
        // if (_isDepthEnabled)
        // {
        //     DepthDirection = _currentState.DepthDirection;
        //     transform.position = (Vector3)Position2D + DepthDirection;
        // }
        // else
        // {
        //     transform.position = _currentState.Position2D;
        // }
        
        transform.localPosition = _currentState.Position2D;
    }

    private void Update()
    {
        _prevPosition = transform.position;
        
        _stateMachine.Tick();
        _currentState = (RegularEnemyMovementStateBase)_stateMachine.CurrentState;
        _stateDebug = _currentState.GetType().Name; // Debug only
        Position2D = _currentState.Position2D;
        
        transform.localPosition = Position2D;
        
        Debug.DrawLine(_prevPosition, transform.position, Color.cyan, 10f);
    }

    private Vector2 GenerateSpawnPosition(float distance, int direction)
    {
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = distance;
        Quaternion rotation = Quaternion.Euler(0, 0, -Random.Range(22, 49));
        spawnPosition = rotation * spawnPosition;
        spawnPosition.x *= -direction;
        return spawnPosition;
    }

    private void SwitchToStateAndApply(RegularEnemyMovementStateBase state) // TODO: implement this in all enemies
    {
        _currentState = state;
        _stateMachine.SetState(_currentState);
        Position2D = _currentState.Position2D;
        Vector2 newPosition = Position2D;
        
        transform.position = newPosition;
        _stateDebug = _currentState.GetType().Name;
    }

    private void OnEnteredAttackRange()
    {
        EnteredAttackRange?.Invoke();
    }

    private void OnPreAttackStarted()
    {
        PreAttackStarted?.Invoke();
    }

    private void OnPreAttackEnded()
    {
        PreAttackEnded?.Invoke();
    }

    private void OnDeathStateEnded()
    {
        DeathStateEnded?.Invoke();
    }

    private void OnAttackStarted()
    {
        AttackStarted?.Invoke();
    }
    
    private void OnStickAttackEnded()
    {
        StickyAttackEnded?.Invoke();
    }
}
