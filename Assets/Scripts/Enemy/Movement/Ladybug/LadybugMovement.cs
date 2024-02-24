using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LadybugMovement : MonoBehaviour, IStateMachineOwner, IPreAttackStateProvider, IDeathStateProvider
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private bool _isDepthEnabled;

    // Movement States
    private EnemyMovementStateMachine _movementStateMachine;
    private EnemyMovementBaseState _currentState;
    private LadybugMovementPatrolState _patrolState;
    private LadybugMovementPreAttackState _preAttackState;
    private LadybugMovementAttackState _attackState;
    private LadybugMovementStickState _stickState;
    private LadybugMovementFallState _fallState;
    
    private int _sideDirection;
    private int _depthDirection;
    private Vector3 _position2d;
    private Vector3 _prevPosition2d;
    

    public event Action OnPreAttackStart;
    public event Action OnPreAttackEnd;
    public event Action OnDeath;
    
    public void SwitchState()
    {
        EnemyMovementBaseState newState = _currentState.State switch
        {
            EnemyStates.Patrol => StartPreAttackState(),
            EnemyStates.PreAttack => StartAttackState(),
            EnemyStates.Attack => _stickState,
            EnemyStates.Stick => _fallState,
            EnemyStates.Fall => _patrolState,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _currentState = newState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, _depthDirection);
    }
    
    private EnemyMovementBaseState StartPreAttackState()
    {
        OnPreAttackStart?.Invoke();
        return _preAttackState;
    }
    
    private EnemyMovementBaseState StartAttackState()
    {
        OnPreAttackEnd?.Invoke();
        return _attackState;
    }

    private void Start()
    {
        Init();
    }
    
    private Vector3 GenerateSpawnPosition(float distance)
    {
        Vector3 spawnPosition = Vector3.zero;
        spawnPosition.x = distance;
        Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        spawnPosition = rotation * spawnPosition;
        return spawnPosition;
    }
    
    private void Spawn()
    {
        _position2d = GenerateSpawnPosition(_radius);
    }

    private void Init()
    {
        _sideDirection = RandomDirection.Generate();
        _depthDirection = RandomDirection.Generate();
        
        _movementStateMachine = new EnemyMovementStateMachine();
        _patrolState  = new LadybugMovementPatrolState(this, _speed, _radius, _verticalAmplitude);
        _preAttackState = new LadybugMovementPreAttackState(this, _speed, _radius, _verticalAmplitude);
        _attackState = new LadybugMovementAttackState(this, _speed, _radius, _verticalAmplitude);
        _stickState = new LadybugMovementStickState(this, _speed, _radius, _verticalAmplitude);
        _fallState = new LadybugMovementFallState(this, _speed, _radius, _verticalAmplitude);

        Spawn();
       
        _currentState = _patrolState;
        _movementStateMachine.SetState(_currentState, _position2d, _sideDirection, 1);
        _position2d = _currentState.Position;
        transform.position = _position2d;
    }

    private void Update()
    {
        _prevPosition2d = _position2d;
        _currentState.ExecuteState(_position2d);
        _position2d = _currentState.Position;
        
        Vector3 position = _position2d;
        
        // Add Depth
        if (_isDepthEnabled)
        {
            position = _position2d + _currentState.Depth;
        }
        
        _movementStateMachine.CheckForStateChange();
        transform.position = position;
        

        Debug.DrawLine(_prevPosition2d, _prevPosition2d + (_position2d-_prevPosition2d).normalized*0.02f, Color.cyan, 5f);
    }
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_currentState.State == EnemyStates.Attack && other.CompareTag("LadybugStickTrigger"))
        {
            SwitchState();
        }
    }
}
