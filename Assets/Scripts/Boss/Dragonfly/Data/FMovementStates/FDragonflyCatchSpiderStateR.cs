using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyCatchSpiderStateR", menuName = "FDragonflyMovementStates/FDragonflyCatchSpiderStateR")]
public class FDragonflyCatchSpiderStateR: FDragonflyAnimBaseState
{
    public event Action OnStarted;
    public override void OnEnter()
    {
        base.OnEnter();
        OnStarted?.Invoke();
    }
}
