using System;
using UnityEngine;

public class DragonflyAnimationClipEventHandler : MonoBehaviour
{
    public event Action OnClipEndedEvent;
    
    public void ClipEnded()
    {
        Debug.Log("ClipEnd");
        OnClipEndedEvent?.Invoke();
    }
}
