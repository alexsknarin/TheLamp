using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyAttackTailFailState", menuName = "DragonflyStates/DragonflyAttackTailFailState")]
public class DragonflyAttackTailFailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.AttackTailFailL;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _fallSpeed = 100f;
    [SerializeField] private float _rotationSpeed = 380f;
    [SerializeField] private float _moveAcceleration = 1.9f;
    public override DragonflyState State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    private int _sideDirection = 0;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            _state = DragonflyState.AttackTailFailL;
        }
        else
        {
            _state = DragonflyState.AttackTailFailR;
        }
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _localTime = 0f;
        _phase = 0f;
        _sideDirection = sideDirection;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 position = currentPosition;
        float fallSpeed = _fallSpeed * Mathf.Pow(_phase, _moveAcceleration);
        position += Vector3.down * (fallSpeed * Time.deltaTime);
        _stateData.VisibleBodyTransform.localPosition = position;
        
        
        Vector3 rotation = _stateData.VisibleBodyTransform.localEulerAngles;
        rotation.y += _rotationSpeed * Time.deltaTime * _sideDirection;
        _stateData.VisibleBodyTransform.localEulerAngles = rotation;
        
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
