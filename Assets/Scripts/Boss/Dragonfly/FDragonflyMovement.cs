using System;
using UnityEngine;
using Object = System.Object;

public class FDragonflyMovement : MonoBehaviour
{
    [SerializeField] private string _currentStateType;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private Transform _animatedTransform;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private Transform _spiderPatrolTransform;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private DragonflyPatrolRotator _spiderPatrolRotator;
    [SerializeField] private DragonflyAnimationClipEventHandler _animationClipEvents;
    [SerializeField] private Transform _fallPoint;
    [Header("States")]  
    [SerializeField] private FDragonflyAttackHeadState _attackHeadState;
    [SerializeField] private FDragonflyAttackHeadSuccessState _attackHeadSuccessState; 
    [SerializeField] private FDragonflyAttackHoverState _attackHoverState;
    [SerializeField] private FDragonflyAttackTailFailStateL _attackTailFailL;
    [SerializeField] private FDragonflyAttackTailFailStateR _attackTailFailR;
    [SerializeField] private FDragonflyAttackTailStateL _attackTailStateL;
    [SerializeField] private FDragonflyAttackTailStateR _attackTailStateR;
    [SerializeField] private FDragonflyAttackTailSuccessStateL _attackTailSuccessL;
    [SerializeField] private FDragonflyAttackTailSuccessStateR _attackTailSuccessR;
    [SerializeField] private FDragonflyBounceHeadState _bounceHeadState;
    [SerializeField] private FDragonflyBounceHoverState _bounceHoverState;
    [SerializeField] private FDragonflyBounceTailStateL _bounceTailStateL;
    [SerializeField] private FDragonflyBounceTailStateR _bounceTailStateR;
    [SerializeField] private FDragonflyCatchSpiderStateL _catchSpiderStateL;
    [SerializeField] private FDragonflyCatchSpiderStateR _catchSpiderStateR;
    [SerializeField] private FDragonflyDeathHeadState _deathHeadState;
    [SerializeField] private FDragonflyDeathTailStateL _deathTailStateL;
    [SerializeField] private FDragonflyDeathTailStateR _deathTailStateR;
    [SerializeField] private FDragonflyEnterToHoverStateL _enterToHoverStateL;
    [SerializeField] private FDragonflyEnterToHoverStateR _enterToHoverStateR;
    [SerializeField] private FDragonflyEnterToPatrolStateL _enterToPatrolStateL;
    [SerializeField] private FDragonflyEnterToPatrolStateR _enterToPatrolStateR;
    [SerializeField] private FDragonflyFallHeadState _fallHeadState;
    [SerializeField] private FDragonflyHoverState _hoverState;
    [SerializeField] private FDragonflyGameoverHoverState _gameoverHoverState;
    [SerializeField] private FDragonflyIdleState _idleState;
    [SerializeField] private FDragonflyMoveToHoverState _moveToHoverState;
    [SerializeField] private FDragonflyMoveToPatrolStateL _moveToPatrolStateL;
    [SerializeField] private FDragonflyMoveToPatrolStateR _moveToPatrolStateR;
    [SerializeField] private FDragonflyPatrolStateL _patrolStateL;
    [SerializeField] private FDragonflyPatrolStateR _patrolStateR;
    [SerializeField] private FDragonflyPreAttackHeadStateL _preAttackHeadStateL;
    [SerializeField] private FDragonflyPreAttackHeadStateR _preAttackHeadStateR;
    [SerializeField] private FDragonflyPreAttackHoverState _preAttackHoverState;
    [SerializeField] private FDragonflyPreAttackTailStateL _preAttackTailStateL;
    [SerializeField] private FDragonflyPreAttackTailStateR _preAttackTailStateR;
    [SerializeField] private FDragonflyReturnHoverState _returnHoverState;
    [SerializeField] private FDragonflyReturnTransitionLRBTState _returnTransitionLRBTState;
    [SerializeField] private FDragonflyReturnTransitionLRTBState _returnTransitionLRTBState;
    [SerializeField] private FDragonflyReturnTransitionRLBTState _returnTransitionRLBTState;
    [SerializeField] private FDragonflyReturnTransitionRLTBState _returnTransitionRLTBState;
    [SerializeField] private FDragonflySpiderPatrolStateL _spiderPatrolStateL;
    [SerializeField] private FDragonflySpiderPatrolStateR _spiderPatrolStateR;
    [SerializeField] private FDragonflySpiderPreattackHeadTransitionStateL _spiderPreAttackHeadTransitionStateL;
    [SerializeField] private FDragonflySpiderPreattackHeadTransitionStateR _spiderPreAttackHeadTransitionStateR;
    [SerializeField] private FDragonflySpiderPushStateL _spiderPushStateL;
    [SerializeField] private FDragonflySpiderPushStateR _spiderPushStateR;
    
