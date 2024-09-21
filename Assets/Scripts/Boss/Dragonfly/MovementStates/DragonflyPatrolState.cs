using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyPatrolState", menuName = "DragonflyStates/DragonflyPatrolState")]
public class DragonflyPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.PatrolL;
    public override DragonflyStates State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.PatrolRotator.SetRotationPhase(currentPosition);
        _stateData.PatrolRotator.Play();
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.PatrolTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
    }

    public override void ExitState()
    {
        _stateData.PatrolRotator.Stop();
    }
}
