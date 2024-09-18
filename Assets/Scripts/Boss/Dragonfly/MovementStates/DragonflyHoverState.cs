using Unity.Mathematics;
using UnityEngine;

public class DragonflyHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.HoverL;
    [SerializeField] private float _amplitude = 1f;
    [SerializeField] private float _frequency = 1f;
    public override DragonflyStates State => _state;
    private float _localTime = 0f;
    private float _initialYPos = 0f;
    private Vector3 _hoverPos = Vector3.zero;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _visibleBodyTransform.SetParent(_owner.transform);
        
        _hoverPos = _visibleBodyTransform.localPosition;
        
        _localTime = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 hoverPos = _visibleBodyTransform.localPosition;
        hoverPos = _hoverPos + _visibleBodyTransform.right * ((Mathf.PerlinNoise1D(_localTime * _frequency + 2) - 0.5f) * _amplitude);
        hoverPos.y = _hoverPos.y + (Mathf.PerlinNoise1D(_localTime * _frequency) - 0.5f) * _amplitude;
        _visibleBodyTransform.localPosition = hoverPos;
        
        _localTime += Time.deltaTime;
    }

}
