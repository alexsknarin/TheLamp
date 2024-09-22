using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAttackTailState", menuName = "DragonflyStates/DragonflyAttackTailState")]
public class DragonflyAttackTailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackTailL;
    [SerializeField] private float _duration = 0.6f;
    [SerializeField] private float _distance = 0.52f;
    [SerializeField] private AnimationCurve _tzCurve;
    [SerializeField] private AnimationCurve _ryCurve;
    [SerializeField] private AnimationCurve _rzCurve;
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    private float _startZPos = 0f;
    private int _sideDirection = 0;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            _state = DragonflyStates.AttackTailL;
        }
        else
        {
            _state = DragonflyStates.AttackTailR;
        }
        _sideDirection = sideDirection;
        
        _stateData.PatrolRotator.SetRotationPhase(currentPosition);
        _stateData.PatrolRotator.Play(sideDirection);
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.PatrolTransform);
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        _startZPos = _stateData.VisibleBodyTransform.localPosition.z;
        
        _localTime = 0f;
        _phase = 0f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        float zPos = Mathf.Lerp(_startZPos, _distance, _tzCurve.Evaluate(_phase));
        float ry = _ryCurve.Evaluate(_phase) * _sideDirection;
        float rz = _rzCurve.Evaluate(_phase) * _sideDirection;
        Vector3 pos = Vector3.zero;
        pos.z = zPos;
        _stateData.VisibleBodyTransform.localPosition = pos;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.Euler(0f, ry, rz);
        
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1f)
        {
            _phase = 1f;
            // _stateData.Owner.SwitchState();
        }
    }

}
