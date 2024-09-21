using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAttackHeadSuccessState", menuName = "DragonflyStates/DragonflyAttackHeadSuccessState")]
public class DragonflyAttackHeadSuccessState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackHeadSuccessL;
    [SerializeField] private float _duration = 0.85f;
    [SerializeField] private AnimationCurve _tyCurve;
    [SerializeField] private AnimationCurve _tzCurve;
    [SerializeField] private AnimationCurve _rxCurve;
    
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.FallPointTransform.position = _stateData.VisibleBodyTransform.position;
        _stateData.FallPointTransform.rotation = _stateData.VisibleBodyTransform.rotation;
        _stateData.VisibleBodyTransform.SetParent(_stateData.FallPointTransform);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        _stateData.VisibleBodyTransform.localScale = Vector3.one;
        
        _localTime = 0f;
        _phase = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 position = Vector3.zero;
        position.y = _tyCurve.Evaluate(_phase);
        position.z = _tzCurve.Evaluate(_phase);
        _stateData.VisibleBodyTransform.localPosition = position;
        
        Vector3 rotation = _stateData.VisibleBodyTransform.localEulerAngles;
        rotation.x = _rxCurve.Evaluate(_phase);
        _stateData.VisibleBodyTransform.localEulerAngles = rotation;

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
