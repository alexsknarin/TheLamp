using System;
using UnityEngine;

public class FWaspAttack01Success02RState : FWaspAttack01RState
{
    public FWaspAttack01Success02RState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public event Action OnEndedEvent;
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
    
    public override void OnExit()
    {
        OnEndedEvent?.Invoke();
    }
}
