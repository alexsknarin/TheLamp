using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyIdleState", menuName = "FDragonflyMovementStates/FDragonflyIdleState")]
public class FDragonflyIdleState : ScriptableObject, IState
{
    // Dependencies
    private Transform _visibleBodyTransform;
    private Transform _baseTransform;
    private Vector3 _startPosition;
    
    public void SetDependencies(Transform visibleBodyTransform, Transform baseTransform)
    {
        _visibleBodyTransform = visibleBodyTransform;
        _baseTransform = baseTransform;
        _startPosition = new Vector3(0f, -8f, 0f);
    }
    
    public void OnEnter()
    {
        _visibleBodyTransform.SetParent(_baseTransform);
        _visibleBodyTransform.localPosition = _startPosition;
        _visibleBodyTransform.rotation = Quaternion.identity;
    }

    public void Tick() { }

    public void OnExit() { }
}
