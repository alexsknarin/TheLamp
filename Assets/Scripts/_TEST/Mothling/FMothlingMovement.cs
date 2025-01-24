using System;
using UnityEngine;

public class FMothlingMovement : FEnemyMovementBase, IPosition2DProvider
{
    [Header("-- Movement Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [Header("---- Spawn Settings ----")]
    [SerializeField] private float _spawnAreaSize = 0.5f;
    [SerializeField] private Vector3 _spawnAreaCenter;
    [Header("---- Noise Settings ----")]
    [SerializeField] private bool _isNoiseEnabled;
    [SerializeField] private float _noise1Frequency;
    [SerializeField] private float _noise1Amplitude;
    [SerializeField] private float _noise2Frequency;
    [SerializeField] private float _noise2Amplitude;
    [Header("-- Smooth Damp Settings --")]
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private float _smoothTime = .3f;
    [Header("---- Depth Settings ----")]
    [SerializeField] bool _isDepthEnabled;
    // Debug
    [SerializeField] private EnemyState _stateDebug;

    // State Machine fields
    private FStateMachine _stateMachine = new();
    private FMothlingMovementStateBase _currentState;
    private FMothlingMovementEnterStateRIn _enterState;
    private FMothlingMovementPatrolStateRIn _patrolState;

    public Vector2 Position2D { get; private set; } 

    public override void Initialize()
    {
        Debug.Log("FMothlingMovement Initialize");
        _enterState = new FMothlingMovementEnterStateRIn();
        _patrolState = new FMothlingMovementPatrolStateRIn(this);
        
        // State transitions
        At(_enterState, _patrolState, () => _enterState.ReadyToSwitch);
       
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        
        // Predicates
        // Func<bool> IsCurrentStateFinished() => () =>
        // {
        //     if (_currentState.ReadyToSwitch)
        //     {
        //         _currentState = _patrolState;
        //         Debug.Log("Switched to Patrol");
        //         return true;
        //     }
        //     return false;
        // };
        
        // Default values set
        
        _currentState = _enterState;
        _stateMachine.SetState(_currentState);
    }


    public override void Play()
    {
        Debug.Log("FMothlingMovement Play");
    }

    public override void TriggerAttack()
    {
        Debug.Log("FMothlingMovement TriggerAttack");
    }

    public override void TriggerFall()
    {
        Debug.Log("FMothlingMovement TriggerFall");
    }

    private void Update()
    {
        _stateMachine.Tick();
        // Position2D = _currentState.Position2D;
        _currentState = (FMothlingMovementStateBase)_stateMachine.CurrentState;
        Position2D = _currentState.Position2D;
        transform.position = (Vector3)Position2D + _currentState.DepthDirection;
    }
}
