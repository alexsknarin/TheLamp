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
        private MegaSpiderEnterRState _enterRState;
        private MegaSpiderZigzagAttackLState _zigzagAttackLState;
        private MegaSpiderZigzagAttackRState _zigzagAttackRState;
        private MegaSpiderProjectileBottomLAttackState _projectileBottomLAttackState;
        private MegaSpiderProjectileBottomRAttackState _projectileBottomRAttackState;
        
    
        public void Initialize()
        {
            _stateFactory = new();
            _stateFactory.SetEnemyDependencies(_animator, _visibleBodyTransform, _animatedTransform);
        
            _enterLState = (MegaSpiderEnterLState)_stateFactory.Create(typeof(MegaSpiderEnterLState));
            _enterRState = (MegaSpiderEnterRState)_stateFactory.Create(typeof(MegaSpiderEnterRState));
            _zigzagAttackLState = (MegaSpiderZigzagAttackLState)_stateFactory.Create(typeof(MegaSpiderZigzagAttackLState));
            _zigzagAttackRState = (MegaSpiderZigzagAttackRState)_stateFactory.Create(typeof(MegaSpiderZigzagAttackRState));
            _projectileBottomLAttackState = (MegaSpiderProjectileBottomLAttackState)_stateFactory.Create(typeof(MegaSpiderProjectileBottomLAttackState));
            _projectileBottomRAttackState = (MegaSpiderProjectileBottomRAttackState)_stateFactory.Create(typeof(MegaSpiderProjectileBottomRAttackState));
 
        }

        public void Play()
        {
            Debug.Log("Play");
            _projectileBottomRAttackState.Enter();
        }
    }
}
