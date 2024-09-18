using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class DragonflyEnterToHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyAnimationClipEventHandler _animationClipEventHandler;
    [SerializeField] private DragonflyStates _state = DragonflyStates.EnterToHoverL;
    public override DragonflyStates State => _state;
    
    private AnimationClipPlayable _clipPlayable;
    private PlayableOutput _playableOutput;
    private PlayableGraph _playableGraph;
    private bool _isClipEnded = false;
    private void OnEnable()
    {
        _animationClipEventHandler.OnClipEndedEvent += OnClipEnded;
    }

    private void OnDisable()
    {
        _animationClipEventHandler.OnClipEndedEvent -= OnClipEnded;
    }
    
    private void OnClipEnded()
    {
        _isClipEnded = true;
    }

    public void Initialize(PlayableOutput playableOutput, PlayableGraph playableGraph, AnimationClipPlayable animClip)
    {
        _clipPlayable = animClip;
        _playableOutput = playableOutput;
        _playableGraph = playableGraph;
    }

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _visibleBodyTransform.SetParent(_animationClipEventHandler.transform, false);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
        
        _isClipEnded = false;
        _clipPlayable.SetTime(0);
        _clipPlayable.SetTime(0); // Unity Bug
        _playableOutput.SetSourcePlayable(_clipPlayable);
        if (_playableGraph.IsValid())
        {
            _playableGraph.Play();    
        }
    }

    public override void CheckForStateChange()
    {
        if (_isClipEnded)
        {
            _owner.SwitchState();
        }
    }
}
