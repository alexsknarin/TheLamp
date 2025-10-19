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
        public event Action StaticBridge1Called;
        public event Action StaticBridge1Broken;
        public event Action StaticBridge2Called;
        public event Action StaticBridge2Broken;
        public event Action StaticBridge3Called;
        public event Action StaticBridge3Broken;
        public event Action HangStartRequested;
        public event Action HangStopRequested;
        public event Action HangBreakRequested;
        public event Action PreattackCalled;
        public event Action AttackCalled;
        public event Action ZigZag3Called;
        public event Action ProjectileDoubleUp2Called;
        public event Action ProjectileDoubleDown2Called;
        public event Action HangJumpCalled;
        public event Action HangJumpDiveCalled;
    
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
            StaticBridge1Called?.Invoke();
        }
        
        public void BreakStaticBridge1()
        {
            StaticBridge1Broken?.Invoke();
        }
        
        public void CallStaticBridge2()
        {
            StaticBridge2Called?.Invoke();
        }
        
        public void BreakStaticBridge2()
        {
            StaticBridge2Broken?.Invoke();
        }
        
        public void CallStaticBridge3()
        {
            StaticBridge3Called?.Invoke();
        }
        
        public void BreakStaticBridge3()
        {
            StaticBridge3Broken?.Invoke();
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

        public void CallPreattack()
        {
            PreattackCalled?.Invoke();
        }
        
        public void CallAttack()
        {
            AttackCalled?.Invoke();
        }
        
        public void CallZigZag3()
        {
            ZigZag3Called?.Invoke();
        }
        
        public void CallProjectileDoubleUp2()
        {
            ProjectileDoubleUp2Called?.Invoke();
        }
        
        public void CallProjectileDoubleDown2()
        {
            ProjectileDoubleDown2Called?.Invoke();
        }
        
        public void CallHangJump()
        {
            HangJumpCalled?.Invoke();
        }
        
        public void CallHangJumpDive()
        {
            HangJumpDiveCalled?.Invoke();
        }
    }
}
