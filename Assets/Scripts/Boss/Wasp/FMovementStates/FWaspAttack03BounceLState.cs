using UnityEngine;

public class FWaspAttack03BounceLState : FWaspAnimBaseState
{
    public FWaspAttack03BounceLState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
}
