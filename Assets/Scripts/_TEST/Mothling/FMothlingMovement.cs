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
    [SerializeField] private Vector3Int _sideDepthDirection = Vector3Int.one;
    
    private Vector3 _position3D;

    // State Machine fields
    private FStateMachine _stateMachine = new();
    private FMothlingMovementStateBase _currentState;
    private FMothlingMovementEnterState _enterState;
    private FMothlingMovementPatrolState _patrolState;
    private FMothlingMovementPreAttackState _preAttackState;
    private FMothlingMovementAttackState _attackState;
    private FMothlingMovementFallState _fallState;
    
    // State parameters
    private bool _isAttacking = false;
    private bool _isCollided = false;

    public Vector2 Position2D { get; private set; } 

    public override void Initialize()
    {
        Debug.Log("FMothlingMovement Initialize");
        _enterState = new FMothlingMovementEnterState();
        _patrolState = new FMothlingMovementPatrolState(this);
        _preAttackState = new FMothlingMovementPreAttackState(this);
        _attackState = new FMothlingMovementAttackState(this);
        _fallState = new FMothlingMovementFallState(this);
        
        // State transitions
        At(_enterState, _patrolState, () => _enterState.ReadyToSwitch);
        At(_patrolState, _preAttackState, IsAttackStarted());
        At(_preAttackState, _attackState, () => _preAttackState.ReadyToSwitch);
        At(_attackState, _fallState, IsCollided());
       
        
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
        
        Func<bool> IsCollided() => () =>
        {
            if (_isCollided)
            {
                _isCollided = false;
                return true;
            }
            return false;
        };
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);

        _currentState = _enterState;
        _stateMachine.SetState(_currentState);
    }


    public override void Play()
    {
        Debug.Log("FMothlingMovement Play");
        _sideDepthDirection.x = 1; //RandomDirection.Generate();
        _sideDepthDirection.z = 1; //RandomDirection.Generate();
        _isAttacking = false;
        _isCollided = false;
    }

    public override void TriggerAttack()
    {
        if (_currentState.Equals(_patrolState))
        {
            _isAttacking = true;
        }
    }

    public override void TriggerFall()
    {
        Debug.Log("FMothlingMovement TriggerFall");
        if (_currentState.Equals(_attackState))
        {
            _isCollided = true;
        }
    }

    private void Update()
    {
        _stateMachine.Tick();
        // Position2D = _currentState.Position2D;
        _currentState = (FMothlingMovementStateBase)_stateMachine.CurrentState;
        Position2D = _currentState.Position2D;
        
        
        // Add Noise
        // if (_isNoiseEnabled && _currentState.State == EnemyStates.Patrol)  
        if (_isNoiseEnabled)  
        {
            Vector3 trajectoryNoise1 = TrajectoryNoise.Generate(_noise1Frequency);
            Vector3 trajectoryNoise2 = TrajectoryNoise.Generate(_noise2Frequency);

            if (_currentState.Equals(_attackState))
            {
                float noiseMultiplier = 0.5f;
                if (Position2D.magnitude < 0.86f)
                {
                    noiseMultiplier = 0.001f;
                }   
                trajectoryNoise1 *= noiseMultiplier;
                trajectoryNoise2 *= noiseMultiplier;
            }
            
            // _position2d += trajectoryNoise * _noiseAmplitude;
            _position3D = (Vector3)Position2D + trajectoryNoise1 * _noise1Amplitude + trajectoryNoise2 * _noise2Amplitude;
        }
        else
        {
            _position3D = (Vector3)Position2D;
        }
        
        // Add Depth
        if (_isDepthEnabled)
        {
            _position3D += _currentState.DepthDirection;
        }
        
        _position3D.x *= _sideDepthDirection.x;
        _position3D.z *= _sideDepthDirection.z;
        transform.position = _position3D;
    }
}
