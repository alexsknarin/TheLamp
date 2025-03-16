using System;
using UnityEngine;

public class WaspAttack01Success02LState : FWaspAnimBaseState
{
    public WaspAttack01Success02LState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
}
