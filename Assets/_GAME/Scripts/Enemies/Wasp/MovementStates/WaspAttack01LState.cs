using System;
using UnityEngine;

public class WaspAttack01LState : FWaspAnimBaseState
{
    public WaspAttack01LState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public event Action Started;
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
        Started?.Invoke();
    }
}
