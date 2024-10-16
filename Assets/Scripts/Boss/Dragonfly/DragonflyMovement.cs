using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class DragonflyMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private Transform _animatedTransform;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private Transform _spiderPatrolTransform;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private DragonflyPatrolRotator _spiderPatrolRotator;
    
    [SerializeField] private DragonflyMovementState _currentDragonflyMovementState;
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
    
    public DragonflyMovementState MovementState => _currentMovementState.State;
    
    public event Action<DragonflyMovementState, DragonflyMovementState> OnReadyToAttackStateEntered; 
    public event Action<DragonflyMovementState> OnAfterAttackExitEnded;
    public event Action OnAttackStarted;
    public event Action OnPreattackStarted;
    public event Action OnAttackEnded;
    public event Action<int> OnCatchSpiderStarted;
    public event Action OnDeathAnimationEnded; 
    
    private DragonflyMovementStateData _stateData;
    private DragonflyMovementBaseState _currentMovementState;
    [SerializeField] private DragonflyMovementState _prevDragonflyMovementState;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    
    private int _sideDirection = 1;
    private DragonflyPatrolAttackMode _currentPatrolAttackMode; 
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    // Add state switch variables
    
    // Return Resolve
    private bool _isReturnResolved = true;
    private DragonflyMovementState _returnMovementState;
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
        _currentDragonflyMovementState = DragonflyMovementState.Idle;
    }

    public void Play(int state, int sideDirection)
    {
        MovementSetup(state, sideDirection);
    }
    
    public void ResolveReturnTransition(DragonflyReturnMode mode, int sideDirection)
    {
        _prevDragonflyMovementState = _currentMovementState.State;
        _sideDirection = sideDirection;
        DragonflyExitEnterDirection exitDirection = GetCurrentExitDirection(_currentMovementState.State, _visibleBodyTransform.position);
        DragonflyMovementState returnMovementMovementState = GetReturnState(mode, sideDirection, exitDirection);
        
        // Set return state
        switch (returnMovementMovementState)
        {
            case DragonflyMovementState.MoveToPatrolL:
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
                _returnMovementState = DragonflyMovementState.MoveToPatrolL;
                break;
            case DragonflyMovementState.MoveToPatrolR:
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
                _returnMovementState = DragonflyMovementState.MoveToPatrolR;
                break;
            case DragonflyMovementState.CatchSpiderL:
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
                _returnMovementState = DragonflyMovementState.CatchSpiderL;
                break;
            case DragonflyMovementState.CatchSpiderR:
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
                _returnMovementState = DragonflyMovementState.CatchSpiderR;
                break;
            case DragonflyMovementState.EnterToHoverL:
                if (exitDirection == DragonflyExitEnterDirection.RLBT || exitDirection == DragonflyExitEnterDirection.L)
                {
                    SetState(_enterToHoverState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = true;
                }
                _returnMovementState = DragonflyMovementState.EnterToHoverL;
                break;
            case DragonflyMovementState.EnterToHoverR:
                if (exitDirection == DragonflyExitEnterDirection.LRBT || exitDirection == DragonflyExitEnterDirection.R)
                {
                    SetState(_enterToHoverState, _visibleBodyTransform.position, -1, 1);
                    _isReturnResolved = true;
                }
                _returnMovementState = DragonflyMovementState.EnterToHoverR;
                break;
            case DragonflyMovementState.MoveToHover:
                if (exitDirection == DragonflyExitEnterDirection.RLTB || exitDirection == DragonflyExitEnterDirection.LRTB)
                {
                    SetState(_moveToHoverState, _visibleBodyTransform.position, 1, 1);
                    _isReturnResolved = true;
                }
                _returnMovementState = DragonflyMovementState.MoveToHover;
                break;
        }
    }

    private void ApplyResolvedReturnTransition()
    {
        switch (_returnMovementState)
        {
            case DragonflyMovementState.MoveToPatrolL:
                SetState(_moveToPatrolState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.MoveToPatrolR:
                SetState(_moveToPatrolState, _visibleBodyTransform.position, -1, 1);
                break;
            case DragonflyMovementState.CatchSpiderL:
                SetState(_catchSpiderState, _visibleBodyTransform.position, 1, 1);
                OnCatchSpiderStarted?.Invoke(1);    
                break;
            case DragonflyMovementState.CatchSpiderR:
                SetState(_catchSpiderState, _visibleBodyTransform.position, -1, 1);
                OnCatchSpiderStarted?.Invoke(-1);
                break;
        }
        _isReturnResolved = true;
    }
    
    private DragonflyExitEnterDirection GetCurrentExitDirection(DragonflyMovementState movementState, Vector3 position)
    {
        switch (movementState)
        {
            case DragonflyMovementState.AttackHeadSuccess:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLBT;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRBT;
                }
            case DragonflyMovementState.FallHead:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLTB;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRTB;
                }
            case DragonflyMovementState.AttackTailSuccessL:
                return DragonflyExitEnterDirection.L;
            case DragonflyMovementState.AttackTailSuccessR:
                return DragonflyExitEnterDirection.R;
            case DragonflyMovementState.AttackTailFailL:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLTB;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRTB;
                }
            case DragonflyMovementState.AttackTailFailR:
                if (position.x < 0)
                {
                    return DragonflyExitEnterDirection.RLTB;
                }
                else
                {
                    return DragonflyExitEnterDirection.LRTB;
                }
            case DragonflyMovementState.ReturnHover:
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
    
    private DragonflyMovementState GetReturnState(DragonflyReturnMode mode, int sideDirection, DragonflyExitEnterDirection exitDirection)
    {
        if (mode == DragonflyReturnMode.Patrol)
        {
            if (sideDirection == 1)
            {
                return DragonflyMovementState.MoveToPatrolL;
            }
            else
            {
                return DragonflyMovementState.MoveToPatrolR;
            }
        }
        else if (mode == DragonflyReturnMode.Hover)
        {
            if (exitDirection == DragonflyExitEnterDirection.RLBT || exitDirection == DragonflyExitEnterDirection.L)
            {
                return DragonflyMovementState.EnterToHoverL;
            }
            if (exitDirection == DragonflyExitEnterDirection.LRBT || exitDirection == DragonflyExitEnterDirection.R)
            {
                return DragonflyMovementState.EnterToHoverR;
            }
            if (exitDirection == DragonflyExitEnterDirection.RLTB || exitDirection == DragonflyExitEnterDirection.LRTB)
            {
                return DragonflyMovementState.MoveToHover;
            }
        }
        else if (mode == DragonflyReturnMode.Spider)
        {
            if (sideDirection == 1)
            {
                return DragonflyMovementState.CatchSpiderL;
            }
            else
            {
                return DragonflyMovementState.CatchSpiderR;
            }
        }
        return DragonflyMovementState.Idle;
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
        if (_currentMovementState.State == DragonflyMovementState.AttackHead ||
            _currentMovementState.State == DragonflyMovementState.AttackTailL ||
            _currentMovementState.State == DragonflyMovementState.AttackTailR ||
            _currentMovementState.State == DragonflyMovementState.AttackHover)
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
        if (_currentMovementState.State == DragonflyMovementState.BounceHead ||
            _currentMovementState.State == DragonflyMovementState.BounceTailL ||
            _currentMovementState.State == DragonflyMovementState.BounceTailR ||
            _currentMovementState.State == DragonflyMovementState.BounceHover)
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
    

    public void PlayClip(DragonflyMovementState movementState)
    {
        AnimationClipPlayable clipPlayable = _playablesContainer.GetClip(movementState);
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
            case DragonflyMovementState.EnterToPatrolL:
                SetState(_patrolState, _visibleBodyTransform.position, 1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.PatrolL, _prevDragonflyMovementState);
                break;
            case DragonflyMovementState.EnterToPatrolR:
                SetState(_patrolState, _visibleBodyTransform.position, -1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.PatrolR, _prevDragonflyMovementState);
                break;
            case DragonflyMovementState.MoveToPatrolL:
                SetState(_patrolState, _visibleBodyTransform.position, 1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.PatrolL, _prevDragonflyMovementState);
                break;
            case DragonflyMovementState.MoveToPatrolR:
                SetState(_patrolState, _visibleBodyTransform.position, -1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.PatrolR, _prevDragonflyMovementState);
                break;
            // Patrol --> PreAttack Head 
            case DragonflyMovementState.PatrolL:
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
            case DragonflyMovementState.PatrolR:
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
            case DragonflyMovementState.PreAttackHeadL:
                OnAttackStarted?.Invoke();
                _isCollided = false;
                SetState(_attackHeadState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.PreAttackHeadR:
                OnAttackStarted?.Invoke();
                _isCollided = false;
                SetState(_attackHeadState, _visibleBodyTransform.position, 1, 1);
                break;
            // Bounce Head
            case DragonflyMovementState.AttackHead:
                SetState(_bounceHeadState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.BounceHead:
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
            case DragonflyMovementState.AttackHeadSuccess:
                OnAfterAttackExitEnded?.Invoke(DragonflyMovementState.AttackHeadSuccess);
                break;
            case DragonflyMovementState.FallHead:
                OnAfterAttackExitEnded?.Invoke(DragonflyMovementState.FallHead);
                break;
            // Tail attack
            case DragonflyMovementState.PreAttackTailL:
                OnAttackStarted?.Invoke();
                _isCollided = false;
                SetState(_attackTailState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.PreAttackTailR:
                OnAttackStarted?.Invoke();
                _isCollided = false;
                SetState(_attackTailState, _visibleBodyTransform.position, -1, 1);
                break;
            // Bounce Tail
            case DragonflyMovementState.AttackTailL:
                SetState(_bounceTailState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.AttackTailR:
                SetState(_bounceTailState, _visibleBodyTransform.position, -1, 1);
                break;
            case DragonflyMovementState.BounceTailL:
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
            case DragonflyMovementState.BounceTailR:
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
            case DragonflyMovementState.AttackTailSuccessL:
                OnAfterAttackExitEnded?.Invoke(DragonflyMovementState.AttackTailSuccessL);
                break;
            case DragonflyMovementState.AttackTailSuccessR:
                OnAfterAttackExitEnded?.Invoke(DragonflyMovementState.AttackTailSuccessR);
                break;
            case DragonflyMovementState.AttackTailFailL:
                OnAfterAttackExitEnded?.Invoke(DragonflyMovementState.AttackTailFailL);
                break;
            case DragonflyMovementState.AttackTailFailR:
                OnAfterAttackExitEnded?.Invoke(DragonflyMovementState.AttackTailFailL);
                break;
            //// --- Enter Hover ---
            case DragonflyMovementState.EnterToHoverL:
                SetState(_hoverState, _visibleBodyTransform.position, 1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.Hover, _prevDragonflyMovementState);
                break;
            case DragonflyMovementState.EnterToHoverR:
                SetState(_hoverState, _visibleBodyTransform.position, -1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.Hover, _prevDragonflyMovementState);
                break;
            case DragonflyMovementState.MoveToHover:
                SetState(_hoverState, _visibleBodyTransform.position, 1, 1);
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.Hover, _prevDragonflyMovementState);
                break;
            // Hover --> PreAttack Hover
            case DragonflyMovementState.Hover:
                OnPreattackStarted?.Invoke();
                SetState(_preAttackHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            // Hover attack
            case DragonflyMovementState.PreAttackHover:
                SetState(_attackHoverState, _visibleBodyTransform.position, 1, 1);
                OnAttackStarted?.Invoke();
                _isCollided = false;
                break;
            // Hover Bounce
            case DragonflyMovementState.AttackHover:
                SetState(_bounceHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.BounceHover:
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
            case DragonflyMovementState.ReturnHover:
                OnAfterAttackExitEnded?.Invoke(DragonflyMovementState.ReturnHover);
                break;
            // -- Catch Spider --
            // Spider 
            case DragonflyMovementState.CatchSpiderL:
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.SpiderPatrolL, _prevDragonflyMovementState);
                SetState(_spiderPatrolState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.CatchSpiderR:
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.SpiderPatrolR, _prevDragonflyMovementState);
                SetState(_spiderPatrolState, _visibleBodyTransform.position, -1, 1);
                break;
            //
            case DragonflyMovementState.SpiderPatrolL:
                SetState(_spiderPushState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.SpiderPatrolR:
                SetState(_spiderPushState, _visibleBodyTransform.position, -1, 1);
                break;
            case DragonflyMovementState.SpiderPushL:
                SetState(_spiderPreAttackHeadTransitionState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.SpiderPushR:
                SetState(_spiderPreAttackHeadTransitionState, _visibleBodyTransform.position, -1, 1);
                break;
            // //
            case DragonflyMovementState.SpiderPreattackHeadTransitionStateL:
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.PatrolL, _prevDragonflyMovementState);
                SetState(_patrolState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyMovementState.SpiderPreattackHeadTransitionStateR:
                OnReadyToAttackStateEntered?.Invoke(DragonflyMovementState.PatrolR, _prevDragonflyMovementState);
                SetState(_patrolState, _visibleBodyTransform.position, -1, 1);
                break;
            case DragonflyMovementState.DeathHead:
                OnDeathAnimationEnded?.Invoke();
                break;
            case DragonflyMovementState.DeathTailL:
                OnDeathAnimationEnded?.Invoke();
                break;
            case DragonflyMovementState.DeathTailR: 
                OnDeathAnimationEnded?.Invoke();
                break;
        }
    }

    
    private void Update()
    {
        // Start Head attack
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_currentMovementState.State == DragonflyMovementState.PatrolL)
            {
                SetState(_preAttackHeadState, _visibleBodyTransform.position, 1, 1);
            }
            if (_currentMovementState.State == DragonflyMovementState.PatrolR)
            {
                SetState(_preAttackHeadState, _visibleBodyTransform.position, -1, 1);
            }
            
            if (_currentMovementState.State == DragonflyMovementState.Hover)
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
            if (_currentMovementState.State == DragonflyMovementState.PatrolL)
            {
                SetState(_preAttackTailState, _visibleBodyTransform.position, 1, 1);    
            }
            if (_currentMovementState.State == DragonflyMovementState.PatrolR)
            {
                SetState(_preAttackTailState, _visibleBodyTransform.position, -1, 1);    
            }
            
            Debug.Log($"preattack tail pos ---- {_visibleBodyTransform.position.normalized}");
        }
        
        _currentMovementState.CheckForStateChange();
        _currentMovementState.ExecuteState(_visibleBodyTransform.position);
        _currentDragonflyMovementState = _currentMovementState.State;
    }
    
    private void SetState(DragonflyMovementBaseState toState, Vector3 position, int sideDirection, int depthDirection)
    {
        if (_isReturnResolved)
        {
            _prevDragonflyMovementState = _currentMovementState.State;
        }
        else
        {
            _prevDragonflyMovementState = _returnMovementState;
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
