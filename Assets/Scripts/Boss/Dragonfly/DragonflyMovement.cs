using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class DragonflyMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
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

    [Header("Patrol Rotation")] 
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private DragonflyPatrolRotator _spiderPpatrolRotator;
    
    [Header("Visible Body")]
    [SerializeField] private Transform _visibleBodyTransform;
    
    
    [Header("Movement Script States")]
    [SerializeField] private float _headPreattackSpeed = 1f;
    [SerializeField] private float _headPreattackDuration = 1f;
    [SerializeField] private float _headPreattackDeccelerationPower = 1f;
    [SerializeField] private float _headAttackSpeed = 1f;
    [SerializeField] private float _headAttackAcceleration = 1f;
    
    [Header("Head Fall")]
    [SerializeField] private Transform _headFallPointTransform;
    [SerializeField] private AnimationCurve _headFallRotateCurve;
    [SerializeField] private AnimationCurve _headFallFallDownCurve;
    [SerializeField] private float _headFallDuration = 2f;
    
    private DragonflyStates _prevDragonflyState;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;
    
    private float _preAttackLocalTime = 0;
    private float _bounceLocalTime = 0;
    private float _headFallLocalTime = 0;
    private Vector3 _attackDirection; 
    
    private bool _isHeadPreAttacking = false;
    private bool _isHeadAttacking = false;
    private bool _isHeadBouncing = false;
    private bool _isHeadFalling = false;

    private float _headFallStartPosY = 0;
    
    
    
    private float _headAttackAccelerationValue = 0;


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
        
        _currentDragonflyState = DragonflyStates.Idle;
        _collisionCatcher.DisableColliders();
        PlayStateClip(_currentDragonflyState);
    }

    
    private void PlayStateClip(DragonflyStates state)
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

    private void Update()
    {
        // Start Enter to patrol
        if (Input.GetKey(KeyCode.A))
        {
            _currentDragonflyState = DragonflyStates.EnterToPatrolL;
            PlayStateClip(_currentDragonflyState);
            ParentVisibleBodyTo(_animatedTransform);
        }
        
        // Start Catch Spider
        if (Input.GetKey(KeyCode.S))
        {
            _currentDragonflyState = DragonflyStates.CatchSpiderL;
            PlayStateClip(_currentDragonflyState);
            ParentVisibleBodyTo(_animatedTransform);
        }
        
        // Start Head attack
        if (Input.GetKey(KeyCode.Z))
        {
            _currentDragonflyState = DragonflyStates.PreAttackHeadL;
            UnParentVisibleBodyKeepPos();
            _attackDirection = -_visibleBodyTransform.position.normalized;
            _isHeadPreAttacking = true;
        }
        
        
        // Movement states:
        if (_isHeadPreAttacking)
        {
            HeadPreattackPlay(_visibleBodyTransform, _attackDirection);
        }
        
        if (_isHeadAttacking)
        {
            HeadAttackPlay(_visibleBodyTransform, _attackDirection);
        }
        
        if (_isHeadBouncing)
        {
            HeadBouncePlay(_visibleBodyTransform, _attackDirection);
        }
        
        if (_isHeadFalling)
        {
            HeadFallPlay(_visibleBodyTransform, _attackDirection);
        }
        
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
        switch (_currentDragonflyState)
        {   
            case DragonflyStates.EnterToPatrolL:
                _patrolRotator.SetRotationPhase(_animatedTransform.position);
                _patrolRotator.Play();
                ParentVisibleBodyTo(_patrolTransform);
                _currentDragonflyState = DragonflyStates.PatrolL;
                break;
            case DragonflyStates.CatchSpiderL:
                _spiderPpatrolRotator.SetRotationPhase(_animatedTransform.position);
                _spiderPpatrolRotator.Play();
                ParentVisibleBodyTo(_spiderPatrolTransform);
                _currentDragonflyState = DragonflyStates.SpiderPatrolL;
                break;
        }
        PlayStateClip(DragonflyStates.Idle);
    }
    
    private void ParentVisibleBodyTo(Transform parent)
    {
        _visibleBodyTransform.SetParent(parent);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
    }
    
    private void UnParentVisibleBodyKeepPos()
    {
        _visibleBodyTransform.SetParent(null);
    }
    
    
    // Script Movement States
    private void HeadPreattackPlay(Transform bodyTransform, Vector3 direction)
    {
        float phase = _preAttackLocalTime / _headPreattackDuration;
        if (phase > 1)
        {
            // Switch to Head Attack
            _currentDragonflyState = DragonflyStates.AttackHeadL;
            _headAttackAccelerationValue = 0;
            
            _isHeadPreAttacking = false;
            _isHeadAttacking = true;
            _isHeadBouncing = false;
            _isHeadFalling = false;

            _collisionCatcher.EnableColliders();
            return;
        }
        // Move away from the target
        
        float decceleration = Mathf.Pow(1-phase, _headPreattackDeccelerationPower);
        bodyTransform.position += -direction * (_headPreattackSpeed * decceleration * Time.deltaTime);

        _preAttackLocalTime += Time.deltaTime;
        
    }
    
    private void HeadAttackPlay(Transform bodyTransform, Vector3 direction)
    {
        // Move towards the target
        bodyTransform.position += direction * (_headPreattackSpeed * Time.deltaTime + _headAttackAccelerationValue);
        _headAttackAccelerationValue += _headAttackAcceleration * Time.deltaTime;
    }
    
    private void HeadBouncePlay(Transform bodyTransform, Vector3 direction)
    {
        // Assume bounce duration is 0.5 seconds
        float phase = _bounceLocalTime / 0.15f;
        if (phase > 1)
        {
            // Switch to Fall
            _isHeadPreAttacking = false;
            _isHeadAttacking = false;
            _isHeadBouncing = false;
            _isHeadFalling = true;
            _currentDragonflyState = DragonflyStates.FallHeadL;
            _headFallLocalTime = 0;
            
            _headFallPointTransform.position = bodyTransform.position;
            _headFallPointTransform.rotation = bodyTransform.rotation;
            bodyTransform.SetParent(_headFallPointTransform);
            
            _headFallStartPosY = _headFallPointTransform.position.y;
            
            
            return;
        }
        // Move away from the target
        
        bodyTransform.position += -direction * (_headPreattackSpeed * Time.deltaTime);
        _bounceLocalTime += Time.deltaTime;
    }

    private void HeadFallPlay(Transform bodyTransform, Vector3 direction)
    {
        float phase = _headFallLocalTime / _headFallDuration;
        if (phase > 1)
        {
            _isHeadPreAttacking = false;
            _isHeadAttacking = false;
            _isHeadBouncing = false;
            _isHeadFalling = false;
            
            _currentDragonflyState = DragonflyStates.Idle;
            return;
        }
        
        Vector3 fallEuler = Vector3.zero;
        fallEuler.x = _headFallRotateCurve.Evaluate(phase);
        _visibleBodyTransform.localEulerAngles = fallEuler;
        
        Vector3 fallPos = _headFallPointTransform.position;
        fallPos.y = _headFallStartPosY + _headFallFallDownCurve.Evaluate(phase);
        _headFallPointTransform.position = fallPos;
        
        _headFallLocalTime += Time.deltaTime;
    }

    private void OnCollision()
    {
        if (_currentDragonflyState == DragonflyStates.AttackHeadL)
        {
            _isHeadPreAttacking = false;
            _isHeadAttacking = false;
            _isHeadBouncing = true;
            _isHeadFalling = false;

            _collisionCatcher.DisableColliders();
            _currentDragonflyState = DragonflyStates.BounceHeadL;
            _bounceLocalTime = 0;
        }
    }
}
