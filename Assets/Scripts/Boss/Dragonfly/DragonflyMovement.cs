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
    [SerializeField] private DragonflyCollisionCatcher _collisionCatcher;
    [SerializeField] private DragonflyStates _currentDragonflyState;
    [SerializeField] private DragonflyAnimationClipEventHandler _animationClipEvents;
    [SerializeField] private Transform _fallPoint;
    
    [Header("Animation Clips")]
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _enterToPatrolLClip;
    [SerializeField] private AnimationClip _catchSpiderLClip;
    [SerializeField] private AnimationClip _enterToHoverLClip;

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

    
    private DragonflyMovementStateData _stateData;
    
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


    private void Awake()
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
        // 

        _currentMovementState = _idleState;
        
        _currentDragonflyState = DragonflyStates.Idle;
        _collisionCatcher.DisableColliders();
    }

    public void PlayClip(DragonflyStates state)
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
            case DragonflyStates.PreAttackHover:
                _collisionCatcher.EnableColliders();
                _isCollided = false;
                SwitchState(_attackHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.AttackHover:
                SwitchState(_bounceHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.BounceHover:
                // SwitchState(_fallHeadLState, _visibleBodyTransform.position, 1, 1);
                SwitchState(_returnHoverState, _visibleBodyTransform.position, 1, 1);
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
            case DragonflyStates.MoveToHover:
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
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchState(_moveToHoverState, _visibleBodyTransform.position, 1, 1);
        }
        
        
        // Start Head attack
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_currentMovementState.State == DragonflyStates.PatrolL)
            {
                SwitchState(_preAttackHeadState, _animatedTransform.position, 1, 1);
            }
            if (_currentMovementState.State == DragonflyStates.Hover)
            {
                SwitchState(_preAttackHoverState, _animatedTransform.position, 1, 1);
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
        SwitchState();
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
            if (_currentDragonflyState == DragonflyStates.AttackHover)
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
