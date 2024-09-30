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
        _playablesContainer.AddClip(DragonflyState.Idle, _idleClip);
        _playablesContainer.AddClip(DragonflyState.EnterToPatrolL, _enterToPatrolLClip);  
        _playablesContainer.AddClip(DragonflyState.EnterToPatrolR, _enterToPatrolRClip);  
        _playablesContainer.AddClip(DragonflyState.CatchSpiderL, _catchSpiderLClip);
        _playablesContainer.AddClip(DragonflyState.EnterToHoverL, _enterToHoverLClip);
        _playablesContainer.AddClip(DragonflyState.EnterToHoverR, _enterToHoverRClip);
        _playablesContainer.AddClip(DragonflyState.CatchSpiderR, _catchSpiderRClip);
        _playablesContainer.AddClip(DragonflyState.MoveToPatrolL, _moveToPatrolLClip);
        _playablesContainer.AddClip(DragonflyState.MoveToPatrolR, _moveToPatrolRClip);
        _playablesContainer.AddClip(DragonflyState.ReturnTransitionLRBT, _returnTransitionLRBTClip);
        _playablesContainer.AddClip(DragonflyState.ReturnTransitionLRTB, _returnTransitionLRTBClip);
        _playablesContainer.AddClip(DragonflyState.ReturnTransitionRLBT, _returnTransitionRLBTClip);
        _playablesContainer.AddClip(DragonflyState.ReturnTransitionRLTB, _returnTransitionRLTBClip);
    }
}
