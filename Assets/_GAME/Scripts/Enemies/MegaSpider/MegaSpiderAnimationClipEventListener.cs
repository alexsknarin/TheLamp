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
        public event Action Bridge1Called;
        public event Action Bridge1Broken;
        public event Action Bridge2Called;
        public event Action Bridge2Broken;
        public event Action HangStartRequested;
        public event Action HangStopRequested;
        public event Action HangBreakRequested;
    
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

        public void CallStaticBridge1()
        {
            Bridge1Called?.Invoke();
        }
        
        public void BreakStaticBridge1()
        {
            Bridge1Broken?.Invoke();
        }
        
        public void CallStaticBridge2()
        {
            Bridge2Called?.Invoke();
        }
        
        public void BreakStaticBridge2()
        {
            Bridge2Broken?.Invoke();
        }
        
        public void RequestHangStart()
        {
            HangStartRequested?.Invoke();
        }
        
        public void RequestHangStop()
        {
            HangStopRequested?.Invoke();
        }
        
        public void RequestHangBreak()
        {
            HangBreakRequested?.Invoke();
        }
    }
}
