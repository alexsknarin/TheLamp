using System;
using UnityEngine;

public class FWaspAttack01DeathRState : FWaspAnimBaseState
{
    public FWaspAttack01DeathRState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public event Action Ended;
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
    
    public override void OnExit()
    {
        Ended?.Invoke();
    }
}
