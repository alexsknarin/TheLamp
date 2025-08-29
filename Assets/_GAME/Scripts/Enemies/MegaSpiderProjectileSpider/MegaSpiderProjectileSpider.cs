using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpiderProjectileSpider
{
    public class MegaSpiderProjectileSpider : MonoBehaviour, IInitializable
    {
        [SerializeField] private MegaSpiderProjectileSpiderMovement _movement;
    
        public void Initialize()
        {
            _movement.Initialize();
            _movement.FallEnded += OnFallEnded;
        }

        private void OnDestroy()
        {
            _movement.FallEnded -= OnFallEnded;
        }

        private void OnFallEnded()
        {
            gameObject.SetActive(false);
        }

        public void Play()
        {
            _movement.Play();
        }

        public void Attack()
        {
            _movement.StartAttack();
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
