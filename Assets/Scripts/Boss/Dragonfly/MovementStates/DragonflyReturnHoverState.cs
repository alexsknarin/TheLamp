using UnityEngine;

public class DragonflyReturnHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.ReturnHover;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float _verticalDistance = 1f;
    [SerializeField] private float _horizontalDistance = 1f;
    [SerializeField] private AnimationCurve _verticalMoveCurve;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    private float _startPosY = 0f;
    private float _endPosY = 0f;
    private Vector3 _direction = Vector3.zero;
    
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _endPos = Vector3.zero;
    public override DragonflyStates State => _state;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _localTime = 0f;
        _phase = 0f;
        
        _startPos = currentPosition;
        _endPosY = _startPos.y - _verticalDistance;
        
        Vector3 moveDirection = _endPos;
        moveDirection.y = 0;
        moveDirection.Normalize();
        _endPos = currentPosition + moveDirection * _horizontalDistance;
        
        _visibleBodyTransform.SetParent(_owner.transform);
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 position = Vector3.Lerp(_startPos, _endPos, _phase);
        position.y = Mathf.Lerp(_startPos.y, _endPosY, _verticalMoveCurve.Evaluate(_phase));
        
        
        _visibleBodyTransform.position = position;
        
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / duration;
        if (_phase > 1)
        {
            _owner.SwitchState();
        }
    }
}
