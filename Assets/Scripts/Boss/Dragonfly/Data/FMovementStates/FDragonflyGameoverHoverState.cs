using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyGameoverHoverState", menuName = "FDragonflyMovementStates/FDragonflyGameoverHoverState")]
public class FDragonflyGameoverHoverState : ScriptableObject, IState
{
    [SerializeField] private float _amplitude = 0.5f;
    [SerializeField] private float _frequency = 0.33f;
    [SerializeField] private float _speed = 5f;
    private float _localTime = 0f;
    private Vector3 _hoverPos = Vector3.zero;
    private Vector3 _direction;
    
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
        _hoverPos = _visibleBodyTransform.localPosition;
        _localTime = 0f;
        _direction = _visibleBodyTransform.position.normalized;
    }

    public void Tick()
    {
        Vector3 hoverPos = _visibleBodyTransform.localPosition;
        // Noise
        hoverPos = _hoverPos + _visibleBodyTransform.right * ((Mathf.PerlinNoise1D(_localTime * _frequency + 2) - 0.5f) * _amplitude);
        hoverPos.y = _hoverPos.y + (Mathf.PerlinNoise1D(_localTime * _frequency) - 0.5f) * _amplitude;
        
        // Move up
        _hoverPos += _direction * (_speed * Time.deltaTime);
        
        _visibleBodyTransform.localPosition = hoverPos;
        
        _localTime += Time.deltaTime;
    }

    public void OnExit()
    {
    }
}
