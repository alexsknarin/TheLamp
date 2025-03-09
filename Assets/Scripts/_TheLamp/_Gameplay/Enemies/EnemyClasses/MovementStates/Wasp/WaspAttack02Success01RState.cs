using System;
using UnityEngine;

public class WaspAttack02Success01RState : FWaspAnimBaseState
{
    public WaspAttack02Success01RState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
