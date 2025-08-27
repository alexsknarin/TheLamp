using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderIdleState : EnemyMovementStateBase
    {
        private Transform _visibleBodyTransform;
        
        public MegaSpiderIdleState(Transform visibleBodyTransform)
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
