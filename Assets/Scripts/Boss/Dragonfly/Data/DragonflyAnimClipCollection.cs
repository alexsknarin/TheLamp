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
        _playablesContainer.AddClip( typeof(FDragonflyIdleState), _idleClip);
        _playablesContainer.AddClip(typeof(FDragonflyEnterToPatrolStateL), _enterToPatrolLClip);  
        _playablesContainer.AddClip(typeof(FDragonflyEnterToPatrolStateR), _enterToPatrolRClip);  
        _playablesContainer.AddClip(typeof(FDragonflyCatchSpiderStateL), _catchSpiderLClip);
        _playablesContainer.AddClip(typeof(FDragonflyCatchSpiderStateR), _catchSpiderRClip);
        _playablesContainer.AddClip(typeof(FDragonflyEnterToHoverStateL), _enterToHoverLClip);
        _playablesContainer.AddClip(typeof(FDragonflyEnterToHoverStateR), _enterToHoverRClip);
        _playablesContainer.AddClip(typeof(FDragonflyMoveToPatrolStateL), _moveToPatrolLClip);
        _playablesContainer.AddClip(typeof(FDragonflyMoveToPatrolStateR), _moveToPatrolRClip);
        _playablesContainer.AddClip(typeof(FDragonflyReturnTransitionLRBTState), _returnTransitionLRBTClip);
        _playablesContainer.AddClip(typeof(FDragonflyReturnTransitionLRTBState), _returnTransitionLRTBClip);
        _playablesContainer.AddClip(typeof(FDragonflyReturnTransitionRLBTState), _returnTransitionRLBTClip);
        _playablesContainer.AddClip(typeof(FDragonflyReturnTransitionRLTBState), _returnTransitionRLTBClip);
    }
}
