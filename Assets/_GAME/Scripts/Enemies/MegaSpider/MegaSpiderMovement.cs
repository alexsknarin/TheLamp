using _GAME.Scripts.Enemies.MegaSpider.MovementStates;
using _GAME.Scripts.Factories;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider
{
    public class MegaSpiderMovement : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _visibleBodyTransform;
        [SerializeField] private Transform _animatedTransform;
        [SerializeField] private Animator _animator;
    
        // States
        private MegaSpiderMovementStateFactory _stateFactory;
        private MegaSpiderEnterLState _enterLState;
    
        public void Initialize()
        {
            _stateFactory = new();
            _stateFactory.SetEnemyDependencies(_animator, _visibleBodyTransform, _animatedTransform);
        
            _enterLState = (MegaSpiderEnterLState)_stateFactory.Create(typeof(MegaSpiderEnterLState));

            
        }

        public void Play()
        {
            Debug.Log("Play");
            _enterLState.Enter();
        }
    }
}
