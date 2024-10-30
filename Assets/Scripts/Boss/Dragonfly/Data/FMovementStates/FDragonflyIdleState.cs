using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyIdleState", menuName = "FDragonflyMovementStates/FDragonflyIdleState")]
public class FDragonflyIdleState : ScriptableObject, IState
{
    // Dependencies
    
    public void SetDependencies()
    {
    }
    
    public void OnEnter()
    {
        // TODO: set position to under the screen
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
    }
}
