using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FLadybugMovement : FEnemyMovementBase, IPositionDirectionProvider
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private bool _isDepthEnabled;
    // Debug
    [SerializeField] private string _stateDebug;
    [SerializeField] private int _sideDirection = 1;
    [SerializeField] private int _depthSideDirection = 0;
    
    private Vector3 _position3D;
    // Debug only
    private Vector3 _prevPosition;
    private Vector3 _prevPosSmooth;
    private Vector3 _velocity = Vector3.zero;
    
    
    // State Machine
    private readonly FStateMachine _stateMachine = new();
    private LadybugMovementStateFactory _stateFactory;
    private RegularEnemyMovementStateBase _currentState;
    
    private FLadybugMovementPatrolState _patrolState;
    private FLadybugMovementPreAttackState _preAttackState;
    private FLadybugMovementAttackState _attackState;
    
    
    public void Construct(
        LadybugMovementStateFactory stateFactory) 
    {
        _stateFactory = stateFactory;
    }
    
    public Vector2 Position2D { get; private set; }
    public Vector3 DepthDirection { get; private set; }
    
    
    public override void Initialize()
    {
        _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude, 0.125f);
        _patrolState = (FLadybugMovementPatrolState)_stateFactory.Create(typeof(FLadybugMovementPatrolState));
        _preAttackState = (FLadybugMovementPreAttackState)_stateFactory.Create(typeof(FLadybugMovementPreAttackState));
        _attackState = (FLadybugMovementAttackState)_stateFactory.Create(typeof(FLadybugMovementAttackState));
        
        At(_patrolState, _preAttackState, () => _patrolState.IsReadyToSwitch);
        At(_preAttackState, _attackState, () => _preAttackState.IsReadyToSwitch);
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }

    public override void Play()
    {
        
        // Spawn position

        _sideDirection = 1;//RandomDirection.Generate();
        SideDirection = _sideDirection;
        _depthSideDirection = 1;//RandomDirection.Generate();
        Position2D = GenerateSpawnPosition(_radius);
        
        
        _currentState = _patrolState;
        _stateMachine.SetState(_currentState);
        
        _position3D = _currentState.Position2D;
        transform.position = _position3D;
        
        
        
    }

    public override void TriggerAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerFall()
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerDeath()
    {
        throw new System.NotImplementedException();
    }

    public override void TriggerSpread()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        _prevPosition = transform.position;
        
        _stateMachine.Tick();
        _currentState = (RegularEnemyMovementStateBase)_stateMachine.CurrentState;
        _stateDebug = _currentState.GetType().Name; // Debug only
        Position2D = _currentState.Position2D;
        
        // Add Depth later
        // Add Smooth?
        
        Vector2 newPosition = Position2D;
        newPosition.x *= _sideDirection;
        
        transform.position = newPosition;
        
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
}
