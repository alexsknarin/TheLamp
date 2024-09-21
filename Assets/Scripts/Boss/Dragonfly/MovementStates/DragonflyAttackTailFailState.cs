using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAttackTailFailState", menuName = "DragonflyStates/DragonflyAttackTailFailState")]
public class DragonflyAttackTailFailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackTailFailL;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _fallSpeed = 100f;
    [SerializeField] private float _rotationSpeed = 380f;
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
        position += Vector3.down * (_fallSpeed * Mathf.Pow(_phase, _moveAcceleration) * Time.deltaTime);
        _stateData.VisibleBodyTransform.position = position;
        
        
        Vector3 rotation = _stateData.VisibleBodyTransform.localEulerAngles;
        rotation.y += _rotationSpeed * Time.deltaTime;
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
