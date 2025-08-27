using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider
{
    public class MegaSpiderAnimationClipEventListener : MonoBehaviour
    {
        public event Action AnimClipEnded;
        public event Action SwarmCalled; // TODO: remove if not needed
    
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
