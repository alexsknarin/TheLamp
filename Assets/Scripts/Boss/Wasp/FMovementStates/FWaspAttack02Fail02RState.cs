using System;
using UnityEngine;

public class FWaspAttack02Fail02RState : FWaspAnimBaseState
{
    public FWaspAttack02Fail02RState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
