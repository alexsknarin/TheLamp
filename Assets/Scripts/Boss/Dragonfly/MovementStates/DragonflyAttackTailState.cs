using UnityEngine;

public class DragonflyAttackTailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.AttackTailL;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _distance = 0.3f;
    [SerializeField] private AnimationCurve _tzCurve;
    [SerializeField] private AnimationCurve _ryCurve;
    [SerializeField] private AnimationCurve _rzCurve;
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    private float _startZPos = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _patrolRotator.SetRotationPhase(currentPosition);
        _patrolRotator.Play();
        
        _visibleBodyTransform.SetParent(_patrolTransform);
        _visibleBodyTransform.localRotation = Quaternion.identity;
        
        _startZPos = _visibleBodyTransform.localPosition.z;
        
        _localTime = 0f;
        _phase = 0f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        float zPos = Mathf.Lerp(_startZPos, _distance, _tzCurve.Evaluate(_phase));
        float ry = _ryCurve.Evaluate(_phase);
        float rz = _rzCurve.Evaluate(_phase);
        Vector3 pos = Vector3.zero;
        pos.z = zPos;
        _visibleBodyTransform.localPosition = pos;
        _visibleBodyTransform.localRotation = Quaternion.Euler(0f, ry, rz);
        
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
