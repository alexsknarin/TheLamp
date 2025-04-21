using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAnimBaseState : IState
    {
        // Dependencies
        protected readonly Animator _animator;
        protected readonly int _clipHash;
        protected Transform _baseTransform;
        protected Vector3 _baseScaleL = Vector3.one;
        protected Vector3 _baseScaleR;
    
        public WaspAnimBaseState(Animator animator, int clipHash, Transform baseTransform)
        {
            _animator = animator;
            _clipHash = clipHash;
            _baseTransform = baseTransform;
            _baseScaleR = _baseScaleL;
            _baseScaleR.x = -1;
        }

        public virtual void Enter() { }

        public void Tick() { }

        public virtual void Exit() { }
    }
}
