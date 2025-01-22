using System;
using UnityEngine;

public class DragonflyAnimationClipEventHandler : MonoBehaviour
{
    public event Action AnimClipEnded;
    public event Action SwarmCalled;
    
    public void ClipEnded()
    {
        AnimClipEnded?.Invoke();
    }
    
    public void SwarmCall()
    {
        SwarmCalled?.Invoke();
    }
}
