using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyAttackHeadSuccessState", menuName = "FDragonflyMovementStates/FDragonflyAttackHeadSuccessState")]
public class FDragonflyAttackHeadSuccessState : ScriptableObject, IState
{
    [SerializeField] private float _duration = 0.85f;
    [SerializeField] private float _afterDelay = .6f;
    [SerializeField] private AnimationCurve _tyCurve;
    [SerializeField] private AnimationCurve _tzCurve;
    [SerializeField] private AnimationCurve _rxCurve;
    private float _localTime = 0f;
    private float _phase = 0f;
    private bool _isAfterDelay = false;

    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _fallPointTransform;

    public event Action Started;
    public event Action Ended;

    public void SetDependencies(Transform visibleBodyTransform, Transform fallPointTransform)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _fallPointTransform = fallPointTransform;
    }

    public void OnEnter()
    {
        _fallPointTransform.position = _visibleBodyTransform.position;
        _fallPointTransform.rotation = _visibleBodyTransform.rotation;
        _visibleBodyTransform.SetParent(_fallPointTransform);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
        _visibleBodyTransform.localScale = Vector3.one;
        
        _localTime = 0f;
        _phase = 0f;    
        _isAfterDelay = false;
        Started?.Invoke();
    }

    public void Tick()
    {
        Vector3 position = Vector3.zero;
        position.y = _tyCurve.Evaluate(_phase);
        position.z = _tzCurve.Evaluate(_phase);
        _visibleBodyTransform.localPosition = position;
        
        Vector3 rotation = _visibleBodyTransform.localEulerAngles;
        rotation.x = _rxCurve.Evaluate(_phase);
        _visibleBodyTransform.localEulerAngles = rotation;

        _localTime += Time.deltaTime;
        CheckForStateChange();
    }
    
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
    
    public void OnExit()
    {
    }
}
