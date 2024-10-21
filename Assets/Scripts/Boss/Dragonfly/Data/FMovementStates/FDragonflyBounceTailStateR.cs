using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyBounceTailStateR", menuName = "FDragonflyMovementStates/FDragonflyBounceTailStateR")]
public class FDragonflyBounceTailStateR : ScriptableObject, IState
{
    [SerializeField] private float _rotationSpeed = 125f;
    [SerializeField] private float _duration = 0.15f;
    
    private float _phase = 0f;
    private float _localTime = 0f;
    private readonly int _sideDirection = -1;
    
    private bool _readyToSwitch = false;
    public bool ReadyToSwitch => _readyToSwitch;
    
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _patrolTransform;
    private DragonflyPatrolRotator _patrolRotator;

    public void SetDependencies(Transform visibleBodyTransform, Transform patrolTransform, 
        DragonflyPatrolRotator patrolRotator)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _patrolTransform = patrolTransform;
        _patrolRotator = patrolRotator;
    }
    
    public void OnEnter()
    {
        Vector3 currentPosition = _visibleBodyTransform.position;
        _patrolRotator.SetRotationPhase(currentPosition);
        _patrolRotator.Play(_sideDirection);
        
        _visibleBodyTransform.SetParent(_patrolTransform, false);
        _localTime = 0f;
        _phase = 0f;
        _readyToSwitch = false;
    }
    
    public void Tick()
    {
        Vector3 eulers = _visibleBodyTransform.localRotation.eulerAngles;
        eulers.y += _rotationSpeed * Time.deltaTime * _sideDirection;
        _visibleBodyTransform.localRotation = Quaternion.Euler(eulers);
        
        _localTime += Time.deltaTime;
        CheckForStateChange();
    }

    public void OnExit()
    {
    }

    private void CheckForStateChange()
    {
        _phase = _localTime / _duration;
        if (_phase > 1f)
        {
            _readyToSwitch = true;
        }
    }
}
