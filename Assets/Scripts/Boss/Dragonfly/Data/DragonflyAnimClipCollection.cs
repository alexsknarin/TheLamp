using UnityEngine;

[CreateAssetMenu(fileName = "DragonflyAnimClipCollection", menuName = "DragonflyData/DragonflyAnimClipCollection")]
public class DragonflyAnimClipCollection : ScriptableObject
{
    [Header("Animation Clips")]
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _enterToPatrolLClip;
    [SerializeField] private AnimationClip _enterToPatrolRClip;
    [SerializeField] private AnimationClip _catchSpiderLClip;
    [SerializeField] private AnimationClip _catchSpiderRClip;
    [SerializeField] private AnimationClip _enterToHoverLClip;
    [SerializeField] private AnimationClip _enterToHoverRClip;
    [SerializeField] private AnimationClip _moveToPatrolLClip;
    [SerializeField] private AnimationClip _moveToPatrolRClip;
    [SerializeField] private AnimationClip _returnTransitionLRBTClip;
    [SerializeField] private AnimationClip _returnTransitionLRTBClip;
    [SerializeField] private AnimationClip _returnTransitionRLBTClip;
    [SerializeField] private AnimationClip _returnTransitionRLTBClip;

    private DragonflyPlayablesContainer _playablesContainer;
    
    public void Initialize(DragonflyPlayablesContainer playablesContainer)
    {
        _playablesContainer = playablesContainer;
        _playablesContainer.AddClip(DragonflyMovementState.Idle, _idleClip);
        _playablesContainer.AddClip(DragonflyMovementState.EnterToPatrolL, _enterToPatrolLClip);  
        _playablesContainer.AddClip(DragonflyMovementState.EnterToPatrolR, _enterToPatrolRClip);  
        _playablesContainer.AddClip(DragonflyMovementState.CatchSpiderL, _catchSpiderLClip);
        _playablesContainer.AddClip(DragonflyMovementState.EnterToHoverL, _enterToHoverLClip);
        _playablesContainer.AddClip(DragonflyMovementState.EnterToHoverR, _enterToHoverRClip);
        _playablesContainer.AddClip(DragonflyMovementState.CatchSpiderR, _catchSpiderRClip);
        _playablesContainer.AddClip(DragonflyMovementState.MoveToPatrolL, _moveToPatrolLClip);
        _playablesContainer.AddClip(DragonflyMovementState.MoveToPatrolR, _moveToPatrolRClip);
        _playablesContainer.AddClip(DragonflyMovementState.ReturnTransitionLRBT, _returnTransitionLRBTClip);
        _playablesContainer.AddClip(DragonflyMovementState.ReturnTransitionLRTB, _returnTransitionLRTBClip);
        _playablesContainer.AddClip(DragonflyMovementState.ReturnTransitionRLBT, _returnTransitionRLBTClip);
        _playablesContainer.AddClip(DragonflyMovementState.ReturnTransitionRLTB, _returnTransitionRLTBClip);
    }
}
