using UnityEngine;

public abstract class FDragonflyAnimBaseState : ScriptableObject, IState
{
    // Dependencies
    protected Transform _visibleBodyTransform;
    protected Transform _animatedTransform;
    protected FDragonflyMovement _movement;
    
    public void SetDependencies(Transform visibleBodyTransform, Transform animatedTransform, FDragonflyMovement movement)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _animatedTransform = animatedTransform;
        _movement = movement;
    }
    
    protected void ParentVisibleBodyToAnimatedTransform()
    {
        _visibleBodyTransform.SetParent(_animatedTransform, false);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
    }

    public virtual void OnEnter()
    {
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
    }
}
