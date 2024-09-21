using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyHoverState", menuName = "DragonflyStates/DragonflyHoverState")]
public class DragonflyHoverState : DragonflyMovementBaseState
{
    [SerializeField] private DragonflyStates _state = DragonflyStates.Hover;
    [SerializeField] private float _amplitude = 0.5f;
    [SerializeField] private float _frequency = 0.33f;
    public override DragonflyStates State => _state;
    private float _localTime = 0f;
    private Vector3 _hoverPos = Vector3.zero;
    
    public override void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
        _stateData.VisibleBodyTransform.SetParent(_stateData.Owner.transform);
        _hoverPos = _stateData.VisibleBodyTransform.localPosition;
        _localTime = 0f;
    }

    public override void ExecuteState(Vector3 currentPosition)
    {
        Vector3 hoverPos = _stateData.VisibleBodyTransform.localPosition;
        hoverPos = _hoverPos + _stateData.VisibleBodyTransform.right * ((Mathf.PerlinNoise1D(_localTime * _frequency + 2) - 0.5f) * _amplitude);
        hoverPos.y = _hoverPos.y + (Mathf.PerlinNoise1D(_localTime * _frequency) - 0.5f) * _amplitude;
        _stateData.VisibleBodyTransform.localPosition = hoverPos;
        
        _localTime += Time.deltaTime;
    }

}
