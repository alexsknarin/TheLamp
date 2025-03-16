using UnityEngine;

public class WaspAttack02BounceRState : FWaspAnimBaseState
{
    public WaspAttack02BounceRState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
