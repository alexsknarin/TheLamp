using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflySpiderPatrolState", menuName = "DragonflyStates/DragonflySpiderPatrolState")]
public class DragonflySpiderPatrolState : DragonflyMovementBaseState
{
    [FormerlySerializedAs("_state")] [SerializeField] private DragonflyState state = DragonflyState.SpiderPatrolL;
    public override DragonflyState State => state;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            state = DragonflyState.SpiderPatrolL;
        }
        else
        {
            state = DragonflyState.SpiderPatrolR;
        }
        
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
