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
    [SerializeField] private FDragonflyAttackHeadSuccessState _attackHeadSuccess; 
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
    [SerializeField] private FDragonflyReturnTransitionBTStateL _returnTransitionBTStateL;
    [SerializeField] private FDragonflyReturnTransitionBTStateR _returnTransitionBTStateR;
    [SerializeField] private FDragonflyReturnTransitionTBStateL _returnTransitionTBStateL;
    [SerializeField] private FDragonflyReturnTransitionTBStateR _returnTransitionTBStateR;
    [SerializeField] private FDragonflySpiderPatrolStateL _spiderPatrolStateL;
    [SerializeField] private FDragonflySpiderPatrolStateR _spiderPatrolStateR;
    [SerializeField] private FDragonflySpiderPreattackHeadTransitionStateL _spiderPreAttackHeadTransitionStateL;
    [SerializeField] private FDragonflySpiderPreattackHeadTransitionStateR _spiderPreAttackHeadTransitionStateR;
    [SerializeField] private FDragonflySpiderPushStateL _spiderPushStateL;
    [SerializeField] private FDragonflySpiderPushStateR _spiderPushStateR;
    
    // Events
    public event Action<IState> OnReadyToAttackStateEntered; 
    public event Action<DragonflyMovementState> OnAfterAttackExitEnded;
    public event Action OnAttackStarted;
    public event Action OnPreattackStarted;
    public event Action OnAttackEnded;
    public event Action<int> OnCatchSpiderStarted;
    public event Action OnDeathAnimationEnded; 
    
    public IState MovementState => _stateMachine.CurrentState;
    
    
    private FStateMachine _stateMachine = new FStateMachine();
    
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
    
    // Return Resolve
    private bool _isReturnResolved = true;
    private DragonflyMovementState _returnMovementState;
    private int _returnSideDirection;

    private bool _isPlaying = false;
    private bool _isAnimClipEnded = false;
    private bool _isBounced = false;
    private int _enterState = 0;
    private int _sideDirection = 1;
    private bool _isCollided = false;
    private bool _isReceivedDamage = false;
    private bool _isDead = false;

    private void OnEnable()
    {
        _animationClipEvents.OnClipEndedEvent += OnClipEnded;
    }

    private void OnDisable()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();    
        }
        _animationClipEvents.OnClipEndedEvent -= OnClipEnded;
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
        _attackHeadSuccess.SetDependencies(_visibleBodyTransform, transform); 
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
        _returnTransitionBTStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _returnTransitionBTStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _returnTransitionTBStateL.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
        _returnTransitionTBStateR.SetDependencies(_visibleBodyTransform, _animatedTransform, this);
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
        // Idle ->
        At(_idleState, _enterToPatrolStateL, () => _isPlaying && _enterState == 0 && _sideDirection == 1);
        At(_idleState, _enterToPatrolStateR, () => _isPlaying && _enterState == 0 && _sideDirection == -1);
        At(_idleState, _enterToHoverStateL, () => _isPlaying && _enterState == 1 && _sideDirection == 1);
        At(_idleState, _enterToHoverStateR, () => _isPlaying && _enterState == 1 && _sideDirection == -1);
        
        // EnterToPatrolL ->
        At(_enterToPatrolStateL, _patrolStateL, IsEnteredToPatrolByAnimation());
        // EnterToPatrolR ->
        At(_enterToPatrolStateR, _patrolStateR, IsEnteredToPatrolByAnimation());
        // EnterToHoverL ->
        At(_enterToHoverStateL, _hoverState, IsEnteredToPatrolByAnimation());
        // EnterToHoverR ->
        At(_enterToHoverStateR, _hoverState, IsEnteredToPatrolByAnimation());
        
        // PatrolL ->
        At(_patrolStateL, _preAttackHeadStateL,  IsStartPatrolAttackHead());
        At(_patrolStateL, _preAttackTailStateL,  IsStartPatrolAttackTail());
        // PatrolR ->
        At(_patrolStateR, _preAttackHeadStateR,  IsStartPatrolAttackHead());
        At(_patrolStateR, _preAttackTailStateR,  IsStartPatrolAttackTail());
        
        // PreAttackHeadL ->
        At(_preAttackHeadStateL, _attackHeadState, IsPreAttackHeadStateLEnded());
        // PreAttackHeadR ->
        At(_preAttackHeadStateR, _attackHeadState, IsPreAttackHeadStateREnded());
        // PreAttackTailL ->
        At(_preAttackTailStateL, _attackTailStateL, IsPreAttackTailStateLEnded());
        // PreAttackTailR ->
        At(_preAttackTailStateR, _attackTailStateR, IsPreAttackTailStateREnded());
        
        
        // Hover ->
        At(_hoverState, _preAttackHoverState, IsStartPatrolAttackHover());
        // PreAttackHover ->
        At(_preAttackHoverState, _attackHoverState, IsPreAttackHoverStateEnded());
        

        
        
        _stateMachine.SetState(_idleState);
        
        // Transition helper methods
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        
        // Transition Predicates
        # region Transition Predicate Delegates
        
        Func<bool> IsEnteredToPatrolByAnimation() => () =>
        {
            if (_isAnimClipEnded)
            {
                _isAnimClipEnded = false;
                OnReadyToAttackStateEntered?.Invoke(_stateMachine.CurrentState); // TODO: find fow to pass a new state here or do something else
                Debug.Log("OnReadyToAttackStateEntered invoked");
                return true;
            }
            return false;
        };
        
        Func<bool> IsStartPatrolAttackHead() => () =>
        {
            if (_isAttacking && _currentPatrolAttackMode == DragonflyPatrolAttackMode.Head)
            {
                _isAttacking = false;
                OnPreattackStarted?.Invoke();
                return true;
            }
            return false;
        };
        
        Func<bool> IsStartPatrolAttackTail() => () =>
        {
            if (_isAttacking && _currentPatrolAttackMode == DragonflyPatrolAttackMode.Tail)
            {
                _isAttacking = false;
                OnPreattackStarted?.Invoke();
                return true;
            }
            return false;
        };
        
        Func<bool> IsStartPatrolAttackHover() => () =>
        {
            if (_isAttacking)
            {
                _isAttacking = false;
                OnPreattackStarted?.Invoke();
                return true;
            }
            return false;
        };

        // TODO: refactor to merge cast an class check
        Func<bool> IsPreAttackHeadStateLEnded() => () =>
        {
            if (_stateMachine.CurrentState != null && _stateMachine.CurrentState is FDragonflyPreAttackHeadStateL)
            {
                if ((_stateMachine.CurrentState as FDragonflyPreAttackHeadStateL).ReadyToSwitch)
                {
                    OnAttackEnded?.Invoke();
                    return true;
                }    
            }
            return false;
        };

        Func<bool> IsPreAttackHeadStateREnded() => () =>
        {
            if (_stateMachine.CurrentState != null && _stateMachine.CurrentState is FDragonflyPreAttackHeadStateR)
            {
                if ((_stateMachine.CurrentState as FDragonflyPreAttackHeadStateR).ReadyToSwitch)
                {
                    OnAttackEnded?.Invoke();
                    return true;
                }    
            }
            return false;
        };
        
        Func<bool> IsPreAttackTailStateLEnded() => () =>
        {
            if (_stateMachine.CurrentState != null && _stateMachine.CurrentState is FDragonflyPreAttackTailStateL)
            {
                if ((_stateMachine.CurrentState as FDragonflyPreAttackTailStateL).ReadyToSwitch)
                {
                    OnAttackEnded?.Invoke();
                    return true;
                }    
            }
            return false;
        };
        
        Func<bool> IsPreAttackTailStateREnded() => () =>
        {
            if (_stateMachine.CurrentState != null && _stateMachine.CurrentState is FDragonflyPreAttackTailStateR)
            {
                if ((_stateMachine.CurrentState as FDragonflyPreAttackTailStateR).ReadyToSwitch)
                {
                    OnAttackEnded?.Invoke();
                    return true;
                }    
            }
            return false;
        };
        
        Func<bool> IsPreAttackHoverStateEnded() => () =>
        {
            if (_stateMachine.CurrentState != null && _stateMachine.CurrentState is FDragonflyPreAttackHoverState)
            {
                if ((_stateMachine.CurrentState as FDragonflyPreAttackHoverState).ReadyToSwitch)
                {
                    OnAttackEnded?.Invoke();
                    return true;
                }    
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
        _isReceivedDamage = false;
        _sideDirection = sideDirection;
        _enterState = state;
        _isPlaying = true;
    }

    public void StartAttack(DragonflyPatrolAttackMode mode)
    {
        _currentPatrolAttackMode = mode;
        _isAttacking = true;
    }

    public void ResolveReturnTransition(DragonflyReturnMode mode, int sideDirection)
    {
    }
    
    public void TriggerBounce()
    {
        _isBounced = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _isPlaying = false;
            _stateMachine.SetState(_idleState);
            MovementInit(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            _isPlaying = false;
            _stateMachine.SetState(_idleState);
            MovementInit(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _isPlaying = false;
            _stateMachine.SetState(_idleState);
            MovementInit(1, 1);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            _isPlaying = false;
            _stateMachine.SetState(_idleState);
            MovementInit(1, -1);
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartAttack(DragonflyPatrolAttackMode.Head); // TODO: unset 
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartAttack(DragonflyPatrolAttackMode.Tail);
        }
        

        if (_isPlaying)
        {
            _stateMachine.Tick();
        }
        // Show current state for debug
        _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", "");
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
}
