using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "DragonflyBounceTailState", menuName = "DragonflyStates/DragonflyBounceTailState")]
public class DragonflyBounceTailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyState _state = DragonflyState.BounceTailL;
    [SerializeField] private float _rotationSpeed = 125f;
    [SerializeField] private float _duration = 0.15f;
    public override DragonflyState State => _state;
    
    private float _phase = 0f;
    private float _localTime = 0f;
    private float _sideDirection = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        if (sideDirection == 1)
        {
            _state = DragonflyState.BounceTailL;
        }
        else
        {
            _state = DragonflyState.BounceTailR;
        }
        _sideDirection = sideDirection;
        
        _stateData.PatrolRotator.SetRotationPhase(currentPosition);
        _stateData.PatrolRotator.Play(sideDirection);
        
        _stateData.VisibleBodyTransform.SetParent(_stateData.PatrolTransform, false);
        _localTime = 0f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 eulers = _stateData.VisibleBodyTransform.localRotation.eulerAngles;
        eulers.y += _rotationSpeed * Time.deltaTime * _sideDirection;
        _stateData.VisibleBodyTransform.localRotation = Quaternion.Euler(eulers);
        
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
