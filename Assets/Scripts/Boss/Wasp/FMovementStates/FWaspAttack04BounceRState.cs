using UnityEngine;

public class FWaspAttack04BounceRState : FWaspAnimBaseState
{
    public FWaspAttack04BounceRState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}