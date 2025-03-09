using UnityEngine;

public class WaspAttack01BounceLState : FWaspAnimBaseState
{
    public WaspAttack01BounceLState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
}
