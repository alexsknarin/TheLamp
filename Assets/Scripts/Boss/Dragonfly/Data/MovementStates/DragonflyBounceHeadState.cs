using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyBounceHeadState", menuName = "DragonflyStates/DragonflyBounceHeadState")]
public class DragonflyBounceHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.BounceHead;
    [SerializeField] private float _speed = 4.1f;
    [SerializeField] private float _duration = 0.15f;
    public override DragonflyMovementState State => _state;
    
    private Vector3 _attackDirection;
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _attackDirection = -_stateData.VisibleBodyTransform.position.normalized;
        _localTime = 0f;
        _phase = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        _stateData.VisibleBodyTransform.position += -_attackDirection * (_speed * Time.deltaTime);

        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1f)
        {
            _stateData.Owner.SwitchState();
        }
    }
}
