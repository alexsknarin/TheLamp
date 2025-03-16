using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyCatchSpiderStateR", menuName = "FDragonflyMovementStates/FDragonflyCatchSpiderStateR")]
public class FDragonflyCatchSpiderStateR: FDragonflyAnimBaseState
{
    public event Action Started;
    public override void OnEnter()
    {
        base.OnEnter();
        Started?.Invoke();
    }
}
