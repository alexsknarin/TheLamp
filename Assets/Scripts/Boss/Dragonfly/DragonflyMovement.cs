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
    [SerializeField] private DragonflyCollisionCatcher _collisionCatcher;
    [SerializeField] private DragonflyStates _currentDragonflyState;
    [SerializeField] private DragonflyAnimationClipEventHandler _animationClipEvents;
    
    [Header("Animation Clips")]
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _enterToPatrolLClip;
    [SerializeField] private AnimationClip _catchSpiderLClip;
    [SerializeField] private AnimationClip _enterToHoverLClip;

    [Header("States")]  
    [SerializeField] private DragonflyMovementBaseState _idleState;
    [SerializeField] private DragonflyEnterToPatrolState _enterToPatrolStateMono;
    private DragonflyMovementBaseState _enterToPatrolState;
    [SerializeField] private DragonflyPatrolState _patrolStateMono;
    private DragonflyMovementBaseState _patrolState;
    [SerializeField] private DragonflyPreAttackHeadState _preAttackHeadStateMono;
    private DragonflyMovementBaseState _preAttackHeadState;
    [SerializeField] private DragonflyMovementBaseState _attackHeadState;
    [SerializeField] private DragonflyMovementBaseState _bounceHeadState;
    [SerializeField] private DragonflyMovementBaseState _fallHeadLState;
    [SerializeField] private DragonflyCatchSpiderState _catchSpiderStateMono;
    private DragonflyMovementBaseState _catchSpiderState;
    [SerializeField] private DragonflySpiderPatrolState _spiderPatrolStateMono;
    private DragonflyMovementBaseState _spiderPatrolState;
    [SerializeField] private DragonflyMovementBaseState _spiderPreAttackHeadTransitionState;
    [SerializeField] private DragonflyMovementBaseState _preAttackTailState;
    [SerializeField] private DragonflyMovementBaseState _attackTailState;
    [SerializeField] private DragonflyMovementBaseState _bounceTailState;
    [SerializeField] private DragonflyMovementBaseState _attackTailSuccess;
    [SerializeField] private DragonflyMovementBaseState _attackTailFail;
    [SerializeField] private DragonflyMovementBaseState _attackHeadSuccess;
    [SerializeField] private DragonflyEnterToHoverState _enterToHoverStateMono;
    private DragonflyMovementBaseState _enterToHoverState;
    [SerializeField] private DragonflyMovementBaseState _hoverState;
    
    
    private DragonflyMovementBaseState _currentMovementState;
    private DragonflyStates _prevDragonflyState;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    
    private bool _isCollided = false;
    
    
    private void OnEnable()
    {
        _animationClipEvents.OnClipEndedEvent += OnClipEnded;
        _collisionCatcher.OnCollidedEvent += OnCollision;
    }
    
    private void OnDisable()
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Destroy();    
        }
        _animationClipEvents.OnClipEndedEvent -= OnClipEnded;
        _collisionCatcher.OnCollidedEvent -= OnCollision;
    }


    private void Start()
    {
        _playableGraph = PlayableGraph.Create();
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        _playableOutput = AnimationPlayableOutput.Create(_playableGraph, "Animation", _animator);
        _playablesContainer = new DragonflyPlayablesContainer(_playableGraph);
        
        // Add clips
        _playablesContainer.AddClip(DragonflyStates.Idle, _idleClip);
        _playablesContainer.AddClip(DragonflyStates.EnterToPatrolL, _enterToPatrolLClip);  
        _playablesContainer.AddClip(DragonflyStates.CatchSpiderL, _catchSpiderLClip);
        _playablesContainer.AddClip(DragonflyStates.EnterToHoverL, _enterToHoverLClip);
        
        // Initialize States
        _idleState.SetCommonStateDependencies(this, _visibleBodyTransform);
        
        _enterToPatrolStateMono.Initialize(_playableOutput, _playableGraph, _playablesContainer.GetClip(DragonflyStates.EnterToPatrolL));
        _enterToPatrolState = _enterToPatrolStateMono;
        _enterToPatrolState.SetCommonStateDependencies(this, _visibleBodyTransform);

        _patrolState = _patrolStateMono;
        _patrolState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _preAttackHeadState = _preAttackHeadStateMono;
        _preAttackHeadState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _attackHeadState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _bounceHeadState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _fallHeadLState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _catchSpiderStateMono.Initialize(_playableOutput, _playableGraph, _playablesContainer.GetClip(DragonflyStates.CatchSpiderL));
        _catchSpiderState = _catchSpiderStateMono;
        _catchSpiderState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _spiderPatrolState = _spiderPatrolStateMono; 
        _spiderPatrolState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _spiderPreAttackHeadTransitionState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _preAttackTailState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _attackTailState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _bounceTailState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _attackTailSuccess.SetCommonStateDependencies(this, _visibleBodyTransform);
        _attackTailFail.SetCommonStateDependencies(this, _visibleBodyTransform);
        _attackHeadSuccess.SetCommonStateDependencies(this, _visibleBodyTransform);
        _enterToHoverStateMono.Initialize(_playableOutput, _playableGraph, _playablesContainer.GetClip(DragonflyStates.EnterToHoverL));
        _enterToHoverState = _enterToHoverStateMono;
        _enterToHoverState.SetCommonStateDependencies(this, _visibleBodyTransform);
        _hoverState.SetCommonStateDependencies(this, _visibleBodyTransform);
        // 

        _currentMovementState = _idleState;
        
        _currentDragonflyState = DragonflyStates.Idle;
        _collisionCatcher.DisableColliders();
    }

    
    public void SwitchState()
    {
        switch (_currentMovementState.State)
        {
            case DragonflyStates.EnterToPatrolL:
                SwitchState(_patrolState, _animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.PatrolL:
                SwitchState(_preAttackHeadState, _animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.PreAttackHeadL:
                _collisionCatcher.EnableColliders();
                _isCollided = false;
                SwitchState(_attackHeadState, _animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.AttackHeadL:
                _collisionCatcher.DisableColliders();
                SwitchState(_bounceHeadState, _animatedTransform.position, 1, 1);
                break;
            // case DragonflyStates.BounceHeadL:
            //     SwitchState(_fallHeadLState, _animatedTransform.position, 1, 1);
            //     break;
            
            case DragonflyStates.BounceHeadL:
                SwitchState(_attackHeadSuccess, _animatedTransform.position, 1, 1);
                break;
            
            case DragonflyStates.CatchSpiderL:
                SwitchState(_spiderPatrolState, _animatedTransform.position, 1, 1);
                break;
            
            case DragonflyStates.SpiderPreattackHeadTransitionStateL:
                SwitchState(_patrolState, _visibleBodyTransform.position, 1, 1);
                break;
            
            case DragonflyStates.PreAttackTailL:
                _collisionCatcher.EnableColliders();
                _isCollided = false;
                SwitchState(_attackTailState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.AttackTailL:
                _collisionCatcher.DisableColliders();
                SwitchState(_bounceTailState, _visibleBodyTransform.position, 1, 1);
                break;
            
            // case DragonflyStates.BounceTailL:
            //     SwitchState(_attackTailSuccess, _visibleBodyTransform.position, 1, 1);
            //     break;
            
            case DragonflyStates.BounceTailL:
                SwitchState(_attackTailFail, _visibleBodyTransform.position, 1, 1);
                break;
            
            case DragonflyStates.EnterToHoverL:
                SwitchState(_hoverState, _visibleBodyTransform.position, 1, 1);
                break;
            
        }
    }

    private void Update()
    {
        // Start Enter to patrol
        if (Input.GetKeyDown(KeyCode.A))
        {
            SwitchState(_enterToPatrolState, _animatedTransform.position, 1, 1);
        }
        
        // Start Catch Spider
        if (Input.GetKeyDown(KeyCode.S))
        {
            SwitchState(_catchSpiderState, _animatedTransform.position, 1, 1);
        }
        // Start Spider patrol transition
        if (Input.GetKeyDown(KeyCode.D))
        {
            SwitchState(_spiderPreAttackHeadTransitionState, _visibleBodyTransform.position, 1, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchState(_preAttackTailState, _visibleBodyTransform.position, 1, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchState(_enterToHoverState, _visibleBodyTransform.position, 1, 1);
        }
        
        
        // Start Head attack
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_currentMovementState.State == DragonflyStates.PatrolL)
            {
                SwitchState(_preAttackHeadState, _animatedTransform.position, 1, 1);
            }
        }
        
        _currentMovementState.CheckForStateChange();
        _currentMovementState.ExecuteState(_animatedTransform.position);
        _currentDragonflyState = _currentMovementState.State;
    }
    
    private void SwitchState(DragonflyMovementBaseState toState, Vector3 position, int sideDirection, int depthDirection)
    {
        _currentMovementState.ExitState();
        _currentMovementState = toState;
        _currentMovementState.EnterState(position, 1, 1);
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
        switch (_currentMovementState.State)
        {   
            case DragonflyStates.EnterToPatrolL:
                SwitchState();
                break;
        }
    }
   
    private void OnCollision()
    {
        if (!_isCollided)
        {
            Debug.Log("Collision");
            if (_currentDragonflyState == DragonflyStates.AttackHeadL)
            {
                _collisionCatcher.DisableColliders();
                _isCollided = true;
                SwitchState();
            }
            if (_currentDragonflyState == DragonflyStates.AttackTailL)
            {
                _collisionCatcher.DisableColliders();
                _isCollided = true;
                SwitchState();
            }    
        }
    }
}
