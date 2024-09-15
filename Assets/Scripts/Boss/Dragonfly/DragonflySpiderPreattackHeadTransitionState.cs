using UnityEngine;

public class DragonflySpiderPreattackHeadTransitionState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovement _owner;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private DragonflyPatrolRotator _spiderPatrolRotator;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private Transform _spiderPatrolTransform;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private float _duration = 2f;
    [SerializeField] private DragonflyStates _state = DragonflyStates.SpiderPreattackHeadTransitionStateL;
    public override DragonflyStates State => _state;
    private Transform _patrolTransformParent;
    
    private float _phase;
    private float _localTime;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _patrolTransformParent = _patrolTransform.parent;
        _spiderPatrolRotator.SetRotationPhase(currentPosition);
        _spiderPatrolRotator.Play();
        _patrolRotator.SetRotationPhase(currentPosition);
        _patrolRotator.Stop();
        
        _visibleBodyTransform.SetParent(_spiderPatrolTransform);
        _visibleBodyTransform.position = _spiderPatrolTransform.position;
        _visibleBodyTransform.rotation = _spiderPatrolTransform.rotation;
        
        _localTime = 0;
        
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        _patrolRotator.SetRotationPhase(_spiderPatrolTransform.position);
        _patrolTransform.SetParent(_spiderPatrolTransform);
        
        Vector3 position = Vector3.Lerp(Vector3.zero, _patrolTransform.localPosition, _phase);
        Quaternion rotation = Quaternion.Slerp(Quaternion.identity, _patrolTransform.localRotation, _phase);
        
        _patrolTransform.SetParent(_patrolTransformParent);
        _patrolTransform.localPosition = Vector3.zero;
        _patrolTransform.localRotation = Quaternion.identity;
        _patrolTransform.localScale = Vector3.one;
        
        _visibleBodyTransform.localPosition = position;
        _visibleBodyTransform.localRotation = rotation;
        
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
