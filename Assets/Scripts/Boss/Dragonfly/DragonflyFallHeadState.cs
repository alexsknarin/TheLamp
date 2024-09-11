using UnityEngine;

public class DragonflyFallHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovement _owner;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private DragonflyStates _state = DragonflyStates.FallHeadL;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private Transform _headFallPointTransform;
    [SerializeField] private AnimationCurve _headFallRotateCurve;
    [SerializeField] private AnimationCurve _headFallFallDownCurve;

    public override DragonflyStates State => _state;
    
    private float _headFallStartPosY = 0f;
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _headFallPointTransform.position = _visibleBodyTransform.position;
        _headFallPointTransform.rotation = _visibleBodyTransform.rotation;
        _visibleBodyTransform.SetParent(_headFallPointTransform);
        _headFallStartPosY = _headFallPointTransform.position.y;
        
        _localTime = 0f;
        _phase = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 fallEuler = Vector3.zero;
        fallEuler.x = _headFallRotateCurve.Evaluate(_phase);
        _visibleBodyTransform.localEulerAngles = fallEuler;
        
        Vector3 fallPos = _headFallPointTransform.position;
        fallPos.y = _headFallStartPosY + _headFallFallDownCurve.Evaluate(_phase);
        _headFallPointTransform.position = fallPos;
        
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
