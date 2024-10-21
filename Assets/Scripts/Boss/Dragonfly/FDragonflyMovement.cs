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
    [SerializeField] private FDragonflyAttackHeadState _attackHeadState;
    [SerializeField] private FDragonflyAttackHeadSuccessState _attackHeadSuccess; 
    [SerializeField] private FDragonflyAttackHoverState _attackHoverState;
    [SerializeField] private FDragonflyAttackTailFailStateL _attackTailFailL;
    [SerializeField] private FDragonflyAttackTailFailStateR _attackTailFailR;
    [SerializeField] private FDragonflyAttackTailStateL _attackTailStateL;
    [SerializeField] private FDragonflyAttackTailStateR _attackTailStateR;
    [SerializeField] private FDragonflyAttackTailSuccessStateL _attackTailSuccess;
    [SerializeField] private FDragonflyAttackTailSuccessStateR _attackTailSuccessR;
    [SerializeField] private FDragonflyBounceHeadState _bounceHeadState;
    [SerializeField] private FDragonflyBounceHoverState _bounceHoverState;
    [SerializeField] private FDragonflyBounceTailStateL _bounceTailStateL;
    [SerializeField] private FDragonflyBounceTailStateR _bounceTailStateR;
    [SerializeField] private FDragonflyCatchSpiderStateL _catchSpiderStateL;
    [SerializeField] private FDragonflyCatchSpiderStateR _catchSpiderStateR;
    [SerializeField] private FDragonflyEnterToHoverStateL _enterToHoverStateL;
    [SerializeField] private FDragonflyEnterToHoverStateR _enterToHoverStateR;
    [SerializeField] private FDragonflyEnterToPatrolStateL _enterToPatrolStateL;
    [SerializeField] private FDragonflyEnterToPatrolStateR _enterToPatrolStateR;
    [SerializeField] private FDragonflyFallHeadState _fallHeadState;
    
    
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
    
    



    
    
    
    public void PlayClip(DragonflyMovementState movementState)
    {
        // TODO: refactor to remove enum
        
        // AnimationClipPlayable clipPlayable = _playablesContainer.GetClip(movementState);
        // clipPlayable.SetTime(0);
        // clipPlayable.SetTime(0); // Unity Bug
        // _playableOutput.SetSourcePlayable(clipPlayable);
        // if (_playableGraph.IsValid())
        // {
        //     _playableGraph.Play();    
        // }
    }

    
    

}
