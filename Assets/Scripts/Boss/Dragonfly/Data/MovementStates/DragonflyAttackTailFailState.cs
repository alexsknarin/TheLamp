using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAttackTailFailState", menuName = "DragonflyStates/DragonflyAttackTailFailState")]
public class DragonflyAttackTailFailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.AttackTailFailL;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _afterDelay = 1f;
    [SerializeField] private float _fallSpeed = 100f;
    [SerializeField] private float _rotationSpeed = 380f;
    [SerializeField] private float _moveAcceleration = 1.9f;
    public override DragonflyMovementState State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    private int _sideDirection = 0;
    private bool _isAfterDelay = false;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            _state = DragonflyMovementState.AttackTailFailL;
        }
        else
        {
            _state = DragonflyMovementState.AttackTailFailR;
        }
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _localTime = 0f;
        _phase = 0f;
        _sideDirection = sideDirection;
        _isAfterDelay = false;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        if (!_isAfterDelay)
        {
            Vector3 position = currentPosition;
            float fallSpeed = _fallSpeed * Mathf.Pow(_phase, _moveAcceleration);
            position += Vector3.down * (fallSpeed * Time.deltaTime);
            _stateData.VisibleBodyTransform.localPosition = position;
        
        
            Vector3 rotation = _stateData.VisibleBodyTransform.localEulerAngles;
            rotation.y += _rotationSpeed * Time.deltaTime * _sideDirection;
            _stateData.VisibleBodyTransform.localEulerAngles = rotation;    
        }
        
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        if (!_isAfterDelay)
        {
            _phase = _localTime / _duration;
            if (_phase > 1)
            {
                _isAfterDelay = true;
                _localTime = 0f;
            }    
        }
        else if (_isAfterDelay && _localTime > _afterDelay)
        {
            _isAfterDelay = false;
            _stateData.Owner.SwitchState();
        }
    }
}
