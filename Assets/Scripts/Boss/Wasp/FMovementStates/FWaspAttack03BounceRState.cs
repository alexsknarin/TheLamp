using UnityEngine;

public class FWaspAttack03BounceRState : FWaspAnimBaseState
{
    public FWaspAttack03BounceRState(Animator animator, int clipHash, Transform baseTransform) : 
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