    // Events
    public event Action<IState> OnReadyToAttackStateEnteredEvent; // TODO: possibly Use Type instead of IState 
    public event Action<IState> OnReadyToSwarmAttackStateEnteredEvent;
    public event Action<IState> OnAfterAttackExitEndedEvent; // TODO: possibly Use Type instead of IState
    public event Action OnAttackStartedEvent;
    public event Action OnPreAttackStartedEvent;
    public event Action OnAttackEndedEvent;
    public event Action<int> OnCatchSpiderStartedEvent;
    public event Action OnReadyToSpiderAttackStateEnteredEvent;
    public event Action OnDeathAnimationEndedEvent;
    public event Action OnSwarmCallEvent;

    private FStateMachine _stateMachine = new();
    public IState MovementState => _stateMachine.CurrentState;
    
    // Animation 
    private readonly int _idleHash = Animator.StringToHash("Idle");
    private readonly int _enterToPatrolLHash = Animator.StringToHash("EnterToPatrolL");
    private readonly int _enterToPatrolRHash = Animator.StringToHash("EnterToPatrolR");
    private readonly int _catchSpiderLHash = Animator.StringToHash("CatchSpiderL");
    private readonly int _catchSpiderRHash = Animator.StringToHash("CatchSpiderR");
    private readonly int _enterToHoverLHash = Animator.StringToHash("EnterToHoverL");
    private readonly int _enterToHoverRHash = Animator.StringToHash("EnterToHoverR");
    private readonly int _moveToPatrolLHash = Animator.StringToHash("MoveToPatrolL");
    private readonly int _moveToPatrolRHash = Animator.StringToHash("MoveToPatrolR");
    private readonly int _returnTransitionLRBTHash = Animator.StringToHash("ReturnTransitionLRBT");
    private readonly int _returnTransitionLRTBHash = Animator.StringToHash("ReturnTransitionLRTB");
    private readonly int _returnTransitionRLBTHash = Animator.StringToHash("ReturnTransitionRLBT");
    private readonly int _returnTransitionRLTBHash = Animator.StringToHash("ReturnTransitionRLTB");
    
    // Attack Modes
    private bool _isAttacking = false;
    private DragonflyPatrolAttackMode _currentPatrolAttackMode;
    
    // Tracking previous state
    private IState _previousState;
    private DragonflyReturnMode _resolvedReturnMode;

    
    // Return Resolve
    private int _returnSideDirection;

    private bool _isPlaying = false;
    private bool _isAnimClipEnded = false;
    private bool _isBounced = false;
    private DragonflyEnterType _enterState = 0;
    private int _sideDirection = 1;
    private bool _isAttackSuccess;
    private bool _isAttackFail;
    private bool _isDead;

