using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyPatrolStateL", menuName = "FDragonflyMovementStates/FDragonflyPatrolStateL")]
public class FDragonflyPatrolStateL : ScriptableObject, IState
{
    private readonly int _sideDirection = 1;
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _patrolTransform;
    private DragonflyPatrolRotator _patrolRotator;
    
    public event Action Started;

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
        Started?.Invoke();
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
        _patrolRotator.Stop();
    }
}
