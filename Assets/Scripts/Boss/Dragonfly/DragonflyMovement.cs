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
    
    
    private DragonflyStates _prevDragonflyState;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;


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
        if (Input.GetKey(KeyCode.A))
        {
            _currentDragonflyState = DragonflyStates.EnterToPatrolL;
            PlayStateClip(_currentDragonflyState);
            ParentVisibleBodyTo(_animatedTransform);
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            _currentDragonflyState = DragonflyStates.CatchSpiderL;
            PlayStateClip(_currentDragonflyState);
            ParentVisibleBodyTo(_animatedTransform);
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
                break;
            case DragonflyStates.CatchSpiderL:
                _spiderPpatrolRotator.SetRotationPhase(_animatedTransform.position);
                _spiderPpatrolRotator.Play();
                ParentVisibleBodyTo(_spiderPatrolTransform);
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
    
    private void UnParentVisibleBody()
    {
        _visibleBodyTransform.SetParent(null);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
    }
}
