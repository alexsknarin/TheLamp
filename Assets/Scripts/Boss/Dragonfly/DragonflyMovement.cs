using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class DragonflyMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _animatedTransform;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private DragonflyStates _currentDragonflyState;
    [SerializeField] private DragonflyAnimationClipEventHandler _animationClipEvents;
    
    [Header("Animation Clips")]
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _enterToPatrolLClip;
    [SerializeField] private AnimationClip _catchSpiderLClip;

    [Header("Patrol Rotation")] 
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    
    [Header("Visible Body")]
    [SerializeField] private Transform _visibleBodyTransform;
    
    
    private DragonflyStates _prevDragonflyState;
    private AnimationClipPlayable _currentAnimationClipPlayable;
    private DragonflyPlayablesContainer _playablesContainer;
    
    private PlayableGraph _playableGraph;
    private PlayableOutput _playableOutput;

    private Transform _currentTransform;

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
        
        PlayStateClip(DragonflyStates.Idle);
        _currentTransform = _animatedTransform;
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
            PlayStateClip(DragonflyStates.EnterToPatrolL);
            _currentTransform = _animatedTransform;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            PlayStateClip(DragonflyStates.CatchSpiderL);
            _currentTransform = _animatedTransform;
        }
        
        _visibleBodyTransform.position = _currentTransform.position;
        _visibleBodyTransform.rotation = _currentTransform.rotation;
        
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
        PlayStateClip(DragonflyStates.Idle);
        
        _patrolRotator.SetRotationPhase(_animatedTransform.position);
        _patrolRotator.Play();
        _currentTransform = _patrolTransform;
    }
}
