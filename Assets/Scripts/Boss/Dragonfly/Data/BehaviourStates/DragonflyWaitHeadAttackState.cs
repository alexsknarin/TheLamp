using System;
using UnityEngine;

public class DragonflyWaitHeadAttackState : IState
{
    public event Action<DragonflyPatrolAttackMode> OnEnded;
    
    private Vector3 _targetPosition;
    private Transform _transform;
    private DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
    private FDragonflyMovement _movement;
    
    private bool _isLastPatrolDirectionSet = false;
    private int _lastPatrolDirection = 0;
    
    public DragonflyWaitHeadAttackState(Transform visibleBodyTransform, 
        DragonflyPatrolAttackPositionProvider patrolAttackPositionProvider, 
        FDragonflyMovement movement)
    {
        _transform = visibleBodyTransform;
        _patrolAttackPositionProvider = patrolAttackPositionProvider;
        _movement = movement;
    }
    
    public void OnEnter()
    {
        _targetPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackHeadPosition(_movement.MovementState);
        _isLastPatrolDirectionSet = false;
        _lastPatrolDirection = 0;
    }

    public void Tick()
    {
        Vector3 currentPosition = _transform.position;
        currentPosition.y = 0;
        currentPosition.Normalize();
        float distance = Vector3.Distance(currentPosition, _targetPosition);
        if (distance < 0.25f)
        {
            if (!_isLastPatrolDirectionSet)
            {
                _lastPatrolDirection = (int)Mathf.Sign((_targetPosition - currentPosition).normalized.x);
                _isLastPatrolDirectionSet = true;
            }
            else
            {
                float currentPatrolDirection = (int)Mathf.Sign((_targetPosition - currentPosition).normalized.x);
                if (currentPatrolDirection + _lastPatrolDirection == 0)
                {
                    OnEnded?.Invoke(DragonflyPatrolAttackMode.Head);
                }
            }
        }
    }

    public void OnExit()
    {
    }
}
