using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FMothMovement : FEnemyMovementBase, IPositionDirectionProvider
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [Header("---- Spawn Settings ----")]
    [SerializeField] private float _spawnXPos;
    [SerializeField] private float _spawnYPosMin;
    [SerializeField] private float _spawnYPosMax;
    [Header("---- Depth Settings ----")]
    [SerializeField] bool _isDepthEnabled;
    // Debug
    [SerializeField] private string _stateDebug;
    [SerializeField] private int _sideDirection = 1;
    [SerializeField] private int _depthSideDirection = 0;
    // State parameters
    private bool _isAttacking = false;
    // Debug
    private Vector3 _prevPosition;

    private readonly FStateMachine _stateMachine = new();
    private MothMovementStateFactory _stateFactory;
    // Movement States
    private RegularEnemyMovementStateBase _currentState;
    private FMothMovementEnterState _enterState;


    public void Construct(MothMovementStateFactory stateFactory)
    {
        _stateFactory = stateFactory;
    }

    public event Action PatrolStarted;
    public event Action PreAttackStarted;
    public event Action PreAttackEnded;
    public event Action DeathStateEnded;

    public Vector2 Position2D { get; private set; }
    public Vector3 DepthDirection { get; private set; }
    
    public int SideDirection => _sideDirection;

    public override void Initialize()
    {
        _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude);
        _enterState = (FMothMovementEnterState)_stateFactory.Create(typeof(FMothMovementEnterState));
    }
    
    public override void Play()
    {
        _sideDirection = RandomDirection.Generate();
        _depthSideDirection = RandomDirection.Generate();
        Position2D = GenerateSpawnPosition(_sideDirection, _spawnXPos, _spawnYPosMin, _spawnYPosMax);
        transform.position = Position2D;
        
        _currentState = _enterState;
        _stateMachine.SetState(_currentState);
        
        _isAttacking = false;
        enabled = true;
    }

    public override void TriggerAttack()
    {
        throw new NotImplementedException();
    }

    public override void TriggerFall()
    {
        throw new NotImplementedException();
    }

    public override void TriggerDeath()
    {
        throw new NotImplementedException();
    }

    public override void TriggerSpread()
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        _prevPosition = transform.position;
        
        _stateMachine.Tick();
        _currentState = (RegularEnemyMovementStateBase)_stateMachine.CurrentState;
        _stateDebug = _currentState.GetType().Name;
        Position2D = _currentState.Position2D;
        DepthDirection = _currentState.DepthDirection;
        
        if (_isDepthEnabled)
        {
            transform.position = (Vector3)Position2D + DepthDirection;
        }
        else
        {
            transform.position = Position2D;
        }
        
        Debug.DrawLine(_prevPosition, transform.position, Color.cyan, 5f);
    }

    private Vector2 GenerateSpawnPosition(int direction, float xPos, float yPosMin, float yPosMax)
    {
        Vector2 spawnPositionSide = Vector2.zero;
        spawnPositionSide.x = xPos * direction;
        spawnPositionSide.y = Random.Range(yPosMin, yPosMax) * RandomDirection.Generate();
        
        Vector2 spawnPositionTopBottom = Vector3.zero;
        spawnPositionTopBottom.x = Random.Range(-xPos, xPos);
        spawnPositionTopBottom.y = yPosMax * RandomDirection.Generate();
        
        if (Random.Range(0, 2) == 0)
        {
            return spawnPositionSide;
        }
        else
        {
            return spawnPositionTopBottom;
        }
    }
}
