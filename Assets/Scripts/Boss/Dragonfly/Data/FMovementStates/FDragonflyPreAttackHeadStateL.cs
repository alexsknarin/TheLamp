using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyPreAttackHeadStateL", menuName = "FDragonflyMovementStates/FDragonflyPreAttackHeadStateL")]
public class FDragonflyPreAttackHeadStateL : ScriptableObject, IState
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _duration = 0.4f;
    [SerializeField] private float _deccelerationPower = 2f;
    [SerializeField] private float _sideSpeed = 0.5f;
    private Vector3 _attackDirection;
    private readonly int _sideDirection = 1;
    private float _localTime = 0f;
    private float _phase = 0f;
    private Quaternion _startRotation;
    private Quaternion _endRotation;
    private bool _readyToSwitch = false;
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _baseTransform;
    public event Action Started;

    public bool ReadyToSwitch => _readyToSwitch;

    public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _baseTransform = baseTransform;
    }

    public void OnEnter()
    {
        _visibleBodyTransform.SetParent(_baseTransform);
        _attackDirection = -_visibleBodyTransform.position.normalized;
        _localTime = 0f;
        _phase = 0f;
        
        _startRotation = _visibleBodyTransform.rotation;
        _endRotation = Quaternion.LookRotation(_attackDirection, Vector3.up);
        _readyToSwitch = false;
        Started?.Invoke();
    }

    public void Tick()
    {
        // Attack direction movement
        float decceleration = Mathf.Pow(1-_phase, _deccelerationPower);
        _visibleBodyTransform.position += -_attackDirection * (_speed * decceleration * Time.deltaTime);

        // Side direction movement
        Vector3 sideVector = _visibleBodyTransform.right.normalized;
        _visibleBodyTransform.position += sideVector * (_sideSpeed * -_sideDirection * Time.deltaTime);
        
        // Rotate
        _visibleBodyTransform.rotation = Quaternion.Slerp(_startRotation, _endRotation, _phase);
        
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
