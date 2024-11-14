using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyReturnHoverState", menuName = "FDragonflyMovementStates/FDragonflyReturnHoverState")]
public class FDragonflyReturnHoverState : ScriptableObject, IState
{
    [SerializeField] private float _verticalDistance = 9.5f;
    [SerializeField] private float _horizontalDistance = 1f;
    [SerializeField] private float _bouncePhaseDuration = 1f;
    [SerializeField] private float _divePhaseDuration = 1f;
    [SerializeField] private float _maxBounceEulerX = 30f;
    [Header("Bounce")]
    [SerializeField] private AnimationCurve _bounceMoveCurve;
    [SerializeField] private AnimationCurve _bounceRotateCurve;
    [Header("Dive")]
    [SerializeField] private AnimationCurve _diveMoveCurve;
    [SerializeField] private AnimationCurve _diveRotateCurve;
    public event Action OnStartedEvent;
    public event Action OnEndedEvent;

    private float _localTime = 0f;
    private float _phase = 0f;
    
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _endPos = Vector3.zero;
    private float _startEulerX = 0f;
    private float _endEulerX = 0f;

    private bool _isBouncePhase = false;
    private bool _isDivePhase = false;
    
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
        _endPos = _startPos + _startPos.normalized * _horizontalDistance;
        
        _isBouncePhase = true;
        _isDivePhase = false;
        _startEulerX = _visibleBodyTransform.eulerAngles.x;
        
        
        _startEulerX = _visibleBodyTransform.eulerAngles.x;
        _endEulerX = _startEulerX + _maxBounceEulerX;
        

        OnStartedEvent?.Invoke();
    }
    
    public void Tick()
    {
        if (_isBouncePhase)
        {
            if (_phase > _bouncePhaseDuration)
            {
                _isBouncePhase = false;
                _isDivePhase = true;
                _localTime = 0f;
                _phase = 0f;
                _startPos = _visibleBodyTransform.position;
                _endPos = _startPos;
                _endPos.y -= _verticalDistance;
                
                _startEulerX = _visibleBodyTransform.eulerAngles.x;
                _endEulerX = _startEulerX + _maxBounceEulerX;
            }
            Vector3 position = Vector3.Lerp(_startPos, _endPos, _bounceMoveCurve.Evaluate(_phase));
            Vector3 euler = _visibleBodyTransform.eulerAngles;
            euler.x = Mathf.Lerp(_startEulerX, _endEulerX, _bounceRotateCurve.Evaluate(_phase));
            _visibleBodyTransform.position = position;
            _visibleBodyTransform.eulerAngles = euler;
        }
        if (_isDivePhase)
        {
            Vector3 position = Vector3.Lerp(_startPos, _endPos, _diveMoveCurve.Evaluate(_phase));
            Vector3 euler = _visibleBodyTransform.eulerAngles;
            euler.x = Mathf.Lerp(_startEulerX, _endEulerX, _diveRotateCurve.Evaluate(_phase));
            _visibleBodyTransform.position = position;
            _visibleBodyTransform.eulerAngles = euler;
        }
        _localTime += Time.deltaTime;
        CheckForStateChange();
    }

    public void OnExit()
    {
    }

    private void CheckForStateChange()
    {
        if (_isBouncePhase)
        {
            _phase = _localTime / _bouncePhaseDuration;    
        }
        else if (_isDivePhase)
        {
            _phase = _localTime / _divePhaseDuration;
        }
        
        if (_phase > _divePhaseDuration && _isDivePhase)
        {
            OnEndedEvent?.Invoke();
        }
    }
}
