using System;
using UnityEngine;

public class WaspAnimationEventsListener : MonoBehaviour
{
    public event Action ClipEnded;
    public event Action SpreadTgiggered;
    public event Action ColliderEnabled;
    public event Action ColliderDisabled;
    public event Action AttackStarted;
    public event Action TrailReset;
    
    public void AnimationClipEnded()
    {
        ClipEnded?.Invoke();
    }
    
    public void TriggerSpread()
    {
        SpreadTgiggered?.Invoke();
    }
    
    public void EnableCollider()
    {
        ColliderEnabled?.Invoke();
    }

    public void DisableCollider()
    {
        ColliderDisabled?.Invoke();
    }
    
    public void ResetTrail()
    {
        TrailReset?.Invoke();
    }
    
    public void StartAttack()
    {
        AttackStarted?.Invoke();
    }
}