    private void OnEnable()
    {
        _animationClipEvents.OnClipEndedEvent += OnClipEndedHandle;
        _animationClipEvents.OnSwarmCallEvent += OnSwarmCallHandle;
        _spiderPushStateL.OnEndedEvent += OnSwarmCallHandle;
        
        _hoverState.OnStartedEvent += OnReadyToAttackEnterHandle;
        _patrolStateL.OnStartedEvent += OnReadyToAttackEnterHandle; // 
        _patrolStateR.OnStartedEvent += OnReadyToAttackEnterHandle; //
        
        _patrolStateL.OnStartedEvent += OnReadyToSwarmAttackEnterHandle;
        _patrolStateR.OnStartedEvent += OnReadyToSwarmAttackEnterHandle;
        
        _spiderPatrolStateL.OnStartedEvent += OnReadyToSpiderAttackHandle;
        _spiderPatrolStateR.OnStartedEvent += OnReadyToSpiderAttackHandle;
        
        _catchSpiderStateL.OnStartedEvent += OnCatchSpiderStartedLHandle;
        _catchSpiderStateR.OnStartedEvent += OnCatchSpiderStartedRHandle;
        
        _preAttackHeadStateL.OnStartedEvent += OnPreAttackStartedHandle;
        _preAttackHeadStateR.OnStartedEvent += OnPreAttackStartedHandle;
        _preAttackTailStateL.OnStartedEvent += OnPreAttackStartedHandle;
        _preAttackTailStateR.OnStartedEvent += OnPreAttackStartedHandle;
        _preAttackHoverState.OnStartedEvent += OnPreAttackStartedHandle;
        
        _attackHeadState.OnStartedEvent += OnAttackStartedHandle;
        _attackTailStateL.OnStartedEvent += OnAttackStartedHandle;
        _attackTailStateR.OnStartedEvent += OnAttackStartedHandle;
        _attackHoverState.OnStartedEvent += OnAttackStartedHandle;
        
        _deathHeadState.OnStartedEvent += OnAttackEndedHandle;
        _deathTailStateL.OnStartedEvent += OnAttackEndedHandle;
        _deathTailStateR.OnStartedEvent += OnAttackEndedHandle;
        _attackHeadSuccessState.OnStartedEvent += OnAttackEndedHandle;
        _fallHeadState.OnStartedEvent += OnAttackEndedHandle;
        _attackTailFailL.OnStartedEvent += OnAttackEndedHandle;
        _attackTailFailR.OnStartedEvent += OnAttackEndedHandle;
        _attackTailSuccessL.OnStartedEvent += OnAttackEndedHandle;
        _attackTailSuccessR.OnStartedEvent += OnAttackEndedHandle;
        _returnHoverState.OnStartedEvent += OnAttackEndedHandle;
        
        _attackHeadSuccessState.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _attackTailSuccessL.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _attackTailSuccessR.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _attackTailFailL.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _attackTailFailR.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _fallHeadState.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _returnHoverState.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _deathHeadState.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _deathTailStateL.OnEndedEvent += OnAfterAttackExitEndedHandle;
        _deathTailStateR.OnEndedEvent += OnAfterAttackExitEndedHandle;
        
        _deathHeadState.OnEndedEvent += OnDeathAnimationEndedHandle;
        _deathTailStateL.OnEndedEvent += OnDeathAnimationEndedHandle;
        _deathTailStateR.OnEndedEvent += OnDeathAnimationEndedHandle;
    }

    private void OnDisable()
    {
        _animationClipEvents.OnClipEndedEvent -= OnClipEndedHandle;
        _animationClipEvents.OnSwarmCallEvent -= OnSwarmCallHandle;
        _spiderPushStateL.OnEndedEvent -= OnSwarmCallHandle;
        
        _hoverState.OnStartedEvent -= OnReadyToAttackEnterHandle;
        _patrolStateL.OnStartedEvent -= OnReadyToAttackEnterHandle; // 
        _patrolStateR.OnStartedEvent -= OnReadyToAttackEnterHandle; //
        
        _patrolStateL.OnStartedEvent -= OnReadyToSwarmAttackEnterHandle;
        _patrolStateR.OnStartedEvent -= OnReadyToSwarmAttackEnterHandle;
        
        _spiderPatrolStateL.OnStartedEvent -= OnReadyToSpiderAttackHandle;
        _spiderPatrolStateR.OnStartedEvent -= OnReadyToSpiderAttackHandle;
        
        _catchSpiderStateL.OnStartedEvent -= OnCatchSpiderStartedLHandle;
        _catchSpiderStateR.OnStartedEvent -= OnCatchSpiderStartedRHandle;
        
        _preAttackHeadStateL.OnStartedEvent -= OnPreAttackStartedHandle;
        _preAttackHeadStateR.OnStartedEvent -= OnPreAttackStartedHandle;
        _preAttackTailStateL.OnStartedEvent -= OnPreAttackStartedHandle;
        _preAttackTailStateR.OnStartedEvent -= OnPreAttackStartedHandle;
        _preAttackHoverState.OnStartedEvent -= OnPreAttackStartedHandle;
        
        _attackHeadState.OnStartedEvent -= OnAttackStartedHandle;
        _attackTailStateL.OnStartedEvent -= OnAttackStartedHandle;
        _attackTailStateR.OnStartedEvent -= OnAttackStartedHandle;
        _attackHoverState.OnStartedEvent -= OnAttackStartedHandle;
        
        _deathHeadState.OnStartedEvent -= OnAttackEndedHandle;
        _deathTailStateL.OnStartedEvent -= OnAttackEndedHandle;
        _deathTailStateR.OnStartedEvent -= OnAttackEndedHandle;
        _attackHeadSuccessState.OnStartedEvent -= OnAttackEndedHandle;
        _fallHeadState.OnStartedEvent -= OnAttackEndedHandle;
        _attackTailFailL.OnStartedEvent -= OnAttackEndedHandle;
        _attackTailFailR.OnStartedEvent -= OnAttackEndedHandle;
        _attackTailSuccessL.OnStartedEvent -= OnAttackEndedHandle;
        _attackTailSuccessR.OnStartedEvent -= OnAttackEndedHandle;
        _returnHoverState.OnStartedEvent -= OnAttackEndedHandle;
        
        _attackHeadSuccessState.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _attackTailSuccessL.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _attackTailSuccessR.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _attackTailFailL.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _attackTailFailR.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _fallHeadState.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _returnHoverState.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _deathHeadState.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _deathTailStateL.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        _deathTailStateR.OnEndedEvent -= OnAfterAttackExitEndedHandle;
        
        _deathHeadState.OnEndedEvent -= OnDeathAnimationEndedHandle;
        _deathTailStateL.OnEndedEvent -= OnDeathAnimationEndedHandle;
        _deathTailStateR.OnEndedEvent -= OnDeathAnimationEndedHandle;
    }

