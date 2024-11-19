using System;
using UnityEngine;

public class FWaspAttack03Fail01LState : FWaspAnimBaseState
{
    public FWaspAttack03Fail01LState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public event Action OnEndedEvent;
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
    
    public override void OnExit()
    {
        OnEndedEvent?.Invoke();
    }
}
