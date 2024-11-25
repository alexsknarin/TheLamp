using System;
using UnityEngine;

public class FWaspAttack01Success02LState : FWaspAnimBaseState
{
    public FWaspAttack01Success02LState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
}
