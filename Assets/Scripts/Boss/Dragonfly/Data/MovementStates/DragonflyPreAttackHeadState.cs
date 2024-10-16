using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyPreAttackHeadState", menuName = "DragonflyStates/DragonflyPreAttackHeadState")]
public class DragonflyPreAttackHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.PreAttackHeadL;
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private float _deccelerationPower = 2f;
    [SerializeField] private float _sideSpeed = 0.5f;
    public override DragonflyMovementState State => _state;
    
    private Vector3 _attackDirection;
    private float _sideDirection;
    private float _localTime = 0f;
    private float _phase = 0f;
    private Quaternion _startRotation;
    private Quaternion _endRotation;
    
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if(depthDirection == 1)
        {
            _state = DragonflyMovementState.PreAttackHeadL;
        }
        else
        {
            _state = DragonflyMovementState.PreAttackHeadR;
        }
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _attackDirection = -_stateData.VisibleBodyTransform.position.normalized;
        _localTime = 0f;
        _phase = 0f;
        _sideDirection = sideDirection;
        
        _startRotation = _stateData.VisibleBodyTransform.rotation;
        _endRotation = Quaternion.LookRotation(_attackDirection, Vector3.up);
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        // Attack direction movement
        float decceleration = Mathf.Pow(1-_phase, _deccelerationPower);
        _stateData.VisibleBodyTransform.position += -_attackDirection * (_speed * decceleration * Time.deltaTime);

        // Side direction movement
        Vector3 sideVector = _stateData.VisibleBodyTransform.right.normalized;
        _stateData.VisibleBodyTransform.position += sideVector * (_sideSpeed * -_sideDirection * Time.deltaTime);
        
        // Rotate
        _stateData.VisibleBodyTransform.rotation = Quaternion.Slerp(_startRotation, _endRotation, _phase);
        
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
