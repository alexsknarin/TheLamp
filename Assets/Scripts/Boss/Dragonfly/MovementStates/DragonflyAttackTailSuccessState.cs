using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAttackTailSuccessState", menuName = "DragonflyStates/DragonflyAttackTailSuccessState")]
public class DragonflyAttackTailSuccessState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackTailSuccessL;
    [SerializeField] private float _duration = 1.8f;
    [SerializeField] private float _sideSpeed = 1f;
    [SerializeField] private float _frontSpeed = 55f;
    [SerializeField] private float _rotationSpeed = 110f;
    [SerializeField] private float _moveAcceleration = 1.9f;
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 position = _stateData.VisibleBodyTransform.position;
        position += _stateData.VisibleBodyTransform.forward * (_frontSpeed * Mathf.Pow(_phase, _moveAcceleration) * Time.deltaTime);
        position += _stateData.VisibleBodyTransform.right * (_sideSpeed * Time.deltaTime);
        _stateData.VisibleBodyTransform.position = position;
        
        
        Vector3 rotation = _stateData.VisibleBodyTransform.localEulerAngles;
        rotation.y += _rotationSpeed * (1 - _phase) * Time.deltaTime;
        _stateData.VisibleBodyTransform.localEulerAngles = rotation;
        
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1)
        {
            Debug.Break();
            _stateData.Owner.SwitchState();
        }
    }
}
