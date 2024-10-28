using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyPreAttackTailStateL", menuName = "FDragonflyMovementStates/FDragonflyPreAttackTailStateL")]
public class FDragonflyPreAttackTailStateL : ScriptableObject, IState
{
    [SerializeField] private float _duration = 0.35f;
    [SerializeField] private float _distance = -0.5f;
    [SerializeField] private AnimationCurve _curve;
    
    public event Action OnStarted;
    
    private readonly int _sideDirection = 1;
    private float _localTime = 0f;
    private float _phase = 0f;
    
    private bool _readyToSwitch = false;
    public bool ReadyToSwitch => _readyToSwitch;
    
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _patrolTransform;
    private DragonflyPatrolRotator _patrolRotator;
    
    public void SetDependencies(Transform visibleBodyTransform, Transform patrolTransform, DragonflyPatrolRotator patrolRotator)
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
        
        _visibleBodyTransform.SetParent(_patrolTransform);
        _visibleBodyTransform.localPosition = Vector3.zero;
        _visibleBodyTransform.localRotation = Quaternion.identity;
        
        _localTime = 0f;
        _phase = 0f;
        _readyToSwitch = false;
        OnStarted?.Invoke();
    }

    public void Tick()
    {
        float zPos = Mathf.Lerp(0f, _distance, _curve.Evaluate(_phase));
        Vector3 pos = Vector3.zero;
        pos.z = zPos;
        _visibleBodyTransform.localPosition = pos;
        
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
