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
    
    private DragonflyMovementBaseState _currentMovementState;
    private DragonflyStates _prevDragonflyState;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
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
        
        // Initialize States
        _enterToPatrolStateMono.Initialize(_playableOutput, _playableGraph, _playablesContainer.GetClip(DragonflyStates.EnterToPatrolL));
        _enterToPatrolState = _enterToPatrolStateMono;
        _patrolState = _patrolStateMono;
        _preAttackHeadState = _preAttackHeadStateMono;
        _catchSpiderStateMono.Initialize(_playableOutput, _playableGraph, _playablesContainer.GetClip(DragonflyStates.CatchSpiderL));
        _catchSpiderState = _catchSpiderStateMono;
        _spiderPatrolState = _spiderPatrolStateMono; 
        // init???

        _currentMovementState = _idleState;
        
        _currentDragonflyState = DragonflyStates.Idle;
        _collisionCatcher.DisableColliders();
    }

    
    public void SwitchState()
    {
        switch (_currentMovementState.State)
        {
            case DragonflyStates.EnterToPatrolL:
                _currentMovementState = _patrolState;
                _currentMovementState.EnterState(_animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.PatrolL:
                _currentMovementState = _preAttackHeadState;
                _currentMovementState.EnterState(_animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.PreAttackHeadL:
                _collisionCatcher.EnableColliders();
                _currentMovementState = _attackHeadState;
                _currentMovementState.EnterState(_animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.AttackHeadL:
                _currentMovementState = _bounceHeadState;
                _currentMovementState.EnterState(_animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.BounceHeadL:
                _currentMovementState = _fallHeadLState;
                _currentMovementState.EnterState(_animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.CatchSpiderL:
                _currentMovementState = _spiderPatrolState;
                _currentMovementState.EnterState(_animatedTransform.position, 1, 1);
                break;
            
            case DragonflyStates.SpiderPreattackHeadTransitionStateL:
                _currentMovementState = _preAttackHeadState;
                _currentMovementState.EnterState(_visibleBodyTransform.position, 1, 1);
                break;
        }
    }

    private void Update()
    {
        // Start Enter to patrol
        if (Input.GetKey(KeyCode.A))
        {
            _currentMovementState = _enterToPatrolState;
            _currentMovementState.EnterState(_animatedTransform.position, 1, 1);
        }
        
        // Start Catch Spider
        if (Input.GetKey(KeyCode.S))
        {
            _currentMovementState = _catchSpiderState;
            _currentMovementState.EnterState(_animatedTransform.position, 1, 1);
        }
        // Start Spider patrol transition
        if (Input.GetKey(KeyCode.D))
        {
            _currentMovementState = _spiderPreAttackHeadTransitionState;
            _currentMovementState.EnterState(_visibleBodyTransform.position, 1, 1);
        }
        
        
        // Start Head attack
        if (Input.GetKey(KeyCode.Z))
        {
            if (_currentMovementState.State == DragonflyStates.PatrolL)
            {
                _currentMovementState = _preAttackHeadState;
                _currentMovementState.EnterState(_animatedTransform.position, 1, 1);    
            }
        }
        
        _currentMovementState.CheckForStateChange();
        _currentMovementState.ExecuteState(_animatedTransform.position);
        _currentDragonflyState = _currentMovementState.State;
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
        Debug.Log("Collision");
        
        if (_currentDragonflyState == DragonflyStates.AttackHeadL)
        {
            SwitchState();
        }
    }
}
