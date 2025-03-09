using System;
using UnityEngine;

public class WaspAttack01RState : FWaspAnimBaseState
{
    public WaspAttack01RState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public event Action Started;
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
        Started?.Invoke();
    }
}
