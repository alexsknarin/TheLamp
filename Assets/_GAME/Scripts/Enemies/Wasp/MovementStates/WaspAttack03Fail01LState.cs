using System;
using UnityEngine;

public class WaspAttack03Fail01LState : FWaspAnimBaseState
{
    public WaspAttack03Fail01LState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
}
