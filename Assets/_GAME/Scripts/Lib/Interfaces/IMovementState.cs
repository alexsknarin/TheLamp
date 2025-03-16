using UnityEngine;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IMovementState
    {
        public void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection);
        public void ExecuteState(Vector3 currentPosition);
        public void CheckForStateChange();
        public void ExitState();
    }
}
