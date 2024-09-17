using UnityEngine;

public class DragonflySpiderPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private DragonflyStates _state = DragonflyStates.SpiderPatrolL;
    public override DragonflyStates State => _state;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _patrolRotator.SetRotationPhase(currentPosition);
        _patrolRotator.Play();
        
        _visibleBodyTransform.SetParent(_patrolTransform, false);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
    }
    
    public void ExitState()
    {
        _patrolRotator.Stop();
    }
}
