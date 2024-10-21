using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class FDragonflyMovement : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _visibleBodyTransform;
    [SerializeField] private Transform _animatedTransform;
    [SerializeField] private Transform _patrolTransform;
    [SerializeField] private Transform _spiderPatrolTransform;
    [SerializeField] private DragonflyPatrolRotator _patrolRotator;
    [SerializeField] private DragonflyPatrolRotator _spiderPatrolRotator;
    [SerializeField] private DragonflyAnimationClipEventHandler _animationClipEvents;
    [SerializeField] private Transform _fallPoint;
    
    [Header("Animation Clips")]
    [SerializeField] private DragonflyAnimClipCollection _animClipCollection;

    [Header("States")]  
    [SerializeField] private FDragonflyIdleState _idleState;
    
    

    // [SerializeField] private DragonflyMovementBaseState _attackHeadState;
    // [SerializeField] private DragonflyMovementBaseState _attackHeadSuccess;
    // [SerializeField] private DragonflyMovementBaseState _attackHoverState;
    // [SerializeField] private DragonflyMovementBaseState _attackTailFail;
    // [SerializeField] private DragonflyMovementBaseState _attackTailState;
    // [SerializeField] private DragonflyMovementBaseState _attackTailSuccess;
    // [SerializeField] private DragonflyMovementBaseState _bounceHeadState;
    // [SerializeField] private DragonflyMovementBaseState _bounceHoverState;
    // [SerializeField] private DragonflyMovementBaseState _bounceTailState;
    // [SerializeField] private DragonflyMovementBaseState _catchSpiderState;
    // [SerializeField] private DragonflyMovementBaseState _enterToHoverState;
    // [SerializeField] private DragonflyMovementBaseState _enterToPatrolState;
    // [SerializeField] private DragonflyMovementBaseState _fallHeadLState;
    // [SerializeField] private DragonflyMovementBaseState _hoverState;
    // [SerializeField] private DragonflyMovementBaseState _moveToHoverState;
    // [SerializeField] private DragonflyMovementBaseState _patrolState;
    // [SerializeField] private DragonflyMovementBaseState _preAttackHeadState;
    // [SerializeField] private DragonflyMovementBaseState _preAttackHoverState;
    // [SerializeField] private DragonflyMovementBaseState _preAttackTailState;
    // [SerializeField] private DragonflyMovementBaseState _returnHoverState;
    // [SerializeField] private DragonflyMovementBaseState _spiderPatrolState;
    // [SerializeField] private DragonflyMovementBaseState _spiderPreAttackHeadTransitionState;
    // [SerializeField] private DragonflyMovementBaseState _moveToPatrolState;
    // [SerializeField] private DragonflyMovementBaseState _spiderPushState;
    // [SerializeField] private DragonflyMovementBaseState _returnTransitionBTState; // Top - Bottom
    // [SerializeField] private DragonflyMovementBaseState _returnTransitionTBState; // Bottom - Top
    // [SerializeField] private DragonflyMovementBaseState _deathHeadState;
    // [SerializeField] private DragonflyMovementBaseState _deathTailState;
    
    





    
    

}
