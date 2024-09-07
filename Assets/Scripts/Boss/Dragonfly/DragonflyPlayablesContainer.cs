using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class DragonflyPlayablesContainer
{
    private PlayableGraph _playableGraph;
    private Dictionary<DragonflyStates, AnimationClipPlayable> _animationClips;
    
    public DragonflyPlayablesContainer(PlayableGraph graph)
    {
        _playableGraph = graph;
        _animationClips = new Dictionary<DragonflyStates, AnimationClipPlayable>();
    }
    
    public void AddClip(DragonflyStates key, AnimationClip clip)
    {
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
        _animationClips.Add(key, clipPlayable);
    }
    
    public AnimationClipPlayable GetClip(DragonflyStates state)
    {
        return _animationClips[state];
    }
    
}
