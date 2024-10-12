using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DragonflyMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private Transform _animatedTransform;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private Transform _spiderPatrolTransform;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private DragonflyPatrolRotator _spiderPatrolRotator;
    
    [FormerlySerializedAs("_currentDragonflyState")] [SerializeField] private DragonflyState currentDragonflyState;
    [SerializeField] private DragonflyAnimationClipEventHandler _animationClipEvents;
    [SerializeField] private Transform _fallPoint;
    
    [Header("Animation Clips")]
    [SerializeField] private DragonflyAnimClipCollection _animClipCollection;

    [Header("States")]  
    [SerializeField] private DragonflyMovementBaseState _idleState;
    [SerializeField] private DragonflyMovementBaseState _attackHeadState;
    [SerializeField] private DragonflyMovementBaseState _attackHeadSuccess;
    [SerializeField] private DragonflyMovementBaseState _attackHoverState;
    [SerializeField] private DragonflyMovementBaseState _attackTailFail;
    [SerializeField] private DragonflyMovementBaseState _attackTailState;
    [SerializeField] private DragonflyMovementBaseState _attackTailSuccess;
    [SerializeField] private DragonflyMovementBaseState _bounceHeadState;
    [SerializeField] private DragonflyMovementBaseState _bounceHoverState;
    [SerializeField] private DragonflyMovementBaseState _bounceTailState;
    [SerializeField] private DragonflyMovementBaseState _catchSpiderState;
    [SerializeField] private DragonflyMovementBaseState _enterToHoverState;
    [SerializeField] private DragonflyMovementBaseState _enterToPatrolState;
    [SerializeField] private DragonflyMovementBaseState _fallHeadLState;
    [SerializeField] private DragonflyMovementBaseState _hoverState;
    [SerializeField] private DragonflyMovementBaseState _moveToHoverState;
    [SerializeField] private DragonflyMovementBaseState _patrolState;
    [SerializeField] private DragonflyMovementBaseState _preAttackHeadState;
    [SerializeField] private DragonflyMovementBaseState _preAttackHoverState;
    [SerializeField] private DragonflyMovementBaseState _preAttackTailState;
    [SerializeField] private DragonflyMovementBaseState _returnHoverState;
    [SerializeField] private DragonflyMovementBaseState _spiderPatrolState;
    [SerializeField] private DragonflyMovementBaseState _spiderPreAttackHeadTransitionState;
    [SerializeField] private DragonflyMovementBaseState _moveToPatrolState;
    [SerializeField] private DragonflyMovementBaseState _spiderPushState;
    [SerializeField] private DragonflyMovementBaseState _returnTransitionBTState; // Top - Bottom
    [SerializeField] private DragonflyMovementBaseState _returnTransitionTBState; // Bottom - Top
    [SerializeField] private DragonflyMovementBaseState _deathHeadState;
    [SerializeField] private DragonflyMovementBaseState _deathTailState;
    
    public DragonflyState State => _currentMovementState.State;
    
    public event Action<DragonflyState, DragonflyState> OnReadyToAttackStateEntered; 
    public event Action<DragonflyState> OnAfterAttackExitEnded;
    public event Action OnAttackStarted;
    public event Action OnPreattackStarted;
    public event Action OnAttackEnded;
    public event Action<int> OnCatchSpiderStarted;
    public event Action OnDeathAnimationEnded; 
    
    private DragonflyMovementStateData _stateData;
    private DragonflyMovementBaseState _currentMovementState;
    [SerializeField] private DragonflyState _prevDragonflyState;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    
    private int _sideDirection = 1;
    private DragonflyPatrolAttackMode _currentPatrolAttackMode; 
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    // Add state switch variables
    
    // Return Resolve
    private bool _isReturnResolved = true;
    private DragonflyState _returnState;
    private int _returnSideDirection;
    
    
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
        _playableGraph = PlayableGraph.Create();
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        _playablesContainer = new DragonflyPlayablesContainer(_playableGraph);
        
        // Add clips to container
        _animClipCollection.Initialize(_playablesContainer);
        
        _stateData = new DragonflyMovementStateData(
            this, 
            _visibleBodyTransform, 
            _fallPoint, 
            _patrolRotator, 
            _patrolTransform, 
            _spiderPatrolRotator, 
            _spiderPatrolTransform,
            _animatedTransform);
        
        
        // Initialize States
        _idleState.SetCommonStateDependencies(_stateData);
        _enterToPatrolState.SetCommonStateDependencies(_stateData);
        _patrolState.SetCommonStateDependencies(_stateData);
        _preAttackHeadState.SetCommonStateDependencies(_stateData);
        _attackHeadState.SetCommonStateDependencies(_stateData);
        _bounceHeadState.SetCommonStateDependencies(_stateData);
        _fallHeadLState.SetCommonStateDependencies(_stateData);
        _catchSpiderState.SetCommonStateDependencies(_stateData);
        _spiderPatrolState.SetCommonStateDependencies(_stateData);
        _spiderPreAttackHeadTransitionState.SetCommonStateDependencies(_stateData);
        _preAttackTailState.SetCommonStateDependencies(_stateData);
        _attackTailState.SetCommonStateDependencies(_stateData);
        _bounceTailState.SetCommonStateDependencies(_stateData);
        _attackTailSuccess.SetCommonStateDependencies(_stateData);
        _attackTailFail.SetCommonStateDependencies(_stateData);
        _attackHeadSuccess.SetCommonStateDependencies(_stateData);
        _enterToHoverState.SetCommonStateDependencies(_stateData);
        _hoverState.SetCommonStateDependencies(_stateData);
        _moveToHoverState.SetCommonStateDependencies(_stateData);
        _preAttackHoverState.SetCommonStateDependencies(_stateData);
        _attackHoverState.SetCommonStateDependencies(_stateData);
        _bounceHoverState.SetCommonStateDependencies(_stateData);
        _returnHoverState.SetCommonStateDependencies(_stateData);
        _moveToPatrolState.SetCommonStateDependencies(_stateData);
        _spiderPushState.SetCommonStateDependencies(_stateData);
        _returnTransitionBTState.SetCommonStateDependencies(_stateData);
        _returnTransitionTBState.SetCommonStateDependencies(_stateData);
        _deathHeadState.SetCommonStateDependencies(_stateData);
        _deathTailState.SetCommonStateDependencies(_stateData);
        // 

        _currentMovementState = _idleState;
        currentDragonflyState = DragonflyState.Idle;
    }

    public void Play(int state, int sideDirection)
    {
        MovementSetup(state, sideDirection);
    }
    
    public void ResolveReturnTransition(DragonflyReturnMode mode, int sideDirection)
    {
        _prevDragonflyState = _currentMovementState.State;
        _sideDirection = sideDirection;
        DragonflyExitEnterDirection exitDirection = GetCurrentExitDirection(_currentMovementState.State, _visibleBodyTransform.position);
        DragonflyState returnMovementState = GetReturnState(mode, sideDirection, exitDirection);
        
        // Set return state
        switch (returnMovementState)
        {
            case DragonflyState.MoveToPatrolL:
                if (exitDirection == DragonflyExitEnterDirection.RLBT)
                {
                    SetState(_returnTransitionTBState, _visibleBodyTransform.position, -1, 1);
                    _isReturnResolved = false;
                }
                else if (exitDirection == DragonflyExitEnterDirection.L || exitDirection == DragonflyExitEnterDirection.RLTB)
                {
                    SetState(_returnTransitionBTState, _visibleBodyTransform.position, -1, 1);
                    _isReturnResolved = false;
                }
                else
                {
                    // Other states
                    SetState(_moveToPatrolState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = true;
                }
                _returnState = DragonflyState.MoveToPatrolL;
                break;
            case DragonflyState.MoveToPatrolR:
                if (exitDirection == DragonflyExitEnterDirection.LRBT)
                {
                    SetState(_returnTransitionTBState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = false;
                }
                else if (exitDirection == DragonflyExitEnterDirection.R || exitDirection == DragonflyExitEnterDirection.LRTB)
                {
                    SetState(_returnTransitionBTState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = false;
                }
                else
                {
                    // Other states
                    SetState(_moveToPatrolState, _visibleBodyTransform.position, -1, 1);
                    _isReturnResolved = true;
                }
                _returnState = DragonflyState.MoveToPatrolR;
                break;
            case DragonflyState.CatchSpiderL:
                if (exitDirection == DragonflyExitEnterDirection.LRBT)
                {
                    SetState(_returnTransitionTBState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = false;
                }
                else if (exitDirection == DragonflyExitEnterDirection.R)
                {
                    SetState(_returnTransitionBTState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = false;
                }
                else
                {
                    // Other states
                    SetState(_catchSpiderState, _visibleBodyTransform.position, 1, 1);
                    OnCatchSpiderStarted?.Invoke(1);
                    _isReturnResolved = true;
                }
                _returnState = DragonflyState.CatchSpiderL;
                break;
            case DragonflyState.CatchSpiderR:
                if (exitDirection == DragonflyExitEnterDirection.RLBT)
                {
                    SetState(_returnTransitionTBState, _visibleBodyTransform.position, -1, 1);
                    _isReturnResolved = false;
                }
                else if (exitDirection == DragonflyExitEnterDirection.L)
                {
                    SetState(_returnTransitionBTState, _visibleBodyTransform.position, -1, 1);
                    _isReturnResolved = false;
                }
                else
                {
                    // Other states
                    SetState(_catchSpiderState, _visibleBodyTransform.position, -1, 1);
                    OnCatchSpiderStarted?.Invoke(-1);
                    _isReturnResolved = true;
                }
                _returnState = DragonflyState.CatchSpiderR;
                break;
            case DragonflyState.EnterToHoverL:
                if (exitDirection == DragonflyExitEnterDirection.RLBT || exitDirection == DragonflyExitEnterDirection.L)
                {
                    SetState(_enterToHoverState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = true;
                }
                _returnState = DragonflyState.EnterToHoverL;
                break;
            case DragonflyState.EnterToHoverR:
                if (exitDirection == DragonflyExitEnterDirection.LRBT || exitDirection == DragonflyExitEnterDirection.R)
                {
                    SetState(_enterToHoverState, _visibleBodyTransform.position, -1, 1);
                    _isReturnResolved = true;
                }
                _returnState = DragonflyState.EnterToHoverR;
                break;
            case DragonflyState.MoveToHover:
                if (exitDirection == DragonflyExitEnterDirection.RLTB || exitDirection == DragonflyExitEnterDirection.LRTB)
                {
                    SetState(_moveToHoverState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = true;
                }
                _returnState = DragonflyState.MoveToHover;
                break;
        }
    }

    private void ApplyResolvedReturnTransition()
    {
        switch (_returnState)
        {
            case DragonflyState.MoveToPatrolL:
                SetState(_moveToPatrolState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.MoveToPatrolR:
                SetState(_moveToPatrolState, _visibleBodyTransform.position, -1, 1);
                break;
            case DragonflyState.CatchSpiderL:
                SetState(_catchSpiderState, _visibleBodyTransform.position, 1, 1);
                OnCatchSpiderStarted?.Invoke(1);    
                break;
            case DragonflyState.CatchSpiderR:
                SetState(_catchSpiderState, _visibleBodyTransform.position, -1, 1);
                OnCatchSpiderStarted?.Invoke(-1);
                break;
        }
        _isReturnResolved = true;
    }
    
    private DragonflyExitEnterDirection GetCurrentExitDirection(DragonflyState state, Vector3 position)
    {
        switch (state)
        {
            case DragonflyState.AttackHeadSuccess:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLBT;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRBT;
                }
            case DragonflyState.FallHead:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLTB;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRTB;
                }
            case DragonflyState.AttackTailSuccessL:
                return DragonflyExitEnterDirection.L;
            case DragonflyState.AttackTailSuccessR:
                return DragonflyExitEnterDirection.R;
            case DragonflyState.AttackTailFailL:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLTB;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRTB;
                }
            case DragonflyState.AttackTailFailR:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLTB;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRTB;
                }
            case DragonflyState.ReturnHover:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLTB;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRTB;
                }
            default:
                return DragonflyExitEnterDirection.L;
        }
    }
    
    private DragonflyState GetReturnState(DragonflyReturnMode mode, int sideDirection, DragonflyExitEnterDirection exitDirection)
    {
        if (mode == DragonflyReturnMode.Patrol)
        {
            if (sideDirection == 1)
            {
                return DragonflyState.MoveToPatrolL;
            }
            else
            {
                return DragonflyState.MoveToPatrolR;
            }
        }
        else if (mode == DragonflyReturnMode.Hover)
        {
            if (exitDirection == DragonflyExitEnterDirection.RLBT || exitDirection == DragonflyExitEnterDirection.L)
            {
                return DragonflyState.EnterToHoverL;
            }
            if (exitDirection == DragonflyExitEnterDirection.LRBT || exitDirection == DragonflyExitEnterDirection.R)
            {
                return DragonflyState.EnterToHoverR;
            }
            if (exitDirection == DragonflyExitEnterDirection.RLTB || exitDirection == DragonflyExitEnterDirection.LRTB)
            {
                return DragonflyState.MoveToHover;
            }
        }
        else if (mode == DragonflyReturnMode.Spider)
        {
            if (sideDirection == 1)
            {
                return DragonflyState.CatchSpiderL;
            }
            else
            {
                return DragonflyState.CatchSpiderR;
            }
        }
        return DragonflyState.Idle;
    }

    public void StartAttack(DragonflyPatrolAttackMode mode)
    {
        _currentPatrolAttackMode = mode;
        SwitchState();
    }
    
    private void MovementSetup(int state, int sideDirection)
    {
        _isDead = false;
        _isReceivedDamage = false;
        _sideDirection = sideDirection;
        if (state == 0)
        {
            SetState(_enterToPatrolState, _animatedTransform.position, _sideDirection, 1);
        }
        else
        {
            SetState(_enterToHoverState, _animatedTransform.position, _sideDirection, 1);
        }
    }

    public void MovementReset()
    {
    }


    public void TriggerBounce()
    {
        if (_currentMovementState.State == DragonflyState.AttackHead ||
            _currentMovementState.State == DragonflyState.AttackTailL ||
            _currentMovementState.State == DragonflyState.AttackTailR ||
            _currentMovementState.State == DragonflyState.AttackHover)
        {
            SwitchState();
        }
        else
        {
            Debug.LogWarning("Collided from non-attack state");
        }
    }
    
    public void TriggerFall(bool isReceivedDamage)
    {
        if (_currentMovementState.State == DragonflyState.BounceHead ||
            _currentMovementState.State == DragonflyState.BounceTailL ||
            _currentMovementState.State == DragonflyState.BounceTailR ||
            _currentMovementState.State == DragonflyState.BounceHover)
        {
            _isReceivedDamage = isReceivedDamage;
            SwitchState();
        }
    }
    
    public void TriggerDeath(bool isReceivedDamage)
    {
        _isReceivedDamage = isReceivedDamage;
        _isDead = true;
        SwitchState();
    }
    

    public void PlayClip(DragonflyState state)
    {
        AnimationClipPlayable clipPlayable = _playablesContainer.GetClip(state);
        clipPlayable.SetTime(0);
        clipPlayable.SetTime(0); // Unity Bug
        _playableOutput.SetSourcePlayable(clipPlayable);
        if (_playableGraph.IsValid())
        {
            _playableGraph.Play();    
        }
    }
    
    public void SwitchState()
    {
        bool isHit = false;
        switch (_currentMovementState.State)
        {
            //// -- Enter Patrol --
            case DragonflyState.EnterToPatrolL:
                SetState(_patrolState, _visibleBodyTransform.position, 1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.PatrolL, _prevDragonflyState);
                break;
            case DragonflyState.EnterToPatrolR:
                SetState(_patrolState, _visibleBodyTransform.position, -1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.PatrolR, _prevDragonflyState);
                break;
            case DragonflyState.MoveToPatrolL:
                SetState(_patrolState, _visibleBodyTransform.position, 1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.PatrolL, _prevDragonflyState);
                break;
            case DragonflyState.MoveToPatrolR:
                SetState(_patrolState, _visibleBodyTransform.position, -1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.PatrolR, _prevDragonflyState);
                break;
            // Patrol --> PreAttack Head 
            case DragonflyState.PatrolL:
                OnPreattackStarted?.Invoke();
                if (_currentPatrolAttackMode == DragonflyPatrolAttackMode.Head)
                {
                    SetState(_preAttackHeadState, _visibleBodyTransform.position, 1, 1);
                }
                else if (_currentPatrolAttackMode == DragonflyPatrolAttackMode.Tail)
                {
                    SetState(_preAttackTailState, _visibleBodyTransform.position, 1, 1);
                }
                break;
            case DragonflyState.PatrolR:
                OnPreattackStarted?.Invoke();
                if (_currentPatrolAttackMode == DragonflyPatrolAttackMode.Head)
                {
                    SetState(_preAttackHeadState, _visibleBodyTransform.position, -1, 1);
                }
                else if (_currentPatrolAttackMode == DragonflyPatrolAttackMode.Tail)
                {
                    SetState(_preAttackTailState, _visibleBodyTransform.position, -1, 1);
                }
                break;
            // Head attack
            case DragonflyState.PreAttackHeadL:
                OnAttackStarted?.Invoke();
                _isCollided = false;
                SetState(_attackHeadState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.PreAttackHeadR:
                OnAttackStarted?.Invoke();
                _isCollided = false;
                SetState(_attackHeadState, _visibleBodyTransform.position, 1, 1);
                break;
            // Bounce Head
            case DragonflyState.AttackHead:
                SetState(_bounceHeadState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.BounceHead:
                OnAttackEnded?.Invoke();
                if (_isReceivedDamage)
                {
                    if (_isDead)
                    {
                        SetState(_deathHeadState, _visibleBodyTransform.position, 1, 1);
                        Debug.Log("Dragonfly Death State started");
                    }
                    else
                    {
                        SetState(_fallHeadLState, _visibleBodyTransform.position, 1, 1);    
                    }
                    _isReceivedDamage = false;
                }
                else
                {
                    SetState(_attackHeadSuccess, _visibleBodyTransform.position, 1, 1);
                    _isReceivedDamage = false;
                }
                break;
            case DragonflyState.AttackHeadSuccess:
                OnAfterAttackExitEnded?.Invoke(DragonflyState.AttackHeadSuccess);
                break;
            case DragonflyState.FallHead:
                OnAfterAttackExitEnded?.Invoke(DragonflyState.FallHead);
                break;
            // Tail attack
            case DragonflyState.PreAttackTailL:
                OnAttackStarted?.Invoke();
                _isCollided = false;
                SetState(_attackTailState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.PreAttackTailR:
                OnAttackStarted?.Invoke();
                _isCollided = false;
                SetState(_attackTailState, _visibleBodyTransform.position, -1, 1);
                break;
            // Bounce Tail
            case DragonflyState.AttackTailL:
                SetState(_bounceTailState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.AttackTailR:
                SetState(_bounceTailState, _visibleBodyTransform.position, -1, 1);
                break;
            case DragonflyState.BounceTailL:
                OnAttackEnded?.Invoke();
                if (_isReceivedDamage)
                {
                    if (_isDead)
                    {
                        SetState(_deathTailState, _visibleBodyTransform.position, 1, 1);
                    }
                    else
                    {
                        SetState(_attackTailFail, _visibleBodyTransform.position, 1, 1);    
                    }
                    _isReceivedDamage = false;
                }
                else
                {
                    SetState(_attackTailSuccess, _visibleBodyTransform.position, 1, 1);
                    _isReceivedDamage = false;
                }
                break;
            case DragonflyState.BounceTailR:
                OnAttackEnded?.Invoke();
                if (_isReceivedDamage)
                {
                    if (_isDead)
                    {
                        SetState(_deathTailState, _visibleBodyTransform.position, -1, 1);
                    }
                    else
                    {
                        SetState(_attackTailFail, _visibleBodyTransform.position, -1, 1);
                    }
                    _isReceivedDamage = false;
                }
                else
                {
                    SetState(_attackTailSuccess, _visibleBodyTransform.position, -1, 1);
                    _isReceivedDamage = false;
                }
                break;
            // Exit Tail Attack
            case DragonflyState.AttackTailSuccessL:
                OnAfterAttackExitEnded?.Invoke(DragonflyState.AttackTailSuccessL);
                break;
            case DragonflyState.AttackTailSuccessR:
                OnAfterAttackExitEnded?.Invoke(DragonflyState.AttackTailSuccessR);
                break;
            case DragonflyState.AttackTailFailL:
                OnAfterAttackExitEnded?.Invoke(DragonflyState.AttackTailFailL);
                break;
            case DragonflyState.AttackTailFailR:
                OnAfterAttackExitEnded?.Invoke(DragonflyState.AttackTailFailL);
                break;
            //// --- Enter Hover ---
            case DragonflyState.EnterToHoverL:
                SetState(_hoverState, _visibleBodyTransform.position, 1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.Hover, _prevDragonflyState);
                break;
            case DragonflyState.EnterToHoverR:
                SetState(_hoverState, _visibleBodyTransform.position, -1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.Hover, _prevDragonflyState);
                break;
            case DragonflyState.MoveToHover:
                SetState(_hoverState, _visibleBodyTransform.position, 1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.Hover, _prevDragonflyState);
                break;
            // Hover --> PreAttack Hover
            case DragonflyState.Hover:
                OnPreattackStarted?.Invoke();
                SetState(_preAttackHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            // Hover attack
            case DragonflyState.PreAttackHover:
                SetState(_attackHoverState, _visibleBodyTransform.position, 1, 1);
                OnAttackStarted?.Invoke();
                _isCollided = false;
                break;
            // Hover Bounce
            case DragonflyState.AttackHover:
                SetState(_bounceHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.BounceHover:
                // TODO:
                OnAttackEnded?.Invoke();
                if (_isReceivedDamage)
                {
                    if (_isDead)
                    {
                        SetState(_deathHeadState, _visibleBodyTransform.position, 1, 1);
                    }
                    else
                    {
                        SetState(_fallHeadLState, _visibleBodyTransform.position, 1, 1);    
                    }
                    _isReceivedDamage = false;
                }
                else
                {
                    SetState(_returnHoverState, _visibleBodyTransform.position, 1, 1);
                    _isReceivedDamage = false;
                }
                break;
            case DragonflyState.ReturnHover:
                OnAfterAttackExitEnded?.Invoke(DragonflyState.ReturnHover);
                break;
            // -- Catch Spider --
            // Spider 
            case DragonflyState.CatchSpiderL:
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.SpiderPatrolL, _prevDragonflyState);
                SetState(_spiderPatrolState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.CatchSpiderR:
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.SpiderPatrolR, _prevDragonflyState);
                SetState(_spiderPatrolState, _visibleBodyTransform.position, -1, 1);
                break;
            //
            case DragonflyState.SpiderPatrolL:
                SetState(_spiderPushState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.SpiderPatrolR:
                SetState(_spiderPushState, _visibleBodyTransform.position, -1, 1);
                break;
            case DragonflyState.SpiderPushL:
                SetState(_spiderPreAttackHeadTransitionState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.SpiderPushR:
                SetState(_spiderPreAttackHeadTransitionState, _visibleBodyTransform.position, -1, 1);
                break;
            // //
            case DragonflyState.SpiderPreattackHeadTransitionStateL:
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.PatrolL, _prevDragonflyState);
                SetState(_patrolState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyState.SpiderPreattackHeadTransitionStateR:
                OnReadyToAttackStateEntered?.Invoke(DragonflyState.PatrolR, _prevDragonflyState);
                SetState(_patrolState, _visibleBodyTransform.position, -1, 1);
                break;
            case DragonflyState.DeathHead:
                OnDeathAnimationEnded?.Invoke();
                break;
            case DragonflyState.DeathTailL:
                OnDeathAnimationEnded?.Invoke();
                break;
            case DragonflyState.DeathTailR: 
                OnDeathAnimationEnded?.Invoke();
                break;
        }
    }

    
    private void Update()
    {
        // Start Head attack
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_currentMovementState.State == DragonflyState.PatrolL)
            {
                SetState(_preAttackHeadState, _visibleBodyTransform.position, 1, 1);
            }
            if (_currentMovementState.State == DragonflyState.PatrolR)
            {
                SetState(_preAttackHeadState, _visibleBodyTransform.position, -1, 1);
            }
            
            if (_currentMovementState.State == DragonflyState.Hover)
            {
                SetState(_preAttackHoverState, _visibleBodyTransform.position, 1, 1);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetState(_patrolState, _visibleBodyTransform.position, 1, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetState(_patrolState, _visibleBodyTransform.position, -1, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_currentMovementState.State == DragonflyState.PatrolL)
            {
                SetState(_preAttackTailState, _visibleBodyTransform.position, 1, 1);    
            }
            if (_currentMovementState.State == DragonflyState.PatrolR)
            {
                SetState(_preAttackTailState, _visibleBodyTransform.position, -1, 1);    
            }
            
            Debug.Log($"preattack tail pos ---- {_visibleBodyTransform.position.normalized}");
        }
        
        _currentMovementState.CheckForStateChange();
        _currentMovementState.ExecuteState(_visibleBodyTransform.position);
        currentDragonflyState = _currentMovementState.State;
    }
    
    private void SetState(DragonflyMovementBaseState toState, Vector3 position, int sideDirection, int depthDirection)
    {
        if (_isReturnResolved)
        {
            _prevDragonflyState = _currentMovementState.State;
        }
        else
        {
            _prevDragonflyState = _returnState;
        }
        _currentMovementState.ExitState();
        _currentMovementState = toState;
        _currentMovementState.EnterState(position, sideDirection, 1);
    }

    private void OnDestroy()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();    
        }
    }
    
    private void OnClipEnded()
    {
        if (_isReturnResolved)
        {
            SwitchState();    
        }
        else
        {
            ApplyResolvedReturnTransition();
        }
        
    }
   
    // private void OnCollision()
    // {
    //     if (!_isCollided)
    //     {
    //         Debug.Log("Collision");
    //         if (currentDragonflyState == DragonflyState.AttackHead)
    //         {
    //             _collisionCatcher.DisableColliders();
    //             _isCollided = true;
    //             SwitchState();
    //         }
    //         if (currentDragonflyState == DragonflyState.AttackHover)
    //         {
    //             _collisionCatcher.DisableColliders();
    //             _isCollided = true;
    //             SwitchState();
    //         }
    //         if (currentDragonflyState == DragonflyState.AttackTailL || currentDragonflyState == DragonflyState.AttackTailR)
    //         {
    //             _collisionCatcher.DisableColliders();
    //             _isCollided = true;
    //             SwitchState();
    //         }    
    //     }
    // }
}
