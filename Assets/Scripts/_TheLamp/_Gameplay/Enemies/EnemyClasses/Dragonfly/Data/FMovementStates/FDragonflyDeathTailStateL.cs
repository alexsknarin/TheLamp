using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyDeathTailStateL", menuName = "FDragonflyMovementStates/FDragonflyDeathTailStateL")]
public class FDragonflyDeathTailStateL : ScriptableObject, IState
{
    [SerializeField] private float _duration = 2f;
    [SerializeField] private float _afterDelay = 1f;
    [SerializeField] private float _fallSpeed = 100f;
    [SerializeField] private float _rotationSpeed = 380f;
    [SerializeField] private float _moveAcceleration = 1.9f;
    private readonly int _sideDirection = 1;
    private float _localTime = 0f;
    private float _phase = 0f;
    private bool _isAfterDelay = false;
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _baseTransform;
    
    public event Action Started;
    public event Action Ended;

    public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _baseTransform = baseTransform;
    }
    
    public void OnEnter()
    {
        _visibleBodyTransform.SetParent(_baseTransform);
        _localTime = 0f;
        _phase = 0f;
        _isAfterDelay = false;
        Started?.Invoke();
    }
    
    public void Tick()
    {
        if (!_isAfterDelay)
        {
            Vector3 position = _visibleBodyTransform.position;
            float fallSpeed = _fallSpeed * Mathf.Pow(_phase, _moveAcceleration);
            position += Vector3.down * (fallSpeed * Time.deltaTime);
            _visibleBodyTransform.localPosition = position;
        
        
            Vector3 rotation = _visibleBodyTransform.localEulerAngles;
            rotation.y += _rotationSpeed * Time.deltaTime * _sideDirection;
            _visibleBodyTransform.localEulerAngles = rotation;    
        }
        
        _localTime += Time.deltaTime;
        CheckForStateChange();
    }

    public void OnExit() { }

    private void CheckForStateChange()
    {
        if (!_isAfterDelay)
        {
            _phase = _localTime / _duration;
            if (_phase > 1)
            {
                _isAfterDelay = true;
                _localTime = 0f;
            }    
        }
        else if (_isAfterDelay && _localTime > _afterDelay)
        {
            _isAfterDelay = false;
            Ended?.Invoke();
        }
    }
}
