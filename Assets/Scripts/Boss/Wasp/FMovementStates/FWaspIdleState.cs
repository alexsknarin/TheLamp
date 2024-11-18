using UnityEngine;

public class FWaspIdleState: FWaspAnimBaseState
{
    public FWaspIdleState(Animator animator, int clipHash, Transform baseTransform) : base(animator, clipHash, baseTransform)
    {
    }

    public override void OnEnter()
    {
        _animator.Play(_clipHash, -1, 0);
    }
}
