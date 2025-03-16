using System;
using UnityEngine;

public class WaspAttack04DeathRState : FWaspAnimBaseState
{
    public WaspAttack04DeathRState(Animator animator, int clipHash, Transform baseTransform) : 
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