using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyIdleState", menuName = "DragonflyStates/DragonflyIdleState")]
public class DragonflyIdleState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.Idle;
    public override DragonflyMovementState State => _state;
}
