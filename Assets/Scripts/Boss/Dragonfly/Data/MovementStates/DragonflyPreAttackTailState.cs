using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyPreAttackTailState", menuName = "DragonflyStates/DragonflyPreAttackTailState")]
public class DragonflyPreAttackTailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.PreAttackTailL;
    [SerializeField] private float _duration = 0.35f;
    [SerializeField] private float _distance = -0.5f;
    [SerializeField] private AnimationCurve _curve;
    public override DragonflyMovementState State => _state;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            _state = DragonflyMovementState.PreAttackTailL;
        }
        else
        {
            _state = DragonflyMovementState.PreAttackTailR;
        }
        
        _stateData.PatrolRotator.SetRotationPhase(currentPosition);
        _stateData.PatrolRotator.Play(sideDirection);
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.PatrolTransform);
        _stateData.VisibleBodyTransform.localPosition = Vector3.zero;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.identity;
        
        _localTime = 0f;
        _phase = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        float zPos = Mathf.Lerp(0f, _distance, _curve.Evaluate(_phase));
        Vector3 pos = Vector3.zero;
        pos.z = zPos;
        _stateData.VisibleBodyTransform.localPosition = pos;
        
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
