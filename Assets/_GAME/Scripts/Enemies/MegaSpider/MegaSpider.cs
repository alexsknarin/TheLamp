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

        private void Awake()
        {
            // TODO:
            Initialize();
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Play();
            }
            
            if (Input.GetKeyDown(KeyCode.I))
            {
                _swarm.Initialize();
            }
        }

        public void Initialize()
        {
            _movement.Initialize();
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
