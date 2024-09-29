using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyEnterToPatrolState", menuName = "DragonflyStates/DragonflyEnterToPatrolState")]
public class DragonflyEnterToPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.EnterToPatrolL;
    public override DragonflyState State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        if (sideDirection == 1)
        {
            _state = DragonflyState.EnterToPatrolL;
        }
        else
        {
            _state = DragonflyState.EnterToPatrolR;
        }
        
        _stateData.Owner.PlayClip(_state);
    }
}