    private void Awake() // TODO: Move to Initialize ????
    {
        _isPlaying = false;
        SetMovementStatesDependencies();
        SetupStateMachine();
    }

    private void SetMovementStatesDependencies()
    { 
        _attackHeadState.SetDependencies(_visibleBodyTransform, transform);
        _attackHeadSuccessState.SetDependencies(_visibleBodyTransform, _fallPoint); 
        _attackHoverState.SetDependencies(_visibleBodyTransform, transform);
        _attackTailFailL.SetDependencies(_visibleBodyTransform, transform);
        _attackTailFailR.SetDependencies(_visibleBodyTransform, transform);
        _attackTailStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _attackTailStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _attackTailSuccessL.SetDependencies(_visibleBodyTransform, transform);
        _attackTailSuccessR.SetDependencies(_visibleBodyTransform, transform);
        _bounceHeadState.SetDependencies(_visibleBodyTransform, transform);
        _bounceHoverState.SetDependencies(_visibleBodyTransform, transform);
        _bounceTailStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _bounceTailStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _catchSpiderStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _catchSpiderLHash);
        _catchSpiderStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _catchSpiderRHash);
        _deathHeadState.SetDependencies(_visibleBodyTransform, _fallPoint);
        _deathTailStateL.SetDependencies(_visibleBodyTransform, _fallPoint);
        _deathTailStateR.SetDependencies(_visibleBodyTransform, _fallPoint);
        _enterToHoverStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _enterToHoverLHash);
        _enterToHoverStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _enterToHoverRHash);
        _enterToPatrolStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _enterToPatrolLHash);
        _enterToPatrolStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _enterToPatrolRHash);
        _fallHeadState.SetDependencies(_visibleBodyTransform, _fallPoint);
        _hoverState.SetDependencies(_visibleBodyTransform, transform);
        _gameoverHoverState.SetDependencies(_visibleBodyTransform, transform);
        _idleState.SetDependencies(_visibleBodyTransform, transform);
        _moveToHoverState.SetDependencies(_visibleBodyTransform, transform);
        _moveToPatrolStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _moveToPatrolLHash);
        _moveToPatrolStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _moveToPatrolRHash);
        _patrolStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _patrolStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _preAttackHeadStateL.SetDependencies(_visibleBodyTransform, transform);
        _preAttackHeadStateR.SetDependencies(_visibleBodyTransform, transform);
        _preAttackHoverState.SetDependencies(_visibleBodyTransform, transform);
        _preAttackTailStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _preAttackTailStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _returnHoverState.SetDependencies(_visibleBodyTransform, transform);
        _returnTransitionRLBTState.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _returnTransitionRLBTHash);
        _returnTransitionLRBTState.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _returnTransitionLRBTHash);
        _returnTransitionRLTBState.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _returnTransitionRLTBHash);
        _returnTransitionLRTBState.SetDependencies(_visibleBodyTransform, _animatedTransform, _animator, _returnTransitionLRTBHash);
        _spiderPatrolStateL.SetDependencies(_visibleBodyTransform, _spiderPatrolTransform, _spiderPatrolRotator);
        _spiderPatrolStateR.SetDependencies(_visibleBodyTransform, _spiderPatrolTransform, _spiderPatrolRotator);
        _spiderPreAttackHeadTransitionStateL.SetDependencies(
            _visibleBodyTransform, _patrolTransform, _spiderPatrolTransform, _patrolRotator, _spiderPatrolRotator);
        _spiderPreAttackHeadTransitionStateR.SetDependencies(
            _visibleBodyTransform, _patrolTransform, _spiderPatrolTransform, _patrolRotator, _spiderPatrolRotator);
        _spiderPushStateL.SetDependencies(_visibleBodyTransform, _spiderPatrolTransform, _spiderPatrolRotator);
        _spiderPushStateR.SetDependencies(_visibleBodyTransform, _spiderPatrolTransform, _spiderPatrolRotator);
    }

    private void SetupStateMachine()
    {
        // Idle -> Enter States
        At(_idleState, _enterToPatrolStateL, () => _isPlaying && _enterState == DragonflyEnterType.Patrol && _sideDirection == 1);
        At(_idleState, _enterToPatrolStateR, () => _isPlaying && _enterState == DragonflyEnterType.Patrol && _sideDirection == -1);
        At(_idleState, _enterToHoverStateL, () => _isPlaying && _enterState == DragonflyEnterType.Hover && _sideDirection == 1);
        At(_idleState, _enterToHoverStateR, () => _isPlaying && _enterState == DragonflyEnterType.Hover && _sideDirection == -1);
        
        // Enter -> Patrol
        At(_enterToPatrolStateL, _patrolStateL, IsAnimationEnded());
        At(_enterToPatrolStateR, _patrolStateR, IsAnimationEnded());
        At(_moveToPatrolStateL, _patrolStateL, IsAnimationEnded());
        At(_moveToPatrolStateR, _patrolStateR, IsAnimationEnded());
        
        
        At(_enterToHoverStateL, _hoverState, IsAnimationEnded());
        At(_enterToHoverStateR, _hoverState, IsAnimationEnded());
        At(_moveToHoverState, _hoverState, () => _moveToHoverState.ReadyToSwitch);
        
        // Patrol -> PreAttack
        At(_patrolStateL, _preAttackHeadStateL,  IsStartHeadAttack());
        At(_patrolStateR, _preAttackHeadStateR,  IsStartHeadAttack());
        At(_patrolStateL, _preAttackTailStateL,  IsStartTailAttack());
        At(_patrolStateR, _preAttackTailStateR,  IsStartTailAttack());
        
        // PreAttack -> Attack
        At(_preAttackHeadStateL, _attackHeadState, ()  => _preAttackHeadStateL.ReadyToSwitch);
        At(_preAttackHeadStateR, _attackHeadState, ()  => _preAttackHeadStateR.ReadyToSwitch);
        At(_preAttackTailStateL, _attackTailStateL, () => _preAttackTailStateL.ReadyToSwitch);
        At(_preAttackTailStateR, _attackTailStateR, () => _preAttackTailStateR.ReadyToSwitch);
        
        // Hover -> PreAttack
        At(_hoverState, _preAttackHoverState, IsStartHeadAttack());
        // PreAttackHover -> Attack
        At(_preAttackHoverState, _attackHoverState, () => _preAttackHoverState.ReadyToSwitch);
        
        // Attack -> Bounce
        At(_attackHeadState, _bounceHeadState, IsBounced());
        At(_attackTailStateL, _bounceTailStateL, IsBounced());
        At(_attackTailStateR, _bounceTailStateR, IsBounced());
        At(_attackHoverState, _bounceHoverState, IsBounced());
        
        // Bounce -> Success
        At(_bounceHeadState, _attackHeadSuccessState, IsAttackSuccess());
        At(_bounceTailStateL, _attackTailSuccessL, IsAttackSuccess());
        At(_bounceTailStateR, _attackTailSuccessR, IsAttackSuccess());
        At(_bounceHoverState, _returnHoverState, IsAttackSuccess());
        
        // Bounce -> Fail
        At(_bounceHeadState, _fallHeadState, IsAttackFail());
        At(_bounceTailStateL, _attackTailFailL, IsAttackFail());
        At(_bounceTailStateR, _attackTailFailR, IsAttackFail());
        At(_bounceHoverState, _fallHeadState, IsAttackFail());
        
        // Bounce - Death
        At(_bounceHeadState, _deathHeadState, IsDied());
        At(_bounceTailStateL, _deathTailStateL, IsDied());
        At(_bounceTailStateR, _deathTailStateR, IsDied());
        At(_bounceHoverState, _deathHeadState, IsDied());
        
        // Spider
        At(_catchSpiderStateL, _spiderPatrolStateL, IsAnimationEnded());
        At(_catchSpiderStateR, _spiderPatrolStateR, IsAnimationEnded());
        
        At(_spiderPatrolStateL, _spiderPushStateL, IsStartSpiderAttack());
        At(_spiderPatrolStateR, _spiderPushStateR, IsStartSpiderAttack());
        
        At(_spiderPushStateL, _spiderPreAttackHeadTransitionStateL, () => _spiderPushStateL.ReadyToSwitch);
        At(_spiderPushStateR, _spiderPreAttackHeadTransitionStateR, () => _spiderPushStateR.ReadyToSwitch);
        
        // Spider to patrol
        At(_spiderPreAttackHeadTransitionStateL, _patrolStateL, () => _spiderPreAttackHeadTransitionStateL.ReadyToSwitch);
        At(_spiderPreAttackHeadTransitionStateR, _patrolStateR, () => _spiderPreAttackHeadTransitionStateR.ReadyToSwitch);
        
        
        // Return Resolve Transitions
        
        At(_returnTransitionLRTBState, _moveToPatrolStateL, IsResolvedToPatrolL());
        At(_returnTransitionLRTBState, _catchSpiderStateR, IsResolvedToCatchSpiderR());
        At(_returnTransitionLRTBState, _moveToHoverState, IsResolvedToHover());
        
        At(_returnTransitionRLTBState, _moveToPatrolStateR, IsResolvedToPatrolR());
        At(_returnTransitionRLTBState, _catchSpiderStateL, IsResolvedToCatchSpiderL());
        At(_returnTransitionRLTBState, _moveToHoverState, IsResolvedToHover());
        
        At(_returnTransitionLRBTState, _catchSpiderStateL, IsResolvedToCatchSpiderL());
        At(_returnTransitionLRBTState, _catchSpiderStateR, IsResolvedToCatchSpiderR());
        At(_returnTransitionRLBTState, _catchSpiderStateL, IsResolvedToCatchSpiderL());
        At(_returnTransitionRLBTState, _catchSpiderStateR, IsResolvedToCatchSpiderR());
        
        // _stateMachine.SetState(_idleState);
        
        // Transition helper methods
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        // void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
        
        // Transition Predicates
        #region Transition Predicate Delegates
        
        Func<bool> IsAnimationEnded() => () =>
        {
            if (_isAnimClipEnded)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsStartHeadAttack() => () =>
        {
            if (_isAttacking && _currentPatrolAttackMode == DragonflyPatrolAttackMode.Head)
            {
                _isAttacking = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsStartTailAttack() => () =>
        {
            if (_isAttacking && _currentPatrolAttackMode == DragonflyPatrolAttackMode.Tail)
            {
                _isAttacking = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsStartSpiderAttack() => () =>
        {
            if (_isAttacking && _currentPatrolAttackMode == DragonflyPatrolAttackMode.Spider)
            {
                _isAttacking = false;
                return true;
            }
            return false;
        };
        

        Func<bool> IsBounced() => () =>
        {
            if (_isBounced)
            {
                _isBounced = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAttackSuccess() => () =>
        {
            if (_isAttackSuccess)
            {
                _isAttackSuccess = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsAttackFail() => () =>
        {
            if (_isAttackFail && !_isDead)
            {
                _isAttackFail = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsDied() => () =>
        {
            if (_isDead)
            {
                _isDead = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsResolvedToPatrolL() => () =>
        {
            if (_isAnimClipEnded && _resolvedReturnMode == DragonflyReturnMode.PatrolL)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsResolvedToPatrolR() => () =>
        {
            if (_isAnimClipEnded && _resolvedReturnMode == DragonflyReturnMode.PatrolR)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsResolvedToCatchSpiderL() => () =>
        {
            if (_isAnimClipEnded && _resolvedReturnMode == DragonflyReturnMode.SpiderL)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsResolvedToCatchSpiderR() => () =>
        {
            if (_isAnimClipEnded && _resolvedReturnMode == DragonflyReturnMode.SpiderR)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        Func<bool> IsResolvedToHover() => () =>
        {
            if (_isAnimClipEnded && _resolvedReturnMode == DragonflyReturnMode.Hover)
            {
                _isAnimClipEnded = false;
                return true;
            }
            return false;
        };
        
        #endregion
    }

    public void Initialize()
    {
        _isPlaying = false;
        _isAnimClipEnded = false;
        _isBounced = false;
        _isAttackSuccess = false;
        _isAttackFail = false;
        _isDead = false;
        
        _stateMachine.SetState(_idleState);
        _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", ""); // DEBUG
    }

    public void Play(DragonflyEnterType state, int sideDirection)
    {
        _isDead = false;
        _isAttackSuccess = false;
        _isAttackFail = false;
        _sideDirection = sideDirection;
        _enterState = state;
        _isPlaying = true;
    }

    private void Update()
    {
        if (_isPlaying)
        {
            _stateMachine.Tick();
            _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", ""); // DEBUG
        }
    }

    public void StartAttack(DragonflyPatrolAttackMode mode)
    {
        _currentPatrolAttackMode = mode;
        _isAttacking = true;
    }

    public void ResolveReturnTransition(DragonflyReturnMode mode)
    {
        _previousState = _stateMachine.CurrentState;
        _resolvedReturnMode = mode;
        IState resolvedState = null;
        
        switch (mode)
        {
            case DragonflyReturnMode.PatrolL:
                resolvedState = _moveToPatrolStateL;
                break;
            case DragonflyReturnMode.PatrolR:
                resolvedState = _moveToPatrolStateR;
                break;
            case DragonflyReturnMode.SpiderL:
                resolvedState = _catchSpiderStateL;
                break;
            case DragonflyReturnMode.SpiderR:
                resolvedState = _catchSpiderStateR;
                break;
            case DragonflyReturnMode.Hover:
                resolvedState = _moveToHoverState;
                break;
        }
        
        // Immediately switch to the resolved state if possible
        
        if ((ReferenceEquals(_stateMachine.CurrentState, _attackHeadSuccessState) && _visibleBodyTransform.position.x < 0))
        {
            if (ReferenceEquals(resolvedState, _moveToPatrolStateR) || ReferenceEquals(resolvedState, _catchSpiderStateL))
            {
                _stateMachine.SetState(resolvedState);
                
            }
            else
            {
                _stateMachine.SetState(_returnTransitionLRTBState);
            }
        }
        else if (ReferenceEquals(_stateMachine.CurrentState, _attackHeadSuccessState) && _visibleBodyTransform.position.x > 0)
        {
            if (ReferenceEquals(resolvedState, _moveToPatrolStateL) || ReferenceEquals(resolvedState, _catchSpiderStateR))
            {
                _stateMachine.SetState(resolvedState);
            }else
            {
                _stateMachine.SetState(_returnTransitionRLTBState);
            }
        } 
        else if (ReferenceEquals(_stateMachine.CurrentState, _attackTailSuccessL))
        {
            if (ReferenceEquals(resolvedState, _moveToPatrolStateR) || 
                ReferenceEquals(resolvedState, _catchSpiderStateL) ||
                ReferenceEquals(resolvedState, _moveToHoverState))
            {
                _stateMachine.SetState(resolvedState);
            }
            else
            {
                _stateMachine.SetState(_returnTransitionLRTBState);
            }
        } 
        else if (ReferenceEquals(_stateMachine.CurrentState, _attackTailSuccessR))
        {
            if (ReferenceEquals(resolvedState, _moveToPatrolStateL) || 
                ReferenceEquals(resolvedState, _catchSpiderStateR) ||
                ReferenceEquals(resolvedState, _moveToHoverState))
            {
                _stateMachine.SetState(resolvedState);
            }
            else
            {
                _stateMachine.SetState(_returnTransitionRLTBState);
            }
        } 
        else if (ReferenceEquals(_stateMachine.CurrentState, _fallHeadState) ||
                   ReferenceEquals(_stateMachine.CurrentState, _returnHoverState) ||
                   ReferenceEquals(_stateMachine.CurrentState, _attackTailFailL) ||
                   ReferenceEquals(_stateMachine.CurrentState, _attackTailFailR))
        {
            if (ReferenceEquals(resolvedState, _moveToPatrolStateL) ||
                ReferenceEquals(resolvedState, _moveToPatrolStateR) ||
                ReferenceEquals(resolvedState, _moveToHoverState))
            {
                _stateMachine.SetState(resolvedState);
            }
            else if (ReferenceEquals(resolvedState, _catchSpiderStateL) && _visibleBodyTransform.position.x < 0)
            {
                _stateMachine.SetState(resolvedState);
            }
            else if (ReferenceEquals(resolvedState, _catchSpiderStateL) && _visibleBodyTransform.position.x > 0)
            {
                _stateMachine.SetState(_returnTransitionRLBTState);
            }
            else if (ReferenceEquals(resolvedState, _catchSpiderStateR) && _visibleBodyTransform.position.x > 0)
            {
                _stateMachine.SetState(resolvedState);
            }
            else if (ReferenceEquals(resolvedState, _catchSpiderStateR) && _visibleBodyTransform.position.x < 0)
            {
                _stateMachine.SetState(_returnTransitionLRBTState);
            }
        }
#if  UNITY_EDITOR
        _currentStateType = resolvedState?.ToString().Replace("FDragonfly", ""); // DEBUG
#endif 
    }

    public void TriggerBounce()
    {
        _isBounced = true;
    }

    public void TriggerFall(bool isReceivedDamage)
    {
        if (isReceivedDamage)
        {
            _isAttackSuccess = false;
            _isAttackFail = true;
        }
        else
        {
            _isAttackSuccess = true;
            _isAttackFail = false;
        }
    }

    public void TriggerDeath()
    {
        _isAttackSuccess = false;
        _isAttackFail = false;
        _isDead = true;
    }
    
    public void TriggerGameOver()
    {
        _stateMachine.SetState(_gameoverHoverState);
    }

    private void OnClipEndedHandle()
    {
        _isAnimClipEnded = true;
    }

    private void OnSwarmCallHandle()
    {
        OnSwarmCallEvent?.Invoke();
    }

    #region State Event Handle methods

    private void OnReadyToAttackEnterHandle()
    {
        OnReadyToAttackStateEnteredEvent?.Invoke(_stateMachine.CurrentState);
    }

    private void OnReadyToSwarmAttackEnterHandle()
    {
        OnReadyToSwarmAttackStateEnteredEvent?.Invoke(_stateMachine.CurrentState);
    }

    private void OnReadyToSpiderAttackHandle()
    {
        OnReadyToSpiderAttackStateEnteredEvent?.Invoke();
    }

    private void OnCatchSpiderStartedLHandle()
    {
        OnCatchSpiderStartedEvent?.Invoke(1);
    }

    private void OnCatchSpiderStartedRHandle()
    {
        OnCatchSpiderStartedEvent?.Invoke(-1);
    }

    private void OnPreAttackStartedHandle()
    {
        OnPreAttackStartedEvent?.Invoke();
    }

    private void OnAttackStartedHandle()
    {
        OnAttackStartedEvent?.Invoke();
    }

    private void OnAttackEndedHandle()
    {
        OnAttackEndedEvent?.Invoke();
    }

    private void OnAfterAttackExitEndedHandle()
    {
        OnAfterAttackExitEndedEvent?.Invoke(_stateMachine.CurrentState);
    }

    private void OnDeathAnimationEndedHandle()
    {
        OnDeathAnimationEndedEvent?.Invoke();
    }

    #endregion
}
