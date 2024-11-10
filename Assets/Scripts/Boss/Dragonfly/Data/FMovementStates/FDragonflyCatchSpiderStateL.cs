using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyCatchSpiderStateL", menuName = "FDragonflyMovementStates/FDragonflyCatchSpiderStateL")]
public class FDragonflyCatchSpiderStateL : FDragonflyAnimBaseState
{
    public event Action OnStarted;
    
    public override void OnEnter()
    {
        base.OnEnter();
        OnStarted?.Invoke();
    }
}
