using UnityEngine;

public class DragonflyIdleState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.Idle;
    public override DragonflyStates State => _state;
}
