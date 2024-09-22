using UnityEngine;

[CreateAssetMenu(fileName = "DragonflySpiderPreattackHeadTransitionState", menuName = "DragonflyStates/DragonflySpiderPreattackHeadTransitionState")]
public class DragonflySpiderPreattackHeadTransitionState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.SpiderPreattackHeadTransitionStateL;
    [SerializeField] private float _duration = 0.65f;
    public override DragonflyStates State => _state;
    private Transform _patrolTransformParent;
    
    private float _phase;
    private float _localTime;

    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _patrolTransformParent = _stateData.PatrolTransform.parent;
        _stateData.SpiderPatrolRotator.SetRotationPhase(currentPosition);
        _stateData.SpiderPatrolRotator.Play(sideDirection);
        _stateData.PatrolRotator.SetRotationPhase(currentPosition);
        _stateData.PatrolRotator.Stop();
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.SpiderPatrolTransform);
        _stateData.VisibleBodyTransform.position = _stateData.SpiderPatrolTransform.position;
        _stateData.VisibleBodyTransform.rotation = _stateData.SpiderPatrolTransform.rotation;
        
        _localTime = 0;
        
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        _stateData.PatrolRotator.SetRotationPhase(_stateData.SpiderPatrolTransform.position);
        _stateData.PatrolTransform.SetParent(_stateData.SpiderPatrolTransform);
        
        Vector3 position = Vector3.Lerp(Vector3.zero, _stateData.PatrolTransform.localPosition, _phase);
        Quaternion rotation = Quaternion.Slerp(Quaternion.identity, _stateData.PatrolTransform.localRotation, _phase);
        
        _stateData.PatrolTransform.SetParent(_patrolTransformParent);
        _stateData.PatrolTransform.localPosition = Vector3.zero;
        _stateData.PatrolTransform.localRotation = Quaternion.identity;
        _stateData.PatrolTransform.localScale = Vector3.one;
        
        _stateData.VisibleBodyTransform.localPosition = position;
        _stateData.VisibleBodyTransform.localRotation = rotation;
        
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
