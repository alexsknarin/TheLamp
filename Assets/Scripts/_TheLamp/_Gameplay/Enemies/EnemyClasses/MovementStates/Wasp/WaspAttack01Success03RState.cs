using System;
using UnityEngine;

public class WaspAttack01Success03RState : FWaspAnimBaseState
{
    public WaspAttack01Success03RState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }   
}
