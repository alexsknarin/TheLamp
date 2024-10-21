using UnityEngine;

[CreateAssetMenu(fileName = "FDragonflyIdleState", menuName = "FDragonflyStates/FDragonflyIdleState")]
public class FDragonflyIdleState : ScriptableObject, IState
{
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
