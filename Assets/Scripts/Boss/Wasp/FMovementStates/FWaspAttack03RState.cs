using System;
using UnityEngine;

public class FWaspAttack03RState : FWaspAnimBaseState
{
    public FWaspAttack03RState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public event Action OnStartedEvent;
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
        OnStartedEvent?.Invoke();
    }
}