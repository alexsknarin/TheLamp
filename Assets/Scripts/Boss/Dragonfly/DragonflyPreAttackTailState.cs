using UnityEngine;

public class DragonflyPreAttackTailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovement _owner;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private DragonflyStates _state = DragonflyStates.PreAttackTailL;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private float _duration = 0.4f;
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _visibleBodyTransform.SetParent(_owner.transform);
        _localTime = 0f;
        _phase = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1f)
        {
            _owner.SwitchState();
        }
    }
}
