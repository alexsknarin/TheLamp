using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyPatrolState", menuName = "DragonflyStates/DragonflyPatrolState")]
public class DragonflyPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.PatrolL;
    public override DragonflyMovementState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if(sideDirection == 1)
        {
            _state = DragonflyMovementState.PatrolL;
        }
        else
        {
            _state = DragonflyMovementState.PatrolR;
        }
        
        _stateData.PatrolRotator.SetRotationPhase(currentPosition);
        _stateData.PatrolRotator.Play(sideDirection);
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.PatrolTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
    }

    public override void ExitState()
    {
        _stateData.PatrolRotator.Stop();
    }
}
