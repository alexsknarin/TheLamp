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
    [SerializeField] private FDragonflyHoverState _hoverState;
    [SerializeField] private FDragonflyMoveToHoverState _moveToHoverState;
    [SerializeField] private FDragonflyPatrolStateL _patrolStateL;
    [SerializeField] private FDragonflyPatrolStateR _patrolStateR;
    [SerializeField] private FDragonflyPreAttackHeadStateL _preAttackHeadStateL;
    [SerializeField] private FDragonflyPreAttackHeadStateR _preAttackHeadStateR;
    [SerializeField] private FDragonflyPreAttackHoverState _preAttackHoverState;
    [SerializeField] private FDragonflyPreAttackTailStateL _preAttackTailStateL;
    [SerializeField] private FDragonflyPreAttackTailStateR _preAttackTailStateR;
    [SerializeField] private FDragonflyReturnHoverState _returnHoverState;
    [SerializeField] private FDragonflySpiderPatrolStateL _spiderPatrolStateL;
    [SerializeField] private FDragonflySpiderPatrolStateR _spiderPatrolStateR;
    [SerializeField] private FDragonflySpiderPreattackHeadTransitionStateL _spiderPreAttackHeadTransitionStateL;
    [SerializeField] private FDragonflySpiderPreattackHeadTransitionStateR _spiderPreAttackHeadTransitionStateR;
    [SerializeField] private FDragonflyMoveToPatrolStateL _moveToPatrolStateL;
    [SerializeField] private FDragonflyMoveToPatrolStateR _moveToPatrolStateR;
    [SerializeField] private FDragonflySpiderPushStateL _spiderPushStateL;
    [SerializeField] private FDragonflySpiderPushStateR _spiderPushStateR;
    [SerializeField] private FDragonflyReturnTransitionBTStateL _returnTransitionBTStateL;
    [SerializeField] private FDragonflyReturnTransitionBTStateR _returnTransitionBTStateR;
    [SerializeField] private FDragonflyReturnTransitionTBStateL _returnTransitionTBStateL;
    [SerializeField] private FDragonflyReturnTransitionTBStateR _returnTransitionTBStateR;
    [SerializeField] private FDragonflyDeathHeadState _deathHeadState;
    [SerializeField] private FDragonflyDeathTailStateL _deathTailStateL;
    [SerializeField] private FDragonflyDeathTailStateR _deathTailStateR;
    

    
    
    
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
