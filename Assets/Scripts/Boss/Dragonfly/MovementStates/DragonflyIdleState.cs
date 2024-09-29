using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyIdleState", menuName = "DragonflyStates/DragonflyIdleState")]
public class DragonflyIdleState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.Idle;
    public override DragonflyState State => _state;
}
