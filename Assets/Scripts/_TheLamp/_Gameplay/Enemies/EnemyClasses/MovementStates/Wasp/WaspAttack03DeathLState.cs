using System;
using UnityEngine;

public class WaspAttack03DeathLState : FWaspAnimBaseState
{
    public WaspAttack03DeathLState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public event Action Ended;
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
    
    public override void OnExit()
    {
        Ended?.Invoke();
    }
}
