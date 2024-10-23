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
    
    private FStateMachine _stateMachine = new FStateMachine();
    
    // Animation 
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    private int _sideDirection = 1;
    private DragonflyPatrolAttackMode _currentPatrolAttackMode; 

    // Return Resolve
    private bool _isReturnResolved = true;
    private DragonflyMovementState _returnMovementState;
    private int _returnSideDirection;
    
    private bool _isCollided = false;
    private bool _isReceivedDamage = false;
    private bool _isDead = false;
    
    private void Awake()
    {
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
        At(_idleState, _enterToPatrolStateL, () => Input.GetAxis("Horizontal") > 0);
        
        
        _stateMachine.SetState(_idleState);
        
        // Transition helper methods
        void At(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition);
        
        // Transition Predicates
    }

    private void Update()
    {
        _stateMachine.Tick();
        // Show current state for debug
        _currentStateType = _stateMachine.CurrentStateType.ToString().Replace("FDragonfly", "");
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

    
    private void OnDestroy()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();    
        }
    }

}
