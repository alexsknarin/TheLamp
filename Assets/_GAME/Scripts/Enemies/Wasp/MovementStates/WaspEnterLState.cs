using UnityEngine;

public class WaspEnterLState : FWaspAnimBaseState
{
    public WaspEnterLState(Animator animator, int clipHash, Transform baseTransform) :
        base(animator, clipHash, baseTransform) { }
    
    public override void OnEnter()
    {
        _baseTransform.localScale = _baseScaleL;
        _animator.Play(_clipHash, -1, 0);
    }
}
