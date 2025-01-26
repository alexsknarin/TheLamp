using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FMothlingMovement : FEnemyMovementBase, IPosition2DProvider
{
    [Header("-- Movement States Base Settings --")]
    [SerializeField] private float _speed;
    [SerializeField] private float _radius;
    [SerializeField] private float _verticalAmplitude;
    [Header("---- Spawn Settings ----")]
    [SerializeField] private float _spawnAreaSize = 0.5f;
    [SerializeField] private Vector2 _spawnAreaCenter;
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
    [SerializeField] private String _stateDebug;
    [SerializeField] private Vector3Int _sideDepthDirection = Vector3Int.one;
    
    private Vector3 _position3D;
    // Debug only
    private Vector3 _prevPosition;
    private Vector3 _prevPosSmooth;
    private Vector3 _velocity = Vector3.zero;
    

    // State Machine fields
    private FStateMachine _stateMachine = new();
    private MothlingMovementStateFactory _stateFactory;
    
    private FMothlingMovementStateBase _currentState;
    private FMothlingMovementEnterState _enterState;
    private FMothlingMovementPatrolState _patrolState;
    private FMothlingMovementPreAttackState _preAttackState;
    private FMothlingMovementAttackState _attackState;
    private FMothlingMovementFallState _fallState;
    
    // State parameters
    private bool _isAttacking = false;
    private bool _isCollided = false;
    
    public void Construct(MothlingMovementStateFactory stateFactory)
    {
        _stateFactory = stateFactory;
    }

    public Vector2 Position2D { get; private set; } 

    public override void Initialize()
    {
        Debug.Log("FMothlingMovement Initialize");
        _stateFactory.SetEnemyDependencies(this, _speed, _radius, _verticalAmplitude);
        _enterState = (FMothlingMovementEnterState)_stateFactory.Create(typeof(FMothlingMovementEnterState));
        _patrolState = (FMothlingMovementPatrolState)_stateFactory.Create(typeof(FMothlingMovementPatrolState));
        _preAttackState = (FMothlingMovementPreAttackState)_stateFactory.Create(typeof(FMothlingMovementPreAttackState));
        _attackState = (FMothlingMovementAttackState)_stateFactory.Create(typeof(FMothlingMovementAttackState));
        _fallState = (FMothlingMovementFallState)_stateFactory.Create(typeof(FMothlingMovementFallState));
        
        // State transitions
        At(_enterState, _patrolState, () => _enterState.ReadyToSwitch);
        At(_patrolState, _preAttackState, IsAttackStarted());
        At(_preAttackState, _attackState, () => _preAttackState.ReadyToSwitch);
        At(_attackState, _fallState, IsCollided());
        At(_fallState, _enterState, IsFallEnded());
       
        
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
        
        Func<bool> IsFallEnded() => () =>
        {
            if (_fallState.ReadyToSwitch)
            {
                Debug.Log("FMothlingMovement IsFallEnded: Position2D: " + transform.position);
                _sideDepthDirection.x = -(int)Mathf.Sign(transform.position.x);
                // _sideDepthDirection.z = RandomDirection.Generate();
                return true;
            }
            return false;
        };
        
        
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
    }


    public override void Play()
    {
        _sideDepthDirection.x = 1; //RandomDirection.Generate();
        _sideDepthDirection.z = 1; //RandomDirection.Generate();
        Position2D = GenerateSpawnPosition(-1);
        
        _currentState = _enterState;
        _stateMachine.SetState(_currentState);
        
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
        if (_currentState.Equals(_attackState))
        {
            _isCollided = true;
        }
    }

    private void Update()
    {
        // Debug only
        _prevPosition = _position3D;
        _prevPosSmooth = transform.position;

        
        _stateMachine.Tick();
        _currentState = (FMothlingMovementStateBase)_stateMachine.CurrentState;
        _stateDebug = _currentState.GetType().Name; // Debug only
        Position2D = _currentState.Position2D;
        
        // Add Noise
        if (_isNoiseEnabled)
        {
            AddMotionNoise();
        }
        else
        {
            _position3D = Position2D;
        }
        
        // Add Depth
        if (_isDepthEnabled)
        {
            _position3D += _currentState.DepthDirection;
        }
        
        // Apply side and depth directions
        _position3D.x *= _sideDepthDirection.x;
        _position3D.z *= _sideDepthDirection.z;
        
        // Add SmoothDamp
        if (_isSmoothDampEnabled)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _position3D, ref _velocity, _smoothTime);
        }
        else
        {
            transform.position = _position3D;
        }
        
        
        Debug.DrawLine(_prevPosition, _prevPosition + (_position3D-_prevPosition).normalized*0.02f, Color.cyan, 5f);
        Debug.DrawLine(_prevPosSmooth, _prevPosSmooth + (transform.position-_prevPosSmooth).normalized*0.02f, Color.yellow, 5f);
    }

    private void AddMotionNoise()
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

    private Vector2 GenerateSpawnPosition(int direction)
    {
        Vector2 spawnPosition = Random.insideUnitCircle * _spawnAreaSize + _spawnAreaCenter;
        spawnPosition = _spawnAreaCenter; // TODO: remove
        spawnPosition.x *= direction;
        return spawnPosition;
    }
}
