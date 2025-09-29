using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderIdleState : EnemyMovementStateBase
    {
        private Transform _visibleBodyTransform;
        
        public MegaspiderIdleState(Transform visibleBodyTransform)
        {
            _visibleBodyTransform = visibleBodyTransform;
        }

        public override void Enter()
        {
            _visibleBodyTransform.position = Vector3.down * 10f;
        }

        public override void Tick() { }
    }
}
