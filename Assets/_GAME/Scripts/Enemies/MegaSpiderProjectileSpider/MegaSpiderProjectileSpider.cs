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
        }

        public void Play()
        {
            Debug.Log("-- p");
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
