using UnityEngine;

public class dragonflyAttackHeadSuccessState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackHeadSuccessL;
    [SerializeField] private Transform _headFallPointTransform;
    [SerializeField] private float _duration = 2.0f;
    [SerializeField] private AnimationCurve _tyCurve;
    [SerializeField] private AnimationCurve _tzCurve;
    [SerializeField] private AnimationCurve _rxCurve;
    
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _headFallPointTransform.position = _visibleBodyTransform.position;
        _headFallPointTransform.rotation = _visibleBodyTransform.rotation;
        _visibleBodyTransform.SetParent(_headFallPointTransform);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
        _visibleBodyTransform.localScale = Vector3.one;
        
        _localTime = 0f;
        _phase = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 position = Vector3.zero;
        position.y = _tyCurve.Evaluate(_phase);
        position.z = _tzCurve.Evaluate(_phase);
        _visibleBodyTransform.localPosition = position;
        
        Vector3 rotation = _visibleBodyTransform.localEulerAngles;
        rotation.x = _rxCurve.Evaluate(_phase);
        _visibleBodyTransform.localEulerAngles = rotation;

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
