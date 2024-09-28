using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAttackTailSuccessState", menuName = "DragonflyStates/DragonflyAttackTailSuccessState")]
public class DragonflyAttackTailSuccessState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackTailSuccessL;
    [SerializeField] private float _duration = 1.8f;
    [SerializeField] private float _startSpeed = 55f;
    [SerializeField] private float _rotationSpeed = 110f;
    [SerializeField] private float _moveAcceleration = 1.9f;
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    private Vector3 _startDirection;
    private Vector3 _endDirection;
    private float _speed;
    
    Quaternion _startRotation;
    Quaternion _endRotation;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _startDirection = _stateData.VisibleBodyTransform.forward.normalized;
        _endDirection = _startDirection;
        _endDirection.y = 0;
        _endDirection.z = 0;
        _endDirection.Normalize();
        _startRotation = _stateData.VisibleBodyTransform.localRotation;
        _endRotation = Quaternion.LookRotation(_endDirection);
        _localTime = 0f;
        _phase = 0f;
        _speed = _startSpeed;
        
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        
        Vector3 dir = Vector3.Lerp(_startDirection, _endDirection, _phase);
        _stateData.VisibleBodyTransform.position += dir * (_speed * Time.deltaTime);
        
        Quaternion rot = Quaternion.Lerp(_startRotation, _endRotation, _phase);
        _stateData.VisibleBodyTransform.rotation = rot;
        
        _speed += _moveAcceleration * Time.deltaTime;
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1)
        {
            _stateData.Owner.SwitchState();
        }
    }
}
