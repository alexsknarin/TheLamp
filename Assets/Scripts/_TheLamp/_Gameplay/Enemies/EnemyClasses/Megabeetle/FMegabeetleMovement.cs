using UnityEngine;

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
    
    
    public void Construct(
        MegabeetleMovementStateFactory stateFactory) 
    {
        _stateFactory = stateFactory;
    }
    
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
        
        
    }

    public override void Play()
    {
        throw new System.NotImplementedException();
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


}
