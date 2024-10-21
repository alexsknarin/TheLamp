using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyAttackTailSuccessStateR", menuName = "FDragonflyMovementStates/FDragonflyAttackTailSuccessStateR")]
public class FDragonflyAttackTailSuccessStateR : ScriptableObject, IState
{
    [SerializeField] private float _duration = 1.8f;
    [SerializeField] private float _startSpeed = 55f;
    [SerializeField] private float _rotationSpeed = 110f;
    [SerializeField] private float _moveAcceleration = 1.9f;

    private float _localTime = 0f;
    private float _phase = 0f;
    private Vector3 _startDirection;
    private Vector3 _endDirection;
    private float _speed;
    
    Quaternion _startRotation;
    Quaternion _endRotation;
    
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
        _visibleBodyTransform.SetParent(_baseTransform);
        _startDirection = _visibleBodyTransform.forward.normalized;
        _endDirection = _startDirection;
        _endDirection.y = 0;
        _endDirection.z = 0;
        _endDirection.Normalize();
        _startRotation = _visibleBodyTransform.localRotation;
        _endRotation = Quaternion.LookRotation(_endDirection);
        _localTime = 0f;
        _phase = 0f;
        _speed = _startSpeed;
        _readyToSwitch = false;
    }
    
    public void Tick()
    {
        Vector3 dir = Vector3.Lerp(_startDirection, _endDirection, _phase);
        _visibleBodyTransform.position += dir * (_speed * Time.deltaTime);
        
        Quaternion rot = Quaternion.Lerp(_startRotation, _endRotation, _phase);
        _visibleBodyTransform.rotation = rot;
        
        _speed += _moveAcceleration * Time.deltaTime;
        
        _localTime += Time.deltaTime;
        CheckForStateChange();
    }

    public void OnExit()
    {
    }

    private void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1)
        {
            _readyToSwitch = true;
        }
    }
}
