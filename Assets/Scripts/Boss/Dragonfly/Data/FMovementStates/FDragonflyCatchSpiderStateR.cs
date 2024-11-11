using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyCatchSpiderStateR", menuName = "FDragonflyMovementStates/FDragonflyCatchSpiderStateR")]
public class FDragonflyCatchSpiderStateR: FDragonflyAnimBaseState
{
    public event Action OnStartedEvent;
    public override void OnEnter()
    {
        base.OnEnter();
        OnStartedEvent?.Invoke();
    }
}
