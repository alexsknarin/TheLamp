using UnityEngine;

public class FWaspAttack01BounceRState : FWaspAnimBaseState
{
    public FWaspAttack01BounceRState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
