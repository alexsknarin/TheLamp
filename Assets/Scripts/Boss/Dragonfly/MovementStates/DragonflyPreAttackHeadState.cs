using UnityEngine;

public class DragonflyPreAttackHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.PreAttackHeadL;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private float _deccelerationPower = 2f;
    public override DragonflyStates State => _state;
    
    private Vector3 _attackDirection;
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _visibleBodyTransform.SetParent(_owner.transform);
        _attackDirection = -_visibleBodyTransform.position.normalized;
        _localTime = 0f;
        _phase = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        float decceleration = Mathf.Pow(1-_phase, _deccelerationPower);
        _visibleBodyTransform.position += -_attackDirection * (_speed * decceleration * Time.deltaTime);

        _localTime += Time.deltaTime;
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
