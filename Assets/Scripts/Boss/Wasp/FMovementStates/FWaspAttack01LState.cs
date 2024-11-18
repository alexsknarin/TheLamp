using UnityEngine;

public class FWaspAttack01LState : FWaspAnimBaseState
{
    public FWaspAttack01LState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }
    
    public override void OnEnter()
    {
        Debug.Log("FWaspAttack01LState");
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
}
