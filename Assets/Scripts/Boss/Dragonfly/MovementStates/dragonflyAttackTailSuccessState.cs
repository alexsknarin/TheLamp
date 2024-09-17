using UnityEngine;

public class dragonflyAttackTailSuccessState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackTailSuccessL;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _sideSpeed;
    [SerializeField] private float _frontSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _moveAcceleration;
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    
    


    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _visibleBodyTransform.SetParent(_owner.transform);
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 position = _visibleBodyTransform.position;
        position += _visibleBodyTransform.forward * (_frontSpeed * Mathf.Pow(_phase, _moveAcceleration) * Time.deltaTime);
        position += _visibleBodyTransform.right * (_sideSpeed * Time.deltaTime);
        _visibleBodyTransform.position = position;
        
        
        Vector3 rotation = _visibleBodyTransform.localEulerAngles;
        rotation.y += _rotationSpeed * (1 - _phase) * Time.deltaTime;
        _visibleBodyTransform.localEulerAngles = rotation;
        
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1)
        {
            Debug.Break();
            _owner.SwitchState();
        }
    }
}
