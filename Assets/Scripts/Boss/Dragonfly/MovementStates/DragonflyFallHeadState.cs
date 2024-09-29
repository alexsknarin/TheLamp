using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyFallHeadState", menuName = "DragonflyStates/DragonflyFallHeadState")]
public class DragonflyFallHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.FallHead;
    [SerializeField] private float _duration = 1.1f;
    [SerializeField] private AnimationCurve _headFallRotateCurve;
    [SerializeField] private AnimationCurve _headFallFallDownCurve;

    public override DragonflyState State => _state;
    
    private float _headFallStartPosY = 0f;
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.FallPointTransform.position = _stateData.VisibleBodyTransform.position;
        _stateData.FallPointTransform.rotation = _stateData.VisibleBodyTransform.rotation;
        _stateData.VisibleBodyTransform.SetParent(_stateData.FallPointTransform);
        _headFallStartPosY = _stateData.FallPointTransform.position.y;
        
        _localTime = 0f;
        _phase = 0f;

    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 fallEuler = Vector3.zero;
        fallEuler.x = _headFallRotateCurve.Evaluate(_phase);
        _stateData.VisibleBodyTransform.localEulerAngles = fallEuler;
        
        Vector3 fallPos = _stateData.FallPointTransform.position;
        fallPos.y = _headFallStartPosY + _headFallFallDownCurve.Evaluate(_phase);
        _stateData.FallPointTransform.position = fallPos;
        
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
