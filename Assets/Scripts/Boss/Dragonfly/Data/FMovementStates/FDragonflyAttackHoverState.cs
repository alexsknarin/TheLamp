using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyAttackHoverState", menuName = "FDragonflyMovementStates/FDragonflyAttackHoverState")]
public class FDragonflyAttackHoverState : ScriptableObject, IState
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _acceleration = 0.75f;
    
    private float _attackAccelerationValue = 0;
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
        Vector3 currentPosition = _visibleBodyTransform.position;
        _visibleBodyTransform.SetParent(_baseTransform);
        // _attackDirection = -_stateData.VisibleBodyTransform.position.normalized;
        _attackAccelerationValue = 0;
        
        Vector3 sideGoal = currentPosition;
        sideGoal.z = 0;
        sideGoal.Normalize();
        sideGoal *= 0.85f;
        
        if (currentPosition.z > 0)
        {
            sideGoal *= 0.95f;
            sideGoal.y *= 0.75f;
        }
        else
        {
            sideGoal *= 0.85f;
        }
        _attackDirection = (sideGoal - currentPosition).normalized;
                
        Debug.DrawLine(currentPosition, sideGoal, Color.yellow, 5f);
    }

    public void Tick()
    {
        _visibleBodyTransform.position += _attackDirection * (_speed * Time.deltaTime + _attackAccelerationValue);
        _attackAccelerationValue += _acceleration * Time.deltaTime;
    }

    public void OnExit()
    {
    }
}
