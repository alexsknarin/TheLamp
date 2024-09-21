using UnityEngine;

public class DragonflyMovementStateData
{
    private DragonflyMovement _owner;
    private Transform _visibleBodyTransform;
    private Transform _fallPointTransform;
    private DragonflyPatrolRotator _patrolRotator;
    private Transform _patrolTransform;
    private DragonflyPatrolRotator _spiderPatrolRotator;
    private Transform _spiderPatrolTransform;
    private Transform _animatedTransform;
   
    public DragonflyMovement Owner => _owner;
    public Transform VisibleBodyTransform => _visibleBodyTransform;
    public Transform FallPointTransform => _fallPointTransform;
    public DragonflyPatrolRotator PatrolRotator => _patrolRotator;
    public Transform PatrolTransform => _patrolTransform;
    public DragonflyPatrolRotator SpiderPatrolRotator => _spiderPatrolRotator;
    public Transform SpiderPatrolTransform => _spiderPatrolTransform;
    public Transform AnimatedTransform => _animatedTransform;
    

    public DragonflyMovementStateData(
        DragonflyMovement owner, 
        Transform visibleBodyTransform, 
        Transform fallPointTransform, 
        DragonflyPatrolRotator patrolRotator, 
        Transform patrolTransform, 
        DragonflyPatrolRotator spiderPatrolRotator, 
        Transform spiderPatrolTransform,
        Transform animatedTransform
        )
    {
        _owner = owner;
        _visibleBodyTransform = visibleBodyTransform;
        _fallPointTransform = fallPointTransform;
        _patrolRotator = patrolRotator;
        _patrolTransform = patrolTransform;
        _spiderPatrolRotator = spiderPatrolRotator;
        _spiderPatrolTransform = spiderPatrolTransform;
        _animatedTransform = animatedTransform;
    }
}
