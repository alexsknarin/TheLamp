using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "DragonflyEnterToPatrolState", menuName = "DragonflyStates/DragonflyEnterToPatrolState")]
public class DragonflyEnterToPatrolState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.EnterToPatrolL;
    public override DragonflyStates State => _state;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.AnimatedTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        _stateData.Owner.PlayClip(_state);
    }
}
