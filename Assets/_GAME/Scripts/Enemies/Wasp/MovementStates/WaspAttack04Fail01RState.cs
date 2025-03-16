using System;
using UnityEngine;

public class WaspAttack04Fail01RState : FWaspAnimBaseState
{
    public WaspAttack04Fail01RState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}