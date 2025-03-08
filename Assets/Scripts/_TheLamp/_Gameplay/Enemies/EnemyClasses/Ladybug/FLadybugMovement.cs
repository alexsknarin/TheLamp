using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FLadybugMovement : FEnemyMovementBase, IPositionDirectionProvider, ISpreadableMovement
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _collisionRadius = 0.125f; // TODO: DI?
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private bool _isDepthEnabled;
    // Debug
    [SerializeField] private string _stateDebug;
    [SerializeField] private int _sideDirection = 1;
    
    private Vector3 _position3D;
    // Debug only
    private Vector3 _prevPosition;
    private Vector3 _prevPosSmooth;
    private Vector3 _velocity = Vector3.zero;
    
    
    // State Machine
    private readonly FStateMachine _stateMachine = new();
    private LadybugMovementStateFactory _stateFactory;
    private RegularEnemyMovementStateBase _currentState;
    
    private FLadybugMovementPatrolStateR _patrolStateR;
    private FLadybugMovementPatrolStateL _patrolStateL;
    private FLadybugMovementPreAttackStateR _preAttackStateR;
    private FLadybugMovementPreAttackStateL _preAttackStateL;
    private FLadybugMovementAttackState _attackState;
    private FLadybugMovementStickState _stickState;
    private FLadybugMovementDeathFallState _deathFallState;
    private FFlyGenericMovementSpreadState _spreadState;
    
    public void Construct(
        LadybugMovementStateFactory stateFactory) 
    {
        _stateFactory = stateFactory;
    }
    
    public event Action PreAttackStarted;
    public event Action PreAttackEnded;
    public event Action DeathStateEnded;
    public event Action SpreadStateEnded;
    public event Action EnteredAttackRange;
    
    public Vector2 Position2D { get; private set; }
    public Vector3 DepthDirection { get; private set; }
    
    public override void Initialize()
    {        
        enabled = false;
        _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude, _collisionRadius);
        _patrolStateR = (FLadybugMovementPatrolStateR)_stateFactory.Create(typeof(FLadybugMovementPatrolStateR));
        _patrolStateL = (FLadybugMovementPatrolStateL)_stateFactory.Create(typeof(FLadybugMovementPatrolStateL));
        _preAttackStateR = (FLadybugMovementPreAttackStateR)_stateFactory.Create(typeof(FLadybugMovementPreAttackStateR));
        _preAttackStateL = (FLadybugMovementPreAttackStateL)_stateFactory.Create(typeof(FLadybugMovementPreAttackStateL));
        _attackState = (FLadybugMovementAttackState)_stateFactory.Create(typeof(FLadybugMovementAttackState));
        _stickState = (FLadybugMovementStickState)_stateFactory.Create(typeof(FLadybugMovementStickState));
        _deathFallState = (FLadybugMovementDeathFallState)_stateFactory.Create(typeof(FLadybugMovementDeathFallState));
        _spreadState = (FFlyGenericMovementSpreadState)_stateFactory.Create(typeof(FFlyGenericMovementSpreadState));
        
        _preAttackStateR.Started += OnPreAttackStateStarted;
        _preAttackStateR.Ended += OnPreAttackStateEnded;
        _preAttackStateL.Started += OnPreAttackStateStarted;
        _preAttackStateL.Ended += OnPreAttackStateEnded;
        _deathFallState.Ended += OnDeathFallStateEnded;
        _spreadState.Ended += OnSpreadStateEnded;
        _patrolStateR.EnteredAttackRange += OnEnteredAttackRange;
        
        At(_patrolStateR, _preAttackStateR, () => _patrolStateR.IsReadyToSwitch);
        At(_preAttackStateR, _attackState, () => _preAttackStateR.IsReadyToSwitch);
        At(_patrolStateL, _preAttackStateL, () => _patrolStateL.IsReadyToSwitch);
        At(_preAttackStateL, _attackState, () => _preAttackStateL.IsReadyToSwitch);
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }

    private void OnDestroy()
    {
        _preAttackStateR.Started -= OnPreAttackStateStarted;
        _preAttackStateR.Ended -= OnPreAttackStateEnded;
        _preAttackStateL.Started -= OnPreAttackStateStarted;
        _preAttackStateL.Ended -= OnPreAttackStateEnded;
        _deathFallState.Ended -= OnDeathFallStateEnded;
        _spreadState.Ended -= OnSpreadStateEnded;
        _patrolStateR.EnteredAttackRange -= OnEnteredAttackRange;
        transform.parent = null;
    }

    private void OnEnteredAttackRange()
    {
        EnteredAttackRange?.Invoke();
    }

    public override void Play()
    {
        // Spawn position
        _sideDirection = RandomDirection.Generate();
        SideDirection = _sideDirection;
        Position2D = GenerateSpawnPosition(_radius);
        
        if (_sideDirection > 0)
        {
            _currentState = _patrolStateR;
        }
        else
        {
            _currentState = _patrolStateL;
        }
        
        _stateMachine.SetState(_currentState);
        
        _position3D = _currentState.Position2D;
        transform.position = _position3D;
        
        enabled = true;

    }

    public override void TriggerAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerFall()
    {
        transform.parent = null;
        SwitchToStateAndApply(_deathFallState);
        enabled = true;
    }

    public override void TriggerDeath()
    {
        transform.parent = null;
        SwitchToStateAndApply(_deathFallState);
        enabled = true;
    }

    public void TriggerSpread()
    {
        SwitchToStateAndApply(_spreadState);
    }

    private void Update()
    {
        _prevPosition = transform.position;
        
        _stateMachine.Tick();
        _currentState = (RegularEnemyMovementStateBase)_stateMachine.CurrentState;
        _stateDebug = _currentState.GetType().Name; // Debug only
        Position2D = _currentState.Position2D;
        
        
        // Add Depth later
        if (_isDepthEnabled)
        {
            DepthDirection = _currentState.DepthDirection;
            transform.position = (Vector3)Position2D + DepthDirection;
        }
        else
        {
            transform.position = Position2D;
        }
        
        // Add Smooth?
        
        Debug.DrawLine(_prevPosition, transform.position, Color.cyan, 10f);
    }

    private Vector2 GenerateSpawnPosition(float distance)
    {
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = distance;
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        spawnPosition = rotation * spawnPosition;
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


    // Sticky specific stuff
    public void TriggerStick(Transform target)
    {
        _currentState = _stickState;
        _stateMachine.SetState(_currentState); // Correct sticky position on enter

        if (_isDepthEnabled)
        {
            DepthDirection = _currentState.DepthDirection;
            transform.position = (Vector3)Position2D + DepthDirection;
        }
        else
        {
            transform.position = _currentState.Position2D;
        }
        
        transform.parent = target;
        enabled = false;
    }

    private void OnPreAttackStateStarted()
    {
        PreAttackStarted?.Invoke();
    }

    private void OnPreAttackStateEnded()
    {
        PreAttackEnded?.Invoke();
    }

    private void OnDeathFallStateEnded()
    {
        DeathStateEnded?.Invoke();
    }

    private void OnSpreadStateEnded()
    {
        SpreadStateEnded?.Invoke();
        Play();
    }
}
