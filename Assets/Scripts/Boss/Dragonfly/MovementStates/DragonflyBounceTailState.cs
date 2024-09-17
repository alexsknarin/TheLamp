using UnityEngine;

public class DragonflyBounceTailState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.BounceTailL;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private float _rotationSpeed = 4f;
    [SerializeField] private float _duration = 0.15f;
    public override DragonflyStates State => _state;
    
    private float _phase = 0f;
    private float _localTime = 0f;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _patrolRotator.SetRotationPhase(currentPosition);
        _patrolRotator.Play();
        
        _visibleBodyTransform.SetParent(_patrolTransform, false);
        _localTime = 0f;
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 eulers = _visibleBodyTransform.localRotation.eulerAngles;
        eulers.y += _rotationSpeed * Time.deltaTime;
        _visibleBodyTransform.localRotation = Quaternion.Euler(eulers);
        
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 0f)
        {
            _owner.SwitchState();
        }
    }
    
}
