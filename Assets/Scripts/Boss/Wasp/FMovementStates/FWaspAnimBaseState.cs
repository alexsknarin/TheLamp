using UnityEngine;

public class FWaspAnimBaseState : IState
{
    // Dependencies
    protected Animator _animator;
    protected int _clipHash;
    protected Transform _baseTransform;
    protected Vector3 _baseScaleL = Vector3.one; // TODO: Global static variables
    protected Vector3 _baseScaleR;
    
    public FWaspAnimBaseState(Animator animator, int clipHash, Transform baseTransform)
    {
        _animator = animator;
        _clipHash = clipHash;
        _baseTransform = baseTransform;
        _baseScaleR = _baseScaleL;
        _baseScaleR.x = -1;
    }

    public virtual void OnEnter()
    {
    }

    public void Tick()
    {
    }

    public virtual void OnExit()
    {
    }
}
