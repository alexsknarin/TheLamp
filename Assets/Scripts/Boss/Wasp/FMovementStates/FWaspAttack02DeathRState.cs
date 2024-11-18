using UnityEngine;

public class FWaspAttack02DeathRState : FWaspAnimBaseState
{
    public FWaspAttack02DeathRState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
