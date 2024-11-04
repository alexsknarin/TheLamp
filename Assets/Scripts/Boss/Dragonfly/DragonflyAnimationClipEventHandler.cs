using System;
using UnityEngine;

public class DragonflyAnimationClipEventHandler : MonoBehaviour
{
    public event Action OnClipEndedEvent;
    public event Action OnSwarmCallEvent;
    
    public void ClipEnded()
    {
        OnClipEndedEvent?.Invoke();
    }
    
    public void SwarmCall()
    {
        OnSwarmCallEvent?.Invoke();
    }
}
