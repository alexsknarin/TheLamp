using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyMoveToHoverState", menuName = "DragonflyStates/DragonflyMoveToHoverState")]
public class DragonflyMoveToHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyMovementState _state = DragonflyMovementState.MoveToHover;
    [SerializeField] private Vector3[] _hoverPoints;
    [SerializeField] private float _zMaxDistance = -1.90932f;
    [SerializeField] private float _zMinDistance = 2f;
    [SerializeField] private float _farDuration = 1.1f;
    [SerializeField] private float _closeDuration = 1.7f;
    [SerializeField] private float _distance = 9.5f;
    [SerializeField] private AnimationCurve _moveCurve;
    
    private float _localTime = 0f;
    private int _currentPointIndex = 0;
    private float _phase = 0f;
    private float _normalizedDuration = 0f;
    
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _endPos = Vector3.zero;
    private Vector3 _startDirection = Vector3.zero;
    private Vector3 _endDirection = Vector3.zero;
    private Quaternion _startRotation = Quaternion.identity;
    private Quaternion _endRotation = Quaternion.identity;
    

    public override DragonflyMovementState State => _state;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _currentPointIndex = Random.Range(0, _hoverPoints.Length);
        _localTime = 0f;
        _phase = 0f;
        
        _endPos = _hoverPoints[_currentPointIndex];
        _startPos = _endPos;
        _startPos.y += -_distance;
        
        _endDirection = -(_endPos).normalized;
        _startDirection = _endDirection;
        _startDirection.y = 0;
        _startDirection.Normalize();
        _startRotation = Quaternion.LookRotation(_startDirection, Vector3.up);
        _endRotation = Quaternion.LookRotation(_endDirection, Vector3.up);

        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _stateData.VisibleBodyTransform.position = _endPos;
        _stateData.VisibleBodyTransform.rotation = Quaternion.LookRotation(_startDirection, Vector3.up);
        
        // Normalize duration by Z distance from camera
        float zPhase = Mathf.InverseLerp(_zMinDistance, _zMaxDistance, _endPos.z);
        _normalizedDuration = Mathf.Lerp(_farDuration, _closeDuration, zPhase);
    }
    
    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 position = Vector3.Lerp(_startPos, _endPos, _moveCurve.Evaluate(_phase));
        _stateData.VisibleBodyTransform.position = position;
        
        Quaternion rotation = Quaternion.Slerp(_startRotation, _endRotation, _phase);
        _stateData.VisibleBodyTransform.rotation = rotation;
        
        _localTime += Time.deltaTime;
    }
    
    public override void CheckForStateChange()
    {
        _phase = _localTime / _normalizedDuration;
        if (_phase > 1)
        {
            _stateData.Owner.SwitchState();
        }
    }
}
