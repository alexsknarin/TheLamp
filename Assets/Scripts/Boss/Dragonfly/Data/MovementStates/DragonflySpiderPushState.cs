using UnityEngine;

[CreateAssetMenu(fileName = "DragonflySpiderPushState", menuName = "DragonflyStates/DragonflySpiderPushState")]
public class DragonflySpiderPushState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.SpiderPushL;
    [SerializeField] private float _distance = 0.5f;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private AnimationCurve _animCurve;
    public override DragonflyMovementState State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            _state = DragonflyMovementState.SpiderPushL;
        }
        else
        {
            _state = DragonflyMovementState.SpiderPushR;
        }
        
        _stateData.SpiderPatrolRotator.SetRotationPhase(currentPosition);
        _stateData.SpiderPatrolRotator.Play(sideDirection);
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.SpiderPatrolTransform, false);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        _localTime = 0f;
        _phase = 0f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 position = Vector3.zero;
        position.y = _animCurve.Evaluate(_phase) * _distance;
        _stateData.VisibleBodyTransform.localPosition = position;
        
        _localTime += Time.deltaTime;     
    }

    public override void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1)
        {
            _stateData.Owner.SwitchState();
        }
    }

    public override void ExitState()
    {
        _stateData.SpiderPatrolRotator.Stop();
    }
}
