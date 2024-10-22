using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnHoverState", menuName = "FDragonflyMovementStates/FDragonflyReturnHoverState")]
public class FDragonflyReturnHoverState : ScriptableObject, IState
{
    [SerializeField] private float duration = 2f;
    [SerializeField] private float _zMaxDistance = -1.90932f;
    [SerializeField] private float _zMinDistance = 2f;
    [SerializeField] private float _farDuration = 1.1f;
    [SerializeField] private float _closeDuration = 1.7f;
    
    [SerializeField] private float _verticalDistance = 9.5f;
    [SerializeField] private float _horizontalDistance = 1f;
    [SerializeField] private AnimationCurve _verticalMoveCurve;
    
    private float _localTime = 0f;
    private float _phase = 0f;
    private float _endPosY = 0f;
    private float _normalizedDuration = 0f;
    private Vector3 _direction = Vector3.zero;
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _endPos = Vector3.zero;
    
    private bool _readyToSwitch = false;
    public bool ReadyToSwitch => _readyToSwitch;
    
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _baseTransform;
    
    public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _baseTransform = baseTransform;
    }
    
    public void OnEnter()
    {
        Vector3 currentPosition = _visibleBodyTransform.position;
        _localTime = 0f;
        _phase = 0f;
        
        _startPos = currentPosition;
        _endPosY = _startPos.y - _verticalDistance;
        
        Vector3 moveDirection = _endPos;
        moveDirection.y = 0;
        moveDirection.Normalize();
        _endPos = currentPosition + moveDirection * _horizontalDistance;
        _visibleBodyTransform.SetParent(_baseTransform);
        // Normalize duration by Z distance from camera
        float zPhase = Mathf.InverseLerp(_zMinDistance, _zMaxDistance, _endPos.z);
        _normalizedDuration = Mathf.Lerp(_farDuration, _closeDuration, zPhase);
        _readyToSwitch = false;
    }
    
    public void Tick()
    {
        Vector3 position = Vector3.Lerp(_startPos, _endPos, _phase);
        position.y = Mathf.Lerp(_startPos.y, _endPosY, _verticalMoveCurve.Evaluate(_phase));
        _visibleBodyTransform.position = position;
        
        _localTime += Time.deltaTime;
        CheckForStateChange();
    }

    public void OnExit()
    {
    }

    private void CheckForStateChange()
    {
        _phase = _localTime / _normalizedDuration;
        if (_phase > 1)
        {
            _readyToSwitch = true;
        }
    }
}
