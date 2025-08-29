using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider
{
    public class MegaSpider : MonoBehaviour
    {
        [SerializeField] private string _stateDebug;
        [Header("-- Attributes --")]
        [SerializeField] private int _maxHealth = 24;
        [SerializeField] private int _currentHealth;
        [Header("-- Movement --")]
        [SerializeField] private MegaSpiderMovement _movement;
        [Header("Swarm")]
        [SerializeField] private MegaSpiderSwarm _swarm;
        [SerializeField] private MegaSpiderAnimationClipEventListener _animationClipEvents;

        private void Awake()
        {
            // TODO:
            Initialize();
            _swarm.Initialize();
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Play();
            }
        }

        public void Initialize()
        {
            _movement.Initialize();
            
            _animationClipEvents.ProjectileResetCalled += _swarm.Reset;
            _animationClipEvents.ProjectileAttack01Called += _swarm.Attack01;
            _animationClipEvents.ProjectileAttack02Called += _swarm.Attack02;
        }

        private void OnDestroy()
        {
            _animationClipEvents.ProjectileResetCalled -= _swarm.Reset;
            _animationClipEvents.ProjectileAttack01Called -= _swarm.Attack01;
            _animationClipEvents.ProjectileAttack02Called -= _swarm.Attack02;
        }

        public void Play()
        {
            _movement.Play();
        }

        public void Collide()
        {
            _movement.Collide();
        }

        public void AttackZoneExit()
        {
            _movement.OnAttackZoneExit();
        }
    }
}
