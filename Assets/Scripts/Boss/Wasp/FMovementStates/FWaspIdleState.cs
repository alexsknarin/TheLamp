using System;
using UnityEngine;

public class FWaspIdleState: FWaspAnimBaseState
{
    public FWaspIdleState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public event Action OnStartedEvent;

    public override void OnEnter()
    {
        _animator.Play(_clipHash, -1, 0);
        OnStartedEvent?.Invoke();
    }
}
