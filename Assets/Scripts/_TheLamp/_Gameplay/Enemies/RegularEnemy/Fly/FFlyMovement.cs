using System;
using UnityEngine;

public class FFlyMovement : MonoBehaviour
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
    [SerializeField] private float _noiseFrequency;
    [SerializeField] private float _noiseAmplitude;
    [Header("-- Smooth Damp Settings --")]
    [SerializeField] private bool _isSmoothDampEnabled;
    [SerializeField] private float _smoothTime = .3f;
    private float _smoothTimeAllowed = 0;
    [Header("---- Depth Settings ----")]
    [SerializeField] bool _isDepthEnabled;
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
    private FlyMovementStateFactory _stateFactory;
    // States
    private FFlyMovementStateBase _currentState;
    private FFlyMovementEnterState _enterState;
    
    // private FMothlingMovementEnterState _enterState;
    // private FMothlingMovementPatrolState _patrolState;
    // private FMothlingMovementPreAttackState _preAttackState;
    // private FMothlingMovementAttackState _attackState;
    // private FMothlingMovementFallState _fallState;
    // private FMothlingMovementDeathState _deathState;
    // private FMothlingMovementSpreadState _spreadState;
    
    // State parameters
    private bool _isAttacking = false;
    private WaitForSeconds _waitSmoothDamp = new(0.5f);
    
    public void Construct(FlyMovementStateFactory stateFactory)
    {
        _stateFactory = stateFactory;
    }
    
    public event Action PatrolStarted;
    public event Action PreAttackStarted;
    public event Action PreAttackEnded;
    public event Action DeathStateEnded;

    public Vector2 Position2D { get; private set; } 
    public Vector3 DepthDirection { get; private set; } 
}
