using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class DragonflyPlayablesContainer
{
    private PlayableGraph _playableGraph;
    private Dictionary<Type, AnimationClipPlayable> _animationClips;
    
    public DragonflyPlayablesContainer(PlayableGraph graph)
    {
        _playableGraph = graph;
        _animationClips = new Dictionary<Type, AnimationClipPlayable>();
    }
    
    public void AddClip(Type key, AnimationClip clip)
    {
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
        _animationClips.Add(key, clipPlayable);
    }
    
    public AnimationClipPlayable GetClip(Type movementState)
    {
        return _animationClips[movementState];
    }
    
}
