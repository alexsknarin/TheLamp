using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly
{
    public class DragonflyAnimationClipEventListener : MonoBehaviour
    {
        public event Action AnimClipEnded;
        public event Action SwarmCalled;
    
        public void ClipEnded()
        {
            AnimClipEnded?.Invoke();
        }
    
        public void SwarmCall()
        {
            SwarmCalled?.Invoke();
        }
    }
}
