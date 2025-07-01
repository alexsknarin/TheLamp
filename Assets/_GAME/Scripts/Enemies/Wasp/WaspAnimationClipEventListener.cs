using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp
{
    public class WaspAnimationClipEventListener : MonoBehaviour
    {
        public event Action ClipEnded;
        public event Action SpreadTgiggered;
        public event Action PreAttackStarted;
        public event Action AttackStarted;
        public event Action TrailReset;
        public event Action ScreenLeft;
        public event Action StartFlying;
    
        public void AnimationClipEnded()
        {
            ClipEnded?.Invoke();
        }
    
        public void TriggerSpread()
        {
            SpreadTgiggered?.Invoke();
        }
    
        public void ResetTrail()
        {
            TrailReset?.Invoke();
        }
    
        public void StartAttack()
        {
            AttackStarted?.Invoke();
        }
        
        public void HandleScreenLeft()
        {
            ScreenLeft?.Invoke();
        }
        
        public void HandleStartFlying()
        {
            StartFlying?.Invoke();
        }

        public void HandlePreAttackStart()
        {
            PreAttackStarted?.Invoke();
        }
    }
}
