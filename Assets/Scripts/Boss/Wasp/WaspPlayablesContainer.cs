using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class WaspPlayablesContainer
{
    private PlayableGraph _playableGraph;
    private Dictionary<WaspState, AnimationClipPlayable> _animationClips;
    private Dictionary<WaspState, int> _sideDirections;

    public WaspPlayablesContainer(PlayableGraph graph)
    {
        _playableGraph = graph;
        _animationClips = new Dictionary<WaspState, AnimationClipPlayable>();
        _sideDirections = new Dictionary<WaspState, int>();
    }
    
    public void AddClip(WaspState keyL, WaspState keyR, AnimationClip clip)
    {
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
        _animationClips.Add(keyL, clipPlayable);
        _sideDirections.Add(keyL, 1);
        _animationClips.Add(keyR, clipPlayable);
        _sideDirections.Add(keyR, -1);
          
    }
    
    public void AddSingleStateClip(WaspState key, AnimationClip clip)
    {
        var clipPlayable = AnimationClipPlayable.Create(_playableGraph, clip);
        _animationClips.Add(key, clipPlayable);
        _sideDirections.Add(key, 1);
    }
    
    public AnimationClipPlayable GetClip(WaspState state)
    {
        return _animationClips[state];
    }
    
    public int GetSideDirection(WaspState state)
    {
        return _sideDirections[state];
    }
    
}
