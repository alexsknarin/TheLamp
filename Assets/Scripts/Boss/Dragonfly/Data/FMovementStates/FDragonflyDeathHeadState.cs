using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyDeathHeadState", menuName = "FDragonflyMovementStates/FDragonflyDeathHeadState")]
public class FDragonflyDeathHeadState : ScriptableObject, IState
{
    [SerializeField] private float _duration = 1.1f;
    [SerializeField] private float _afterDelay = .6f;
    [SerializeField] private AnimationCurve _headFallRotateCurve;
    [SerializeField] private AnimationCurve _headFallFallDownCurve;
   
    public event Action OnStarted;
    public event Action OnEnded;
    
    private float _headFallStartPosY = 0f;
    private float _localTime = 0f;
    private float _phase = 0f;
    private bool _isAfterDelay = false;
    
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _fallPointTransform;
    
    public void SetDependencies(Transform visibleBodyTransform, Transform fallPointTransform)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _fallPointTransform = fallPointTransform;
    }
    
    public  void OnEnter()
    {
        _fallPointTransform.position = _visibleBodyTransform.position;
        _fallPointTransform.rotation = _visibleBodyTransform.rotation;
        _visibleBodyTransform.SetParent(_fallPointTransform);
        _headFallStartPosY = _fallPointTransform.position.y;
        
        _localTime = 0f;
        _phase = 0f;
        _isAfterDelay = false;
        OnStarted?.Invoke();
    }

    public void Tick()
    {
        Vector3 fallEuler = Vector3.zero;
        fallEuler.x = _headFallRotateCurve.Evaluate(_phase);
        _visibleBodyTransform.localEulerAngles = fallEuler;
        
        Vector3 fallPos = _fallPointTransform.position;
        fallPos.y = _headFallStartPosY + _headFallFallDownCurve.Evaluate(_phase);
        _fallPointTransform.position = fallPos;
        
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
            OnEnded?.Invoke();
        }
    }
}
