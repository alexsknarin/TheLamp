using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderAnimationClipEventListener : MonoBehaviour
    {
        public event Action AnimClipEnded;
        public event Action ProjectileAttack01Called;
        public event Action ProjectileAttack02Called;
        public event Action ProjectileResetCalled;
    
        public void ClipEnded()
        {
            AnimClipEnded?.Invoke();
        }
        
        public void ProjectileAttack01()
        {
            ProjectileAttack01Called?.Invoke();
        }
        
        public void ProjectileAttack02()
        {
            ProjectileAttack02Called?.Invoke();
        }

        public void ProjectilesReset()
        {
            ProjectileResetCalled?.Invoke();
        }
    }
}
