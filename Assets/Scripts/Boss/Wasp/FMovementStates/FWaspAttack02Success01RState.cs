using UnityEngine;

public class FWaspAttack02Success01RState : FWaspAnimBaseState
{
    public FWaspAttack02Success01RState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
