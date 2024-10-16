using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyDeathHeadState", menuName = "DragonflyStates/DragonflyDeathHeadState")]
public class DragonflyDeathHeadState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.DeathHead;
    [SerializeField] private float _duration = 1.1f;
    [SerializeField] private float _afterDelay = .6f;
    [SerializeField] private AnimationCurve _headFallRotateCurve;
    [SerializeField] private AnimationCurve _headFallFallDownCurve;

    public override DragonflyMovementState State => _state;
    
    private float _headFallStartPosY = 0f;
    private float _localTime = 0f;
    private float _phase = 0f;
    private bool _isAfterDelay = false;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.FallPointTransform.position = _stateData.VisibleBodyTransform.position;
        _stateData.FallPointTransform.rotation = _stateData.VisibleBodyTransform.rotation;
        _stateData.VisibleBodyTransform.SetParent(_stateData.FallPointTransform);
        _headFallStartPosY = _stateData.FallPointTransform.position.y;
        
        _localTime = 0f;
        _phase = 0f;
        _isAfterDelay = false;

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
