using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class WaspPlayablesContainer
{
    private PlayableGraph _playableGraph;
    private Dictionary<WaspStates, AnimationClipPlayable> _animationClips;
    private Dictionary<WaspStates, int> _sideDirections;

    public WaspPlayablesContainer(PlayableGraph graph)
    {
        _playableGraph = graph;
        _animationClips = new Dictionary<WaspStates, AnimationClipPlayable>();
        _sideDirections = new Dictionary<WaspStates, int>();
    }
    
    public void AddClip(WaspStates keyL, WaspStates keyR, AnimationClip clip)
    {
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
        _animationClips.Add(keyL, clipPlayable);
        _sideDirections.Add(keyL, 1);
        _animationClips.Add(keyR, clipPlayable);
        _sideDirections.Add(keyR, -1);
          
    }
    
    public AnimationClipPlayable GetClip(WaspStates state)
    {
        return _animationClips[state];
    }
    
    public int GetSideDirection(WaspStates state)
    {
        return _sideDirections[state];
    }
    
}
