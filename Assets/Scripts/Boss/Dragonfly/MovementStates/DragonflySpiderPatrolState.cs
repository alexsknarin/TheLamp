using UnityEngine;

[CreateAssetMenu(fileName = "DragonflySpiderPatrolState", menuName = "DragonflyStates/DragonflySpiderPatrolState")]
public class DragonflySpiderPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.SpiderPatrolL;
    public override DragonflyStates State => _state;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.SpiderPatrolRotator.SetRotationPhase(currentPosition);
        _stateData.SpiderPatrolRotator.Play(sideDirection);
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.SpiderPatrolTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
    }
    
    public override void ExitState()
    {
        _stateData.SpiderPatrolRotator.Stop();
    }
}
