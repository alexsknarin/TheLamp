using UnityEngine;

public class DragonflySpiderPreattackHeadTransitionState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovement _owner;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private DragonflyPatrolRotator _spiderPatrolRotator;
    [SerializeField] private Transform _spiderPatrolTransform;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private DragonflyStates _state = DragonflyStates.SpiderPreattackHeadTransitionStateL;
    public override DragonflyStates State => _state;
    
    private float _phase;
    private float _localTime;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _spiderPatrolRotator.SetRotationPhase(currentPosition);
        _spiderPatrolRotator.Play();
        
        _visibleBodyTransform.SetParent(_spiderPatrolTransform, false);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
        
        _localTime = 0;
        
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 localPosition = Vector3.Lerp(Vector3.zero, _patrolTransform.localPosition, _phase);
        Quaternion localRotation = Quaternion.Slerp(Quaternion.identity, _patrolTransform.localRotation, _phase);
        
        _visibleBodyTransform.localPosition = localPosition;
        _visibleBodyTransform.localRotation = localRotation;
        
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
