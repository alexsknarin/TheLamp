using UnityEngine;

public class FWaspAttack02BounceRState : FWaspAnimBaseState
{
    public FWaspAttack02BounceRState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
