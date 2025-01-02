using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyFallHeadState", menuName = "FDragonflyMovementStates/FDragonflyFallHeadState")]
public class FDragonflyFallHeadState : ScriptableObject, IState
{
    [SerializeField] private float _duration = 1.1f;
    [SerializeField] private float _afterDelay = .6f;
    [SerializeField] private AnimationCurve _headFallRotateCurve;
    [SerializeField] private AnimationCurve _headFallFallDownCurve;
    [SerializeField] private float _bounceDistance = .25f;
    [SerializeField] private AnimationCurve _bounceCurve;

    public event Action Started;
    public event Action Ended;
    
    private float _headFallStartPosY = 0f;
    private float _localTime = 0f;
    private float _phase = 0f;
    private bool _isAfterDelay = false;
    
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _fallPointTransform;
    private float _bouncePosition = 0f;
    
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
        _headFallStartPosY = _fallPointTransform.position.y;
        
        _localTime = 0f;
        _phase = 0f;
        _isAfterDelay = false;
        _bouncePosition = 0f;
        Started?.Invoke();
    }

    public void Tick()
    {
        Vector3 fallEuler = Vector3.zero;
        fallEuler.x = _headFallRotateCurve.Evaluate(_phase);
        _visibleBodyTransform.localEulerAngles = fallEuler;
        
        Vector3 fallPos = _fallPointTransform.position;
        fallPos.y = _headFallStartPosY + _headFallFallDownCurve.Evaluate(_phase);
        _fallPointTransform.position = fallPos;
        
        _bouncePosition = -_bounceCurve.Evaluate(_phase) * _bounceDistance;
        Vector3 bouncePos = _visibleBodyTransform.localPosition;
        bouncePos.z = _bouncePosition;
        _visibleBodyTransform.localPosition = bouncePos;
        
        _localTime += Time.deltaTime;
        CheckForStateChange();
    }

    public void OnExit()
    {
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
}
