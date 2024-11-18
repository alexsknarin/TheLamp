using UnityEngine;

public class FWaspAttack02Fail01RState : FWaspAnimBaseState
{
    public FWaspAttack02Fail01RState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleR;
        _animator.Play(_clipHash, -1, 0);
    }
}
