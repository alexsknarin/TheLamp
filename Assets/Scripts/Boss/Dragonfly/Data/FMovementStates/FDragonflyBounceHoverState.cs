using UnityEngine;


[CreateAssetMenu(fileName = "FDragonflyBounceHoverState", menuName = "FDragonflyMovementStates/FDragonflyBounceHoverState")]
public class FDragonflyBounceHoverState : ScriptableObject, IState
{
    [SerializeField] private float _speed = 4f;
    
    private Vector3 _attackDirection;
    
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
        _attackDirection = -_visibleBodyTransform.position.normalized;
    }

    public void Tick()
    {
        _visibleBodyTransform.position += -_attackDirection * (_speed * Time.deltaTime);
    }

    public void OnExit()
    {
    }
}
