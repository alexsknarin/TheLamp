using UnityEngine;

public class FWaspAttack04Fail01LState : FWaspAnimBaseState
{
    public FWaspAttack04Fail01LState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
}