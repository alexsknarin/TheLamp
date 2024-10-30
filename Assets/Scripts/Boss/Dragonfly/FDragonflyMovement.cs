using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

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
    
    [Header("Animation Clips")]
    [SerializeField] private DragonflyAnimClipCollection _animClipCollection;

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
    public event Action<IState> OnReadyToAttackStateEntered; // TODO: possibly Use Type instead of IState 
    public event Action<IState> OnReadyToSwarmAttackStateEntered;
    public event Action<IState> OnAfterAttackExitEnded; // TODO: possibly Use Type instead of IState
    public event Action OnAttackStarted;
    public event Action OnPreattackStarted;
    public event Action OnAttackEnded;
    public event Action<int> OnCatchSpiderStarted;
    public event Action OnReadyToSpiderAttackStateEntered;
    public event Action OnDeathAnimationEnded;

    private FStateMachine _stateMachine = new FStateMachine();
    public IState MovementState => _stateMachine.CurrentState;
    
    // Animation 
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    // Attack Modes
    private bool _isAttacking = false;
    private DragonflyPatrolAttackMode _currentPatrolAttackMode;
    
    // Tracking previous state
    private IState _previousState;
    private DragonflyReturnMode _resolvedReturnMode;

    
    // Return Resolve
    private DragonflyMovementState _returnMovementState;
    private int _returnSideDirection;

    private bool _isPlaying = false;
    private bool _isAnimClipEnded = false;
    private bool _isBounced = false;
    private int _enterState = 0;
    private int _sideDirection = 1;
    private bool _isCollided = false;
    private bool _isAttackSuccess = false;
    private bool _isAttackFail = false;
    private bool _isDead = false;

    private void OnEnable()
    {
        _animationClipEvents.OnClipEndedEvent += OnClipEnded;
        
        _hoverState.OnStarted += OnReadyToAttackEnterHandle;
        _patrolStateL.OnStarted += OnReadyToAttackEnterHandle; // 
        _patrolStateR.OnStarted += OnReadyToAttackEnterHandle; //
        
        _patrolStateL.OnStarted += OnReadyToSwarmAttackEnterHandle;
        _patrolStateR.OnStarted += OnReadyToSwarmAttackEnterHandle;
        
        _spiderPatrolStateL.OnStarted += OnReadyToSpiderAttackHandle;
        _spiderPatrolStateR.OnStarted += OnReadyToSpiderAttackHandle;
        
        _catchSpiderStateL.OnStarted += OnCatchSpiderStartedLHandle;
        _catchSpiderStateR.OnStarted += OnCatchSpiderStartedRHandle;
        
        _preAttackHeadStateL.OnStarted += OnPreAttackStartedHandle;
        _preAttackHeadStateR.OnStarted += OnPreAttackStartedHandle;
        _preAttackTailStateL.OnStarted += OnPreAttackStartedHandle;
        _preAttackTailStateR.OnStarted += OnPreAttackStartedHandle;
        _preAttackHoverState.OnStarted += OnPreAttackStartedHandle;
        
        _attackHeadState.OnStarted += OnAttackStartedHandle;
        _attackTailStateL.OnStarted += OnAttackStartedHandle;
        _attackTailStateR.OnStarted += OnAttackStartedHandle;
        _attackHoverState.OnStarted += OnAttackStartedHandle;
        
        _deathHeadState.OnStarted += OnAttackEndedHandle;
        _deathTailStateL.OnStarted += OnAttackEndedHandle;
        _deathTailStateR.OnStarted += OnAttackEndedHandle;
        _attackHeadSuccessState.OnStarted += OnAttackEndedHandle;
        _fallHeadState.OnStarted += OnAttackEndedHandle;
        _attackTailFailL.OnStarted += OnAttackEndedHandle;
        _attackTailFailR.OnStarted += OnAttackEndedHandle;
        _attackTailSuccessL.OnStarted += OnAttackEndedHandle;
        _attackTailSuccessR.OnStarted += OnAttackEndedHandle;
        _returnHoverState.OnStarted += OnAttackEndedHandle;
        
        _attackHeadSuccessState.OnEnded += OnAfterAttackExitEndedHandle;
        _attackTailSuccessL.OnEnded += OnAfterAttackExitEndedHandle;
        _attackTailSuccessR.OnEnded += OnAfterAttackExitEndedHandle;
        _attackTailFailL.OnEnded += OnAfterAttackExitEndedHandle;
        _attackTailFailR.OnEnded += OnAfterAttackExitEndedHandle;
        _fallHeadState.OnEnded += OnAfterAttackExitEndedHandle;
        _returnHoverState.OnEnded += OnAfterAttackExitEndedHandle;
        _deathHeadState.OnEnded += OnAfterAttackExitEndedHandle;
        _deathTailStateL.OnEnded += OnAfterAttackExitEndedHandle;
        _deathTailStateR.OnEnded += OnAfterAttackExitEndedHandle;
        
        _deathHeadState.OnEnded += OnDeathAnimationEndedHandle;
        _deathTailStateL.OnEnded += OnDeathAnimationEndedHandle;
        _deathTailStateR.OnEnded += OnDeathAnimationEndedHandle;
    }

    private void OnDisable()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();    
        }
        _animationClipEvents.OnClipEndedEvent -= OnClipEnded;
        
        _hoverState.OnStarted -= OnReadyToAttackEnterHandle;
        _patrolStateL.OnStarted -= OnReadyToAttackEnterHandle; // 
        _patrolStateR.OnStarted -= OnReadyToAttackEnterHandle; //
        
        _patrolStateL.OnStarted -= OnReadyToSwarmAttackEnterHandle;
        _patrolStateR.OnStarted -= OnReadyToSwarmAttackEnterHandle;
        
        _spiderPatrolStateL.OnStarted -= OnReadyToSpiderAttackHandle;
        _spiderPatrolStateR.OnStarted -= OnReadyToSpiderAttackHandle;
        
        _catchSpiderStateL.OnStarted -= OnCatchSpiderStartedLHandle;
        _catchSpiderStateR.OnStarted -= OnCatchSpiderStartedRHandle;
        
        _preAttackHeadStateL.OnStarted -= OnPreAttackStartedHandle;
        _preAttackHeadStateR.OnStarted -= OnPreAttackStartedHandle;
        _preAttackTailStateL.OnStarted -= OnPreAttackStartedHandle;
        _preAttackTailStateR.OnStarted -= OnPreAttackStartedHandle;
        _preAttackHoverState.OnStarted -= OnPreAttackStartedHandle;
        
        _attackHeadState.OnStarted -= OnAttackStartedHandle;
        _attackTailStateL.OnStarted -= OnAttackStartedHandle;
        _attackTailStateR.OnStarted -= OnAttackStartedHandle;
        _attackHoverState.OnStarted -= OnAttackStartedHandle;
        
        _deathHeadState.OnStarted -= OnAttackEndedHandle;
        _deathTailStateL.OnStarted -= OnAttackEndedHandle;
        _deathTailStateR.OnStarted -= OnAttackEndedHandle;
        _attackHeadSuccessState.OnStarted -= OnAttackEndedHandle;
        _fallHeadState.OnStarted -= OnAttackEndedHandle;
        _attackTailFailL.OnStarted -= OnAttackEndedHandle;
        _attackTailFailR.OnStarted -= OnAttackEndedHandle;
        _attackTailSuccessL.OnStarted -= OnAttackEndedHandle;
        _attackTailSuccessR.OnStarted -= OnAttackEndedHandle;
        _returnHoverState.OnStarted -= OnAttackEndedHandle;
        
        _attackHeadSuccessState.OnEnded -= OnAfterAttackExitEndedHandle;
        _attackTailSuccessL.OnEnded -= OnAfterAttackExitEndedHandle;
        _attackTailSuccessR.OnEnded -= OnAfterAttackExitEndedHandle;
        _attackTailFailL.OnEnded -= OnAfterAttackExitEndedHandle;
        _attackTailFailR.OnEnded -= OnAfterAttackExitEndedHandle;
        _fallHeadState.OnEnded -= OnAfterAttackExitEndedHandle;
        _returnHoverState.OnEnded -= OnAfterAttackExitEndedHandle;
        _deathHeadState.OnEnded -= OnAfterAttackExitEndedHandle;
        _deathTailStateL.OnEnded -= OnAfterAttackExitEndedHandle;
        _deathTailStateR.OnEnded -= OnAfterAttackExitEndedHandle;
        
        _deathHeadState.OnEnded -= OnDeathAnimationEndedHandle;
        _deathTailStateL.OnEnded -= OnDeathAnimationEndedHandle;
        _deathTailStateR.OnEnded -= OnDeathAnimationEndedHandle;
    }

    private void Awake()
    {
        _isPlaying = false;
        
        _playableGraph = PlayableGraph.Create();
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        _playablesContainer = new DragonflyPlayablesContainer(_playableGraph);
        
        // Add clips to container
        _animClipCollection.Initialize(_playablesContainer);
        
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
        _catchSpiderStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _catchSpiderStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _deathHeadState.SetDependencies(_visibleBodyTransform, _fallPoint);
        _deathTailStateL.SetDependencies(_visibleBodyTransform, _fallPoint);
        _deathTailStateR.SetDependencies(_visibleBodyTransform, _fallPoint);
        _enterToHoverStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _enterToHoverStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _enterToPatrolStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _enterToPatrolStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _fallHeadState.SetDependencies(_visibleBodyTransform, _fallPoint);
        _hoverState.SetDependencies(_visibleBodyTransform, transform);
        _idleState.SetDependencies();
        _moveToHoverState.SetDependencies(_visibleBodyTransform, transform);
        _moveToPatrolStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, this);;
        _moveToPatrolStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, this);;
        _patrolStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _patrolStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _preAttackHeadStateL.SetDependencies(_visibleBodyTransform, transform);
        _preAttackHeadStateR.SetDependencies(_visibleBodyTransform, transform);
        _preAttackHoverState.SetDependencies(_visibleBodyTransform, transform);
        _preAttackTailStateL.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _preAttackTailStateR.SetDependencies(_visibleBodyTransform, _patrolTransform, _patrolRotator);
        _returnHoverState.SetDependencies(_visibleBodyTransform, transform);
        _returnTransitionRLBTState.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _returnTransitionLRBTState.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _returnTransitionRLTBState.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _returnTransitionLRTBState.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
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
        At(_idleState, _enterToPatrolStateL, () => _isPlaying && _enterState == 0 && _sideDirection == 1);
        At(_idleState, _enterToPatrolStateR, () => _isPlaying && _enterState == 0 && _sideDirection == -1);
        At(_idleState, _enterToHoverStateL, () => _isPlaying && _enterState == 1 && _sideDirection == 1);
        At(_idleState, _enterToHoverStateR, () => _isPlaying && _enterState == 1 && _sideDirection == -1);
        
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
        // TODO: bounce should be based on the exit from the collision zone, to the bounce duration TODO: check 
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
        
        // Set Initial State
        _stateMachine.SetState(_idleState);
        
        // Transition helper methods
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        // void Any(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);
        
        // Transition Predicates
        # region Transition Predicate Delegates
        
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
        
        # endregion
    }

    public void Play(int state, int sideDirection)
    {
        MovementInit(state, sideDirection);
    }

    private void MovementInit(int state, int sideDirection)
    {
        _isDead = false;
        _isAttackSuccess = false;
        _isAttackFail = false;
        _sideDirection = sideDirection;
        _enterState = state;
        _isPlaying = true;
    }

    public void StartAttack(DragonflyPatrolAttackMode mode)
    {
        Debug.Log("Start Attack!!!!!!!!!!!!!!! >>>>>> " + mode.ToString());
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
        
        if ((_stateMachine.CurrentState == _attackHeadSuccessState && _visibleBodyTransform.position.x < 0))
        {
            if (resolvedState == _moveToPatrolStateR || resolvedState == _catchSpiderStateL)
            {
                _stateMachine.SetState(resolvedState);
                
            }
            else
            {
                _stateMachine.SetState(_returnTransitionLRTBState);
            }
            _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
        }
        
        if (_stateMachine.CurrentState == _attackHeadSuccessState && _visibleBodyTransform.position.x > 0)
        {
            if (resolvedState == _moveToPatrolStateL || resolvedState == _catchSpiderStateR)
            {
                _stateMachine.SetState(resolvedState);
            }else
            {
                _stateMachine.SetState(_returnTransitionRLTBState);
            }
            _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
        }
        
        if (_stateMachine.CurrentState == _attackTailSuccessL)
        {
            if (resolvedState == _moveToPatrolStateR || resolvedState == _catchSpiderStateL || resolvedState == _moveToHoverState)
            {
                _stateMachine.SetState(resolvedState);
            }
            else
            {
                _stateMachine.SetState(_returnTransitionLRTBState);
            }
            _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
        }
        
        if (_stateMachine.CurrentState == _attackTailSuccessR)
        {
            if (resolvedState == _moveToPatrolStateL || resolvedState == _catchSpiderStateR || resolvedState == _moveToHoverState)
            {
                _stateMachine.SetState(resolvedState);
            }
            else
            {
                _stateMachine.SetState(_returnTransitionRLTBState);
            }
            _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
        }
        
        if (_stateMachine.CurrentState == _fallHeadState ||
            _stateMachine.CurrentState == _returnHoverState ||
            _stateMachine.CurrentState == _attackTailFailL ||
            _stateMachine.CurrentState == _attackTailFailR)
        {
            if (resolvedState == _moveToPatrolStateL || resolvedState == _moveToPatrolStateR || resolvedState == _moveToHoverState)
            {
                _stateMachine.SetState(resolvedState);
                _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
            }
            if (resolvedState == _catchSpiderStateL)
            {
                _stateMachine.SetState(_returnTransitionRLBTState);
                _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
            }
            if (resolvedState == _catchSpiderStateR)
            {
                _stateMachine.SetState(_returnTransitionLRBTState);
                _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
            }
        }

        // if (_stateMachine.CurrentState == _fallHeadState ||
        //     _stateMachine.CurrentState == _returnHoverState ||
        //     _stateMachine.CurrentState == _attackTailFailL ||
        //     _stateMachine.CurrentState == _attackTailFailR)
        // {
        //     if (resolvedState == _catchSpiderStateL)
        //     {
        //         _stateMachine.SetState(_returnTransitionRLBTState);
        //         _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
        //     }
        //     if (resolvedState == _catchSpiderStateR)
        //     {
        //         _stateMachine.SetState(_returnTransitionLRBTState);
        //         _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
        //     }
        // }
        
        // if ((_stateMachine.CurrentState == _fallHeadState && _visibleBodyTransform.position.x > 0) ||
        //     (_stateMachine.CurrentState == _returnHoverState && _visibleBodyTransform.position.x > 0) ||
        //     _stateMachine.CurrentState == _attackTailFailR)
        // {
        //     if (resolvedState == _catchSpiderStateL)
        //     {
        //         _stateMachine.SetState(_returnTransitionLRBTState);
        //         _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
        //     }
        //     if (resolvedState == _catchSpiderStateR)
        //     {
        //         _stateMachine.SetState(_returnTransitionRLBTState);
        //         _currentStateType = resolvedState.ToString().Replace("FDragonfly", ""); // DEBUG
        //     }
        // }
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

    private void Update()
    {
        _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", ""); // DEBUG
        if (_isPlaying)
        {
            _stateMachine.Tick();
        }
        _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", ""); // DEBUG
    }


    private void OnDestroy()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();    
        }
    }

    public void PlayClip(DragonflyMovementState movementState)
    {
        Debug.Log("PlayClip: " + movementState);
        // TODO: refactor to remove enum
        AnimationClipPlayable clipPlayable = _playablesContainer.GetClip(movementState);
        clipPlayable.SetTime(0);
        clipPlayable.SetTime(0); // Unity Bug
        _playableOutput.SetSourcePlayable(clipPlayable);
        if (_playableGraph.IsValid())
        {
            _playableGraph.Play();    
        }
    }

    private void OnClipEnded()
    {
        // if (_isReturnResolved)
        // {
        //     SwitchState();    
        // }
        // else
        // {
        //     ApplyResolvedReturnTransition();
        // }
        _isAnimClipEnded = true;
    }

    #region State Event Handle methods

    private void OnReadyToAttackEnterHandle()
    {
        OnReadyToAttackStateEntered?.Invoke(_stateMachine.CurrentState);
    }

    private void OnReadyToSwarmAttackEnterHandle()
    {
        OnReadyToSwarmAttackStateEntered?.Invoke(_stateMachine.CurrentState);
    }

    private void OnReadyToSpiderAttackHandle()
    {
        OnReadyToSpiderAttackStateEntered?.Invoke();
    }

    private void OnCatchSpiderStartedLHandle()
    {
        OnCatchSpiderStarted?.Invoke(1);
    }

    private void OnCatchSpiderStartedRHandle()
    {
        OnCatchSpiderStarted?.Invoke(-1);
    }

    private void OnPreAttackStartedHandle()
    {
        OnPreattackStarted?.Invoke();
    }

    private void OnAttackStartedHandle()
    {
        OnAttackStarted?.Invoke();
    }

    private void OnAttackEndedHandle()
    {
        OnAttackEnded?.Invoke();
    }

    private void OnAfterAttackExitEndedHandle()
    {
        OnAfterAttackExitEnded?.Invoke(_stateMachine.CurrentState);
    }

    private void OnDeathAnimationEndedHandle()
    {
        OnDeathAnimationEnded?.Invoke();
    }

    #endregion
}
