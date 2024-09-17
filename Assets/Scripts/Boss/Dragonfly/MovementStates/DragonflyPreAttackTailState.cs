using UnityEngine;

public class DragonflyPreAttackTailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.PreAttackTailL;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private float _distance = -0.4f;
    [SerializeField] private AnimationCurve _curve;
    public override DragonflyStates State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _patrolRotator.SetRotationPhase(currentPosition);
        _patrolRotator.Play();
        
        _visibleBodyTransform.SetParent(_patrolTransform);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
        
        _localTime = 0f;
        _phase = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        float zPos = Mathf.Lerp(0f, _distance, _curve.Evaluate(_phase));
        Vector3 pos = Vector3.zero;
        pos.z = zPos;
        _visibleBodyTransform.localPosition = pos;
        
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
