using System;
using UnityEngine;

public class FWaspAttack02LState : FWaspAnimBaseState
{
    public FWaspAttack02LState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public event Action OnStartedEvent;
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
        OnStartedEvent?.Invoke();
    }
}

