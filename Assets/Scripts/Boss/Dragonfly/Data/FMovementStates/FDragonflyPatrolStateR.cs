using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyPatrolStateR", menuName = "FDragonflyMovementStates/FDragonflyPatrolStateR")]
public class FDragonflyPatrolStateR : ScriptableObject, IState
{
    private readonly int _sideDirection = -1;
    
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _patrolTransform;
    private DragonflyPatrolRotator _patrolRotator;

    public void SetDependencies(Transform visibleBodyTransform, Transform patrolTransform, DragonflyPatrolRotator patrolRotator)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _patrolTransform = patrolTransform;
        _patrolRotator = patrolRotator;
    }
    
    public void OnEnter()
    {
        Vector3 currentPosition = _visibleBodyTransform.position;
        _patrolRotator.SetRotationPhase(currentPosition);
        _patrolRotator.Play(_sideDirection);
        
        _visibleBodyTransform.SetParent(_patrolTransform, false);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
        _patrolRotator.Stop();
    }
}
