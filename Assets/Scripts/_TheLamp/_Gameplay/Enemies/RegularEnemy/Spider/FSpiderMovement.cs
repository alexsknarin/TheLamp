using System;
using UnityEngine;

public class FSpiderMovement : FEnemyMovementBase, IPositionDirectionProvider
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    // Debug
    [SerializeField] private string _stateDebug;
    [SerializeField] private int _sideDirection = 1; // TODO: Try without it
    [SerializeField] private int _depthSideDirection = 0;
    [SerializeField] private float _height = 5f;
    [SerializeField] private float _xCenter = 1.12f;
    
    private Vector3 _position3D;
    // Debug only
    private Vector3 _prevPosition;
    private Vector3 _prevPosSmooth;
    private Vector3 _velocity = Vector3.zero;
    
    // State Machine
    private readonly FStateMachine _stateMachine = new();
    private SpiderMovementStateFactory _stateFactory;
    private RegularEnemyMovementStateBase _currentState;
    
    private FSpiderMovementEnterState _enterState;
    private FSpiderMovementPatrolState _patrolState;
    private FSpiderMovementPreAttackState _preAttackState;
    private FSpiderMovementAttackState _attackState;
    private FSpiderMovementReturnState _returnState;
    private FFlyGenericMovementDeathState _deathState;
    private FSpiderMovementClimbUpState _climbUpState;
    
    public void Construct(SpiderMovementStateFactory stateFactory)
    {
        _stateFactory = stateFactory;
    }
    
    public event Action ReadyToAttackStateStarted;
    public event Action ReadyToAttackStateEnded;
    public event Action PreAttackStarted;
    public event Action PreAttackEnded;
    public event Action DeathStateEnded;
    public event Action SpreadStateEnded;

    public Vector2 Position2D { get; private set; } 
    public Vector3 DepthDirection { get; private set; }
    
    public override int SideDirection => _sideDirection;
    
    public override void Initialize()
    {
        Debug.Log("FFlyMovement Initializing");
        // Create Movement States
        // TODO: get collision radius from configs
        _stateFactory.SetEnemyDependencies(this, _speed, _xCenter, _height);
        _enterState = (FSpiderMovementEnterState)_stateFactory.Create(typeof(FSpiderMovementEnterState));
        _patrolState = (FSpiderMovementPatrolState)_stateFactory.Create(typeof(FSpiderMovementPatrolState));
        _preAttackState = (FSpiderMovementPreAttackState)_stateFactory.Create(typeof(FSpiderMovementPreAttackState));
        _attackState = (FSpiderMovementAttackState)_stateFactory.Create(typeof(FSpiderMovementAttackState));
        _returnState = (FSpiderMovementReturnState)_stateFactory.Create(typeof(FSpiderMovementReturnState));
        _deathState = (FFlyGenericMovementDeathState)_stateFactory.Create(typeof(FFlyGenericMovementDeathState));
        _climbUpState = (FSpiderMovementClimbUpState)_stateFactory.Create(typeof(FSpiderMovementClimbUpState));
        
        // _patrolState = (FFlyGenericMovementPatrolState)_stateFactory.Create(typeof(FFlyGenericMovementPatrolState));
        // _preAttackStateR = (FFlyMovementPreAttackStateR)_stateFactory.Create(typeof(FFlyMovementPreAttackStateR));
        // _preAttackStateL = (FFlyMovementPreAttackStateL)_stateFactory.Create(typeof(FFlyMovementPreAttackStateL));
        // _attackState = (FFlyGenericMovementAcceleratedAttackState)_stateFactory.Create(typeof(FFlyGenericMovementAcceleratedAttackState));
        // _fallState = (FFlyGenericMovementFallState)_stateFactory.Create(typeof(FFlyGenericMovementFallState));
        // _deathState = (FFlyMovementDeathState)_stateFactory.Create(typeof(FFlyMovementDeathState));
        // _spreadState = (FFlyGenericMovementSpreadState)_stateFactory.Create(typeof(FFlyGenericMovementSpreadState));
        
        // Subscribe to state events
        _patrolState.Started += OnPatrolStateStarted; 
        _patrolState.Ended += OnPatrolStateEnded;
        _preAttackState.Started += OnPreAttackStateStarted;
        _preAttackState.Ended += OnPreAttackStateEnded;
        _deathState.Ended += OnDeathStateEnded;
        // _spreadState.Ended += OnSpreadStateEnded;
        
        // Automatic State transitions
        // At(_enterState, _patrolState, () => _enterState.IsReadyToSwitch);
        // At(_patrolState, _preAttackStateR, IsAttackStartedR());
        // At(_patrolState, _preAttackStateL, IsAttackStartedL());
        // At(_preAttackStateR, _attackState, () => _preAttackStateR.IsReadyToSwitch);
        // At(_preAttackStateL, _attackState, () => _preAttackStateL.IsReadyToSwitch);
        // At(_fallState, _enterState, IsFallEnded());
        At(_enterState, _patrolState, () => _enterState.IsReadyToSwitch);
        At(_patrolState, _preAttackState, IsAttackStarted());
        At(_preAttackState, _attackState, () => _preAttackState.IsReadyToSwitch);
        At(_returnState, _patrolState, () => _returnState.IsReadyToSwitch);
        At(_climbUpState, _enterState, () => _climbUpState.IsReadyToSwitch);
        
        // Predicates
        Func<bool> IsAttackStarted() => () =>
        {
            if (_isAttacking)
            {
                _isAttacking = false;
                return true;
            }
            return false;
        };
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }

    private void OnDestroy()
    {
        _patrolState.Started += OnPatrolStateStarted; 
        _patrolState.Ended += OnPatrolStateEnded;
        _preAttackState.Started -= OnPreAttackStateStarted;
        _preAttackState.Ended -= OnPreAttackStateEnded;
        _deathState.Ended -= OnDeathStateEnded;
    }

    public override void Play()
    {
        _currentState = _enterState;
        _stateMachine.SetState(_currentState);
        
        Position2D = _currentState.Position2D;
        _position3D = Position2D; // TODO: do we need it
        transform.position = _position3D;
        
        _isAttacking = false;
        enabled = true;
    }

    public override void TriggerAttack()
    {
        _isAttacking = true;
    }

    public override void TriggerFall()
    {
        _currentState = _returnState;
        _stateMachine.SetState(_currentState);
        Position2D = _currentState.Position2D;
        transform.position = Position2D;
        _stateDebug = _currentState.GetType().Name; // Debug only
    }

    public override void TriggerDeath()
    {
        _currentState = _deathState;
        _stateMachine.SetState(_currentState);
        Position2D = _currentState.Position2D;
        transform.position = Position2D;
        _stateDebug = _currentState.GetType().Name; // Debug only
    }

    public override void TriggerSpread()
    {
        _currentState = _climbUpState;
        _stateMachine.SetState(_currentState);
        Position2D = _currentState.Position2D;
        transform.position = Position2D;
        _stateDebug = _currentState.GetType().Name; // Debug only TODO: extract method for state set
    }

    private void Update()
    {
        _stateMachine.Tick();
        _currentState = (RegularEnemyMovementStateBase)_stateMachine.CurrentState;
        _stateDebug = _currentState.GetType().Name; // Debug only
        Position2D = _currentState.Position2D;
        
        transform.position = Position2D;
    }

    private void OnPatrolStateStarted()
    {
        ReadyToAttackStateStarted?.Invoke();
    }

    private void OnPatrolStateEnded()
    {
        ReadyToAttackStateEnded?.Invoke();
    }

    private void OnPreAttackStateStarted()
    {
        PreAttackStarted?.Invoke();        
    }

    private void OnPreAttackStateEnded()
    {
        PreAttackEnded?.Invoke();
    }

    private void OnDeathStateEnded()
    {
        DeathStateEnded?.Invoke();
    }
}
