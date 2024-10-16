using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class DragonflyPlayablesContainer
{
    private PlayableGraph _playableGraph;
    private Dictionary<DragonflyMovementState, AnimationClipPlayable> _animationClips;
    
    public DragonflyPlayablesContainer(PlayableGraph graph)
    {
        _playableGraph = graph;
        _animationClips = new Dictionary<DragonflyMovementState, AnimationClipPlayable>();
    }
    
    public void AddClip(DragonflyMovementState key, AnimationClip clip)
    {
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
        _animationClips.Add(key, clipPlayable);
    }
    
    public AnimationClipPlayable GetClip(DragonflyMovementState movementState)
    {
        return _animationClips[movementState];
    }
    
}
