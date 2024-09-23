using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
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
    [SerializeField] private DragonflyCollisionCatcher _collisionCatcher;
    [SerializeField] private DragonflyStates _currentDragonflyState;
    [SerializeField] private DragonflyAnimationClipEventHandler _animationClipEvents;
    [SerializeField] private Transform _fallPoint;
    
    [Header("Animation Clips")]
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _enterToPatrolLClip;
    [SerializeField] private AnimationClip _enterToPatrolRClip;
    [SerializeField] private AnimationClip _catchSpiderLClip;
    [SerializeField] private AnimationClip _enterToHoverLClip;
    [SerializeField] private AnimationClip _enterToHoverRClip;
    [SerializeField] private AnimationClip _catchSpiderRClip;

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
    
    private int _sideDirection = 1;
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    // Add state switch variables
    
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
        _playablesContainer.AddClip(DragonflyStates.EnterToPatrolR, _enterToPatrolRClip);  
        _playablesContainer.AddClip(DragonflyStates.CatchSpiderL, _catchSpiderLClip);
        _playablesContainer.AddClip(DragonflyStates.EnterToHoverL, _enterToHoverLClip);
        _playablesContainer.AddClip(DragonflyStates.EnterToHoverR, _enterToHoverRClip);
        _playablesContainer.AddClip(DragonflyStates.CatchSpiderR, _catchSpiderRClip);
        
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

    private void Start()
    {
    }

    public void Play()
    {
        MovementSetup();
    }
    
    private void MovementSetup()
    {
        _sideDirection = RandomDirection.Generate();
        // Randomly chose way to enter
        int enterType = Random.Range(0, 2);
        if (enterType == 0)
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
        bool isHit = false;
        switch (_currentMovementState.State)
        {
            case DragonflyStates.EnterToPatrolL:
                SetState(_patrolState, _animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.EnterToPatrolR:
                SetState(_patrolState, _animatedTransform.position, -1, 1);
                break;
            //
            case DragonflyStates.PatrolL:
                SetState(_preAttackHeadState, _animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.PatrolR:
                SetState(_preAttackHeadState, _animatedTransform.position, -1, 1);
                break;
            //
            case DragonflyStates.PreAttackHeadL:
                _collisionCatcher.EnableColliders();
                _isCollided = false;
                SetState(_attackHeadState, _animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.PreAttackHeadR:
                _collisionCatcher.EnableColliders();
                _isCollided = false;
                SetState(_attackHeadState, _animatedTransform.position, 1, 1);
                break;
            //
            case DragonflyStates.AttackHead:
                _collisionCatcher.DisableColliders();
                SetState(_bounceHeadState, _animatedTransform.position, 1, 1);
                break;
            //
            case DragonflyStates.BounceHead:
                // TODO:
                isHit = Random.value <= 0.5f;
                if (isHit)
                {
                    SetState(_fallHeadLState, _visibleBodyTransform.position, 1, 1);
                }
                else
                {
                    SetState(_attackHeadSuccess, _visibleBodyTransform.position, 1, 1);
                }
                break;
            //
            case DragonflyStates.CatchSpiderL:
                SetState(_spiderPatrolState, _animatedTransform.position, 1, 1);
                break;
            case DragonflyStates.CatchSpiderR:
                SetState(_spiderPatrolState, _animatedTransform.position, -1, 1);
                break;
            // //
            case DragonflyStates.SpiderPreattackHeadTransitionStateL:
                SetState(_patrolState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.Hover:
                SetState(_preAttackHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.PreAttackHover:
                _collisionCatcher.EnableColliders();
                _isCollided = false;
                SetState(_attackHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.AttackHover:
                SetState(_bounceHoverState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.BounceHover:
                // TODO:
                isHit = Random.value <= 0.5f;
                if (isHit)
                {
                    SetState(_returnHoverState, _visibleBodyTransform.position, 1, 1);
                }
                else
                {
                    SetState(_fallHeadLState, _visibleBodyTransform.position, 1, 1);
                }
                break;
            //
            //
            case DragonflyStates.PreAttackTailL:
                _collisionCatcher.EnableColliders();
                _isCollided = false;
                SetState(_attackTailState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.PreAttackTailR:
                _collisionCatcher.EnableColliders();
                _isCollided = false;
                SetState(_attackTailState, _visibleBodyTransform.position, -1, 1);
                break;
            //
            case DragonflyStates.AttackTailL:
                _collisionCatcher.DisableColliders();
                SetState(_bounceTailState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.AttackTailR:
                _collisionCatcher.DisableColliders();
                SetState(_bounceTailState, _visibleBodyTransform.position, -1, 1);
                break;
            // 
            case DragonflyStates.BounceTailL:
                // TODO:    
                // isHit = Random.value <= 0.5f;
                isHit = true;
                if (isHit)
                {
                    SetState(_attackTailSuccess, _visibleBodyTransform.position, 1, 1);
                }
                else
                {
                    SetState(_attackTailFail, _visibleBodyTransform.position, 1, 1);
                }
                break;
            case DragonflyStates.BounceTailR:
                // isHit = Random.value <= 0.5f;
                isHit = true;
                if (isHit)
                {
                    SetState(_attackTailSuccess, _visibleBodyTransform.position, -1, 1);
                }
                else
                {
                    SetState(_attackTailFail, _visibleBodyTransform.position, -1, 1);
                }
                break;
            //
            case DragonflyStates.EnterToHoverL:
                SetState(_hoverState, _visibleBodyTransform.position, 1, 1);
                break;
            case DragonflyStates.EnterToHoverR:
                SetState(_hoverState, _visibleBodyTransform.position, -1, 1);
                break;
            
            case DragonflyStates.MoveToHover:
                SetState(_hoverState, _visibleBodyTransform.position, 1, 1);
                break;
            
        }
    }

    private void Update()
    {
        // Restart 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovementReset();
            MovementSetup();
        }
        
        
        // Start Enter to patrol
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetState(_enterToPatrolState, _animatedTransform.position, -1, 1);
        }
        
        // Start Catch Spider
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetState(_catchSpiderState, _animatedTransform.position, -1, 1);
        }
        // Start Spider patrol transition
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (_currentMovementState.State == DragonflyStates.SpiderPatrolL)
            {
                SetState(_spiderPreAttackHeadTransitionState, _visibleBodyTransform.position, 1, 1);
            }
            
            if (_currentMovementState.State == DragonflyStates.SpiderPatrolR)
            {
                SetState(_spiderPreAttackHeadTransitionState, _visibleBodyTransform.position, -1, 1);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_currentMovementState.State == DragonflyStates.PatrolL)
            {
                SetState(_preAttackTailState, _visibleBodyTransform.position, 1, 1);
            }
            if (_currentMovementState.State == DragonflyStates.PatrolR)
            {
                SetState(_preAttackTailState, _visibleBodyTransform.position, -1, 1);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetState(_enterToHoverState, _visibleBodyTransform.position, 1, 1);
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetState(_moveToHoverState, _visibleBodyTransform.position, 1, 1);
        }
        
        
        // Start Head attack
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (_currentMovementState.State == DragonflyStates.PatrolL)
            {
                SetState(_preAttackHeadState, _animatedTransform.position, 1, 1);
            }
            if (_currentMovementState.State == DragonflyStates.Hover)
            {
                SetState(_preAttackHoverState, _animatedTransform.position, 1, 1);
            }
        }
        
        _currentMovementState.CheckForStateChange();
        _currentMovementState.ExecuteState(_visibleBodyTransform.position);
        _currentDragonflyState = _currentMovementState.State;
    }
    
    private void SetState(DragonflyMovementBaseState toState, Vector3 position, int sideDirection, int depthDirection)
    {
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
        SwitchState();
    }
   
    private void OnCollision()
    {
        if (!_isCollided)
        {
            Debug.Log("Collision");
            if (_currentDragonflyState == DragonflyStates.AttackHead)
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
            if (_currentDragonflyState == DragonflyStates.AttackTailL || _currentDragonflyState == DragonflyStates.AttackTailR)
            {
                _collisionCatcher.DisableColliders();
                _isCollided = true;
                SwitchState();
            }    
        }
    }
}
