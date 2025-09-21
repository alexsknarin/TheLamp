using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderPresentation : MonoBehaviour, IInitializable
    {
        [SerializeField] private Megaspider _megaspider;
        [SerializeField] private MegaspiderMovement _movement;
        [SerializeField] private MegaspiderAnimationClipEventListener _animationClipEvents;
        [SerializeField] private MegaspiderSpiderwebController _spiderwebController;
        
        public void Initialize()
        {
            _spiderwebController.Initialize();
        }
    }
}
